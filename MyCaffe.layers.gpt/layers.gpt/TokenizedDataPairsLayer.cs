﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCaffe.basecode;
using MyCaffe.common;
using MyCaffe.param;
using MyCaffe.fillers;
using System.IO;
using MyCaffe.db.image;
using MyCaffe.param.gpt;
using System.Net;
using System.Globalization;
using System.Diagnostics;
using System.Xml.Linq;

namespace MyCaffe.layers.gpt
{
    /// <summary>
    /// The TokenizedDataPairsLayer loads and tokenizes data for a transformer model where data is loaded in the form: data, pos, target(optional)
    /// </summary>
    /// <typeparam name="T">Specifies the base type <i>float</i> or <i>double</i>.  Using <i>float</i> is recommended to conserve GPU memory.</typeparam>
    public class TokenizedDataPairsLayer<T> : Layer<T>
    {
        CancelEvent m_evtCancel;
        InputData m_encoderData;
        InputData m_decoderData;
        Blob<T> m_blobX = null;
        Blob<T> m_blobY = null;
        Blob<T> m_blobTriangle = null;
        Random m_random = new Random();

        /// <summary>
        /// Defines the input source.
        /// </summary>
        public enum VOCABULARY
        {
            /// <summary>
            /// Specifies the encoder input source.
            /// </summary>
            ENCODER,
            /// <summary>
            /// Specifies the decoder input source.
            /// </summary>
            DECODER
        }

        /// <summary>
        /// The TokenizedDataPairsLayer constructor.
        /// </summary>
        /// <param name="cuda">Specifies the CudaDnn connection to Cuda.</param>
        /// <param name="log">Specifies the Log for output.</param>
        /// <param name="p">
        /// Provides TokenizedDataPairsParameter model_data_param with options:
        ///  - source.  The encoder input data source.
        /// 
        ///  - target.  The decoder input/output data source.
        /// 
        ///  - batch_size.  The batch size
        ///  
        ///  - time_steps.  The maximum number of time steps.
        ///  
        ///  - input_dim.  The input dimension of the encoder input.
        ///  
        ///  - sample_size.  The number of samples to load for training.
        ///  
        ///  - shuffle.  Whether or not to shuffle the data.
        /// </param>
        /// <param name="db">Specifies the external database to use.</param>
        /// <param name="evtCancel">Specifies the CancelEvent used to cancel any pre-fetching operations.</param>
        public TokenizedDataPairsLayer(CudaDnn<T> cuda, Log log, LayerParameter p, IXImageDatabaseBase db, CancelEvent evtCancel)
            : base(cuda, log, p)
        {
            m_evtCancel = evtCancel;
            m_type = LayerParameter.LayerType.TOKENIZED_DATA_PAIRS;
        }

        /// <summary>
        /// Release all internal blobs.
        /// </summary>
        protected override void dispose()
        {
            dispose(ref m_blobY);
            dispose(ref m_blobX);
            dispose(ref m_blobTriangle);

            base.dispose();
        }

        /// <summary>
        /// No bottom blobs for the data layer, except when running.
        /// </summary>
        public override int MaxBottomBlobs
        {
            get { return (m_phase == Phase.RUN) ? 1 : 0; }
        }

        /// <summary>
        /// Returns the minimum number of bottom blobs.
        /// </summary>
        public override int MinBottomBlobs
        {
            get { return 0; }
        }

        /// <summary>
        /// Returns the minimum number of required top (output) Blobs: enc_in, dec_in, dec_out, e_mask, d_mask
        /// </summary>
        public override int ExactNumTopBlobs
        {
            get { return 5; }
        }

        /// <summary>
        /// Setup the layer.
        /// </summary>
        /// <param name="colBottom">Not used.</param>
        /// <param name="colTop">Specifies the collection of top (output) Blobs.</param>
        public override void LayerSetUp(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            switch (m_param.tokenized_data_pairs_param.input_type)
            {
                case TokenizedDataParameter.INPUT_TYPE.TEXT_FILE:
                    m_encoderData = new TextListData(m_log, m_param.tokenized_data_pairs_param.source, m_param.tokenized_data_pairs_param.source_vocab_file, false, m_param.tokenized_data_pairs_param.vocabulary_type, m_param.tokenized_data_pairs_param.seed, m_param.phase);
                    m_decoderData = new TextListData(m_log, m_param.tokenized_data_pairs_param.target, m_param.tokenized_data_pairs_param.target_vocab_file, true, m_param.tokenized_data_pairs_param.vocabulary_type, m_param.tokenized_data_pairs_param.seed, m_param.phase);
                    m_log.WriteLine("Encoder Vocabulary: " + m_encoderData.VocabularySize.ToString());
                    m_log.WriteLine("Decoder Vocabulary: " + m_decoderData.VocabularySize.ToString());
                    break;

                default:
                    throw new Exception("Unknown input type '" + m_param.tokenized_data_pairs_param.input_type.ToString() + "'");
            }
        }

        /// <summary>
        /// Reshape the top based on the parameter batch and block size.
        /// </summary>
        /// <param name="colBottom">Specifies the collection of bottom (input) Blobs - Used only during RUN phase.</param>
        /// <param name="colTop">Specifies the collection of top (output) Blobs.</param>
        public override void Reshape(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            if (m_phase == Phase.RUN)
            {
                int nBatchSize = 1;
                int nBlockSize = (int)m_param.tokenized_data_pairs_param.block_size;
                int nTokenSize = (int)m_encoderData.TokenSize;

                Blob<T> blobEncIn = colTop[0];
                Blob<T> blobDecIn = colTop[1];
                Blob<T> blobDecOut = colTop[2];
                Blob<T> blobEncMask = colTop[3];
                Blob<T> blobDecMask = colTop[4];

                int nCount = 3;
                if (nTokenSize == 1)
                    nCount = 2;
                int[] rgShape = new int[nCount];
                
                blobEncIn.SetParameter("vocab_size", m_encoderData.VocabularySize);
                blobDecIn.SetParameter("vocab_size", m_decoderData.VocabularySize);

                // reshape for single characters (each character is an index into the vocab vector)
                rgShape[0] = nBatchSize;
                rgShape[1] = nBlockSize;
                if (rgShape.Length > 2)
                    rgShape[2] = nTokenSize;
                
                blobEncIn.Reshape(rgShape);
                blobDecIn.Reshape(rgShape);
                blobDecOut.Reshape(rgShape);
                blobEncMask.Reshape(nBatchSize, nBlockSize, 1, 1);
                blobDecMask.Reshape(nBatchSize, 1, 1, 1);
            }
            else
            {
                m_log.CHECK_EQ(colBottom.Count, 0, "Data Layer takes no input blobs.");
                m_log.CHECK_EQ(colTop.Count, 5, "The TokenizedDataPairsLayer requires 5 top blobs.");

                int nBatchSize = (int)m_param.tokenized_data_pairs_param.batch_size;
                int nBlockSize = (int)m_param.tokenized_data_pairs_param.block_size;
                int nTokenSize = (int)m_encoderData.TokenSize;

                Blob<T> blobEncIn = colTop[0];
                Blob<T> blobDecIn = colTop[1];
                Blob<T> blobDecOut = colTop[2];
                Blob<T> blobEncMask = colTop[3];
                Blob<T> blobDecMask = colTop[4];

                int nCount = 3;
                if (nTokenSize == 1)
                    nCount = 2;
                int[] rgShape = new int[nCount];

                blobEncIn.SetParameter("vocab_size", m_encoderData.VocabularySize);
                blobDecIn.SetParameter("vocab_size", m_decoderData.VocabularySize);
                // reshape for single characters (each character is an index into the vocab vector)
                rgShape[0] = nBatchSize;
                rgShape[1] = nBlockSize;
                if (rgShape.Length > 2)
                    rgShape[2] = nTokenSize;

                blobEncIn.Reshape(rgShape);
                blobDecIn.Reshape(rgShape);
                blobDecOut.Reshape(rgShape);
                blobEncMask.Reshape(nBatchSize, nBlockSize, 1, 1);
                blobDecMask.Reshape(nBatchSize, nBlockSize, nBlockSize, 1);

                if (m_blobTriangle == null)
                    m_blobTriangle = new Blob<T>(m_cuda, m_log, false);

                if (!m_blobTriangle.CompareShape(blobDecMask.shape()))
                {
                    m_blobTriangle.ReshapeLike(blobDecMask);

                    T[] rgMask = new T[m_blobTriangle.count()];
                    for (int n = 0; n < m_blobTriangle.num; n++)
                    {
                        for (int c = 0; c < m_blobTriangle.channels; c++)
                        {
                            for (int h = 0; h < m_blobTriangle.height; h++)
                            {
                                int nIdx = n * nBlockSize * nBlockSize + c * nBlockSize + h;
                                rgMask[nIdx] = (h > c) ? m_tZero : m_tOne;
                            }
                        }
                    }

                    m_blobTriangle.mutable_cpu_data = rgMask;
                }
            }
        }

        /// <summary>
        /// Run the Forward computation, which fills the data into the top (output) Blobs.
        /// </summary>
        /// <param name="colBottom">Not used.</param>
        /// <param name="colTop">top output blob vector (length 2-5)
        ///  -# @f$ (N \times C \times 1 \times 1) @f$  (TEXT input)
        ///     the data outputs.  
        ///  -# @f$ (N \times C \times 1 \times 1) @f$
        ///     the position outputs.
        ///  -# @f$ (N \times C \times 1 \times 1) @f$ (TEXT input, only on training and testing)
        ///     the target outputs
        /// ENCODER mask
        ///  -# @f$ (N \times C \times 1 \times 1) @f$ (1's on each input, otherwise 0's)
        ///     the encoder mask.
        /// DECODER mask
        ///  -# @f$ (N \times C \times C \times 1) @f$ (1's on each input, otherwise 0's. Duplicated across C channels, overlaid with triangle mask - see remarks.)
        ///     the encoder mask.
        /// </param>
        /// <remarks>
        /// The encoder and decoder masks use the following formats.
        /// 
        /// Encoder Mask:
        /// shape = (batch, seq_len, 1)
        /// The sequence length is filled with 1 where data exists in each sequence, and
        /// 0 otherwise.  For example, when using a sequence length of 4 and batch = 3, 
        /// the following input:
        /// <code>
        ///  encoder input                encoder mask
        ///  shape = (3,4)                (3,4)
        ///  [33, 44, 22, 55]             [  1,  1,  1,  1]
        ///  [44, 33, 0,  0 ] has mask -> [  1,  1,  0,  0]
        ///  [88, 99, 22, 0 ]             [  1,  1,  1,  0]
        /// </code>
        /// 
        /// Decoder Mask:
        /// shape (batch, seq_len, seq_len)
        /// The decoder mask is first filled with a mask similar to the encoder mask, whre each
        /// sequence for each entry is duplicated for the number of sequences high to create an
        /// initial mask like the following. Next a triangular mask is anded to avoid right side info.
        /// <code>
        ///  decoder input                encoder like mask        triangular mask     final decoder mask
        ///  shape = (3,4)                (3,4,4)                  (3,4,4)             (3,4,4)
        ///  [33, 44, 22, 55]             [  1,  1,  1,  1]        [  1,  0,  0,  0]   [  1,  0,  0,  0]
        ///                               [  1,  1,  1,  1]        [  1,  1,  0,  0]   [  1,  1,  0,  0]
        ///                               [  1,  1,  1,  1] -and-> [  1,  1,  1,  0] = [  1,  1,  1,  0]
        ///                               [  1,  1,  1,  1]        [  1,  1,  1,  1]   [  1,  1,  1,  1]
        ///  [44, 33, 0,  0 ] has mask -> [  1,  1,  0,  0]        [  1,  0,  0,  0]   [  1,  0,  0,  0]
        ///                               [  1,  1,  0,  0]        [  1,  1,  0,  0]   [  1,  1,  0,  0]
        ///                               [  1,  1,  0,  0] -and-> [  1,  1,  1,  0] = [  1,  1,  0,  0]
        ///                               [  1,  1,  0,  0]        [  1,  1,  1,  1]   [  1,  1,  0,  0]
        ///  [88, 99, 22, 0 ]             [  1,  1,  1,  0]        [  1,  0,  0,  0]   [  1,  0,  0,  0]
        ///                               [  1,  1,  1,  0]        [  1,  1,  0,  0]   [  1,  1,  0,  0]
        ///                               [  1,  1,  1,  0] -and-> [  1,  1,  1,  0] = [  1,  1,  1,  0]
        ///                               [  1,  1,  1,  0]        [  1,  1,  1,  1]   [  1,  1,  1,  0]
        /// </code>                              
        /// </remarks>
        protected override void forward(BlobCollection<T> colBottom, BlobCollection<T> colTop)
        {
            if (m_phase == Phase.RUN)
            {
                m_log.CHECK_EQ(colBottom.Count, 1, "There must be one input blob when running.");

                colTop[0].CopyFrom(colBottom[0]);

                float[] rgDecIn = new float[1] { m_decoderData.BOS };
                colTop[1].mutable_cpu_data = convert(rgDecIn);

                // colTop[2] NO Dec target data when running.

                float[] rgEncMask = new float[1] { 1 };
                colTop[3].mutable_cpu_data = convert(rgEncMask);

                float[] rgDecMask = new float[1] { 1 };
                colTop[4].mutable_cpu_data = convert(rgDecMask);
            }
            else
            {
                int[] rgnIdx;
                Tuple<float[], float[]> encData = m_encoderData.GetData((int)m_param.tokenized_data_pairs_param.batch_size, (int)m_param.tokenized_data_pairs_param.block_size, m_decoderData, out rgnIdx);
                Tuple<float[], float[]> decData = m_decoderData.GetDataAt((int)m_param.tokenized_data_pairs_param.batch_size, (int)m_param.tokenized_data_pairs_param.block_size, rgnIdx);

                float fMin1 = encData.Item1.Min();
                float fMax1 = encData.Item1.Max();

                float fMin2 = decData.Item1.Min();
                float fMax2 = decData.Item1.Max();

                colTop[0].mutable_cpu_data = convert(encData.Item1);
                colTop[1].mutable_cpu_data = convert(decData.Item1);
                colTop[2].mutable_cpu_data = convert(decData.Item2);
                m_cuda.sign(colTop[0].count(), colTop[0].gpu_data, colTop[3].mutable_gpu_data);
                m_cuda.sign(colTop[1].count(), colTop[1].gpu_data, colTop[4].mutable_gpu_data);
                m_cuda.mul(colTop[4].count(), colTop[4].gpu_data, m_blobTriangle.gpu_data, colTop[4].mutable_gpu_data);
            }
        }

        /// @brief Not implemented - data Layers do not perform backward..
        protected override void backward(BlobCollection<T> colTop, List<bool> rgbPropagateDown, BlobCollection<T> colBottom)
        {
        }

        /// <summary>
        /// Specifies that this layer supports preprocessing.
        /// </summary>
        public override bool SupportsPreProcessing
        {
            get { return true; }
        }

        /// <summary>
        /// Specifies that this layer supports post processing the logits.
        /// </summary>
        public override bool SupportsPostProcessingLogits
        {
            get { return true; }
        }

        /// <summary>
        /// Tokenize an input string using the internal vocabulary.
        /// </summary>
        /// <param name="str">Specifies the string to tokenize.</param>
        /// <param name="vocab">Specifies the vocabulary to use, ENCODER or DECODER.</param>
        /// <returns>A list of tokens corresponding to the input is returned.</returns>
        public List<int> Tokenize(string str, VOCABULARY vocab)
        {
            if (vocab == VOCABULARY.ENCODER)
                return m_encoderData.Tokenize(str);
            else
                return m_decoderData.Tokenize(str);
        }

        /// <summary>
        /// Detokenize a set of tokens from the data specified.
        /// </summary>
        /// <param name="rg">Specifies an array of tokens.</param>
        /// <param name="nStartIdx">Specifies the start index.</param>
        /// <param name="nCount">Specifies the number of tokens to detokenize.</param>
        /// <param name="vocab">Specifies the vocabulary to use: ENCODER or DECODER.</param>
        /// <returns>The detokenized string is returned.</returns>
        public string Detokenize(float[] rg, int nStartIdx, int nCount, VOCABULARY vocab)
        {
            InputData inputData = (vocab == VOCABULARY.ENCODER) ? m_encoderData : m_decoderData;
            return inputData.Detokenize(rg, nStartIdx, nCount, false, false);
        }

        /// <summary>
        /// Get the vocabulary size for the specified vocabulary source.
        /// </summary>
        /// <param name="src">Specifies the vocabulary source (ENCODER or DECODER).</param>
        /// <returns>The vocabulary size is returned.</returns>
        public uint GetVocabuarySize(VOCABULARY src)
        {
            InputData input = (src == VOCABULARY.ENCODER) ? m_encoderData : m_decoderData;
            return input.VocabularySize;
        }

        /// <summary>
        /// Preproces the input and return as a set of bottom blobs.
        /// </summary>
        /// <param name="customInput">Specifies the custom text input.</param>
        /// <param name="colBottom">The output is placed in the bottom blobs as: tokidx, pos</param>
        /// <returns>The bottom blob collection is returned.</returns>
        public override BlobCollection<T> PreProcessInput(PropertySet customInput, BlobCollection<T> colBottom = null)
        {
            Blob<T> blobIdx = new Blob<T>(m_cuda, m_log, false);

            string strInput = customInput.GetProperty("InputData");
            if (string.IsNullOrEmpty(strInput))
                throw new Exception("Could not find 'InputData' property!");

            int[] rgShape = new int[2];
            rgShape[0] = 1;
            rgShape[1] = strInput.Length;

            blobIdx.Reshape(rgShape);

            List<int> rgTokens = m_encoderData.Tokenize(strInput);
            float[] rgInput = new float[rgTokens.Count];

            for (int i = 0; i < strInput.Length; i++)
            {
                rgInput[i] = rgTokens[i];
            }

            blobIdx.mutable_cpu_data = convert(rgInput);

            return new BlobCollection<T>() { blobIdx };
        }

        /// <summary>
        /// Preproces the input and return as a set of bottom blobs.
        /// </summary>
        /// <param name="str">Specifies the string input, can be null.</param>
        /// <param name="nTokIdx">Specifies the token input.</param>
        /// <param name="colBottom">The output is placed in the bottom blobs as: tokidx, pos</param>
        /// <returns>The bottom blob collection is returned.</returns>
        public override void PreProcessInput(string str, int? nTokIdx, BlobCollection<T> colBottom = null)
        {
            List<float> rgTok = convertF(colBottom[0].mutable_cpu_data).ToList();

            rgTok.Add(nTokIdx.Value);
            if (rgTok.Count > m_param.tokenized_data_pairs_param.block_size)
                rgTok.RemoveAt(0);

            List<int> rgShape = Utility.Clone<int>(colBottom[0].shape());
            rgShape[1] = rgTok.Count;
            colBottom[0].Reshape(rgShape);
            
            colBottom[0].mutable_cpu_data = convert(rgTok.ToArray());
        }

        /// <summary>
        /// Allows post processing the logits output data by converting the logits to and selecting 
        /// from the probability distribution produced and detokenizing the results to the string character.
        /// </summary>
        /// <param name="blobLogits">Specifies the output of the last inner product layer.</param>
        /// <param name="softmax">Specifies the softmax layer.</param>
        /// <param name="nK">Specifies the TopK max items of the logits to use, or 0 to ignore.</param>
        /// <returns>
        /// The detokenized data is returned.
        /// </returns>
        public override List<Tuple<string, int, double>> PostProcessLogitsOutput(Blob<T> blobLogits, Layer<T> softmax, int nK = 1)
        {
            float[] rgData = convertF(blobLogits.mutable_cpu_data);
            int nVocabCount = blobLogits.count(softmax.layer_param.softmax_param.axis);
            float[] rgLogits = new float[nVocabCount];
            int nIdxStart = blobLogits.count() - nVocabCount;
            Dictionary<int, float> rgTopK = new Dictionary<int, float>();

            for (int i = nIdxStart; i < blobLogits.count(); i++)
            {
                float fVal = rgData[i];
                rgTopK.Add(i - nIdxStart, fVal);

                if (rgTopK.Count > nK)
                {
                    float fMin = float.MaxValue;
                    int nMinIdx = -1;

                    foreach (KeyValuePair<int, float> kv in rgTopK)
                    {
                        if (kv.Value < fMin)
                        {
                            fMin = kv.Value;
                            nMinIdx = kv.Key;
                        }
                    }

                    rgTopK.Remove(nMinIdx);
                }
            }

            for (int i = 0; i < rgLogits.Count(); i++)
            {
                if (rgTopK.ContainsKey(i))
                    rgLogits[i] = rgTopK[i];
                else
                    rgLogits[i] = -float.MaxValue;
            }

            if (m_blobX == null)
                m_blobX = new Blob<T>(m_cuda, m_log, false);
            if (m_blobY == null)
                m_blobY = new Blob<T>(m_cuda, m_log, false);

            m_blobX.Reshape(1, 1, nVocabCount, 1);
            m_blobX.mutable_cpu_data = convert(rgLogits);

            BlobCollection<T> colBottom = new BlobCollection<T>() { m_blobX };
            BlobCollection<T> colTop = new BlobCollection<T>() { m_blobY };
            softmax.Forward(colBottom, colTop);

            float[] rgProb = convertF(m_blobY.mutable_cpu_data);
            float fRand = (float)m_random.NextDouble();
            float fTotal = 0;
            int nCharIdx = rgProb.Length - 1;

            for (int i = 0; i < rgProb.Length; i++)
            {
                fTotal += rgProb[i];

                if (fTotal >= fRand)
                {
                    nCharIdx = i;
                    break;
                }
            }

            string str = "";
            str += m_decoderData.Detokenize(nCharIdx, true, true);

            return new List<Tuple<string, int, double>>() { new Tuple<string, int, double>(str, nCharIdx, 0) };
        }
    }

    /// <summary>
    /// The TextListData manages parallel lists of data where the first list contains the encoder input data and the second the decoder input/target data.
    /// </summary>
    public class TextListData : InputData
    {
        List<string> m_rgstrData = new List<string>();
        List<Tuple<int[], int[]>> m_rgnData = new List<Tuple<int[], int[]>>();
        IVocabulary m_vocab;
        float[] m_rgData = null;
        float[] m_rgTgt = null;
        Phase m_phase;
        Log m_log;

        /// <summary>
        /// Defines the vocabulary time to use.
        /// </summary>
        public enum VOCABUARY_TYPE
        {
            /// <summary>
            /// Specifies a character vocabulary.
            /// </summary>
            CHARACTER,
            /// <summary>
            /// Specifies a space separated word vocabulary.
            /// </summary>
            WORD
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="log">Specifies the output log.</param>
        /// <param name="strSrcFile">Specifies the text file name for the data source.</param>"
        /// <param name="strVocabFile">Specifies the vocabulary file (used by SENTENCEPICE type).</param>
        /// <param name="bIncludeTarget">Specifies to create the target tokens.</param>
        /// <param name="vocabType">Specifies the vocabulary type to use.</param>
        /// <param name="nRandomSeed">Optionally, specifies a random seed for testing.</param>
        /// <param name="phase">Specifies the currently running phase.</param>
        public TextListData(Log log, string strSrcFile, string strVocabFile, bool bIncludeTarget, TokenizedDataParameter.VOCABULARY_TYPE vocabType, int? nRandomSeed = null, Phase phase = Phase.NONE) : base(nRandomSeed)
        {
            m_log = log;
            m_phase = phase;

            Stopwatch sw = new Stopwatch();

            if (vocabType == TokenizedDataParameter.VOCABULARY_TYPE.WORD)
                m_vocab = new VocabularyWord(m_random, true, true);
            else if (vocabType == TokenizedDataParameter.VOCABULARY_TYPE.SENTENCEPIECE)
                m_vocab = new VocabularySentencePiece(m_random, true, true, strVocabFile);
            else
                m_vocab = new VocabularyCharacter(m_random, true, true);

            string strProgData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            strSrcFile = Utility.ReplaceMacro(strSrcFile, "$ProgramData$", strProgData);

            string[] rgstr = File.ReadAllLines(strSrcFile);

            sw.Start();

            for (int i = 0; i < rgstr.Length; i++)
            {
                m_rgstrData.Add(rgstr[i]);
                m_vocab.Add(rgstr[i]);

                if (sw.Elapsed.TotalMilliseconds > 1000)
                {
                    sw.Restart();
                    m_log.Progress = (double)i / rgstr.Length;
                    m_log.WriteLine("Loading vocabulary " + i.ToString("N0") + " of " + rgstr.Length.ToString("N0") + "...", true);
                }
            }

            m_vocab.Build();
            
            for (int i = 0; i < m_rgstrData.Count; i++)
            {
                string str = m_rgstrData[i];
                int[] rgnSrc = m_vocab.Tokenize(str, bIncludeTarget, !bIncludeTarget);
                int[] rgnTrg = null;

                if (bIncludeTarget)
                    rgnTrg = m_vocab.CreateTarget(rgnSrc);

                m_rgnData.Add(new Tuple<int[], int[]>(rgnSrc, rgnTrg));

                if (sw.Elapsed.TotalMilliseconds > 1000)
                {
                    sw.Restart();
                    m_log.Progress = (double)i / m_rgstrData.Count;
                    m_log.WriteLine("Tokenizing data " + i.ToString("N0") + " of " + m_rgstrData.Count.ToString("N0") + "...", true);
                }
            }
        }

        /// <summary>
        /// Return the raw data.
        /// </summary>
        public override List<string> RawData
        {
            get { return m_rgstrData; }
        }

        /// <summary>
        /// The text data token size is a single character.
        /// </summary>
        public override uint TokenSize
        {
            get { return 1; }
        }

        /// <summary>
        /// Returns the number of unique characters in the data.
        /// </summary>
        public override uint VocabularySize
        {
            get { return (uint)m_vocab.Count; }
        }

        /// <summary>
        /// Returns true if data is available at the given index.
        /// </summary>
        /// <param name="nIdx">Specifies the index to check</param>
        /// <param name="bIncludeSrc">Specifies to include the source in the check.</param>
        /// <param name="bIncludeTrg">Specifies to include the target in the check.</param>
        /// <returns>If the data is available, true is returned.</returns>
        public override bool GetDataAvailabilityAt(int nIdx, bool bIncludeSrc, bool bIncludeTrg)
        {
            if (bIncludeSrc && m_rgnData[nIdx].Item1.Length == 0)
                return false;

            if (bIncludeTrg && m_rgnData[nIdx].Item2.Length == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Retrieve random blocks from the source data where the data and target are the same
        /// but offset by one element where the target is offset +1 from the data.
        /// </summary>
        /// <param name="nBatchSize">Specifies the batch size.</param>
        /// <param name="nBlockSize">Specifies teh block size.</param>
        /// <param name="rgnIdx">Returns an array of the indexes of the data returned.</param>
        /// <returns>A tuple containing the data and target is returned.</returns>
        public override Tuple<float[], float[]> GetData(int nBatchSize, int nBlockSize, InputData trgData, out int[] rgnIdx)
        {
            int nSize = nBatchSize * nBlockSize;

            if (m_rgData == null || m_rgData.Length != nSize)
                m_rgData = new float[nSize];
            else
                Array.Clear(m_rgData, 0, m_rgData.Length);

            if (m_rgTgt == null || m_rgTgt.Length != nSize)
                m_rgTgt = new float[nSize];
            else
                Array.Clear(m_rgTgt, 0, m_rgTgt.Length);

            rgnIdx = new int[nBatchSize];

            for (int i = 0; i < nBatchSize; i++)
            {
                int nDataIdx = m_random.Next(m_rgnData.Count);
                int[] rgSrc = m_rgnData[nDataIdx].Item1;
                int nRetryCount = 0;

                while (rgSrc.Length == 0 || !trgData.GetDataAvailabilityAt(nDataIdx, true, true))
                {
                    nDataIdx = m_random.Next(m_rgnData.Count);
                    rgSrc = m_rgnData[nDataIdx].Item1;

                    nRetryCount++;
                    if (nRetryCount > 20 && (rgSrc.Length == 0 || !trgData.GetDataAvailabilityAt(nDataIdx, true, true)))
                        throw new Exception("Could not find a non-empty source data item!");
                }

                int[] rgTrg = m_rgnData[nDataIdx].Item2;
                int nDstIdx = i * nBlockSize;

                rgnIdx[i] = nDataIdx;


                for (int j = 0; j < nBlockSize; j++)
                {
                    if (j < rgSrc.Length)
                        m_rgData[nDstIdx + j] = rgSrc[j];

                    if (rgTrg != null && j < rgTrg.Length)
                        m_rgTgt[nDstIdx + j] = rgTrg[j];
                }

                if (rgTrg != null &&
                    rgTrg[rgTrg.Length - 1] == EOS &&
                    m_rgTgt[nDstIdx + nBlockSize - 1] != 0 &&
                    m_rgTgt[nDstIdx + nBlockSize - 1] != EOS)
                    m_rgTgt[nDstIdx + nBlockSize - 1] = EOS;

                if (rgSrc[rgSrc.Length - 1] == EOS &&
                    m_rgData[nDstIdx + nBlockSize - 1] != 0 &&
                    m_rgData[nDstIdx + nBlockSize - 1] != EOS)
                    m_rgData[nDstIdx + nBlockSize - 1] = EOS;
            }

            return new Tuple<float[], float[]>(m_rgData, m_rgTgt);
        }

        /// <summary>
        /// Fill a batch of data from a specified array of indexes.
        /// </summary>
        /// <param name="nBatchSize">Specifies the number of blocks in the batch.</param>
        /// <param name="nBlockSize">Specifies the size of each block.</param>
        /// <param name="rgnIdx">Specifies the array of indexes to the data to be retrieved.</param>
        /// <returns>A tuple containing the data and target is returned.</returns>
        public override Tuple<float[], float[]> GetDataAt(int nBatchSize, int nBlockSize, int[] rgnIdx)
        {
            int nSize = nBatchSize * nBlockSize;

            if (m_rgData == null || m_rgData.Length != nSize)
                m_rgData = new float[nSize];
            else
                Array.Clear(m_rgData, 0, m_rgData.Length);

            if (m_rgTgt == null || m_rgTgt.Length != nSize)
                m_rgTgt = new float[nSize];
            else
                Array.Clear(m_rgTgt, 0, m_rgTgt.Length);

            for (int i = 0; i < rgnIdx.Length; i++)
            {
                int nDataIdx = rgnIdx[i];
                int nDstIdx = i * nBlockSize;

                int[] rgSrc = m_rgnData[nDataIdx].Item1;
                int[] rgTrg = m_rgnData[nDataIdx].Item2;

                for (int j = 0; j < nBlockSize; j++)
                {
                    if (j < rgSrc.Length)
                        m_rgData[nDstIdx + j] = rgSrc[j];

                    if (j < rgTrg.Length && rgTrg != null)
                        m_rgTgt[nDstIdx + j] = rgTrg[j];
                }

                if (rgTrg != null &&
                    rgTrg[rgTrg.Length - 1] == EOS &&
                    m_rgTgt[nDstIdx + nBlockSize - 1] != 0 &&
                    m_rgTgt[nDstIdx + nBlockSize - 1] != EOS)
                    m_rgTgt[nDstIdx + nBlockSize - 1] = EOS;

                if (rgSrc[rgSrc.Length - 1] == EOS &&
                    m_rgData[nDstIdx + nBlockSize - 1] != 0 &&
                    m_rgData[nDstIdx + nBlockSize - 1] != EOS)
                    m_rgData[nDstIdx + nBlockSize - 1] = EOS;
            }

            return new Tuple<float[], float[]>(m_rgData, m_rgTgt);
        }

        /// <summary>
        /// Tokenize an input string using the internal vocabulary.
        /// </summary>
        /// <param name="str">Specifies the string to tokenize.</param>
        /// <returns>A list of tokens corresponding to the input is returned.</returns>
        public override List<int> Tokenize(string str)
        {
            return m_vocab.Tokenize(str);
        }

        /// <summary>
        /// Detokenize an array into a string.
        /// </summary>
        /// <param name="rgfTokIdx">Specifies the array of tokens to detokenize.</param>
        /// <param name="nStartIdx">Specifies the starting index where detokenizing begins.</param>
        /// <param name="nCount">Specifies the number of tokens to detokenize.</param>
        /// <param name="bIgnoreBos">Specifies to ignore the BOS token.</param>
        /// <param name="bIgnoreEos">Specifies to ignore the EOS token.</param>
        /// <returns>The detokenized string is returned.</returns>
        public override string Detokenize(float[] rgfTokIdx, int nStartIdx, int nCount, bool bIgnoreBos, bool bIgnoreEos)
        {
            string str = "";
            for (int i=nStartIdx; i<nStartIdx + nCount; i++)
            {
                string strItem = m_vocab.Detokenize((int)rgfTokIdx[i], bIgnoreBos, bIgnoreEos);
                if (string.IsNullOrEmpty(strItem))
                    break;

                str += strItem + " ";
            }

            return str.TrimEnd(' ');
        }

        /// <summary>
        /// Detokenize a single token.
        /// </summary>
        /// <param name="nTokIdx">Specifies an index to the token to be detokenized.</param>
        /// <param name="bIgnoreBos">Specifies to ignore the BOS token.</param>
        /// <param name="bIgnoreEos">Specifies to ignore the EOS token.</param>
        /// <returns>The detokenized character is returned.</returns>
        public override string Detokenize(int nTokIdx, bool bIgnoreBos, bool bIgnoreEos)
        {
            return m_vocab.Detokenize(nTokIdx, bIgnoreBos, bIgnoreEos);
        }

        /// <summary>
        /// Return the special begin of sequence character.
        /// </summary>
        public override char BOS
        {
            get { return m_vocab.BOS; }
        }

        /// <summary>
        /// Return the special end of sequence character.
        /// </summary>
        public override char EOS
        {
            get { return m_vocab.EOS; }
        }
    }
}