﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MyCaffe.basecode;
using MyCaffe.common;
using MyCaffe.param;

namespace MyCaffe.layers.gpt
{
    /// <summary>
    /// The PositionalEncodingLayer is a neuron layer that adds positional encoding to the input.
    /// </summary>
    /// <remarks>
    /// @see [GitHub:devjwsong:transformer-translator-pytorch](https://github.com/devjwsong/transformer-translator-pytorch/blob/master/src/layers.py) by Song, 2021, GitHub:devjwsong
    /// </remarks>
    /// <typeparam name="T">Specifies the base type <i>float</i> or <i>double</i>.  Using <i>float</i> is recommended to conserve GPU memory.</typeparam>
    public class PositionalEncodingLayer<T> : Layer<T>
    {
        Blob<T> m_blobPosEnc;
        List<int> m_rgShape = new List<int>() { 1, 1, 1 };
        double m_dfScale;
        int m_nBlockSize;
        int m_nEmbed;

        /// <summary>
        /// The PositionalEncoderLayer constructor.
        /// </summary>
        /// <param name="cuda">Specifies the CudaDnn connection to Cuda.</param>
        /// <param name="log">Specifies the Log for output.</param>
        /// <param name="p">Specifies the LayerParameter of type Mish with parameter Mish_param
        /// </param>
        public PositionalEncodingLayer(CudaDnn<T> cuda, Log log, LayerParameter p)
            : base(cuda, log, p)
        {
            m_type = LayerParameter.LayerType.POSITIONAL_ENCODER;
            m_nBlockSize = (int)p.positional_encoder_param.block_size;
            m_nEmbed = (int)p.positional_encoder_param.embed;
            m_dfScale = Math.Sqrt(m_nEmbed);

            m_blobPosEnc = new Blob<T>(m_cuda, m_log, false);
            m_blobPosEnc.Name = p.name + " posenc";

            setup_internal_blobs(m_colInternalBlobs);
        }

        /// <summary>
        /// Release any resources used.
        /// </summary>
        protected override void dispose()
        {
            dispose(ref m_blobPosEnc);
            base.dispose();
        }

        /** @copydoc Layer::setup_internal_blobs */
        protected override void setup_internal_blobs(BlobCollection<T> col)
        {
            if (col.Count > 0)
                return;

            col.Add(m_blobPosEnc);
        }

        /// <summary>
        /// Returns the exact number of required bottom (input) Blobs: embed
        /// </summary>
        public override int ExactNumBottomBlobs
        {
            get { return 1; }
        }

        /// <summary>
        /// Returns the exact number of required top (output) Blobs: embed
        /// </summary>
        public override int ExactNumTopBlobs
        {
            get { return 1; }
        }
       
        /// <summary>
        /// Setup the layer.
        /// </summary>
        /// <param name="colBottom">Specifies the collection of bottom (input) Blobs.</param>
        /// <param name="colTop">Specifies the collection of top (output) Blobs.</param>
        public override void LayerSetUp(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
        }

        /// <summary>
        /// Reshape the data as needed by the layer.
        /// </summary>
        /// <param name="colBottom"></param>
        /// <param name="colTop"></param>
        public override void Reshape(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            colTop[0].ReshapeLike(colBottom[0]);

            int nBatch = colBottom[0].num;
            m_rgShape[0] = nBatch;
            m_rgShape[1] = m_nBlockSize;
            m_rgShape[2] = m_nEmbed;

            shareLayerBlob(m_blobPosEnc, m_rgShape);
            if (!m_blobPosEnc.CompareShape(m_rgShape, true))
            {
                m_blobPosEnc.Reshape(m_rgShape);
                m_blobPosEnc.Reshape(1, m_rgShape[1], m_rgShape[2], 1);
                int nDim = m_nBlockSize * m_nEmbed;

                if (typeof(T) == typeof(float))
                {
                    float[] rgPosEnc1 = new float[nDim];
                    for (int pos = 0; pos < m_nBlockSize; pos++)
                    {
                        for (int i = 0; i < m_nEmbed; i++)
                        {
                            int nIdx = pos * m_nEmbed + i;
                            double df1 = 2 * i / (double)m_nEmbed;
                            double dfPow = Math.Pow(10000, df1);
                            double dfPos = pos / dfPow;

                            if (i % 2 == 0)
                            {
                                double dfSin = Math.Sin(dfPos);
                                rgPosEnc1[nIdx] = (float)dfSin;
                            }
                            else if (i % 2 == 1)
                            {
                                double dfCos = Math.Cos(dfPos);
                                rgPosEnc1[nIdx] = (float)dfCos;
                            }
                        }
                    }

                    m_blobPosEnc.mutable_cpu_data = convert(rgPosEnc1);
                }
                else
                {
                    double[] rgPosEnc1 = new double[nDim];
                    for (int pos = 0; pos < m_nBlockSize; pos++)
                    {
                        for (int i = 0; i < m_nEmbed; i++)
                        {
                            int nIdx = pos * m_nEmbed + i;
                            double df1 = 2 * i / (double)m_nEmbed;
                            double dfPow = Math.Pow(10000, df1);
                            double dfPos = pos / dfPow;

                            if (i % 2 == 0)
                            {
                                double dfSin = Math.Sin(dfPos);
                                rgPosEnc1[nIdx] = dfSin;
                            }
                            else if (i % 2 == 1)
                            {
                                double dfCos = Math.Cos(dfPos);
                                rgPosEnc1[nIdx] = dfCos;
                            }
                        }
                    }

                    m_blobPosEnc.mutable_cpu_data = convert(rgPosEnc1);
                }

                if (nBatch > 1)
                {
                    m_blobPosEnc.Reshape(m_rgShape);

                    for (int i = 1; i < nBatch; i++)
                    {
                        m_cuda.copy(nDim, m_blobPosEnc.gpu_data, m_blobPosEnc.mutable_gpu_data, 0, i * nDim);
                    }
                }
            }
        }

        /// <summary>
        /// Forward computation
        /// </summary>
        /// <param name="colBottom">inpub Blob vector (length 1)
        ///  -# @f$ (N \times C \times H \times W) @f$ 
        ///     the inputs @f$ x @f$
        ///  </param>
        /// <param name="colTop">top output Blob vector (length 1)
        ///  -# @f$ (N \times C \times H \times W) @f$
        ///     the computed outputs @f$ 
        ///         y  = x * sqrt(m_nEmbed) + pos_enc
        ///     @f$.
        /// </param>
        protected override void forward(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            long hBottomData = colBottom[0].gpu_data;
            long hTopData = colTop[0].mutable_gpu_data;
            int nCount = colBottom[0].count();
            
            m_cuda.add(nCount, m_blobPosEnc.gpu_data, hBottomData, hTopData, m_dfScale);
        }

        /// <summary>
        /// Computes the error gradient w.r.t. the PositionalEncoder value inputs.
        /// </summary>
        /// <param name="colTop">top output blob vector (length 1), providing the error gradient
        /// with respect to outputs
        ///  -# @f$ (N \times C \times H \times W) @f$
        ///     containing error gradients @f$ \frac{\partial E}{\partial y} @f$
        ///     with respect to computed outputs @f$ y @f$
        /// </param>
        /// <param name="rgbPropagateDown">propagate_down see Layer::Backward.</param>
        /// <param name="colBottom">bottom input blob vector (length 1)
        ///  -# @f$ (N \times C \times H \times W) @f$
        ///     gradients f$ y' = scale @f$
        ///     @f$ if propagate_down[0]
        /// </param>
        protected override void backward(BlobCollection<T> colTop, List<bool> rgbPropagateDown, BlobCollection<T> colBottom)
        {
            long hTopDiff = colTop[0].gpu_diff;
            long hBottomDiff = colBottom[0].mutable_gpu_diff;
            int nCount = colBottom[0].count();

            m_cuda.scale(nCount, m_dfScale, hTopDiff, hBottomDiff);
        }
    }
}
