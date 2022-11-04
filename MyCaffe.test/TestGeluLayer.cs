﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCaffe.param;
using MyCaffe.basecode;
using MyCaffe.common;
using MyCaffe.fillers;
using MyCaffe.layers;
using System.Diagnostics;

namespace MyCaffe.test
{
    [TestClass]
    public class TestGeluLayer
    {
        [TestMethod]
        public void TestForward()
        {
            GeluLayerTest2 test = new GeluLayerTest2(EngineParameter.Engine.CAFFE);

            try
            {
                foreach (IGeluLayerTest2 t in test.Tests)
                {
                    t.TestForward();
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestGradient()
        {
            GeluLayerTest2 test = new GeluLayerTest2(EngineParameter.Engine.CAFFE);

            try
            {
                foreach (IGeluLayerTest2 t in test.Tests)
                {
                    t.TestGradient();
                }
            }
            finally
            {
                test.Dispose();
            }
        }
    }

    interface IGeluLayerTest2 : ITest
    {
        void TestForward();
        void TestGradient();
    }

    class GeluLayerTest2 : TestBase
    {
        public GeluLayerTest2(EngineParameter.Engine engine = EngineParameter.Engine.DEFAULT)
            : base("GELU Layer Test", TestBase.DEFAULT_DEVICE_ID, engine)
        {
        }

        protected override ITest create(common.DataType dt, string strName, int nDeviceID, EngineParameter.Engine engine)
        {
            if (dt == common.DataType.DOUBLE)
                return new GeluLayerTest2<double>(strName, nDeviceID, engine);
            else
                return new GeluLayerTest2<float>(strName, nDeviceID, engine);
        }
    }

    class GeluLayerTest2<T> : TestEx<T>, IGeluLayerTest2
    {
        public GeluLayerTest2(string strName, int nDeviceID, EngineParameter.Engine engine)
            : base(strName, new List<int>() { 2, 3, 4, 5 }, nDeviceID)
        {
            m_engine = engine;
        }

        protected override FillerParameter getFillerParam()
        {
            return base.getFillerParam();
        }

        protected override void dispose()
        {
            base.dispose();
        }

        /// <summary>
        /// Calculate the native GELU
        /// </summary>
        /// <param name="x">Specifies the input.</param>
        /// <returns>The calculated Gelu value is returned.</returns>
        /// <remarks>
        /// Computes the mish non-linearity @f$ y  = 0.5 * (1.0 + tanh(sqrt(2.0/PI) * (x + 0.044715 * x^3))) @f$.
        /// @see [Github - Karpathy: NewGELU, line 21](https://github.com/karpathy/minGPT/blob/master/mingpt/model.py) by Karpathy, 2022.
        /// </remarks>
        protected double gelu_native(double x)
        {
            return 0.5 * (1.0 + Math.Tanh(Math.Sqrt(2.0 / Math.PI) * (x + 0.044715 * Math.Pow(x, 3))));
        }

        /// <summary>
        /// Calculate the native GELU gradient
        /// </summary>
        /// <param name="x">Specifies the input.</param>
        /// <returns>The calculated Gelu value is returned.</returns>
        /// <remarks>
        /// Computes the gelu non-linearity @f$ y  = 0.5 * (1.0 + tanh(sqrt(2.0/PI) * (x + 0.044715 * x^3))) @f$.
        /// with                            @f$ y' = \frac{0.107032 * (x^2 + 7.45462)}{cosh(0.0713548 * x^3 + 1.59577 * x) + 1} @f$
        /// Note, see Wolfram Alpha with 'derivative of @f$ d/dx  = 0.5 * (1.0 + tanh(sqrt(2.0/PI) * (x + 0.044715 * x^3))) @f$'                                         
        protected double gelu_native_grad(double x)
        {
            double dfNum = 0.107032 * (Math.Pow(x, 2) + 7.45462);
            double dfDen = Math.Cosh(0.0713548 * Math.Pow(x, 3) + 1.59577 * x) + 1;

            if (dfDen == 0)
                return 0;

            return dfNum / dfDen;
        }

        public void TestForward(double dfFillerStd)
        {
            FillerParameter fp = new FillerParameter("gaussian");
            fp.std = dfFillerStd;
            Filler<T> filler = Filler<T>.Create(m_cuda, m_log, fp);

            filler.Fill(Bottom);

            LayerParameter p = new LayerParameter(LayerParameter.LayerType.GELU);
            Layer<T> layer = Layer<T>.Create(m_cuda, m_log, p, new CancelEvent());

            try
            {
                m_log.CHECK(layer.type == LayerParameter.LayerType.GELU, "The layer type is incorrect!");

                layer.Setup(BottomVec, TopVec);
                layer.Forward(BottomVec, TopVec);

                // Now, check values
                double[] rgBottomData = convert(Bottom.update_cpu_data());
                double[] rgTopData = convert(Top.update_cpu_data());
                double dfMinPrecision = 1e-5;

                for (int i = 0; i < Bottom.count(); i++)
                {
                    double dfExpectedValue = gelu_native(rgBottomData[i]);
                    double dfPrecision = Math.Max(Math.Abs(dfExpectedValue * 1e-4), dfMinPrecision);
                    m_log.EXPECT_NEAR(dfExpectedValue, rgTopData[i], dfPrecision);
                }
            }
            finally
            {
                layer.Dispose();
            }
        }

        public void TestBackward(double dfFillerStd)
        {
            FillerParameter fp = new FillerParameter("gaussian");
            fp.std = dfFillerStd;
            Filler<T> filler = Filler<T>.Create(m_cuda, m_log, fp);

            filler.Fill(Bottom);

            LayerParameter p = new LayerParameter(LayerParameter.LayerType.GELU);
            Layer<T> layer = Layer<T>.Create(m_cuda, m_log, p, new CancelEvent());

            try
            {
                m_log.CHECK(layer.type == LayerParameter.LayerType.GELU, "The layer type is incorrect!");

                GradientChecker<T> checker = new GradientChecker<T>(m_cuda, m_log);
                checker.CheckGradientEltwise(layer, BottomVec, TopVec);
            }
            finally
            {
                layer.Dispose();
            }
        }

        public void TestForward()
        {
            TestForward(1.0);
        }

        public void TestGradient()
        {
            TestBackward(1.0);
        }
    }
}