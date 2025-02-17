﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCaffe.param;
using MyCaffe.db.image;
using MyCaffe.basecode;
using MyCaffe.common;
using MyCaffe.layers;
using System.Threading;
using System.Diagnostics;
using MyCaffe.basecode.descriptors;
using System.Drawing;
using System.IO;

namespace MyCaffe.test
{
    [TestClass]
    public class TestDataLayer
    {
        [TestMethod]
        public void TestInitialize()
        {
            DataLayerTest test = new DataLayerTest("MNIST");
                     
            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestInitialization(test.SourceName);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestSetup()
        {
            DataLayerTest test = new DataLayerTest("MNIST");

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestSetup(test.SourceName);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestForward()
        {
            DataLayerTest test = new DataLayerTest("MNIST");

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestForward(test.SourceName);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestForwardPairs()
        {
            DataLayerTest test = new DataLayerTest("MNIST");

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestForwardPairs(test.SourceName, 2);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestForwardNoPairs()
        {
            DataLayerTest test = new DataLayerTest("MNIST");

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestForwardPairs(test.SourceName, 1);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestForwardMask()
        {
            DataLayerTest test = new DataLayerTest("CIFAR-10");

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestForwardMask(test.SourceName, 2);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestForward2()
        {
            DataLayerTest test = new DataLayerTest("MNIST");

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestForward2(test.SourceName);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestForward_OneHotLabelConversion()
        {
            DataLayerTest test = new DataLayerTest("MNIST");

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestForward_OneHotLabelConversion(test.SourceName);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestForward_TimeAlign_LoadAll()
        {
            DataLayerTest test = new DataLayerTest("test_qry", DB_LOAD_METHOD.LOAD_ALL, DB_ITEM_SELECTION_METHOD.RANDOM);
            
            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestForward_TimeAlign(test.SourceName, true);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestForward_TimeAlign_LoadOnDemand()
        {
            DataLayerTest test = new DataLayerTest("test_qry", DB_LOAD_METHOD.LOAD_ON_DEMAND, DB_ITEM_SELECTION_METHOD.RANDOM);

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestForward_TimeAlign(test.SourceName, true);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestForward_TimeAlign_LoadAll_NoIndexCheck()
        {
            DataLayerTest test = new DataLayerTest("test_qry", DB_LOAD_METHOD.LOAD_ALL, DB_ITEM_SELECTION_METHOD.RANDOM);

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestForward_TimeAlign(test.SourceName, false);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestForward_TimeAlign_LoadOnDemand_NoIndexCheck()
        {
            DataLayerTest test = new DataLayerTest("test_qry", DB_LOAD_METHOD.LOAD_ON_DEMAND, DB_ITEM_SELECTION_METHOD.RANDOM);

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestForward_TimeAlign(test.SourceName, false);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReadLoadAll()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = false;
                    t.Fill(unique_pixels);
                    t.TestRead(DB_LOAD_METHOD.LOAD_ALL);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReadLoadOnDemand()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = false;
                    t.Fill(unique_pixels);
                    t.TestRead(DB_LOAD_METHOD.LOAD_ON_DEMAND);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReadLoadOnDemandNoCache()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = false;
                    t.Fill(unique_pixels);
                    t.TestRead(DB_LOAD_METHOD.LOAD_ON_DEMAND_NOCACHE);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestSkipLoadAll()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.Fill(false);
                    t.TestSkip(DB_LOAD_METHOD.LOAD_ALL);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestSkipLoadOnDemand()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.Fill(false);
                    t.TestSkip(DB_LOAD_METHOD.LOAD_ON_DEMAND);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReshapeLoadAll()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestReshape(DB_LOAD_METHOD.LOAD_ALL);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReshapeLoadOnDemand()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    t.TestReshape(DB_LOAD_METHOD.LOAD_ON_DEMAND);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReadCropTrainLoadAll()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = true;

                    for (int i = 0; i < 2; i++)
                    {
                        t.Fill(unique_pixels);
                        t.TestReadCrop(Phase.TRAIN, DB_LOAD_METHOD.LOAD_ALL);
                    }
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReadCropTrainLoadOnDemand()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = true;
                    t.Fill(unique_pixels);
                    t.TestReadCrop(Phase.TRAIN, DB_LOAD_METHOD.LOAD_ON_DEMAND);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReadCropTrainSequenceSeededLoadAll()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = true;
                    t.Fill(unique_pixels);
                    t.TestReadCropSequenceSeeded(DB_LOAD_METHOD.LOAD_ALL);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReadCropTrainSequenceSeededLoadOnDemand()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = true;
                    t.Fill(unique_pixels);
                    t.TestReadCropSequenceSeeded(DB_LOAD_METHOD.LOAD_ON_DEMAND);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReadCropTrainSequenceUnseededLoadAll()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = true;
                    t.Fill(unique_pixels);
                    t.TestReadCropSequenceSeeded(DB_LOAD_METHOD.LOAD_ALL);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReadCropTrainSequenceUnseededLoadOnDemand()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = true;
                    t.Fill(unique_pixels);
                    t.TestReadCropSequenceSeeded(DB_LOAD_METHOD.LOAD_ON_DEMAND);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReadCropTestLoadAll()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = true;

                    for (int i = 0; i < 2; i++)
                    {
                        t.Fill(unique_pixels);
                        t.TestReadCrop(Phase.TEST, DB_LOAD_METHOD.LOAD_ALL);
                    }
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestReadCropTestLoadOnDemand()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = true;

                    for (int i = 0; i < 2; i++)
                    {
                        t.Fill(unique_pixels);
                        t.TestReadCrop(Phase.TEST, DB_LOAD_METHOD.LOAD_ON_DEMAND);
                    }
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestDataLabelMapping()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = true;

                    t.Fill(unique_pixels, 5);
                    t.TestDataLabelMapping(DB_LOAD_METHOD.LOAD_ON_DEMAND);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestDataLabelMappingWithBoost()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = true;

                    t.Fill3(unique_pixels);
                    t.TestDataLabelMappingWithBoost(DB_LOAD_METHOD.LOAD_ON_DEMAND);
                }
            }
            finally
            {
                test.Dispose();
            }
        }

        [TestMethod]
        public void TestDataLabelMappingWithBoostAndFalseCondition()
        {
            DataLayerTest test = new DataLayerTest();

            try
            {
                foreach (IDataLayerTest t in test.Tests)
                {
                    bool unique_pixels = true;

                    t.Fill3(unique_pixels);
                    t.TestDataLabelMappingWithBoostAndFalseCondition(DB_LOAD_METHOD.LOAD_ON_DEMAND);
                }
            }
            finally
            {
                test.Dispose();
            }
        }
    }

    class DataLayerTest : TestBase
    {
        SettingsCaffe m_settings;
        IXImageDatabaseBase m_db;
        CancelEvent m_evtCancel = new CancelEvent();
        string m_strSrc = "MNIST.training";

        public DataLayerTest(string strDs = null, DB_LOAD_METHOD loadMethod = DB_LOAD_METHOD.LOAD_ON_DEMAND, DB_ITEM_SELECTION_METHOD imgQry = DB_ITEM_SELECTION_METHOD.NONE)
            : base("Data Layer Test")
        {
            m_settings = new SettingsCaffe();
            m_settings.EnableLabelBalancing = false;
            m_settings.EnableLabelBoosting = false;
            m_settings.EnablePairInputSelection = false;
            m_settings.EnableRandomInputSelection = (imgQry == DB_ITEM_SELECTION_METHOD.RANDOM) ? true : false;
            m_settings.DbLoadMethod = loadMethod;

            if (strDs != null && strDs.Length > 0)
            {
                m_db = createImageDb(null);
                m_db.InitializeWithDsName1(m_settings, strDs);

                DatasetDescriptor ds = m_db.GetDatasetByName(strDs);
                m_strSrc = ds.TrainingSourceName;
            }            
        }

        protected override ITest create(DataType dt, string strName, int nDeviceID, EngineParameter.Engine engine = EngineParameter.Engine.DEFAULT)
        {
            string strPath = TestBase.CudaPath;

            if (dt == DataType.DOUBLE)
            {
                CudaDnn<double>.SetDefaultCudaPath(strPath);
                return new DataLayerTest<double>(strName, nDeviceID, this);
            }
            else
            {
                CudaDnn<float>.SetDefaultCudaPath(strPath);
                return new DataLayerTest<float>(strName, nDeviceID, this);
            }
        }

        protected override void dispose()
        {
            if (m_db != null)
            {
                ((IDisposable)m_db).Dispose();
                m_db = null;
            }

            base.dispose();
        }

        public string SourceName
        {
            get { return m_strSrc; }
        }

        public IXImageDatabaseBase db
        {
            get { return m_db; }
        }

        public SettingsCaffe Settings
        {
            get { return m_settings; }
        }

        public CancelEvent CancelEvent
        {
            get { return m_evtCancel; }
        }
    }

    interface IDataLayerTest 
    {
        DataType Type { get; }
        void TestInitialization(string strSrc);
        void TestSetup(string strSrc);
        void TestForward(string strSrc);
        void TestForward2(string strSrc);
        void TestForward_OneHotLabelConversion(string strSrc);
        void TestForward_TimeAlign(string strSrc, bool bIndexCheck);
        void TestForwardPairs(string strSrc, int nImagesPerBlob);
        void TestForwardMask(string strSrc, int nImagesPerBlob);
        string Fill(bool unique_pixels, int nMaxLabel = -1);
        string Fill2(int num_inputs);
        string Fill3(bool unique_pixels);
        void TestRead(DB_LOAD_METHOD loadMethod);
        void TestSkip(DB_LOAD_METHOD loadMethod);
        void TestReshape(DB_LOAD_METHOD loadMethod);
        void TestReadCrop(Phase phase, DB_LOAD_METHOD loadMethod);
        void TestReadCropSequenceSeeded(DB_LOAD_METHOD loadMethod);
        void TestReadCropSequenceUnSeeded(DB_LOAD_METHOD loadMethod);
        void TestDataLabelMapping(DB_LOAD_METHOD loadMethod);
        void TestDataLabelMappingWithBoost(DB_LOAD_METHOD loadMethod);
        void TestDataLabelMappingWithBoostAndFalseCondition(DB_LOAD_METHOD loadMethod);
    }

    class DataLayerTest<T> : TestEx<T>, IDataLayerTest
    {
        Blob<T> m_blob_top_label;
        Blob<T> m_blob_top_idx;
        DataLayerTest m_parent;
        int m_nSrcID1 = 0;
        int m_nSrcID2 = 0;
        string m_strSrc1 = "test_data";
        string m_strSrc2 = "test_data.t";
        int m_nDsID = 0;

        public DataLayerTest(string strName, int nDeviceID, DataLayerTest parent, List<int> rgBottomShape = null)
            : base(strName, rgBottomShape, nDeviceID)
        {
            m_parent = parent;
            m_blob_top_label = new Blob<T>(m_cuda, m_log);
            m_blob_top_idx = new Blob<T>(m_cuda, m_log);

            TopVec.Add(m_blob_top_label);
            BottomVec.Clear();
        }

        protected override void dispose()
        {
            m_blob_top_label.Dispose();
            m_blob_top_idx.Dispose();
            base.dispose();
        }

        public DataType Type
        {
            get { return m_dt; }
        }

        public Blob<T> TopLabel
        {
            get { return m_blob_top_label; }
        }

        public void TestInitialization(string strSrc)
        {
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);

            m_log.CHECK(p.data_param != null, "The data_param is null!");
            m_log.CHECK(p.transform_param != null, "The transform_para is null!");

            p.data_param.batch_size = 1;
            p.data_param.source = strSrc;

            m_log.CHECK(p.data_param.enable_pair_selection == null, "Pair selection should be off by default, letting the image database decide.");
            m_log.CHECK(p.data_param.enable_random_selection == null, "Random selection should be off by default, letting the image database decide.");
            m_log.CHECK_GT(p.data_param.prefetch, 0, "Pre fetch should be greater than zero.");

            DataLayer<T> layer = new DataLayer<T>(m_cuda, m_log, p, m_parent.db, m_parent.CancelEvent);

            layer.Dispose();
        }

        public void TestSetup(string strSrc)
        {
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);

            m_log.CHECK(p.data_param != null, "The data_param is null!");
            m_log.CHECK(p.transform_param != null, "The transform_para is null!");

            p.data_param.batch_size = 1;
            p.data_param.source = strSrc;
            p.data_param.enable_random_selection = false;
            p.data_param.enable_pair_selection = false;

            DataLayer<T> layer = new DataLayer<T>(m_cuda, m_log, p, m_parent.db, m_parent.CancelEvent);

            try
            {
                layer.LayerSetUp(BottomVec, TopVec);
                layer.Reshape(BottomVec, TopVec);

                Thread.Sleep(2000);
            }
            finally
            {
                layer.Dispose();
                m_parent.CancelEvent.Reset();
            }
        }

        public string Fill(bool unique_pixels, int nMaxLabel = -1)
        {
            DatasetFactory factory = new DatasetFactory();

            m_log.WriteLine("Creating temporary dataset '" + m_strSrc1 + "'.");
            SourceDescriptor srcTrain = new SourceDescriptor(0, m_strSrc1, 2, 4, 3, false, true);
            srcTrain.ID = factory.AddSource(srcTrain);
            m_nSrcID1 = srcTrain.ID;
            SourceDescriptor srcTest = new SourceDescriptor(0, m_strSrc2, 2, 4, 3, false, true);
            srcTest.ID = factory.AddSource(srcTest);
            m_nSrcID2 = srcTest.ID;

            List<SourceDescriptor> rgSrcId = new List<SourceDescriptor>() { srcTrain, srcTest };

            for (int k = 0; k < 2; k++)
            {
                factory.Open(rgSrcId[k]);
                factory.DeleteSourceData();

                int nCount = factory.GetImageCount();
                for (int i = nCount; i < nCount + 5; i++)
                {
                    List<byte> rgData = new List<byte>();

                    for (int j = 0; j < 24; j++)
                    {
                        int datum = unique_pixels ? j : i;
                        rgData.Add((byte)datum);
                    }

                    int nLabel = i;

                    if (nMaxLabel > 0)
                        nLabel = nLabel % nMaxLabel;

                    SimpleDatum sd = new SimpleDatum(false, 2, 4, 3, nLabel, DateTime.Today, rgData, 0, false, i);
                    factory.PutRawImage(i, sd);
                }

                factory.Close();
            }

            DatasetDescriptor ds = new DatasetDescriptor(0, "test_data", null, null, srcTrain, srcTest, null, null);
            ds.ID = factory.AddDataset(ds);

            factory.UpdateDatasetCounts(ds.ID);
            m_nDsID = ds.ID;

            return m_strSrc1;
        }

        public string Fill2(int num_inputs)
        {
            DatasetFactory factory = new DatasetFactory();

            string strName = m_strSrc1 + ".x";
            m_log.WriteLine("Creating temporary dataset '" + m_strSrc1 + "'.");
            SourceDescriptor srcTrain = new SourceDescriptor(0, strName, 2, 4, 3, false, true);
            m_nSrcID1 = factory.AddSource(srcTrain);
            SourceDescriptor srcTest = new SourceDescriptor(0, strName + ".t", 2, 4, 3, false, true);
            m_nSrcID2 = factory.AddSource(srcTest);

            List<SourceDescriptor> rgSrcId = new List<SourceDescriptor>() { srcTrain, srcTest };

            for (int k = 0; k < 2; k++)
            {
                factory.Open(rgSrcId[k]);

                int nCount = factory.GetImageCount();
                for (int i = nCount; i < num_inputs; i++)
                {
                    List<byte> rgData = new List<byte>();
                    int nChannels = 2;
                    int nHeight = i % 2 + 1;
                    int nWidth = i % 4 + 1;
                    int nDataSize = nChannels * nHeight * nWidth;

                    for (int j = 0; j < nDataSize; j++)
                    {
                        rgData.Add((byte)j);
                    }

                    SimpleDatum sd = new SimpleDatum(false, nChannels, nWidth, nHeight, i, DateTime.Today, rgData, 0, false, i);
                    factory.PutRawImage(i, sd);
                }

                factory.Close();
            }

            DatasetDescriptor ds = new DatasetDescriptor(0, strName, null, null, srcTrain, srcTest, null, null);
            ds.ID = factory.AddDataset(ds);

            factory.UpdateDatasetCounts(ds.ID);
            m_nDsID = ds.ID;

            return strName;
        }

        public string Fill3(bool unique_pixels)
        {
            DatasetFactory factory = new DatasetFactory();

            m_log.WriteLine("Creating temporary dataset '" + m_strSrc1 + "'.");
            SourceDescriptor srcTrain = new SourceDescriptor(0, m_strSrc1, 2, 4, 3, false, true);
            srcTrain.ID = factory.AddSource(srcTrain);
            m_nSrcID1 = srcTrain.ID;
            SourceDescriptor srcTest = new SourceDescriptor(0, m_strSrc2, 2, 4, 3, false, true);
            srcTest.ID = factory.AddSource(srcTest);
            m_nSrcID2 = srcTest.ID;

            List<SourceDescriptor> rgSrcId = new List<SourceDescriptor>() { srcTrain, srcTest };

            factory.Open(rgSrcId[0]);
            factory.DeleteSourceData();

            int nIdx = 0;

            for (int k = 0; k < 10; k++)
            {
                for (int i = 0; i < 6; i++)
                {
                    List<byte> rgData = new List<byte>();

                    for (int j = 0; j < 24; j++)
                    {
                        int datum = unique_pixels ? j : i;
                        rgData.Add((byte)datum);
                    }

                    int nLabel = i;
                    SimpleDatum sd = new SimpleDatum(false, 2, 4, 3, nLabel, DateTime.Today, rgData, 0, false, nIdx);

                    if (k > 1 && k < 7)
                    {
                        if (i == 1 || i == 3)
                            sd.Boost = 1;
                        else if (i == 2 || i == 4)
                            sd.Boost = 2;
                    }

                    factory.PutRawImage(nIdx, sd);
                    nIdx++;
                }
            }

            factory.Close();

            DatasetDescriptor ds = new DatasetDescriptor(0, "test_data", null, null, srcTrain, srcTest, null, null);
            ds.ID = factory.AddDataset(ds);

            factory.UpdateDatasetCounts(ds.ID);
            m_nDsID = ds.ID;

            return m_strSrc1;
        }

        public void TestRead(DB_LOAD_METHOD loadMethod)
        {
            Assert.AreNotEqual(0, m_nSrcID1, "You must call 'Fill' first to set the source id!");
            double dfScale = 3;
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);
            p.phase = Phase.TRAIN;
            p.data_param.batch_size = 5;
            p.data_param.source = m_strSrc1;
            p.data_param.enable_random_selection = false;
            p.data_param.backend = DataParameter.DB.IMAGEDB;

            p.transform_param.scale = dfScale;

            IXImageDatabaseBase db = createImageDb(m_log);

            m_parent.Settings.DbLoadMethod = loadMethod;
            db.InitializeWithDsId1(m_parent.Settings, m_nDsID);
            CancelEvent evtCancel = new CancelEvent();

            DataLayer<T> layer = new DataLayer<T>(m_cuda, m_log, p, db, evtCancel);

            try
            {
                layer.Setup(BottomVec, TopVec);

                m_log.CHECK_EQ(Top.num, 5, "The top should have num = 5");
                m_log.CHECK_EQ(Top.channels, 2, "The top should have channels = 2");
                m_log.CHECK_EQ(Top.height, 3, "The top should have height = 3");
                m_log.CHECK_EQ(Top.width, 4, "The top should have width = 4");
                m_log.CHECK_EQ(TopLabel.num, 5, "The top label should have num = 5");
                m_log.CHECK_EQ(TopLabel.channels, 1, "The top label should have channels = 1");
                m_log.CHECK_EQ(TopLabel.height, 1, "The top label should have height = 1");
                m_log.CHECK_EQ(TopLabel.width, 1, "The top label should have width = 1");

                for (int iter = 0; iter < 100; iter++)
                {
                    layer.Forward(BottomVec, TopVec);

                    for (int i = 0; i < 5; i++)
                    {
                        double dfTopLabel = convert(TopLabel.GetData(i));
                        m_log.CHECK_EQ(i, dfTopLabel, "The top label value at " + i.ToString() + " is not correct.");
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 24; j++)
                        {
                            double dfValue = dfScale * i;
                            double dfTop = convert(Top.GetData(i * 24 + j));

                            m_log.CHECK_EQ(dfValue, dfTop, "debug : iter " + iter.ToString() + " i " + i.ToString() + " j " + j.ToString());
                        }
                    }
                }
            }
            finally
            {
                layer.Dispose();
                m_parent.CancelEvent.Reset();
                ((IDisposable)db).Dispose();
            }
        }

        public void TestSkip(DB_LOAD_METHOD loadMethod)
        {
            int nBatchSize = 5;
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);
            p.phase = Phase.TRAIN;
            p.data_param.batch_size = (uint)nBatchSize;
            p.data_param.source = m_strSrc1;
            p.data_param.enable_random_selection = false;
            p.data_param.backend = DataParameter.DB.IMAGEDB;

            IXImageDatabaseBase db = createImageDb(m_log);
            m_parent.Settings.DbLoadMethod = loadMethod;
            db.InitializeWithDsId1(m_parent.Settings, m_nDsID);
            CancelEvent evtCancel = new CancelEvent();

            try
            {
                int nSolverCount = 8;
                p.solver_count = nSolverCount;

                for (int dev = 0; dev < nSolverCount; dev++)
                {
                    int nSolverRank = dev;
                    p.solver_rank = nSolverRank;

                    DataLayer<T> layer = new DataLayer<T>(m_cuda, m_log, p, db, evtCancel);

                    try
                    {
                        layer.Setup(BottomVec, TopVec);

                        int nLabel = dev;

                        for (int iter = 0; iter < 10; iter++)
                        {
                            layer.Forward(BottomVec, TopVec);

                            double[] rgTopLabel = convert(TopLabel.update_cpu_data());

                            for (int i = 0; i < nBatchSize; i++)
                            {
                                m_log.CHECK_EQ(nLabel % nBatchSize, (int)rgTopLabel[i], "The label is not as expected at " + i.ToString());
                                nLabel += nSolverCount;
                            }
                        }
                    }
                    finally
                    {
                        layer.Dispose();
                        m_parent.CancelEvent.Reset();
                    }
                }
            }
            finally
            {
                ((IDisposable)db).Dispose();
            }
        }

        public void TestReshape(DB_LOAD_METHOD loadMethod)
        {
            int num_inputs = 5;
            // Save data of varying shapes

            string strName = Fill2(num_inputs);
            m_log.WriteLine("Using temporary dataset '" + strName + "'.");

            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);
            p.phase = Phase.TEST;
            p.data_param.batch_size = 1;
            p.data_param.source = strName;
            p.data_param.enable_random_selection = false;
            p.data_param.backend = DataParameter.DB.IMAGEDB;

            IXImageDatabaseBase db = createImageDb(m_log);
            m_parent.Settings.DbLoadMethod = loadMethod;
            db.InitializeWithDsId1(m_parent.Settings, m_nDsID);
            CancelEvent evtCancel = new CancelEvent();

            DataLayer<T> layer = new DataLayer<T>(m_cuda, m_log, p, db, evtCancel);

            try
            {
                layer.Setup(BottomVec, TopVec);

                m_log.CHECK_EQ(Top.num, 1, "The top should have num = 1");
                m_log.CHECK_EQ(Top.channels, 2, "The top should have channels = 2");
                m_log.CHECK_EQ(TopLabel.num, 1, "The top label should have num = 1");
                m_log.CHECK_EQ(TopLabel.channels, 1, "The top label should have channels = 1");
                m_log.CHECK_EQ(TopLabel.height, 1, "The top label should have height = 1");
                m_log.CHECK_EQ(TopLabel.width, 1, "The top label should have width = 1");

                for (int iter = 0; iter < num_inputs; iter++)
                {
                    layer.Forward(BottomVec, TopVec);

                    m_log.CHECK_EQ(Top.height, iter % 2 + 1, "The top height is not as expected.");
                    m_log.CHECK_EQ(Top.width, iter % 4 + 1, "The top width is not as expected.");
                    m_log.CHECK_EQ(iter, convert(TopLabel.GetData(0)), "The top label is not as expected.");

                    int nChannels = Top.channels;
                    int nHeight = Top.height;
                    int nWidth = Top.width;

                    for (int c = 0; c < nChannels; c++)
                    {
                        for (int h = 0; h < nHeight; h++)
                        {
                            for (int w = 0; w < nWidth; w++)
                            {
                                int nIdx = (c * nHeight + h) * nWidth + w;
                                m_log.CHECK_EQ(nIdx, (int)convert(Top.GetData(nIdx)), "dbug: iter " + iter.ToString() + " c " + c.ToString() + " h " + h.ToString() + " w " + w.ToString());
                            }
                        }
                    }
                }
            }
            finally
            {
                layer.Dispose();
                m_parent.CancelEvent.Reset();
                ((IDisposable)db).Dispose();
            }
        }

        public void TestReadCrop(Phase phase, DB_LOAD_METHOD loadMethod)
        {
            Assert.AreNotEqual(0, m_nSrcID1, "You must call 'Fill' first to set the source id!");
            m_log.WriteLine("Using temporary dataset '" + m_strSrc1 + "'.");
            double dfScale = 3;
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);
            p.phase = phase;
            p.data_param.batch_size = 5;
            p.data_param.source = m_strSrc1;
            p.data_param.enable_random_selection = false;
            p.data_param.backend = DataParameter.DB.IMAGEDB;

            p.transform_param.scale = dfScale;
            p.transform_param.crop_size = 1;
            p.transform_param.random_seed = 1701;

            IXImageDatabaseBase db = createImageDb(m_log);
            m_parent.Settings.DbLoadMethod = loadMethod;
            db.InitializeWithDsId1(m_parent.Settings, m_nDsID);
            CancelEvent evtCancel = new CancelEvent();

            DataLayer<T> layer = new DataLayer<T>(m_cuda, m_log, p, db, evtCancel);

            try
            {
                layer.Setup(BottomVec, TopVec);

                m_log.CHECK_EQ(Top.num, 5, "The top should have num = 5");
                m_log.CHECK_EQ(Top.channels, 2, "The top should have channels = 2");
                m_log.CHECK_EQ(Top.height, 1, "The top should have channels = 1");
                m_log.CHECK_EQ(Top.width, 1, "The top should have channels = 1");
                m_log.CHECK_EQ(TopLabel.num, 5, "The top label should have num = 5");
                m_log.CHECK_EQ(TopLabel.channels, 1, "The top label should have channels = 1");
                m_log.CHECK_EQ(TopLabel.height, 1, "The top label should have channels = 1");
                m_log.CHECK_EQ(TopLabel.width, 1, "The top label should have channels = 1");

                for (int iter = 0; iter < 2; iter++)
                {
                    layer.Forward(BottomVec, TopVec);

                    for (int i = 0; i < 5; i++)
                    {
                        m_log.CHECK_EQ(i, convert(TopLabel.GetData(i)), "The top label value at " + i.ToString() + " is not correct.");
                    }

                    int num_with_center_value = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            double dfCenterValue = dfScale * (j != 0 ? 17 : 5);
                            double dfTop = convert(Top.GetData(i * 2 + j));

                            if (dfCenterValue == dfTop)
                                num_with_center_value++;

                            // At TEST time, check that we always get center value.
                            if (phase == Phase.TEST)
                                m_log.CHECK_EQ(dfCenterValue, dfTop, "The center value '" + dfCenterValue.ToString() + "' should equal the top value '" + dfTop.ToString() + "', debug : iter " + iter.ToString() + " i " + i.ToString() + " j " + j.ToString());
                        }
                    }

                    // At TRAIN time, check that we did not get the center crop all 10 times.
                    // (This check fails with probability 1-1/12^10 in a correct
                    // implementation, so we set the random_seed param value).
                    if (phase == Phase.TRAIN)
                        m_log.CHECK_LT(num_with_center_value, 10, "The num_with_center_value should be less than 10");
                }
            }
            finally
            {
                layer.Dispose();
                m_parent.CancelEvent.Reset();
                ((IDisposable)db).Dispose();
            }
        }

        public void TestReadCropSequenceSeeded(DB_LOAD_METHOD loadMethod)
        {
            Assert.AreNotEqual(0, m_nSrcID1, "You must call 'Fill' first to set the source id!");
            m_log.WriteLine("Using temporary dataset '" + m_strSrc1 + "'.");
            double dfScale = 3;
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);
            p.phase = Phase.TRAIN;
            p.data_param.batch_size = 5;
            p.data_param.source = m_strSrc1;
            p.data_param.enable_random_selection = false;
            p.data_param.backend = DataParameter.DB.IMAGEDB;

            p.transform_param.scale = dfScale;
            p.transform_param.crop_size = 1;
            p.transform_param.mirror = true;
            p.transform_param.random_seed = 1701;

            m_parent.Settings.DbLoadMethod = loadMethod;
            IXImageDatabaseBase db = createImageDb(m_log);
            db.InitializeWithDsId1(m_parent.Settings, m_nDsID);
            CancelEvent evtCancel = new CancelEvent();

            // Get crop sequence with seed 1701.
            List<List<double>> crop_sequence = new List<List<double>>();
            {
                DataLayer<T> layer1 = new DataLayer<T>(m_cuda, m_log, p, db, evtCancel);

                try
                {
                    layer1.Setup(BottomVec, TopVec);

                    for (int iter = 0; iter < 2; iter++)
                    {
                        layer1.Forward(BottomVec, TopVec);

                        for (int i = 0; i < 5; i++)
                        {
                            m_log.CHECK_EQ(i, convert(TopLabel.GetData(i)), "The top label value at " + i.ToString() + " is not correct.");
                        }

                        List<double> iter_crop_sequence = new List<double>();

                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                double dfTop = convert(Top.GetData(i * 2 + j));
                                iter_crop_sequence.Add(dfTop);
                            }
                        }

                        crop_sequence.Add(iter_crop_sequence);
                    }
                }
                finally
                {
                    layer1.Dispose();
                    ((IDisposable)db).Dispose();
                }
            } // destroy 1st data layer and unlock the db.


            db = createImageDb(m_log);
            db.InitializeWithDsId1(m_parent.Settings, m_nDsID);

            // Get crop sequence after reseeding caffe (done within the data transformer)
            p.transform_param.random_seed = 1701;
            DataLayer<T> layer2 = new DataLayer<T>(m_cuda, m_log, p, db, evtCancel);

            try
            {
                layer2.Setup(BottomVec, TopVec);

                for (int iter = 0; iter < 2; iter++)
                {
                    layer2.Forward(BottomVec, TopVec);

                    for (int i = 0; i < 5; i++)
                    {
                        m_log.CHECK_EQ(i, convert(TopLabel.GetData(i)), "The top label value at " + i.ToString() + " is not correct.");
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            double dfValue = crop_sequence[iter][i * 2 + j];
                            double dfTop = convert(Top.GetData(i * 2 + j));
                            m_log.CHECK_EQ(dfValue, dfTop, "debug: iter " + iter.ToString() + " i " + i.ToString() + " j " + j.ToString());
                        }
                    }
                }
            }
            finally
            {
                layer2.Dispose();
                m_parent.CancelEvent.Reset();
                ((IDisposable)db).Dispose();
            }
        }

        public void TestReadCropSequenceUnSeeded(DB_LOAD_METHOD loadMethod)
        {
            Assert.AreNotEqual(0, m_nSrcID1, "You must call 'Fill' first to set the source id!");
            m_log.WriteLine("Using temporary dataset '" + m_strSrc1 + "'.");
            double dfScale = 3;
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);
            p.phase = Phase.TRAIN;
            p.data_param.batch_size = 5;
            p.data_param.source = m_strSrc1;
            p.data_param.enable_random_selection = false;
            p.data_param.backend = DataParameter.DB.IMAGEDB;

            p.transform_param.scale = dfScale;
            p.transform_param.crop_size = 1;
            p.transform_param.mirror = true;

            m_parent.Settings.DbLoadMethod = loadMethod;
            IXImageDatabaseBase db = createImageDb(m_log);
            db.InitializeWithDsId1(m_parent.Settings, m_nDsID);
            CancelEvent evtCancel = new CancelEvent();

            try
            {
                // Get crop sequence without setting a seed.
                List<List<double>> crop_sequence = new List<List<double>>();
                {
                    DataLayer<T> layer1 = new DataLayer<T>(m_cuda, m_log, p, db, evtCancel);

                    try
                    {
                        layer1.Setup(BottomVec, TopVec);

                        for (int iter = 0; iter < 2; iter++)
                        {
                            layer1.Forward(BottomVec, TopVec);

                            for (int i = 0; i < 5; i++)
                            {
                                m_log.CHECK_EQ(i, convert(TopLabel.GetData(i)), "The top label value at " + i.ToString() + " is not correct.");
                            }

                            List<double> iter_crop_sequence = new List<double>();

                            for (int i = 0; i < 5; i++)
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    double dfTop = convert(Top.GetData(i * 2 + j));
                                    iter_crop_sequence.Add(dfTop);
                                }
                            }

                            crop_sequence.Add(iter_crop_sequence);
                        }
                    }
                    finally
                    {
                        layer1.Dispose();
                    }
                } // destroy 1st data layer and unlock the db.


                // Get crop sequence without reseeding.  Check that the
                // sequence differs from the original.
                DataLayer<T> layer2 = new DataLayer<T>(m_cuda, m_log, p, db, evtCancel);

                try
                {
                    layer2.Setup(BottomVec, TopVec);

                    for (int iter = 0; iter < 2; iter++)
                    {
                        layer2.Forward(BottomVec, TopVec);

                        for (int i = 0; i < 5; i++)
                        {
                            m_log.CHECK_EQ(i, convert(TopLabel.GetData(i)), "The top label value at " + i.ToString() + " is not correct.");
                        }

                        int num_sequence_matches = 0;

                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                double dfValue = crop_sequence[iter][i * 2 + j];
                                double dfTop = convert(Top.GetData(i * 2 + j));

                                if (dfValue == dfTop)
                                    num_sequence_matches++;
                            }
                        }

                        m_log.CHECK_LT(num_sequence_matches, 10, "The number of sequence matches doesn't differ when it should.");
                    }
                }
                finally
                {
                    layer2.Dispose();
                    m_parent.CancelEvent.Reset();
                }
            }
            finally
            {
                ((IDisposable)db).Dispose();
            }
        }

        public void TestForward(string strSrc)
        {
            TestingProgressSet progress = new TestingProgressSet();
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);

            m_log.CHECK(p.data_param != null, "The data_param is null!");
            m_log.CHECK(p.transform_param != null, "The transform_para is null!");

            p.data_param.batch_size = 1;
            p.data_param.source = strSrc;
            p.data_param.enable_random_selection = false;
            p.data_param.enable_pair_selection = false;

            DataLayer<T> layer = new DataLayer<T>(m_cuda, m_log, p, m_parent.db, m_parent.CancelEvent);
            int nSrcID = m_parent.db.GetSourceID(strSrc);

            layer.LayerSetUp(BottomVec, TopVec);
            layer.Reshape(BottomVec, TopVec);

            int nCount = 1000;
            Stopwatch sw = new Stopwatch();
            Stopwatch swProgress = new Stopwatch();
            double dfTotalTime = 0;

            for (int i = 0; i < nCount; i++)
            {
                sw.Start();
                layer.Forward(BottomVec, TopVec);
                dfTotalTime += sw.ElapsedMilliseconds;
                sw.Stop();
                sw.Reset();

                if (i == 3)
                    Thread.Sleep(1000);

                m_log.CHECK_EQ(TopVec.Count, 2, "The top vec should have one element.");
                T[] rgData = TopVec[0].update_cpu_data();

                SimpleDatum d = m_parent.db.QueryItem(nSrcID, i, DB_LABEL_SELECTION_METHOD.NONE, DB_ITEM_SELECTION_METHOD.NONE);
                byte[] rgData2 = d.ByteData;

                m_log.CHECK_EQ(rgData2.Length, rgData.Length, "The data from the data forward should have the same length as the first item in the database for the source = " + strSrc);

                for (int j = 0; j < rgData.Length; j++)
                {
                    double dfVal1 = (double)Convert.ChangeType(rgData[j], typeof(double));
                    double dfVal2 = (double)Convert.ChangeType(rgData2[j], typeof(double));

                    m_log.CHECK_EQ(dfVal1, dfVal2, "The values at index " + j.ToString() + " for image at index " + i.ToString() + " in source = " + strSrc + " do not match!");
                }

                if (swProgress.Elapsed.TotalMilliseconds > 1000)
                {
                    progress.SetProgress((double)i / (double)nCount);
                    swProgress.Restart();
                }
            }

            string str = (dfTotalTime / (double)nCount).ToString() + " ms.";
            Trace.WriteLine("Average DataLayer Forward Time = " + str);

            layer.Dispose();
            m_parent.CancelEvent.Reset();

            progress.SetProgress(0);
            progress.Dispose();
        }

        public void TestForward2(string strSrc)
        {
            TestingProgressSet progress = new TestingProgressSet();
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);

            m_log.CHECK(p.data_param != null, "The data_param is null!");
            m_log.CHECK(p.transform_param != null, "The transform_para is null!");

            p.data_param.batch_size = 1;
            p.data_param.source = strSrc;
            p.data_param.enable_random_selection = true;
            p.data_param.enable_pair_selection = false;

            DataLayer<T> layer = new DataLayer<T>(m_cuda, m_log, p, m_parent.db, m_parent.CancelEvent);
            int nSrcID = m_parent.db.GetSourceID(strSrc);

            layer.LayerSetUp(BottomVec, TopVec);
            layer.Reshape(BottomVec, TopVec);

            int nCount = 1000;
            Stopwatch sw = new Stopwatch();
            Stopwatch swProgress = new Stopwatch();
            double dfTotalTime = 0;

            for (int i = 0; i < nCount; i++)
            {
                sw.Start();
                layer.Forward(BottomVec, TopVec);
                dfTotalTime += sw.ElapsedMilliseconds;
                sw.Stop();
                sw.Reset();

                m_log.CHECK_EQ(TopVec.Count, 2, "The top vec should have one element.");
                T[] rgData = TopVec[0].update_cpu_data();

                SimpleDatum d = m_parent.db.QueryItem(nSrcID, i, DB_LABEL_SELECTION_METHOD.NONE, DB_ITEM_SELECTION_METHOD.NONE);
                byte[] rgData2 = d.ByteData;

                m_log.CHECK_EQ(rgData2.Length, rgData.Length, "The data from the data forward should have the same length as the first item in the database for the source = " + strSrc);

                int nMatches = 0;

                for (int j = 0; j < rgData.Length; j++)
                {
                    double dfVal1 = (double)Convert.ChangeType(rgData[j], typeof(double));
                    double dfVal2 = (double)Convert.ChangeType(rgData2[j], typeof(double));

                    if (dfVal1 != 0 || dfVal2 != 0)
                    {
                        if (dfVal1 == dfVal2)
                            nMatches++;
                    }
                }

                m_log.CHECK_LE(nMatches, rgData.Length, "The images at index " + i.ToString() + " in source = " + strSrc + " should not match!");
           
                if (swProgress.Elapsed.TotalMilliseconds > 1000)
                {
                    progress.SetProgress((double)i / (double)nCount);
                    swProgress.Restart();
                }
            }

            string str = (dfTotalTime / (double)nCount).ToString() + " ms.";
            Trace.WriteLine("Average DataLayer Forward Time = " + str);

            layer.Dispose();
            m_parent.CancelEvent.Reset();

            progress.SetProgress(0);
            progress.Dispose();
        }

        public void TestForward_OneHotLabelConversion(string strSrc)
        {
            TestingProgressSet progress = new TestingProgressSet();
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);

            m_log.CHECK(p.data_param != null, "The data_param is null!");
            m_log.CHECK(p.transform_param != null, "The transform_para is null!");

            int nBatch = 3;
            p.data_param.batch_size = (uint)nBatch;
            p.data_param.source = strSrc;
            p.data_param.enable_random_selection = false;
            p.data_param.enable_pair_selection = false;
            p.data_param.one_hot_label_size = 16;
            p.data_param.label_type = LayerParameterBase.LABEL_TYPE.MULTIPLE;
            DataLayer<T> layer = new DataLayer<T>(m_cuda, m_log, p, m_parent.db, m_parent.CancelEvent);
            int nSrcID = m_parent.db.GetSourceID(strSrc);

            layer.LayerSetUp(BottomVec, TopVec);
            layer.Reshape(BottomVec, TopVec);

            int nCount = 1000;
            Stopwatch sw = new Stopwatch();
            Stopwatch swProgress = new Stopwatch();
            double dfTotalTime = 0;

            double[] rgOneHot = new double[p.data_param.one_hot_label_size];

            for (int i = 0; i < nCount; i++)
            {
                sw.Start();
                layer.Forward(BottomVec, TopVec);
                dfTotalTime += sw.ElapsedMilliseconds;
                sw.Stop();
                sw.Reset();

                m_log.CHECK_EQ(TopVec.Count, 2, "The top vec should have two elements: data and label.");
                T[] rgData = TopVec[0].update_cpu_data();
                int nDataDim = TopVec[0].count(1);

                T[] rgLabel = TopVec[1].update_cpu_data();
                int nLabelDim = TopVec[1].count(1);

                for (int k = 0; k < nBatch; k++)
                {
                    SimpleDatum d = m_parent.db.QueryItem(nSrcID, i * nBatch + k, DB_LABEL_SELECTION_METHOD.NONE, DB_ITEM_SELECTION_METHOD.NONE);
                    byte[] rgData2 = d.ByteData;
                    
                    m_log.CHECK_EQ(rgData2.Length, rgData.Length / nBatch, "The data from the data forward should have the same length as the first item in the database for the source = " + strSrc);

                    int nMatches = 0;

                    for (int j = 0; j < rgData2.Length; j++)
                    {
                        double dfVal1 = (double)Convert.ChangeType(rgData[k * nDataDim + j], typeof(double));
                        double dfVal2 = (double)Convert.ChangeType(rgData2[j], typeof(double));

                        if (dfVal1 != 0 || dfVal2 != 0)
                        {
                            if (dfVal1 == dfVal2)
                                nMatches++;
                        }
                    }

                    m_log.CHECK_LE(nMatches, rgData.Length, "The images at index " + i.ToString() + " in source = " + strSrc + " should not match!");

                    int nMask = 0x1;
                    for (int j = 0; j < p.data_param.one_hot_label_size; j++)
                    {
                        if ((d.Label & nMask) != 0)
                            rgOneHot[j] = 1;
                        else
                            rgOneHot[j] = 0;
                        nMask <<= 1;
                    }

                    m_log.CHECK_EQ(rgOneHot.Length, rgLabel.Length / nBatch, "The label from the data forward should have the same length as the first item in the database for the source = " + strSrc);
                    
                    for (int j = 0; j < rgOneHot.Length; j++)
                    {
                        double dfVal1 = (double)Convert.ChangeType(rgLabel[k * nLabelDim + j], typeof(double));
                        if (dfVal1 != rgOneHot[j])
                            m_log.FAIL("The label and expected one-hot vector do not match!");
                    }

                    if (swProgress.Elapsed.TotalMilliseconds > 1000)
                    {
                        progress.SetProgress((double)(i * nBatch + k) / (double)(nCount * nBatch));
                        swProgress.Restart();
                    }
                }
            }

            string str = (dfTotalTime / (double)nCount).ToString() + " ms.";
            Trace.WriteLine("Average DataLayer Forward Time = " + str);

            layer.Dispose();
            m_parent.CancelEvent.Reset();

            progress.SetProgress(0);
            progress.Dispose();
        }

        public void TestForward_TimeAlign(string strSrc, bool bIndexCheck)
        {
            TestingProgressSet progress = new TestingProgressSet();
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);

            m_log.CHECK(p.data_param != null, "The data_param is null!");
            m_log.CHECK(p.transform_param != null, "The transform_para is null!");

            int nBatch = 3;
            p.data_param.batch_size = (uint)nBatch;
            p.data_param.source = strSrc;
            p.data_param.enable_random_selection = false;
            p.data_param.enable_pair_selection = false;
            p.data_param.one_hot_label_size = 16;
            p.data_param.time_align = true;
            p.data_param.output_image_index = bIndexCheck;
            p.data_param.label_type = LayerParameterBase.LABEL_TYPE.SINGLE;
            DataLayer<T> layer = new DataLayer<T>(m_cuda, m_log, p, m_parent.db, m_parent.CancelEvent);
            int nSrcID = m_parent.db.GetSourceID(strSrc);

            if (bIndexCheck)
                TopVec.Add(m_blob_top_idx);

            layer.LayerSetUp(BottomVec, TopVec);
            layer.Reshape(BottomVec, TopVec);

            int nCount = 1000;
            Stopwatch sw = new Stopwatch();
            Stopwatch swProgress = new Stopwatch();
            double dfTotalTime = 0;

            Database db = new Database();
            db.Open(nSrcID);

            swProgress.Start();
            int nIdxNullCount = 0;
            int nIdxTotalCount = 0;

            for (int i = 0; i < nCount; i++)
            {
                sw.Start();
                layer.Forward(BottomVec, TopVec);
                dfTotalTime += sw.ElapsedMilliseconds;
                sw.Stop();
                sw.Reset();

                m_log.CHECK_EQ(TopVec.Count, (bIndexCheck) ? 3 : 2, "The top vec should have two elements: data, label and index.");
                T[] rgData = TopVec[0].update_cpu_data();
                int nDataDim = TopVec[0].count(1);

                T[] rgLabel = TopVec[1].update_cpu_data();
                int nLabelDim = TopVec[1].count(1);

                if (bIndexCheck)
                {
                    T[] rgIndex = TopVec[2].update_cpu_data();
                    int nIndexDim = TopVec[2].count(1);

                    for (int k = 0; k < nBatch; k++)
                    {
                        nIdxTotalCount++;

                        int nIdx = (int)Convert.ChangeType(rgIndex[k], typeof(int));
                        SimpleDatum d = m_parent.db.QueryItem(nSrcID, nIdx, DB_LABEL_SELECTION_METHOD.NONE, DB_ITEM_SELECTION_METHOD.NONE, null, false, false, false);
                        if (d == null)
                        {
                            nIdxNullCount++;
                            continue;
                        }

                        byte[] rgData2 = d.ByteData;

                        RawImage img = db.GetRawImageAt(d.Index);
                        m_log.CHECK_EQ(img.ID, d.ImageID, "The image ID's do not match!");
                        m_log.CHECK_EQ(img.ActiveLabel.Value, d.Label, "The image Labels do not match!");
                        m_log.CHECK_EQ(img.Height.Value, d.Height, "The image heights do not match!");
                        m_log.CHECK_EQ(img.Width.Value, d.Width, "The image widths do not match!");
                        m_log.CHECK(img.TimeStamp.Value == d.TimeStamp, "The image time stamps do not match!");

                        if (swProgress.Elapsed.TotalMilliseconds > 1000)
                        {
                            progress.SetProgress((double)(i * nBatch + k) / (double)(nCount * nBatch));
                            swProgress.Restart();
                        }
                    }
                }
            }

            db.Close();

            if (bIndexCheck)
                Trace.WriteLine("Null Images Found = " + nIdxNullCount.ToString() + " out of " + nIdxTotalCount.ToString() + " (" + ((double)nIdxNullCount/nIdxTotalCount).ToString("P") + ")");

            string str = (dfTotalTime / (double)nCount).ToString() + " ms.";
            Trace.WriteLine("Average DataLayer Forward Time = " + str);

            layer.Dispose();
            m_parent.CancelEvent.Reset();

            progress.SetProgress(0);
            progress.Dispose();
        }

        public void TestForwardMask(string strSrc, int nImagesPerBlob = 1)
        {
            LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);

            int nSrcID = m_parent.db.GetSourceID(strSrc);
            SourceDescriptor src = m_parent.db.GetSourceById(nSrcID);

            p.data_param.batch_size = 6;
            p.data_param.images_per_blob = nImagesPerBlob;
            p.data_param.output_all_labels = (nImagesPerBlob > 1) ? true : false;
            p.data_param.enable_noise_for_nonmatch = (nImagesPerBlob > 1) ? true : false;
            p.data_param.data_noise_param.use_noisy_mean = true;
            p.data_param.balance_matches = false;
            p.data_param.source = strSrc;
            p.data_param.enable_random_selection = false;
            p.data_param.enable_pair_selection = false;

            int nL = (int)(src.Width * 0.5);
            int nR = src.Width;
            int nT = (int)(src.Height * 0.25);
            int nB = (int)(src.Height * 0.75);

            p.transform_param.mask_param.Active = false;
            p.transform_param.mask_param.boundary_left = nL;
            p.transform_param.mask_param.boundary_right = nR;
            p.transform_param.mask_param.boundary_top = nT;
            p.transform_param.mask_param.boundary_bottom = nB;

            double dfAveBlack = 0;
            DataLayer<T> layer = new DataLayer<T>(m_cuda, m_log, p, m_parent.db, m_parent.CancelEvent);

            try
            {
                layer.LayerSetUp(BottomVec, TopVec);
                layer.Reshape(BottomVec, TopVec);

                int nTotalCount = 0;
                double dfTotalBlack = 0;

                for (int i = 0; i < 10; i++)
                {
                    layer.Forward(BottomVec, TopVec);

                    int nDim = src.Channels * src.Height * src.Width;
                    float[] rgData1 = new float[nDim];
                    float[] rgData = convertF(TopVec[0].mutable_cpu_data);

                    for (int n = 0; n < 6; n++)
                    {
                        for (int j = 0; j < nImagesPerBlob; j++)
                        {
                            Array.Copy(rgData, n * nImagesPerBlob * nDim + j * nDim, rgData1, 0, nDim);
                            byte[] rgb = rgData1.Select(pv => Math.Min((byte)pv, (byte)255)).ToArray();

                            SimpleDatum sd = new SimpleDatum(false, src.Channels, src.Width, src.Height, -1, DateTime.MinValue, rgb, 0, false, 0);
                            Bitmap bmp = ImageData.GetImage(sd);
                            //bmp.Save("c:\\temp\\img_" + n.ToString() + "_" + j.ToString() + ".png");
                            LockBitmap bmp1 = new LockBitmap(bmp);
                            bmp1.LockBits();

                            int nNonBlackCount = 0;
                            int nNonBlackTotal = 0;

                            for (int y = 0; y < bmp.Height; y++)
                            {
                                for (int x = 0; x < bmp.Width; x++)
                                {
                                    Color clr = bmp1.GetPixel(x, y);

                                    if (clr != Color.Black)
                                        nNonBlackCount++;

                                    nNonBlackTotal++;
                                }
                            }

                            double dfNonBlackPct = (nNonBlackTotal == 0) ? 0 : (double)nNonBlackCount / (double)nNonBlackTotal;
                            dfTotalBlack += dfNonBlackPct;
                            nTotalCount++;

                            m_log.CHECK_GE(dfNonBlackPct, 0.75, "The non black percent should be >= 75%!");

                            bmp1.UnlockBits();
                            bmp.Dispose();
                        }
                    }
                }

                // Verify that when enabled, the images are masked out.
                layer.Dispose();
                p.transform_param.mask_param.Active = true;
                layer = new DataLayer<T>(m_cuda, m_log, p, m_parent.db, m_parent.CancelEvent);

                layer.LayerSetUp(BottomVec, TopVec);
                layer.Reshape(BottomVec, TopVec);

                for (int i = 0; i < 10; i++)
                {
                    layer.Forward(BottomVec, TopVec);

                    int nDim = src.Channels * src.Height * src.Width;
                    float[] rgData1 = new float[nDim];
                    float[] rgData = convertF(TopVec[0].mutable_cpu_data);

                    for (int n = 0; n < 6; n++)
                    {
                        for (int j = 0; j < nImagesPerBlob; j++)
                        {
                            Array.Copy(rgData, n * nImagesPerBlob * nDim + j * nDim, rgData1, 0, nDim);
                            byte[] rgb = rgData1.Select(pv => Math.Min((byte)pv, (byte)255)).ToArray();

                            SimpleDatum sd = new SimpleDatum(false, src.Channels, src.Width, src.Height, -1, DateTime.MinValue, rgb, 0, false, 0);
                            Bitmap bmp = ImageData.GetImage(sd);
                            //bmp.Save("c:\\temp\\img_masked_" + n.ToString() + "_" + j.ToString() + ".png");
                            LockBitmap bmp1 = new LockBitmap(bmp);
                            bmp1.LockBits();

                            int nNonBlackCount = 0;
                            int nNonBlackTotal = 0;

                            for (int y = 0; y < bmp.Height; y++)
                            {
                                for (int x = 0; x < bmp.Width; x++)
                                {
                                    Color clr = bmp1.GetPixel(x, y);

                                    if (y >= nT && y <= nB && x >= nL && x <= nR)
                                        m_log.CHECK(clr.R == 0 && clr.G == 0 && clr.G == 0, "The pixel " + x.ToString() + "," + y.ToString() + " is within the mask and should be black!");
                                    else
                                    {
                                        if (clr != Color.Black)
                                            nNonBlackCount++;

                                        nNonBlackTotal++;
                                    }
                                }
                            }

                            double dfNonBlackPct = (nNonBlackTotal == 0) ? 0 : (double)nNonBlackCount / (double)nNonBlackTotal;
                            m_log.CHECK_GE(dfNonBlackPct, dfAveBlack * 0.8, "The non black percent should be >= 80% of the average of " + dfAveBlack.ToString("P") + "!");

                            bmp1.UnlockBits();
                            bmp.Dispose();
                        }
                    }
                }
            }
            finally
            {
                layer.Dispose();
            }
        }

        public void TestForwardPairs(string strSrc, int nImagesPerBlob = 1)
        {
            Layer<T> layer = null;

            try
            {
                TestingProgressSet progress = new TestingProgressSet();
                LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);

                m_log.CHECK(p.data_param != null, "The data_param is null!");
                m_log.CHECK(p.transform_param != null, "The transform_para is null!");

                p.data_param.batch_size = 6;
                p.data_param.images_per_blob = nImagesPerBlob;
                p.data_param.output_all_labels = (nImagesPerBlob > 1) ? true : false;
                p.data_param.balance_matches = false;
                p.data_param.source = strSrc;
                p.data_param.enable_random_selection = false;
                p.data_param.enable_pair_selection = false;

                layer = new DataLayer<T>(m_cuda, m_log, p, m_parent.db, m_parent.CancelEvent);
                int nSrcID = m_parent.db.GetSourceID(strSrc);

                SourceDescriptor src = m_parent.db.GetSourceById(nSrcID);

                layer.LayerSetUp(BottomVec, TopVec);
                layer.Reshape(BottomVec, TopVec);

                int nCount = 1000;
                Stopwatch sw = new Stopwatch();
                Stopwatch swProgress = new Stopwatch();
                double dfTotalTime = 0;
                int nIdx = 0;

                swProgress.Start();

                for (int i = 0; i < nCount; i++)
                {
                    sw.Start();
                    layer.Forward(BottomVec, TopVec);
                    dfTotalTime += sw.ElapsedMilliseconds;
                    sw.Stop();
                    sw.Reset();

                    if (i == 3)
                        Thread.Sleep(1000);

                    m_log.CHECK_EQ(TopVec.Count, 2, "The top vec should have two elements (data, labels).");
                    m_log.CHECK_EQ(TopVec[0].num, p.data_param.batch_size, "The data num should = the batch size.");
                    m_log.CHECK_EQ(TopVec[1].num, p.data_param.batch_size, "The label num should = the batch size.");

                    m_log.CHECK_EQ(TopVec[0].channels, src.Channels * p.data_param.images_per_blob, "The data should have channels = " + (src.Channels * p.data_param.images_per_blob).ToString());
                    m_log.CHECK_EQ(TopVec[0].height, src.Height, "The data should have height = " + src.Height.ToString());
                    m_log.CHECK_EQ(TopVec[0].width, src.Width, "The data should have width = " + src.Width.ToString());

                    if (p.data_param.images_per_blob > 1)
                    {
                        if (p.data_param.output_all_labels)
                            m_log.CHECK_EQ(TopVec[1].channels, p.data_param.images_per_blob, "The label channels should = " + p.data_param.images_per_blob.ToString());
                        else
                            m_log.CHECK_EQ(TopVec[1].channels, 1, "The label channels should = 1");
                    }
                    else
                    {
                        m_log.CHECK_EQ(TopVec[1].channels, 1, "The label channels should = 1");
                    }

                    // Check the data.
                    T[] rgData = TopVec[0].update_cpu_data();
                    int nDataDim = TopVec[0].count(1);
                    List<List<SimpleDatum>> rgrgData = new List<List<SimpleDatum>>();

                    for (int j = 0; j < p.data_param.batch_size; j++)
                    {
                        SimpleDatum d = m_parent.db.QueryItem(nSrcID, nIdx, DB_LABEL_SELECTION_METHOD.NONE, DB_ITEM_SELECTION_METHOD.NONE);
                        nIdx++;

                        List<SimpleDatum> rgD = new List<SimpleDatum>();
                        if (p.data_param.images_per_blob > 1)
                        {
                            for (int k = 0; k < p.data_param.images_per_blob - 1; k++)
                            {
                                rgD.Add(m_parent.db.QueryItem(nSrcID, nIdx, DB_LABEL_SELECTION_METHOD.NONE, DB_ITEM_SELECTION_METHOD.NONE));
                                nIdx++;
                            }

                            rgD.Insert(0, d);
                            rgrgData.Add(rgD);
                        }
                        else
                        {
                            rgrgData.Add(new List<SimpleDatum>() { d });
                        }

                        List<byte> rgFullData = new List<byte>();
                        rgFullData.AddRange(d.ByteData);

                        for (int k = 1; k < rgD.Count; k++)
                        {
                            rgFullData.AddRange(rgD[k].ByteData);
                        }

                        m_log.CHECK_EQ(rgFullData.Count, rgData.Length / p.data_param.batch_size, "The data from the data forward should have the same length as the first item in the database for the source = " + strSrc);

                        for (int k = 0; k < rgFullData.Count; k++)
                        {
                            double dfVal1 = (double)Convert.ChangeType(rgData[j * nDataDim + k], typeof(double));
                            double dfVal2 = (double)Convert.ChangeType(rgFullData[k], typeof(double));

                            m_log.CHECK_EQ(dfVal1, dfVal2, "The values at index " + k.ToString() + " for image at index " + i.ToString() + " in source = " + strSrc + " do not match!");
                        }
                    }

                    // Check the label
                    T[] rgLabel = TopVec[1].update_cpu_data();
                    int nLabelDim = 1;

                    if (p.data_param.images_per_blob > 1 && p.data_param.output_all_labels)
                        nLabelDim = p.data_param.images_per_blob;

                    for (int j = 0; j < p.data_param.batch_size; j++)
                    {
                        if (p.data_param.images_per_blob > 1)
                        {
                            if (p.data_param.output_all_labels)
                            {
                                List<double> rgLabels = new List<double>();

                                for (int k = 0; k < p.data_param.images_per_blob; k++)
                                {
                                    double dfLabel = convert(rgLabel[j * nLabelDim + k]);
                                    rgLabels.Add(dfLabel);
                                }

                                m_log.CHECK_EQ(rgLabels.Count, p.data_param.images_per_blob, "There should only be " + p.data_param.images_per_blob.ToString() + " labels.");

                                if (rgLabels.Count == 2)
                                {
                                    m_log.CHECK_EQ(rgLabels[0], rgrgData[j][0].Label, "The labels do not match!");
                                    m_log.CHECK_EQ(rgLabels[1], rgrgData[j][1].Label, "The labels do not match!");
                                }
                            }
                        }
                        else
                        {
                            double dfLabel = convert(rgLabel[j]);
                            m_log.CHECK_EQ(dfLabel, rgrgData[j][0].Label, "The labels do not match.");
                        }
                    }

                    if (swProgress.Elapsed.TotalMilliseconds > 1000)
                    {
                        progress.SetProgress((double)i / (double)nCount);
                        swProgress.Restart();
                    }
                }

                string str = (dfTotalTime / (double)nCount).ToString() + " ms.";
                Trace.WriteLine("Average DataLayer Forward Time = " + str);

                m_parent.CancelEvent.Reset();

                progress.SetProgress(0);
                progress.Dispose();
            }
            finally
            {
                if (layer != null)
                    layer.Dispose();
            }
        }

        public void TestDataLabelMapping(DB_LOAD_METHOD loadMethod)
        {
            Layer<T> layer = null;
            IXImageDatabaseBase db = null;

            try
            {
                LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);
                p.phase = Phase.TRAIN;
                p.data_param.batch_size = 5;
                p.data_param.source = m_strSrc1;
                p.data_param.enable_random_selection = true;
                p.data_param.backend = DataParameter.DB.IMAGEDB;

                m_parent.Settings.DbLoadMethod = loadMethod;


                // Verify no label mapping by default
                db = createImageDb(m_log, 1701);
                db.InitializeWithDsId1(m_parent.Settings, m_nDsID);
                CancelEvent evtCancel = new CancelEvent();
                DatasetDescriptor ds = db.GetDatasetById(m_nDsID);
                List<int> rgOriginalLabels = new List<int>();
                Dictionary<int, int> rgLabelCounts = new Dictionary<int, int>();

                layer = Layer<T>.Create(m_cuda, m_log, p, evtCancel, db);
                layer.Setup(BottomVec, TopVec);

                for (int i = 0; i < 5; i++)
                {
                    layer.Forward(BottomVec, TopVec);

                    Blob<T> blobLabel = TopVec[1];
                    float[] rgLabel = convertF(blobLabel.mutable_cpu_data);

                    m_log.CHECK_EQ(rgLabel.Length, p.data_param.batch_size, "The label count should equal the batch size!");

                    for (int j = 0; j < rgLabel.Length; j++)
                    {
                        int nLabel = (int)rgLabel[j];
                        rgOriginalLabels.Add(nLabel);

                        if (!rgLabelCounts.ContainsKey(nLabel))
                            rgLabelCounts.Add(nLabel, 1);
                        else
                            rgLabelCounts[nLabel]++;
                    }
                }

                layer.Dispose();
                ((IDisposable)db).Dispose();

                m_log.CHECK_EQ(rgLabelCounts.Count, 5, "There should be 5 labels!");


                // Map even labels to 0 and odd labels to 1.
                int nIdx = 0;
                db = createImageDb(m_log, 1701);
                db.InitializeWithDsId1(m_parent.Settings, m_nDsID);

                p.transform_param.label_mapping.Active = true;
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(0, 0, null, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(1, 1, null, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(2, 0, null, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(3, 1, null, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(4, 0, null, null));

                layer = Layer<T>.Create(m_cuda, m_log, p, evtCancel, db);
                layer.Setup(BottomVec, TopVec);

                for (int i = 0; i < 5; i++)
                {
                    layer.Forward(BottomVec, TopVec);

                    Blob<T> blobLabel = TopVec[1];
                    float[] rgLabel = convertF(blobLabel.mutable_cpu_data);

                    m_log.CHECK_EQ(rgLabel.Length, p.data_param.batch_size, "The label count should equal the batch size!");

                    for (int j = 0; j < rgLabel.Length; j++)
                    {
                        int nLabel = (int)rgLabel[j];
                        int nExpectedLabel = rgOriginalLabels[nIdx] % 2;

                        m_log.CHECK_EQ(nLabel, nExpectedLabel, "The labels do not match!");
                        nIdx++;
                    }
                }
            }
            finally
            {
                if (layer != null)
                    layer.Dispose();

                if (db != null)
                    ((IDisposable)db).Dispose();
            }
        }

        public void TestDataLabelMappingWithBoost(DB_LOAD_METHOD loadMethod)
        {
            Layer<T> layer = null;
            IXImageDatabaseBase db = null;

            try
            {
                LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);
                p.phase = Phase.TRAIN;
                p.data_param.batch_size = 5;
                p.data_param.source = m_strSrc1;
                p.data_param.enable_random_selection = true;
                p.data_param.backend = DataParameter.DB.IMAGEDB;

                p.data_param.enable_debug_output = true;
                p.data_param.data_debug_param.debug_save_path = getTestPath("\\MyCaffe\\test_data\\test", true, true);
                p.data_param.data_debug_param.iterations = int.MaxValue;

                m_parent.Settings.DbLoadMethod = loadMethod;
                deleteFiles(p.data_param.data_debug_param.debug_save_path);

                // Verify no label mapping by default
                db = createImageDb(m_log, 1701);
                db.InitializeWithDsId1(m_parent.Settings, m_nDsID);
                CancelEvent evtCancel = new CancelEvent();
                DatasetDescriptor ds = db.GetDatasetById(m_nDsID);
                List<int> rgOriginalLabels = new List<int>();
                Dictionary<int, int> rgLabelCounts = new Dictionary<int, int>();

                layer = Layer<T>.Create(m_cuda, m_log, p, evtCancel, db);
                layer.Setup(BottomVec, TopVec);

                for (int i = 0; i < 5; i++)
                {
                    layer.Forward(BottomVec, TopVec);

                    Blob<T> blobLabel = TopVec[1];
                    float[] rgLabel = convertF(blobLabel.mutable_cpu_data);

                    m_log.CHECK_EQ(rgLabel.Length, p.data_param.batch_size, "The label count should equal the batch size!");

                    for (int j = 0; j < rgLabel.Length; j++)
                    {
                        int nLabel = (int)rgLabel[j];
                        rgOriginalLabels.Add(nLabel);

                        if (!rgLabelCounts.ContainsKey(nLabel))
                            rgLabelCounts.Add(nLabel, 1);
                        else
                            rgLabelCounts[nLabel]++;
                    }
                }

                layer.Dispose();
                ((IDisposable)db).Dispose();

                m_log.CHECK_EQ(rgLabelCounts.Count, 6, "Only " + rgLabelCounts.Count.ToString() + " labels found - There should be 6 labels!");
                List<SimpleDatum> rgExpectedData = SimpleDatum.LoadFromPath(p.data_param.data_debug_param.debug_save_path);


                // Map even labels to 0 and odd labels to 1.
                int nIdx = 0;
                db = createImageDb(m_log, 1701);
                db.InitializeWithDsId1(m_parent.Settings, m_nDsID);

                // Set labels to their boost value and 0 for all others.
                p.data_param.enable_debug_output = false;
                p.transform_param.label_mapping.Active = true;
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(0, 0, 0, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(1, 0, 0, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(2, 0, 0, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(3, 0, 0, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(4, 0, 0, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(5, 0, 0, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(1, 1, 1, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(2, 2, 2, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(3, 1, 1, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(4, 2, 2, null));

                layer = Layer<T>.Create(m_cuda, m_log, p, evtCancel, db);
                layer.Setup(BottomVec, TopVec);

                for (int i = 0; i < 5; i++)
                {
                    layer.Forward(BottomVec, TopVec);

                    Blob<T> blobLabel = TopVec[1];
                    float[] rgLabel = convertF(blobLabel.mutable_cpu_data);

                    m_log.CHECK_EQ(rgLabel.Length, p.data_param.batch_size, "The label count should equal the batch size!");

                    for (int j = 0; j < rgLabel.Length; j++)
                    {
                        int nLabel = (int)rgLabel[j];
                        int nExpectedLabel = rgOriginalLabels[nIdx];
                        SimpleDatum sd = rgExpectedData[nIdx];

                        // Verify sequencing.
                        m_log.CHECK_EQ(nExpectedLabel, sd.Label, "The labels do not match!");

                        nExpectedLabel = sd.Boost;

                        m_log.CHECK_EQ(nLabel, nExpectedLabel, "The labels do not match!");
                        nIdx++;
                    }
                }
            }
            finally
            {
                if (layer != null)
                    layer.Dispose();
                if (db != null)
                    ((IDisposable)db).Dispose();
            }
        }

        public void TestDataLabelMappingWithBoostAndFalseCondition(DB_LOAD_METHOD loadMethod)
        {
            Layer<T> layer = null;
            IXImageDatabaseBase db = null;

            try
            {
                LayerParameter p = new LayerParameter(LayerParameter.LayerType.DATA);
                p.phase = Phase.TRAIN;
                p.data_param.batch_size = 5;
                p.data_param.source = m_strSrc1;
                p.data_param.enable_random_selection = true;
                p.data_param.backend = DataParameter.DB.IMAGEDB;

                p.data_param.enable_debug_output = true;
                p.data_param.data_debug_param.debug_save_path = getTestPath("\\MyCaffe\\test_data\\test", true, true);
                p.data_param.data_debug_param.iterations = int.MaxValue;

                m_parent.Settings.DbLoadMethod = loadMethod;
                deleteFiles(p.data_param.data_debug_param.debug_save_path);

                // Verify no label mapping by default
                db = createImageDb(m_log, 1701);
                db.InitializeWithDsId1(m_parent.Settings, m_nDsID);
                CancelEvent evtCancel = new CancelEvent();
                DatasetDescriptor ds = db.GetDatasetById(m_nDsID);
                List<int> rgOriginalLabels = new List<int>();
                Dictionary<int, int> rgLabelCounts = new Dictionary<int, int>();

                layer = Layer<T>.Create(m_cuda, m_log, p, evtCancel, db);
                layer.Setup(BottomVec, TopVec);

                for (int i = 0; i < 5; i++)
                {
                    layer.Forward(BottomVec, TopVec);

                    Blob<T> blobLabel = TopVec[1];
                    float[] rgLabel = convertF(blobLabel.mutable_cpu_data);

                    m_log.CHECK_EQ(rgLabel.Length, p.data_param.batch_size, "The label count should equal the batch size!");

                    for (int j = 0; j < rgLabel.Length; j++)
                    {
                        int nLabel = (int)rgLabel[j];
                        rgOriginalLabels.Add(nLabel);

                        if (!rgLabelCounts.ContainsKey(nLabel))
                            rgLabelCounts.Add(nLabel, 1);
                        else
                            rgLabelCounts[nLabel]++;
                    }
                }

                layer.Dispose();
                ((IDisposable)db).Dispose();

                m_log.CHECK_EQ(rgLabelCounts.Count, 6, "Only " + rgLabelCounts.Count.ToString() + " labels found - There should be 6 labels!");
                List<SimpleDatum> rgExpectedData = SimpleDatum.LoadFromPath(p.data_param.data_debug_param.debug_save_path);


                // Map even labels to 0 and odd labels to 1.
                int nIdx = 0;
                db = createImageDb(m_log, 1701);
                db.InitializeWithDsId1(m_parent.Settings, m_nDsID);

                // Set labels to their boost value and 0 for all others.
                p.data_param.enable_debug_output = false;
                p.transform_param.label_mapping.Active = true;
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(5, 0, 0, null));
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(1, 1, 1, 0)); // Map label 1 -> 1 if boost == 1, 0 otherwise.
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(2, 2, 2, 0)); // Map label 2 -> 2 if boost == 2, 0 otherwise.
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(3, 1, 1, 0)); // Map label 3 -> 1 if boost == 1, 0 otherwise.
                p.transform_param.label_mapping.mapping.Add(new LabelMapping(4, 2, 2, 0)); // Map label 4 -> 2 if boost == 2, 0 otherwise.

                layer = Layer<T>.Create(m_cuda, m_log, p, evtCancel, db);
                layer.Setup(BottomVec, TopVec);

                for (int i = 0; i < 5; i++)
                {
                    layer.Forward(BottomVec, TopVec);

                    Blob<T> blobLabel = TopVec[1];
                    float[] rgLabel = convertF(blobLabel.mutable_cpu_data);

                    m_log.CHECK_EQ(rgLabel.Length, p.data_param.batch_size, "The label count should equal the batch size!");

                    for (int j = 0; j < rgLabel.Length; j++)
                    {
                        int nLabel = (int)rgLabel[j];
                        int nExpectedLabel = rgOriginalLabels[nIdx];
                        SimpleDatum sd = rgExpectedData[nIdx];

                        // Verify sequencing.
                        m_log.CHECK_EQ(nExpectedLabel, sd.Label, "The labels do not match!");

                        nExpectedLabel = sd.Boost;

                        m_log.CHECK_EQ(nLabel, nExpectedLabel, "The labels do not match!");
                        nIdx++;
                    }
                }
            }
            finally
            {
                if (layer != null)
                    layer.Dispose();
                if (db != null)
                    ((IDisposable)db).Dispose();
            }
        }

        private void deleteFiles(string strPath)
        {
            string[] rgstrFiles = Directory.GetFiles(strPath);

            foreach (string strFile in rgstrFiles)
            {
                File.Delete(strFile);
            }
        }
    }
}
