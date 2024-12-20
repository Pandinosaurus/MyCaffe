﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MyCaffe.basecode;

namespace MyCaffe.param.gpt
{
    /// <summary>
    /// Specifies the parameters for the CausalSelfAttentionLayer.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CausalSelfAttentionParameter : LayerParameterBase
    {
        uint m_nHeads = 6;
        uint m_nEmbed = 192;
        double m_dfAttnDropout;
        double m_dfResidDropout;
        uint m_nBlockSize = 128;
        uint m_nLayers = 6;
        WeightAdapterParameter m_weight_adapter_q = new WeightAdapterParameter("q");
        WeightAdapterParameter m_weight_adapter_k = new WeightAdapterParameter("k");
        WeightAdapterParameter m_weight_adapter_v = new WeightAdapterParameter("v");
        WeightAdapterParameter m_weight_adapter_out = new WeightAdapterParameter("out");
        bool m_bEnableCudaScaledDotProductAttention = false;
        bool m_bEnableRotaryPositionalEmbedding = false;
        int m_nRopeSharedIndex = 1;
        bool m_bEnableKeyValueCache = false;
        bool m_bBiasTerm = true;

        /** @copydoc LayerParameterBase */
        public CausalSelfAttentionParameter()
        {
        }

        /// <summary>
        /// Specifies whether or not to enable the CudaScaledDotProductAttention.  When enabled, the scaled dot product attention is computed at the CUDA level.
        /// </summary>
        /// <remarks>
        /// Currently only supported by the CausalSelfAttentionLayer2.
        /// </remarks>
        [Description("Specifies whether or not to enable the CudaScaledDotProductAttention.  When enabled, the scaled dot product attention is computed at the CUDA level.")]
        public bool enable_cuda_scaled_dot_product_attention
        {
            get { return m_bEnableCudaScaledDotProductAttention; }
            set { m_bEnableCudaScaledDotProductAttention = value; }
        }

        /// <summary>
        /// Specifies whether or not to enable the rotary positional embedding.
        /// </summary>
        [Description("Specifies whether or not to enable the rotary positional embedding.")]
        public bool enable_rotary_positional_embedding
        {
            get { return m_bEnableRotaryPositionalEmbedding; }
            set { m_bEnableRotaryPositionalEmbedding = value; }
        }

        /// <summary>
        /// Specifies the rope shared index so that only one rope is used for all layers. To use a unique rope for each layer, set this value to -1.
        /// </summary>
        [Description("Specifies the rope shared index so that only one rope is used for all layers. To use a unique rope for each layer, set this value to -1.")]
        public int rope_shared_index
        {
            get { return m_nRopeSharedIndex; }
            set { m_nRopeSharedIndex = value; }
        }

        /// <summary>
        /// Specifies whether or not to enable the key value cache, which is used with Llama style models to save memory during inference.
        /// </summary>
        [Description]
        public bool enable_key_value_cache
        {
            get { return m_bEnableKeyValueCache; }
            set { m_bEnableKeyValueCache = value; }
        }

        /// <summary>
        /// Specifies whether or not to use a bias term on wq, wk, wv, and wo.
        /// </summary>
        [Description("Specifies whether or not to use a bias term on wq, wk, wv, and wo.")]
        public bool bias_term
        {
            get { return m_bBiasTerm; }
            set { m_bBiasTerm = value; }
        }

        /// <summary>
        /// The number of layers (transformer blocks) used.
        /// </summary>
        [Description("Specifies number of layers (transformer blocks) used.")]
        public uint layers
        {
            get { return m_nLayers; }
            set { m_nLayers = value; }
        }

        /// <summary>
        /// The number of heads used.
        /// </summary>
        [Description("Specifies number of heads used.")]
        public uint heads
        {
            get { return m_nHeads; }
            set { m_nHeads = value; }
        }

        /// <summary>
        /// Specifies size of the embed.
        /// </summary>
        public uint embed
        {
            get { return m_nEmbed; }
            set { m_nEmbed = value; }
        }

        /// <summary>
        /// Specifies size of the block.
        /// </summary>
        public uint block_size
        {
            get { return m_nBlockSize; }
            set { m_nBlockSize = value; }
        }
        
        /// <summary>
        /// Specifies dropout probability used on the attention weights.
        /// </summary>
        public double attn_dropout
        {
            get { return m_dfAttnDropout; }
            set { m_dfAttnDropout = value; }
        }

        /// <summary>
        /// Specifies dropout probability used on the residual weights.
        /// </summary>
        public double resid_dropout
        {
            get { return m_dfResidDropout; }
            set { m_dfResidDropout = value; }
        }

        /// <summary>
        /// Specifies the weight adapter for the 'q' Linear layer.
        /// </summary>
        /// <remarks>
        /// When using the CausalSelfAttentionLayer, the weight adapter for the 'q' Linear layer is used for the combined Q,K,V Linear layer.  When using the 
        /// CausalSelfAttentionLayer2, the q, k, v WeightAdapters are used for the individual Q, K, V Linear layers of the MultiheadAttentionLayer used.
        /// </remarks>
        [Description("Specifies the weight adapter for the 'q' Linear layer.")]
        public WeightAdapterParameter weight_adapter_q
        {
            get { return m_weight_adapter_q; }
            set { m_weight_adapter_q = value; }
        }

        /// <summary>
        /// Specifies the weight adapter for the 'q' Linear layer.
        /// </summary>
        [Description("Specifies the weight adapter for the 'k' Linear layer.")]
        public WeightAdapterParameter weight_adapter_k
        {
            get { return m_weight_adapter_k; }
            set { m_weight_adapter_k = value; }
        }

        /// <summary>
        /// Specifies the weight adapter for the 'v' Linear layer.
        /// </summary>
        [Description("Specifies the weight adapter for the 'v' Linear layer.")]
        public WeightAdapterParameter weight_adapter_v
        {
            get { return m_weight_adapter_v; }
            set { m_weight_adapter_v = value; }
        }

        /// <summary>
        /// Specifies the weight adapter for the 'out' Linear layer.
        /// </summary>
        [Description("Specifies the weight adapter for the 'out' Linear layer.")]
        public WeightAdapterParameter weight_adapter_out
        {
            get { return m_weight_adapter_out; }
            set { m_weight_adapter_out = value; }
        }

        /** @copydoc LayerParameterBase::Load */
        public override object Load(System.IO.BinaryReader br, bool bNewInstance = true)
        {
            RawProto proto = RawProto.Parse(br.ReadString());
            CausalSelfAttentionParameter p = FromProto(proto);

            if (!bNewInstance)
                Copy(p);

            return p;
        }

        /** @copydoc LayerParameterBase::Copy */
        public override void Copy(LayerParameterBase src)
        {
            CausalSelfAttentionParameter p = (CausalSelfAttentionParameter)src;

            m_nLayers = p.layers;
            m_nHeads = p.heads;
            m_nEmbed = p.embed;
            m_nBlockSize = p.block_size;
            m_dfAttnDropout = p.attn_dropout;
            m_dfResidDropout = p.resid_dropout;
            m_weight_adapter_q = p.weight_adapter_q.Clone();
            m_weight_adapter_k = p.weight_adapter_k.Clone();
            m_weight_adapter_v = p.weight_adapter_v.Clone();
            m_weight_adapter_out = p.weight_adapter_out.Clone();
            m_bEnableCudaScaledDotProductAttention = p.enable_cuda_scaled_dot_product_attention;
            m_bEnableRotaryPositionalEmbedding = p.enable_rotary_positional_embedding;
            m_nRopeSharedIndex = p.rope_shared_index;
            m_bBiasTerm = p.bias_term;
            m_bEnableKeyValueCache = p.enable_key_value_cache;
        }

        /** @copydoc LayerParameterBase::Clone */
        public override LayerParameterBase Clone()
        {
            CausalSelfAttentionParameter p = new CausalSelfAttentionParameter();
            p.Copy(this);
            return p;
        }

        /// <summary>
        /// Convert the parameter into a RawProto.
        /// </summary>
        /// <param name="strName">Specifies the name to associate with the RawProto.</param>
        /// <returns>The new RawProto is returned.</returns>
        public override RawProto ToProto(string strName)
        {
            RawProtoCollection rgChildren = new RawProtoCollection();

            rgChildren.Add("layers", layers.ToString());
            rgChildren.Add("heads", heads.ToString());
            rgChildren.Add("embed", embed.ToString());
            rgChildren.Add("block_size", block_size.ToString());
            rgChildren.Add("attn_dropout", attn_dropout.ToString());
            rgChildren.Add("resid_dropout", resid_dropout.ToString());
            rgChildren.Add(weight_adapter_q.ToProto("weight_adapter_q"));
            rgChildren.Add(weight_adapter_k.ToProto("weight_adapter_k"));
            rgChildren.Add(weight_adapter_v.ToProto("weight_adapter_v"));
            rgChildren.Add(weight_adapter_out.ToProto("weight_adapter_out"));
            rgChildren.Add("enable_cuda_scaled_dot_product_attention", enable_cuda_scaled_dot_product_attention.ToString());
            rgChildren.Add("enable_rotary_positional_embedding", enable_rotary_positional_embedding.ToString());
            rgChildren.Add("rope_shared_index", rope_shared_index.ToString());
            rgChildren.Add("bias_term", bias_term.ToString());
            rgChildren.Add("enable_key_value_cache", enable_key_value_cache.ToString());

            return new RawProto(strName, "", rgChildren);
        }

        /// <summary>
        /// Parses the parameter from a RawProto.
        /// </summary>
        /// <param name="rp">Specifies the RawProto to parse.</param>
        /// <returns>A new instance of the parameter is returned.</returns>
        public static CausalSelfAttentionParameter FromProto(RawProto rp)
        {
            string strVal;
            CausalSelfAttentionParameter p = new CausalSelfAttentionParameter();

            if ((strVal = rp.FindValue("layers")) != null)
                p.layers = uint.Parse(strVal);
            
            if ((strVal = rp.FindValue("heads")) != null)
                p.heads = uint.Parse(strVal);
            
            if ((strVal = rp.FindValue("embed")) != null)
                p.embed = uint.Parse(strVal);

            if ((strVal = rp.FindValue("block_size")) != null)
                p.block_size = uint.Parse(strVal);

            if ((strVal = rp.FindValue("attn_dropout")) != null)
                p.attn_dropout = double.Parse(strVal);

            if ((strVal = rp.FindValue("resid_dropout")) != null)
                p.resid_dropout = double.Parse(strVal);

            RawProto rp1 = rp.FindChild("weight_adapter_q");
            if (rp1 != null)
                p.weight_adapter_q = WeightAdapterParameter.FromProto(rp1);

            rp1 = rp.FindChild("weight_adapter_k");
            if (rp1 != null)
                p.weight_adapter_k = WeightAdapterParameter.FromProto(rp1);

            rp1 = rp.FindChild("weight_adapter_v");
            if (rp1 != null)
                p.weight_adapter_v = WeightAdapterParameter.FromProto(rp1);

            rp1 = rp.FindChild("weight_adapter_out");
            if (rp1 != null)
                p.weight_adapter_out = WeightAdapterParameter.FromProto(rp1);

            if ((strVal = rp.FindValue("enable_cuda_scaled_dot_product_attention")) != null)
                p.enable_cuda_scaled_dot_product_attention = bool.Parse(strVal);

            if ((strVal = rp.FindValue("enable_rotary_positional_embedding")) != null)
                p.enable_rotary_positional_embedding = bool.Parse(strVal);

            if ((strVal = rp.FindValue("rope_shared_index")) != null)
                p.rope_shared_index = int.Parse(strVal);

            if ((strVal = rp.FindValue("bias_term")) != null)
                p.bias_term = bool.Parse(strVal);

            if ((strVal = rp.FindValue("enable_key_value_cache")) != null)
                p.enable_key_value_cache = bool.Parse(strVal);

            return p;
        }
    }
}
