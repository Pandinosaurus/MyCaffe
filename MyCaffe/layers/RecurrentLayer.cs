﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCaffe.basecode;
using MyCaffe.db.image;
using MyCaffe.common;
using MyCaffe.param;
using MyCaffe.fillers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MyCaffe.layers
{
    /// <summary>
    /// The RecurrentLayer is an abstract class for implementing recurrent behavior inside of an
    /// unrolled newtork.  This layer type cannot be instantiated -- instead,
    /// you should use one of teh implementations which defines the recurrent
    /// architecture, such as RNNLayer or LSTMLayer.
    /// This layer is initialized with the MyCaffe.param.RecurrentParameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RecurrentLayer<T> : Layer<T>
    {
        Layer<T> m_transposeData = null;
        Layer<T> m_transposeClip = null;
        Blob<T> m_blobBtmData = null;
        Blob<T> m_blobBtmClip = null;
        Blob<T> m_blobTopData = null;
        Blob<T> m_blobWork = null;
        BlobCollection<T> m_colBtm = null;
        BlobCollection<T> m_colTop = null;
        /// <summary>
        /// A Net to implement the Recurrent functionality.
        /// </summary>
        Net<T> m_unrolledNet = null;

        /// <summary>
        /// The number of independent streams to process simultaneously.
        /// </summary>
        protected int m_nN;

        /// <summary>
        /// The number of timesteps in the layer's input, and the number of 
        /// timesteps over which to backpropagate through time.
        /// </summary>
        protected int m_nT;

        /// <summary>
        /// Whether the layer has a 'static' input copies across all timesteps.
        /// </summary>
        protected bool m_bStaticInput;

        /// <summary>
        /// The last layer to run in the network.  (Any later layers are losses
        /// added to force the recurrent net to do backprop.)
        /// </summary>
        int m_nLastLayerIndex;

        /// <summary>
        /// Whether the layer's hidden state at the first and last timesteps
        /// are layer inputs.
        /// </summary>
        bool m_bExposeHiddenInput;

        /// <summary>
        /// Whether the layer's hidden state at the first and last timesteps
        /// are layer outputs.
        /// </summary>
        bool m_bExposeHiddenOutput;

        BlobCollection<T> m_colRecurInputBlobs = new BlobCollection<T>();
        BlobCollection<T> m_colRecurOutputBlobs = new BlobCollection<T>();
        BlobCollection<T> m_colOutputBlobs = new BlobCollection<T>();
        Blob<T> m_blobXInputBlob;       
        Blob<T> m_blobXStaticInputBlob; 
        Blob<T> m_blobContInputBlob;
        CancelEvent m_evtCancel;

        // cuDNN Specific Members
        long m_hCuDnn;
        int m_nInputSize = 1;
        int m_nHiddenSize;
        int m_nNumLayers;
        Blob<T> m_blobX;
        Blob<T> m_blobHx;
        Blob<T> m_blobCx;
        Blob<T> m_blobY;
        Blob<T> m_blobHy;
        Blob<T> m_blobCy;
        Blob<T> m_blobWts;
        long m_hXDesc;
        long m_hYDesc;
        long m_hHxDesc;
        long m_hCxDesc;
        long m_hHyDesc;
        long m_hCyDesc;
        long m_hDropoutDesc;
        long m_hDropoutStates;
        long m_hWeightDesc;
        long m_hRnnDesc;
        long m_hRnn8;
        long m_hWorkspace;
        ulong m_nWorkspaceSizeInBytes;
        bool m_bWorkspaceOwned = true;
        long m_hReserved;
        ulong m_nReservedSizeInBytes;
        bool m_bReservedOwned = true;
        RNN_MODE m_rnnMode;
        bool m_bUseTensors = false;
        List<int> m_rgShape = new List<int>(4);
        bool m_bWarningShown = false;
        bool m_bCudnnRnn8Supported = false;
        bool m_bUseCudnnRnn8 = false;
        List<int> m_rgShapeBtm0 = null;
        List<int> m_rgShapeBtm1 = null;

        /// <summary>
        /// The RecurrentLayer constructor.
        /// </summary>
        /// <param name="cuda">Specifies the CudaDnn connection to Cuda.</param>
        /// <param name="log">Specifies the Log for output.</param>
        /// <param name="p">Specifies the LayerParameter of type LSTM or RNN.
        /// </param>
        /// <param name="evtCancel">Specifies the CancelEvent used to cancel training operations.</param>
        public RecurrentLayer(CudaDnn<T> cuda, Log log, LayerParameter p, CancelEvent evtCancel)
            : base(cuda, log, p)
        {
            m_evtCancel = evtCancel;

            if (p.type == LayerParameter.LayerType.LSTM)
                m_rnnMode = RNN_MODE.LSTM;
            else
                m_rnnMode = RNN_MODE.RNN_RELU;
        }

        private void free_tensor(ref long h)
        {
            if (h != 0)
            {
                m_cuda.FreeTensorDesc(h);
                h = 0;
            }
        }

        /** @copydoc Layer::dispose */
        protected override void dispose()
        {
            base.dispose();

            if (m_unrolledNet != null)
            {
                m_unrolledNet.Dispose();
                m_unrolledNet = null;
            }

            dispose(ref m_blobHx);
            dispose(ref m_blobCx);
            dispose(ref m_blobHy);
            dispose(ref m_blobCy);
            dispose(ref m_blobWts);
            dispose(ref m_blobBtmData);
            dispose(ref m_blobBtmClip);
            dispose(ref m_blobTopData);
            dispose(ref m_blobWork);

            free_tensor(ref m_hHxDesc);
            free_tensor(ref m_hCxDesc);
            free_tensor(ref m_hHyDesc);
            free_tensor(ref m_hCyDesc);

            if (m_hWeightDesc != 0)
            {
                m_cuda.FreeFilterDesc(m_hWeightDesc);
                m_hWeightDesc = 0;
            }

            if (m_hRnnDesc != 0)
            {
                m_cuda.FreeRnnDesc(m_hRnnDesc);
                m_hRnnDesc = 0;
            }

            if (m_hDropoutDesc != 0)
            {
                m_cuda.FreeDropoutDesc(m_hDropoutDesc);
                m_hDropoutDesc = 0;
            }

            if (m_hDropoutStates != 0)
            {
                m_cuda.FreeMemory(m_hDropoutStates);
                m_hDropoutStates = 0;
            }

            if (m_hXDesc != 0)
            {
                m_cuda.FreeRnnDataDesc(m_hXDesc);
                m_hXDesc = 0;
            }

            if (m_hYDesc != 0)
            {
                m_cuda.FreeRnnDataDesc(m_hYDesc);
                m_hYDesc = 0;
            }

            if (m_hWorkspace != 0)
            {
                if (m_bWorkspaceOwned)
                    m_cuda.FreeMemory(m_hWorkspace);
                m_hWorkspace = 0;
            }

            if (m_hReserved != 0)
            {
                if (m_bReservedOwned)
                    m_cuda.FreeMemory(m_hReserved);
                m_hReserved = 0;
            }

            if (m_hCuDnn != 0)
            {
                m_cuda.FreeCuDNN(m_hCuDnn);
                m_hCuDnn = 0;
            }

            if (m_transposeData != null)
            {
                m_transposeData.Dispose();
                m_transposeData = null;
            }

            if (m_transposeClip != null)
            {
                m_transposeClip.Dispose();
                m_transposeClip = null;
            }
        }

        /// <summary>
        /// Set the OnDebug event on the unrolled net.
        /// </summary>
        /// <param name="fn">Specifies the event function to call when the OnDebug event fires.</param>
        public override void SetOnDebug(EventHandler<GetWorkBlobArgs<T>> fn)
        {
            base.SetOnDebug(fn);

            if (m_unrolledNet == null)
                return;

            foreach (Layer<T> layer in m_unrolledNet.layers)
            {
                layer.SetOnDebug(fn);
            }
        }

        /// <summary>
        /// Reset the OnDebug event, disabling it on the unrolled net.
        /// </summary>
        /// <param name="fn">Specifies the event function to call when the OnDebug event fires.</param>
        public override void ResetOnDebug(EventHandler<GetWorkBlobArgs<T>> fn)
        {
            base.ResetOnDebug(fn);

            if (m_unrolledNet == null)
                return;

            foreach (Layer<T> layer in m_unrolledNet.layers)
            {
                layer.ResetOnDebug(fn);
            }
        }

        private void addBtmTop(Blob<T> btm, Blob<T> top)
        {
            m_colBtm.Clear();
            m_colBtm.Add(btm);
            m_colTop.Clear();
            m_colTop.Add(top);
        }

        /// <summary>
        /// Setup the layer.
        /// </summary>
        /// <param name="colBottom">Specifies the collection of bottom (input) Blobs.</param>
        /// <param name="colTop">Specifies the collection of top (output) Blobs.</param>
        public override void LayerSetUp(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            Blob<T> blobBtm0 = colBottom[0];
            Blob<T> blobBtm1 = colBottom[1];

            m_bWarningShown = false;
            m_bCudnnRnn8Supported = m_cuda.IsRnn8Supported();
            if (m_bCudnnRnn8Supported && m_param.recurrent_param.use_cudnn_rnn8_if_supported)
                m_bUseCudnnRnn8 = true;

            m_blobBtmData = new Blob<T>(m_cuda, m_log);
            m_blobTopData = new Blob<T>(m_cuda, m_log);

            if (m_param.recurrent_param.batch_first)
            {
                m_colBtm = new BlobCollection<T>();
                m_colTop = new BlobCollection<T>();

                LayerParameter transpose = new LayerParameter(LayerParameter.LayerType.TRANSPOSE, m_param.name + ".trans");
                transpose.transpose_param.dim[0] = 1;
                transpose.transpose_param.dim[1] = 0;

                while (transpose.transpose_param.dim.Count > colBottom[0].num_axes)
                {
                    transpose.transpose_param.dim.RemoveAt(transpose.transpose_param.dim.Count - 1);
                }

                m_transposeData = Layer<T>.Create(m_cuda, m_log, convertLayerParam(transpose, m_param), null);

                addBtmTop(colBottom[0], m_blobBtmData);
                m_transposeData.Setup(m_colBtm, m_colTop);
                blobBtm0 = m_blobBtmData;

                while (transpose.transpose_param.dim.Count > colBottom[1].num_axes)
                {
                    transpose.transpose_param.dim.RemoveAt(transpose.transpose_param.dim.Count - 1);
                }

                m_transposeClip = Layer<T>.Create(m_cuda, m_log, convertLayerParam(transpose, m_param), null);
                m_blobBtmClip = new Blob<T>(m_cuda, m_log);

                addBtmTop(colBottom[1], m_blobBtmClip);
                m_transposeClip.Setup(m_colBtm, m_colTop);

                m_rgShape.Clear();
                m_rgShape.Add(m_blobBtmClip.num);
                m_rgShape.Add(m_blobBtmClip.channels);
                m_blobBtmClip.Reshape(m_rgShape);

                blobBtm1 = m_blobBtmClip;
            }

            m_log.CHECK_GE(blobBtm0.num_axes, 2, "Bottom[0] must have at least 2 axes -- (#timesteps, #streams, ...)");
            m_nT = blobBtm0.shape(0);
            m_nN = blobBtm0.shape(1);

            if (blobBtm0.num_axes > 2)
                m_nInputSize = colBottom[0].count(2);

            m_log.WriteLine("Initializing recurrent layer: assuming input batch contains " + m_nT.ToString() + " timesteps of " + m_nN.ToString() + " independent streams.");

            m_log.CHECK_EQ(blobBtm1.num_axes, 2, "Bottom[1] must have exactly 2 axes -- (#timesteps, #streams)");
            m_log.CHECK_EQ(m_nT, blobBtm1.shape(0), "The bottom[1].shape(0) must equal T = " + m_nT.ToString());
            m_log.CHECK_EQ(m_nN, blobBtm1.shape(1), "The bottom[1].shape(1) must equal N = " + m_nN.ToString());

            // If expose_hidden is set, we take as input and produce as output
            // the hidden state blobs at the first and last timesteps.
            m_bExposeHiddenInput = m_param.recurrent_param.expose_hidden_input;
            m_bExposeHiddenOutput = m_param.recurrent_param.expose_hidden_output;

            m_blobWork = new Blob<T>(m_cuda, m_log);

            if (m_param.recurrent_param.useCudnn())
                layerSetUpCuDnn(colBottom, colTop);
            else
                layerSetUpCaffe(colBottom, colTop);
        }

        private void layerSetUpCuDnn(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            if (m_bUseCudnnRnn8)
                layerSetupCudnnRnn8(colBottom, colTop);
            else
                layerSetupCudnnRnn(colBottom, colTop);
        }

        private void setupSharedWorkspaceAndReserved(ulong ulWsInBytes, ulong ulResInBytes)
        {
            m_nWorkspaceSizeInBytes = ulWsInBytes;
            m_bWorkspaceOwned = true;
            m_nReservedSizeInBytes = ulResInBytes;
            m_bReservedOwned = true;

            if (ulWsInBytes > 0)
                m_hWorkspace = m_cuda.AllocMemory((long)m_nWorkspaceSizeInBytes);
            if (ulResInBytes > 0)
                m_hReserved = m_cuda.AllocMemory((long)ulResInBytes);
        }

        private void layerSetupCudnnRnn8(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            try
            {
                if (m_param.recurrent_param.cudnn_enable_tensor_cores)
                    m_log.WriteLine("WARNING: RNN8 currently does not support Tensor Cores, disabling Tensor Cores for RNN8.");

                m_nHiddenSize = (int)m_param.recurrent_param.num_output;
                m_nNumLayers = (int)m_param.recurrent_param.num_layers;

                m_hCuDnn = m_cuda.CreateCuDNN();

                m_blobX = new Blob<T>(m_cuda, m_log);
                m_blobX.Name = m_param.name + " x";
                m_blobY = new Blob<T>(m_cuda, m_log);
                m_blobY.Name = m_param.name + " y";

                m_blobHx = new Blob<T>(m_cuda, m_log);
                m_blobHx.Name = m_param.name + " hx";
                m_blobCx = new Blob<T>(m_cuda, m_log);
                m_blobCx.Name = m_param.name + " cx";
                m_blobHy = new Blob<T>(m_cuda, m_log);
                m_blobHy.Name = m_param.name + " hy";
                m_blobCy = new Blob<T>(m_cuda, m_log);
                m_blobCy.Name = m_param.name + " cy";
                m_blobWts = new Blob<T>(m_cuda, m_log);
                m_blobWts.Name = m_param.name + " weights";

                blobs.Clear();
                blobs.Add(m_blobWts);

                int nBidirectionalScale = (m_param.recurrent_param.bidirectional) ? 2 : 1;

                m_hRnn8 = m_cuda.CreateRnn8();
                m_cuda.SetRnn8(m_hCuDnn,
                               m_hRnn8,
                               (m_phase == Phase.TRAIN) ? true : false,
                               RNN_DATALAYOUT.RNN_SEQ_MAJOR_PACKED,
                               m_rnnMode,
                               RNN_BIAS_MODE.RNN_DOUBLE_BIAS,
                               m_nT,
                               m_nN,
                               m_nInputSize,
                               m_nHiddenSize,
                               m_nHiddenSize * nBidirectionalScale, // Outputs
                               m_nHiddenSize,                       // Projection
                               m_nNumLayers,
                               (float)m_param.recurrent_param.dropout_ratio,
                               (ulong)m_param.recurrent_param.dropout_seed,
                               m_param.recurrent_param.bidirectional);

                Blob<T> blobBtm0 = colBottom[0];
                if (m_param.recurrent_param.batch_first)
                    blobBtm0 = m_blobBtmData;

                m_blobX.ReshapeLike(blobBtm0);
                m_blobX.ShareData(blobBtm0);
                m_blobX.ShareDiff(blobBtm0);
                m_log.CHECK_EQ(m_blobX.count(), m_nT * m_nN * m_nInputSize, "The input should be Sequence * Batch * InputSize in length.");

                int nDir = (m_param.recurrent_param.bidirectional) ? 2 : 1;
                m_blobHx.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, nDir);
                m_blobCx.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, nDir);

                m_blobY.Reshape(m_nT, m_nN, m_nHiddenSize, nDir);
                m_blobHy.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, nDir);
                m_blobCy.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, nDir);

                m_blobHx.SetData(0);
                m_blobCx.SetData(0);
                m_blobHy.SetData(0);
                m_blobCy.SetData(0);

                // Setup parameters - do this after the rnn descriptor is set
                // otherwise we will not know how many parameters we have to allocate.
                ulong szWtCount;
                ulong ulWorkspaceSizeInBytes;
                ulong ulReservedSizeInBytes;
                m_cuda.GetRnn8MemorySizes(m_hCuDnn, m_hRnn8, out szWtCount, out ulWorkspaceSizeInBytes, out ulReservedSizeInBytes);

                List<int> rgWtShape = new List<int>() { (int)szWtCount, 1, 1 };
                m_blobWts.Reshape(rgWtShape);

                // Setup the workspace and reserved memory.
                setupSharedWorkspaceAndReserved(ulWorkspaceSizeInBytes, ulReservedSizeInBytes);

                // Fill the weights.
                if (!shareParameter(m_blobWts, rgWtShape))
                {
                    double dfWtVal = 0;
                    double dfWtVal2 = 0;
                    RNN_FILLER_TYPE ftWt = RNN_FILLER_TYPE.RNN_CONSTANT_FILLER;
                    if (m_param.recurrent_param.weight_filler.type == "xavier")
                        ftWt = RNN_FILLER_TYPE.RNN_XAVIER_FILLER;
                    else if (m_param.recurrent_param.weight_filler.type == "gaussian")
                    {
                        dfWtVal = m_param.recurrent_param.weight_filler.mean;
                        dfWtVal2 = m_param.recurrent_param.weight_filler.std;
                        ftWt = RNN_FILLER_TYPE.RNN_GAUSSIAN_FILLER;
                    }
                    else if (m_param.recurrent_param.weight_filler.type == "constant")
                        dfWtVal = m_param.recurrent_param.weight_filler.value;
                    else
                        throw new Exception("Currently the RNN2 weights only support 'constant' and 'xavier' fillers.");

                    double dfBiasVal = 0;
                    double dfBiasVal2 = 0;
                    RNN_FILLER_TYPE ftBias = RNN_FILLER_TYPE.RNN_CONSTANT_FILLER;
                    if (m_param.recurrent_param.bias_filler.type == "xavier")
                        ftBias = RNN_FILLER_TYPE.RNN_XAVIER_FILLER;
                    else if (m_param.recurrent_param.bias_filler.type == "gaussian")
                    {
                        dfBiasVal = m_param.recurrent_param.bias_filler.mean;
                        dfBiasVal2 = m_param.recurrent_param.bias_filler.std;
                        ftBias = RNN_FILLER_TYPE.RNN_GAUSSIAN_FILLER;
                    }
                    else if (m_param.recurrent_param.bias_filler.type == "constant")
                        dfBiasVal = m_param.recurrent_param.bias_filler.value;
                    else
                        throw new Exception("Currently the RNN2 bias' only support 'constant' and 'xavier' fillers.");

                    m_cuda.InitializeRnn8Weights(m_hCuDnn, m_hRnn8, m_blobWts.mutable_gpu_data, ftWt, dfWtVal, dfWtVal2, ftBias, dfBiasVal, dfBiasVal2);
                }

                m_blobWts.SetDiff(0);
            }
            catch (Exception excpt)
            {
                throw excpt;
            }
            finally
            {
            }
        }

        private void layerSetupCudnnRnn(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            try
            {
                m_bUseTensors = m_param.recurrent_param.cudnn_enable_tensor_cores;
                m_nHiddenSize = (int)m_param.recurrent_param.num_output;
                m_nNumLayers = (int)m_param.recurrent_param.num_layers;

                m_hCuDnn = m_cuda.CreateCuDNN();

                m_blobX = new Blob<T>(m_cuda, m_log);
                m_blobX.Name = m_param.name + " x";
                m_blobY = new Blob<T>(m_cuda, m_log);
                m_blobY.Name = m_param.name + " y";

                m_blobHx = new Blob<T>(m_cuda, m_log);
                m_blobHx.Name = m_param.name + " hx";
                m_blobCx = new Blob<T>(m_cuda, m_log);
                m_blobCx.Name = m_param.name + " cx";
                m_blobHy = new Blob<T>(m_cuda, m_log);
                m_blobHy.Name = m_param.name + " hy";
                m_blobCy = new Blob<T>(m_cuda, m_log);
                m_blobCy.Name = m_param.name + " cy";
                m_blobWts = new Blob<T>(m_cuda, m_log);
                m_blobWts.Name = m_param.name + " weights";

                blobs.Clear();
                blobs.Add(m_blobWts);

                m_hXDesc = m_cuda.CreateRnnDataDesc();
                m_hYDesc = m_cuda.CreateRnnDataDesc();

                m_hHxDesc = m_cuda.CreateTensorDesc();
                m_hCxDesc = m_cuda.CreateTensorDesc();
                m_hHyDesc = m_cuda.CreateTensorDesc();
                m_hCyDesc = m_cuda.CreateTensorDesc();

                // Setup Rnn Descriptor
                m_hRnnDesc = m_cuda.CreateRnnDesc();
                m_hWeightDesc = m_cuda.CreateFilterDesc();
                m_hDropoutDesc = m_cuda.CreateDropoutDesc();


                //------------------------------------
                //  Start reshape here.
                //------------------------------------

                Blob<T> blobBtm0 = colBottom[0];
                if (m_param.recurrent_param.batch_first)
                    blobBtm0 = m_blobBtmData;

                m_blobX.ReshapeLike(blobBtm0);
                m_blobX.ShareData(blobBtm0);
                m_blobX.ShareDiff(blobBtm0);
                m_log.CHECK_EQ(m_blobX.count(), m_nT * m_nN * m_nInputSize, "The input should be Sequence * Batch * InputSize in length.");

                int nDir = (m_param.recurrent_param.bidirectional) ? 2 : 1;
                m_blobHx.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, nDir);
                m_blobCx.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, nDir);

                m_blobY.Reshape(m_nT, m_nN, m_nHiddenSize, nDir);
                m_blobHy.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, nDir);
                m_blobCy.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, nDir);

                m_blobHx.SetData(0);
                m_blobCx.SetData(0);
                m_blobHy.SetData(0);
                m_blobCy.SetData(0);

                // Set the input/output data descriptors
                m_cuda.SetRnnDataDesc(m_hXDesc, RNN_DATALAYOUT.RNN_SEQ_MAJOR_UNPACKED, m_nT, m_nN, m_nInputSize, false);
                m_cuda.SetRnnDataDesc(m_hYDesc, RNN_DATALAYOUT.RNN_SEQ_MAJOR_UNPACKED, m_nT, m_nN, m_nHiddenSize, m_param.recurrent_param.bidirectional);

                int[] rgDimA = new int[3];
                int[] rgStrideA = new int[3];

                rgDimA[0] = m_nNumLayers * ((m_param.recurrent_param.bidirectional) ? 2 : 1); 
                rgDimA[1] = m_nN; // mini batch.
                rgDimA[2] = m_nHiddenSize;

                rgStrideA[0] = rgDimA[2] * rgDimA[1];
                rgStrideA[1] = rgDimA[2];
                rgStrideA[2] = 1;

                m_cuda.SetTensorNdDesc(m_hHxDesc, rgDimA, rgStrideA);
                m_cuda.SetTensorNdDesc(m_hCxDesc, rgDimA, rgStrideA);
                m_cuda.SetTensorNdDesc(m_hHyDesc, rgDimA, rgStrideA);
                m_cuda.SetTensorNdDesc(m_hCyDesc, rgDimA, rgStrideA);

                // Setup the dropout descriptor.
                ulong ulStateCount;
                ulong ulReservedCount;
                m_cuda.GetDropoutInfo(m_hCuDnn, 0, out ulStateCount, out ulReservedCount);
                m_hDropoutStates = m_cuda.AllocMemory((long)ulStateCount);
                m_cuda.SetDropoutDesc(m_hCuDnn, m_hDropoutDesc, m_param.recurrent_param.dropout_ratio, m_hDropoutStates, m_param.recurrent_param.dropout_seed);

                // Setup the RNN descriptor.
                RNN_DIRECTION dir = (m_param.recurrent_param.bidirectional) ? RNN_DIRECTION.RNN_BIDIRECTIONAL : RNN_DIRECTION.RNN_UNIDIRECTIONAL;
                m_cuda.SetRnnDesc(m_hCuDnn, m_hRnnDesc, m_nHiddenSize, m_nNumLayers, m_hDropoutDesc, m_rnnMode, m_bUseTensors, dir);

                // Setup parameters - do this after the rnn descriptor is set
                // otherwise we will not know how many parameters we have to allocate.
                int nCount = m_cuda.GetRnnParamCount(m_hCuDnn, m_hRnnDesc, m_hXDesc);
                List<int> rgWtShape = new List<int>() { nCount, 1, 1 };
                m_blobWts.Reshape(rgWtShape);

                int[] rgDimW = new int[3];
                rgDimW[0] = nCount;
                rgDimW[1] = 1;
                rgDimW[2] = 1;

                m_cuda.SetFilterNdDesc(m_hWeightDesc, rgDimW);

                // Setup the workspace and reserved memory.
                ulong ulReservedSizeInBytes;
                ulong ulWorkspaceSizeInBytes = m_cuda.GetRnnWorkspaceCount(m_hCuDnn, m_hRnnDesc, m_hXDesc, out ulReservedSizeInBytes);

                // Setup the workspace and reserved memory.
                setupSharedWorkspaceAndReserved(ulWorkspaceSizeInBytes, ulReservedSizeInBytes);

                // Fill the weights.
                if (!shareParameter(m_blobWts, rgWtShape))
                {
                    int nNumLinearLayers = (m_rnnMode == RNN_MODE.LSTM) ? 8 : 2;
                    Filler<T> fillerWt = Filler<T>.Create(m_cuda, m_log, m_param.recurrent_param.weight_filler);
                    Filler<T> fillerBias = Filler<T>.Create(m_cuda, m_log, m_param.recurrent_param.bias_filler);
                    int nWtCount;
                    long hWt;
                    int nBiasCount;
                    long hBias;
                    int nBidir = (m_param.recurrent_param.bidirectional) ? 2 : 1;

                    for (int i = 0; i < m_nNumLayers * nBidir; i++)
                    {
                        for (int j = 0; j < nNumLinearLayers; j++)
                        {
                            m_cuda.GetRnnLinLayerParams(m_hCuDnn, m_hRnnDesc, i, m_hXDesc, m_hWeightDesc, m_blobWts.gpu_data, j, out nWtCount, out hWt, out nBiasCount, out hBias);

                            if (nWtCount % 2 != 0)
                            {
                                // Since, some fillers (gaussian) require an even number of items,
                                // we can temporarily use the all weight diff area and then copy 
                                // the non-even number of items into the layer weights.
                                fillerWt.Fill(nWtCount + 1, m_blobWts.mutable_gpu_diff);
                                m_cuda.copy(nWtCount, m_blobWts.mutable_gpu_diff, hWt);
                            }
                            else
                            {
                                fillerWt.Fill(nWtCount, hWt);
                            }

                            if (nBiasCount % 2 != 0)
                            {
                                // Since, some fillers (gaussian) require an even number of items,
                                // we can temporarily use the all weight diff area and then copy 
                                // the non-even number of items into the layer bias.
                                fillerBias.Fill(nBiasCount + 1, m_blobWts.mutable_gpu_diff);
                                m_cuda.copy(nBiasCount, m_blobWts.mutable_gpu_diff, hBias);
                            }
                            else
                            {
                                fillerBias.Fill(nBiasCount, hBias);
                            }

                            m_cuda.FreeMemoryPointer(hWt);
                            m_cuda.FreeMemoryPointer(hBias);
                        }
                    }
                }

                m_blobWts.SetDiff(0);
            }
            catch (Exception excpt)
            {
                throw excpt;
            }
            finally
            {
            }
        }

        private void layerSetUpCaffe(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            if (m_param.recurrent_param.auto_repeat_hidden_states_across_layers)
                m_log.FAIL("The 'auto_repeat_hidden_states_across_layers' setting is not supported in the Caffe implementation, use the cuDNN implementation instead.");

            Blob<T> blobBtm0 = colBottom[0];
            Blob<T> blobBtm1 = colBottom[1];
            if (m_param.recurrent_param.batch_first)
            {
                blobBtm0 = m_blobBtmData;
                blobBtm1 = m_blobBtmClip;
            }

            // Get (recurrent) input/output names.
            List<string> rgOutputNames = new List<string>();
            OutputBlobNames(rgOutputNames);

            List<string> rgRecurInputNames = new List<string>();
            RecurrentInputBlobNames(rgRecurInputNames);

            List<string> rgRecurOutputNames = new List<string>();
            RecurrentOutputBlobNames(rgRecurOutputNames);

            int nNumRecurBlobs = rgRecurInputNames.Count;
            m_log.CHECK_EQ(nNumRecurBlobs, rgRecurOutputNames.Count, "The number of recurrent input names must equal the number of recurrent output names.");

            // If provided, bottom[2] is a static input to the recurrent net.
            int nNumHiddenExposed = (m_bExposeHiddenOutput) ? nNumRecurBlobs : 0;
            int nBottomCount = (m_bExposeHiddenInput) ? 4 : 2;
            m_bStaticInput = (colBottom.Count > nBottomCount + nNumHiddenExposed) ? true : false;

            if (m_bStaticInput)
            {
                m_log.CHECK_GE(colBottom[2].num_axes, 1, "When static input is present, the bottom[2].num_axes must be >= 1");
                m_log.CHECK_EQ(m_nN, colBottom[2].shape(1), "When static input is present, the bottom[2].shape(1) must = N which is " + m_nN.ToString());

                // Original appears to be a bug, for ordering is T,N,x,x
                //m_log.CHECK_EQ(m_nN, colBottom[2].shape(0), "When static input is present, the bottom[2].shape(0) must = N which is " + m_nN.ToString());
            }

            // Create a NetParameter; setup the inputs that aren't unique to particular
            // recurrent architectures.
            NetParameter net_param = new NetParameter();

            LayerParameter input_layer = new LayerParameter(LayerParameter.LayerType.INPUT);
            input_layer.top.Add("x");
            BlobShape input_shape1 = new param.BlobShape();
            for (int i = 0; i < blobBtm0.num_axes; i++)
            {
                input_shape1.dim.Add(blobBtm0.shape(i));
            }
            input_layer.input_param.shape.Add(input_shape1);

            input_layer.top.Add("cont");
            BlobShape input_shape2 = new param.BlobShape();
            for (int i = 0; i < blobBtm1.num_axes; i++)
            {
                input_shape2.dim.Add(blobBtm1.shape(i));
            }
            input_layer.input_param.shape.Add(input_shape2);

            if (m_bStaticInput)
            {
                input_layer.top.Add("x_static");
                BlobShape input_shape3 = new BlobShape();
                for (int i = 0; i < colBottom[2].num_axes; i++)
                {
                    input_shape3.dim.Add(colBottom[2].shape(i));
                }
                input_layer.input_param.shape.Add(input_shape3);
            }

            net_param.layer.Add(input_layer);

            // Call the child's FillUnrolledNet implementation to specify the unrolled
            // recurrent architecture.
            FillUnrolledNet(net_param);

            // Prepend this layer's name to the names of each layer in the unrolled net.
            string strLayerName = m_param.name;
            if (strLayerName.Length > 0)
            {
                for (int i = 0; i < net_param.layer.Count; i++)
                {
                    LayerParameter layer = net_param.layer[i];
                    layer.name = strLayerName + "_" + layer.name;
                }
            }

            // Add 'pseudo-losses' to all outputs to force backpropagation.
            // (Setting force_backward is too agressive as we may not need to backprop to 
            // all inputs, e.g., the sequence continuation indicators.)
            List<string> rgPseudoLosses = new List<string>();
            for (int i = 0; i < rgOutputNames.Count; i++)
            {
                rgPseudoLosses.Add(rgOutputNames[i] + "_pseudoloss");
                LayerParameter layer = new LayerParameter(LayerParameter.LayerType.REDUCTION, rgPseudoLosses[i]);
                layer.bottom.Add(rgOutputNames[i]);
                layer.top.Add(rgPseudoLosses[i]);
                layer.loss_weight.Add(1.0);
                net_param.layer.Add(layer);
            }

            // Create the unrolled net.
            Net<T> sharedNet = null;
            if (m_param is LayerParameterExFull<T>)
            {
                RecurrentLayer<T> sharedLayer = ((LayerParameterExFull<T>)m_param).SharedLayer as RecurrentLayer<T>;
                if (sharedLayer != null)
                    sharedNet = sharedLayer.m_unrolledNet;
            }

            m_unrolledNet = new Net<T>(m_cuda, m_log, net_param, m_evtCancel, null, m_phase, null, sharedNet);
            m_unrolledNet.set_debug_info(m_param.recurrent_param.debug_info);

            // Setup pointers to the inputs.
            m_blobXInputBlob = m_unrolledNet.blob_by_name("x");
            m_blobContInputBlob = m_unrolledNet.blob_by_name("cont");

            if (m_bStaticInput)
                m_blobXStaticInputBlob = m_unrolledNet.blob_by_name("x_static");

            // Setup pointers to paired recurrent inputs/outputs.
            m_colRecurInputBlobs = new common.BlobCollection<T>();
            m_colRecurOutputBlobs = new common.BlobCollection<T>();

            for (int i = 0; i < nNumRecurBlobs; i++)
            {
                m_colRecurInputBlobs.Add(m_unrolledNet.blob_by_name(rgRecurInputNames[i]));
                m_colRecurOutputBlobs.Add(m_unrolledNet.blob_by_name(rgRecurOutputNames[i]));
            }

            // Setup pointers to outputs.
            m_log.CHECK_EQ(colTop.Count() - nNumHiddenExposed, rgOutputNames.Count, "OutputBlobNames must provide output blob name for each top.");
            m_colOutputBlobs = new common.BlobCollection<T>();
            for (int i = 0; i < rgOutputNames.Count; i++)
            {
                m_colOutputBlobs.Add(m_unrolledNet.blob_by_name(rgOutputNames[i]));
            }

            // We should have 2 inputs (x and cont), plus a number of recurrent inputs,
            // plus maybe a static input.
            int nStaticInput = (m_bStaticInput) ? 1 : 0;
            m_log.CHECK_EQ(2 + nNumRecurBlobs + nStaticInput, m_unrolledNet.input_blobs.Count, "The unrolled net input count should equal 2 + number of recurrent blobs (" + nNumRecurBlobs.ToString() + ") + static inputs (" + nStaticInput.ToString() + ")");

            // This layer's parameters are any parameters in the layers of the unrolled 
            // net.  We only want one copy of each parameter, so check that the parameter
            // is 'owned' by the layer, rather than shared with another.
            blobs.Clear();
            for (int i = 0; i < m_unrolledNet.parameters.Count; i++)
            {
                if (m_unrolledNet.param_owners[i] == -1)
                {
                    m_log.WriteLine("Adding parameter " + i.ToString() + ": " + m_unrolledNet.param_display_names[i]);
                    blobs.Add(m_unrolledNet.parameters[i]);
                }
            }

            // Check that param_propagate_down is set for all of the parameters in the 
            // unrolled net; set param_propagate_down to true in this layer.
            for (int i = 0; i < m_unrolledNet.layers.Count; i++)
            {
                for (int j = 0; j < m_unrolledNet.layers[i].blobs.Count; j++)
                {
                    m_log.CHECK(m_unrolledNet.layers[i].param_propagate_down(j), "param_propagate_down not set for layer " + i.ToString() + ", param " + j.ToString());
                }
            }
            m_rgbParamPropagateDown = new DictionaryMap<bool>(blobs.Count, true);

            // Set the diffs of recurrent outputs to 0 -- we can't backpropagate across
            // batches.
            for (int i = 0; i < m_colRecurOutputBlobs.Count; i++)
            {
                m_colRecurOutputBlobs[i].SetDiff(0);
            }

            // Check that the last output_names.count layers are the pseudo-losses;
            // set last_layer_index so that we don't actually run these layers.
            List<string> rgLayerNames = m_unrolledNet.layer_names;
            m_nLastLayerIndex = rgLayerNames.Count - 1 - rgPseudoLosses.Count;
            for (int i = m_nLastLayerIndex + 1, j = 0; i < rgLayerNames.Count; i++, j++)
            {
                m_log.CHECK(rgLayerNames[i] == rgPseudoLosses[j], "The last layer at idx " + i.ToString() + " should be the pseudo layer named " + rgPseudoLosses[j]);
            }

            // Setup shared Hx, Cx, Hy, Cy for transfers between tops and bottoms in 
            // forward and backward when specified - Sharing is used so that code
            // is similar between Caffe and CuDnn.
            Blob<T> blob;
            m_blobHx = new Blob<T>(m_cuda, m_log);
            m_blobHx.Name = m_param.name + " hx";
            m_blobHx.reshape_when_sharing = false;
            blob = m_colRecurInputBlobs[0];
            m_blobHx.ReshapeLike(blob);
            m_blobHx.ShareData(blob);
            m_blobHx.ShareDiff(blob);

            if (m_colRecurInputBlobs.Count > 1)
            {
                m_blobCx = new Blob<T>(m_cuda, m_log);
                m_blobCx.Name = m_param.name + " cx";
                m_blobCx.reshape_when_sharing = false;
                blob = m_colRecurInputBlobs[1];
                m_blobCx.ReshapeLike(blob);
                m_blobCx.ShareData(blob);
                m_blobCx.ShareDiff(blob);
            }

            m_blobHy = new Blob<T>(m_cuda, m_log);
            m_blobHy.Name = m_param.name + " hy";
            m_blobHy.reshape_when_sharing = false;
            blob = m_colRecurOutputBlobs[0];
            m_blobHy.ReshapeLike(blob);
            m_blobHy.ShareData(blob);
            m_blobHy.ShareDiff(blob);

            if (m_colRecurOutputBlobs.Count > 1)
            {
                m_blobCy = new Blob<T>(m_cuda, m_log);
                m_blobCy.Name = m_param.name + " cy";
                m_blobCy.reshape_when_sharing = false;
                blob = m_colRecurOutputBlobs[1];
                m_blobCy.ReshapeLike(blob);
                m_blobCy.ShareData(blob);
                m_blobCy.ShareDiff(blob);
            }
        }

        /// <summary>
        /// Returns true if a reshape is needed.
        /// </summary>
        /// <param name="colBottom">Specifies the bottom blobs.</param>
        /// <param name="colTop">Specifies the top blobs.</param>
        /// <param name="bReset">Specifies to reset the reshape needed.</param>
        /// <returns>Returns true when a reshape is needed.</returns>
        protected override bool reshapeNeeded(BlobCollection<T> colBottom, BlobCollection<T> colTop, bool bReset = true)
        {
            if (!bReset && m_rgShapeBtm0 != null && m_rgShapeBtm1 != null && colBottom[0].CompareShape(m_rgShapeBtm0) && colBottom[1].CompareShape(m_rgShapeBtm1))
                return false;

            m_rgShapeBtm0 = Utility.Clone<int>(colBottom[0].shape());
            m_rgShapeBtm1 = Utility.Clone<int>(colBottom[1].shape());

            return true;
        }

        /// <summary>
        /// Reshape the bottom (input) and top (output) blobs.
        /// </summary>
        /// <param name="colBottom">Specifies the collection of bottom (input) Blobs.</param>
        /// <param name="colTop">Specifies the collection of top (output) Blobs.</param>
        public override void Reshape(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            if (!reshapeNeeded(colBottom, colTop, false))
                return;

            Blob<T> blobBtm0 = colBottom[0];
            Blob<T> blobBtm1 = colBottom[1];

            if (m_param.recurrent_param.batch_first)
            {
                addBtmTop(colBottom[0], m_blobBtmData);
                m_transposeData.Reshape(m_colBtm, m_colTop);
                blobBtm0 = m_blobBtmData;

                addBtmTop(colBottom[1], m_blobBtmClip);
                m_transposeClip.Reshape(m_colBtm, m_colTop);

                m_rgShape.Clear();
                m_rgShape.Add(m_blobBtmClip.num);
                m_rgShape.Add(m_blobBtmClip.channels);
                m_blobBtmClip.Reshape(m_rgShape);

                blobBtm1 = m_blobBtmClip;
            }

            m_log.CHECK_GE(blobBtm0.num_axes, 2, "bottom[0] must have at least 2 axes -- (#timesteps, #streams, ...)");
            m_log.CHECK_EQ(m_nT, blobBtm0.shape(0), "input number of timesteps changed.");
            m_nN = blobBtm0.shape(1);
            m_log.CHECK_EQ(blobBtm1.num_axes, 2, "bottom[1] must have exactly 2 axes -- (#timesteps, #streams)");
            m_log.CHECK_EQ(m_nT, blobBtm1.shape(0), "bottom[1].shape(0) should equal the timesteps T (" + m_nT.ToString() + ")");
            m_log.CHECK_EQ(m_nN, blobBtm1.shape(1), "bottom[1].shape(1) should equal the streams N (" + m_nN + ")");

            if (m_param.recurrent_param.useCudnn())
                reshapeCuDnn(colBottom, colTop);
            else
                reshapeCaffe(colBottom, colTop);

            if (m_param.recurrent_param.batch_first)
            {
                addBtmTop(m_blobTopData, colTop[0]);
                m_transposeData.Reshape(m_colBtm, m_colTop);
            }
        }

        private void reshapeCuDnn(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            if (m_bUseCudnnRnn8)
                reshapeCudnnRnn8(colBottom, colTop);
            else
                reshapeCudnnRnn(colBottom, colTop);
        }

        private void reshapeCudnnRnn8(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            Blob<T> blobBtm0 = colBottom[0];
            Blob<T> blobTop0 = colTop[0];

            if (m_param.recurrent_param.batch_first)
            {
                blobBtm0 = m_blobBtmData;
                blobTop0 = m_blobTopData;
            }

            m_blobX.ReshapeLike(blobBtm0);
            m_blobX.ShareData(blobBtm0);
            m_blobX.ShareDiff(blobBtm0);
            m_log.CHECK_EQ(m_blobX.count(), m_nT * m_nN * m_nInputSize, "The input should be Sequence * Batch * InputSize in length.");

            m_blobHx.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, 1);
            m_blobHx.SetData(0);
            m_blobCx.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, 1);
            m_blobCx.SetData(0);

            m_blobY.Reshape(m_nT, m_nN, m_nHiddenSize, 1);
            m_blobHy.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, 1);
            m_blobCy.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, 1);

            blobTop0.ReshapeLike(m_blobY);
            blobTop0.ShareData(m_blobY);
            blobTop0.ShareDiff(m_blobY);

            if (m_param.recurrent_param.expose_hidden_output)
            {
                colTop[1].ReshapeLike(m_blobHy);
                colTop[1].ShareData(m_blobHy);
                colTop[1].ShareDiff(m_blobHy);

                colTop[2].ReshapeLike(m_blobCy);
                colTop[2].ShareData(m_blobCy);
                colTop[2].ShareDiff(m_blobCy);
            }

            int nBidirectionalScale = (m_param.recurrent_param.bidirectional) ? 2 : 1;
            m_cuda.SetRnn8(m_hCuDnn,
                           m_hRnn8,
                           (m_phase == Phase.TRAIN) ? true : false,
                           RNN_DATALAYOUT.RNN_SEQ_MAJOR_PACKED,
                           m_rnnMode,
                           RNN_BIAS_MODE.RNN_DOUBLE_BIAS,
                           m_nT,
                           m_nN,
                           m_nInputSize,
                           m_nHiddenSize,
                           m_nHiddenSize * nBidirectionalScale, // Outputs
                           m_nHiddenSize,                       // Projection
                           m_nNumLayers,
                           (float)m_param.recurrent_param.dropout_ratio,
                           (ulong)m_param.recurrent_param.dropout_seed,
                           m_param.recurrent_param.bidirectional);
        }

        private void reshapeCudnnRnn(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            Blob<T> blobBtm0 = colBottom[0];
            Blob<T> blobTop0 = colTop[0];

            if (m_param.recurrent_param.batch_first)
            {
                blobBtm0 = m_blobBtmData;
                blobTop0 = m_blobTopData;
            }

            m_blobX.ReshapeLike(blobBtm0);
            m_blobX.ShareData(blobBtm0);
            m_blobX.ShareDiff(blobBtm0);
            m_log.CHECK_EQ(m_blobX.count(), m_nT * m_nN * m_nInputSize, "The input should be Sequence * Batch * InputSize in length.");

            m_blobHx.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, 1);
            m_blobCx.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, 1);

            m_blobY.Reshape(m_nT, m_nN, m_nHiddenSize, 1);
            m_blobHy.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, 1);
            m_blobCy.Reshape(m_nNumLayers, m_nN, m_nHiddenSize, 1);

            blobTop0.ReshapeLike(m_blobY);
            blobTop0.ShareData(m_blobY);
            blobTop0.ShareDiff(m_blobY);

            if (m_param.recurrent_param.expose_hidden_output)
            {
                colTop[1].ReshapeLike(m_blobHy);
                colTop[1].ShareData(m_blobHy);
                colTop[1].ShareDiff(m_blobHy);

                colTop[2].ReshapeLike(m_blobCy);
                colTop[2].ShareData(m_blobCy);
                colTop[2].ShareDiff(m_blobCy);
            }

            m_cuda.SetRnnDataDesc(m_hXDesc, RNN_DATALAYOUT.RNN_SEQ_MAJOR_UNPACKED, m_nT, m_nN, m_nInputSize, false);
            m_cuda.SetRnnDataDesc(m_hYDesc, RNN_DATALAYOUT.RNN_SEQ_MAJOR_UNPACKED, m_nT, m_nN, m_nHiddenSize, m_param.recurrent_param.bidirectional);
        }

        private void reshapeCaffe(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            Blob<T> blobBtm0 = colBottom[0];
            Blob<T> blobBtm1 = colBottom[1];
            Blob<T> blobTop0 = colTop[0];

            if (m_param.recurrent_param.batch_first)
            {
                blobBtm0 = m_blobBtmData;
                blobBtm1 = m_blobBtmClip;
                blobTop0 = m_blobTopData;
            }

            m_blobXInputBlob.ReshapeLike(blobBtm0);
            List<int> rgContShape = blobBtm1.shape();
            m_blobContInputBlob.Reshape(rgContShape);

            if (m_bStaticInput)
                m_blobXStaticInputBlob.ReshapeLike(colBottom[2]);

            List<BlobShape> rgRecurInputShapes = new List<BlobShape>();
            RecurrentInputShapes(rgRecurInputShapes);
            m_log.CHECK_EQ(rgRecurInputShapes.Count, m_colRecurInputBlobs.Count, "The number of recurrent input shapes must equal the number of recurrent input blobs!");

            for (int i = 0; i < rgRecurInputShapes.Count; i++)
            {
                m_colRecurInputBlobs[i].Reshape(rgRecurInputShapes[i]);
            }

            m_unrolledNet.Reshape();

            m_blobXInputBlob.ShareData(blobBtm0);
            m_blobXInputBlob.ShareDiff(blobBtm0);
            m_blobContInputBlob.ShareData(blobBtm1);

            int nStaticInput = 0;

            if (m_bStaticInput)
            {
                nStaticInput = 1;
                m_blobXStaticInputBlob.ShareData(colBottom[2]);
                m_blobXStaticInputBlob.ShareDiff(colBottom[2]);
            }

            if (m_bExposeHiddenInput)
            {
                int nBottomOffset = 2 + nStaticInput;
                for (int i = nBottomOffset, j = 0; i < colBottom.Count; i++, j++)
                {
                    m_log.CHECK(Utility.Compare<int>(m_colRecurInputBlobs[j].shape(), colBottom[i].shape()), "Shape mismatch - recur_input_blobs_[" + j.ToString() + "]: '" + m_colRecurInputBlobs[j].shape_string + "' vs. bottom[" + i.ToString() + "]: '" + colBottom[i].shape_string + "'");
                    m_colRecurInputBlobs[j].ShareData(colBottom[i]);
                }
            }

            for (int i = 0; i < m_colOutputBlobs.Count; i++)
            {
                if (i == 0)
                {
                    blobTop0.ReshapeLike(m_colOutputBlobs[i]);
                    blobTop0.ShareData(m_colOutputBlobs[i]);
                    blobTop0.ShareDiff(m_colOutputBlobs[i]);
                }
                else
                {
                    colTop[i].ReshapeLike(m_colOutputBlobs[i]);
                    colTop[i].ShareData(m_colOutputBlobs[i]);
                    colTop[i].ShareDiff(m_colOutputBlobs[i]);
                }
            }

            if (m_bExposeHiddenOutput)
            {
                int nTopOffset = m_colOutputBlobs.Count;
                for (int i = nTopOffset, j = 0; i < colTop.Count; i++, j++)
                {
                    colTop[i].ReshapeLike(m_colRecurOutputBlobs[j]);
                    colTop[i].ShareData(m_colRecurOutputBlobs[j]);
                    colTop[i].ShareDiff(m_colRecurOutputBlobs[j]);
                }
            }
        }

        /// <summary>
        /// Reset the hidden state of the net by zeroing out all recurrent outputs.
        /// </summary>
        public virtual void Reset()
        {
            for (int i = 0; i < m_colRecurOutputBlobs.Count; i++)
            {
                m_colRecurOutputBlobs[i].SetData(0);
            }
        }

        /// <summary>
        /// Returns the minimum number of required bottom (input) Blobs.
        /// </summary>
        public override int MinBottomBlobs
        {
            get
            {
                int nMinBottoms = 2;

                if (m_param.recurrent_param.expose_hidden_input)
                {
                    List<string> rgInputs = new List<string>();
                    RecurrentInputBlobNames(rgInputs);
                    nMinBottoms += rgInputs.Count;
                    nMinBottoms -= 1;
                }

                return nMinBottoms;
            }
        }

        /// <summary>
        /// Returns the maximum number of required bottom (input) Blobs: min+1
        /// </summary>
        public override int MaxBottomBlobs
        {
            get { return MinBottomBlobs + 1; }
        }

        /// <summary>
        /// Returns the min number of required top (output) Blobs.
        /// </summary>
        //public override int MinTopBlobs
        //{
        //    get { return 1; }
        //}

        /// <summary>
        /// Returns the max number of required top (output) Blobs.
        /// </summary>
        public override int ExactNumTopBlobs
        {
            get
            {
                int nNumTops = 1; // MinTopBlobs;

                if (m_param.recurrent_param.expose_hidden_output)
                {
                    List<string> rgOutputs = new List<string>();
                    RecurrentOutputBlobNames(rgOutputs);
                    nNumTops += rgOutputs.Count;
                }

                return nNumTops;
            }
        }

        /// <summary>
        /// Returns <i>true</i> for all but the bottom index = 1, for you can't propagate to the sequence continuation indicators.
        /// </summary>
        /// <param name="nBottomIdx">Specifies the bottom index.</param>
        /// <returns>Returns whether or not to allow forced backward.</returns>
        public override bool AllowForceBackward(int nBottomIdx)
        {
            // Can't propagate to sequence continuation indicators.
            return (nBottomIdx != 1) ? true : false;
        }

        /// <summary>
        /// Fills net_param with the recurrent network architecture.  Subclasses
        /// should define this -- see RNNLayer and LSTMLayer for examples.
        /// </summary>
        /// <param name="net_param">Specifies the net_param to be filled.</param>
        protected abstract void FillUnrolledNet(NetParameter net_param);

        /// <summary>
        /// Fills names with the names of the 0th timestep recurrent input
        /// Blob's.  Subclasses should define this -- see RNNlayer and LSTMLayer
        /// for examples.
        /// </summary>
        /// <param name="rgNames">Specifies the input names.</param>
        protected abstract void RecurrentInputBlobNames(List<string> rgNames);

        /// <summary>
        /// Fills shapes with the shapes of the recurrent input Blob's.
        /// Subclassses should define this -- see RNNLayer and LSTMLayer
        /// for examples.
        /// </summary>
        /// <param name="rgShapes">Specifies the shapes to be filled.</param>
        protected abstract void RecurrentInputShapes(List<BlobShape> rgShapes);

        /// <summary>
        /// Fills names with the names of the Tth timestep recurrent output
        /// Blob's.  Subclassses should define this -- see RNNLayer and LSTMLayer
        /// for examples.
        /// </summary>
        /// <param name="rgNames">Specifies the output names.</param>
        protected abstract void RecurrentOutputBlobNames(List<string> rgNames);

        /// <summary>
        /// Fills names with the names of the output blobs, concatenated across
        /// all timesteps. Should return a name for each top Blob.
        /// Subclassses should define this -- see RNNLayer and LSTMLayer
        /// for examples.
        /// </summary>
        /// <param name="rgNames">Specifies the output names.</param>
        protected abstract void OutputBlobNames(List<string> rgNames);

        /** @copydoc Layer::setup_internal_blobs */
        protected override void setup_internal_blobs(BlobCollection<T> col)
        {
            if (col.Count > 0)
                return;

            if (m_blobCx != null)
                col.Add(m_blobCx);

            if (m_blobHx != null)
                col.Add(m_blobHx);

            if (m_blobCy != null)
                col.Add(m_blobCy);

            if (m_blobHy != null)
                col.Add(m_blobHy);
        }

        /// <summary>
        /// Peforms the forward calculation.
        /// </summary>
        /// <param name="colBottom">bottom input Blob vector (length 2-3)
        /// -# @f$ (T \times N \times ...) @f$
        ///     the time-varying input @f$ x @f$. After the first two axes, whose
        ///     dimensions must correspond to the number of timesteps @f$ T @f$ and
        ///     the number of independent streams @f$ N @f$, respectively, its
        ///     dimensions may be arbitrary.  Note that the ordering of dimensions --
        ///     @f$ (T \times N \times ...) @f$, rather than
        ///     @f$ (N \times T \times ...) @f$ -- means that the @f$ N @f$ independent input
        ///     streams must be 'interleaved'.
        ///     
        /// -# @f$ (T \times N) @f$
        ///     the sequence continuation indicators @f$ \delta @f$. 
        ///     These inputs should be binary (0 or 1) indicators, where
        ///     @f$ \delta{t,n} = 0 @f$ means that timestep @f$ t @f$ of stream
        ///     @f$ n @f$ is the beginning of a new sequence, and hence the previous
        ///     hidden state @f$ h_{t-1} @f$ is mulitplied by @f$ \delta_t = 0 @f$ and
        ///     has no effect on the cell's output at timestep 't', and 
        ///     a value of @f$ \delta_{t,n} = 1 @f$ means that timestep @f$ t @f$ of
        ///     stream @f$ n @f$ is a continuation from the previous timestep
        ///     @f$ t-1 @f$, and the previous hidden state @f$ h_{t-1} @f$ affects the
        ///     updated hidden state and output.
        ///     
        /// -# @f$ (N \times ...) @f$ (optional)
        ///     the static (non-time-varying) input @f$ x_{static} @f$.
        ///     After the first axis, whose dimensions must be the number of
        ///     independent streams, its dimensions must be the number of
        ///     independent streams, its dimensions may be arbitrary.
        ///     This is mathematically equivalent to using a time-varying input of
        ///     @f$ x'_t = [x_t; x_{static}] @f$ -- i.e., tiling the static input
        ///     across the 'T' timesteps and concatenating with the time-varying
        ///     input.  Note that if this input is used, all timesteps in a single
        ///     batch within a particular one of the @f$ N @f$ streams must share the
        ///     same static input, even if the sequence continuation indicators
        ///     suggest that difference sequences are ending and beginning within a 
        ///     single batch.  This may require padding and/or truncation for uniform
        ///     length.
        /// </param>
        /// <param name="colTop">top output Blob (length 1)
        /// -# @f$ (T \times N \times D) @f$
        ///     the time-varying output @f$ y @f$, where @f$ d @f$ is
        ///     <code>recurrent_param.num_output</code>.
        ///     Refer to documentation for particular RecurrentLayer implementations
        ///     (such as RNNLayer or LSTMLayer) for the definition of @f$ y @f$.
        /// </param>
        protected override void forward(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            if (m_param.recurrent_param.batch_first)
            {
                addBtmTop(colBottom[0], m_blobBtmData);
                m_transposeData.Forward(m_colBtm, m_colTop);
                addBtmTop(colBottom[1], m_blobBtmClip);
                m_transposeClip.Forward(m_colBtm, m_colTop);
            }

            if (m_param.recurrent_param.useCudnn())
                forward_cudnn(colBottom, colTop);
            else
                forward_cuda(colBottom, colTop);

            if (m_param.recurrent_param.batch_first)
            {
                addBtmTop(m_blobTopData, colTop[0]);
                m_transposeData.Forward(m_colBtm, m_colTop);
            }
        }

        private void copy_or_repeat_fwd(Blob<T> bBtm, Blob<T> bTop) 
        {
            if (!m_param.recurrent_param.auto_repeat_hidden_states_across_layers || bBtm.count() == bTop.count())
            {
                if (bBtm.count() == bTop.count())
                    m_cuda.copy(bBtm.count(), bBtm.gpu_data, bTop.mutable_gpu_data);
            }
            else
            {
                // Repeat the hidden for each layer               
                m_log.CHECK_EQ(bBtm.count(bBtm.num_axes - 2), bTop.count(1), "The '" + bBtm.Name.ToString() + "' should have the same shape as '" + bTop.Name.ToString() + "' which has a shape after the first axis = " + bTop.shape_string);
                m_cuda.channel_copy(bBtm.count(), 1, 1, bTop.num, bBtm.count(), 0, bTop.mutable_gpu_data, bBtm.gpu_data, DIR.BWD);
                for (int i = 1; i < bTop.num; i++)
                {
                    m_cuda.channel_copy(bBtm.count(), 1, 1, bTop.num, bBtm.count(), i, bTop.mutable_gpu_data, bBtm.gpu_data, DIR.BWD);
                }
            }
        }

        private void copy_or_repeat_bwd(Blob<T> bBtm, Blob<T> bTop)
        {
            if (!m_param.recurrent_param.auto_repeat_hidden_states_across_layers || bBtm.count() == bTop.count())
            {
                m_log.CHECK_EQ(bBtm.count(), bTop.count(), "The '" + bBtm.Name.ToString() + "' should have the same shape as '" + bTop.Name.ToString() + "' which has a shape = " + bTop.shape_string);
                m_cuda.copy(bBtm.count(), bTop.gpu_diff, bBtm.mutable_gpu_diff);
            }
            else
            {
                // Repeat the hidden for each layer
                m_log.CHECK_EQ(bBtm.count(), bTop.count(1), "The '" + bBtm.Name.ToString() + "' should have the same shape as '" + bTop.Name.ToString() + "' which has a shape after the first axis = " + bTop.shape_string);
                m_cuda.channel_copy(bBtm.count(), 1, 1, bTop.num, bBtm.count(), 0, bTop.gpu_diff, bBtm.mutable_gpu_diff, DIR.FWD);

                for (int i = 1; i < bTop.num; i++)
                {
                    m_cuda.channel_add(bBtm.count(), 1, 1, bTop.num, bBtm.count(), i, bTop.gpu_diff, bBtm.mutable_gpu_diff, DIR.FWD);
                }
            }
        }

        private void forward_cudnn(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            if (m_bUseCudnnRnn8)
                forward_cudnnRnn8(colBottom, colTop);
            else
                forward_cudnnRnn(colBottom, colTop);
        }

        private void forward_cudnnRnn8(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            if (colBottom.Count > 2)
            {
                // Allow for setting initial state used with cuDnn LSTM
                if (colBottom.Count > 2)
                    copy_or_repeat_fwd(colBottom[2], m_blobHx);

                if (colBottom.Count > 3)
                    copy_or_repeat_fwd(colBottom[3], m_blobCx);

                m_blobHy.CopyFrom(m_blobHx); // initialized with previous state in LayerSetup when colBottom.Count > 3
                m_blobCy.CopyFrom(m_blobCx); // initialized with previous state in LayerSetup when colBottom.Count > 2
            }

            m_cuda.Rnn8Forward(m_hCuDnn,
                               m_hRnn8,
                               m_blobX.gpu_data,
                               m_blobY.mutable_gpu_data,
                               m_blobHx.gpu_data,
                               m_blobHy.mutable_gpu_data,
                               m_blobCx.gpu_data,
                               m_blobCy.mutable_gpu_data,
                               m_blobWts.gpu_data,
                               m_hWorkspace,
                               m_hReserved);
        }

        private void forward_cudnnRnn(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            Blob<T> blobBtm1 = colBottom[1];
            if (m_param.recurrent_param.batch_first)
                blobBtm1 = m_blobBtmClip;

            double dfClip = Utility.ConvertVal<T>(blobBtm1.GetData(0));

            if (dfClip > 0 || colBottom.Count > 2)
            {
                // Allow for setting initial state used with cuDnn LSTM
                if (colBottom.Count > 2)
                    copy_or_repeat_fwd(colBottom[2], m_blobHy);

                if (colBottom.Count > 3)
                    copy_or_repeat_fwd(colBottom[3], m_blobCy);

                m_blobCx.CopyFrom(m_blobCy); // initialized with previous state in LayerSetup when colBottom.Count > 2
                m_blobHx.CopyFrom(m_blobHy); // initialized with previous state in LayerSetup when colBottom.Count > 3
            }

            m_cuda.RnnForward(m_hCuDnn,
                              m_hRnnDesc,
                              m_hXDesc,
                              m_blobX.gpu_data,
                              m_hHxDesc,
                              m_blobHx.gpu_data,
                              m_hCxDesc,
                              m_blobCx.gpu_data,
                              m_hWeightDesc,
                              m_blobWts.gpu_data,
                              m_hYDesc,
                              m_blobY.mutable_gpu_data,
                              m_hHyDesc,
                              m_blobHy.mutable_gpu_data,
                              m_hCyDesc,
                              m_blobCy.mutable_gpu_data,
                              m_hWorkspace,
                              m_nWorkspaceSizeInBytes,
                              m_hReserved,
                              m_nReservedSizeInBytes,
                              (m_phase == Phase.TRAIN) ? true : false);

            // Tops are shared with cy and hy in Reshape
        }

        private void forward_cuda(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            // Hacky fix for test time... reshare all the shared blobs.
            // TODO: somehow make this work non-hackily.
            //if (m_phase == Phase.TEST || m_phase == Phase.RUN)
            //    m_unrolledNet.ShareWeights();

            m_log.CHECK_EQ(m_colRecurInputBlobs.Count, m_colRecurOutputBlobs.Count, "The recurrent input and output blobs must have the same count.");

            if (!m_bExposeHiddenInput)
            {
                // Copy timestep T to timestep 0
                for (int i = 0; i < m_colRecurInputBlobs.Count; i++)
                {
                    int nCount = m_colRecurInputBlobs[i].count();
                    m_log.CHECK_EQ(nCount, m_colRecurOutputBlobs[i].count(), "The input and output blob at " + i.ToString() + " must have the same count.");
                    long hTimestep_T_Data = m_colRecurOutputBlobs[i].gpu_data;
                    long hTimestep_0_Data = m_colRecurInputBlobs[i].mutable_gpu_data;
                    m_cuda.copy(nCount, hTimestep_T_Data, hTimestep_0_Data);
                }
            }

            m_unrolledNet.ForwardFromTo(0, m_nLastLayerIndex);

            // Tops are shared with cy and hy in Reshape
        }

        /// <summary>
        /// Backward computation.
        /// </summary>
        /// <param name="colTop">See 'foward' documetation.</param>
        /// <param name="rgbPropagateDown">Specifies whether or not to propagate down.</param>
        /// <param name="colBottom">See 'forward' documentation.</param>
        protected override void backward(BlobCollection<T> colTop, List<bool> rgbPropagateDown, BlobCollection<T> colBottom)
        {
            if (m_param.recurrent_param.batch_first)
            {
                addBtmTop(m_blobTopData, colTop[0]);
                m_transposeData.Backward(m_colTop, rgbPropagateDown, m_colBtm);
            }

            if (m_param.recurrent_param.useCudnn())
                backward_cudnn(colTop, rgbPropagateDown, colBottom);
            else
                backward_cuda(colTop, rgbPropagateDown, colBottom);

            if (m_param.recurrent_param.batch_first)
            {
                addBtmTop(colBottom[0], m_blobBtmData);
                m_transposeData.Backward(m_colTop, rgbPropagateDown, m_colBtm);
            }
        }

        private void backward_cudnn(BlobCollection<T> colTop, List<bool> rgbPropagateDown, BlobCollection<T> colBottom)
        {
            if (m_bUseCudnnRnn8)
                backward_cudnnRnn8(colTop, rgbPropagateDown, colBottom);
            else
                backward_cudnnRnn(colTop, rgbPropagateDown, colBottom);
        }

        private void backward_cudnnRnn8(BlobCollection<T> colTop, List<bool> rgbPropagateDown, BlobCollection<T> colBottom)
        {
            // Copy top diffs to timestep T diffs
            long hhYDiff = 0;
            long hcYDiff = 0;

            if (colTop.Count > 2)
            {
                // Copy state diffs back to previous LSTM
                if (colTop.Count > 1)
                {
                    m_log.CHECK_EQ(colTop[1].count(), m_blobHy.count(), "The bottom(1) should have the same shape as 'hy' which has a shape = " + m_blobHy.shape_string);
                    m_blobHy.CopyFrom(colTop[1], true);
                    hhYDiff = m_blobHy.gpu_diff;
                }
                if (colTop.Count > 2)
                {
                    m_log.CHECK_EQ(colTop[2].count(), m_blobCy.count(), "The bottom(2) should have the same shape as 'cy' which has a shape = " + m_blobCy.shape_string);
                    m_blobCy.CopyFrom(colTop[2], true);
                    hcYDiff = m_blobCy.gpu_diff;
                }
            }

            m_cuda.Rnn8Backward(m_hCuDnn,
                                m_hRnn8,
                                m_blobY.gpu_data,
                                m_blobY.gpu_diff,
                                m_blobX.gpu_data,
                                m_blobX.mutable_gpu_diff,
                                m_blobHx.gpu_data,
                                hhYDiff,
                                m_blobHx.mutable_gpu_diff,
                                m_blobCx.gpu_data,
                                hcYDiff,
                                m_blobCx.mutable_gpu_diff,
                                m_blobWts.gpu_data,
                                m_blobWts.mutable_gpu_diff,
                                m_hWorkspace,
                                m_hReserved);

            // Copy timestep 0 diff to bottom diffs
            if (colBottom.Count > 2)
            {
                // Copy state diffs back to previous LSTM
                if (colBottom.Count > 2)
                    copy_or_repeat_bwd(colBottom[2], m_blobHx);

                if (colBottom.Count > 3)
                    copy_or_repeat_bwd(colBottom[3], m_blobCx);
            }
        }

        private void backward_cudnnRnn(BlobCollection<T> colTop, List<bool> rgbPropagateDown, BlobCollection<T> colBottom)
        {
            if (rgbPropagateDown[1] && !m_bWarningShown)
            {
                m_log.WriteLine("WARNING: Cannot backpropagate to sequence indicators, sequence backprop will be ignored.");
                m_bWarningShown = true;
            }

            // Copy top diffs to timestep T diffs
            if (colTop.Count > 2)
            {
                // Copy state diffs back to previous LSTM
                if (colTop.Count > 1)
                {
                    m_log.CHECK_EQ(colTop[1].count(), m_blobHy.count(), "The bottom(1) should have the same shape as 'hy' which has a shape = " + m_blobHy.shape_string);
                    m_blobHy.CopyFrom(colTop[1], true);
                }
                if (colTop.Count > 2)
                {
                    m_log.CHECK_EQ(colTop[2].count(), m_blobCy.count(), "The bottom(2) should have the same shape as 'cy' which has a shape = " + m_blobCy.shape_string);
                    m_blobCy.CopyFrom(colTop[2], true);
                }
            }

            m_cuda.RnnBackwardData(m_hCuDnn,
                              m_hRnnDesc,
                              m_hYDesc,
                              m_blobY.gpu_data,
                              m_blobY.gpu_diff,
                              m_hHyDesc,
                              m_blobHy.gpu_diff,
                              m_hCyDesc,
                              m_blobCy.gpu_diff,
                              m_hWeightDesc,
                              m_blobWts.gpu_data,
                              m_hHxDesc,
                              m_blobHx.gpu_data,
                              m_hCxDesc,
                              m_blobCx.gpu_data,
                              m_hXDesc,
                              m_blobX.mutable_gpu_diff,
                              m_hHxDesc,
                              m_blobHx.mutable_gpu_diff,
                              m_hCxDesc,
                              m_blobCx.mutable_gpu_diff,
                              m_hWorkspace,
                              m_nWorkspaceSizeInBytes,
                              m_hReserved,
                              m_nReservedSizeInBytes);
            // cudnnBackwardWeights adds to the data in weight diff.
            m_blobWts.SetDiff(0);

            m_cuda.RnnBackwardWeights(m_hCuDnn,
                              m_hRnnDesc,
                              m_hXDesc,
                              m_blobX.gpu_data,
                              m_hHxDesc,
                              m_blobHx.gpu_data,
                              m_hYDesc,
                              m_blobY.gpu_data,
                              m_hWorkspace,
                              m_nWorkspaceSizeInBytes,
                              m_hWeightDesc,
                              m_blobWts.mutable_gpu_diff,
                              m_hReserved,
                              m_nReservedSizeInBytes);

            // Copy timestep 0 diff to bottom diffs
            if (colBottom.Count > 2)
            {
                // Copy state diffs back to previous LSTM
                if (colBottom.Count > 2)
                    copy_or_repeat_bwd(colBottom[2], m_blobHx);

                if (colBottom.Count > 3)
                    copy_or_repeat_bwd(colBottom[3], m_blobCx);
            }
        }

        private void backward_cuda(BlobCollection<T> colTop, List<bool> rgbPropagateDown, BlobCollection<T> colBottom)
        {
            m_log.CHECK(!rgbPropagateDown[1], "Cannot backpropagate to sequence indicators.");

            // Copy top diffs to timestep T diffs (done automatically with tops sharing diffs)

            // TODO: skip backpropagation to inputs and parameters inside the unrolled
            // net according to propagate_down[0] and propagate_down[2].  For now just
            // backprop to inputs and parameters unconditionally, as either the inputs or
            // the parameters do need backward (or Net would have set
            // layer_needs_backward[i] = false for this layer).
            m_unrolledNet.Backward(m_nLastLayerIndex);

            // Copy timestep 0 diff to bottom diffs
            int nCount = (m_bStaticInput) ? 3 : 2;
            if (colBottom.Count > nCount)
            {
                // Copy state diffs back to previous LSTM
                if (colBottom.Count > nCount)
                {
                    m_log.CHECK_EQ(colBottom[nCount].count(), m_blobHx.count(), "The bottom(" + nCount.ToString() + ") should have the same shape as 'hx' which has a shape = " + m_blobHx.shape_string);
                    colBottom[nCount].CopyFrom(m_blobHx, true);
                }
                if (colBottom.Count > nCount+1)
                {
                    m_log.CHECK_EQ(colBottom[nCount + 1].count(), m_blobCx.count(), "The bottom(" + (nCount + 1).ToString() + ") should have the same shape as 'cx' which has a shape = " + m_blobCx.shape_string);
                    colBottom[nCount + 1].CopyFrom(m_blobCx, true);
                }
            }
        }
    }
}
