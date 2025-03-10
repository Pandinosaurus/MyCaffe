﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using MyCaffe.basecode;

namespace MyCaffe.param
{
    /// <summary>
    /// Specifies the parameters for the PairwiseLossLayer.
    /// </summary>
    /// <remarks>
    /// References:
    /// @see [Deep Portfolio Management Using Deep Learning](https://arxiv.org/abs/2405.01604) by Ashish Anil Pawar, Vishnureddy Prashant Muskawar, Ritesh Tiku., 2024.
    /// The paper proposes a Deep Reinforcement Learning (DRL) approach for portfolio management that directly learns to allocate weights to different assets 
    /// (both long and short positions) and shows it can outperform traditional portfolio management methods in terms of risk-adjusted returns.
    /// 
    /// @see [Deep Learning for Portfolio Optimization](https://arxiv.org/abs/2005.13665) by Zhang et al., 2020.
    /// Introduces weighted ranking losses for portfolio selection.
    /// 
    /// @see [From RankNet to LambdaRank to LambdaMART:AnOverview](https://www.microsoft.com/en-us/research/uploads/prod/2016/02/MSR-TR-2010-82.pdf) by Burges, 2016.
    /// This technical report provides a comprehensive explanation of three related learning-to-rank algorithms - RankNet, LambdaRank, and LambdaMART - which 
    /// progressively improved ranking performance by introducing the novel concept of directly specifying gradients rather than deriving them from cost functions.
    /// 
    /// @see [Learning to Rank: From Pairwise Approach to Listwise Approach](https://www.microsoft.com/en-us/research/wp-content/uploads/2016/02/tr-2007-40.pdf) by Cao et al., 2007.
    /// This paper proposes a "listwise" approach to learning-to-rank that treats entire lists of items as training instances rather than pairs of items, introducing probability 
    /// models and neural networks to define loss functions that outperform traditional pairwise ranking methods.
    /// 
    /// Implementation References:
    /// @see [LightGBM Ranking Implementation](https://github.com/microsoft/LightGBM/blob/master/src/objective/rank_objective.hpp)
    /// Efficient C++ implementation of ranking losses.
    /// 
    /// @see [FastAI Pairwise Ranking](https://github.com/fastai/fastai/blob/master/fastai/losses.py)
    /// Python implementation of pairwise ranking losses with emphasis on efficiency.    
    /// </remarks>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PairwiseLossParameter : LayerParameterBase
    {
        double m_dfMargin = 1.0;
        bool m_bWeightByReturnDiff = true;
        double m_dfMinReturnDiff = 1e-6;

        /// <summary>
        /// The PairwiseLossParameter constructor.
        /// </summary>
        public PairwiseLossParameter()
        {
        }

        /// <summary>
        /// Specifies the margin in the loss function that separates positive and negative pairs (default = 1.0).
        /// </summary>
        [Description("Specifies the margin in the loss function that separates positive and negative pairs (default = 1.0).")]
        public double margin
        {
            get { return m_dfMargin; }
            set { m_dfMargin = value; }
        }

        /// <summary>
        /// Specifies whether to weight the loss by return difference magnitude (default = true).
        /// </summary>
        [Description("Specifies whether to weight the loss by return difference magnitude (default = true).")]
        public bool weight_by_return_diff
        {
            get { return m_bWeightByReturnDiff; }
            set { m_bWeightByReturnDiff = value; }
        }

        /// <summary>
        /// Specifies the minimum return difference to consider a pair valid (default = 1e-6).
        /// </summary>
        [Description("Specifies the minimum return difference to consider a pair valid (default = 1e-6).")]
        public double min_return_diff
        {
            get { return m_dfMinReturnDiff; }
            set { m_dfMinReturnDiff = value; }
        }

        /// <summary>
        /// Load the parameter from a binary reader.
        /// </summary>
        /// <param name="br">Specifies the binary reader.</param>
        /// <param name="bNewInstance">When <i>true</i> a new instance is created (the default), otherwise the existing instance is loaded from the binary reader.</param>
        /// <returns>Returns an instance of the parameter.</returns>
        public override object Load(System.IO.BinaryReader br, bool bNewInstance = true)
        {
            RawProto proto = RawProto.Parse(br.ReadString());
            PairwiseLossParameter p = FromProto(proto);

            if (!bNewInstance)
                Copy(p);

            return p;
        }

        /// <summary>
        /// Copy one parameter to another.
        /// </summary>
        /// <param name="src">Specifies the parameter to copy.</param>
        public override void Copy(LayerParameterBase src)
        {
            PairwiseLossParameter p = (PairwiseLossParameter)src;
            m_dfMargin = p.m_dfMargin;
            m_bWeightByReturnDiff = p.m_bWeightByReturnDiff;
            m_dfMinReturnDiff = p.m_dfMinReturnDiff;
        }

        /// <summary>
        /// Creates a new copy of this instance of the parameter.
        /// </summary>
        /// <returns>A new instance of this parameter is returned.</returns>
        public override LayerParameterBase Clone()
        {
            PairwiseLossParameter p = new PairwiseLossParameter();
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

            rgChildren.Add("margin", margin.ToString());
            rgChildren.Add("weight_by_return_diff", weight_by_return_diff.ToString());
            rgChildren.Add("min_return_diff", min_return_diff.ToString());

            return new RawProto(strName, "", rgChildren);
        }

        /// <summary>
        /// Parses the parameter from a RawProto.
        /// </summary>
        /// <param name="rp">Specifies the RawProto to parse.</param>
        /// <returns>A new instance of the parameter is returned.</returns>
        public static PairwiseLossParameter FromProto(RawProto rp)
        {
            string strVal;
            PairwiseLossParameter p = new PairwiseLossParameter();

            if ((strVal = rp.FindValue("margin")) != null)
                p.margin = ParseDouble(strVal);

            if ((strVal = rp.FindValue("weight_by_return_diff")) != null)
                p.weight_by_return_diff = bool.Parse(strVal);

            if ((strVal = rp.FindValue("min_return_diff")) != null)
                p.min_return_diff = ParseDouble(strVal);

            return p;
        }
    }
}