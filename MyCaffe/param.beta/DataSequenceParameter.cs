﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCaffe.basecode;
using System.ComponentModel;

/// <summary>
/// The MyCaffe.param.beta parameters are used by the MyCaffe.layer.beta layers.
/// </summary>
/// <remarks>
/// Using parameters within the MyCaffe.layer.beta namespace are used by layers that require the MyCaffe.layers.beta.dll.
/// </remarks>
namespace MyCaffe.param.beta
{
    /// <summary>
    /// Specifies the parameters for the DataSequenceLayer.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DataSequenceParameter : LayerParameterBase 
    {
        int m_nK = 0;
        bool m_bBalanceMatches = true;
        int m_nCacheSize = 256;
        bool m_bOutputLabels = true;
        int m_nLabelCount = 0;
        int m_nLabelStart = 0;

        /** @copydoc LayerParameterBase */
        public DataSequenceParameter()
        {
        }

        /// <summary>
        /// Specifies the cache size used for each labeled image cache, which should be at least 4 x the batch size used (default = 256).
        /// </summary>
        /// <remarks>
        /// The cache size should be higher than the maximum number of items for a given label within a batch, otherwise items within a given batch
        /// will overrite other items from the batch for a given label.
        /// 
        /// NOTE: The batch size must also be large enough to contain at least two items from each label.
        /// </remarks>
        [Description("Specifies the cache size used for each labeled image cache, which should be at least 4 x the batch size used (default = 256).")]
        public int cache_size
        {
            get { return m_nCacheSize; }
            set { m_nCacheSize = value; }
        }

        /// <summary>
        /// Specifies the 'k' number of negatively matched labled images (default = 0, maximum = 10).  When specifying 0, the output is just the anchor and one alternating negatively/positive matched image.
        /// </summary>
        /// <remarks>
        /// When specifying k>0, the anchor is output with a positive match and 'k' number of negative matches.
        /// </remarks>
        [Description("Specifies the 'k' number of negatively matched labled images (default = 0, maximum = 10).  When specifying 0, the output is just the anchor and a negatively matched image.")]
        public int k
        {
            get { return m_nK; }
            set { m_nK = value; }
        }

        /// <summary>
        /// Specifies to balance the matching image between negative and positive matches.  This setting only applies when k=0 (default = true).
        /// </summary>
        [Description("Specifies to balance the matching image between negative and positive matches. This setting only applies when k=0 (default = true).")]
        public bool balance_matches
        {
            get { return m_bBalanceMatches; }
            set { m_bBalanceMatches = value; }
        }

        /// <summary>
        /// Specifies whether or not to output the labels in an additional top output.  (default = true).
        /// </summary>
        /// <remarks>
        /// Labels areoutput in order per tuplet, so if k = 0 (e.g. output anchor and one negative) the output lables are as follows:
        /// 0 anchor label
        /// 0 negative label
        /// 1 anchor label
        /// 1 negative label
        /// :
        /// </remarks>
        [Description("Specifies whether or not to output the labels in an additional top output where labels are organized in the same batch order and listed by tuplet order (e.g. when k = 0, balance_matches = true; anchor, positive, anchor, negative; when k = 1, anchor, positive, negative, etc")]
        public bool output_labels
        {
            get { return m_bOutputLabels; }
            set { m_bOutputLabels = value; }
        }

        /// <summary>
        /// Specifies the number of labels in the data set, or 0 to use dynamic label discovery (requires large enough batch sizes to cover all labels within first batch) - (default = 0).
        /// </summary>
        [Description("Specifies the number of labels in the data set, or 0 to use dynamic label discovery (requires large enough batch sizes to cover all labels within first batch) - (default = 0).")]
        public int label_count
        {
            get { return m_nLabelCount; }
            set { m_nLabelCount = value; }
        }

        /// <summary>
        /// Specifies the first label in the label set (default = 0).
        /// </summary>
        [Description("Specifies the first label in the label set (default = 0).")]
        public int label_start
        {
            get { return m_nLabelStart; }
            set { m_nLabelStart = value; }
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
            DataSequenceParameter p = FromProto(proto);

            if (!bNewInstance)
                Copy(p);

            return p;
        }

        /// <summary>
        /// Copy on parameter to another.
        /// </summary>
        /// <param name="src">Specifies the parameter to copy.</param>
        public override void Copy(LayerParameterBase src)
        {
            DataSequenceParameter p = (DataSequenceParameter)src;
            m_nCacheSize = p.m_nCacheSize;
            m_nK = p.m_nK;
            m_bOutputLabels = p.m_bOutputLabels;
            m_nLabelStart = p.m_nLabelStart;
            m_nLabelCount = p.m_nLabelCount;
            m_bBalanceMatches = p.m_bBalanceMatches;
        }

        /// <summary>
        /// Creates a new copy of this instance of the parameter.
        /// </summary>
        /// <returns>A new instance of this parameter is returned.</returns>
        public override LayerParameterBase Clone()
        {
            DataSequenceParameter p = new DataSequenceParameter();
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

            rgChildren.Add("cache_size", cache_size.ToString());
            rgChildren.Add("k", k.ToString());
            rgChildren.Add("output_labels", output_labels.ToString());
            rgChildren.Add("label_count", label_count.ToString());
            rgChildren.Add("label_start", label_start.ToString());
            rgChildren.Add("balance_matches", balance_matches.ToString());

            return new RawProto(strName, "", rgChildren);
        }

        /// <summary>
        /// Parses the parameter from a RawProto.
        /// </summary>
        /// <param name="rp">Specifies the RawProto to parse.</param>
        /// <returns>A new instance of the parameter is returned.</returns>
        public static DataSequenceParameter FromProto(RawProto rp)
        {
            string strVal;
            DataSequenceParameter p = new DataSequenceParameter();

            if ((strVal = rp.FindValue("cache_size")) != null)
                p.cache_size = int.Parse(strVal);

            if ((strVal = rp.FindValue("k")) != null)
                p.k = int.Parse(strVal);

            if ((strVal = rp.FindValue("output_labels")) != null)
                p.output_labels = bool.Parse(strVal);

            if ((strVal = rp.FindValue("label_count")) != null)
                p.label_count = int.Parse(strVal);

            if ((strVal = rp.FindValue("label_start")) != null)
                p.label_start = int.Parse(strVal);

            if ((strVal = rp.FindValue("balance_matches")) != null)
                p.balance_matches = bool.Parse(strVal);

            return p;
        }
    }
}
