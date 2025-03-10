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
using MyCaffe.db.image;
using MyCaffe.basecode.descriptors;
using MyCaffe.data;
using MyCaffe.layers.lnn;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.IO;

/// <summary>
/// Testing the LtcCell layer.
/// 
/// LtcCell Layer - layer calculate gated linear unit.
/// </remarks> 
namespace MyCaffe.test
{
    [TestClass]
    public class TestLNN_LtcUnitLayer
    {
        [TestMethod]
        public void TestForward()
        {
            LtcCellLayerTest test = new LtcCellLayerTest();

            try
            {
                foreach (ILtcCellLayerTest t in test.Tests)
                {
                    t.TestForward(false);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestBackward()
        {
            LtcCellLayerTest test = new LtcCellLayerTest();

            try
            {
                foreach (ILtcCellLayerTest t in test.Tests)
                {
                    t.TestBackward(false);
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
            LtcCellLayerTest test = new LtcCellLayerTest();

            try
            {
                foreach (ILtcCellLayerTest t in test.Tests)
                {
                    t.TestGradient(false);
                }
            }
            finally
            {
                test.Dispose();
            }
        }
    }

    interface ILtcCellLayerTest : ITest
    {
        void TestForward(bool bNoGate);
        void TestBackward(bool bNoGate);
        void TestGradient(bool bNoGate);
    }

    class LtcCellLayerTest : TestBase
    {
        public LtcCellLayerTest(EngineParameter.Engine engine = EngineParameter.Engine.DEFAULT)
            : base("LtcCell Layer Test", TestBase.DEFAULT_DEVICE_ID, engine)
        {
        }

        protected override ITest create(common.DataType dt, string strName, int nDeviceID, EngineParameter.Engine engine)
        {
            if (dt == common.DataType.DOUBLE)
                return new LtcCellLayerTest<double>(strName, nDeviceID, engine);
            else
                return new LtcCellLayerTest<float>(strName, nDeviceID, engine);
        }
    }

    class LtcCellLayerTest<T> : TestEx<T>, ILtcCellLayerTest
    {
        public LtcCellLayerTest(string strName, int nDeviceID, EngineParameter.Engine engine)
            : base(strName, null, nDeviceID)
        {
            m_engine = engine;
        }

        protected override void dispose()
        {
            base.dispose();
        }

        protected override FillerParameter getFillerParam()
        {
            return new FillerParameter("gaussian");
        }

        private string getTestDataPath(string strSubPath)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\MyCaffe\\test_data\\LNN\\test\\" + strSubPath + "\\iter_0\\";
        }

        private string getTestWtsPath(string strSubPath)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\MyCaffe\\test_data\\LNN\\test\\" + strSubPath + "\\iter_0\\weights\\";
        }

        private void verifyFileDownload(string strSubPath, string strFile)
        {
            string strPath = getTestDataPath(strSubPath);
            if (!File.Exists(strPath + strFile))
                throw new Exception("ERROR: You need to download the LNN test data by running the MyCaffe Test Application and selecting the 'Download Test Data | LNN' menu.");
        }

        private void load_weights(Layer<T> layer, string strPath, string strTag = "")
        {
            int nIdx = 0;

            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.GLEAK].LoadFromNumpy(strPath + strTag + "gleak.npy");
            nIdx++;
            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.VLEAK].LoadFromNumpy(strPath + strTag + "vleak.npy");
            nIdx++;
            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.CM].LoadFromNumpy(strPath + strTag + "cm.npy");
            nIdx++;
            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.SIGMA].LoadFromNumpy(strPath + strTag + "sigma.npy");
            nIdx++;
            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.MU].LoadFromNumpy(strPath + strTag + "mu.npy");
            nIdx++;
            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.W].LoadFromNumpy(strPath + strTag + "w.npy");
            nIdx++;
            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.EREV].LoadFromNumpy(strPath + strTag + "erev.npy");
            nIdx++;
            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.SENSORY_SIGMA].LoadFromNumpy(strPath + strTag + "sensory_sigma.npy");
            nIdx++;
            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.SENSORY_MU].LoadFromNumpy(strPath + strTag + "sensory_mu.npy");
            nIdx++;
            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.SENSORY_W].LoadFromNumpy(strPath + strTag + "sensory_w.npy");
            nIdx++;
            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.SENSORY_EREV].LoadFromNumpy(strPath + strTag + "sensory_erev.npy");
            nIdx++;
            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.INPUT_WT].LoadFromNumpy(strPath + strTag + "input_w.npy");
            nIdx++;
            layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.INPUT_BIAS].LoadFromNumpy(strPath + strTag + "input_b.npy");
            nIdx++;
        }

        private void check_weight_grads(Blob<T> blobVal, Blob<T> blobWork, Layer<T> layer, string strPath, string strTag = "")
        {
            blobVal.LoadFromNumpy(strPath + "gleak.grad.npy", true);
            m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.GLEAK], blobWork, true, 8e-07), "The gradient grads for the gleak are incorrect!");

            blobVal.LoadFromNumpy(strPath + "vleak.grad.npy", true);
            m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.VLEAK], blobWork, true, 1e-06), "The gradient grads for the vleak are incorrect!");

            blobVal.LoadFromNumpy(strPath + "erev.grad.npy", true);
            m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.EREV], blobWork, true, 5e-07), "The gradient grads for the erev are incorrect!");

            blobVal.LoadFromNumpy(strPath + "w.grad.npy", true);
            m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.W], blobWork, true, 5e-07), "The gradient grads for the w are incorrect!");

            blobVal.LoadFromNumpy(strPath + "mu.grad.npy", true);
            m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.MU], blobWork, true, 2e-06), "The gradient grads for the mu are incorrect!");

            blobVal.LoadFromNumpy(strPath + "sigma.grad.npy", true);
            m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.SIGMA], blobWork, true, 2e-07), "The gradient grads for the sigma are incorrect!");

            //blobVal.LoadFromNumpy(strPath + "cm.grad.npy", true);
            //m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.CM], blobWork, true), "The gradient grads for the cm are incorrect!");

            blobVal.LoadFromNumpy(strPath + "sensory_sigma.grad.npy", true);
            m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.SENSORY_SIGMA], blobWork, true, 5e-08), "The gradient grads for the sensory_sigma are incorrect!");

            blobVal.LoadFromNumpy(strPath + "sensory_mu.grad.npy", true);
            m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.SENSORY_MU], blobWork, true, 6e-07), "The gradient grads for the sensory_mu are incorrect!");

            blobVal.LoadFromNumpy(strPath + "sensory_w.grad.npy", true);
            m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.SENSORY_W], blobWork, true, 6e-07), "The gradient grads for the sensory_w are incorrect!");

            blobVal.LoadFromNumpy(strPath + "sensory_erev.grad.npy", true);
            m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.SENSORY_EREV], blobWork, true, 5e-07), "The gradient grads for the sensory_erev are incorrect!");

            blobVal.LoadFromNumpy(strPath + "mi.input_w.grad.npy", true);
            m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.INPUT_WT], blobWork, true, 2e-06), "The gradient grads for the input_w are incorrect!");

            blobVal.LoadFromNumpy(strPath + "mi.input_b.grad.npy", true);
            m_log.CHECK(blobVal.Compare(layer.blobs[(int)LtcUnitLayer<T>.WEIGHT.INPUT_BIAS], blobWork, true, 4e-06), "The gradient grads for the input_b are incorrect!");
        }

        /// <summary>
        /// Test LtcUnit forward
        /// </summary>
        /// <remarks>
        /// To generate the test data, run the following:
        /// Code: test_ltc_cell.py
        /// Path: ltc_cell
        /// </remarks>
        public void TestForward(bool bNoGate)
        {
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.LTC_UNIT);
            p.ltc_unit_param.input_size = 82;
            p.ltc_unit_param.hidden_size = 256;
            Layer<T> layer = null;
            Blob<T> blobX = null;
            Blob<T> blobHx = null;
            Blob<T> blobTs = null;
            Blob<T> blobY = null;
            Blob<T> blobYexp = null;
            Blob<T> blobWork = null;
            string strSubPath = (bNoGate) ? "ltc_cell_no_gate" : "ltc_cell_gate";
            string strPath = getTestDataPath(strSubPath);
            string strPathWts = getTestWtsPath(strSubPath);

            verifyFileDownload(strSubPath, "0.cm_t.b.npy");

            try
            {
                layer = Layer<T>.Create(m_cuda, m_log, p, null);
                blobX = new Blob<T>(m_cuda, m_log);
                blobHx = new Blob<T>(m_cuda, m_log);
                blobTs = new Blob<T>(m_cuda, m_log);
                blobY = new Blob<T>(m_cuda, m_log);
                blobYexp = new Blob<T>(m_cuda, m_log);
                blobWork = new Blob<T>(m_cuda, m_log);

                m_log.CHECK(layer != null, "The layer was not created correctly.");
                m_log.CHECK(layer.type == LayerParameter.LayerType.LTC_UNIT, "The layer type is incorrect.");

                blobX.LoadFromNumpy(strPath + "x.npy");
                blobHx.LoadFromNumpy(strPath + "hx.npy");
                blobTs.LoadFromNumpy(strPath + "ts.npy");

                BottomVec.Clear();
                BottomVec.Add(blobX);
                BottomVec.Add(blobHx);
                BottomVec.Add(blobTs);
                TopVec.Clear();
                TopVec.Add(blobY);

                layer.Setup(BottomVec, TopVec);
                load_weights(layer, strPathWts);

                layer.Forward(BottomVec, TopVec);

                blobYexp.LoadFromNumpy(strPath + "h_state.npy");
                m_log.CHECK(TopVec[0].Compare(blobYexp, blobWork, false, 2e-07), "The blobs do not match.");
            }
            finally
            {
                dispose(ref blobYexp);
                dispose(ref blobWork);
                dispose(ref blobX);
                dispose(ref blobHx);
                dispose(ref blobTs);
                dispose(ref blobY);

                if (layer != null)
                    layer.Dispose();
            }
        }

        /// <summary>
        /// WORK IN PROGRESS - Test LtcUnit backward
        /// </summary>
        /// <remarks>
        /// To generate the test data, run the following:
        /// Code: test_ltc_cell.py
        /// Path: ltc_cell
        /// </remarks>
        public void TestBackward(bool bNoGate)
        {
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.LTC_UNIT);
            p.ltc_unit_param.input_size = 82;
            p.ltc_unit_param.hidden_size = 256;
            Layer<T> layer = null;
            Blob<T> blobX = null;
            Blob<T> blobHx = null;
            Blob<T> blobTs = null;
            Blob<T> blobY = null;
            Blob<T> blobYexp = null;
            Blob<T> blobWork = null;
            Blob<T> blobXgrad = null;
            Blob<T> blobHxgrad = null;
            Blob<T> blobTsgrad = null;
            Blob<T> blobVal = null;
            string strSubPath = (bNoGate) ? "ltc_cell_no_gate" : "ltc_cell_gate";
            string strPath = getTestDataPath(strSubPath);
            string strPathWts = getTestWtsPath(strSubPath);

            verifyFileDownload(strSubPath, "0.cm_t.b.npy");

            try
            {
                layer = Layer<T>.Create(m_cuda, m_log, p, null);
                blobX = new Blob<T>(m_cuda, m_log);
                blobHx = new Blob<T>(m_cuda, m_log);
                blobTs = new Blob<T>(m_cuda, m_log);
                blobY = new Blob<T>(m_cuda, m_log);
                blobYexp = new Blob<T>(m_cuda, m_log);
                blobWork = new Blob<T>(m_cuda, m_log);
                blobVal = new Blob<T>(m_cuda, m_log);
                blobXgrad = new Blob<T>(m_cuda, m_log);
                blobHxgrad = new Blob<T>(m_cuda, m_log);
                blobTsgrad = new Blob<T>(m_cuda, m_log);

                m_log.CHECK(layer != null, "The layer was not created correctly.");
                m_log.CHECK(layer.type == LayerParameter.LayerType.LTC_UNIT, "The layer type is incorrect.");

                blobX.LoadFromNumpy(strPath + "x.npy");
                blobHx.LoadFromNumpy(strPath + "hx.npy");
                blobTs.LoadFromNumpy(strPath + "ts.npy");

                BottomVec.Clear();
                BottomVec.Add(blobX);
                BottomVec.Add(blobHx);
                BottomVec.Add(blobTs);
                TopVec.Clear();
                TopVec.Add(blobY);

                layer.Setup(BottomVec, TopVec);
                load_weights(layer, strPathWts);

                layer.Forward(BottomVec, TopVec);

                blobYexp.LoadFromNumpy(strPath + "h_state.npy");
                m_log.CHECK(TopVec[0].Compare(blobYexp, blobWork, false, 2e-07), "The blobs do not match.");

                //** BACKWARD **

                TopVec[0].LoadFromNumpy(strPath + "h_state.grad.npy", true);

                layer.Backward(TopVec, new List<bool>() { true }, BottomVec);

                blobXgrad.LoadFromNumpy(strPath + "x.grad.npy", true);
                m_log.CHECK(blobXgrad.Compare(blobX, blobWork, true, 4e-07), "The blobs do not match.");

                blobHxgrad.LoadFromNumpy(strPath + "hx.grad.npy", true);
                m_log.CHECK(blobHxgrad.Compare(blobHx, blobWork, true, 1e-07), "The blobs do not match.");
                blobTsgrad.LoadFromNumpy(strPath + "ts.grad.npy", true);
                m_log.CHECK(blobTsgrad.Compare(blobTs, blobWork, true, 2e-07), "The blobs do not match.");

                check_weight_grads(blobVal, blobWork, layer, strPath);
            }
            finally
            {
                dispose(ref blobYexp);
                dispose(ref blobWork);
                dispose(ref blobVal);
                dispose(ref blobX);
                dispose(ref blobHx);
                dispose(ref blobTs);
                dispose(ref blobXgrad);
                dispose(ref blobHxgrad);
                dispose(ref blobTsgrad);
                dispose(ref blobY);

                if (layer != null)
                    layer.Dispose();
            }
        }

        /// <summary>
        /// Test LtcCell gradient check
        /// </summary>
        public void TestGradient(bool bNoGate)
        {
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.LTC_UNIT);
            p.ltc_unit_param.input_size = 82;
            p.ltc_unit_param.hidden_size = 256;
            Layer<T> layer = null;
            Blob<T> blobX = null;
            Blob<T> blobHx = null;
            Blob<T> blobTs = null;
            Blob<T> blobY = null;
            string strSubPath = (bNoGate) ? "ltc_cell_no_gate" : "ltc_cell_gate";
            string strPath = getTestDataPath(strSubPath);
            string strPathWts = getTestWtsPath(strSubPath);

            verifyFileDownload(strSubPath, "0.cm_t.b.npy");

            try
            {
                layer = Layer<T>.Create(m_cuda, m_log, p, null);
                blobX = new Blob<T>(m_cuda, m_log);
                blobHx = new Blob<T>(m_cuda, m_log);
                blobTs = new Blob<T>(m_cuda, m_log);
                blobY = new Blob<T>(m_cuda, m_log);

                m_log.CHECK(layer != null, "The layer was not created correctly.");
                m_log.CHECK(layer.type == LayerParameter.LayerType.LTC_UNIT, "The layer type is incorrect.");

                blobX.LoadFromNumpy(strPath + "x.npy");
                blobHx.LoadFromNumpy(strPath + "hx.npy");
                blobTs.LoadFromNumpy(strPath + "ts.npy");

                BottomVec.Clear();
                BottomVec.Add(blobX);
                BottomVec.Add(blobHx);
                BottomVec.Add(blobTs);
                TopVec.Clear();
                TopVec.Add(blobY);

                layer.Setup(BottomVec, TopVec);
                load_weights(layer, strPathWts);

                GradientChecker<T> checker = new GradientChecker<T>(m_cuda, m_log, 0.01, 0.01);
                checker.CheckGradient(layer, BottomVec, TopVec, -1, 1, 0.01);
            }
            finally
            {
                dispose(ref blobX);
                dispose(ref blobHx);
                dispose(ref blobTs);
                dispose(ref blobY);

                if (layer != null)
                    layer.Dispose();
            }
        }
    }
}
