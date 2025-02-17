﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MyCaffe.basecode;

namespace MyCaffe.param
{
    /// <summary>
    /// Specifies the parameters for the AccuracyRegressionLayer.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AccuracyRegressionParameter : LayerParameterBase
    {
        ALGORITHM m_alg = ALGORITHM.MAPE;
        double m_dfBucketMin = -2.0;
        double m_dfBucketMax = 2.0;
        int m_nBucketCount = 10;
        float? m_fIgnoreMin = null;
        float? m_fIgnoreMax = null;
        bool m_bEnableOverride = false;
        double m_dfBucketCenterValue = 0;
        double? m_dfCenterBucketPercentFromMid = null;

        /// <summary>
        /// Defines the MAPE algorithm to use.
        /// </summary>
        public enum ALGORITHM
        {
            /// <summary>
            /// Defines the Mean Absolute Percentage Error algorithm.
            /// </summary>
            MAPE,
            /// <summary>
            /// Defines the Symmetric Mean Absolute Percentage Error algorithm.
            /// </summary>
            SMAPE,
            /// <summary>
            /// Defines the Bucketed method of calculating accuracy.
            /// </summary>
            BUCKETING
        }

        /** @copydoc LayerParameterBase */
        public AccuracyRegressionParameter()
        {
        }

        /// <summary>
        /// When enabled, and when the 'LabelBucketConfig' dataset parameter exists (set with the Dataset Creator), the bucketing values are set from the 'LabelBucketConfig' dataset parameter.
        /// </summary>
        public bool enable_override
        {
            get { return m_bEnableOverride; }
            set { m_bEnableOverride = value; }
        }

        /// <summary>
        /// Specifies the algorithm to use: MAPE or SMAPE.
        /// </summary>
        [Description("Specifies algorithm used in the MAPE, SMAPE or BUCKETING calculation.")]
        public ALGORITHM algorithm
        {
            get { return m_alg; }
            set { m_alg = value; }
        }

        /// <summary>
        /// Specifies the value to use as the center value for the bucketing (default = 0.0).
        /// </summary>
        [Description("Specifies the value to use as the center value for the bucketing (default = 0.0)")]
        public double bucket_center
        {
            get { return m_dfBucketCenterValue; }
            set { m_dfBucketCenterValue= value; }
        }

        /// <summary>
        /// Specifies the percent from the bucket center value to set the center bucket bounds with (optional, default = null).
        /// </summary>
        [Description("Specifies the percent from the bucket center value to set the center bucket bounds with (optional, default = null).")]
        public double? center_bucket_percent_from_mid
        {
            get { return m_dfCenterBucketPercentFromMid; }
            set { m_dfCenterBucketPercentFromMid= value; }
        }

        /// <summary>
        /// Ignore all scores below this value (default = null).  Note when using ignore ranges both min and max must be specified.
        /// </summary>
        [Description("Ignore all scores below this value (default = null).  Note when using ignore ranges both min and max must be specified.")]
        public float? bucket_ignore_max
        {
            get { return m_fIgnoreMin; }
            set { m_fIgnoreMin = value; }
        }

        /// <summary>
        /// Ignore all scores above this value (default = null).  Note when using ignore ranges both min and max must be specified.
        /// </summary>
        [Description("Ignore all scores above this value (default = null).  Note when using ignore ranges both min and max must be specified.")]
        public float? bucket_ignore_min
        {
            get { return m_fIgnoreMax; }
            set { m_fIgnoreMax = value; }
        }

        /// <summary>
        /// Specifies the minimum value of the bucket range used with BUCKETING.
        /// </summary>
        [Description("Specifies the minimum value of the bucket range used with BUCKETING.")]
        public double bucket_min
        {
            get { return m_dfBucketMin; }
            set { m_dfBucketMin = value; }
        }

        /// <summary>
        /// Specifies the maximum value of the bucket range used with BUCKETING.
        /// </summary>
        [Description("Specifies the maximum value of the bucket range used with BUCKETING.")]
        public double bucket_max
        {
            get { return m_dfBucketMax; }
            set { m_dfBucketMax = value; }
        }

        /// <summary>
        /// Specifies the number of buckets to use with BUCKETING.
        /// </summary>
        [Description("Specifies the number of buckets to use with BUCKETING (bucket count must be 2 or greater).")]
        public int bucket_count
        {
            get { return m_nBucketCount; }
            set { m_nBucketCount = value; }
        }

        /** @copydoc LayerParameterBase::Load */
        public override object Load(System.IO.BinaryReader br, bool bNewInstance = true)
        {
            RawProto proto = RawProto.Parse(br.ReadString());
            AccuracyRegressionParameter p = FromProto(proto);

            if (!bNewInstance)
                Copy(p);

            return p;
        }

        /** @copydoc LayerParameterBase::Copy */
        public override void Copy(LayerParameterBase src)
        {
            AccuracyRegressionParameter p = (AccuracyRegressionParameter)src;

            m_alg = p.m_alg;
            m_dfBucketMin = p.m_dfBucketMin;
            m_dfBucketMax = p.m_dfBucketMax;
            m_nBucketCount = p.m_nBucketCount;
            m_fIgnoreMin = p.m_fIgnoreMin;
            m_fIgnoreMax = p.m_fIgnoreMax;
            m_bEnableOverride = p.m_bEnableOverride;
            m_dfBucketCenterValue = p.m_dfBucketCenterValue;
            m_dfCenterBucketPercentFromMid = p.m_dfCenterBucketPercentFromMid;
        }

        /** @copydoc LayerParameterBase::Clone */
        public override LayerParameterBase Clone()
        {
            AccuracyRegressionParameter p = new AccuracyRegressionParameter();
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

            rgChildren.Add("algorithm", algorithm.ToString());
            rgChildren.Add("bucket_min", bucket_min.ToString());
            rgChildren.Add("bucket_max", bucket_max.ToString());
            rgChildren.Add("bucket_count", bucket_count.ToString());
            rgChildren.Add("enable_override", enable_override.ToString().ToLower());
            rgChildren.Add("bucket_center", bucket_center.ToString());

            if (bucket_ignore_min.HasValue)
                rgChildren.Add("bucket_ignore_min", bucket_ignore_min.Value.ToString());
            if (bucket_ignore_max.HasValue)
                rgChildren.Add("bucket_ignore_max", bucket_ignore_max.Value.ToString());

            if (center_bucket_percent_from_mid.HasValue)
                rgChildren.Add("center_bucket_percent_from_mid", center_bucket_percent_from_mid.Value.ToString());

            return new RawProto(strName, "", rgChildren);
        }

        /// <summary>
        /// Parses the parameter from a RawProto.
        /// </summary>
        /// <param name="rp">Specifies the RawProto to parse.</param>
        /// <returns>A new instance of the parameter is returned.</returns>
        public static AccuracyRegressionParameter FromProto(RawProto rp)
        {
            string strVal;
            AccuracyRegressionParameter p = new AccuracyRegressionParameter();

            if ((strVal = rp.FindValue("algorithm")) != null)
                p.algorithm = (ALGORITHM)Enum.Parse(typeof(ALGORITHM), strVal, true);

            if ((strVal = rp.FindValue("bucket_min")) != null)
                p.bucket_min = BaseParameter.ParseDouble(strVal);

            if ((strVal = rp.FindValue("bucket_max")) != null)
                p.bucket_max = BaseParameter.ParseDouble(strVal);

            if ((strVal = rp.FindValue("bucket_center")) != null)
                p.bucket_center = BaseParameter.ParseDouble(strVal);

            if ((strVal = rp.FindValue("bucket_count")) != null)
                p.bucket_count = int.Parse(strVal);

            if ((strVal = rp.FindValue("bucket_ignore_min")) != null)
                p.bucket_ignore_min = BaseParameter.ParseFloat(strVal);

            if ((strVal = rp.FindValue("bucket_ignore_max")) != null)
                p.bucket_ignore_max = BaseParameter.ParseFloat(strVal);

            if ((strVal = rp.FindValue("enable_override")) != null)
                p.enable_override = bool.Parse(strVal);

            if ((strVal = rp.FindValue("center_bucket_percent_from_mid")) != null)
                p.center_bucket_percent_from_mid = float.Parse(strVal);

            return p;
        }
    }
}
