﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCaffe.basecode;
using MyCaffe.common;
using MyCaffe.param;
using MyCaffe.param.beta;

namespace MyCaffe.layers.beta
{
    /// <summary>
    /// The ConvolutionOctaveLayer processes high and low frequency portions of images using convolution.
    /// </summary>
    /// <remarks>
    /// @see [Drop an Octave: Reducing Spatial Redundancy in Convolutional Neural Networks with Octave Convolution](https://arxiv.org/abs/1904.05049) by 
    /// Yunpeng Chen, Haoqi Fan, Bing Xu, Zhicheng Yan, Yannis Kalantidis, Marcus Rohrbach, Shuicheng Yan, and Jiashi Feng, 2019, arXiv:1904.05049
    /// </remarks>
    /// <typeparam name="T">Specifies the base type <i>float</i> or <i>double</i>.  Using <i>float</i> is recommended to conserve GPU memory.</typeparam>
    public class ConvolutionOctaveLayer<T> : Layer<T>
    {
        double m_dfAlphaIn = 0.5;
        double m_dfAlphaOut = 0.5;
        int m_nStride;
        Layer<T> m_downsampleLayer1;
        Layer<T> m_downsampleLayer2;
        Layer<T> m_downsampleLayer3;
        Layer<T> m_upsampleLayer;
        Layer<T> m_conv_l2l = null;
        Layer<T> m_conv_l2h = null;
        Layer<T> m_conv_h2l = null;
        Layer<T> m_conv_h2h = null;
        Layer<T> m_add = null;
        Blob<T> m_blob_x_h = null;
        Blob<T> m_blob_x_l = null;
        Blob<T> m_blob_x_h_ds = null;
        Blob<T> m_blob_x_l_ds = null;
        Blob<T> m_blob_x_h2h = null;
        Blob<T> m_blob_x_h2l = null;
        Blob<T> m_blob_x_l2l = null;
        Blob<T> m_blob_x_l2h = null;
        Blob<T> m_blob_x_l2h_us = null;
        BlobCollection<T> m_rgTop = new BlobCollection<T>();
        BlobCollection<T> m_rgBtm = new BlobCollection<T>();
        long m_hWorkspaceData = 0;
        ulong m_lWorkspaceSizeInBytes = 0;

        /// <summary>
        /// The ConvolutionOctaveLayer constructor.
        /// </summary>
        /// <param name="cuda">Specifies the CudaDnn connection to Cuda.</param>
        /// <param name="log">Specifies the Log for output.</param>
        /// <param name="p">provides FlattenParameter flatten_param
        /// </param>
        public ConvolutionOctaveLayer(CudaDnn<T> cuda, Log log, LayerParameter p)
            : base(cuda, log, p)
        {
            m_type = LayerParameter.LayerType.CONVOLUTION_OCTAVE;
        }

        /// <summary>
        /// Release all resources used.
        /// </summary>
        protected override void dispose()
        {
            if (m_downsampleLayer1 != null)
            {
                m_downsampleLayer1.Dispose();
                m_downsampleLayer1 = null;
            }

            if (m_downsampleLayer2 != null)
            {
                m_downsampleLayer2.Dispose();
                m_downsampleLayer2 = null;
            }

            if (m_downsampleLayer3 != null)
            {
                m_downsampleLayer3.Dispose();
                m_downsampleLayer3 = null;
            }

            if (m_upsampleLayer != null)
            {
                m_upsampleLayer.Dispose();
                m_upsampleLayer = null;
            }

            if (m_conv_l2l != null)
            {
                m_conv_l2l.Dispose();
                m_conv_l2l = null;
            }

            if (m_conv_l2h != null)
            {
                m_conv_l2h.Dispose();
                m_conv_l2h = null;
            }

            if (m_conv_h2l != null)
            {
                m_conv_h2l.Dispose();
                m_conv_h2l = null;
            }

            if (m_conv_h2h != null)
            {
                m_conv_h2h.Dispose();
                m_conv_h2h = null;
            }

            if (m_add != null)
            {
                m_add.Dispose();
                m_add = null;
            }

            if (m_blob_x_h != null)
            {
                m_blob_x_h.Dispose();
                m_blob_x_h = null;
            }

            if (m_blob_x_l != null)
            {
                m_blob_x_l.Dispose();
                m_blob_x_l = null;
            }

            if (m_blob_x_h_ds != null)
            {
                m_blob_x_h_ds.Dispose();
                m_blob_x_h_ds = null;
            }

            if (m_blob_x_l_ds != null)
            {
                m_blob_x_l_ds.Dispose();
                m_blob_x_l_ds = null;
            }

            if (m_blob_x_h2h != null)
            {
                m_blob_x_h2h.Dispose();
                m_blob_x_h2h = null;
            }

            if (m_blob_x_h2l != null)
            {
                m_blob_x_h2l.Dispose();
                m_blob_x_h2l = null;
            }

            if (m_blob_x_l2l != null)
            {
                m_blob_x_l2l.Dispose();
                m_blob_x_l2l = null;
            }

            if (m_blob_x_l2h != null)
            {
                m_blob_x_l2h.Dispose();
                m_blob_x_l2h = null;
            }

            if (m_blob_x_l2h_us != null)
            {
                m_blob_x_l2h_us.Dispose();
                m_blob_x_l2h_us = null;
            }

            if (m_hWorkspaceData != 0)
            {
                m_cuda.FreeMemory(m_hWorkspaceData);
                m_hWorkspaceData = 0;
            }

            base.dispose();
        }

        /// <summary>
        /// Returns the minimum number of required bottom (input) Blobs: input.
        /// </summary>
        public override int MinBottomBlobs
        {
            get { return 1; }
        }

        /// <summary>
        /// Returns the maximum number of bottom (input) Blobs: in_h, in_l
        /// </summary>
        public override int MaxBottomBlobs
        {
            get { return 2; }
        }

        /// <summary>
        /// Returns the minimum number of required top (output) Blobs: output
        /// </summary>
        public override int MinTopBlobs
        {
            get { return 1; }
        }

        /// <summary>
        /// Returns the maximum number of top (output) Blobs: out_h, out_l
        /// </summary>
        public override int MaxTopBlobs
        {
            get { return 2; }
        }

        /// <summary>
        /// Setup the layer.
        /// </summary>
        /// <param name="colBottom">Specifies the collection of bottom (input) Blobs.</param>
        /// <param name="colTop">Specifies the collection of top (output) Blobs.</param>
        public override void LayerSetUp(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            m_dfAlphaIn = m_param.convolution_octave_param.alpha_in;
            m_dfAlphaOut = m_param.convolution_octave_param.alpha_out;

            m_log.CHECK_GE(m_dfAlphaIn, 0, "The alpha in must be >= 0.");
            m_log.CHECK_LE(m_dfAlphaIn, 1, "The alpha in must be <= 1.");
            m_log.CHECK_GE(m_dfAlphaOut, 0, "The alpha out must be >= 0.");
            m_log.CHECK_LT(m_dfAlphaOut, 1, "The alpha out must be < 1.");

            m_nStride = (int)m_param.convolution_param.stride[0];
            m_log.CHECK_GE(m_nStride, 1, "The stride should be >= 1.");
            m_log.CHECK_LE(m_nStride, 2, "The stride should be <= 2.");


            //--------------------------------------------
            // Create the blobs.
            //--------------------------------------------

            // process high frequency.
            m_blob_x_h = new Blob<T>(m_cuda, m_log);
            m_blob_x_h.Name = "x_h";
            m_blob_x_h2h = new Blob<T>(m_cuda, m_log);
            m_blob_x_h2h.Name = "x_h2h";

            if (m_dfAlphaOut > 0)
            {
                m_blob_x_h_ds = new Blob<T>(m_cuda, m_log);
                m_blob_x_h_ds.Name = "x_h_ds";
                m_blob_x_h2l = new Blob<T>(m_cuda, m_log);
                m_blob_x_h2l.Name = "x_h2l";
            }

            // process low frequency.
            if (colBottom.Count > 1)
            {
                m_blob_x_l = new Blob<T>(m_cuda, m_log);
                m_blob_x_l.Name = "x_l";
                m_blob_x_l_ds = new Blob<T>(m_cuda, m_log);
                m_blob_x_l_ds.Name = "x_l_ds";
                m_blob_x_l2h = new Blob<T>(m_cuda, m_log);
                m_blob_x_l2h.Name = "x_l2h";
                m_blob_x_l2h_us = new Blob<T>(m_cuda, m_log);
                m_blob_x_l2h_us.Name = "x_l2h_us";
                m_blob_x_l2l = new Blob<T>(m_cuda, m_log);
                m_blob_x_l2l.Name = "x_l2l";
            }

            //--------------------------------------------
            // Create the internal layers.
            //--------------------------------------------

            LayerParameter poolParam = new LayerParameter(LayerParameter.LayerType.POOLING, "downsample");
            poolParam.pooling_param.kernel_size.Add(2);
            poolParam.pooling_param.stride.Add(2);
            poolParam.pooling_param.pool = PoolingParameter.PoolingMethod.AVE;
            poolParam.pooling_param.engine = m_param.convolution_param.engine;

            if (m_nStride == 2)
            {
                m_downsampleLayer1 = Layer<T>.Create(m_cuda, m_log, poolParam, null);
                setupBtmTop(colBottom[0], m_blob_x_h);
                m_downsampleLayer1.LayerSetUp(m_rgBtm, m_rgTop);
                m_downsampleLayer1.Reshape(m_rgBtm, m_rgTop);
            }
            else
            {
                m_blob_x_h.ReshapeLike(colBottom[0]);
            }

            LayerParameter convParamBase = new LayerParameter(LayerParameter.LayerType.CONVOLUTION);
            convParamBase.convolution_param.engine = m_param.convolution_param.engine;
            convParamBase.convolution_param.kernel_size = m_param.convolution_param.kernel_size;
            convParamBase.convolution_param.stride.Add(1);
            convParamBase.convolution_param.pad = m_param.convolution_param.pad;
            convParamBase.convolution_param.dilation = m_param.convolution_param.dilation;
            convParamBase.convolution_param.bias_filler = m_param.convolution_param.bias_filler;
            convParamBase.convolution_param.bias_term = m_param.convolution_param.bias_term;

            int nInChannels = colBottom[0].channels;
            uint nOutChannels = m_param.convolution_param.num_output;
            uint nGroup = m_param.convolution_param.group;
            uint nGroupTmp;

            // h2h Layer
            {
                LayerParameter convParam = convParamBase.Clone(false);
                convParam.name = "h2h conv";
                convParam.convolution_param.num_output = nOutChannels - (uint)(m_dfAlphaOut * nOutChannels);
                nGroupTmp = (uint)Math.Ceiling(nGroup - m_dfAlphaIn * nGroup);

                convParam.convolution_param.group = (nInChannels % nGroupTmp == 0) ? nGroupTmp : 1;
                m_conv_h2h = Layer<T>.Create(m_cuda, m_log, convParam, null);
                m_conv_h2h.OnGetWorkspace += layer_OnGetWorkspace;
                m_conv_h2h.OnSetWorkspace += layer_OnSetWorkspace;

                setupBtmTop(m_blob_x_h, m_blob_x_h2h);
                m_conv_h2h.LayerSetUp(m_rgBtm, m_rgTop);
                m_conv_h2h.Reshape(m_rgBtm, m_rgTop);
                m_colBlobs.Add(m_conv_h2h.blobs);
            }

            // h2l Layer
            if (m_dfAlphaOut > 0)
            {
                m_downsampleLayer2 = Layer<T>.Create(m_cuda, m_log, poolParam, null);
                setupBtmTop(m_blob_x_h, m_blob_x_h_ds);
                m_downsampleLayer2.LayerSetUp(m_rgBtm, m_rgTop);
                m_downsampleLayer2.Reshape(m_rgBtm, m_rgTop);

                LayerParameter convParam = convParamBase.Clone(false);
                convParam.name = "h2l conv";
                convParam.convolution_param.num_output = (uint)(m_dfAlphaOut * nOutChannels);
                convParam.convolution_param.group = (convParam.convolution_param.num_output % nGroup == 0) ? nGroup : 1;

                m_conv_h2l = Layer<T>.Create(m_cuda, m_log, convParam, null);
                m_conv_h2l.OnGetWorkspace += layer_OnGetWorkspace;
                m_conv_h2l.OnSetWorkspace += layer_OnSetWorkspace;

                setupBtmTop(m_blob_x_h_ds, m_blob_x_h2l);
                m_conv_h2l.LayerSetUp(m_rgBtm, m_rgTop);
                m_conv_h2l.Reshape(m_rgBtm, m_rgTop);
                m_colBlobs.Add(m_conv_h2l.blobs);
            }

            if (colBottom.Count > 1)
            {
                m_blob_x_l.ReshapeLike(colBottom[1]);

                // downsample3 Layer
                if (m_nStride == 2)
                {
                    m_downsampleLayer3 = Layer<T>.Create(m_cuda, m_log, poolParam, null);
                    setupBtmTop(colBottom[1], m_blob_x_l_ds);
                    m_downsampleLayer3.LayerSetUp(m_rgBtm, m_rgTop);
                    m_downsampleLayer3.Reshape(m_rgBtm, m_rgTop);
                }
                else
                {
                    m_blob_x_l_ds.ReshapeLike(m_blob_x_l);
                }

                // l2l layer
                if (m_dfAlphaOut > 0)
                {
                    LayerParameter convParam = convParamBase.Clone(false);
                    convParam.name = "l2l conv";
                    convParam.convolution_param.num_output = (uint)(m_dfAlphaOut * nOutChannels);
                    nGroupTmp = (uint)Math.Ceiling(m_dfAlphaIn * nGroup);
                    convParam.convolution_param.group = (convParam.convolution_param.num_output % nGroupTmp == 0) ? nGroupTmp : 1;
                    m_conv_l2l = Layer<T>.Create(m_cuda, m_log, convParam, null);
                    m_conv_l2l.OnGetWorkspace += layer_OnGetWorkspace;
                    m_conv_l2l.OnSetWorkspace += layer_OnSetWorkspace;

                    setupBtmTop(m_blob_x_l_ds, m_blob_x_l2l);
                    m_conv_l2l.LayerSetUp(m_rgBtm, m_rgTop);
                    m_conv_l2l.Reshape(m_rgBtm, m_rgTop);
                    m_colBlobs.Add(m_conv_l2l.blobs);
                }

                // l2h Layer
                {
                    LayerParameter convParam = convParamBase.Clone(false);
                    convParam.name = "l2h conv";
                    convParam.convolution_param.num_output = nOutChannels - (uint)(m_dfAlphaOut * nOutChannels);
                    convParam.convolution_param.group = (convParam.convolution_param.num_output % nGroup == 0) ? nGroup : 1;
                    m_conv_l2h = Layer<T>.Create(m_cuda, m_log, convParam, null);
                    m_conv_l2h.OnGetWorkspace += layer_OnGetWorkspace;
                    m_conv_l2h.OnSetWorkspace += layer_OnSetWorkspace;

                    setupBtmTop(m_blob_x_l, m_blob_x_l2h);
                    m_conv_l2h.LayerSetUp(m_rgBtm, m_rgTop);
                    m_conv_l2h.Reshape(m_rgBtm, m_rgTop);
                    m_colBlobs.Add(m_conv_l2h.blobs);
                }

                // upsample Layer
                if (m_nStride == 1)
                {
                    LayerParameter interpParam = new LayerParameter(LayerParameter.LayerType.INTERP, "upsample");
                    interpParam.interp_param.zoom_factor = 2;
                    m_upsampleLayer = Layer<T>.Create(m_cuda, m_log, interpParam, null);

                    setupBtmTop(m_blob_x_l2h, m_blob_x_l2h_us);
                    m_upsampleLayer.LayerSetUp(m_rgBtm, m_rgTop);
                    m_upsampleLayer.Reshape(m_rgBtm, m_rgTop);
                }
                else
                {
                    m_blob_x_l2h_us.ReshapeLike(m_blob_x_l2h);
                }

                // add Layer
                LayerParameter eltAdd = new LayerParameter(LayerParameter.LayerType.ELTWISE);
                eltAdd.name = "eltadd";
                eltAdd.eltwise_param.operation = EltwiseParameter.EltwiseOp.SUM;
                m_add = Layer<T>.Create(m_cuda, m_log, eltAdd, null);

                setupBtmTop(m_blob_x_l2h_us, m_blob_x_h2h, colTop[0]);
                m_add.LayerSetUp(m_rgBtm, m_rgTop);
                m_add.Reshape(m_rgBtm, m_rgTop);

                if (m_dfAlphaOut > 0)
                {
                    setupBtmTop(m_blob_x_h2l, m_blob_x_l2l, colTop[1]);
                    m_add.Reshape(m_rgBtm, m_rgTop);
                }
            }
        }

        private void layer_OnSetWorkspace(object sender, WorkspaceArgs e)
        {
            if (e.WorkspaceSizeInBytes < m_lWorkspaceSizeInBytes || e.WorkspaceSizeInBytes == 0)
                return;

            m_lWorkspaceSizeInBytes = e.WorkspaceSizeInBytes;
            m_cuda.DisableGhostMemory();

            if (m_hWorkspaceData != 0)
                m_cuda.FreeMemory(m_hWorkspaceData);

            m_hWorkspaceData = m_cuda.AllocMemory((long)m_lWorkspaceSizeInBytes);
            m_cuda.ResetGhostMemory();
        }

        private void layer_OnGetWorkspace(object sender, WorkspaceArgs e)
        {
            e.WorkspaceData = m_hWorkspaceData;
            e.WorkspaceSizeInBytes = m_lWorkspaceSizeInBytes;
        }

        private void setupBtmTop(Blob<T> btm, Blob<T> top)
        {
            m_rgBtm.Clear();
            m_rgBtm.Add(btm);
            m_rgTop.Clear();
            m_rgTop.Add(top);
        }

        private void setupBtmTop(Blob<T> btm1, Blob<T> btm2, Blob<T> top)
        {
            m_rgBtm.Clear();
            m_rgBtm.Add(btm1);
            m_rgBtm.Add(btm2);
            m_rgTop.Clear();
            m_rgTop.Add(top);
        }

        /// <summary>
        /// Reshape the bottom (input) and top (output) blobs.
        /// </summary>
        /// <param name="colBottom">Specifies the collection of bottom (input) Blobs.</param>
        /// <param name="colTop">Specifies the collection of top (output) Blobs.</param>
        public override void Reshape(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            if (m_nStride == 2)
            {
                setupBtmTop(colBottom[0], m_blob_x_h);
                m_downsampleLayer1.Reshape(m_rgBtm, m_rgTop);
            }
            else
            {
                m_blob_x_h.ReshapeLike(colBottom[0]);
            }

            setupBtmTop(m_blob_x_h, m_blob_x_h2h);
            m_conv_h2h.Reshape(m_rgBtm, m_rgTop);

            if (m_dfAlphaOut > 0)
            {
                setupBtmTop(m_blob_x_h, m_blob_x_h_ds);
                m_downsampleLayer2.Reshape(m_rgBtm, m_rgTop);
                setupBtmTop(m_blob_x_h_ds, m_blob_x_h2l);
                m_conv_h2l.Reshape(m_rgBtm, m_rgTop);
            }

            if (colBottom.Count > 1)
            {
                m_blob_x_l.ReshapeLike(colBottom[1]);

                if (m_nStride == 2)
                {
                    setupBtmTop(colBottom[1], m_blob_x_l_ds);
                    m_downsampleLayer3.Reshape(m_rgBtm, m_rgTop);
                }
                else
                {
                    m_blob_x_l_ds.ReshapeLike(m_blob_x_l);
                }

                if (m_dfAlphaOut > 0)
                {
                    setupBtmTop(m_blob_x_l_ds, m_blob_x_l2l);
                    m_conv_l2l.Reshape(m_rgBtm, m_rgTop);
                }

                setupBtmTop(m_blob_x_l, m_blob_x_l2h);
                m_conv_l2h.Reshape(m_rgBtm, m_rgTop);

                if (m_nStride == 1)
                {
                    setupBtmTop(m_blob_x_l2h, m_blob_x_l2h_us);
                    m_upsampleLayer.Reshape(m_rgBtm, m_rgTop);
                }
                else
                {
                    m_blob_x_l2h_us.ReshapeLike(m_blob_x_l2h);
                }

                setupBtmTop(m_blob_x_l2h_us, m_blob_x_h2h, colTop[0]);
                m_add.Reshape(m_rgBtm, m_rgTop);

                if (m_dfAlphaOut > 0)
                {
                    setupBtmTop(m_blob_x_h2l, m_blob_x_l2l, colTop[1]);
                    m_add.Reshape(m_rgBtm, m_rgTop);
                }
            }
            else
            {
                colTop[0].ReshapeLike(m_blob_x_h2h);

                if (m_dfAlphaOut > 0)
                    colTop[1].ReshapeLike(m_blob_x_h2l);
            }
        }

        /// <summary>
        /// Forward computation.
        /// </summary>
        /// <param name="colBottom">bottom input blob vector (length 2+)
        ///  -# @f$ (N \times C \times H \times W) @f$ the inputs.
        ///     the inputs.</param>
        /// <param name="colTop">top output blob vector (length 1)
        ///  -# @f$ (N \times CHW \times 1 \times 1) @f$ the outputs -- i.e., the (virtually) copied, flattened inputs
        /// </param>
        protected override void forward(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            if (m_nStride == 2)
            {
                setupBtmTop(colBottom[0], m_blob_x_h);              // x_h = self.downsample(bottom)
                m_downsampleLayer1.Forward(m_rgBtm, m_rgTop);
            }
            else
            {
                m_blob_x_h.CopyFrom(colBottom[0]);
            }

            setupBtmTop(m_blob_x_h, m_blob_x_h2h);                  // x_h2h = self_conv_h2h(x_h)      x_h2h -> x_h
            m_conv_h2h.Forward(m_rgBtm, m_rgTop);

            if (m_dfAlphaOut > 0)
            {
                setupBtmTop(m_blob_x_h, m_blob_x_h_ds);             // tmp = self.downsample(x_h)       bwd tmp -> x_h
                m_downsampleLayer2.Forward(m_rgBtm, m_rgTop);
                setupBtmTop(m_blob_x_h_ds, m_blob_x_h2l);           // m_h2l = self.conv_h2l(tmp)       bwd m_h2l -> tmp
                m_conv_h2l.Forward(m_rgBtm, m_rgTop);
            }

            if (colBottom.Count > 1)
            {
                m_blob_x_l.CopyFrom(colBottom[1]);

                if (m_nStride == 2)
                {
                    setupBtmTop(m_blob_x_l, m_blob_x_l_ds);
                    m_downsampleLayer3.Forward(m_rgBtm, m_rgTop);
                }
                else
                {
                    m_blob_x_l_ds.CopyFrom(m_blob_x_l);
                }

                if (m_dfAlphaOut > 0)
                {
                    setupBtmTop(m_blob_x_l_ds, m_blob_x_l2l);
                    m_conv_l2l.Forward(m_rgBtm, m_rgTop);
                }

                setupBtmTop(m_blob_x_l, m_blob_x_l2h);
                m_conv_l2h.Forward(m_rgBtm, m_rgTop);

                if (m_nStride == 1)
                {
                    setupBtmTop(m_blob_x_l2h, m_blob_x_l2h_us);
                    m_upsampleLayer.Forward(m_rgBtm, m_rgTop);
                }
                else
                {
                    m_blob_x_l2h_us.CopyFrom(m_blob_x_l2h);
                }

                setupBtmTop(m_blob_x_l2h_us, m_blob_x_h2h, colTop[0]);
                m_add.Forward(m_rgBtm, m_rgTop);

                if (m_dfAlphaOut > 0)
                {
                    setupBtmTop(m_blob_x_h2l, m_blob_x_l2l, colTop[1]);
                    m_add.Forward(m_rgBtm, m_rgTop);
                }

                return;
            }
            else
            {
                colTop[0].CopyFrom(m_blob_x_h2h);

                if (m_dfAlphaOut > 0)
                    colTop[1].CopyFrom(m_blob_x_h2l);
            }

        }

        /// <summary>
        /// Computes the error gradient w.r.t. the concatenate inputs.
        /// </summary>
        /// <param name="colTop">top output Blob vecotr (length 1), 
        /// providing the error gradient with respect to the outputs.</param>
        /// <param name="rgbPropagateDown">see Layer::Backward</param>
        /// <param name="colBottom">input Blob vecotor (length @f$ k @f$), into which the top error
        /// gradient is (virtually) copied.</param>
        protected override void backward(BlobCollection<T> colTop, List<bool> rgbPropagateDown, BlobCollection<T> colBottom)
        {
            if (!rgbPropagateDown[0])
                return;

            if (colBottom.Count > 1)
            {
                if (m_dfAlphaOut > 0)
                {
                    setupBtmTop(m_blob_x_h2l, m_blob_x_l2l, colTop[1]);
                    m_add.Backward(m_rgTop, rgbPropagateDown, m_rgBtm);
                }

                setupBtmTop(m_blob_x_l2h_us, m_blob_x_h2h, colTop[0]);
                m_add.Backward(m_rgTop, rgbPropagateDown, m_rgBtm);

                if (m_nStride == 1)
                {
                    setupBtmTop(m_blob_x_l2h, m_blob_x_l2h_us);
                    m_upsampleLayer.Backward(m_rgTop, rgbPropagateDown, m_rgBtm);
                }
                else
                {
                    m_blob_x_l2h.CopyFrom(m_blob_x_l2h_us, true);
                }

                setupBtmTop(m_blob_x_l_ds, m_blob_x_l2h);
                m_conv_l2h.Backward(m_rgTop, rgbPropagateDown, m_rgBtm);

                if (m_dfAlphaOut > 0)
                {
                    setupBtmTop(m_blob_x_l_ds, m_blob_x_l2l);
                    m_conv_l2l.Backward(m_rgTop, rgbPropagateDown, m_rgBtm);
                }

                if (m_nStride == 2)
                {
                    setupBtmTop(m_blob_x_l, m_blob_x_l_ds);
                    m_downsampleLayer3.Backward(m_rgTop, rgbPropagateDown, m_rgBtm);
                }
                else
                {
                    m_blob_x_l.CopyFrom(m_blob_x_l_ds, true);
                }

                colBottom[1].CopyFrom(m_blob_x_l, true);
            }
            else
            {
                m_blob_x_h2h.CopyFrom(colTop[0], true);

                if (m_dfAlphaOut > 0)
                    m_blob_x_h2l.CopyFrom(colTop[1], true);
            }

            if (m_dfAlphaOut > 0)
            {
                setupBtmTop(m_blob_x_h_ds, m_blob_x_h2l);
                m_conv_h2l.Backward(m_rgTop, rgbPropagateDown, m_rgBtm);

                setupBtmTop(m_blob_x_h, m_blob_x_h_ds);
                m_downsampleLayer2.Backward(m_rgTop, rgbPropagateDown, m_rgBtm);
            }

            setupBtmTop(m_blob_x_h, m_blob_x_h2h);
            m_conv_h2h.Backward(m_rgTop, rgbPropagateDown, m_rgBtm);

            if (m_nStride == 2)
            {
                setupBtmTop(colBottom[0], m_blob_x_h);
                m_downsampleLayer1.Backward(m_rgTop, rgbPropagateDown, m_rgBtm);
            }
            else
            {
                colBottom[0].CopyFrom(m_blob_x_h, true);
            }
        }
    }
}
