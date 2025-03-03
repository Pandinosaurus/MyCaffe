﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.IO;
using MyCaffe.basecode;
using System.ComponentModel;
using System.Runtime.Remoting.Channels;
using System.Xml.Linq;
using System.Collections.Specialized;

namespace MyCaffe.common
{
    /// <summary>
    /// Specifies the function indexes supported by the Low-Level Cuda Extension DLL for LLM's.
    /// </summary>
    /// <remarks><b>IMPORTANT:</b> These index values must match the index values 
    /// specified within the Low-Level Cuda Extension DLL for LLMs.</remarks>
    public enum CUDAFN_EXTENSION_LLM
    {
        /// <summary>
        /// Specifies to create a new LLM inference engine.
        /// </summary>
        CREATE = 1,
        /// <summary>
        /// Specifies to destroy an existing LLM inference engine.
        /// </summary>
        DESTROY = 2,
        /// <summary>
        /// Specifies to load a model into an LLM inference engine.
        /// </summary>
        LOAD = 3,
        /// <summary>
        /// Specifies to query the status of a LLM inference engine.
        /// </summary>
        QUERY_STATUS = 4,
        /// <summary>
        /// Specifies to generate text using a LLM inference engine.
        /// </summary>
        GENERATE = 5,
        /// <summary>
        /// Specifies to query the generated response of an LLM inference engine.
        /// </summary>
        QUERY_RESPONSE = 6,
        /// <summary>
        /// Specifies to abort generating a response.
        /// </summary>
        ABORT_GENERATE = 7
    }

    /// <summary>
    /// Defines the direction of data flow.
    /// </summary>
    public enum DIR
    {
        /// <summary>
        /// Specifies data is moving forward.
        /// </summary>
        FWD = 0,
        /// <summary>
        /// Specifies data is moving backward.
        /// </summary>
        BWD = 1
    }

    /// <summary>
    /// Defines the side where the threshold is applied.
    /// </summary>
    public enum SIDE
    {
        /// <summary>
        /// Specifies values above the threshold value.
        /// </summary>
        BELOW = 0,
        /// <summary>
        /// Specifies values below the threshold value.
        /// </summary>
        ABOVE = 1
    }

    /// <summary>
    /// Defines the type of Mean Error to use.
    /// </summary>
    public enum MEAN_ERROR
    {
        /// <summary>
        /// Mean Squared Error (MSE)
        /// @f$ L(y, y\hat) = \frac{1}{N} \sum_{i=0}^{N} (y - \hat{y}{i})^2 @f$ where @f$ \hat{y} @f$ is the predicted value.
        /// </summary>
        MSE = 1,
        /// Mean Absolute Error (MAE)
        /// @f$ L(y, y\hat) = \frac{1}{N} \sum_{i=0}^{N} |y - \hat{y}{i}| @f$ where @f$ \hat{y} @f$ is the predicted value.
        MAE = 2
    }

    /// <summary>
    /// Defines the mathematical function to run.
    /// </summary>
    public enum MATH_FUNCTION
    {
        /// <summary>
        /// Specifies to run a no operation.
        /// </summary>
        NOP = 0,

        /// <summary>
        /// Specifies to run the acos function.
        /// </summary>
        ACOS = 1,
        /// <summary>
        /// Specifies to run the acosh function.
        /// </summary>
        ACOSH = 2,
        /// <summary>
        /// Specifies to run the cos function.
        /// </summary>
        COS = 3,
        /// <summary>
        /// Specifies to run the cosh function.
        /// </summary>
        COSH = 4,

        /// <summary>
        /// Specifies to run the asin function.
        /// </summary>
        ASIN = 10,
        /// <summary>
        /// Specifies to run the asinh function.
        /// </summary>
        ASINH = 11,
        /// <summary>
        /// Specifies to run the sin function.
        /// </summary>
        SIN = 12,
        /// <summary>
        /// Specifies to run the sinh function.
        /// </summary>
        SINH = 13,

        /// <summary>
        /// Specifies to run the atan function.
        /// </summary>
        ATAN = 20,
        /// <summary>
        /// Specifies to run the atanh function.
        /// </summary>
        ATANH = 21,
        /// <summary>
        /// Specifies to run the tan function.
        /// </summary>
        TAN = 22,
        /// <summary>
        /// Specifies to run the tanh function.
        /// </summary>
        TANH = 23,

        /// <summary>
        /// Specifies to run the ceil function.
        /// </summary>
        CEIL = 30,
        /// <summary>
        /// Specifies to run the floor function.
        /// </summary>
        FLOOR = 31,
        /// <summary>
        /// Specifies to flip the sign of the inputs.
        /// </summary>
        NEG = 32,
        /// <summary>
        /// Specifies to run the sign function.
        /// </summary>
        SIGN = 33,
        /// <summary>
        /// Specifies to run the sqrt function.
        /// </summary>
        SQRT = 34
    }

    /// <summary>
    /// Defines the operations performed by the channel_op function.
    /// </summary>
    public enum OP
    {
        /// <summary>
        /// Specifies to perform a multiplication operation.
        /// </summary>
        MUL = 1,
        /// <summary>
        /// Specifies to perform a division operation.
        /// </summary>
        DIV = 2,
        /// <summary>
        /// Specifies to perform an addition operation.
        /// </summary>
        ADD = 3,
        /// <summary>
        /// Specifies to perform a subtraction operation.
        /// </summary>
        SUB = 4
    }


    /// <summary>
    /// Specifies the distance method used when calculating batch distances.
    /// </summary>
    public enum DistanceMethod
    {
        /// <summary>
        /// Specifies to calculate the hamming distance.
        /// </summary>
        HAMMING = 0,
        /// <summary>
        /// Specifies to calculate the euclidean distance.
        /// </summary>
        EUCLIDEAN = 1
    }

    /// <summary>
    /// Specifies the pooling method used by the cuDnn function SetPoolingDesc.
    /// </summary>
    /// <remarks>
    /// @see CudaDnn::SetPoolingDesc
    /// </remarks>
    public enum PoolingMethod
    {
        /// <summary>
        /// Specifies to use <code>CUDNN_POOLING_MAX</code> in CUDA C++ code.
        /// </summary>
        MAX = 0,        
        /// <summary>
        /// Specifies to use <code>CUDNN_POOLING_AVERAGE_COUNT_INCLUDE_PADDING</code> in CUDA C++ code.
        /// </summary>
        AVE = 1         
    }

    /// <summary>
    /// Specifies the base datatype corresponding the the template type 'T'.  Currently, only <code>double</code> and <code>float</code> types are supported. 
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// Specifies the double type.
        /// </summary>
        DOUBLE,
        /// <summary>
        /// Specifies the single type.
        /// </summary>
        FLOAT
    }

    /// <summary>
    /// Specifies the initialization flags used when initializing CUDA.
    /// </summary>
    public enum DEVINIT
    {
        /// <summary>
        /// No flag specified.
        /// </summary>
        NONE = 0x0000,

        /// <summary>
        /// Initialize cuBlas.  This should be initialized for cuBlas is used for many of the math operations.
        /// </summary>
        CUBLAS = 0x0001,

        /// <summary>
        /// Initialize cuRand.  This should be initialized for cuRand is used for most of the random operations.
        /// </summary>
        CURAND = 0x0002,

        /// <summary>
        /// Set the cuRand random number generator seed - typically only used when testing to ensure that 
        /// random numbers are generated in a predictable ordering.
        /// </summary>
        SETSEED = 0x0004
    }

    /// <summary>
    /// Specifies the cuDnn batch norm mode to use.
    /// </summary>
    /// <remarks>
    /// @see [NVIDIA cuDnn](https://developer.nvidia.com/cudnn) documenation for more details.
    /// </remarks>
    public enum BATCHNORM_MODE
    {
        /// <summary>
        /// Specifies to use the per-activation batch normalization mode.
        /// </summary>
        PER_ACTIVATION = 0,
        /// <summary>
        /// Specifies to use the spatial batch normalization mode.
        /// </summary>
        SPATIAL = 1,
        /// <summary>
        /// Specifies to use the spatial persistent batch normalization mode.
        /// </summary>
        SPATIAL_PERSISTENT = 2
    }

    /// <summary>
    /// Specifies the cuDnn convolution forward algorithm to use.
    /// </summary>
    /// <remarks>
    /// @see [NVIDIA cuDnn](https://developer.nvidia.com/cudnn) documenation for more details.
    /// </remarks>
    public enum CONV_FWD_ALGO
    {
        /// <summary>
        /// Specifies to not use a forward algorithm.
        /// </summary>
        NONE = -1,
        /// <summary>
        /// Specifies to use the implicit gemm algorithm.
        /// </summary>
        IMPLICIT_GEMM = 0,
        /// <summary>
        /// Specifies to use the implicit pre-computation gemm algorithm.
        /// </summary>
        IMPLICIT_PRECOMP_GEMM = 1,
        /// <summary>
        /// Specifies to use the gemm algorithm.
        /// </summary>
        ALGO_GEMM = 2,
        /// <summary>
        /// Specifies to use the direct algorithm.
        /// </summary>
        ALGO_DIRECT = 3,
        /// <summary>
        /// Specifies to use the fft algorithm.
        /// </summary>
        ALGO_FFT = 4,
        /// <summary>
        /// Specifies to use the fft tiling algorithm.
        /// </summary>
        ALGO_FFT_TILING = 5,
        /// <summary>
        /// Specifies to use the winograd algorithm.
        /// </summary>
        ALGO_WINOGRAD = 6,
        /// <summary>
        /// Specifies to use the non-fused winograd algorithm.
        /// </summary>
        ALGO_WINOGRAD_NONFUSED = 7
    }

    /// <summary>
    /// Specifies the cuDnn convolution backward filter algorithm to use.
    /// </summary>
    /// <remarks>
    /// @see [NVIDIA cuDnn](https://developer.nvidia.com/cudnn) documenation for more details.
    /// </remarks>
    public enum CONV_BWD_FILTER_ALGO
    {
        /// <summary>
        /// Specifies to use algorithm 0 - which is non-deterministic.
        /// </summary>
        ALGO_0 = 0,     
        /// <summary>
        /// Specifies to use algorithm 1.
        /// </summary>
        ALGO_1 = 1,
        /// <summary>
        /// Specifies to use the fft algorithm.
        /// </summary>
        ALGO_FFT = 2,
        /// <summary>
        /// Specifies to use algorithm 0 with a workspace - which is non-deterministic.
        /// </summary>
        ALGO_3 = 3      
    }

    /// <summary>
    /// Specifies the cuDnn convolution backward data algorithm to use.
    /// </summary>
    /// <remarks>
    /// @see [NVIDIA cuDnn](https://developer.nvidia.com/cudnn) documenation for more details.
    /// </remarks>
    public enum CONV_BWD_DATA_ALGO
    {
        /// <summary>
        /// Specifies to use algorithm 0 - which is non-deterministic.
        /// </summary>
        ALGO_0 = 0,     
        /// <summary>
        /// Specifies to use algorithm 1.
        /// </summary>
        ALGO_1 = 1,
        /// <summary>
        /// Specifies to use the fft algorithm.
        /// </summary>
        ALGO_FFT = 2
    }

    /// <summary>
    /// Specifies the pooling method to use when using the Caffe pooling (instead of the pooling from NVIDIA's cuDnn).
    /// </summary>
    /// <remarks>
    /// @see CudaDnn::pooling_fwd
    /// </remarks>
    public enum POOLING_METHOD
    {
        /// <summary>
        /// Select the maximum value from the kernel.
        /// </summary>
        MAX = 0,
        /// <summary>
        /// Select the average of the values in the kernel.
        /// </summary>
        AVE = 1,
        /// <summary>
        /// Select the stochastic value in the kernel - used during a training pass.
        /// </summary>
        STO_TRAIN = 2,
        /// <summary>
        /// Select the stochastic value in the kernel - used during a testing pass.
        /// </summary>
        STO_TEST = 3
    }

    /// <summary>
    /// Specifies the RNN mode to use with the Recurrent Layer when using the cuDNN engine.
    /// </summary>
    public enum RNN_MODE
    {
        /// <summary>
        /// Specifies to use a single RelU gate Recurrent Learning unit.
        /// </summary>
        RNN_RELU = 0,
        /// <summary>
        /// Specifies to use a single TanH gate Recurrent Learning unit.
        /// </summary>
        RNN_TANH = 1,
        /// <summary>
        /// Specifies to use a 4 gate LSTM Recurrent Learning unit.
        /// </summary>
        LSTM = 2,
        /// <summary>
        /// Specifies to use the GRU RNN where
        /// @f$ h' = tanh(r * Uh(t-1) + Wx) @f$ and 
        /// @f$ h = (1 - z) * h' + z * h(t-1) @f$
        /// </summary>
        GRU = 3
    }

    /// <summary>
    /// Specifies the RNN bias mode to use with the Recurrent Layer when using the cuDNN engine.
    /// </summary>
    public enum RNN_BIAS_MODE
    {
        /// <summary>
        /// Specifies to use no bias in the RNN cells.
        /// </summary>
        RNN_NO_BIAS = 0,
        /// <summary>
        /// Specifies to use one bias in the input Gemm of the rnn cell.
        /// </summary>
        RNN_SINGLE_INP_BIAS = 1,
        /// <summary>
        /// Specifies to use two bias in the input Gemm and recurrent Gemm of the rnn cell (default).
        /// </summary>
        RNN_DOUBLE_BIAS = 2,
        /// <summary>
        /// Specifies to use one recurrent bias in the recurrent Gemm of the rnn cell.
        /// </summary>
        RNN_SINGLE_REC_BIAS = 3
    }

    /// <summary>
    /// Specifies the RNN data layout of the data input.
    /// </summary>
    public enum RNN_DATALAYOUT
    {
        /// <summary>
        /// Specifies ordering with sequence major ordering, and padded outer stride from one time-step to the next.
        /// </summary>
        RNN_SEQ_MAJOR_UNPACKED = 0,
        /// <summary>
        /// Specifies ordering with sequence major ordering, and sequence length sorted and packed.
        /// </summary>
        RNN_SEQ_MAJOR_PACKED = 1,
        /// <summary>
        /// Specifies ordering with batch major ordering, padded, outer stride from one batch to the next.
        /// </summary>
        RNN_BATCH_MAJOR_UNPACKED = 2
    }

    /// <summary>
    /// Specifies the RNN directional used.
    /// </summary>
    public enum RNN_DIRECTION
    {
        /// <summary>
        /// Specifies a single direction RNN (default)
        /// </summary>
        RNN_UNIDIRECTIONAL,
        /// <summary>
        /// Specifies a bi-direction RNN where the output is concatinated at each layer.
        /// </summary>
        RNN_BIDIRECTIONAL
    }

    /// <summary>
    /// Defines the filler types used to fill the RNN8 weights.
    /// </summary>
    public enum RNN_FILLER_TYPE
    {
        /// <summary>
        /// Specifies to fill with a constant value.
        /// </summary>
        RNN_CONSTANT_FILLER,
        /// <summary>
        /// Specifies to fill with a uniform distribution.
        /// </summary>
        RNN_XAVIER_FILLER,
        /// <summary>
        /// Specifies to fill with a gaussian distribution.
        /// </summary>
        RNN_GAUSSIAN_FILLER
    }

    /// <summary>
    /// Specifies certain device properties to query from Cuda.
    /// </summary>
    public enum DEVPROP
    {
        /// <summary>
        /// Query the number of devices (gpu's) installed.
        /// </summary>
        DEVICECOUNT = 1,
        /// <summary>
        /// Query the name of a given GPU.
        /// </summary>
        NAME = 2,
        /// <summary>
        /// Query a GPU board group ID.
        /// </summary>
        MULTIGPUBOARDGROUPID = 3,
        /// <summary>
        /// Query the GPU compute level for the given GPU.
        /// </summary>
        COMPUTELEVEL = 4
    }

    /// <summary>
    /// Specifies the memory test to perform.
    /// </summary>
    /// <remarks>
    /// @see CudaDnn::RunMemoryTest
    /// </remarks>
    public enum MEMTEST_TYPE
    {
        /// <summary>
        /// Specifies the mov-inv-8 test.
        /// </summary>
        MOV_INV_8 = 1
    }

    /// <summary>
    /// Specifies the reduction operation to use with 'Nickel' NCCL.
    /// </summary>
    /// <remarks>
    /// @see CudaDnn::NcclAllReduce
    /// </remarks>
    public enum NCCL_REDUCTION_OP
    {
        /// <summary>
        /// Sum the values.
        /// </summary>
        SUM = 0,
        /// <summary>
        /// Multiply the values.
        /// </summary>
        PROD = 1,
        /// <summary>
        /// Return the maximum value.
        /// </summary>
        MAX = 2,
        /// <summary>
        /// Return the minimum value.
        /// </summary>
        MIN = 3
    }

    /// <summary>
    /// Defines the mining type used during SSD cuda training.
    /// </summary>
    /// <remarks>
    /// This enum matches the values of the MultiBoxLossParameter.MiningType with the values supported
    /// in the low level CudaDnnDll.
    /// </remarks>
    public enum SSD_MINING_TYPE
    {
        /// <summary>
        /// Use all negatives.
        /// </summary>
        NONE = 0,
        /// <summary>
        /// Select negatives based on the score.
        /// </summary>
        MAX_NEGATIVE = 1,
        /// <summary>
        /// Select hard examples based on Shrivastava et. al. method.
        /// </summary>
        /// <remarks>
        /// @see [Training Region-based Object Detectors with Online Hard Example Mining](https://arxiv.org/abs/1604.03540) by Abhinav Shrivastava, Abhinav Gupta, Ross Girshick, 2016.
        /// </remarks>
        HARD_EXAMPLE = 2
    }

    /// <summary>
    /// Defines the matching method used during SSD cuda training.
    /// </summary>
    /// <remarks>
    /// This enum matches the values of the MultiBoxLossParameter.MatchType with the values supported
    /// in the low level CudaDnnDll.
    /// </remarks>
    public enum SSD_MATCH_TYPE
    {
        /// <summary>
        /// Specifies to use Bi-Partite.
        /// </summary>
        BIPARTITE,
        /// <summary>
        /// Specifies to use per-prediction matching.
        /// </summary>
        PER_PREDICTION
    }

    /// <summary>
    /// Defines the encode/decode type used during SSD cuda training.
    /// </summary>
    /// <remarks>
    /// This enum matches the values of the PriorBoxParameter.CodeType with the values supported
    /// in the low level CudaDnnDll.
    /// </remarks>
    public enum SSD_CODE_TYPE
    {
        /// <summary>
        /// Encode the corner.
        /// </summary>
        CORNER = 1,
        /// <summary>
        /// Encode the center size.
        /// </summary>
        CENTER_SIZE = 2,
        /// <summary>
        /// Encode the corner size.
        /// </summary>
        CORNER_SIZE = 3
    }

    /// <summary>
    /// Defines the confidence loss types used during SSD cuda training.
    /// </summary>
    /// <remarks>
    /// This enum matches the values of the MultiboxLossParameter.ConfLossType with the values supported
    /// in the low level CudaDnnDll.
    /// </remarks>
    public enum SSD_CONF_LOSS_TYPE
    {
        /// <summary>
        /// Specifies to use softmax.
        /// </summary>
        SOFTMAX,
        /// <summary>
        /// Specifies to use logistic.
        /// </summary>
        LOGISTIC
    }

    /// <summary>
    /// Defines the location loss types used during SSD cuda training.
    /// </summary>
    /// <remarks>
    /// This enum matches the values of the MultiboxLossParameter.LocLossType with the values supported
    /// in the low level CudaDnnDll.
    /// </remarks>
    public enum SSD_LOC_LOSS_TYPE
    {
        /// <summary>
        /// Specifies to use L2 loss.
        /// </summary>
        L2,
        /// <summary>
        /// Specifies to use smooth L1 loss.
        /// </summary>
        SMOOTH_L1
    }

    /// <summary>
    /// Specifies the orientation of a matrix.
    /// </summary>
    /// <remarks>
    /// @see CudaDnn::matrix_add_vector
    /// </remarks>
    public enum ORIENTATION 
    {
        /// <summary>
        /// Specifies to add the vector to each column.
        /// </summary>
        COL = 0,
        /// <summary>
        /// Specifies to add the vector to each row.
        /// </summary>
        ROW = 1
    }

    /// <summary>
    /// Specifies the type of operation to perform along with a matrix transposition.
    /// </summary>
    /// <remarks>
    /// @see CudaDnn::matrix_transpose_operation
    /// </remarks>
    public enum TRANSPOSE_OPERATION 
    {
        /// <summary>
        /// Add the matrix values after transposing.
        /// </summary>
        ADD = 0,
        /// <summary>
        /// Multiply the matrix values after transposing.
        /// </summary>
        MUL = 1,
        /// <summary>
        /// Divide the matrix values after transposing.
        /// </summary>
        DIV = 2
    }

    /// <summary>
    /// Specifies different aggregation operations.
    /// </summary>
    public enum AGGREGATIONS 
    {
        /// <summary>
        /// Sum the values.
        /// </summary>
        SUM = 0,
        /// <summary>
        /// Return the maximum value.
        /// </summary>
        MAX = 1,
        /// <summary>
        /// Return the minimum value.
        /// </summary>
        MIN = 2
    }

    /// <summary>
    /// Specifies the SOFTMAX algorithm to use.
    /// </summary>
    public enum SOFTMAX_ALGORITHM
    {
        /// <summary>
        /// Specifies to use the default algorithm.
        /// </summary>
        DEFAULT = 1,
        /// <summary>
        /// Specifies to use the fast algorithm.
        /// </summary>
        FAST = 0,
        /// <summary>
        /// Specifies to use the accurate algorithm.
        /// </summary>
        ACCURATE = 1,
        /// <summary>
        /// Specifies to use the log algorithm.
        /// </summary>
        LOG = 2
    }

    /// <summary>
    /// Specifies the SOFTMAX mode to use.
    /// </summary>
    public enum SOFTMAX_MODE
    {
        /// <summary>
        /// Specifies to run the softmax separately for each N, across CHW dimensions.
        /// </summary>
        INSTANCE,
        /// <summary>
        /// Specifies to run the softmax separately for each N*C, across HW dimensions.
        /// </summary>
        CHANNEL
    }

    /// <summary>
    /// Specifies the data type to use with the fused computation.
    /// </summary>
    public enum FUSEDCOMPUTE_DATA_TYPE
    {
        /// <summary>
        /// Specifies to use the float 32 data type.
        /// </summary>
        FLOAT = 1,
        /// <summary>
        /// Specifies to use the double data type.
        /// </summary>
        DOUBLE = 2,
        /// <summary>
        /// Specifies to use the half data type.
        /// </summary>
        HALF = 3,
    }

    /// <summary>
    /// Specifies the prebuilt operation to use with the fused computation.
    /// </summary>
    public enum FUSEDCOMPUTE_PREBUILT_OP
    {
        /// <summary>
        /// Specifies the NOP operation.
        /// </summary>
        NONE = 0,
        /// <summary>
        /// Specifies a MATMUL operation.
        /// </summary>
        MATMUL = 1
    }

    /// <summary>
    /// Specifies the operation to use with the fused computation.
    /// </summary>
    public enum FUSEDCOMPUTE_OP
    {
        /// <summary>
        /// Specifies the MATMUL operation.
        /// </summary>
        MATMUL = 1
    }

    /// <summary>
    /// Specifies the heuristic mode to use with the fused computation.
    /// </summary>
    public enum FUSEDCOMP_HEUR_MODE
    {
        /// <summary>
        /// Specifies the heuristic mode A.  This mode is the fastest method.
        /// </summary>
        A = 0,
        /// <summary>
        /// Specifies the heuristic mode B.  This mode can help improve generalization but is slower.
        /// </summary>
        B = 1,
        /// <summary>
        /// Specifies the fallback mode.  Provides functionality but given GPU resources, but may not be optimal in performance.
        /// </summary>
        FALLBACK = 2,
    }

    /// <summary>
    /// Specifies the support for the fused computation.
    /// </summary>
    public enum FUSEDCOMP_SUPPORT
    {
        /// <summary>
        /// Specifies that the operation is not supported on the current GPU resources.
        /// </summary>
        NOT_SUPPORTED = 0,
        /// <summary>
        /// Specifies that the operation is supported on the current GPU resources.
        /// </summary>
        SUPPORTED = 1
    }

#pragma warning disable 1591

    /// <summary>
    /// Specifies the general cuda device interface.
    /// </summary>
    /// <remarks>
    /// This interface is primarily used for testing.
    /// </remarks>
    public interface ICudaDevice /** @private */
    {
        void SetDeviceID(int nDeviceID, DEVINIT flags = DEVINIT.NONE, long? lSeed = null);
        void SetRandomSeed(long lSeed);
        int GetDeviceCount();
        int GetDeviceID();
        void ResetDevice();
        void SynchronizeDevice();
        string GetDeviceName(int nDeviceID);
        string GetDeviceP2PInfo(int nDeviceID);
        string GetRequiredCompute(out int nMinMajor, out int nMinMinor);

    }

    /// <summary>
    /// Specifies the cuda memory operations interface.
    /// </summary>
    /// <remarks>
    /// This interface is primarily used for testing.
    /// </remarks>
    public interface ICudaMemory /** @private */
    {
        long AllocMemory(long lCount, bool bHalf = false);
        long AllocMemory(List<double> rg);
        long AllocMemory(List<float> rg);
        long AllocMemory(double[] rgSrc, long hStream = 0);
        long AllocMemory(float[] rgSrc, long hStream = 0);
        void FreeMemory(long hMem);
        double[] GetMemoryDouble(long hMem, long lCount = -1);
        float[] GetMemoryFloat(long hMem, long lCount = -1);
        void SetMemory(long hMem, List<double> rg);
        void SetMemory(long hMem, List<float> rg);
        void SetMemory(long hMem, double[] rgSrc, long hStream = 0);
        void SetMemory(long hMem, float[] rgSrc, long hStream = 0);
        void SetMemoryAt(long hMem, double[] rgSrc, int nOffset);
        void SetMemoryAt(long hMem, float[] rgSrc, int nOffset);
        long AllocHostBuffer(long lCount);
        void FreeHostBuffer(long hMem);
        double[] GetHostMemoryDouble(long hMem);
        float[] GetHostMemoryFloat(long hMem);
        long CreateMemoryPointer(long hData, long lOffset, long lCount);
        void FreeMemoryPointer(long hMem);
    }

    /// <summary>
    /// Specifies the interface to common cuDnn functionality.
    /// </summary>
    /// <remarks>
    /// This interface is primarily used for testing.
    /// </remarks>
    public interface ICudaCuDnn /** @private */
    {
        long CreateStream(bool bNonBlocking = false, int nIndex = -1);
        void FreeStream(long h); 
        void SynchronizeStream(long h = 0);
        void SynchronizeThread();

        long CreateCuDNN(long hStream = 0);
        void FreeCuDNN(long h);

        long CreateTensorDesc();
        void FreeTensorDesc(long h);
        void SetTensorNdDesc(long hHandle, int[] rgDim, int[] rgStride, bool bHalf = false);
        void SetTensorDesc(long hHandle, int n, int c, int h, int w, bool bHalf = false);
        void SetTensorDesc(long hHandle, int n, int c, int h, int w, int nStride, int cStride, int hStride, int wStride, bool bHalf = false);
        void AddTensor(long hHandle, long hSrcDesc, long hSrc, int nSrcOffset, long hDstDesc, long hDst, int nDstOffset);

        void DeriveBatchNormDesc(long hFwdScaleBiasMeanVarDesc, long hFwdBottomDesc, long hBwdScaleBiasMeanVarDesc, long hBwdBottomDesc, BATCHNORM_MODE mode);

        long CreateFilterDesc();
        void FreeFilterDesc(long h);
        void SetFilterNdDesc(long hHandle, int[] rgDim, bool bHalf = false);
        void SetFilterDesc(long hHandle, int n, int c, int h, int w, bool bHalf = false);

        long CreateConvolutionDesc();
        void FreeConvolutionDesc(long h);
        void SetConvolutionDesc(long hHandle, int hPad, int wPad, int hStride, int wStride, int hDilation, int wDilation, bool bUseTensorCores, bool bHalf = false);

        long CreatePoolingDesc();
        void FreePoolingDesc(long h);
        void SetPoolingDesc(long hHandle, PoolingMethod method, int h, int w, int hPad, int wPad, int hStride, int wStride);
        
        long CreateLRNDesc();
        void FreeLRNDesc(long h);
        void SetLRNDesc(long hHandle, uint nSize, double fAlpha, double fBeta, double fK);

        long CreateRnnDataDesc();
        void FreeRnnDataDesc(long h);
        void SetRnnDataDesc(long hRnnDataDesc, RNN_DATALAYOUT layout, int nMaxSeqLen, int nBatchSize, int nVectorSize, bool bBidirectional = false, int[] rgSeqLen = null);

        long CreateRnnDesc();
        void FreeRnnDesc(long h);
        void SetRnnDesc(long hHandle, long hRnnDesc, int nHiddenSize, int nNumLayers, long hDropoutDesc, RNN_MODE mode, bool bUseTensorCores, RNN_DIRECTION direction = RNN_DIRECTION.RNN_UNIDIRECTIONAL);
        int GetRnnParamCount(long hHandle, long hRnnDesc, long hXDesc);
        ulong GetRnnWorkspaceCount(long hHandle, long hRnnDesc, long hXDesc, out ulong nReservedCount);
        void GetRnnLinLayerParams(long hHandle, long hRnnDesc, int nLayer, long hXDesc, long hWtDesc, long hWtData, int nLinLayer, out int nWtCount, out long hWt, out int nBiasCount, out long hBias);
        void RnnForward(long hHandle, long hRnnDesc, long hXDesc, long hXData, long hHxDesc, long hHxData, long hCxDesc, long hCxData, long hWtDesc, long hWtData, long hYDesc, long hYData, long hHyDesc, long hHyData, long hCyDesc, long hCyData, long hWorkspace, ulong nWsCount, long hReserved, ulong hResCount, bool bTraining);
        void RnnBackwardData(long hHandle, long hRnnDesc, long hYDesc, long hYData, long hYDiff, long hHyDesc, long hHyDiff, long hCyDesc, long hCyDiff, long hWtDesc, long hWtData, long hHxDesc, long hHxData, long hCxDesc, long hCxData, long hXDesc, long hXDiff, long hdHxDesc, long hHxDiff, long hdCxDesc, long hCxDiff, long hWorkspace, ulong nWsCount, long hReserved, ulong nResCount);
        void RnnBackwardWeights(long hHandle, long hRnnDesc, long hXDesc, long hXData, long hHxDesc, long hHxData, long hYDesc, long hYData, long hWorkspace, ulong nWsCount, long hWtDesc, long hWtDiff, long hReserved, ulong nResCount);
    }

    /// <summary>
    /// Specifies the interface to common math functions.
    /// </summary>
    /// <remarks>
    /// This interface is primarily used for testing.
    /// </remarks>
    public interface ICudaMath /** @private */
    {
        void set(int nCount, long hHandle, double fVal, int nIdx = -1);
        void set(int nCount, long hHandle, float fVal, int nIdx = -1);
        double[] get_double(int nCount, long hHandle, int nIdx = -1);
        float[] get_float(int nCount, long hHandle, int nIdx = -1);
        void copy(int nCount, long hSrc, long hDst, int nSrcOffset = 0, int nDstOffset = 0, long hAsyncStream = -1, bool? bSrcHalfOverride = null, bool? bDstHalfOverride = null);
        void copy(int nCount, int nNum, int nDim, long hSrc1, long hSrc2, long hDst, long hSimilar, bool bInvert = false);
        void copy_expand(int n, int nNum, int nDim, long hSrc, long hDs);
        void fill(int n, int nDim, long hSrc, int nSrcOff, int nCount, long hDst);
        void sort(int nCount, long hY);

        void channel_interpolate_linear(int nCount, int nOuterNum, int nChannels, int nInnerNumSrc, int nInnerNumDst, long hX, long hY, DIR dir = DIR.FWD);
        void channel_compare(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY);
        void channel_fill(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, int nLabelDim, long hLabels, long hY);
        void channel_fillfrom(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, DIR dir);
        void channel_scale(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hA, long hY);
        void channel_mulv(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hA, long hX, long hC);
        void channel_min(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, bool bReturnIdx = false, bool bAcrossChannels = false);
        void channel_max(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, bool bReturnIdx = false, bool bAcrossChannels = false);

        void channel_sum(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY);
        void channel_sumEx(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, bool bSumAcrossChannels = true, DIR dir = DIR.FWD, int nChanalesY = -1);
        void channel_mean(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, int nXOff = 0);
        void channel_stdev(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, long hZ, float fEps, bool bUnbiased);
        void channel_copy(int nCount, int nOuterNum, int nChannels, int nBlocks, int nInnerNum, int nOffset, long hX, long hY, DIR dir);
        void channel_copyall(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY);
        void channel_duplicate(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY);
        void channel_percentile(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, double dfPercentile);
        void channel_op_fwd(OP op, int nCount, int nC, int nN1, int nSD1, int nN2, int nSD2, long hA, long hB, long hY);
        void channel_op_bwd(OP op, int nCount, int nC, int nN1, int nSD1, int nN2, int nSD2, int nCy, int nSDy, long hA, long hB, long hY, long hAd, long hBd, long hYd, long hWork);

        void gemm(bool bTransA, bool bTransB, int m, int n, int k, double fAlpha, long hA, long hB, double fBeta, long hC);
        void gemm(bool bTransA, bool bTransB, int m, int n, int k, float fAlpha, long hA, long hB, float fBeta, long hC);
        void gemv(bool bTransA, int m, int n, double fAlpha, long hA, long hX, double fBeta, long hY);
        void gemv(bool bTransA, int m, int n, float fAlpha, long hA, long hX, float fBeta, long hY);
        void geam(bool bTransA, bool bTransB, int m, int n, double fAlpha, long hA, long hB, double fBeta, long hC);
        void geam(bool bTransA, bool bTransB, int m, int n, float fAlpha, long hA, long hB, float fBeta, long hC);

        void ger(int m, int n, double fAlpha, long hX, long hY, long hA);
        void ger(int m, int n, float fAlpha, long hX, long hY, long hA);
        void axpy(int n, double fAlpha, long hX, long hY);
        void axpy(int n, float fAlpha, long hX, long hY);
        void axpby(int n, double fAlpha, long hX, double fBeta, long hY);
        void axpby(int n, float fAlpha, long hX, float fBeta, long hY);
        void scal(int n, double fAlpha, long hX, int nXOff = 0);
        void scal(int n, float fAlpha, long hX, int nXOff = 0);
        double dot_double(int n, long hX, long hY);
        float dot_float(int n, long hX, long hY);
        double asum_double(int n, long hX, int nXOff = 0);
        float asum_float(int n, long hX, int nXOff = 0);
        void scale(int n, double fAlpha, long hX, long hY);
        void scale(int n, float fAlpha, long hX, long hY);
        void add_scalar(int n, double fAlpha, long hY);
        void add_scalar(int n, float fAlpha, long hY);
        void add(int n, long hA, long hB, long hY);
        void add(int n, long hA, long hB, long hY, double dfAlpha);
        void add(int n, long hA, long hB, long hY, float fAlpha);
        void sub(int n, long hA, long hB, long hY, int nAOff = 0, int nBOff = 0, int nYOff = 0, int nB = 0);
        void mul(int n, long hA, long hB, long hY, int nAOff = 0, int nBOff = 0, int nYOff = 0);
        void mul_scalar(int n, double fAlpha, long hY);
        void mul_scalar(int n, float fAlpha, long hY);
        void muladd(int n, long hX, long hA, long hY, DIR dir, long hdX = 0, long hdA = 0);
        void div(int n, long hA, long hB, long hY);
        void abs(int n, long hA, long hY);
        void exp(int n, long hA, long hY);
        void log(int n, long hA, long hY);
        void invert(int n, long hA, long hY, int nAOff = 0, int nYOff = 0, double dfScaleNum = 1, double dfScaleDen = 1);
        void powx(int n, long hA, double fAlpha, long hY, int nAOff = 0, int nYOff = 0);
        void powx(int n, long hA, float fAlpha, long hY, int nAOff = 0, int nYOff = 0);
        void sign(int n, long hX, long hY, int nXOff = 0, int nYOff = 0);
        double min(int n, long hA, out long lPos, int nAOff = 0, long hWork = 0);
        double max(int n, long hA, out long lPos, int nAOff = 0, long hWork = 0);
        double sumsq(int n, long hW, long hA, int nAOff = 0);
        double sumsqdiff(int n, long hW, long hA, long hB, int nAOff = 0, int nBOff = 0);
        void sqrt(int n, long hA, long hY, double dfEpsilon = 0);
        void sqrt_scale(int n, long hA, long hY);
        
        void mask(int n, int nMaskDim, double fSearch, double fReplace, long hX, long hMask, long hY);
        void mask(int n, int nMaskDim, float fSearch, float fReplace, long hX, long hMask, long hY);
        void mask_batch(int n, int nBatch, int nMaskDim, double fSearch, double fReplace, long hX, long hMask, long hY);
        void mask_batch(int n, int nBatch, int nMaskDim, float fSearch, float fReplace, long hX, long hMask, long hY);

        void im2col(long hDataIm, int nDataImOffset, int nChannels, int nHeight, int nWidth, int nKernelH, int nKernelW, int nPadH, int nPadW, int nStrideH, int nStrideW, int nDilationH, int nDilationW, long hDataCol, int nDataColOffset);
        void im2col_nd(long hDataIm, int nDataImOffset, int nNumSpatialAxes, int nColCount, int nChannelAxis, long hImShape, long hColShape, long hKernelShape, long hPad, long hStride, long hDilation, long hDataCol, int nDataColOffset);
        void col2im(long hDataCol, int nDataColOffset, int nChannels, int nHeight, int nWidth, int nKernelH, int nKernelW, int nPadH, int nPadW, int nStrideH, int nStrideW, int nDilationH, int nDilationW, long hDataIm, int nDataImOffset);
        void col2im_nd(long hDataCol, int nDataColOffset, int nNumSpatialAxes, int nColCount, int nChannelAxis, long hImShape, long hColShape, long hKernelShape, long hPad, long hStride, long hDilation, long hDataIm, int nDataImOffset);
    }

    /// <summary>
    /// Specifies the interface to common random number generation functions.
    /// </summary>
    /// <remarks>
    /// This interface is primarily used for testing.
    /// </remarks>
    public interface ICudaRandom /** @private */
    {
        void rng_setseed(long lSeed);
        void rng_uniform(int n, double fMin, double fMax, long hY);
        void rng_uniform(int n, float fMin, float fMax, long hY);
        void rng_gaussian(int n, double fMu, double fSigma, long hY);
        void rng_gaussian(int n, float fMu, float fSigma, long hY);
        void rng_bernoulli(int n, double fNonZeroProb, long hY);
        void rng_bernoulli(int n, float fNonZeroProb, long hY);
    }

    /// <summary>
    /// Specifies the combination interface that encompasses all other interfaces.
    /// </summary>
    /// <remarks>
    /// This interface is primarily used for testing.
    /// </remarks>
    public interface ICudaDnn : ICudaDevice, ICudaMemory, ICudaCuDnn, ICudaMath, ICudaRandom /** @private */
    {
    }

#pragma warning restore 1591 


    /// <summary>
    /// The CudaDnn object is the main interface to the Low-Level Cuda C++ DLL.
    /// </summary>
    /// <remarks>
    /// This is the transition location where C# meets C++.
    /// </remarks>
    /// <typeparam name="T">Specifies the base type <i>float</i> or <i>double</i>.  Using <i>float</i> is recommended to conserve GPU memory.</typeparam>
    public class CudaDnn<T> : ICudaDnn, IDisposable
    {
        Params m_param = new Params();
        CudaDnnMemoryTracker<T> m_memTracker;
        int m_nDeviceId;
        string m_strPath = "";
        static int s_nIdxSeed = 0;
        static string s_strCudaPath = "";
        CudaControlLib.ICudaKernel m_cuda;
        long m_hKernel = 0;
        DataType m_dt;
        CryptoRandom m_random = new CryptoRandom();
        T m_tOne;
        T m_tZero;
        int m_nIdx;
        long m_nGhostMemoryIndex = 1000;
        Dictionary<long, T[]> m_rgGhostMemory = null;
        bool m_bGhostMemoryEnabled = false;
        bool m_bOwner = true;
        object m_memSync = new object();
        bool m_bEnableRnnExtendedVersion = false;
        static object m_createSync = new object();
        static object m_getconvSync = new object();
        static ulong m_lBaseSize = (ulong)((typeof(T) == typeof(float)) ? sizeof(float) : sizeof(double));
        static object m_cleanupSync = new object();

        /// <summary>
        /// Specifies the type of string information to quer from the Cuda C++ layer.
        /// </summary>
        public enum CUDAQRY
        {
            /// <summary>
            /// Query the device (GPU) name.
            /// </summary>
            DEVICE_NAME = 1000,
            /// <summary>
            /// Query the device (GPU) Peer-to-Peer information.  Note, P2P mode is only available when
            /// running a device in TCC mode.  For more information see the [NVIDIA SMI Documentation](http://developer.download.nvidia.com/compute/DCGM/docs/nvidia-smi-367.38.pdf)
            /// </summary>
            DEVICE_P2P_INFO = 1001,
            /// <summary>
            /// Query the device (GPU) general information such as memory and processor usage.
            /// </summary>
            DEVICE_INFO = 1002,
            /// <summary>
            /// Query a set of strings from an extension.
            /// </summary>
            EXTENSION_STRING = 1003
        }

#pragma warning disable 1591

        /// <summary>
        /// Specifies the function indexes supported by the Low-Level Cuda Dnn DLL.
        /// </summary>
        /// <remarks><b>IMPORTANT:</b> These index values must match the index values 
        /// specified within the Low-Level Cuda Dnn DLL.</remarks>
        public enum CUDAFN /** @private */
        {
            INITIALIZE = -2,
            CLEANUP = -3,
            KERNEL_MEMCOPY = -4,
            KERNEL_ADD = -5,
            KERNEL_COPY_NCCL = -10,

            SETDEVICE = 1,
            SETRANDOMSEED = 2,
            GETDEVICE = 3,
            RESETDEVICE = 4,
            SYNCHRONIZEDEVICE = 5,
            GETDEVICEPROP = 6,
            CHECKMEMORYATTRIB = 7,
            GETDEVICEMEMORY = 8,
            GETREQUIREDCOMPUTE = 9,

            DEVICE_CANACCESSPEER = 10,
            DEVICE_ENABLEPEERACCESS = 11,
            DEVICE_DISABLEPEERACCESS = 12,

            COPY_DEVICE_TO_HOST = 14,
            COPY_HOST_TO_DEVICE = 15,

            CREATE_MEMORYPOINTER = 16,
            FREE_MEMORYPOINTER = 17,

            ALLOCMEM_HALF = 19,
            ALLOCMEM = 20,
            FREEMEM = 21,
            GETMEM = 22,
            SETMEM = 23,
            SETMEMAT = 24,

            ALLOCHOSTBUFFER = 25,
            FREEHOSTBUFFER = 26,
            GETHOSTMEM = 27,
            SETHOSTMEM = 28,
            GETHOSTBUFFERCAPACITY = 29,

            CREATE_STREAM = 30,
            FREE_STREAM = 31,
            SYNCRHONIZE_STREAM = 32,
            SYNCHRONIZE_THREAD = 33,

            CREATE_MEMTEST = 34,
            FREE_MEMTEST = 35,
            RUN_MEMTEST = 36,

            CREATE_IMAGEOP = 37,
            FREE_IMAGEOP = 38,
            DISTORTIMAGE_IMAGEOP = 39,

            CREATE_NCCL = 40,
            FREE_NCCL = 41,
            NCCL_INIT_SINGLEPROCESS = 42,
            NCCL_INIT_MULTIPROCESS = 43,
            NCCL_BROADCAST = 44,
            NCCL_ALLREDUCE = 45,

            SETPIXEL = 46,

            CREATE_CUDNN = 47,
            FREE_CUDNN = 48,

            CREATE_TENSORDESC = 50,
            FREE_TENSORDESC = 51,
            SET_TENSORDESC = 52,
            ADD_TENSOR = 53,
            SET_TENSORNDDESC = 54,

            CREATE_FILTERDESC = 60,
            FREE_FILTERDESC = 61,
            SET_FILTERDESC = 62,
            SET_FILTERNDDESC = 63,

            CREATE_EXTENSION = 67,
            FREE_EXTENSION = 68,
            EXTENSION_RUN = 69,

            CREATE_CONVDESC = 70,
            FREE_CONVDESC = 71,
            SET_CONVDESC = 72,
            GET_CONVINFO = 73,
            FWD_CONV = 74,
            BWD_CONV_BIAS = 75,
            BWD_CONV_FILTER = 76,
            BWD_CONV_DATA = 77,

            CREATE_POOLDESC = 80,
            FREE_POOLDESC = 81,
            SET_POOLDESC = 82,
            FWD_POOL = 83,
            BWD_POOL = 84,

            DERIVE_BNDESC = 86,
            FWD_BN = 87,
            BWD_BN = 88,

            CREATE_LRNDESC = 90,
            FREE_LRNDESC = 91,
            SET_LRNDESC = 92,

            GET_DROPOUT_INFO = 94,
            CREATE_DROPOUTDESC = 95,
            FREE_DROPOUTDESC = 96,
            SET_DROPOUTDESC = 97,
            FWD_DROPOUT = 98,
            BWD_DROPOUT = 99,

            TANH_FWD = 100,
            TANH_BWD = 101,

            ELU_FWD = 102,
            ELU_BWD = 103,

            SIGMOID_FWD = 104,
            SIGMOID_BWD = 105,

            RELU_FWD = 108,
            RELU_BWD = 109,

            SOFTMAX_FWD = 111,
            SOFTMAX_BWD = 112,

            LRN_CC_FWD = 120,
            LRN_CC_BWD = 121,
            LCN_CC_FWD = 122,
            LCN_CC_BWD = 123,

            // DEPRECIATED, use RNN8 instead
            CREATE_RNN_DATA_DESC = 130,
            FREE_RNN_DATA_DESC = 131,
            SET_RNN_DATA_DESC = 132,

            // DEPRECIATED, use RNN8 instead
            CREATE_RNN_DATA_DESCEX = 135,
            FREE_RNN_DATA_DESCEX = 136,
            SET_RNN_DATA_DESCEX = 137,

            // DEPRECIATED, use RNN8 instead
            CREATE_RNN_DESC = 140,
            FREE_RNN_DESC = 141,
            SET_RNN_DESC = 142,
            GET_RNN_PARAMCOUNT = 143,
            GET_RNN_WORKSPACECOUNT = 144,
            GET_RNN_LINLAYERPARAMS = 145,
            FWD_RNN = 146,
            BWD_RNN_DATA = 147,
            BWD_RNN_WTS = 148,

            RNN8_IS_SUPPORTED = 150,
            RNN8_CREATE = 151,
            RNN8_FREE = 152,
            RNN8_SET = 153,
            RNN8_GET_MEMORY_SIZES = 154,
            RNN8_INIT_WEIGHTS = 155,
            RNN8_FWD = 156,
            RNN8_BWD = 157,

            ATTN_CREATE = 160,
            ATTN_FREE = 161,
            ATTN_SET = 162,
            ATTN_SCALED_DOT_PRODUCT_FWD = 163,
            ATTN_SCALED_DOT_PRODUCT_BWD = 164,

            CPD_CREATE = 180,
            CPD_FREE = 181,
            CPD_SET = 182,
            CPD_COMPUTE_T_VALUE_AT = 183,
            CPD_COMPUTE_S_VALUES = 184,

            CUDA_SET = 200,
            CUDA_GET = 201,
            CUDA_COPY = 202,
            CUDA_COPY_SIM = 203,
            CUDA_COPY_FILL = 204,
            CUDA_SORT = 205,
            CUDA_COPY_BATCH = 206,
            CUDA_COPY_SEQUENCE = 207,
            CUDA_COPY_EXPAND = 208,
            CUDA_COPY_SEQUENCE2 = 209,

            CUDA_RSQRT = 216,
            CUDA_ADD3 = 217,
            CUDA_GEAM = 218,
            CUDA_GEMM2 = 219,
            CUDA_GEMM = 220,
            CUDA_GEMV = 221,
            CUDA_AXPY = 222,
            CUDA_AXPBY = 223,
            CUDA_SCAL = 224,
            CUDA_DOT = 225,
            CUDA_ASUM = 226,
            CUDA_SCALE = 227,
            CUDA_ADD_SCALAR = 228,
            CUDA_ADD = 229,
            CUDA_SUB = 230,
            CUDA_MUL = 231,
            CUDA_MUL_SCALAR = 232,
            CUDA_DIV = 233,
            CUDA_ABS = 234,
            CUDA_EXP = 235,
            CUDA_LOG = 236,
            CUDA_POWX = 237,
            CUDA_SIGN = 238,
            CUDA_SQRT = 239,
            CUDA_RECIPROCOL = 240,
            CUDA_STUDENT = 241,
            CUDA_LOGISTIC1 = 242,
            CUDA_LOGISTIC2 = 243,
            CUDA_ADD2 = 244,
            CUDA_COMPARE_SIGNS = 245,
            CUDA_MAXVAL = 246,
            CUDA_MINVAL = 247,
            CUDA_SUMSQ = 248,
            CUDA_SUMSQDIFF = 249,
            CUDA_WIDTH = 250,
            CUDA_CONTAINS_POINT = 251,
            CUDA_DENAN = 252,
            CUDA_SUB_AND_DOT = 253,
            CUDA_MINMAXVAL = 254,
            CUDA_SUM = 255,
            CUDA_SQRT_SCALE = 256,
            CUDA_GER = 257,
            CUDA_SET_BOUNDS = 259,
            CUDA_MINMAXVEC = 260,
            CUDA_TRANSPOSE = 261,
            CUDA_SCALE_TO_RANGE = 262,
            CUDA_ERF = 263,
            CUDA_MASK = 264,

            CUDA_INTERP2 = 265,
            CUDA_MASK_BATCH = 266,
            CUDA_TRANSPOSE_HW = 267,

            CUDA_MAX = 268,
            CUDA_MIN = 269,

            CUDA_MULBSX = 270,
            CUDA_DIVBSX = 271,

            CUDA_MAX_BWD2 = 272,
            CUDA_INVERT = 273,
            CUDA_THRESHOLD = 274,
            
            CUDA_MULADD = 275,
            CUDA_Z_SCORE = 276,

            CUDA_IM2COL = 280,
            CUDA_IM2COL_ND = 281,
            CUDA_COL2IM = 282,
            CUDA_COL2IM_ND = 283,

            CUDA_ACCURACY_FWD = 286,

            CUDA_CHANNEL_MEAN = 287,
            CUDA_CHANNEL_STDEV = 288,
            CUDA_CHANNEL_MIN = 289,
            CUDA_CHANNEL_MAX = 290,
            CUDA_CHANNEL_SUB = 291,
            CUDA_CHANNEL_SUM = 292,
            CUDA_CHANNEL_DIV = 293,
            CUDA_CHANNEL_DOT = 294,
            CUDA_CHANNEL_MUL = 295,
            CUDA_CHANNEL_COMPARE = 296,
            CUDA_CHANNEL_FILL = 297,
            CUDA_CHANNEL_SCALE = 298,
            CUDA_CHANNEL_MULV = 299,
            CUDA_CHANNEL_COPY = 300,
            CUDA_CHANNEL_FILLFROM = 301,
            CUDA_CHANNEL_COPYALL = 302,
            CUDA_CHANNEL_DUP = 303,
            CUDA_CHANNEL_ADD = 304,
            CUDA_CHANNEL_PERCENTILE = 305,
            CUDA_CHANNEL_OP_FWD = 306,
            CUDA_CHANNEL_OP_BWD = 307,
            CUDA_CHANNEL_SUM_ALL = 308,
            CUDA_CHANNEL_SUM2 = 309,
            CUDA_CHANNEL_INTERPOLATE_LINEAR = 310,

            CUDA_RNG_SETSEED = 349,
            CUDA_RNG_UNIFORM = 350,
            CUDA_RNG_GAUSSIAN = 351,
            // CUDA_RNG_BERNOULLI = 352,   // Not implemented yet.

            CUDA_BATCHREIDX_FWD = 386,
            CUDA_BATCHREIDX_BWD = 387,

            CUDA_EMBED_FWD = 390,
            CUDA_EMBED_BWD = 391,

            CUDA_CLIP_FWD = 394,
            CUDA_CLIP_BWD = 395,

            CUDA_POOL_FWD = 400,
            CUDA_POOL_BWD = 401,

            CUDA_UNPOOL_FWD = 410,
            CUDA_UNPOOL_BWD = 411,

            CUDA_TANH_FWD = 420,
            CUDA_TANH_BWD = 421,

            CUDA_MISH_FWD = 422,
            CUDA_MISH_BWD = 423,

            CUDA_SIGMOID_FWD = 424,
            CUDA_SIGMOID_BWD = 425,

            CUDA_SWISH_BWD = 427,

            CUDA_RELU_FWD = 428,
            CUDA_RELU_BWD = 429,

            CUDA_ELU_FWD = 430,
            CUDA_ELU_BWD = 431,

            CUDA_DROPOUT_FWD = 432,
            CUDA_DROPOUT_BWD = 433,

            CUDA_BNLL_FWD = 435,
            CUDA_BNLL_BWD = 436,

            CUDA_PRELU_FWD = 438,
            CUDA_PRELU_BWD = 439,
            CUDA_PRELU_BWD_PARAM = 440,

            CUDA_NLLLOSS_FWD = 442,
            CUDA_NLLLOSS_BWD = 443,

            CUDA_SOFTMAXLOSS_FWD = 444,
            CUDA_SOFTMAXLOSS_BWD = 445,

            CUDA_MIN_FWD = 446,
            CUDA_MIN_BWD = 447,

            CUDA_MAX_FWD = 448,
            CUDA_MAX_BWD = 449,

            CUDA_CROP_FWD = 450,
            CUDA_CROP_BWD = 451,

            CUDA_CONCAT_FWD = 452,
            CUDA_CONCAT_BWD = 453,

            CUDA_SLICE_FWD = 455,
            CUDA_SLICE_BWD = 456,

            CUDA_TILE_FWD = 457,
            CUDA_TILE_BWD = 458,

            CUDA_BIAS_FWD = 460,

            CUDA_SCALE_FWD = 461,

            CUDA_THRESHOLD_FWD = 462,

            CUDA_CLL_BWD = 463,

            CUDA_LRN_FILLSCALE = 465,
            CUDA_LRN_COMPUTEOUTPUT = 466,
            CUDA_LRN_COMPUTEDIFF = 467,

            CUDA_SMOOTHL1_FWD = 470,
            CUDA_SMOOTHL1_BWD = 471,

            CUDA_SERF_FWD = 472,
            CUDA_SERF_BWD = 473,

            CUDA_PERMUTE = 474,

            CUDA_GATHER_FWD = 476,
            CUDA_GATHER_BWD = 477,

            CUDA_LSTM_FWD = 480,
            CUDA_LSTM_BWD = 481,

            CUDA_LSTM_UNIT_FWD = 482,
            CUDA_LSTM_UNIT_BWD = 483,

            CUDA_MATH_FWD = 487,
            CUDA_MATH_BWD = 488,

            CUDA_COEFF_SUM_FWD = 490,
            CUDA_COEFF_SUM_BWD = 491,

            CUDA_COEFF_SUB_FWD = 492,
            CUDA_COEFF_SUB_BWD = 493,

            CUDA_MEAN_ERROR_LOSS_BWD = 495,

            CUDA_SIGMOID_CROSS_ENTROPY_FWD = 496,
            CUDA_SIGMOID_CROSS_ENTROPY_BWD = 497,
            CUDA_SOFTMAX_CROSS_ENTROPY_FWD = 498,
            CUDA_SOFTMAX_CROSS_ENTROPY_BWD = 499,

            CUDA_SGD_UPDATE = 500,
            CUDA_NESTEROV_UPDATE = 501,
            CUDA_ADAGRAD_UPDATE = 502,
            CUDA_ADADELTA_UPDATE = 503,
            CUDA_ADAM_UPDATE = 504,
            CUDA_RMSPROP_UPDATE = 505,
            CUDA_ADAMW_UPDATE = 506,

            CUDA_COMBINE_DATA = 550,

            CUDA_GELU_FWD = 600,
            CUDA_GELU_BWD = 601,

            CUDA_SILU_FWD = 605,
            CUDA_SILU_BWD = 606,

            CUDA_SOFTPLUS_FWD = 610,
            CUDA_SOFTPLUS_BWD = 611,

            CUDA_LECUN_FWD = 615,
            CUDA_LECUN_BWD = 616,

            CUDA_MTX_SET_DIAGONAL = 700,
            CUDA_MTX_SET_DIAGONAL2 = 701,
            CUDA_MTX_ADD_VECTOR = 702,
            CUDA_MTX_TRANSPOSE_OPERATION = 703,
            CUDA_MTX_AGGREGATE_COLS = 704,
            CUDA_MTX_AGGREGATE_ROWS = 705,
            CUDA_MTX_TRANSPOSE = 706,
            CUDA_MTX_MEANCENTER_BY_COL = 707,
            CUDA_MTX_MEANCENTER_BY_ROW = 708,
            CUDA_MTX_EUCLIDEAN_DIST = 709,
            CUDA_MTX_DOT = 710,
            CUDA_MTX_MEAN = 711,
            CUDA_MTX_STDEV = 712,
            CUDA_MTX_CORRELATIONS = 714,

            CUDA_CREATE_PCA = 800,
            CUDA_RUN_PCA = 801,
            CUDA_FREE_PCA = 802,

            CUDA_TSNE_UPDATE = 850,
            CUDA_TSNE_UPDATE_GRAD = 851,
            CUDA_TSNE_COMPUTE_EXACT_ERROR = 852,
            CUDA_TSNE_COMPUTE_SQUARED_EUCLIDEAN_DISTANCE = 854,
            CUDA_TSNE_COMPUTE_Q_MATRIX = 855,
            CUDA_TSNE_COMPUTE_EXACT_GRADIENT = 856,
            CUDA_TSNE_SYMMETRIZE_MATRIX = 858,
            CUDA_TSNE_COMPUTE_KNN_BOUNDS = 859,

            CUDA_TSNE_CREATE_GAUSSIAN_PERPLEXITY = 870,
            CUDA_TSNE_FREE_GAUSSIAN_PERPLEXITY = 871,
            CUDA_TSNE_FIND_GAUSSIAN_PERPLEXITY = 872,

            CUDA_TSNE_CREATE = 875,
            CUDA_TSNE_FREE = 876,
            CUDA_TSNE_COMPUTE_GRADIENT1 = 877,
            CUDA_TSNE_COMPUTE_ERROR1 = 878,

            CUDA_GUASSIAN_BLUR = 900,
            CUDA_HAMMING_DIFF = 901,
            CUDA_CALC_BATCH_DIST = 902,
            CUDA_CALC_DFT = 903,

            CUDA_CREATE_SSD = 950,
            CUDA_FREE_SSD = 951,
            CUDA_SETUP_SSD = 952,
            CUDA_SSD_FWD_MULTIBOXLOSS = 955,
            CUDA_SSD_ENCODE_LOCPRED = 958,
            CUDA_SSD_ENCODE_CONFPRED = 959,

            CUDAFN_BCE_WITH_LOGITS_LOSS_FWD = 960,
            CUDAFN_BCE_WITH_LOGITS_LOSS_BWD = 961,

            CUDA_CREATE_LAYERNORM = 970,
            CUDA_FREE_LAYERNORM = 971,
            CUDA_LAYERNORM_FWD = 975,
            CUDA_LAYERNORM_BWD = 976,

            CUDA_CREATE_ROPE = 980,
            CUDA_FREE_ROPE = 981,
            CUDA_ROPE_FWD = 982,
            CUDA_ROPE_BWD = 983,

            CUDA_CREATE_BLOBLOADER = 990,
            CUDA_FREE_BLOBLOADER = 991,
            CUDA_BLOBLOADER_LOAD = 992,
            CUDA_BLOBLOADER_RESETOFFSET = 993,
            CUDA_BLOBLOADER_ADDTOOFFSET = 994,

            CUDA_CREATE_FUSEDCOMP = 1001,
            CUDA_FREE_FUSEDCOMP = 1002,
            CUDA_FUSED_COMP_ADD_TENSOR = 1003,
            CUDA_FUSED_COMP_GET_TENSOR = 1004,
            CUDA_FUSED_COMP_ADD_OP = 1005,
            CUDA_FUSED_COMP_BUILD = 1006,
            CUDA_FUSED_COMP_EXECUTE = 1007,

            CUDA_DEBUG = 10000
        }

#pragma warning restore 1591


        /// <summary>
        /// The CudaDnn constructor.
        /// </summary>
        /// <param name="nDeviceID">Specifies the zero-based device (GPU) id.  Note, if there are 5 GPU's in the system, the device ID's will be numbered 0, 1, 2, 3, 4.</param>
        /// <param name="flags">Specifies the flags under which to initialize the Low-Level Cuda system.</param>
        /// <param name="lSeed">Optionally specifies the random number generator seed.  Typically this is only used during testing.</param>
        /// <param name="strPath">Specifies the file path of the Low-Level Cuda DNN Dll file. When NULL or empty, the Low-Level <code>CudaDNNDll.dll</code> file in the directory of 
        /// the currently executing process (that is using the CudaDnn object) is used.</param>
        /// <param name="bResetFirst">Specifies to reset the device before initialzing.  <b>IMPORTANT:</b> It is only recommended to set this to <code>true</code> when testing.</param>
        /// <param name="bEnableMemoryTrace">Optionally, specifies to enable the memory tracing (only supported in debug mode and dramatically slows down processing).</param>
        public CudaDnn(int nDeviceID, DEVINIT flags = (DEVINIT.CUBLAS | DEVINIT.CURAND), long? lSeed = null, string strPath = "", bool bResetFirst = false, bool bEnableMemoryTrace = false)
        {
            m_memTracker = new CudaDnnMemoryTracker<T>(bEnableMemoryTrace);
            m_nDeviceId = nDeviceID;
            m_nIdx = get_index();

            if (strPath == null || strPath.Length == 0)
                strPath = s_strCudaPath;

            m_strPath = strPath;
            m_dt = (typeof(T) == typeof(double)) ? DataType.DOUBLE : DataType.FLOAT;

            try
            {
                m_cuda = new CudaControlLib.CudaKernel();
            }
            catch (Exception excpt)
            {
                throw new Exception("The CudaControl is not registered! Make sure that you are using the 'x64' build and if so, run 'regsvr32 CudaControl.dll' from a CMD window with Administrative privileges to register.", excpt);
            }

            try
            {
                if (string.IsNullOrEmpty(strPath))
                    strPath = GetCudaDnnDllPath();

                m_strPath = strPath;

                string strDir = System.IO.Path.GetDirectoryName(strPath);
                string strCurDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(strDir);

                m_cuda.Load(strPath);

                Directory.SetCurrentDirectory(strCurDir);
            }
            catch (Exception excpt)
            {
                if (excpt.Message != null && excpt.Message.Length > 0)
                    throw excpt;

                throw new Exception("The CudaDnnDll.x.dll at '" + strPath + "' failed to load.  The error code = 0x" + excpt.HResult.ToString("X"));
            }

            try
            {
                lock (m_createSync)
                {
                    if (m_dt == DataType.DOUBLE)
                    {
                        double[] rg = m_cuda.RunDouble(0, (int)CUDAFN.INITIALIZE, m_param.AsDouble(nDeviceID, (int)flags));
                        m_hKernel = (long)rg[0];
                    }
                    else
                    {
                        float[] rg = m_cuda.RunFloat(0, (int)CUDAFN.INITIALIZE, m_param.AsFloat(nDeviceID, (int)flags));
                        m_hKernel = (long)rg[0];
                    }
                }
            }
            catch (Exception excpt)
            {
                if (excpt.Message != null && excpt.Message.Length > 0)
                    throw excpt;

                throw new Exception("CudaDnn failed to initialize.  You may need to reboot or reset the Cuda GPU #" + nDeviceID.ToString() + ".  The error code = 0x" + excpt.HResult.ToString("X"));
            }

            if (bResetFirst)
            {
                ResetDevice();

                lock (m_createSync)
                {
                    if (m_dt == DataType.DOUBLE)
                    {
                        double[] rg = m_cuda.RunDouble(0, (int)CUDAFN.INITIALIZE, m_param.AsDouble(nDeviceID, (int)flags));
                        m_hKernel = (long)rg[0];
                    }
                    else
                    {
                        float[] rg = m_cuda.RunFloat(0, (int)CUDAFN.INITIALIZE, m_param.AsFloat(nDeviceID, (int)flags));
                        m_hKernel = (long)rg[0];
                    }
                }
            }

            if (lSeed.HasValue)
                SetRandomSeed(lSeed.Value);

            m_tOne = (T)Convert.ChangeType(1.0, typeof(T));
            m_tZero = (T)Convert.ChangeType(0.0, typeof(T));
        }

        /// <summary>
        /// Alternate CudaDnn constructor.
        /// </summary>
        /// <param name="cuda">Specifies an already created CudaDn instance.  The internal Cuda Control of this instance is used by the new instance.</param>
        /// <param name="bEnableGhostMemory">Specifies to enable the ghost memory used to estimate GPU memory usage without allocating any GPU memory.</param>
        public CudaDnn(CudaDnn<T> cuda, bool bEnableGhostMemory)
        {
            m_nDeviceId = cuda.m_nDeviceId;
            m_nIdx = get_index();

            m_strPath = cuda.m_strPath;
            m_dt = cuda.m_dt;
            m_cuda = cuda.m_cuda;
            m_hKernel = cuda.m_hKernel;
            m_tOne = cuda.m_tOne;
            m_tZero = cuda.m_tZero;

            if (bEnableGhostMemory)
            {
                m_rgGhostMemory = new Dictionary<long, T[]>();
                m_bGhostMemoryEnabled = true;
            }

            m_bOwner = false;
        }

        /// <summary>
        /// Disposes this instance freeing up all of its host and GPU memory.
        /// </summary>
        /// <param name="bDisposing">When true, specifies that the call is from a Dispose call.</param>
        protected virtual void Dispose(bool bDisposing)
        {
            lock (m_cleanupSync)
            {
                if (m_bOwner && m_hKernel != 0)
                {
                    if (m_dt == DataType.DOUBLE)
                        m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CLEANUP, new double[] { m_nIdx });
                    else
                        m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CLEANUP, new float[] { m_nIdx });

                    m_hKernel = 0;
                    m_cuda = null;
                }
            }
        }

        /// <summary>
        /// Disposes this instance freeing up all of its host and GPU memory.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Returns the path to the CudaDnnDll module to use for low level CUDA processing.
        /// </summary>
        /// <returns>The CudaDnnDll path is returned.</returns>
        public static string GetCudaDnnDllPath()
        {
            FileInfo fi = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);

            string strPath = fi.DirectoryName + "\\cuda_12.3\\CudaDnnDll.12.3.dll";
            if (!File.Exists(strPath))
            {
                strPath = fi.DirectoryName + "\\CudaDnnDll.12.3.dll";
                if (!File.Exists(strPath))
                {
                    strPath = fi.DirectoryName + "\\cuda_12.2\\CudaDnnDll.12.2.dll";
                    if (!File.Exists(strPath))
                    {
                        strPath = fi.DirectoryName + "\\CudaDnnDll.12.2.dll";
                        if (!File.Exists(strPath))
                        {
                            strPath = fi.DirectoryName + "\\cuda_12.1\\CudaDnnDll.12.1.dll";

                            if (!File.Exists(strPath))
                            {
                                strPath = fi.DirectoryName + "\\CudaDnnDll.12.1.dll";
                                if (!File.Exists(strPath))
                                {
                                    strPath = fi.DirectoryName + "\\cuda_12.0\\CudaDnnDll.12.0.dll";
                                    if (!File.Exists(strPath))
                                    {
                                        strPath = fi.DirectoryName + "\\CudaDnnDll.12.0.dll";
                                        if (!File.Exists(strPath))
                                        {
                                            if (!File.Exists(strPath))
                                            {
                                                strPath = fi.DirectoryName + "\\cuda_11.8\\CudaDnnDll.11.8.dll";
                                                if (!File.Exists(strPath))
                                                {
                                                    strPath = fi.DirectoryName + "\\CudaDnnDll.11.8.dll";
                                                    if (!File.Exists(strPath))
                                                    {
                                                        strPath = fi.DirectoryName + "\\cuda_11.7\\CudaDnnDll.11.7.dll";
                                                        if (!File.Exists(strPath))
                                                        {
                                                            strPath = fi.DirectoryName + "\\CudaDnnDll.11.7.dll";
                                                            if (!File.Exists(strPath))
                                                            {
                                                                strPath = fi.DirectoryName + "\\cuda_11.6\\CudaDnnDll.11.6.dll";
                                                                if (!File.Exists(strPath))
                                                                {
                                                                    strPath = fi.DirectoryName + "\\CudaDnnDll.11.6.dll";
                                                                    if (!File.Exists(strPath))
                                                                    {
                                                                        strPath = fi.DirectoryName + "\\cuda_11.5\\CudaDnnDll.11.5.dll";
                                                                        if (!File.Exists(strPath))
                                                                        {
                                                                            strPath = fi.DirectoryName + "\\CudaDnnDll.11.5.dll";
                                                                            if (!File.Exists(strPath))
                                                                            {
                                                                                strPath = fi.DirectoryName + "\\cuda_11.4\\CudaDnnDll.11.4.dll";
                                                                                if (!File.Exists(strPath))
                                                                                {
                                                                                    strPath = fi.DirectoryName + "\\CudaDnnDll.11.4.dll";
                                                                                    if (!File.Exists(strPath))
                                                                                    {
                                                                                        strPath = fi.DirectoryName + "\\cuda_11.3\\CudaDnnDll.11.3.dll";
                                                                                        if (!File.Exists(strPath))
                                                                                        {
                                                                                            strPath = fi.DirectoryName + "\\CudaDnnDll.11.3.dll";
                                                                                            if (!File.Exists(strPath))
                                                                                            {
                                                                                                strPath = fi.DirectoryName + "\\cuda_11.2\\CudaDnnDll.11.2.dll";
                                                                                                if (!File.Exists(strPath))
                                                                                                {
                                                                                                    strPath = fi.DirectoryName + "\\CudaDnnDll.11.2.dll";
                                                                                                    if (!File.Exists(strPath))
                                                                                                    {
                                                                                                        strPath = fi.DirectoryName + "\\cuda_11.1\\CudaDnnDll.11.1.dll";
                                                                                                        if (!File.Exists(strPath))
                                                                                                        {
                                                                                                            strPath = fi.DirectoryName + "\\CudaDnnDll.11.1.dll";
                                                                                                            if (!File.Exists(strPath))
                                                                                                            {
                                                                                                                strPath = fi.DirectoryName + "\\cuda_11.0\\CudaDnnDll.11.0.dll";
                                                                                                                if (!File.Exists(strPath))
                                                                                                                {
                                                                                                                    strPath = fi.DirectoryName + "\\CudaDnnDll.11.0.dll";
                                                                                                                    if (!File.Exists(strPath))
                                                                                                                    {
                                                                                                                        strPath = fi.DirectoryName + "\\cuda_10.2\\CudaDnnDll.10.2.dll";
                                                                                                                        if (!File.Exists(strPath))
                                                                                                                        {
                                                                                                                            strPath = fi.DirectoryName + "\\CudaDnnDll.10.2.dll";
                                                                                                                            if (!File.Exists(strPath))
                                                                                                                            {
                                                                                                                                strPath = fi.DirectoryName + "\\cuda_10.2.3_5\\CudaDnnDll.10.2.dll";
                                                                                                                                if (!File.Exists(strPath))
                                                                                                                                {
                                                                                                                                    strPath = fi.DirectoryName + "\\CudaDnnDll.10.2.3_5.dll";
                                                                                                                                    if (!File.Exists(strPath))
                                                                                                                                    {
                                                                                                                                        strPath = fi.DirectoryName + "\\CudaDnnDll.10.1.dll";
                                                                                                                                        if (!File.Exists(strPath))
                                                                                                                                        {
                                                                                                                                            strPath = fi.DirectoryName + "\\CudaDnnDll.10.0.dll";
                                                                                                                                            if (!File.Exists(strPath))
                                                                                                                                            {
                                                                                                                                                strPath = fi.DirectoryName + "\\CudaDnnDll.9.2.dll";
                                                                                                                                                if (!File.Exists(strPath))
                                                                                                                                                {
                                                                                                                                                    strPath = fi.DirectoryName + "\\CudaDnnDll.9.1.dll";
                                                                                                                                                    if (!File.Exists(strPath))
                                                                                                                                                    {
                                                                                                                                                        if (!File.Exists(strPath))
                                                                                                                                                            strPath = fi.DirectoryName + "\\CudaDnnDll.8.dll";
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return strPath;
        }

        /// <summary>
        /// Disables the ghost memory, if enabled.
        /// </summary>
        public void DisableGhostMemory()
        {
            m_bGhostMemoryEnabled = false;
        }

        /// <summary>
        /// Resets the ghost memory by enabling it if this instance was configured to use ghost memory.
        /// </summary>
        public void ResetGhostMemory()
        {
            if (m_rgGhostMemory != null)
                m_bGhostMemoryEnabled = true;
            else
                m_bGhostMemoryEnabled = false;
        }

        /// <summary>
        /// Returns the total amount of GPU memory used by this instance.
        /// </summary>
        public ulong TotalMemoryUsed
        {
            get { return m_memTracker.TotalMemoryUsed; }
        }

        /// <summary>
        /// Returns the total amount of memory used.
        /// </summary>
        public string TotalMemoryUsedAsText
        {
            get { return m_memTracker.TotalMemoryUsedText; }
        }

        /// <summary>
        /// Returns the Low-Level kernel handle used for this instance.  Each Low-Level kernel maintains its own
        /// set of look-up tables for memory, streams, cuDnn constructs, etc.
        /// </summary>
        public long KernelHandle
        {
            get { return m_hKernel; }
        }

        /// <summary>
        /// Copy memory from the look-up tables in one kernel to another.
        /// </summary>
        /// <param name="nCount">Specifies the number of items to copy.</param>
        /// <param name="hSrc">Specifies the handle to the source memory.</param>
        /// <param name="nSrcOffset">Specifies the offset (in items, not bytes) from which to start the copy in the source memory.</param>
        /// <param name="hDstKernel">Specifies the destination kernel holding the look-up table and memory where the data is to be copied.</param>
        /// <param name="hDst">Specifies the handle to the destination memory where the data is to be copied.</param>
        /// <param name="nDstOffset">Specifies the offset (in items, not bytes) where the copy to to be placed within the destination data.</param>
        /// <param name="hHostBuffer">Specifies the handle to the host buffer to be used when transfering the data from one kernel to another.</param>
        /// <param name="hHostKernel">Optionally, specifies the handle to the kernel holding the look-up table for the host buffer.</param>
        /// <param name="hStream">Optionally, specifies the handle to the CUDA stream to use for the transfer.</param>
        /// <param name="hSrcKernel">Optionally, specifies the handle to the source kernel.</param>
        public void KernelCopy(int nCount, long hSrc, int nSrcOffset, long hDstKernel, long hDst, int nDstOffset, long hHostBuffer, long hHostKernel = -1, long hStream = -1, long hSrcKernel = -1)
        {
            if (hSrcKernel == -1)
                hSrcKernel = m_hKernel;

            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)hSrcKernel, (int)CUDAFN.KERNEL_MEMCOPY, m_param.AsDouble(nCount, hSrc, nSrcOffset, hDstKernel, hDst, nDstOffset, hHostBuffer, hHostKernel, hStream));
            else
                m_cuda.RunFloat((int)hSrcKernel, (int)CUDAFN.KERNEL_MEMCOPY, m_param.AsFloat(nCount, hSrc, nSrcOffset, hDstKernel, hDst, nDstOffset, hHostBuffer, hHostKernel, hStream));
        }

        /// <summary>
        /// Add memory from one kernel to memory residing on another kernel.
        /// </summary>
        /// <param name="nCount">Specifies the number of items within both A and B.</param>
        /// <param name="hA">Specifies the handle to the memory A.</param>
        /// <param name="hDstKernel">Specifies the kernel where the memory B and the desitnation memory C reside.</param>
        /// <param name="hB">Specifies the handle to the memory B (for which A will be added).</param>
        /// <param name="hC">Specifies the destination data where A+B will be placed.</param>
        public void KernelAdd(int nCount, long hA, long hDstKernel, long hB, long hC)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.KERNEL_ADD, m_param.AsDouble(nCount, hA, hDstKernel, hB, hC));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.KERNEL_ADD, m_param.AsFloat(nCount, hA, hDstKernel, hB, hC));
        }

        /// <summary>
        /// Copies an Nccl handle from one kernel to the current kernel of the current CudaDnn instance.
        /// </summary>
        /// <remarks>
        /// Nccl handles are created on the main Kernel, but when used must transferred to the destination kernel (running on
        /// a different thread) where the secondary Nccl handle is used.
        /// </remarks>
        /// <param name="hSrcKernel">Specifies the source kernel (typically where the Nccl handle was created).</param>
        /// <param name="hSrcNccl">Specifies the source Nccl handle to be copied.</param>
        /// <returns></returns>
        public long KernelCopyNccl(long hSrcKernel, long hSrcNccl)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.KERNEL_COPY_NCCL, m_param.AsDouble(hSrcKernel, hSrcNccl));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.KERNEL_COPY_NCCL, m_param.AsFloat(hSrcKernel, hSrcNccl));
                return (long)rg[0];
            }
        }

        private static int get_index()
        {
            s_nIdxSeed++;
            return s_nIdxSeed;
        }

        /// <summary>
        /// Used to <i>optionally</i> set the default path to the Low-Level Cuda Dnn DLL file.
        /// </summary>
        /// <param name="strPath">Specifies the file path to the Low-Level Cuda Dnn DLL file to use.</param>
        public static void SetDefaultCudaPath(string strPath)
        {
            s_strCudaPath = strPath;
        }

        /// <summary>
        /// Returns the base type size in bytes.
        /// </summary>
        /// <param name="bUseHalfSize">Specifies whether or not to use half size or the base size.</param>
        public static ulong basetype_size(bool bUseHalfSize)
        {
            if (bUseHalfSize)
                return 2;

            if (typeof(T) == typeof(float))
                return 4;
            else
                return 8;
        }

        private double convertD(T fVal)
        {
            return (double)Convert.ChangeType(fVal, typeof(double));
        }

        private float convertF(T fVal)
        {
            return (float)Convert.ChangeType(fVal, typeof(float));
        }

        /// <summary>
        /// Specifies the file path used to load the Low-Level Cuda DNN Dll file.
        /// </summary>
        public string Path
        {
            get { return m_strPath; }
        }

        /// <summary>
        /// Specifies the default path used t load the Low-Level Cuda DNN Dll file.
        /// </summary>
        public static string DefaultPath
        {
            get { return s_strCudaPath; }
        }

        /// <summary>
        /// Specifies the ignore label used to ignore the ignore label.
        /// </summary>
        public static int IGNORE_LABEL_NOP
        {
            get { return -9999; }
        }

#pragma warning disable 1591

    public void CombineData(int nCount, long hOriginal, long hUpdated, double dfUpdatedPct, long hServer, double dfServerPct, long hNewData) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COMBINE_DATA, m_param.AsDouble(dfUpdatedPct, dfServerPct), m_param.AsLong(nCount, hOriginal, hUpdated, 0, hServer, 0, hNewData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COMBINE_DATA, m_param.AsFloat((float)dfUpdatedPct, (float)dfServerPct), m_param.AsLong(nCount, hOriginal, hUpdated, 0, hServer, 0, hNewData));
        }

#pragma warning restore 1591


        //---------------------------------------------------------------------
        //  ICudaDevice Methods
        //---------------------------------------------------------------------
        #region ICudaDevice Methods

        /// <summary>
        /// Set the device ID used by the current instance of CudaDnn.
        /// </summary>
        /// <param name="nDeviceID">Specifies the zero-based device (GPU) id.  When -1, the device ID is set to the device ID used to create the instance of CudaDnn.</param>
        /// <param name="flags">Optionally, specifies the initialization flags.</param>
        /// <param name="lSeed">Optionally, specifies the random number generator seed.</param>
        public void SetDeviceID(int nDeviceID = -1, DEVINIT flags = DEVINIT.NONE, long? lSeed = null)
        {
            if (m_cuda == null || m_hKernel <= 0)
                throw new Exception("CudaDnn has already been disposed!");

            if (nDeviceID == -1)
                nDeviceID = m_nDeviceId;
            else
                m_nDeviceId = nDeviceID;

            if (m_dt == DataType.DOUBLE)
            {
                if (lSeed.HasValue)
                    m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.SETDEVICE, m_param.AsDouble(nDeviceID, (int)flags, lSeed.Value));
                else
                    m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.SETDEVICE, m_param.AsDouble(nDeviceID, (int)flags));
            }
            else
            {
                if (lSeed.HasValue)
                    m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.SETDEVICE, m_param.AsFloat(nDeviceID, (int)flags, lSeed.Value));
                else
                    m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.SETDEVICE, m_param.AsFloat(nDeviceID, (int)flags));
            }
        }

        /// <summary>
        /// Set the random number generator seed.
        /// </summary>
        /// <param name="lSeed">Specifies the seed to set.</param>
        public void SetRandomSeed(long lSeed)
        {
            if (m_cuda == null || m_hKernel <= 0)
                throw new Exception("CudaDnn has already been disposed!");

            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.SETRANDOMSEED, m_param.AsDouble(lSeed));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.SETRANDOMSEED, m_param.AsFloat(lSeed));
        }

        /// <summary>
        /// Returns the original device ID used to create the instance of CudaDnn.
        /// </summary>
        public int OriginalDeviceID
        {
            get { return m_nDeviceId; }
        }

        /// <summary>
        /// Returns the current device id set within Cuda.
        /// </summary>
        /// <returns>The device id.</returns>
        public int GetDeviceID()
        {
            if (m_cuda == null || m_hKernel <= 0)
                throw new Exception("CudaDnn has already been disposed!");

            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.GETDEVICE, null);
                return (int)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.GETDEVICE, null);
                return (int)rg[0];
            }
        }

        /// <summary>
        /// Query the name of a device.
        /// </summary>
        /// <param name="nDeviceID">Specifies the device id.</param>
        /// <returns>The name of the GPU at the device id is returned.</returns>
        public string GetDeviceName(int nDeviceID)
        {
            if (m_cuda == null || m_hKernel <= 0)
                throw new Exception("CudaDnn has already been disposed!");

            string[] rgstr = m_cuda.QueryString((int)m_hKernel, (int)CUDAQRY.DEVICE_NAME, new int[] { nDeviceID });
            return rgstr[0];
        }

        /// <summary>
        /// Query the peer-to-peer information of a device.
        /// </summary>
        /// <param name="nDeviceID">Specifies the device id.</param>
        /// <returns>The peer-to-per information of the GPU at the device id is returned.</returns>
        public string GetDeviceP2PInfo(int nDeviceID)
        {
            if (m_cuda == null || m_hKernel <= 0)
                throw new Exception("CudaDnn has already been disposed!");

            string[] rgstr = m_cuda.QueryString((int)m_hKernel, (int)CUDAQRY.DEVICE_P2P_INFO, new int[] { nDeviceID });
            return rgstr[0];
        }

        /// <summary>
        /// Query the device information of a device.
        /// </summary>
        /// <param name="nDeviceID">Specifies the device id.</param>
        /// <param name="bVerbose">When true, more detailed information is returned.</param>
        /// <returns></returns>
        public string GetDeviceInfo(int nDeviceID, bool bVerbose = false)
        {
            if (m_cuda == null || m_hKernel <= 0)
                throw new Exception("CudaDnn has already been disposed!");

            string[] rgstr = m_cuda.QueryString((int)m_hKernel, (int)CUDAQRY.DEVICE_INFO, new int[] { nDeviceID, (bVerbose) ? 1 : 0 });
            return rgstr[0];
        }

        /// <summary>
        /// Reset the current device.
        /// </summary>
        /// <remarks><b>IMPORTANT:</b> This function will delete all memory and state information on the current device, which may
        /// cause other CudaDnn instances using the same device, to fail.  For that reason, it is recommended to only call
        /// this function when testing.</remarks>
        public void ResetDevice()
        {
            if (m_cuda == null || m_hKernel <= 0)
                throw new Exception("CudaDnn has already been disposed!");

            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.RESETDEVICE, null);
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.RESETDEVICE, null);
        }

        /// <summary>
        /// Synchronize the operations on the current device.
        /// </summary>
        public void SynchronizeDevice()
        {
            if (m_cuda == null || m_hKernel <= 0)
                throw new Exception("CudaDnn has already been disposed!");

            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.SYNCHRONIZEDEVICE, null);
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.SYNCHRONIZEDEVICE, null);
        }

        /// <summary>
        /// Query the mutli-gpu board group id for a device.
        /// </summary>
        /// <param name="nDeviceID">Specifies the device id.</param>
        /// <returns>The mutli-gpu board group id is returned.</returns>
        public int GetMultiGpuBoardGroupID(int nDeviceID)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.GETDEVICEPROP, m_param.AsDouble(nDeviceID, (int)DEVPROP.MULTIGPUBOARDGROUPID));
                return (int)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.GETDEVICEPROP, m_param.AsFloat(nDeviceID, (int)DEVPROP.MULTIGPUBOARDGROUPID));
                return (int)rg[0];
            }
        }

        /// <summary>
        /// Query the number of devices (gpu's) installed.
        /// </summary>
        /// <returns>The number of GPU's is returned.</returns>
        public int GetDeviceCount()
        {
            if (m_cuda == null || m_hKernel <= 0)
                return 0;

            try
            {
                if (m_dt == DataType.DOUBLE)
                {
                    double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.GETDEVICEPROP, m_param.AsDouble(0, (int)DEVPROP.DEVICECOUNT));
                    return (int)rg[0];
                }
                else
                {
                    float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.GETDEVICEPROP, m_param.AsFloat(0, (int)DEVPROP.DEVICECOUNT));
                    return (int)rg[0];
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Query the current Gpu Compute Level
        /// </summary>
        /// <returns>The major, minor compute level is returned.</returns>
        public int[] GetComputeLevel(int nDeviceID = -1)
        {
            if (m_cuda == null || m_hKernel <= 0)
                return null;

            try
            {
                if (m_dt == DataType.DOUBLE)
                {
                    double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.GETDEVICEPROP, m_param.AsDouble(nDeviceID, (int)DEVPROP.COMPUTELEVEL));
                    return new int[] { (int)rg[0], (int)rg[1] };
                }
                else
                {
                    float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.GETDEVICEPROP, m_param.AsFloat(nDeviceID, (int)DEVPROP.COMPUTELEVEL));
                    return new int[] { (int)rg[0], (int)rg[1] };
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Check the memory attributes of two memory blocks on different devices to see if they are compatible
        /// for peer-to-peer memory transfers.
        /// </summary>
        /// <param name="hSrc">Specifies the handle to the source memory.</param>
        /// <param name="nSrcDeviceID">Specifies the device id where the source memory resides.</param>
        /// <param name="hDst">Specifies the handle to the destination memory.</param>
        /// <param name="nDstDeviceID">Specifies the device id where the destination memory resides.</param>
        /// <returns>This function returns <code>true</code> when both devices support peer-to-peer communcation, <code>false</code> otherwise.</returns>
        public bool CheckMemoryAttributes(long hSrc, int nSrcDeviceID, long hDst, int nDstDeviceID)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CHECKMEMORYATTRIB, m_param.AsDouble(hSrc, nSrcDeviceID, hDst, nDstDeviceID));
                return (rg[0] == 0) ? false : true;
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CHECKMEMORYATTRIB, m_param.AsFloat(hSrc, nSrcDeviceID, hDst, nDstDeviceID));
                return (rg[0] == 0) ? false : true;
            }
        }

        /// <summary>
        /// Queries the amount of total, free and used memory on a given GPU.
        /// </summary>
        /// <param name="dfFree">Specifies the amount of free memory in GB.</param>
        /// <param name="dfUsed">Specifies the amount of used memory in GB.</param>
        /// <param name="bCudaCallUsed">Specifies whether or not the used memory is an estimate calculated using the Low-Level Cuda DNN Dll handle table.</param>
        /// <param name="nDeviceID">Specifies the specific device id to query, or if -1, uses calculates an estimate of the memory used using the current low-level Cuda DNN Dll handle table.</param>
        /// <returns>The device's total amount of memory in GB is returned.</returns>
        public double GetDeviceMemory(out double dfFree, out double dfUsed, out bool bCudaCallUsed, int nDeviceID = -1)
        {
            if (nDeviceID == -1)
                nDeviceID = m_nDeviceId;

            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.GETDEVICEMEMORY, m_param.AsDouble(nDeviceID));
                dfFree = rg[1];
                dfUsed = rg[2];
                bCudaCallUsed = (rg[3] == 0) ? false : true;
                return rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.GETDEVICEMEMORY, m_param.AsFloat(nDeviceID));
                dfFree = (double)rg[1];
                dfUsed = (double)rg[2];
                bCudaCallUsed = (rg[3] == 0) ? false : true;
                return (double)rg[0];
            }
        }

        /// <summary>
        /// The GetRequiredCompute function returns the Major and Minor compute values required by the current CudaDNN DLL used.
        /// </summary>
        /// <param name="nMinMajor">Specifies the minimum required major compute value.</param>
        /// <param name="nMinMinor">Specifies the minimum required minor compute value.</param>
        /// <remarks>
        /// Together the Major.Minor compute values define the minimum required compute for the CudaDNN DLL used.
        /// </remarks>
        /// <returns>
        /// The path to the CudaDNN dll in use is returned.
        /// </returns>
        public string GetRequiredCompute(out int nMinMajor, out int nMinMinor)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.GETREQUIREDCOMPUTE, null);
                nMinMajor = (int)rg[0];
                nMinMinor = (int)rg[1];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.GETREQUIREDCOMPUTE, null);
                nMinMajor = (int)rg[0];
                nMinMinor = (int)rg[1];
            }

            return m_strPath;
        }

        /// <summary>
        /// Query whether or not two devices can access each other via peer-to-peer memory copies.
        /// </summary>
        /// <param name="nSrcDeviceID">Specifies the device id of the source.</param>
        /// <param name="nPeerDeviceID">Specifies the device id of the peer to the source device.</param>
        /// <returns><code>true</code> is returned if the source device can access the peer device via peer-to-peer communcation, <code>false</code> otherwise.</returns>
        public bool DeviceCanAccessPeer(int nSrcDeviceID, int nPeerDeviceID)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.DEVICE_CANACCESSPEER, m_param.AsDouble(nSrcDeviceID, nPeerDeviceID));
                return (rg[0] == 0) ? false : true;
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.DEVICE_CANACCESSPEER, m_param.AsFloat(nSrcDeviceID, nPeerDeviceID));
                return (rg[0] == 0) ? false : true;
            }
        }

        /// <summary>
        /// Enables peer-to-peer access between the current device used by the CudaDnn instance and a peer device.
        /// </summary>
        /// <param name="nPeerDeviceID">Specifies the device id of the peer device.</param>
        public void DeviceEnablePeerAccess(int nPeerDeviceID)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.DEVICE_ENABLEPEERACCESS, m_param.AsDouble(nPeerDeviceID));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.DEVICE_ENABLEPEERACCESS, m_param.AsFloat(nPeerDeviceID));
        }

        /// <summary>
        /// Disables peer-to-peer access between the current device used by the CudaDnn instance and a peer device.
        /// </summary>
        /// <param name="nPeerDeviceID">Specifies the device id of the peer device.</param>
        public void DeviceDisablePeerAccess(int nPeerDeviceID)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.DEVICE_DISABLEPEERACCESS, m_param.AsDouble(nPeerDeviceID));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.DEVICE_DISABLEPEERACCESS, m_param.AsFloat(nPeerDeviceID));
        }

        #endregion

        //---------------------------------------------------------------------
        //  ICudaMemory Methods
        //---------------------------------------------------------------------
        #region ICudaMemory Methods

        /// <summary>
        /// Allocate a block of GPU memory and copy a list of doubles to it.
        /// </summary>
        /// <remarks>This function converts the input array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="rg">Specifies a list of doubles to copy to the GPU.</param>
        /// <returns>The handle to the GPU memory is returned.</returns>
        public long AllocMemory(List<double> rg)
        {
            return AllocMemory(rg.ToArray());
        }

        /// <summary>
        /// Allocate a block of GPU memory and copy a list of floats to it.
        /// </summary>
        /// <remarks>This function converts the input array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="rg">Specifies a list of floats to copy to the GPU.</param>
        /// <returns>The handle to the GPU memory is returned.</returns>
        public long AllocMemory(List<float> rg)
        {
            return AllocMemory(rg.ToArray());
        }

        /// <summary>
        /// Allocate a block of GPU memory and copy an array of doubles to it, optionally using a stream for the copy.
        /// </summary>
        /// <remarks>This function converts the input array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="rgSrc">Specifies an array of doubles to copy to the GPU.</param>
        /// <param name="hStream">Optionally specifies a stream to use for the copy.</param>
        /// <returns>The handle to the GPU memory is returned.</returns>
        public long AllocMemory(double[] rgSrc, long hStream = 0)
        {
            return AllocMemory(convert(rgSrc), hStream);
        }

        /// <summary>
        /// Allocate a block of GPU memory and copy an array of float to it, optionally using a stream for the copy.
        /// </summary>
        /// <remarks>This function converts the input array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="rgSrc">Specifies an array of float to copy to the GPU.</param>
        /// <param name="hStream">Optionally specifies a stream to use for the copy.</param>
        /// <returns>The handle to the GPU memory is returned.</returns>
        public long AllocMemory(float[] rgSrc, long hStream = 0)
        {
            return AllocMemory(convert(rgSrc), hStream);
        }

        /// <summary>
        /// Allocate a block of GPU memory and copy an array of type 'T' to it, optionally using a stream for the copy.
        /// </summary>
        /// <param name="rgSrc">Specifies an array of 'T' to copy to the GPU.</param>
        /// <param name="hStream">Optionally, specifies a stream to use for the copy.</param>
        /// <param name="bHalfSize">Optionally, specifies to use half size float memory - only available with the 'float' base type.</param>
        /// <returns>The handle to the GPU memory is returned.</returns>
        public long AllocMemory(T[] rgSrc, long hStream = 0, bool bHalfSize = false)
        {
            if (rgSrc == null)
                throw new ArgumentNullException();

            if (rgSrc.Length == 0)
                throw new ArgumentOutOfRangeException();

            try
            {
                if (m_dt == DataType.DOUBLE)
                {
                    if (bHalfSize)
                        throw new Exception("Half sizes are only supported with the 'float' base type.");

                    List<double> rgInput = new List<double>() { rgSrc.Length };
                    List<long> rgInput2 = new List<long>() { rgSrc.Length };

                    if (hStream > 0)
                    {
                        rgInput.Add(hStream);
                        rgInput2.Add(hStream);
                    }

                    rgInput.AddRange(convertD(rgSrc));

                    double[] rg;

                    lock (m_memSync)
                    {
                        if (m_rgGhostMemory == null || !m_bGhostMemoryEnabled)
                        {
                            rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.ALLOCMEM, rgInput.ToArray(), rgInput2.ToArray());
                        }
                        else
                        {
                            m_nGhostMemoryIndex++;
                            m_rgGhostMemory.Add(m_nGhostMemoryIndex, convert(Utility.Clone<double>(rgInput).ToArray()));
                            rg = new double[] { m_nGhostMemoryIndex };
                        }

                        return m_memTracker.AllocMemory(m_hKernel, m_nDeviceId, (long)rg[0], (ulong)rgInput.Count, bHalfSize);
                    }
                }
                else
                {
                    List<float> rgInput = new List<float>() { rgSrc.Length };
                    List<long> rgInput2 = new List<long>() { rgSrc.Length };

                    if (hStream > 0)
                    {
                        rgInput.Add(hStream);
                        rgInput2.Add(hStream);
                    }

                    rgInput.AddRange(convertF(rgSrc));

                    float[] rg;

                    lock (m_memSync)
                    {
                        if (m_rgGhostMemory == null || !m_bGhostMemoryEnabled)
                        {
                            if (bHalfSize)
                                rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.ALLOCMEM_HALF, rgInput.ToArray(), rgInput2.ToArray());
                            else
                                rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.ALLOCMEM, rgInput.ToArray(), rgInput2.ToArray());
                        }
                        else
                        {
                            m_nGhostMemoryIndex++;
                            m_rgGhostMemory.Add(m_nGhostMemoryIndex, convert(Utility.Clone<float>(rgInput).ToArray()));
                            rg = new float[] { m_nGhostMemoryIndex };
                        }

                        return m_memTracker.AllocMemory(m_hKernel, m_nDeviceId, (long)rg[0], (ulong)rgInput.Count, bHalfSize);
                    }
                }
            }
            catch (Exception excpt)
            {
                string strMemory = m_memTracker.TotalMemoryUsedText;
                string strDevice = GetDeviceName(m_nDeviceId);
                throw new Exception("Out of memory!  You are currently using " + strMemory + " of memory on " + strDevice + ".  You may need to use a different GPU that has more memory.", excpt);
            }
        }

        /// <summary>
        /// Returns the base data type size (e.g. float= 4, double = 8).
        /// </summary>
        public static ulong BaseSize
        {
            get { return m_lBaseSize; }
        }

        /// <summary>
        /// Converts the byte size into the number of items in the base data type of float or double.
        /// </summary>
        /// <param name="ulSizeInBytes">Specifies the size in bytes to convert.</param>
        /// <returns>The number of items is returned.</returns>
        public static ulong ConvertByteSizeToCount(ulong ulSizeInBytes)
        {
            return ulSizeInBytes / m_lBaseSize;
        }

        /// <summary>
        /// Allocate a block of GPU memory with a specified capacity.
        /// </summary>
        /// <param name="lCapacity">Specifies the capacity to allocate (in items, not bytes).</param>
        /// <param name="bHalfSize">Optionally, specifies to use half size float memory - only available with the 'float' base type.</param>
        /// <returns>The handle to the GPU memory is returned.</returns>
        public long AllocMemory(long lCapacity, bool bHalfSize = false)
        {
            if (lCapacity <= 0)
                throw new ArgumentOutOfRangeException();

            long[] rgIn = new long[] { lCapacity };

            try
            {
                if (m_dt == DataType.DOUBLE)
                {
                    if (bHalfSize)
                        throw new Exception("Half sizes are only supported with the 'float' base type.");

                    double[] rgOut;
                    lock (m_memSync)
                    {
                        if (m_rgGhostMemory == null || !m_bGhostMemoryEnabled)
                        {
                            rgOut = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.ALLOCMEM, null, rgIn);
                        }
                        else
                        {
                            m_nGhostMemoryIndex++;
                            m_rgGhostMemory.Add(m_nGhostMemoryIndex, convert(Utility.Create<double>((int)lCapacity, 0).ToArray()));
                            rgOut = new double[] { m_nGhostMemoryIndex };
                        }

                        return m_memTracker.AllocMemory(m_hKernel, m_nDeviceId, (long)rgOut[0], (ulong)lCapacity, bHalfSize);
                    }
                }
                else
                {
                    float[] rgOut;
                    lock (m_memSync)
                    {
                        if (m_rgGhostMemory == null || !m_bGhostMemoryEnabled)
                        {
                            if (bHalfSize)
                                rgOut = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.ALLOCMEM_HALF, null, rgIn);
                            else
                                rgOut = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.ALLOCMEM, null, rgIn);
                        }
                        else
                        {
                            m_nGhostMemoryIndex++;
                            m_rgGhostMemory.Add(m_nGhostMemoryIndex, convert(Utility.Create<float>((int)lCapacity, 0).ToArray()));
                            rgOut = new float[] { m_nGhostMemoryIndex };
                        }

                        return m_memTracker.AllocMemory(m_hKernel, m_nDeviceId, (long)rgOut[0], (ulong)lCapacity, bHalfSize);
                    }
                }
            }
            catch (Exception excpt)
            {
                string strMemory = m_memTracker.TotalMemoryUsedText;
                string strDevice = GetDeviceName(m_nDeviceId);
                long lMb = (lCapacity * (int)basetype_size(false)) / 1000000;

                throw new Exception("Out of memory!  There is not enough memory to allocate the requested " + lMb.ToString("N0") + " MB of memory.  You are currently using " + strMemory + " of memory on " + strDevice + ".  You may need to use a different GPU that has more memory.", excpt);
            }
        }

        /// <summary>
        /// Free previously allocated GPU memory.
        /// </summary>
        /// <param name="hMem">Specifies the handle to the GPU memory.</param>
        public void FreeMemory(long hMem)
        {
            if (m_cuda == null || m_hKernel <= 0)
            {
                Trace.WriteLine("WARNING: CudaDnn has already been disposed, cannot free memory.");
                return;
            }

            lock (m_memSync)
            {
                if (m_dt == DataType.DOUBLE)
                {
                    m_memTracker.FreeMemory(m_hKernel, m_nDeviceId, hMem);

                    if (m_rgGhostMemory == null || !m_bGhostMemoryEnabled)
                        m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.FREEMEM, null, m_param.AsLong(hMem));
                    else
                        m_rgGhostMemory.Remove(hMem);
                }
                else
                {
                    m_memTracker.FreeMemory(m_hKernel, m_nDeviceId, hMem);

                    if (m_rgGhostMemory == null || !m_bGhostMemoryEnabled)
                        m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.FREEMEM, null, m_param.AsLong(hMem));
                    else
                        m_rgGhostMemory.Remove(hMem);
                }
            }
        }

        /// <summary>
        /// Copy from GPU memory to Host memory.
        /// </summary>
        /// <param name="lCount">Specifies the number of items (of base type each) to copy.</param>
        /// <param name="hGpuSrc">Specifies the GPU memory containing the source data.</param>
        /// <param name="hHostDst">Specifies the Host memory containing the host destination.</param>
        public void CopyDeviceToHost(long lCount, long hGpuSrc, long hHostDst)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.COPY_DEVICE_TO_HOST, null, m_param.AsLong(lCount, hGpuSrc, hHostDst));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.COPY_DEVICE_TO_HOST, null, m_param.AsLong(lCount, hGpuSrc, hHostDst));
        }

        /// <summary>
        /// Copy from Host memory to GPU memory.
        /// </summary>
        /// <param name="lCount">Specifies the number of items (of base type each) to copy.</param>
        /// <param name="hHostSrc">Specifies the Host memory containing the host source data.</param>
        /// <param name="hGpuDst">Specifies the GPU memory containing the destination.</param>
        public void CopyHostToDevice(long lCount, long hHostSrc, long hGpuDst)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.COPY_HOST_TO_DEVICE, null, m_param.AsLong(lCount, hHostSrc, hGpuDst));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.COPY_HOST_TO_DEVICE, null, m_param.AsLong(lCount, hHostSrc, hGpuDst));
        }

        /// <summary>
        /// Allocate a block of host memory with a specified capacity.
        /// </summary>
        /// <param name="lCapacity">Specifies the capacity to allocate (in items, not bytes).</param>
        /// <returns>The handle to the host memory is returned.</returns>
        public long AllocHostBuffer(long lCapacity)
        {
            if (lCapacity == 0)
                throw new ArgumentOutOfRangeException();

            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.ALLOCHOSTBUFFER, null, m_param.AsLong(lCapacity));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.ALLOCHOSTBUFFER, null, m_param.AsLong(lCapacity));
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free previously allocated host memory.
        /// </summary>
        /// <param name="hMem">Specifies the handle to the host memory.</param>
        public void FreeHostBuffer(long hMem)
        {
            if (m_cuda == null || m_hKernel <= 0)
            {
                Trace.WriteLine("WARNING: CudaDnn has already been disposed, cannot free memory.");
                return;
            }

            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.FREEHOSTBUFFER, null, m_param.AsLong(hMem));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.FREEHOSTBUFFER, null, m_param.AsLong(hMem));
        }

        /// <summary>
        /// Returns the host memory capacity.
        /// </summary>
        /// <param name="hMem">Specfies the host memory.</param>
        /// <returns>The current host memory capacity is returned.</returns>
        public long GetHostBufferCapacity(long hMem)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.GETHOSTBUFFERCAPACITY, null, m_param.AsLong(hMem));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.GETHOSTBUFFERCAPACITY, null, m_param.AsLong(hMem));
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Retrieves the host memory as an array of doubles.
        /// </summary>
        /// <remarks>This function converts the output array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="hMem">Specifies the handle to the host memory.</param>
        /// <returns>An array of doubles is returned.</returns>
        public double[] GetHostMemoryDouble(long hMem)
        {
            return convertD(GetHostMemory(hMem));
        }

        /// <summary>
        /// Retrieves the host memory as an array of floats.
        /// </summary>
        /// <remarks>This function converts the output array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="hMem">Specifies the handle to the host memory.</param>
        /// <returns>An array of floats is returned.</returns>
        public float[] GetHostMemoryFloat(long hMem)
        {
            return convertF(GetHostMemory(hMem));
        }

        /// <summary>
        /// Retrieves the host memory as an array of type 'T'
        /// </summary>
        /// <param name="hMem">Specifies the handle to the host memory.</param>
        /// <returns>An array of type 'T' is returned.</returns>
        public T[] GetHostMemory(long hMem)
        {
            if (m_dt == DataType.DOUBLE)
                return convert(m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.GETHOSTMEM, null, m_param.AsLong(hMem)));
            else
                return convert(m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.GETHOSTMEM, null, m_param.AsLong(hMem)));
        }

        /// <summary>
        /// Retrieves the GPU memory as an array of doubles.
        /// </summary>
        /// <remarks>This function converts the output array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="hMem">Specifies the handle to the GPU memory.</param>
        /// <param name="lCount">Optionally, specifies a count of items to retrieve.</param>
        /// <returns>An array of double is returned.</returns>
        public double[] GetMemoryDouble(long hMem, long lCount = -1)
        {
            return convertD(GetMemory(hMem, lCount));
        }

        /// <summary>
        /// Retrieves the GPU memory as an array of float.
        /// </summary>
        /// <remarks>This function converts the output array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="hMem">Specifies the handle to the GPU memory.</param>
        /// <param name="lCount">Optionally, specifies a count of items to retrieve.</param>
        /// <returns>An array of float is returned.</returns>
        public float[] GetMemoryFloat(long hMem, long lCount = -1)
        {
            return convertF(GetMemory(hMem, lCount));
        }

        /// <summary>
        /// Retrieves the GPU memory as an array of type 'T'
        /// </summary>
        /// <param name="hMem">Specifies the handle to the GPU memory.</param>
        /// <param name="lCount">Optionally, specifies a count of items to retrieve.</param>
        /// <returns>An array of type 'T' is returned.</returns>
        public T[] GetMemory(long hMem, long lCount = -1)
        {
            if (m_dt == DataType.DOUBLE)
            {
                if (m_rgGhostMemory == null)
                {
                    double[] rgr = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.GETMEM, null, m_param.AsLong(hMem, lCount));
                    return convert(rgr);
                }
                else
                {
                    return m_rgGhostMemory[hMem];
                }
            }
            else
            {
                if (m_rgGhostMemory == null)
                {
                    float[] rgr = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.GETMEM, null, m_param.AsLong(hMem, lCount));
                    return convert(rgr);
                }
                else
                {
                    return m_rgGhostMemory[hMem];
                }
            }
        }

        /// <summary>
        /// Copies a list of doubles into a block of already allocated GPU memory.
        /// </summary>
        /// <remarks>This function converts the input array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="hMem">Specifies the handle to the GPU memory.</param>
        /// <param name="rg">Specifies the list of doubles to copy.</param>
        public void SetMemory(long hMem, List<double> rg)
        {
            SetMemory(hMem, rg.ToArray());
        }

        /// <summary>
        /// Copies a list of float into a block of already allocated GPU memory.
        /// </summary>
        /// <remarks>This function converts the input array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="hMem">Specifies the handle to the GPU memory.</param>
        /// <param name="rg">Specifies the list of float to copy.</param>
        public void SetMemory(long hMem, List<float> rg)
        {
            SetMemory(hMem, rg.ToArray());
        }

        /// <summary>
        /// Copies an array of double into a block of already allocated GPU memory.
        /// </summary>
        /// <remarks>This function converts the input array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="hMem">Specifies the handle to the GPU memory.</param>
        /// <param name="rgSrc">Specifies the array of double to copy.</param>
        /// <param name="hStream">Optionally specifies the stream to use for the copy operation.</param>
        public void SetMemory(long hMem, double[] rgSrc, long hStream = 0)
        {
            SetMemory(hMem, convert(rgSrc), hStream);
        }

        /// <summary>
        /// Copies an array of float into a block of already allocated GPU memory.
        /// </summary>
        /// <remarks>This function converts the input array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="hMem">Specifies the handle to the GPU memory.</param>
        /// <param name="rgSrc">Specifies the array of float to copy.</param>
        /// <param name="hStream">Optionally specifies the stream to use for the copy operation.</param>
        public void SetMemory(long hMem, float[] rgSrc, long hStream = 0)
        {
            SetMemory(hMem, convert(rgSrc), hStream);
        }

        /// <summary>
        /// Copies an array of type 'T' into a block of already allocated GPU memory.
        /// </summary>
        /// <param name="hMem">Specifies the handle to the GPU memory.</param>
        /// <param name="rgSrc">Specifies the array of type 'T' to copy.</param>
        /// <param name="hStream">Optionally specifies the stream to use for the copy operation.</param>
        /// <param name="nCount">Optionally, specifies a count of items to retrieve.</param>
        public void SetMemory(long hMem, T[] rgSrc, long hStream = 0, int nCount = -1)
        {
            if (nCount == -1)
                nCount = rgSrc.Length;

            if (rgSrc == null || nCount == 0)
                throw new ArgumentOutOfRangeException("There are no data items to set!");

            if (m_hKernel > 0)
            {
                if (m_rgGhostMemory != null)
                {
                    m_rgGhostMemory[hMem] = Utility.Clone<T>(rgSrc);
                }
                else
                {
                    if (m_dt == DataType.DOUBLE)
                    {
                        int nDataCount = 2;

                        if (hStream > 0)
                            nDataCount++;

                        nDataCount += nCount;

                        double[] rg = new double[nDataCount];

                        rg[0] = hMem;
                        rg[1] = nCount;
                        int nIdx = 2;

                        if (hStream > 0)
                        {
                            rg[nIdx] = hStream;
                            nIdx++;
                        }

                        long[] rgIn = new long[] { hMem, nCount };

                        convertD(rgSrc, rg, nIdx, nCount);
                        m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SETMEM, rg, rgIn);
                    }
                    else
                    {
                        int nDataCount = 2;

                        if (hStream > 0)
                            nDataCount++;

                        nDataCount += nCount;

                        float[] rg = new float[nDataCount];

                        rg[0] = hMem;
                        rg[1] = nCount;
                        int nIdx = 2;

                        if (hStream > 0)
                        {
                            rg[nIdx] = hStream;
                            nIdx++;
                        }

                        long[] rgIn = new long[] { hMem, nCount };

                        convertF(rgSrc, rg, nIdx, nCount);
                        m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SETMEM, rg, rgIn);
                    }
                }
            }
        }

        /// <summary>
        /// Copies an array of double into a block of already allocated GPU memory starting at a specific offset.
        /// </summary>
        /// <remarks>This function converts the input array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="hMem">Specifies the handle to the GPU memory.</param>
        /// <param name="rgSrc">Specifies the array of double to copy.</param>
        /// <param name="nOffset">Specifies offset within the GPU memory from where the copy is to start.</param>
        public void SetMemoryAt(long hMem, double[] rgSrc, int nOffset)
        {
            SetMemoryAt(hMem, convert(rgSrc), nOffset);
        }

        /// <summary>
        /// Copies an array of float into a block of already allocated GPU memory starting at a specific offset.
        /// </summary>
        /// <remarks>This function converts the input array into the base type 'T' for which the instance of CudaDnn was defined.</remarks>
        /// <param name="hMem">Specifies the handle to the GPU memory.</param>
        /// <param name="rgSrc">Specifies the array of float to copy.</param>
        /// <param name="nOffset">Specifies offset within the GPU memory from where the copy is to start.</param>
        public void SetMemoryAt(long hMem, float[] rgSrc, int nOffset)
        {
            SetMemoryAt(hMem, convert(rgSrc), nOffset);
        }

        /// <summary>
        /// Copies an array of type 'T' into a block of already allocated GPU memory starting at a specific offset.
        /// </summary>
        /// <param name="hMem">Specifies the handle to the GPU memory.</param>
        /// <param name="rgSrc">Specifies the array of type 'T' to copy.</param>
        /// <param name="nOffset">Specifies offset within the GPU memory from where the copy is to start.</param>
        public void SetMemoryAt(long hMem, T[] rgSrc, int nOffset)
        {
            if (rgSrc == null || rgSrc.Length == 0)
                throw new ArgumentOutOfRangeException("There are no data items to set!");

            if (m_hKernel > 0)
            {
                if (m_rgGhostMemory != null)
                    throw new Exception("Ghost memory does not support SetMemoryAt.");

                if (m_dt == DataType.DOUBLE)
                {
                    int nDataCount = 3 + rgSrc.Length;
                    double[] rg = new double[nDataCount];

                    rg[0] = hMem;
                    rg[1] = rgSrc.Length;
                    rg[2] = nOffset;

                    long[] rgIn = new long[] { hMem, rgSrc.Length, nOffset };

                    convertD(rgSrc, rg, 3);
                    m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SETMEMAT, rg, rgIn);
                }
                else
                {
                    int nDataCount = 3 + rgSrc.Length;
                    float[] rg = new float[nDataCount];

                    rg[0] = hMem;
                    rg[1] = rgSrc.Length;
                    rg[2] = nOffset;

                    long[] rgIn = new long[] { hMem, rgSrc.Length, nOffset };

                    convertF(rgSrc, rg, 3);
                    m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SETMEMAT, rg, rgIn);
                }
            }
        }

        /// <summary>
        /// Set a pixel value where each pixel is defined a set index, value tuple.
        /// </summary>
        /// <param name="hMem">Specifies the memory where the values are set.</param>
        /// <param name="nCount">Specifies the number of allocated items in the memory.</param>
        /// <param name="bReturnOriginal">Specifies whether or not to return the original values (before setting).</param>
        /// <param name="nOffset">Specifies the offset of where the first pixel data starts.</param>
        /// <param name="rgPixel">Specifies the pixel values.</param>
        /// <returns>When 'bReturnOriginal' is True, the original values (before setting) are returned.</returns>
        public T[] SetPixel(long hMem, int nCount, bool bReturnOriginal, int nOffset, params Tuple<int, T>[] rgPixel)
        {
            if (rgPixel.Length == 0)
                throw new Exception("You must specify at least one pixel!");

            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = new double[5 + rgPixel.Length * 2];

                rg[0] = hMem;
                rg[1] = nCount;
                rg[2] = (bReturnOriginal) ? 1 : 0;
                rg[3] = nOffset;
                rg[4] = rgPixel.Length;
                int nIdx = 5;

                for (int i = 0; i < rgPixel.Length; i++)
                {
                    rg[nIdx] = rgPixel[i].Item1;
                    nIdx++;
                    rg[nIdx] = convertD1(rgPixel[i].Item2);
                    nIdx++;
                }

                rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.SETPIXEL, rg);
                if (rg == null)
                    return null;

                return convert(rg);
            }
            else
            {
                float[] rg = new float[5 + rgPixel.Length * 2];

                rg[0] = hMem;
                rg[1] = nCount;
                rg[2] = (bReturnOriginal) ? 1 : 0;
                rg[3] = nOffset;
                rg[4] = rgPixel.Length;
                int nIdx = 5;

                for (int i = 0; i < rgPixel.Length; i++)
                {
                    rg[nIdx] = rgPixel[i].Item1;
                    nIdx++;
                    rg[nIdx] = convertF1(rgPixel[i].Item2);
                    nIdx++;
                }

                rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.SETPIXEL, rg);
                if (rg == null)
                    return null;

                return convert(rg);
            }
        }

        /// <summary>
        /// Copies an array of type 'T' into a block of already allocated host memory.
        /// </summary>
        /// <param name="hMem">Specifies the handle to the host memory.</param>
        /// <param name="rgSrc">Specifies the array of type 'T' to copy.</param>
        public void SetHostMemory(long hMem, T[] rgSrc)
        {
            if (m_dt == DataType.DOUBLE)
            {
                int nDataCount = 2 + rgSrc.Length;
                double[] rg = new double[nDataCount];

                rg[0] = hMem;
                rg[1] = rgSrc.Length;

                convertD(rgSrc, rg, 2);
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SETHOSTMEM, rg, m_param.AsLong(hMem, rgSrc.Length));
            }
            else
            {
                int nDataCount = 2 + rgSrc.Length;
                float[] rg = new float[nDataCount];

                rg[0] = hMem;
                rg[1] = rgSrc.Length;

                convertF(rgSrc, rg, 2);
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SETHOSTMEM, rg, m_param.AsLong(hMem, rgSrc.Length));
            }
        }

        /// <summary>
        /// Creates a memory pointer into an already existing block of GPU memory.
        /// </summary>
        /// <param name="hData">Specifies a handle to the GPU memory.</param>
        /// <param name="lOffset">Specifies the offset into the GPU memory (in items, not bytes), where the pointer is to start.</param>
        /// <param name="lCount">Specifies the number of items (not bytes) in the 'virtual' memory block pointed to by the memory pointer.</param>
        /// <returns>A handle to the memory pointer is returned.  Handles to memory poitners can be used like any other handle to GPU memory.</returns>
        public long CreateMemoryPointer(long hData, long lOffset, long lCount)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CREATE_MEMORYPOINTER, null, m_param.AsLong(hData, lOffset, lCount));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CREATE_MEMORYPOINTER, null, m_param.AsLong(hData, lOffset, lCount));
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Frees a memory pointer.
        /// </summary>
        /// <param name="hData">Specifies the handle to the memory pointer.</param>
        public void FreeMemoryPointer(long hData)
        {
            if (m_cuda == null || m_hKernel <= 0)
            {
                Trace.WriteLine("WARNING: CudaDnn has already been disposed, cannot free memory pointer.");
                return;
            }

            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.FREE_MEMORYPOINTER, null, m_param.AsLong(hData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.FREE_MEMORYPOINTER, null, m_param.AsLong(hData));
        }

        /// <summary>
        /// Creates a new memory test on the current GPU.
        /// </summary>
        /// <param name="ulTotalNumBlocks">Returns the total number of blocks available to test.</param>
        /// <param name="dfMemAllocatedInGB">Returns the total amount of allocated memory, specified in GB.</param>
        /// <param name="ulMemStartAddr">Returns the start address of the memory test.</param>
        /// <param name="ulBlockSize">Returns the block size of the memory to be tested.</param>
        /// <param name="dfPctToAllocate">Specifies the percentage of avaiable memory to test, where 1.0 = 100%.</param>
        /// <returns>A handle to the memory test is returned.</returns>
        public long CreateMemoryTest(out ulong ulTotalNumBlocks, out double dfMemAllocatedInGB, out ulong ulMemStartAddr, out ulong ulBlockSize, double dfPctToAllocate = 1.0)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CREATE_MEMTEST, m_param.AsDouble(dfPctToAllocate));
                ulTotalNumBlocks = (ulong)rg[1];
                dfMemAllocatedInGB = (double)rg[2];
                ulMemStartAddr = (ulong)rg[3];
                ulBlockSize = (ulong)rg[4];
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CREATE_MEMTEST, m_param.AsFloat((float)dfPctToAllocate));
                ulTotalNumBlocks = (ulong)rg[1];
                dfMemAllocatedInGB = (double)rg[2];
                ulMemStartAddr = (ulong)rg[3];
                ulBlockSize = (ulong)rg[4];
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free a memory test, freeing up all GPU memory used.
        /// </summary>
        /// <param name="h">Specifies the handle to the memory test.</param>
        public void FreeMemoryTest(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_MEMTEST, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_MEMTEST, m_param.AsFloat(h));
        }

        /// <summary>
        /// The RunMemoryTest method runs the memory test from the block start offset through the block count on the
        /// memory previously allocated using CreateMemoryTest.
        /// </summary>
        /// <param name="h">Specifies the handle to the memory test data.</param>
        /// <param name="type">Specifies the type of memory test to run.</param>
        /// <param name="ulBlockStartOffset">Specifies the block start offset (offset into the total blocks returned
        /// by CreateMemoryTest).</param>
        /// <param name="ulBlockCount">Specifies the number of blocks to test.</param>
        /// <param name="bVerbose">When disabled, the memory test is just run once and the number of errors is returned.
        /// When eanbled, the memory test is run twice and the erroring adresses are returned along with the error count.</param>
        /// <returns>The format of the array returned is as follows:
        /// rg[0] - specifies the starting memory address used for this memory test run.
        /// rg[1] - specifies the number of addresses over which the test was run (specified in 1 byte increments).
        /// rg[2] - specifies the number of errors found.
        /// rg[3, ...] - specifies the erroring addresses (specified in 1-bit increments)
        /// <param name="bWrite">Specifies to perform a write test.</param>
        /// <param name="bReadWrite">Specifies to perform a read/write test.</param>
        /// <param name="bRead">Specifies to peroform a read test.</param>
        /// </returns>
        public T[] RunMemoryTest(long h, MEMTEST_TYPE type, ulong ulBlockStartOffset, ulong ulBlockCount, bool bVerbose, bool bWrite, bool bReadWrite, bool bRead)
        {
            List<ulong> rgErrorAddresses = new List<ulong>();

            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.RUN_MEMTEST, null, m_param.AsLong(h, (long)type, (long)ulBlockStartOffset, (long)ulBlockCount, (bVerbose) ? 1 : 0, (bWrite) ? 1 : 0, (bReadWrite) ? 1 : 0, (bRead) ? 1 : 0));
                return (T[])Convert.ChangeType(rg, typeof(T[]));
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.RUN_MEMTEST, null, m_param.AsLong(h, (long)type, (long)ulBlockStartOffset, (long)ulBlockCount, (bVerbose) ? 1 : 0, (bWrite) ? 1 : 0, (bReadWrite) ? 1 : 0, (bRead) ? 1 : 0));
                return (T[])Convert.ChangeType(rg, typeof(T[]));
            }
        }

        /// <summary>
        /// Create a new ImageOp used to perform image operations on the GPU.
        /// </summary>
        /// <param name="nNum">Specifies the number of items (usually the blob.num).</param>
        /// <param name="dfBrightnessProb">Specifies the brightness probability [0,1].</param>
        /// <param name="dfBrightnessDelta">Specifies the brightness delta.</param>
        /// <param name="dfContrastProb">Specifies the contrast probability [0,1]</param>
        /// <param name="dfContrastLower">Specifies the contrast lower bound value.</param>
        /// <param name="dfContrastUpper">Specifies the contrast upper bound value.</param>
        /// <param name="dfSaturationProb">Specifies the saturation probability [0,1]</param>
        /// <param name="dfSaturationLower">Specifies the saturation lower bound value.</param>
        /// <param name="dfSaturationUpper">Specifies the saturation upper bound value.</param>
        /// <param name="lRandomSeed">Optionally, specifies the random seed or 0 to ignore (default = 0).</param>
        /// <returns>A handle to the ImageOp is returned.</returns>
        public long CreateImageOp(int nNum, double dfBrightnessProb, double dfBrightnessDelta, double dfContrastProb, double dfContrastLower, double dfContrastUpper, double dfSaturationProb, double dfSaturationLower, double dfSaturationUpper, long lRandomSeed = 0)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CREATE_IMAGEOP, m_param.AsDouble(nNum, dfBrightnessProb, dfBrightnessDelta, dfContrastProb, dfContrastLower, dfContrastUpper, dfSaturationProb, dfSaturationLower, dfSaturationUpper, lRandomSeed));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CREATE_IMAGEOP, m_param.AsFloat(nNum, (float)dfBrightnessProb, (float)dfBrightnessDelta, (float)dfContrastProb, (float)dfContrastLower, (float)dfContrastUpper, (float)dfSaturationProb, (float)dfSaturationLower, (float)dfSaturationUpper, lRandomSeed));
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free an image op, freeing up all GPU memory used.
        /// </summary>
        /// <param name="h">Specifies the handle to the image op.</param>
        public void FreeImageOp(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_IMAGEOP, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_IMAGEOP, m_param.AsFloat(h));
        }

        /// <summary>
        /// Distort an image using the ImageOp handle provided.
        /// </summary>
        /// <param name="h">Specifies a handle to the ImageOp that defines how the image will be distorted.</param>
        /// <param name="nCount">Specifies the total number of data elements within 'hX' and 'hY'.</param>
        /// <param name="nNum">Specifies the number of items to be distorted (typically blob.num) in 'hX' and 'hY'.</param>
        /// <param name="nDim">Specifies the dimension of each item.</param>
        /// <param name="hX">Specifies a handle to the GPU memory containing the source data to be distorted.</param>
        /// <param name="hY">Specifies a handle to the GPU memory containing the destination of the distortion.</param>
        public void DistortImage(long h, int nCount, int nNum, int nDim, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.DISTORTIMAGE_IMAGEOP, null, m_param.AsLong(h, nCount, nNum, nDim, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.DISTORTIMAGE_IMAGEOP, null, m_param.AsLong(h, nCount, nNum, nDim, hX, hY));
        }

        #endregion

        //---------------------------------------------------------------------
        //  ICudaDnn Methods
        //---------------------------------------------------------------------
        #region ICudaDnn Methods

        /// <summary>
        /// Create a new stream on the current GPU.
        /// </summary>
        /// <param name="bNonBlocking">When <code>false</code> (the default) the created stream is a 'blocking' stream, otherwise it is an asynchronous, non-blocking stream.</param>
        /// <param name="nIndex">Specifies an index for the stream where indexed streams are shared when the index = 0 or greater.</param>
        /// <returns>The handle to the stream is returned.</returns>
        public long CreateStream(bool bNonBlocking = false, int nIndex = -1)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CREATE_STREAM, m_param.AsDouble((bNonBlocking) ? 1 : 0, nIndex));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CREATE_STREAM, m_param.AsFloat((bNonBlocking) ? 1 : 0, nIndex));
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free a stream.
        /// </summary>
        /// <param name="h">Specifies the handle to the stream.</param>
        public void FreeStream(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_STREAM, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_STREAM, m_param.AsFloat(h));
        }

        /// <summary>
        /// Synchronize a stream on the current GPU, waiting for its operations to complete.
        /// </summary>
        /// <param name="h">Specifies the handle to the stream.</param>
        public void SynchronizeStream(long h = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.SYNCRHONIZE_STREAM, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.SYNCRHONIZE_STREAM, m_param.AsFloat(h));
        }

        /// <summary>
        /// Synchronize all kernel threads on the current GPU.
        /// </summary>
        public void SynchronizeThread()
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.SYNCHRONIZE_THREAD, null);
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.SYNCHRONIZE_THREAD, null);
        }

        /// <summary>
        /// Create a new instance of [NVIDIA's cuDnn](https://developer.nvidia.com/cudnn).
        /// </summary>
        /// <param name="hStream">Specifies a stream used by cuDnn.</param>
        /// <returns>The handle to cuDnn is returned.</returns>
        public long CreateCuDNN(long hStream = 0)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CREATE_CUDNN, m_param.AsDouble(hStream));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CREATE_CUDNN, m_param.AsFloat(hStream));
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free an instance of cuDnn.
        /// </summary>
        /// <param name="h">Specifies the handle to cuDnn.</param>
        public void FreeCuDNN(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_CUDNN, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_CUDNN, m_param.AsFloat(h));
        }

        /// <summary>
        /// Create an instance of [NVIDIA's NCCL 'Nickel'](https://devblogs.nvidia.com/parallelforall/fast-multi-gpu-collectives-nccl/)
        /// </summary>
        /// <param name="nDeviceId">Specifies the device where this instance of NCCL is going to run.</param>
        /// <param name="nCount">Specifies the total number of NCCL instances used.</param>
        /// <param name="nRank">Specifies the zero-based rank of this instance of NCCL.</param>
        /// <param name="guid">Specifies the unique Guid for this isntance of NCCL.</param>
        /// <returns>The handle to a new instance of NCCL is returned.</returns>
        public long CreateNCCL(int nDeviceId, int nCount, int nRank, Guid guid)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<double> rgParam = new List<double>() { nDeviceId, nCount, nRank };
                List<double> rgGuid = guidToArrayDouble(guid);

                rgParam.Add(rgGuid.Count);
                rgParam.AddRange(rgGuid);

                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CREATE_NCCL, rgParam.ToArray());
                return (long)rg[0];
            }
            else
            {
                List<float> rgParam = new List<float>() { nDeviceId, nCount, nRank };
                List<float> rgGuid = guidToArrayFloat(guid);

                rgParam.Add(rgGuid.Count);
                rgParam.AddRange(rgGuid);

                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CREATE_NCCL, rgParam.ToArray());
                return (long)rg[0];
            }
        }

        private List<double> guidToArrayDouble(Guid guid)
        {
            List<double> rgdf = new List<double>();
            string str = guid.ToString();
            string[] rgstr = str.Split('-');

            foreach (string str1 in rgstr)
            {
                long val = Convert.ToInt64(str1, 16);
                rgdf.Add(val);
            }

            return rgdf;
        }

        private List<float> guidToArrayFloat(Guid guid)
        {
            List<double> rgDf = guidToArrayDouble(guid);
            List<float> rg = new List<float>();

            foreach (double df in rgDf)
            {
                rg.Add((float)df);
            }

            return rg;
        }

        /// <summary>
        /// Free an instance of NCCL.
        /// </summary>
        /// <param name="hNccl">Specifies the handle to NCCL.</param>
        /// <param name="bDetachOnly">Optionally, specifies to detach the NCCL only and do not delete it (used when sharing NCCL between threads).</param>
        public void FreeNCCL(long hNccl, bool bDetachOnly = false)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_NCCL, m_param.AsDouble(hNccl, (bDetachOnly) ? 1 : 0));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_NCCL, m_param.AsFloat(hNccl, (bDetachOnly) ? 1 : 0));
        }

        /// <summary>
        /// Initializes a set of NCCL instances for use in a single process.
        /// </summary>
        /// <remarks>
        /// See [Fast Multi-GPU collectives with NCCL](https://devblogs.nvidia.com/parallelforall/fast-multi-gpu-collectives-nccl/).
        /// </remarks>
        /// <param name="rghNccl">Specifies the array of NCCL handles that will be working together.</param>
        public void NcclInitializeSingleProcess(params long[] rghNccl)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<double> rg = new List<double>() { 0, rghNccl.Length };

                for (int i = 0; i < rghNccl.Length; i++)
                {
                    rg.Add(rghNccl[i]);
                }

                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.NCCL_INIT_SINGLEPROCESS, rg.ToArray());
            }
            else
            {
                List<float> rg = new List<float>() { 0, rghNccl.Length };

                for (int i = 0; i < rghNccl.Length; i++)
                {
                    rg.Add(rghNccl[i]);
                }

                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.NCCL_INIT_SINGLEPROCESS, rg.ToArray());
            }
        }

        /// <summary>
        /// Initializes a set of NCCL instances for use in different processes.
        /// </summary>
        /// <remarks>
        /// See [Fast Multi-GPU collectives with NCCL](https://devblogs.nvidia.com/parallelforall/fast-multi-gpu-collectives-nccl/).
        /// </remarks>
        /// <param name="hNccl">Specifies the handle of NCCL to initialize.</param>
        public void NcclInitializeMultiProcess(long hNccl)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.NCCL_INIT_MULTIPROCESS, m_param.AsDouble(hNccl));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.NCCL_INIT_MULTIPROCESS, m_param.AsFloat(hNccl));
        }

        /// <summary>
        /// Broadcasts a block of GPU data to all NCCL instances.
        /// </summary>
        /// <remarks>
        /// See [Fast Multi-GPU collectives with NCCL](https://devblogs.nvidia.com/parallelforall/fast-multi-gpu-collectives-nccl/).
        /// </remarks>
        /// <param name="hNccl">Specifies a handle to an NCCL instance.</param>
        /// <param name="hStream">Specifies a handle to the stream to use for synchronization.</param>
        /// <param name="hX">Specifies a handle to the GPU data to be broadcasted (or recieved).</param>
        /// <param name="nCount">Specifies the number of items (not bytes) in the data.</param>
        public void NcclBroadcast(long hNccl, long hStream, long hX, int nCount)
        {
            Trace.WriteLine("Broadcasting from device ID " + GetDeviceID().ToString());
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.NCCL_BROADCAST, null, m_param.AsLong(hNccl, hStream, hX, nCount));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.NCCL_BROADCAST, null, m_param.AsLong(hNccl, hStream, hX, nCount));
        }

        /// <summary>
        /// Performs a reduction on all NCCL instances as specified by the reduction operation.
        /// </summary>
        /// <remarks>
        /// See [Fast Multi-GPU collectives with NCCL](https://devblogs.nvidia.com/parallelforall/fast-multi-gpu-collectives-nccl/).
        /// </remarks>
        /// <param name="hNccl">Specifies a handle to an NCCL instance.</param>
        /// <param name="hStream">Specifies a handle to the stream to use for synchronization.</param>
        /// <param name="hX">Specifies a handle to the GPU data to reduce with the other instances of NCCL.</param>
        /// <param name="nCount">Specifies the number of items (not bytes) in the data.</param>
        /// <param name="op">Specifies the reduction operation to perform.</param>
        /// <param name="dfScale">Optionally, specifies a scaling to be applied to the final reduction.</param>
        public void NcclAllReduce(long hNccl, long hStream, long hX, int nCount, NCCL_REDUCTION_OP op, double dfScale = 1.0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.NCCL_ALLREDUCE, m_param.AsDouble(dfScale), m_param.AsLong(hNccl, hStream, hX, nCount, (int)op, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.NCCL_ALLREDUCE, m_param.AsFloat((float)dfScale), m_param.AsLong(hNccl, hStream, hX, nCount, (int)op, 0));
        }


        /// <summary>
        /// Create an instance of an Extension DLL.
        /// </summary>
        /// <param name="strExtensionDllPath">Specifies the file path to the extension DLL.</param>
        /// <returns>The handle to a new instance of Extension is returned.</returns>
        public long CreateExtension(string strExtensionDllPath)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx((int)m_hKernel, (int)CUDAFN.CREATE_EXTENSION, null, strExtensionDllPath);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx((int)m_hKernel, (int)CUDAFN.CREATE_EXTENSION, null, strExtensionDllPath);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free an instance of an Extension.
        /// </summary>
        /// <param name="hExtension">Specifies the handle to the Extension.</param>
        public void FreeExtension(long hExtension)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_EXTENSION, m_param.AsDouble(hExtension));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_EXTENSION, m_param.AsFloat(hExtension));
        }

        /// <summary>
        /// Run a function on the extension specified.
        /// </summary>
        /// <param name="hExtension">Specifies the handle to the extension created with CreateExtension.</param>
        /// <param name="lfnIdx">Specifies the extension function to run.</param>
        /// <param name="rgParam">Specifies the parameters to pass to the extension.</param>
        /// <returns>The values returned by the extension are returned.</returns>
        public T[] RunExtension(long hExtension, long lfnIdx, T[] rgParam)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<double> rgdf = new List<double>() { hExtension, lfnIdx };

                if (rgParam != null)
                    rgdf.AddRange(Utility.ConvertVec<T>(rgParam));

                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.EXTENSION_RUN, rgdf.ToArray());
                return Utility.ConvertVec<T>(rg);
            }
            else
            {
                List<float> rgf = new List<float>() { hExtension, lfnIdx };

                if (rgParam != null)
                    rgf.AddRange(Utility.ConvertVecF<T>(rgParam));

                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.EXTENSION_RUN, rgf.ToArray());
                return Utility.ConvertVec<T>(rg);
            }
        }


        /// <summary>
        /// Run a function on the extension specified.
        /// </summary>
        /// <param name="hExtension">Specifies the handle to the extension created with CreateExtension.</param>
        /// <param name="lfnIdx">Specifies the extension function to run.</param>
        /// <param name="rgParam">Specifies the parameters to pass to the extension.</param>
        /// <param name="strInput">Specifies the string input.</param>
        /// <returns>The values returned by the extension are returned.</returns>
        public T[] RunExtensionEx(long hExtension, long lfnIdx, T[] rgParam, string strInput)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<double> rgdf = new List<double>() { hExtension, lfnIdx };

                if (rgParam != null)
                    rgdf.AddRange(Utility.ConvertVec<T>(rgParam));

                double[] rg = m_cuda.RunDoubleEx((int)m_hKernel, (int)CUDAFN.EXTENSION_RUN, rgdf.ToArray(), strInput);
                return Utility.ConvertVec<T>(rg);
            }
            else
            {
                List<float> rgf = new List<float>() { hExtension, lfnIdx };

                if (rgParam != null)
                    rgf.AddRange(Utility.ConvertVecF<T>(rgParam));

                float[] rg = m_cuda.RunFloatEx((int)m_hKernel, (int)CUDAFN.EXTENSION_RUN, rgf.ToArray(), strInput);
                return Utility.ConvertVec<T>(rg);
            }
        }

        /// <summary>
        /// Query a string result from the extension specified.
        /// </summary>
        /// <param name="hExtension">Specifies the handle to the extension created with CreateExtension.</param>
        /// <param name="lfnIdx">Specifies the extension function to run.</param>
        /// <param name="rgParam">Specifies the parameters to pass to the extension.</param>
        /// <returns>The values returned by the extension are returned.</returns>
        public string[] QueryExtensionStrings(long hExtension, long lfnIdx, int[] rgParam)
        {
            if (m_cuda == null || m_hKernel <= 0)
                throw new Exception("CudaDnn has already been disposed!");

            List<int> rgParam1 = new List<int>() { (int)hExtension, (int)lfnIdx };
            rgParam1.AddRange(rgParam);

            if (m_dt == DataType.DOUBLE)
                return m_cuda.QueryStringDoubleEx((int)m_hKernel, (int)CUDAQRY.EXTENSION_STRING, rgParam1.ToArray());
            else
                return m_cuda.QueryStringFloatEx((int)m_hKernel, (int)CUDAQRY.EXTENSION_STRING, rgParam1.ToArray());
        }


        /// <summary>
        /// Create a new instance of a tensor descriptor for use with [NVIDIA's cuDnn](https://developer.nvidia.com/cudnn).
        /// </summary>
        /// <returns>The tensor descriptor handle is returned.</returns>
        public long CreateTensorDesc()
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CREATE_TENSORDESC, null);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CREATE_TENSORDESC, null);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free a tensor descriptor instance.
        /// </summary>
        /// <param name="h">Specifies the handle to the tensor descriptor instance.</param>
        public void FreeTensorDesc(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_TENSORDESC, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_TENSORDESC, m_param.AsFloat(h));
        }

        /// <summary>
        /// Sets the values of a tensor descriptor.
        /// </summary>
        /// <param name="hHandle">Specifies the handle to the tensor descriptor.</param>
        /// <param name="rgDim">Specifies the dimensions of the data.</param>
        /// <param name="rgStride">Specifies the stride of the data.</param>
        /// <param name="bHalf">Optionally, specifies whether or not to use the FP16 half data type.</param>
        public void SetTensorNdDesc(long hHandle, int[] rgDim, int[] rgStride, bool bHalf = false)
        {
            if (rgDim.Length != rgStride.Length)
                throw new Exception("The stride and dim arrays must have the same length.");

            if (m_dt == DataType.DOUBLE)
            {
                List<long> rgArg = new List<long>() { hHandle, (bHalf) ? 1 : 0, rgDim.Length };

                for (int i = 0; i < rgDim.Length; i++)
                {
                    rgArg.Add(rgDim[i]);
                }

                for (int i = 0; i < rgStride.Length; i++)
                {
                    rgArg.Add(rgStride[i]);
                }

                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SET_TENSORNDDESC, null, rgArg.ToArray());
            }
            else
            {
                List<long> rgArg = new List<long>() { hHandle, (bHalf) ? 1 : 0, rgDim.Length };

                for (int i = 0; i < rgDim.Length; i++)
                {
                    rgArg.Add(rgDim[i]);
                }

                for (int i = 0; i < rgStride.Length; i++)
                {
                    rgArg.Add(rgStride[i]);
                }

                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SET_TENSORNDDESC, null, rgArg.ToArray());
            }
        }

        /// <summary>
        /// Sets the values of a tensor descriptor.
        /// </summary>
        /// <param name="hHandle">Specifies the handle to the tensor descriptor.</param>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="c">Specifies the number of channels in each item.</param>
        /// <param name="h">Specifies the height of each item.</param>
        /// <param name="w">Specifies the width of each item.</param>
        /// <param name="bHalf">Optionally, specifies whether or not to use the FP16 half data type.</param>
        public void SetTensorDesc(long hHandle, int n, int c, int h, int w, bool bHalf = false)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SET_TENSORDESC, null, m_param.AsLong(hHandle, (bHalf) ? 1 : 0, n, c, h, w));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SET_TENSORDESC, null, m_param.AsLong(hHandle, (bHalf) ? 1 : 0, n, c, h, w));
        }

        /// <summary>
        /// Sets the values of a tensor descriptor.
        /// </summary>
        /// <param name="hHandle">Specifies the handle to the tensor descriptor.</param>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="c">Specifies the number of channels in each item.</param>
        /// <param name="h">Specifies the height of each item.</param>
        /// <param name="w">Specifies the width of each item.</param>
        /// <param name="nStride">Specifies the stride between two images.</param>
        /// <param name="cStride">Specifies the stride between two channels.</param>
        /// <param name="hStride">Specifies the stride between two rows.</param>
        /// <param name="wStride">Specifies the stride between two columns.</param>
        /// <param name="bHalf">Optionally, specifies whether or not to use the FP16 half data type.</param>
        public void SetTensorDesc(long hHandle, int n, int c, int h, int w, int nStride, int cStride, int hStride, int wStride, bool bHalf = false)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SET_TENSORDESC, null, m_param.AsLong(hHandle, (bHalf) ? 1 : 0, n, c, h, w, nStride, cStride, hStride, wStride));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SET_TENSORDESC, null, m_param.AsLong(hHandle, (bHalf) ? 1 : 0, n, c, h, w, nStride, cStride, hStride, wStride));
        }

        /// <summary>
        /// Add two tensors together.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the cuDnn instance.</param>
        /// <param name="hSrcDesc">Specifies a handle to the source tensor descriptor.</param>
        /// <param name="hSrc">Specifies a handle to the source GPU memory.</param>
        /// <param name="nSrcOffset">Specifies an offset within the GPU memory.</param>
        /// <param name="hDstDesc">Specifies a handle to the destination tensor descriptor.</param>
        /// <param name="hDst">Specifies a handle to the desination GPU memory.</param>
        /// <param name="nDstOffset">Specifies an offset within the GPU memory.</param>
        public void AddTensor(long hCuDnn, long hSrcDesc, long hSrc, int nSrcOffset, long hDstDesc, long hDst, int nDstOffset)
        {
            AddTensor(hCuDnn, m_tOne, hSrcDesc, hSrc, nSrcOffset, m_tOne, hDstDesc, hDst, nDstOffset);
        }

        /// <summary>
        /// Add two tensors together.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the cuDnn instance.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the source GPU memory before the add.</param>
        /// <param name="hSrcDesc">Specifies a handle to the source tensor descriptor.</param>
        /// <param name="hSrc">Specifies a handle to the source GPU memory.</param>
        /// <param name="nSrcOffset">Specifies an offset within the GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the destination GPU memory before the add.</param>
        /// <param name="hDstDesc">Specifies a handle to the destination tensor descriptor.</param>
        /// <param name="hDst">Specifies a handle to the desination GPU memory.</param>
        /// <param name="nDstOffset">Specifies an offset within the GPU memory.</param>
        public void AddTensor(long hCuDnn, T fAlpha, long hSrcDesc, long hSrc, int nSrcOffset, T fBeta, long hDstDesc, long hDst, int nDstOffset)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.ADD_TENSOR, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hSrcDesc, hSrc, nSrcOffset, 0, hDstDesc, hDst, nDstOffset));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.ADD_TENSOR, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hSrcDesc, hSrc, nSrcOffset, 0, hDstDesc, hDst, nDstOffset));
        }


        /// <summary>
        /// Create a new instance of a filter descriptor for use with [NVIDIA's cuDnn](https://developer.nvidia.com/cudnn).
        /// </summary>
        /// <returns>The filter descriptor handle is returned.</returns>
        public long CreateFilterDesc()
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CREATE_FILTERDESC, null);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CREATE_FILTERDESC, null);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free a filter descriptor instance.
        /// </summary>
        /// <param name="h">Specifies the handle to the filter descriptor instance.</param>
        public void FreeFilterDesc(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_FILTERDESC, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_FILTERDESC, m_param.AsFloat(h));
        }

        /// <summary>
        /// Sets the values of a filter descriptor.
        /// </summary>
        /// <param name="hHandle">Specifies the handle to the filter descriptor.</param>
        /// <param name="rgDim">Specifies the dimensions of the data.</param>
        /// <param name="bHalf">Optionally, specifies whether or not to use the FP16 half data type.</param>
        public void SetFilterNdDesc(long hHandle, int[] rgDim, bool bHalf = false)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<long> rgArg = new List<long>() { hHandle, (bHalf) ? 1 : 0, rgDim.Length };

                for (int i = 0; i < rgDim.Length; i++)
                {
                    rgArg.Add(rgDim[i]);
                }

                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SET_FILTERNDDESC, null, rgArg.ToArray());
            }
            else
            {
                List<long> rgArg = new List<long>() { hHandle, (bHalf) ? 1 : 0, rgDim.Length };

                for (int i = 0; i < rgDim.Length; i++)
                {
                    rgArg.Add(rgDim[i]);
                }

                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SET_FILTERNDDESC, null, rgArg.ToArray());
            }
        }

        /// <summary>
        /// Sets the values of a filter descriptor.
        /// </summary>
        /// <param name="hHandle">Specifies the handle to the filter descriptor.</param>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="c">Specifies the number of channels in each item.</param>
        /// <param name="h">Specifies the height of each item.</param>
        /// <param name="w">Specifies the width of each item.</param>
        /// <param name="bHalf">Optionally, specifies whether or not to use the FP16 half data type.</param>
        public void SetFilterDesc(long hHandle, int n, int c, int h, int w, bool bHalf = false)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SET_FILTERDESC, null, m_param.AsLong(hHandle, (bHalf) ? 1 : 0, n, c, h, w));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SET_FILTERDESC, null, m_param.AsLong(hHandle, (bHalf) ? 1 : 0, n, c, h, w));
        }

        /// <summary>
        /// Create a new instance of a convolution descriptor for use with [NVIDIA's cuDnn](https://developer.nvidia.com/cudnn).
        /// </summary>
        /// <returns>The convolution descriptor handle is returned.</returns>
        public long CreateConvolutionDesc()
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CREATE_CONVDESC, null);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CREATE_CONVDESC, null);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free a convolution descriptor instance.
        /// </summary>
        /// <param name="h">Specifies the handle to the convolution descriptor instance.</param>
        public void FreeConvolutionDesc(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_CONVDESC, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_CONVDESC, m_param.AsFloat(h));
        }

        /// <summary>
        /// Set the values of a convolution descriptor.
        /// </summary>
        /// <param name="hHandle">Specifies the handle to the convolution descriptor.</param>
        /// <param name="hPad">Specifies the pad applied to the height.</param>
        /// <param name="wPad">Specifies the pad applied to the width.</param>
        /// <param name="hStride">Specifies the stride of the height.</param>
        /// <param name="wStride">Specifies the stride of the width.</param>
        /// <param name="hDilation">Specifies the dilation of the height (default = 1).</param>
        /// <param name="wDilation">Specifies the dilation of the width (default = 1).</param>
        /// <param name="bUseTensorCores">Optionally, specifies whether or not to use the Tensor Cores (if available).</param>
        /// <param name="bHalf">Optionally, specifies whether or not to use the FP16 half data type.</param>
        public void SetConvolutionDesc(long hHandle, int hPad, int wPad, int hStride, int wStride, int hDilation, int wDilation, bool bUseTensorCores, bool bHalf = false)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SET_CONVDESC, null, m_param.AsLong(hHandle, (bHalf) ? 1 : 0, hPad, wPad, hStride, wStride, hDilation, wDilation, (bUseTensorCores) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SET_CONVDESC, null, m_param.AsLong(hHandle, (bHalf) ? 1 : 0, hPad, wPad, hStride, wStride, hDilation, wDilation, (bUseTensorCores) ? 1 : 0));
        }

        /// <summary>
        /// Queryies the algorithms and workspace sizes used for a given convolution descriptor.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hBottomDesc">Specifies a handle to the bottom tensor descriptor.</param>
        /// <param name="hFilterDesc">Specifies a handle to the filter descriptor.</param>
        /// <param name="hConvDesc">Specifies a handle to the convolution descriptor.</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="lWorkspaceSizeLimitInBytes">Specifies the workspace limits (in bytes).</param>
        /// <param name="bUseTensorCores">Specifies whether or not to use tensor cores (this parameter must match the setting of the 'bUseTensorCores' specified in the 'SetConvolutionDesc' method.</param>
        /// <param name="algoFwd">Returns the algorithm used for the convolution foward.</param>
        /// <param name="lWsSizeFwd">Returns the workspace size (in bytes) for the convolution foward.</param>
        /// <param name="algoBwdFilter">Returns the algorithm used for the backward filter.</param>
        /// <param name="lWsSizeBwdFilter">Returns the workspace size (int bytes) for the backward filter.</param>
        /// <param name="algoBwdData">Returns the algorithm for the backward data.</param>
        /// <param name="lWsSizeBwdData">Returns the workspace (in bytes) for the backward data.</param>
        /// <param name="preferredFwdAlgo">Optionally, specifies a preferred forward algo to attempt to use for forward convolution.  The new algo is only used if the current device supports it.</param>
        public void GetConvolutionInfo(long hCuDnn, long hBottomDesc, long hFilterDesc, long hConvDesc, long hTopDesc, ulong lWorkspaceSizeLimitInBytes, bool bUseTensorCores, out CONV_FWD_ALGO algoFwd, out ulong lWsSizeFwd, out CONV_BWD_FILTER_ALGO algoBwdFilter, out ulong lWsSizeBwdFilter, out CONV_BWD_DATA_ALGO algoBwdData, out ulong lWsSizeBwdData, CONV_FWD_ALGO preferredFwdAlgo = CONV_FWD_ALGO.NONE)
        {
            lock (m_getconvSync)
            {
                if (m_dt == DataType.DOUBLE)
                {
                    double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.GET_CONVINFO, null, m_param.AsLong(hCuDnn, hBottomDesc, hFilterDesc, hConvDesc, hTopDesc, (long)lWorkspaceSizeLimitInBytes, (bUseTensorCores) ? 1 : 0, (int)preferredFwdAlgo));
                    algoFwd = (CONV_FWD_ALGO)rg[0];
                    lWsSizeFwd = (ulong)rg[1];
                    algoBwdFilter = (CONV_BWD_FILTER_ALGO)rg[2];
                    lWsSizeBwdFilter = (ulong)rg[3];
                    algoBwdData = (CONV_BWD_DATA_ALGO)rg[4];
                    lWsSizeBwdData = (ulong)rg[5];
                }
                else
                {
                    float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.GET_CONVINFO, null, m_param.AsLong(hCuDnn, hBottomDesc, hFilterDesc, hConvDesc, hTopDesc, (long)lWorkspaceSizeLimitInBytes, (bUseTensorCores) ? 1 : 0, (int)preferredFwdAlgo));
                    algoFwd = (CONV_FWD_ALGO)rg[0];
                    lWsSizeFwd = (ulong)rg[1];
                    algoBwdFilter = (CONV_BWD_FILTER_ALGO)rg[2];
                    lWsSizeBwdFilter = (ulong)rg[3];
                    algoBwdData = (CONV_BWD_DATA_ALGO)rg[4];
                    lWsSizeBwdData = (ulong)rg[5];
                }
            }
        }

        /// <summary>
        /// Perform a convolution forward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hBottomDesc">Specifies a handle to the bottom tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="nBottomOffset">Specifies an offset into the bottom memory (in items, not bytes).</param>
        /// <param name="hFilterDesc">Specifies a handle to the filter descriptor.</param>
        /// <param name="hWeight">Specifies a handle to the weight data in GPU memory.</param>
        /// <param name="nWeightOffset">Specifies an offset into the weight memory (in items, not bytes).</param>
        /// <param name="hConvDesc">Specifies a handle to the convolution descriptor.</param>
        /// <param name="algoFwd">Specifies the algorithm to use for the foward operation.</param>
        /// <param name="hWorkspace">Specifies a handle to the GPU memory to use for the workspace.</param>
        /// <param name="nWorkspaceOffset">Specifies an offset into the workspace memory.</param>
        /// <param name="lWorkspaceSize">Specifies the size of the workspace memory (in bytes).</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="nTopOffset">Specifies an offset into the top memory (in items, not bytes).</param>
        /// <param name="bSyncStream">Optionally, specifies whether or not to syncrhonize the stream. The default = <i>true</i>.</param>
        public void ConvolutionForward(long hCuDnn, long hBottomDesc, long hBottomData, int nBottomOffset, long hFilterDesc, long hWeight, int nWeightOffset, long hConvDesc, CONV_FWD_ALGO algoFwd, long hWorkspace, int nWorkspaceOffset, ulong lWorkspaceSize, long hTopDesc, long hTopData, int nTopOffset, bool bSyncStream = true)
        {
            ConvolutionForward(hCuDnn, m_tOne, hBottomDesc, hBottomData, nBottomOffset, hFilterDesc, hWeight, nWeightOffset, hConvDesc, algoFwd, hWeight, nWeightOffset, lWorkspaceSize, m_tZero, hTopDesc, hTopData, nTopOffset, bSyncStream);
        }

        /// <summary>
        /// Perform a convolution forward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hBottomDesc">Specifies a handle to the bottom tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="nBottomOffset">Specifies an offset into the bottom memory (in items, not bytes).</param>
        /// <param name="hFilterDesc">Specifies a handle to the filter descriptor.</param>
        /// <param name="hWeight">Specifies a handle to the weight data in GPU memory.</param>
        /// <param name="nWeightOffset">Specifies an offset into the weight memory (in items, not bytes).</param>
        /// <param name="hConvDesc">Specifies a handle to the convolution descriptor.</param>
        /// <param name="algoFwd">Specifies the algorithm to use for the foward operation.</param>
        /// <param name="hWorkspace">Specifies a handle to the GPU memory to use for the workspace.</param>
        /// <param name="nWorkspaceOffset">Specifies an offset into the workspace memory.</param>
        /// <param name="lWorkspaceSize">Specifies the size of the workspace memory (in bytes).</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="nTopOffset">Specifies an offset into the top memory (in items, not bytes).</param>
        /// <param name="bSyncStream">Optionally, specifies whether or not to syncrhonize the stream. The default = <i>true</i>.</param>
        public void ConvolutionForward(long hCuDnn, T fAlpha, long hBottomDesc, long hBottomData, int nBottomOffset, long hFilterDesc, long hWeight, int nWeightOffset, long hConvDesc, CONV_FWD_ALGO algoFwd, long hWorkspace, int nWorkspaceOffset, ulong lWorkspaceSize, T fBeta, long hTopDesc, long hTopData, int nTopOffset, bool bSyncStream = true)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.FWD_CONV, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDesc, hBottomData, nBottomOffset, hFilterDesc, hWeight, nWeightOffset, hConvDesc, (long)algoFwd, hWorkspace, nWorkspaceOffset, (long)lWorkspaceSize, 0, hTopDesc, hTopData, nTopOffset, (bSyncStream) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.FWD_CONV, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDesc, hBottomData, nBottomOffset, hFilterDesc, hWeight, nWeightOffset, hConvDesc, (long)algoFwd, hWorkspace, nWorkspaceOffset, (long)lWorkspaceSize, 0, hTopDesc, hTopData, nTopOffset, (bSyncStream) ? 1 : 0));
        }

        /// <summary>
        /// Perform a convolution backward pass on the bias.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nTopOffset">Specifies an offset into the top memory (in items, not bytes).</param>
        /// <param name="hBiasDesc">Specifies a handle to the bias tensor descriptor.</param>
        /// <param name="hBiasDiff">Specifies a handle to the bias diff in GPU memory.</param>
        /// <param name="nBiasOffset">Specifies an offset into the diff memory (in items, not bytes).</param>
        /// <param name="bSyncStream">Optionally, specifies whether or not to syncrhonize the stream. The default = <i>true</i>.</param>
        public void ConvolutionBackwardBias(long hCuDnn, long hTopDesc, long hTopDiff, int nTopOffset, long hBiasDesc, long hBiasDiff, int nBiasOffset, bool bSyncStream = true)
        {
            ConvolutionBackwardBias(hCuDnn, m_tOne, hTopDesc, hTopDiff, nTopOffset, m_tOne, hBiasDesc, hBiasDiff, nBiasOffset, bSyncStream);
        }

        /// <summary>
        /// Perform a convolution backward pass on the bias.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nTopOffset">Specifies an offset into the top memory (in items, not bytes).</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hBiasDesc">Specifies a handle to the bias tensor descriptor.</param>
        /// <param name="hBiasDiff">Specifies a handle to the bias diff in GPU memory.</param>
        /// <param name="nBiasOffset">Specifies an offset into the diff memory (in items, not bytes).</param>
        /// <param name="bSyncStream">Optionally, specifies whether or not to syncrhonize the stream. The default = <i>true</i>.</param>
        public void ConvolutionBackwardBias(long hCuDnn, T fAlpha, long hTopDesc, long hTopDiff, int nTopOffset, T fBeta, long hBiasDesc, long hBiasDiff, int nBiasOffset, bool bSyncStream = true)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.BWD_CONV_BIAS, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hTopDesc, hTopDiff, nTopOffset, 0, hBiasDesc, hBiasDiff, nBiasOffset, (bSyncStream) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.BWD_CONV_BIAS, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hTopDesc, hTopDiff, nTopOffset, 0, hBiasDesc, hBiasDiff, nBiasOffset, (bSyncStream) ? 1 : 0));
        }

        /// <summary>
        /// Perform a convolution backward pass on the filter.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hBottomDesc">Specifies a handle to the bottom tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="nBottomOffset">Specifies an offset into the bottom memory (in items, not bytes).</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nTopOffset">Specifies an offset into the top memory (in items, not bytes).</param>
        /// <param name="hConvDesc">Specifies a handle to the convolution descriptor.</param>
        /// <param name="algoBwd">Specifies the algorithm to use when performing the backward operation.</param>
        /// <param name="hWorkspace">Specifies a handle to the GPU memory to use for the workspace.</param>
        /// <param name="nWorkspaceOffset">Specifies an offset into the workspace memory.</param>
        /// <param name="lWorkspaceSize">Specifies the size of the workspace memory (in bytes).</param>
        /// <param name="hFilterDesc">Specifies a handle to the filter descriptor.</param>
        /// <param name="hWeightDiff">Specifies a handle to the weight diff in GPU memory.</param>
        /// <param name="nWeightOffset">Specifies an offset into the weight memory (in items, not bytes).</param>
        /// <param name="bSyncStream">Optionally, specifies whether or not to syncrhonize the stream. The default = <i>true</i>.</param>
        public void ConvolutionBackwardFilter(long hCuDnn, long hBottomDesc, long hBottomData, int nBottomOffset, long hTopDesc, long hTopDiff, int nTopOffset, long hConvDesc, CONV_BWD_FILTER_ALGO algoBwd, long hWorkspace, int nWorkspaceOffset, ulong lWorkspaceSize, long hFilterDesc, long hWeightDiff, int nWeightOffset, bool bSyncStream)
        {
            ConvolutionBackwardFilter(hCuDnn, m_tOne, hBottomDesc, hBottomData, nBottomOffset, hTopDesc, hTopDiff, nTopOffset, hConvDesc, algoBwd, hWorkspace, nWorkspaceOffset, lWorkspaceSize, m_tOne, hFilterDesc, hWeightDiff, nWeightOffset, bSyncStream);
        }

        /// <summary>
        /// Perform a convolution backward pass on the filter.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hBottomDesc">Specifies a handle to the bottom tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="nBottomOffset">Specifies an offset into the bottom memory (in items, not bytes).</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nTopOffset">Specifies an offset into the top memory (in items, not bytes).</param>
        /// <param name="hConvDesc">Specifies a handle to the convolution descriptor.</param>
        /// <param name="algoBwd">Specifies the algorithm to use when performing the backward operation.</param>
        /// <param name="hWorkspace">Specifies a handle to the GPU memory to use for the workspace.</param>
        /// <param name="nWorkspaceOffset">Specifies an offset into the workspace memory.</param>
        /// <param name="lWorkspaceSize">Specifies the size of the workspace memory (in bytes).</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hFilterDesc">Specifies a handle to the filter descriptor.</param>
        /// <param name="hWeightDiff">Specifies a handle to the weight diff in GPU memory.</param>
        /// <param name="nWeightOffset">Specifies an offset into the weight memory (in items, not bytes).</param>
        /// <param name="bSyncStream">Optionally, specifies whether or not to syncrhonize the stream. The default = <i>true</i>.</param>
        public void ConvolutionBackwardFilter(long hCuDnn, T fAlpha, long hBottomDesc, long hBottomData, int nBottomOffset, long hTopDesc, long hTopDiff, int nTopOffset, long hConvDesc, CONV_BWD_FILTER_ALGO algoBwd, long hWorkspace, int nWorkspaceOffset, ulong lWorkspaceSize, T fBeta, long hFilterDesc, long hWeightDiff, int nWeightOffset, bool bSyncStream = true)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.BWD_CONV_FILTER, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDesc, hBottomData, nBottomOffset, hTopDesc, hTopDiff, nTopOffset, hConvDesc, (long)algoBwd, hWorkspace, nWorkspaceOffset, (long)lWorkspaceSize, 0, hFilterDesc, hWeightDiff, nWeightOffset, (bSyncStream) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.BWD_CONV_FILTER, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDesc, hBottomData, nBottomOffset, hTopDesc, hTopDiff, nTopOffset, hConvDesc, (long)algoBwd, hWorkspace, nWorkspaceOffset, (long)lWorkspaceSize, 0, hFilterDesc, hWeightDiff, nWeightOffset, (bSyncStream) ? 1 : 0));
        }

        /// <summary>
        /// Perform a convolution backward pass on the data.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hFilterDesc">Specifies a handle to the filter descriptor.</param>
        /// <param name="hWeight">Specifies a handle to the weight data in GPU memory.</param>
        /// <param name="nWeightOffset">Specifies an offset into the weight memory (in items, not bytes).</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nTopOffset">Specifies an offset into the top memory (in items, not bytes).</param>
        /// <param name="hConvDesc">Specifies a handle to the convolution descriptor.</param>
        /// <param name="algoBwd">Specifies the algorithm to use when performing the backward operation.</param>
        /// <param name="hWorkspace">Specifies a handle to the GPU memory to use for the workspace.</param>
        /// <param name="nWorkspaceOffset">Specifies an offset into the workspace memory.</param>
        /// <param name="lWorkspaceSize">Specifies the size of the workspace memory (in bytes).</param>
        /// <param name="hBottomDesc">Specifies a handle to the bottom tensor descriptor.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="nBottomOffset">Specifies an offset into the bottom memory (in items, not bytes).</param>
        /// <param name="bSyncStream">Optionally, specifies whether or not to syncrhonize the stream. The default = <i>true</i>.</param>
        public void ConvolutionBackwardData(long hCuDnn, long hFilterDesc, long hWeight, int nWeightOffset, long hTopDesc, long hTopDiff, int nTopOffset, long hConvDesc, CONV_BWD_DATA_ALGO algoBwd, long hWorkspace, int nWorkspaceOffset, ulong lWorkspaceSize, long hBottomDesc, long hBottomDiff, int nBottomOffset, bool bSyncStream = true)
        {
            ConvolutionBackwardData(hCuDnn, m_tOne, hFilterDesc, hWeight, nWeightOffset, hTopDesc, hTopDiff, nTopOffset, hConvDesc, algoBwd, hWorkspace, nWorkspaceOffset, lWorkspaceSize, m_tZero, hBottomDesc, hBottomDiff, nBottomOffset, bSyncStream);
        }

        /// <summary>
        /// Perform a convolution backward pass on the data.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hFilterDesc">Specifies a handle to the filter descriptor.</param>
        /// <param name="hWeight">Specifies a handle to the weight data in GPU memory.</param>
        /// <param name="nWeightOffset">Specifies an offset into the weight memory (in items, not bytes).</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nTopOffset">Specifies an offset into the top memory (in items, not bytes).</param>
        /// <param name="hConvDesc">Specifies a handle to the convolution descriptor.</param>
        /// <param name="algoBwd">Specifies the algorithm to use when performing the backward operation.</param>
        /// <param name="hWorkspace">Specifies a handle to the GPU memory to use for the workspace.</param>
        /// <param name="nWorkspaceOffset">Specifies an offset into the workspace memory.</param>
        /// <param name="lWorkspaceSize">Specifies the size of the workspace memory (in bytes).</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hBottomDesc">Specifies a handle to the bottom tensor descriptor.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="nBottomOffset">Specifies an offset into the bottom memory (in items, not bytes).</param>
        /// <param name="bSyncStream">Optionally, specifies whether or not to syncrhonize the stream. The default = <i>true</i>.</param>
        public void ConvolutionBackwardData(long hCuDnn, T fAlpha, long hFilterDesc, long hWeight, int nWeightOffset, long hTopDesc, long hTopDiff, int nTopOffset, long hConvDesc, CONV_BWD_DATA_ALGO algoBwd, long hWorkspace, int nWorkspaceOffset, ulong lWorkspaceSize, T fBeta, long hBottomDesc, long hBottomDiff, int nBottomOffset, bool bSyncStream = true)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.BWD_CONV_DATA, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hFilterDesc, hWeight, nWeightOffset, hTopDesc, hTopDiff, nTopOffset, hConvDesc, (long)algoBwd, hWorkspace, nWorkspaceOffset, (long)lWorkspaceSize, 0, hBottomDesc, hBottomDiff, nBottomOffset, (bSyncStream) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.BWD_CONV_DATA, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hFilterDesc, hWeight, nWeightOffset, hTopDesc, hTopDiff, nTopOffset, hConvDesc, (long)algoBwd, hWorkspace, nWorkspaceOffset, (long)lWorkspaceSize, 0, hBottomDesc, hBottomDiff, nBottomOffset, (bSyncStream) ? 1 : 0));
        }

        /// <summary>
        /// Create a new instance of a pooling descriptor for use with [NVIDIA's cuDnn](https://developer.nvidia.com/cudnn).
        /// </summary>
        /// <returns>The pooling descriptor handle is returned.</returns>
        public long CreatePoolingDesc()
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CREATE_POOLDESC, null);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CREATE_POOLDESC, null);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free a pooling descriptor instance.
        /// </summary>
        /// <param name="h">Specifies the handle to the pooling descriptor instance.</param>
        public void FreePoolingDesc(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_POOLDESC, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_POOLDESC, m_param.AsFloat(h));
        }

        /// <summary>
        /// Set the values of a pooling descriptor.
        /// </summary>
        /// <param name="hHandle">Specifies the handle to the convolution descriptor.</param>
        /// <param name="method">Specifies the pooling method to use.</param>
        /// <param name="h">Specifies the pooling area height.</param>
        /// <param name="w">Specifies the pooling area width.</param>
        /// <param name="hPad">Specifies the height padding.</param>
        /// <param name="wPad">Specifies the width padding.</param>
        /// <param name="hStride">Specifies the height stride.</param>
        /// <param name="wStride">Specifies the width stride.</param>
        public void SetPoolingDesc(long hHandle, PoolingMethod method, int h, int w, int hPad, int wPad, int hStride, int wStride)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SET_POOLDESC, null, m_param.AsLong(hHandle, (int)method, h, w, hPad, wPad, hStride, wStride));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SET_POOLDESC, null, m_param.AsLong(hHandle, (int)method, h, w, hPad, wPad, hStride, wStride));
        }

        /// <summary>
        /// Perform a pooling forward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hPoolingDesc">Specifies a handle to the pooling descriptor.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hBottomDesc">Specifies a handle to the bottom tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void PoolingForward(long hCuDnn, long hPoolingDesc, T fAlpha, long hBottomDesc, long hBottomData, T fBeta, long hTopDesc, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.FWD_POOL, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, hPoolingDesc, 0, hBottomDesc, hBottomData, 0, hTopDesc, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.FWD_POOL, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, hPoolingDesc, 0, hBottomDesc, hBottomData, 0, hTopDesc, hTopData));
        }

        /// <summary>
        /// Perform a pooling backward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hPoolingDesc">Specifies a handle to the pooling descriptor.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hTopDiffDesc">Specifies a handle to the top diff tensor descriptor.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hBottomDiffDesc">Specifies a handle to the bottom diff tensor descriptor.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void PoolingBackward(long hCuDnn, long hPoolingDesc, T fAlpha, long hTopDataDesc, long hTopData, long hTopDiffDesc, long hTopDiff, long hBottomDataDesc, long hBottomData, T fBeta, long hBottomDiffDesc, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.BWD_POOL, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, hPoolingDesc, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, hBottomDataDesc, hBottomData, 0, hBottomDiffDesc, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.BWD_POOL, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, hPoolingDesc, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, hBottomDataDesc, hBottomData, 0, hBottomDiffDesc, hBottomDiff));
        }

        /// <summary>
        /// Derive the batch norm descriptors for both the forward and backward passes.
        /// </summary>
        /// <param name="hFwdScaleBiasMeanVarDesc">Specifies a handle to the scale bias mean var tensor descriptor for the forward pass.</param>
        /// <param name="hFwdBottomDesc">Specifies a handle to the forward bottom tensor descriptor.</param>
        /// <param name="hBwdScaleBiasMeanVarDesc">Specifies a handle to the scale bias mean var tensor descriptor for the backward pass.</param>
        /// <param name="hBwdBottomDesc">Specifies a handle to the backward bottom tensor descriptor.</param>
        /// <param name="mode"></param>
        public void DeriveBatchNormDesc(long hFwdScaleBiasMeanVarDesc, long hFwdBottomDesc, long hBwdScaleBiasMeanVarDesc, long hBwdBottomDesc, BATCHNORM_MODE mode)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.DERIVE_BNDESC, null, m_param.AsLong(hFwdScaleBiasMeanVarDesc, hFwdBottomDesc, hBwdScaleBiasMeanVarDesc, hBwdBottomDesc, (int)mode));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.DERIVE_BNDESC, null, m_param.AsLong(hFwdScaleBiasMeanVarDesc, hFwdBottomDesc, hBwdScaleBiasMeanVarDesc, hBwdBottomDesc, (int)mode));
        }

        /// <summary>
        /// Run the batch norm forward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="mode">Specifies the batch normalization mode.</param>
        /// <param name="fAlpha">Specifies the alpha value.</param>
        /// <param name="fBeta">Specifies the beta value.</param>
        /// <param name="hFwdBottomDesc">Specifies a handle to the forward bottom tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data tensor.</param>
        /// <param name="hFwdTopDesc">Specifies a handle to the forward top tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top tensor.</param>
        /// <param name="hFwdScaleBiasMeanVarDesc">Specifies a handle to the forward scale bias mean variance descriptor.</param>
        /// <param name="hScaleData">Specifies a handle to the scale tensor.</param>
        /// <param name="hBiasData">Specifies a handle to the bias tensor.</param>
        /// <param name="dfFactor">Specifies a scaling factor.</param>
        /// <param name="hGlobalMean">Specifies a handle to the global mean tensor.</param>
        /// <param name="hGlobalVar">Specifies a handle to the global variance tensor.</param>
        /// <param name="dfEps">Specifies the epsilon value to avoid dividing by zero.</param>
        /// <param name="hSaveMean">Specifies a handle to the saved mean tensor.</param>
        /// <param name="hSaveInvVar">Specifies a handle to the saved variance tensor.</param>
        /// <param name="bTraining">Specifies that this is a training pass when <i>true</i>, and a testing pass when <i>false</i>.</param>
        public void BatchNormForward(long hCuDnn, BATCHNORM_MODE mode, T fAlpha, T fBeta, long hFwdBottomDesc, long hBottomData, long hFwdTopDesc, long hTopData, long hFwdScaleBiasMeanVarDesc, long hScaleData, long hBiasData, double dfFactor, long hGlobalMean, long hGlobalVar, double dfEps, long hSaveMean, long hSaveInvVar, bool bTraining)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.FWD_BN, m_param.AsDouble(convertD(fAlpha), convertD(fBeta), dfFactor, dfEps), m_param.AsLong(hCuDnn, (int)mode, 0, 0, hFwdBottomDesc, hBottomData, hFwdTopDesc, hTopData, hFwdScaleBiasMeanVarDesc, hScaleData, hBiasData, 0, hGlobalMean, hGlobalVar, 0, hSaveMean, hSaveInvVar, (bTraining) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.FWD_BN, m_param.AsFloat(convertF(fAlpha), convertF(fBeta), (float)dfFactor, (float)dfEps), m_param.AsLong(hCuDnn, (int)mode, 0, 0, hFwdBottomDesc, hBottomData, hFwdTopDesc, hTopData, hFwdScaleBiasMeanVarDesc, hScaleData, hBiasData, 0, hGlobalMean, hGlobalVar, 0, hSaveMean, hSaveInvVar, (bTraining) ? 1 : 0));
        }

        /// <summary>
        /// Run the batch norm backward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="mode">Specifies the batch normalization mode.</param>
        /// <param name="fAlphaDiff">Specifies the alpha value applied to the diff.</param>
        /// <param name="fBetaDiff">Specifies the beta value applied to the diff.</param>
        /// <param name="fAlphaParamDiff">Specifies the alpha value applied to the param diff.</param>
        /// <param name="fBetaParamDiff">Specifies the beta value applied to the param diff.</param>
        /// <param name="hBwdBottomDesc">Specifies a handle to the backward bottom tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data tensor.</param>
        /// <param name="hTopDiffDesc">Specifies a handle to the top diff tensor descriptor.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff tensor.</param>
        /// <param name="hBottomDiffDesc">Specifies a handle to the bottom diff tensor descriptor.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff tensor.</param>
        /// <param name="hBwdScaleBiasMeanVarDesc">Specifies a handle to the backward scale bias mean var descriptor.</param>
        /// <param name="hScaleData">Specifies a handle to the scale data tensor.</param>
        /// <param name="hScaleDiff">Specifies a handle to the scale diff tensor.</param>
        /// <param name="hBiasDiff">Specifies a handle to the bias diff tensor.</param>
        /// <param name="dfEps">Specifies the epsilon value.</param>
        /// <param name="hSaveMean">Specifies a handle to the saved mean tensor.</param>
        /// <param name="hSaveInvVar">Specifies a handle to the saved variance tensor.</param>
        public void BatchNormBackward(long hCuDnn, BATCHNORM_MODE mode, T fAlphaDiff, T fBetaDiff, T fAlphaParamDiff, T fBetaParamDiff, long hBwdBottomDesc, long hBottomData, long hTopDiffDesc, long hTopDiff, long hBottomDiffDesc, long hBottomDiff, long hBwdScaleBiasMeanVarDesc, long hScaleData, long hScaleDiff, long hBiasDiff, double dfEps, long hSaveMean, long hSaveInvVar)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.BWD_BN, m_param.AsDouble(convertD(fAlphaDiff), convertD(fBetaDiff), convertD(fAlphaParamDiff), convertD(fBetaParamDiff), dfEps), m_param.AsLong(hCuDnn, (int)mode, 0, 0, 0, 0, hBwdBottomDesc, hBottomData, hTopDiffDesc, hTopDiff, hBottomDiffDesc, hBottomDiff, hBwdScaleBiasMeanVarDesc, hScaleData, hScaleDiff, hBiasDiff, 0, hSaveMean, hSaveInvVar));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.BWD_BN, m_param.AsFloat(convertF(fAlphaDiff), convertF(fBetaDiff), convertF(fAlphaParamDiff), convertF(fBetaParamDiff), (float)dfEps), m_param.AsLong(hCuDnn, (int)mode, 0, 0, 0, 0, hBwdBottomDesc, hBottomData, hTopDiffDesc, hTopDiff, hBottomDiffDesc, hBottomDiff, hBwdScaleBiasMeanVarDesc, hScaleData, hScaleDiff, hBiasDiff, 0, hSaveMean, hSaveInvVar));
        }

        /// <summary>
        /// Create a new instance of a dropout descriptor for use with [NVIDIA's cuDnn](https://developer.nvidia.com/cudnn).
        /// </summary>
        /// <returns>The dropout descriptor handle is returned.</returns>
        public long CreateDropoutDesc()
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CREATE_DROPOUTDESC, null);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CREATE_DROPOUTDESC, null);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free a dropout descriptor instance.
        /// </summary>
        /// <param name="h">Specifies the handle to the dropout descriptor instance.</param>
        public void FreeDropoutDesc(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_DROPOUTDESC, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_DROPOUTDESC, m_param.AsFloat(h));
        }

        /// <summary>
        /// Set the dropout descriptor values.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hDropoutDesc">Specifies a handle to the dropout descriptor.</param>
        /// <param name="dfDropout">Specifies the droput probability (0.5 = 50%).</param>
        /// <param name="hStates">Specifies a handle to the state data in GPU memory.</param>
        /// <param name="lSeed">Specifies the random number-generator seed.</param>
        public void SetDropoutDesc(long hCuDnn, long hDropoutDesc, double dfDropout, long hStates, long lSeed)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SET_DROPOUTDESC, m_param.AsDouble(dfDropout), m_param.AsLong(hCuDnn, hDropoutDesc, 0, hStates, lSeed));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SET_DROPOUTDESC, m_param.AsFloat((float)dfDropout), m_param.AsLong(hCuDnn, hDropoutDesc, 0, hStates, lSeed));
        }

        /// <summary>
        /// Query the dropout state and reserved counts.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hBottomDesc">Specifies a handle to the bottom tensor descriptor.</param>
        /// <param name="ulStateCount">Returns the state count.</param>
        /// <param name="ulReservedCount">Returns the reserved count.</param>
        public void GetDropoutInfo(long hCuDnn, long hBottomDesc, out ulong ulStateCount, out ulong ulReservedCount)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.GET_DROPOUT_INFO, null, m_param.AsLong(hCuDnn, hBottomDesc));
                ulStateCount = (ulong)Math.Round(rg[0] / sizeof(double), 0, MidpointRounding.AwayFromZero);
                ulReservedCount = (ulong)Math.Round(rg[1] / sizeof(double), 0, MidpointRounding.AwayFromZero);
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.GET_DROPOUT_INFO, null, m_param.AsLong(hCuDnn, hBottomDesc));
                ulStateCount = (ulong)Math.Round(rg[0] / sizeof(float), 0, MidpointRounding.AwayFromZero);
                ulReservedCount = (ulong)Math.Round(rg[1] / sizeof(float), 0, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// Performs a dropout forward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hDropoutDesc">Specifies a handle to the dropout descriptor.</param>
        /// <param name="hBottomDesc">Specifies a handle to the bottom tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hReserved">Specifies a handle to the reseved data in GPU memory.</param>
        public void DropoutForward(long hCuDnn, long hDropoutDesc, long hBottomDesc, long hBottomData, long hTopDesc, long hTopData, long hReserved)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.FWD_DROPOUT, null, m_param.AsLong(hCuDnn, hDropoutDesc, hBottomDesc, hBottomData, hTopDesc, hTopData, hReserved));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.FWD_DROPOUT, null, m_param.AsLong(hCuDnn, hDropoutDesc, hBottomDesc, hBottomData, hTopDesc, hTopData, hReserved));
        }

        /// <summary>
        /// Performs a dropout backward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hDropoutDesc">Specifies a handle to the dropout descriptor.</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="hTop">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hBottomDesc">Specifies a handle to the bottom tensor descriptor.</param>
        /// <param name="hBottom">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hReserved">Specifies a handle to the reseved data in GPU memory.</param>
        public void DropoutBackward(long hCuDnn, long hDropoutDesc, long hTopDesc, long hTop, long hBottomDesc, long hBottom, long hReserved)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.BWD_DROPOUT, null, m_param.AsLong(hCuDnn, hDropoutDesc, hTopDesc, hTop, hBottomDesc, hBottom, hReserved));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.BWD_DROPOUT, null, m_param.AsLong(hCuDnn, hDropoutDesc, hTopDesc, hTop, hBottomDesc, hBottom, hReserved));
        }

        /// <summary>
        /// Create a new instance of a LRN descriptor for use with [NVIDIA's cuDnn](https://developer.nvidia.com/cudnn).
        /// </summary>
        /// <returns>The LRN descriptor handle is returned.</returns>
        public long CreateLRNDesc()
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CREATE_LRNDESC, null);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CREATE_LRNDESC, null);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free a LRN descriptor instance.
        /// </summary>
        /// <param name="h">Specifies the handle to the LRN descriptor instance.</param>
        public void FreeLRNDesc(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_LRNDESC, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_LRNDESC, m_param.AsFloat(h));
        }

        /// <summary>
        /// Set the LRN descriptor values.
        /// </summary>
        /// <param name="hHandle">Specifies a handle to an LRN descriptor.</param>
        /// <param name="nSize">Specifies the normalization window width.  Default = 5.</param>
        /// <param name="fAlpha">Specifies the alpha variance.  Caffe default = 1.0; cuDnn default = 1e-4.</param>
        /// <param name="fBeta">Specifies the beta power parameter.  Caffe and cuDnn default = 0.75.</param>
        /// <param name="fK">Specifies the normalization 'k' parameter.  Caffe default = 1.0; cuDnn default = 2.0.</param>
        public void SetLRNDesc(long hHandle, uint nSize, double fAlpha, double fBeta, double fK)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SET_LRNDESC, m_param.AsDouble(fAlpha, fBeta, fK), m_param.AsLong(hHandle, nSize, 0, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SET_LRNDESC, m_param.AsFloat((float)fAlpha, (float)fBeta, (float)fK), m_param.AsLong(hHandle, nSize, 0, 0, 0));
        }

        /// <summary>
        /// Perform LRN cross channel forward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hNormDesc">Specifies a handle to an LRN descriptor.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hBottomDesc">Specifies a handle to the bottom tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hTopDesc">Specifies a handle to the top tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void LRNCrossChannelForward(long hCuDnn, long hNormDesc, T fAlpha, long hBottomDesc, long hBottomData, T fBeta, long hTopDesc, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.LRN_CC_FWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, hNormDesc, 0, hBottomDesc, hBottomData, 0, hTopDesc, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.LRN_CC_FWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, hNormDesc, 0, hBottomDesc, hBottomData, 0, hTopDesc, hTopData));
        }

        /// <summary>
        /// Perform LRN cross channel backward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hNormDesc">Specifies a handle to an LRN descriptor.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hTopDiffDesc">Specifies a handle to the top diff tensor descriptor.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hBottomDiffDesc">Specifies a handle to the bottom diff descriptor.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void LRNCrossChannelBackward(long hCuDnn, long hNormDesc, T fAlpha, long hTopDataDesc, long hTopData, long hTopDiffDesc, long hTopDiff, long hBottomDataDesc, long hBottomData, T fBeta, long hBottomDiffDesc, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.LRN_CC_BWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, hNormDesc, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, hBottomDataDesc, hBottomData, 0, hBottomDiffDesc, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.LRN_CC_BWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, hNormDesc, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, hBottomDataDesc, hBottomData, 0, hBottomDiffDesc, hBottomDiff));
        }

        /// <summary>
        /// Performs a Devisive Normalization forward pass.
        /// </summary>
        /// <remarks>
        /// See [What is the Best Feature Learning Procedure in Hierarchical Recognition Architectures?](https://arxiv.org/abs/1606.01535) by Jarrett, et al.
        /// </remarks>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hNormDesc">Specifies a handle to an LRN descriptor.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTemp1">Temporary data in GPU memory.</param>
        /// <param name="hTemp2">Temporary data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void DivisiveNormalizationForward(long hCuDnn, long hNormDesc, T fAlpha, long hBottomDataDesc, long hBottomData, long hTemp1, long hTemp2, T fBeta, long hTopDataDesc, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.LCN_CC_FWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, hNormDesc, 0, hBottomDataDesc, hBottomData, hTemp1, hTemp2, 0, hTopDataDesc, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.LCN_CC_FWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, hNormDesc, 0, hBottomDataDesc, hBottomData, hTemp1, hTemp2, 0, hTopDataDesc, hTopData));
        }

        /// <summary>
        /// Performs a Devisive Normalization backward pass.
        /// </summary>
        /// <remarks>
        /// See [What is the Best Feature Learning Procedure in Hierarchical Recognition Architectures?](https://arxiv.org/abs/1606.01535) by Jarrett, et al.
        /// </remarks>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hNormDesc">Specifies a handle to an LRN descriptor.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTemp1">Temporary data in GPU memory.</param>
        /// <param name="hTemp2">Temporary data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hBottomDiffDesc">Specifies a handle to the bottom diff tensor descriptor.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void DivisiveNormalizationBackward(long hCuDnn, long hNormDesc, T fAlpha, long hBottomDataDesc, long hBottomData, long hTopDiff, long hTemp1, long hTemp2, T fBeta, long hBottomDiffDesc, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.LCN_CC_BWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, hNormDesc, 0, hBottomDataDesc, hBottomData, hTopDiff, hTemp1, hTemp2, 0, hBottomDiffDesc, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.LCN_CC_BWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, hNormDesc, 0, hBottomDataDesc, hBottomData, hTopDiff, hTemp1, hTemp2, 0, hBottomDiffDesc, hBottomDiff));
        }

        /// <summary>
        /// Perform a Tanh forward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void TanhForward(long hCuDnn, T fAlpha, long hBottomDataDesc, long hBottomData, T fBeta, long hTopDataDesc, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.TANH_FWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDataDesc, hBottomData, 0, hTopDataDesc, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.TANH_FWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDataDesc, hBottomData, 0, hTopDataDesc, hTopData));
        }

        /// <summary>
        /// Perform a Tanh backward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hTopDiffDesc">Specifies a handle to the top diff tensor descriptor</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hBottomDiffDesc">Specifies a handle to the bottom diff tensor descriptor.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void TanhBackward(long hCuDnn, T fAlpha, long hTopDataDesc, long hTopData, long hTopDiffDesc, long hTopDiff, long hBottomDataDesc, long hBottomData, T fBeta, long hBottomDiffDesc, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.TANH_BWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, hBottomDataDesc, hBottomData, 0, hBottomDiffDesc, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.TANH_BWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, hBottomDataDesc, hBottomData, 0, hBottomDiffDesc, hBottomDiff));
        }

        /// <summary>
        /// Perform a Elu forward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void EluForward(long hCuDnn, T fAlpha, long hBottomDataDesc, long hBottomData, T fBeta, long hTopDataDesc, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.ELU_FWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDataDesc, hBottomData, 0, hTopDataDesc, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.ELU_FWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDataDesc, hBottomData, 0, hTopDataDesc, hTopData));
        }

        /// <summary>
        /// Perform a Elu backward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hTopDiffDesc">Specifies a handle to the top diff tensor descriptor</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hBottomDiffDesc">Specifies a handle to the bottom diff tensor descriptor.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void EluBackward(long hCuDnn, T fAlpha, long hTopDataDesc, long hTopData, long hTopDiffDesc, long hTopDiff, long hBottomDataDesc, long hBottomData, T fBeta, long hBottomDiffDesc, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.ELU_BWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, hBottomDataDesc, hBottomData, 0, hBottomDiffDesc, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.ELU_BWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, hBottomDataDesc, hBottomData, 0, hBottomDiffDesc, hBottomDiff));
        }

        /// <summary>
        /// Perform a Sigmoid forward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void SigmoidForward(long hCuDnn, T fAlpha, long hBottomDataDesc, long hBottomData, T fBeta, long hTopDataDesc, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SIGMOID_FWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDataDesc, hBottomData, 0, hTopDataDesc, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SIGMOID_FWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDataDesc, hBottomData, 0, hTopDataDesc, hTopData));
        }

        /// <summary>
        /// Perform a Sigmoid backward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hTopDiffDesc">Specifies a handle to the top diff tensor descriptor</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hBottomDiffDesc">Specifies a handle to the bottom diff tensor descriptor.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void SigmoidBackward(long hCuDnn, T fAlpha, long hTopDataDesc, long hTopData, long hTopDiffDesc, long hTopDiff, long hBottomDataDesc, long hBottomData, T fBeta, long hBottomDiffDesc, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SIGMOID_BWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, hBottomDataDesc, hBottomData, 0, hBottomDiffDesc, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SIGMOID_BWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, hBottomDataDesc, hBottomData, 0, hBottomDiffDesc, hBottomDiff));
        }

        /// <summary>
        /// Perform a ReLU forward pass.
        /// </summary>
        /// <remarks>
        /// See [Rectifier Nonlinearities Improve Neural Network Acoustic Models](https://www.semanticscholar.org/paper/Rectifier-Nonlinearities-Improve-Neural-Network-Maas-Hannun/367f2c63a6f6a10b3b64b8729d601e69337ee3cc) by 
        /// Maas, A. L., Hannun, A. Y., and Ng, A. Y. (2013),  In ICML Workshop on Deep Learning
        /// for Audio, Speech, and Language Processing.
        /// </remarks>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void ReLUForward(long hCuDnn, T fAlpha, long hBottomDataDesc, long hBottomData, T fBeta, long hTopDataDesc, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.RELU_FWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDataDesc, hBottomData, 0, hTopDataDesc, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.RELU_FWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDataDesc, hBottomData, 0, hTopDataDesc, hTopData));
        }

        /// <summary>
        /// Perform a ReLU backward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hTopDiffDesc">Specifies a handle to the top diff tensor descriptor</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hBottomDiffDesc">Specifies a handle to the bottom diff tensor descriptor.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void ReLUBackward(long hCuDnn, T fAlpha, long hTopDataDesc, long hTopData, long hTopDiffDesc, long hTopDiff, long hBottomDataDesc, long hBottomData, T fBeta, long hBottomDiffDesc, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.RELU_BWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, hBottomDataDesc, hBottomData, 0, hBottomDiffDesc, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.RELU_BWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, hBottomDataDesc, hBottomData, 0, hBottomDiffDesc, hBottomDiff));
        }

        /// <summary>
        /// Perform a Softmax forward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="alg">Specifies the SoftmaxAlgorithm to use (FAST, ACCURATE or LOG).</param>
        /// <param name="mode">Specifies the SoftmaxMode to use (INSTANCE across NxCHW, or CHANNEL across NCxHW)</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hBottomDataDesc">Specifies a handle to the bottom data tensor descriptor.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void SoftmaxForward(long hCuDnn, SOFTMAX_ALGORITHM alg, SOFTMAX_MODE mode, T fAlpha, long hBottomDataDesc, long hBottomData, T fBeta, long hTopDataDesc, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SOFTMAX_FWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDataDesc, hBottomData, 0, hTopDataDesc, hTopData, (int)alg, (int)mode));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SOFTMAX_FWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hBottomDataDesc, hBottomData, 0, hTopDataDesc, hTopData, (int)alg, (int)mode));
        }

        /// <summary>
        /// Perform a Softmax backward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="alg">Specifies the SoftmaxAlgorithm to use (FAST, ACCURATE or LOG).</param>
        /// <param name="mode">Specifies the SoftmaxMode to use (INSTANCE across NxCHW, or CHANNEL across NCxHW)</param>
        /// <param name="fAlpha">Specifies a scaling factor applied to the result.</param>
        /// <param name="hTopDataDesc">Specifies a handle to the top data tensor descriptor.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hTopDiffDesc">Specifies a handle to the top diff tensor descriptor.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="fBeta">Specifies a scaling factor applied to the prior destination value.</param>
        /// <param name="hBottomDiffDesc">Specifies a handle to the bottom diff tensor descriptor.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void SoftmaxBackward(long hCuDnn, SOFTMAX_ALGORITHM alg, SOFTMAX_MODE mode, T fAlpha, long hTopDataDesc, long hTopData, long hTopDiffDesc, long hTopDiff, T fBeta, long hBottomDiffDesc, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SOFTMAX_BWD, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(hCuDnn, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, 0, hBottomDiffDesc, hBottomDiff, (int)alg, (int)mode));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SOFTMAX_BWD, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(hCuDnn, 0, hTopDataDesc, hTopData, hTopDiffDesc, hTopDiff, 0, hBottomDiffDesc, hBottomDiff, (int)alg, (int)mode));
        }

        /// <summary>
        /// Create the RNN Data Descriptor.
        /// </summary>
        /// <returns>A handle to the RNN Data descriptor is returned.</returns>
        public long CreateRnnDataDesc()
        {
            int nFn = (m_bEnableRnnExtendedVersion) ? (int)CUDAFN.CREATE_RNN_DATA_DESCEX : (int)CUDAFN.CREATE_RNN_DATA_DESC;

            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, nFn, null);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, nFn, null);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free an existing RNN Data descriptor.
        /// </summary>
        /// <param name="h">Specifies the handle to the RNN Data descriptor created with CreateRnnDataDesc</param>
        public void FreeRnnDataDesc(long h)
        {
            int nFn = (m_bEnableRnnExtendedVersion) ? (int)CUDAFN.FREE_RNN_DATA_DESCEX : (int)CUDAFN.FREE_RNN_DATA_DESC;

            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, nFn, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, nFn, m_param.AsFloat(h));
        }

        /// <summary>
        /// Sets the RNN Data Descriptor values.
        /// </summary>
        /// <param name="hRnnDataDesc">Specifies the handle to the RNN descriptor created with CreateRnnDesc</param>
        /// <param name="layout">Specifies the input data layout (either SEQUENCE major or BATCH major).</param>
        /// <param name="nMaxSeqLen">Specifies the maximum sequence length.</param>
        /// <param name="nBatchSize">Specifies the batch count.</param>
        /// <param name="nVectorSize">Specifies the input vector count.</param>
        /// <param name="bBidirectional">Specifies whether the Rnn is bidirectional or not (default = false).</param>
        /// <param name="rgSeqLen">Specifies the sequence lengths - currently this should be <i>null</i> which sets all sequence lengths to nMaxSeqLen.</param>
        public void SetRnnDataDesc(long hRnnDataDesc, RNN_DATALAYOUT layout, int nMaxSeqLen, int nBatchSize, int nVectorSize, bool bBidirectional = false, int[] rgSeqLen = null)
        {
            if (!m_bEnableRnnExtendedVersion && layout != RNN_DATALAYOUT.RNN_SEQ_MAJOR_UNPACKED)
                throw new Exception("The non-extended functions only support RNN_SEQ_MAJOR ordering.");

            int nFn = (m_bEnableRnnExtendedVersion) ? (int)CUDAFN.SET_RNN_DATA_DESCEX : (int)CUDAFN.SET_RNN_DATA_DESC;

            if (m_dt == DataType.DOUBLE)
            {
                List<long> rgArg = new List<long>() { hRnnDataDesc, (long)layout, nMaxSeqLen, nBatchSize, nVectorSize, (bBidirectional) ? 1 : 0 };

                if (rgSeqLen != null)
                {
                    for (int i = 0; i < rgSeqLen.Length; i++)
                    {
                        rgArg.Add(rgSeqLen[i]);
                    }
                }

                m_cuda.RunDoubleEx2((int)m_hKernel, nFn, null, rgArg.ToArray());
            }
            else
            {
                List<long> rgArg = new List<long>() { hRnnDataDesc, (long)layout, nMaxSeqLen, nBatchSize, nVectorSize, (bBidirectional) ? 1 : 0 };

                if (rgSeqLen != null)
                {
                    for (int i = 0; i < rgSeqLen.Length; i++)
                    {
                        rgArg.Add(rgSeqLen[i]);
                    }
                }

                m_cuda.RunFloatEx2((int)m_hKernel, nFn, null, rgArg.ToArray());
            }
        }

        /// <summary>
        /// Create the RNN Descriptor.
        /// </summary>
        /// <returns>A handle to the RNN descriptor is returned.</returns>
        public long CreateRnnDesc()
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CREATE_RNN_DESC, null);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CREATE_RNN_DESC, null);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free an existing RNN descriptor.
        /// </summary>
        /// <param name="h">Specifies the handle to the RNN descriptor created with CreateRnnDesc</param>
        public void FreeRnnDesc(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.FREE_RNN_DESC, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.FREE_RNN_DESC, m_param.AsFloat(h));
        }

        /// <summary>
        /// Sets the RNN Descriptor values.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hRnnDesc">Specifies the handle to the RNN descriptor created with CreateRnnDesc</param>
        /// <param name="nHiddenCount">Specifies the hidden input (typically the input) count.</param>
        /// <param name="nNumLayers">Specifies the number of layers.</param>
        /// <param name="hDropoutDesc">Specifies the handle to the Droput descriptor (or 0 to ignore).  The droput descriptor is only used with two or more layers.</param>
        /// <param name="mode">Specifies the RNN_MODE (LSTM, RNN_RELU, RNN_TANH) to use.</param>
        /// <param name="bUseTensorCores">Optionally, specifies whether or not to use the Tensor Cores (if available).</param>
        /// <param name="direction">Optionally, specifies the direction of the RNN; Unidirectional or BiDirectional.</param>
        public void SetRnnDesc(long hCuDnn, long hRnnDesc, int nHiddenCount, int nNumLayers, long hDropoutDesc, RNN_MODE mode, bool bUseTensorCores, RNN_DIRECTION direction = RNN_DIRECTION.RNN_UNIDIRECTIONAL)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.SET_RNN_DESC, null, m_param.AsLong(hCuDnn, hRnnDesc, nHiddenCount, nNumLayers, hDropoutDesc, (int)mode, (bUseTensorCores) ? 1 : 0, (long)direction));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.SET_RNN_DESC, null, m_param.AsLong(hCuDnn, hRnnDesc, nHiddenCount, nNumLayers, hDropoutDesc, (int)mode, (bUseTensorCores) ? 1 : 0, (long)direction));
        }

        /// <summary>
        /// Returns the RNN parameter count.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hRnnDesc">Specifies the handle to the RNN descriptor created with CreateRnnDesc</param>
        /// <param name="hXDesc">Specifies the handle to the first X descriptor.</param>
        /// <returns>The number of parameters (weights) is returned.</returns>
        public int GetRnnParamCount(long hCuDnn, long hRnnDesc, long hXDesc)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.GET_RNN_PARAMCOUNT, null, m_param.AsLong(hCuDnn, hRnnDesc, hXDesc, (m_bEnableRnnExtendedVersion) ? 1 : 0));
                return (int)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.GET_RNN_PARAMCOUNT, null, m_param.AsLong(hCuDnn, hRnnDesc, hXDesc, (m_bEnableRnnExtendedVersion) ? 1 : 0));
                return (int)rg[0];
            }
        }

        /// <summary>
        /// Returns the workspace and reserved counts.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hRnnDesc">Specifies the handle to the RNN descriptor created with CreateRnnDesc</param>
        /// <param name="hXDesc">Specifies a handle to the data descriptor created with CreateRnnDataDesc.</param>
        /// <param name="nReservedCount">Returns the reserved count needed.</param>
        /// <returns>Returns the workspace count needed.</returns>
        public ulong GetRnnWorkspaceCount(long hCuDnn, long hRnnDesc, long hXDesc, out ulong nReservedCount)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.GET_RNN_WORKSPACECOUNT, null, m_param.AsLong(hCuDnn, hRnnDesc, (m_bEnableRnnExtendedVersion) ? 1 : 0, hXDesc));
                nReservedCount = (ulong)rg[1];
                return (ulong)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.GET_RNN_WORKSPACECOUNT, null, m_param.AsLong(hCuDnn, hRnnDesc, (m_bEnableRnnExtendedVersion) ? 1 : 0, hXDesc));
                nReservedCount = (ulong)rg[1];
                return (ulong)rg[0];
            }
        }

        /// <summary>
        /// Returns the linear layer parameters (weights).
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hRnnDesc">Specifies the handle to the RNN descriptor created with CreateRnnDesc</param>
        /// <param name="nLayer">Specifies the current layer index.</param>
        /// <param name="hXDesc">Specifies the input data elelement descriptor.</param>
        /// <param name="hWtDesc">Specifies the weight descriptor.</param>
        /// <param name="hWtData">Specifies the weight memory containing all weights.</param>
        /// <param name="nLinLayer">Specifies the linear layer index (e.g. LSTM has 8 linear layers, RNN has 2)</param>
        /// <param name="nWtCount">Returns the number of weight items.</param>
        /// <param name="hWt">Returns a handle to the weight GPU memory.</param>
        /// <param name="nBiasCount">Returns the number of bias items.</param>
        /// <param name="hBias">Returns a handle to the bias GPU memory.</param>
        public void GetRnnLinLayerParams(long hCuDnn, long hRnnDesc, int nLayer, long hXDesc, long hWtDesc, long hWtData, int nLinLayer, out int nWtCount, out long hWt, out int nBiasCount, out long hBias)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.GET_RNN_LINLAYERPARAMS, null, m_param.AsLong(hCuDnn, hRnnDesc, nLayer, hXDesc, hWtDesc, hWtData, nLinLayer, (m_bEnableRnnExtendedVersion) ? 1 : 0));
                nWtCount = (int)rg[0];
                hWt = (long)rg[1];
                nBiasCount = (int)rg[2];
                hBias = (long)rg[3];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.GET_RNN_LINLAYERPARAMS, null, m_param.AsLong(hCuDnn, hRnnDesc, nLayer, hXDesc, hWtDesc, hWtData, nLinLayer, (m_bEnableRnnExtendedVersion) ? 1 : 0));
                nWtCount = (int)rg[0];
                hWt = (long)rg[1];
                nBiasCount = (int)rg[2];
                hBias = (long)rg[3];
            }
        }

        /// <summary>
        /// Run the RNN through a forward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hRnnDesc">Specifies the handle to the RNN descriptor created with CreateRnnDesc</param>
        /// <param name="hXDesc">Specifies a handle to the input data descriptor.</param>
        /// <param name="hXData">Specifies a handle to the input GPU data.</param>
        /// <param name="hHxDesc">Specifies a handle to the hidden data descriptor.</param>
        /// <param name="hHxData">Specifies a handle to the hidden GPU data.</param>
        /// <param name="hCxDesc">Specifies a handle to the cont data descriptor.</param>
        /// <param name="hCxData">Specifies a handle to the cont GPU data.</param>
        /// <param name="hWtDesc">Specifies a handle to the weight descriptor.</param>
        /// <param name="hWtData">Specifies a handle to the weight data.</param>
        /// <param name="hYDesc">Specifies a handle to the output data descriptor.</param>
        /// <param name="hYData">Specifies a handle to the output GPU data.</param>
        /// <param name="hHyDesc">Specifies a handle to the output hidden descriptor.</param>
        /// <param name="hHyData">Specifies a handle to the output hidden data.</param>
        /// <param name="hCyDesc">Specifies a handle to the output cont descriptor.</param>
        /// <param name="hCyData">Specifies a handle to the output cont data.</param>
        /// <param name="hWorkspace">Specifies a handle to the workspace GPU memory.</param>
        /// <param name="nWsCount">Specifies the number of items within the workspace.</param>
        /// <param name="hReserved">Specifies a handle to the reserved GPU memory.</param>
        /// <param name="nResCount">Specifies the number of items within the reserved memory.</param>
        /// <param name="bTraining">Specifies the whether the forward pass is during taining or not.</param>
        public void RnnForward(long hCuDnn, long hRnnDesc, long hXDesc, long hXData, long hHxDesc, long hHxData, long hCxDesc, long hCxData, long hWtDesc, long hWtData, long hYDesc, long hYData, long hHyDesc, long hHyData, long hCyDesc, long hCyData, long hWorkspace, ulong nWsCount, long hReserved, ulong nResCount, bool bTraining)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<long> rgArg = new List<long>() { hCuDnn, hRnnDesc };

                rgArg.Add(hXDesc);
                rgArg.Add(hXData);

                rgArg.Add(hHxDesc);
                rgArg.Add(hHxData);
                rgArg.Add(hCxDesc);
                rgArg.Add(hCxData);

                rgArg.Add(hWtDesc);
                rgArg.Add(hWtData);

                rgArg.Add(hYDesc);
                rgArg.Add(hYData);

                rgArg.Add(hHyDesc);
                rgArg.Add(hHyData);
                rgArg.Add(hCyDesc);
                rgArg.Add(hCyData);

                rgArg.Add(hWorkspace);
                rgArg.Add((long)nWsCount);
                rgArg.Add(hReserved);
                rgArg.Add((long)nResCount);
                rgArg.Add((bTraining) ? 1 : 0);

                if (m_bEnableRnnExtendedVersion)
                    rgArg.Add(1);

                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.FWD_RNN, null, rgArg.ToArray());
            }
            else
            {
                List<long> rgArg = new List<long>() { hCuDnn, hRnnDesc };

                rgArg.Add(hXDesc);
                rgArg.Add(hXData);

                rgArg.Add(hHxDesc);
                rgArg.Add(hHxData);
                rgArg.Add(hCxDesc);
                rgArg.Add(hCxData);

                rgArg.Add(hWtDesc);
                rgArg.Add(hWtData);

                rgArg.Add(hYDesc);
                rgArg.Add(hYData);

                rgArg.Add(hHyDesc);
                rgArg.Add(hHyData);
                rgArg.Add(hCyDesc);
                rgArg.Add(hCyData);

                rgArg.Add(hWorkspace);
                rgArg.Add((long)nWsCount);
                rgArg.Add(hReserved);
                rgArg.Add((long)nResCount);
                rgArg.Add((bTraining) ? 1 : 0);

                if (m_bEnableRnnExtendedVersion)
                    rgArg.Add(1);

                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.FWD_RNN, null, rgArg.ToArray());
            }
        }

        /// <summary>
        /// Run the RNN backward pass through the data.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hRnnDesc">Specifies the handle to the RNN descriptor created with CreateRnnDesc</param>
        /// <param name="hYDesc">Specifies a handle to the output data descriptor.</param>
        /// <param name="hYData">Specifies a handle to the output GPU data.</param>
        /// <param name="hYDiff">Specifies a handle to the output GPU gradients.</param>
        /// <param name="hHyDesc">Specifies a handle to the output hidden descriptor.</param>
        /// <param name="hHyDiff">Specifies a handle to the output hidden gradients.</param>
        /// <param name="hCyDesc">Specifies a handle to the output cont descriptor.</param>
        /// <param name="hCyDiff">Specifies a handle to the output cont gradients.</param>
        /// <param name="hWtDesc">Specifies a handle to the weight descriptor.</param>
        /// <param name="hWtData">Specifies a handle to the weight data.</param>
        /// <param name="hHxDesc">Specifies a handle to the hidden data descriptor.</param>
        /// <param name="hHxData">Specifies a handle to the hidden GPU data.</param>
        /// <param name="hCxDesc">Specifies a handle to the cont data descriptor.</param>
        /// <param name="hCxData">Specifies a handle to the cont GPU data.</param>
        /// <param name="hXDesc">Specifies a handle to the input data descriptor.</param>
        /// <param name="hXDiff">Specifies a handle to the input GPU gradients.</param>
        /// <param name="hdHxDesc">Specifies a handle to the input hidden descriptor for the gradients.</param>
        /// <param name="hHxDiff">Specifis a handle to the input hidden GPU gradients.</param>
        /// <param name="hdCxDesc">Specifies a handle to the input cont descriptor of the gradients.</param>
        /// <param name="hCxDiff">Specifies a handle to the input cont GPU gradients.</param>
        /// <param name="hWorkspace">Specifies a handle to the workspace GPU memory.</param>
        /// <param name="nWsCount">Specifies the number of items within the workspace.</param>
        /// <param name="hReserved">Specifies a handle to the reserved GPU memory.</param>
        /// <param name="nResCount">Specifies the number of items within the reserved memory.</param>
        public void RnnBackwardData(long hCuDnn, long hRnnDesc, long hYDesc, long hYData, long hYDiff, long hHyDesc, long hHyDiff, long hCyDesc, long hCyDiff, long hWtDesc, long hWtData, long hHxDesc, long hHxData, long hCxDesc, long hCxData, long hXDesc, long hXDiff, long hdHxDesc, long hHxDiff, long hdCxDesc, long hCxDiff, long hWorkspace, ulong nWsCount, long hReserved, ulong nResCount)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<long> rgArg = new List<long>() { hCuDnn, hRnnDesc };

                rgArg.Add(hYDesc);
                rgArg.Add(hYData);
                rgArg.Add(hYDiff);

                rgArg.Add(hHyDesc);
                rgArg.Add(hHyDiff);
                rgArg.Add(hCyDesc);
                rgArg.Add(hCyDiff);

                rgArg.Add(hWtDesc);
                rgArg.Add(hWtData);

                rgArg.Add(hHxDesc);
                rgArg.Add(hHxData);
                rgArg.Add(hCxDesc);
                rgArg.Add(hCxData);

                rgArg.Add(hXDesc);
                rgArg.Add(hXDiff);

                rgArg.Add(hdHxDesc);
                rgArg.Add(hHxDiff);
                rgArg.Add(hdCxDesc);
                rgArg.Add(hCxDiff);

                rgArg.Add(hWorkspace);
                rgArg.Add((long)nWsCount);
                rgArg.Add(hReserved);
                rgArg.Add((long)nResCount);

                if (m_bEnableRnnExtendedVersion)
                    rgArg.Add(1);

                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.BWD_RNN_DATA, null, rgArg.ToArray());
            }
            else
            {
                List<long> rgArg = new List<long>() { hCuDnn, hRnnDesc };

                rgArg.Add(hYDesc);
                rgArg.Add(hYData);
                rgArg.Add(hYDiff);

                rgArg.Add(hHyDesc);
                rgArg.Add(hHyDiff);
                rgArg.Add(hCyDesc);
                rgArg.Add(hCyDiff);

                rgArg.Add(hWtDesc);
                rgArg.Add(hWtData);

                rgArg.Add(hHxDesc);
                rgArg.Add(hHxData);
                rgArg.Add(hCxDesc);
                rgArg.Add(hCxData);

                rgArg.Add(hXDesc);
                rgArg.Add(hXDiff);

                rgArg.Add(hdHxDesc);
                rgArg.Add(hHxDiff);
                rgArg.Add(hdCxDesc);
                rgArg.Add(hCxDiff);

                rgArg.Add(hWorkspace);
                rgArg.Add((long)nWsCount);
                rgArg.Add(hReserved);
                rgArg.Add((long)nResCount);

                if (m_bEnableRnnExtendedVersion)
                    rgArg.Add(1);

                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.BWD_RNN_DATA, null, rgArg.ToArray());
            }
        }

        /// <summary>
        /// Run the RNN backward pass on the weights.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hRnnDesc">Specifies the handle to the RNN descriptor created with CreateRnnDesc</param>
        /// <param name="hXDesc">Specifies a handle to the input data descriptor.</param>
        /// <param name="hXData">Specifies a handle to the input GPU data.</param>
        /// <param name="hHxDesc">Specifies a handle to the hidden data descriptor.</param>
        /// <param name="hHxData">Specifies a handle to the hidden GPU data.</param>
        /// <param name="hYDesc">Specifies a handle to the output data descriptor.</param>
        /// <param name="hYData">Specifies a handle to the output GPU data.</param>
        /// <param name="hWorkspace">Specifies a handle to the workspace GPU memory.</param>
        /// <param name="nWsCount">Specifies the number of items within the workspace.</param>
        /// <param name="hWtDesc">Specifies a handle to the weight descriptor.</param>
        /// <param name="hWtDiff">Specifies a handle to the weight gradients.</param>
        /// <param name="hReserved">Specifies a handle to the reserved GPU memory.</param>
        /// <param name="nResCount">Specifies the number of items within the reserved memory.</param>
        public void RnnBackwardWeights(long hCuDnn, long hRnnDesc, long hXDesc, long hXData, long hHxDesc, long hHxData, long hYDesc, long hYData, long hWorkspace, ulong nWsCount, long hWtDesc, long hWtDiff, long hReserved, ulong nResCount)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<long> rgArg = new List<long>() { hCuDnn, hRnnDesc };

                rgArg.Add(hXDesc);
                rgArg.Add(hXData);

                rgArg.Add(hHxDesc);
                rgArg.Add(hHxData);

                rgArg.Add(hYDesc);
                rgArg.Add(hYData);

                rgArg.Add(hWorkspace);
                rgArg.Add((long)nWsCount);

                rgArg.Add(hWtDesc);
                rgArg.Add(hWtDiff);

                rgArg.Add(hReserved);
                rgArg.Add((long)nResCount);

                if (m_bEnableRnnExtendedVersion)
                    rgArg.Add(1);

                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.BWD_RNN_WTS, null, rgArg.ToArray());
            }
            else
            {
                List<long> rgArg = new List<long>() { hCuDnn, hRnnDesc };

                rgArg.Add(hXDesc);
                rgArg.Add(hXData);

                rgArg.Add(hHxDesc);
                rgArg.Add(hHxData);

                rgArg.Add(hYDesc);
                rgArg.Add(hYData);

                rgArg.Add(hWorkspace);
                rgArg.Add((long)nWsCount);

                rgArg.Add(hWtDesc);
                rgArg.Add(hWtDiff);

                rgArg.Add(hReserved);
                rgArg.Add((long)nResCount);

                if (m_bEnableRnnExtendedVersion)
                    rgArg.Add(1);

                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.BWD_RNN_WTS, null, rgArg.ToArray());
            }
        }


        /// <summary>
        /// Returns whether or not RNN8 is supported.
        /// </summary>
        public bool IsRnn8Supported()
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.RNN8_IS_SUPPORTED, null);
                return (rg[0] == 1) ? true : false;
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.RNN8_IS_SUPPORTED, null);
                return (rg[0] == 1) ? true : false;
            }
        }

        /// <summary>
        /// Create the RNN8.
        /// </summary>
        /// <returns>A handle to the RNN8 is returned.</returns>
        public long CreateRnn8()
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.RNN8_CREATE, null);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.RNN8_CREATE, null);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free an existing RNN8.
        /// </summary>
        /// <param name="h">Specifies the handle to the RNN8 created with CreateRnn8</param>
        public void FreeRnn8(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.RNN8_FREE, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.RNN8_FREE, m_param.AsFloat(h));
        }

        /// <summary>
        /// Set the RNN8 parameters.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hRnn">Specifies the handle to the RNN8 created with CreateRnn8.</param>
        /// <param name="bTraining">Specifies true for training and false for inference.</param>
        /// <param name="layout">Specifies the data layout ordering.</param>
        /// <param name="cellMode">Specifies the cell mode (RELU, TANH, LSTM or GRU), </param>
        /// <param name="biasMode">Specifies the bias mode (default = RNN_DOUBLE_BIAS)</param>
        /// <param name="nSequenceLen">Specifies the sequence length.</param>
        /// <param name="nBatchSize">Specifies the batch size.</param>
        /// <param name="nInputs">Specifies the number of inputs. X input is of size (SeqLen, BatchSize, Inputs)</param>
        /// <param name="nHidden">Specifies the number of hidden. H and C are of size (BatchSize, Hidden)</param>
        /// <param name="nOutputs">Specifies the number of outputs. Y output is of size (SeqLen, BatchSize, Outputs)</param>
        /// <param name="nProjection">Specifies the projection size.</param>
        /// <param name="nNumLayers">Specifies the number of layers.</param>
        /// <param name="fDropout">Specifies the dropout ratio.</param>
        /// <param name="lSeed">Specifies the dropout seed.</param>
        /// <param name="bBidirectional">Specifies unidirectional (false) or bidirectional (true), (default = false)</param>
        public void SetRnn8(long hCuDnn, long hRnn, bool bTraining, RNN_DATALAYOUT layout, RNN_MODE cellMode, RNN_BIAS_MODE biasMode, int nSequenceLen, int nBatchSize, int nInputs, int nHidden, int nOutputs, int nProjection, int nNumLayers, float fDropout, ulong lSeed, bool bBidirectional = false)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.RNN8_SET, m_param.AsDouble((double)fDropout), m_param.AsLong(hCuDnn, hRnn, (bTraining) ? 1 : 0, (int)layout, (int)cellMode, (int)biasMode, nSequenceLen, nBatchSize, nInputs, nHidden, nOutputs, nProjection, nNumLayers, (long)lSeed, (bBidirectional) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.RNN8_SET, m_param.AsFloat(fDropout), m_param.AsLong(hCuDnn, hRnn, (bTraining) ? 1 : 0, (int)layout, (int)cellMode, (int)biasMode, nSequenceLen, nBatchSize, nInputs, nHidden, nOutputs, nProjection, nNumLayers, (long)lSeed, (bBidirectional) ? 1 : 0));
        }

        /// <summary>
        /// Returns the memory sizes required for the RNN8.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hRnn">Specifies the handle to the RNN8 created with CreateRnn8.</param>
        /// <param name="szWtCount">Returns the required weight count (in items).</param>
        /// <param name="szWorkSize">Returns the rquired work size (in bytes).</param>
        /// <param name="szReservedSize">Returns the required reserved size (in bytes).</param>
        public void GetRnn8MemorySizes(long hCuDnn, long hRnn, out ulong szWtCount, out ulong szWorkSize, out ulong szReservedSize)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.RNN8_GET_MEMORY_SIZES, null, m_param.AsLong(hCuDnn, hRnn));
                szWtCount = (ulong)rg[0];
                szWorkSize = (ulong)rg[1];
                szReservedSize = (ulong)rg[2];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.RNN8_GET_MEMORY_SIZES, null, m_param.AsLong(hCuDnn, hRnn));
                szWtCount = (ulong)rg[0];
                szWorkSize = (ulong)rg[1];
                szReservedSize = (ulong)rg[2];
            }
        }

        /// <summary>
        /// Initialize the RNN8 weights
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hRnn">Specifies the handle to the RNN8 created with CreateRnn8.</param>
        /// <param name="hWt">Specifies the handle to the GPU data containing the weights to be initialized.</param>
        /// <param name="wtFt">Specifies the weight filler type.</param>
        /// <param name="fWtVal">Specifies the weight filler value.</param>
        /// <param name="fWtVal2">Specifies a secondary weight filler value.</param>
        /// <param name="biasFt">Specifies the bias filler type.</param>
        /// <param name="fBiasVal">Specifies the bias filler value.</param>
        /// <param name="fBiasVal2">Specifies a secondary bias filler value.</param>
        public void InitializeRnn8Weights(long hCuDnn, long hRnn, long hWt, RNN_FILLER_TYPE wtFt, double fWtVal, double fWtVal2, RNN_FILLER_TYPE biasFt, double fBiasVal, double fBiasVal2)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.RNN8_INIT_WEIGHTS, m_param.AsDouble(fWtVal, fWtVal2, fBiasVal, fBiasVal2), m_param.AsLong(hCuDnn, hRnn, hWt, (int)wtFt, (int)biasFt));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.RNN8_INIT_WEIGHTS, m_param.AsFloat((float)fWtVal, (float)fWtVal2, (float)fBiasVal, (float)fBiasVal2), m_param.AsLong(hCuDnn, hRnn, hWt, (int)wtFt, (int)biasFt));
        }

        /// <summary>
        /// Calculate the forward pass through the RNN8.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hRnn">Specifies the handle to the RNN8 created with CreateRnn8.</param>
        /// <param name="hX">Specifies a handle to the GPU memory of shape (SeqLen, BatchSize, Inputs) containing the inputs.</param>
        /// <param name="hY">Specifies a handle to the GPU memory of shape (SeqLen, BatchSize, Outputs) where the outputs are placed.</param>
        /// <param name="hhX">Specifies a handle to the GPU memory of shape (BatchSize, Hidden) containing the hidden inputs.</param>
        /// <param name="hhY">Specifies a handle to the GPU memory of shape (BatchSize, Hidden) where the hidden outputs are placed.</param>
        /// <param name="hcX">Specifies a handle to the GPU memory of shape (BatchSize, Hidden) containing the hidden cell inputs.</param>
        /// <param name="hcY">Specifies a handle to the GPU memory of shape (BatchSize, Hidden) where the hidden cell outputs are placed.</param>
        /// <param name="hWts">Specifies a handle to the GPU memory of size szWt calculated with GetRnn8MemorySizes, containing the weights.</param>
        /// <param name="hWork">Specifies a handle to the GPU memory of size szWork calculated with GetRnn8MemorySizes, used as temporary work data.</param>
        /// <param name="hReserved">Specifies a handle to the GPU memory of size szReserved calculated with GetRnn8MemorySizes, used as temporary reserve data.</param>
        public void Rnn8Forward(long hCuDnn, long hRnn, long hX, long hY, long hhX, long hhY, long hcX, long hcY, long hWts, long hWork, long hReserved)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.RNN8_FWD, null, m_param.AsLong(hCuDnn, hRnn, hX, hY, hhX, hhY, hcX, hcY, hWts, hWork, hReserved));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.RNN8_FWD, null, m_param.AsLong(hCuDnn, hRnn, hX, hY, hhX, hhY, hcX, hcY, hWts, hWork, hReserved));
        }

        /// <summary>
        /// Calculate the backward pass through the RNN8 for both data and weights.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hRnn">Specifies the handle to the RNN8 created with CreateRnn8.</param>
        /// <param name="hY">Specifies a handle to the GPU memory of shape (SeqLen, BatchSize, Outputs) containing the outputs from the forward.</param>
        /// <param name="hdY">Specifies a handle to the GPU memory of shape (SeqLen, BatchSize, Outputs) containing the inbound gradients for Y.</param>
        /// <param name="hX">Specifies a handle to the GPU memory of shape (SeqLen, BatchSize, Inputs) containing the inputs.</param>
        /// <param name="hdX">Specifies a handle to the GPU memory of shape (SeqLen, BatchSize, Outputs) where the outbound, calculated gradients for X are placed.</param>
        /// <param name="hhX">Specifies a handle to the GPU memory of shape (BatchSize, Hidden) containing the hidden inputs.</param>
        /// <param name="hdhY">Specifies a handle to the GPU memory of shape (BatchSize, Hidden) containing the inbound gradients for hidden.</param>
        /// <param name="hdhX">Specifies a handle to the GPU memory of shape (BatchSize, Hidden) where the outbound, calculated gradients for hidden are placed.</param>
        /// <param name="hcX">Specifies a handle to the GPU memory of shape (BatchSize, Hidden) containing the hidden cell inputs.</param>
        /// <param name="hdcY">Specifies a handle to the GPU memory of shape (BatchSize, Hidden) containing the inbound sgradients for the cell hidden.</param>
        /// <param name="hdcX">Specifies a handle to the GPU memory of shape (BatchSize, Hidden) where the outbound, calculated gradients for cell hidden are placed.</param>
        /// <param name="hWt">Specifies a handle to the GPU memory of size szWt calculated with GetRnn8MemorySizes, containing the weights.</param>
        /// <param name="hdWt">Specifies a handle to the GPU memory of size szWt calculated with GetRnn8MemorySizes, where the weight gradients are placed.</param>
        /// <param name="hWork">Specifies a handle to the GPU memory of size szWork calculated with GetRnn8MemorySizes, used as temporary work data.</param>
        /// <param name="hReserved">Specifies a handle to the GPU memory of size szReserved calculated with GetRnn8MemorySizes, used as temporary reserve data.</param>
        public void Rnn8Backward(long hCuDnn, long hRnn, long hY, long hdY, long hX, long hdX, long hhX, long hdhY, long hdhX, long hcX, long hdcY, long hdcX, long hWt, long hdWt, long hWork, long hReserved)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.RNN8_BWD, null, m_param.AsLong(hCuDnn, hRnn, hY, hdY, hX, hdX, hhX, hdhY, hdhX, hcX, hdcY, hdcX, hWt, hdWt, hWork, hReserved));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.RNN8_BWD, null, m_param.AsLong(hCuDnn, hRnn, hY, hdY, hX, hdX, hhX, hdhY, hdhX, hcX, hdcY, hdcX, hWt, hdWt, hWork, hReserved));
        }


        /// <summary>
        /// Create the Attn.
        /// </summary>
        /// <returns>A handle to the Attn is returned.</returns>
        public long CreateAttn()
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.ATTN_CREATE, null);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.ATTN_CREATE, null);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free an existing Attn.
        /// </summary>
        /// <param name="h">Specifies the handle to the ATTN created with CreateAttn</param>
        public void FreeAttn(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.ATTN_FREE, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.ATTN_FREE, m_param.AsFloat(h));
        }

        /// <summary>
        /// Set the ATTN parameters.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hAttn">Specifies the handle to the ATTN created with CreateAttn.</param>
        /// <param name="nGpuID">Specifies the GPUID for which the attention is run.</param>
        /// <param name="bTraining">Specifies true for training and false for inference.</param>
        /// <param name="nBatch">Specifies the batch size.</param>
        /// <param name="nBlockSize">Specifies the block or sequence size.</param>
        /// <param name="nHeads">Specifies the number of heads.</param>
        /// <param name="nSize">Specifies the state size where nSize * nHeads = the Embedding size for the multi-headed attention layer.</param>
        /// <param name="fDropout">Specifies the dropout ratio used, or 0 to ignore.</param>
        /// <param name="lSeed">Optionally, specifies the dropout seed (default = 0 to ignore).</param>
        public void SetAttn(long hCuDnn, long hAttn, int nGpuID, bool bTraining, int nBatch, int nBlockSize, int nHeads, int nSize, float fDropout, long lSeed = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.ATTN_SET, m_param.AsDouble((double)fDropout), m_param.AsLong(hCuDnn, hAttn, nGpuID, (bTraining) ? 1 : 0, nBatch, nBlockSize, nHeads, nSize, lSeed));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.ATTN_SET, m_param.AsFloat(fDropout), m_param.AsLong(hCuDnn, hAttn, nGpuID, (bTraining) ? 1 : 0, nBatch, nBlockSize, nHeads, nSize, lSeed));
        }

        /// <summary>
        /// Calculate the scaled dot product attention forward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hAttn">Specifies the handle to the RNN8 created with CreateAttn.</param>
        /// <param name="nBlockSize">Specifies the block size which must be less than or equal to the block size set in the SetAttn call.</param>
        /// <param name="hQdata">Specifies a handle to the GPU memory containing the Q inputs.</param>
        /// <param name="hKdata">Specifies a handle to the GPU memory containing the K inputs.</param>
        /// <param name="hVdata">Specifies a handle to the GPU memory containing the V inputs.</param>
        /// <param name="hMaskdata">Specifies a handle to the GPU memory containing the Mask inputs, or 0 to ignore.</param>
        /// <param name="hYdata">Specifies a handle to the GPU memory containing the Y outputs.</param>
        /// <param name="bBatchMask">Specifies to use the batch mask mode (default = true).</param>
        public void AttnScaledDotProductForward(long hCuDnn, long hAttn, int nBlockSize, long hQdata, long hKdata, long hVdata, long hMaskdata, long hYdata, bool bBatchMask = true)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.ATTN_SCALED_DOT_PRODUCT_FWD, null, m_param.AsLong(hCuDnn, hAttn, nBlockSize, hQdata, hKdata, hVdata, hMaskdata, hYdata, (bBatchMask) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.ATTN_SCALED_DOT_PRODUCT_FWD, null, m_param.AsLong(hCuDnn, hAttn, nBlockSize, hQdata, hKdata, hVdata, hMaskdata, hYdata, (bBatchMask) ? 1 : 0));
        }

        /// <summary>
        /// Calculate the scaled dot product attention backward pass.
        /// </summary>
        /// <param name="hCuDnn">Specifies a handle to the instance of cuDnn.</param>
        /// <param name="hAttn">Specifies the handle to the RNN8 created with CreateAttn.</param>
        /// <param name="hQdata">Specifies a handle to the GPU memory containing the Q inputs.</param>
        /// <param name="hQdiff">Specifies a handle to the GPU memory containing the Q diff outputs.</param>
        /// <param name="hKdata">Specifies a handle to the GPU memory containing the K inputs.</param>
        /// <param name="hKdiff">Specifies a handle to the GPU memory containing the K diff outputs.</param>
        /// <param name="hVdata">Specifies a handle to the GPU memory containing the V inputs.</param>
        /// <param name="hVdiff">Specifies a handle to the GPU memory containing the V diff outputs.</param>
        /// <param name="hMaskdata">Specifies a handle to the GPU memory containing the Mask inputs, or 0 to ignore.</param>
        /// <param name="hYData">Specifies a handle to the GPU memory containing the Y data.</param>
        /// <param name="hYdiff">Specifies a handle to the GPU memory containing the Y diff inputs.</param>
        public void AttnScaledDotProductBackward(long hCuDnn, long hAttn, long hQdata, long hQdiff, long hKdata, long hKdiff, long hVdata, long hVdiff, long hMaskdata, long hYData, long hYdiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.ATTN_SCALED_DOT_PRODUCT_BWD, null, m_param.AsLong(hCuDnn, hAttn, hQdata, hQdiff, hKdata, hKdiff, hVdata, hVdiff, hMaskdata, hYData, hYdiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.ATTN_SCALED_DOT_PRODUCT_BWD, null, m_param.AsLong(hCuDnn, hAttn, hQdata, hQdiff, hKdata, hKdiff, hVdata, hVdiff, hMaskdata, hYData, hYdiff));
        }


        /// <summary>
        /// Create the CPD for change point detection primitives.
        /// </summary>
        /// <returns>A handle to the CPD is returned.</returns>
        public long CreateCpd()
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CPD_CREATE, null);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CPD_CREATE, null);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free an existing CPD.
        /// </summary>
        /// <param name="h">Specifies the handle to the CPD created with CreateCpd</param>
        public void FreeCpd(long h)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CPD_FREE, m_param.AsDouble(h));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CPD_FREE, m_param.AsFloat(h));
        }

        /// <summary>
        /// Set the CPD parameters.
        /// </summary>
        /// <param name="hCpd">Specifies the handle to the CPD created with CreateCpd.</param>
        /// <param name="nN">Specifies the number of items in the sequence.</param>
        /// <param name="nB">Specifies the clipping range for numerical stability (default = 10).</param>
        public void SetCpd(long hCpd, int nN, int nB = 10)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CPD_SET, null, m_param.AsLong(hCpd, nN, nB));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CPD_SET, null, m_param.AsLong(hCpd, nN, nB));
        }

        /// <summary>
        /// Compute the Cpd T value at a given position.
        /// </summary>
        /// <param name="hCpd">Specifies the handle to the CPD created with CreateCpd.</param>
        /// <param name="nT">Specifies the number of items in the curren t sequence.</param>
        /// <param name="nTau">Specifies the position within the sequence to calcualte the T value.</param>
        /// <param name="nZ">Specifies the number of items int the Z memory.</param>
        /// <param name="hZ">Specifies a handle to the GPU memory containing the Z data output from the Neural Net having the length of 't'.</param>
        /// <returns>The calculated T value is returned.</returns>
        public double ComputeCpdTvalueAt(long hCpd, int nT, int nTau, int nZ, long hZ)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CPD_COMPUTE_T_VALUE_AT, null, m_param.AsLong(hCpd, nT, nTau, nZ, hZ));
                return rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CPD_COMPUTE_T_VALUE_AT, null, m_param.AsLong(hCpd, nT, nTau, nZ, hZ));
                return rg[0];
            }
        }

        /// <summary>
        /// Compute the Cpd S values from the internal T matrix of values (previously filled with ComputeTvaluesAt).
        /// </summary>
        /// <param name="hCpd">Specifies the handle to the CPD created with CreateCpd.</param>
        /// <param name="nS">Specifies the number of items int the S memory.</param>
        /// <param name="hS">Specifies a handle to the GPU memory containing the S data where the output values are placed.  This memory shoudl be 'nN' in lenght as specified in the SetCpd method.</param>
        /// <param name="nT">Optionally, specifies the number of items in the hT matrix, or 0 to ignore (default = 0).</param>
        /// <param name="hT">Optionally, specifies a handle to the GPU memory containing the T data with shape (nN, nN, 1, 1).  When specified, this T matrix overrides the internal T matrix previously calculated when calling ComputeCpdTvalueAt.</param>
        public void ComputeCpdSvalues(long hCpd, int nS, long hS, int nT = 0, long hT = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CPD_COMPUTE_S_VALUES, null, m_param.AsLong(hCpd, nS, hS, nT, hT));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CPD_COMPUTE_S_VALUES, null, m_param.AsLong(hCpd, nS, hS, nT, hT));
        }

        /// <summary>
        /// Allocates the GPU memory for the PCA Data.
        /// </summary>
        /// <remarks>
        /// See [Parallel GPU Implementation of Iterative PCA Algorithms](https://arxiv.org/abs/0811.1081) by Mircea Andrecut
        /// </remarks>
        /// <param name="nM">Specifies the data width (number of rows).</param>
        /// <param name="nN">Specifies the data height (number of columns).</param>
        /// <param name="nK">Specifies the number of components (K &lt;= N).</param>
        /// <param name="nCount">Returns the total number of items in the allocated data (nM * nN).</param>
        /// <returns></returns>
        public long AllocPCAData(int nM, int nN, int nK, out int nCount)
        {
            nCount = nM * nN;
            return AllocMemory(nCount);
        }

        /// <summary>
        /// Allocates the GPU memory for the PCA scores.
        /// </summary>
        /// <remarks>
        /// See [Parallel GPU Implementation of Iterative PCA Algorithms](https://arxiv.org/abs/0811.1081) by Mircea Andrecut
        /// </remarks>
        /// <param name="nM">Specifies the data width (number of rows).</param>
        /// <param name="nN">Specifies the data height (number of columns).</param>
        /// <param name="nK">Specifies the number of components (K &lt;= N).</param>
        /// <param name="nCount">Returns the total number of items in the allocated data (nM * nN).</param>
        /// <returns></returns>
        public long AllocPCAScores(int nM, int nN, int nK, out int nCount)
        {
            nCount = nM * nK;
            return AllocMemory(nCount);
        }

        /// <summary>
        /// Allocates the GPU memory for the PCA loads.
        /// </summary>
        /// <remarks>
        /// See [Parallel GPU Implementation of Iterative PCA Algorithms](https://arxiv.org/abs/0811.1081) by Mircea Andrecut
        /// </remarks>
        /// <param name="nM">Specifies the data width (number of rows).</param>
        /// <param name="nN">Specifies the data height (number of columns).</param>
        /// <param name="nK">Specifies the number of components (K &lt;= N).</param>
        /// <param name="nCount">Returns the total number of items in the allocated data (nM * nN).</param>
        /// <returns></returns>
        public long AllocPCALoads(int nM, int nN, int nK, out int nCount)
        {
            nCount = nN * nK;
            return AllocMemory(nCount);
        }

        /// <summary>
        /// Allocates the GPU memory for the PCA eigenvalues.
        /// </summary>
        /// <remarks>
        /// See [Parallel GPU Implementation of Iterative PCA Algorithms](https://arxiv.org/abs/0811.1081) by Mircea Andrecut
        /// </remarks>
        /// <param name="nM">Specifies the data width (number of rows).</param>
        /// <param name="nN">Specifies the data height (number of columns).</param>
        /// <param name="nK">Specifies the number of components (K &lt;= N).</param>
        /// <param name="nCount">Returns the total number of items in the allocated data (nM * nN).</param>
        /// <returns></returns>
        public long AllocPCAEigenvalues(int nM, int nN, int nK, out int nCount)
        {
            nCount = nK * 1;
            return AllocHostBuffer(nCount);
        }

        /// <summary>
        /// Creates a new PCA instance and returns the handle to it.
        /// </summary>
        /// <remarks>
        /// See [Parallel GPU Implementation of Iterative PCA Algorithms](https://arxiv.org/abs/0811.1081) by Mircea Andrecut
        /// </remarks>
        /// <param name="nMaxIterations">Specifies the number of iterations to run.</param>
        /// <param name="nM">Specifies the data width (number of rows).</param>
        /// <param name="nN">Specifies the data height (number of columns).</param>
        /// <param name="nK">Specifies the number of components (K less than or equal to N).</param>
        /// <param name="hData">Specifies a handle to the data allocated using AllocatePCAData.</param>
        /// <param name="hScoresResult">Specifies a handle to the data allocated using AllocatePCAScores.</param>
        /// <param name="hLoadsResult">Specifies a handle to the data allocated using AllocatePCALoads.</param>
        /// <param name="hResiduals">Specifies a handle to the data allocated using AllocatePCAData.</param>
        /// <param name="hEigenvalues">Specifies a handle to the data allocated using AllocatePCAEigenvalues.</param>
        /// <returns></returns>
        public long CreatePCA(int nMaxIterations, int nM, int nN, int nK, long hData, long hScoresResult, long hLoadsResult, long hResiduals = 0, long hEigenvalues = 0)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CREATE_PCA, null, m_param.AsLong(nMaxIterations, nM, nN, nK, hData, hScoresResult, hLoadsResult, hResiduals, hEigenvalues));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CREATE_PCA, null, m_param.AsLong(nMaxIterations, nM, nN, nK, hData, hScoresResult, hLoadsResult, hResiduals, hEigenvalues));
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Runs a number of steps of the iterative PCA algorithm.
        /// </summary>
        /// <remarks>
        /// See [Parallel GPU Implementation of Iterative PCA Algorithms](https://arxiv.org/abs/0811.1081) by Mircea Andrecut
        /// </remarks>
        /// <param name="hPCA">Specifies a handle to the PCA instance to use.</param>
        /// <param name="nSteps">Specifies the number of steps to run.</param>
        /// <param name="nCurrentK">Returns the current component value.</param>
        /// <param name="nCurrentIteration">Returns the current iteration.</param>
        /// <returns><code>true</code> is returned when the maximum number of iterations have been run as specified in CreatePCA.</returns>
        public bool RunPCA(long hPCA, int nSteps, out int nCurrentK, out int nCurrentIteration)
        {
            bool bDone = false;

            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_RUN_PCA, null, m_param.AsLong(hPCA, nSteps));
                bDone = (rg[0] == 1.0) ? true : false;
                nCurrentIteration = (int)rg[1];
                nCurrentK = (int)rg[2];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_RUN_PCA, null, m_param.AsLong(hPCA, nSteps));
                bDone = (rg[0] == 1.0f) ? true : false;
                nCurrentIteration = (int)rg[1];
                nCurrentK = (int)rg[2];
            }

            return bDone;
        }

        /// <summary>
        /// Free the PCA instance associated with handle.
        /// </summary>
        /// <remarks>
        /// See [Parallel GPU Implementation of Iterative PCA Algorithms](https://arxiv.org/abs/0811.1081) by Mircea Andrecut
        /// </remarks>
        /// <param name="hPCA">Specifies a handle to the PCA instance to free.</param>
        public void FreePCA(long hPCA)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CUDA_FREE_PCA, m_param.AsDouble(hPCA));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CUDA_FREE_PCA, m_param.AsFloat(hPCA));
        }

        /// <summary>
        /// Create an instance of the SSD GPU support.
        /// </summary>
        /// <param name="nNumClasses">Specifies the number of classes.</param>
        /// <param name="bShareLocation">Specifies whether or not to share the location.</param>
        /// <param name="nLocClasses">Specifies the number of location classes.</param>
        /// <param name="nBackgroundLabelId">Specifies the background label ID.</param>
        /// <param name="bUseDiffcultGt">Specifies whether or not to use difficult ground truths.</param>
        /// <param name="miningType">Specifies the mining type to use.</param>
        /// <param name="matchType">Specifies the matching method to use.</param>
        /// <param name="fOverlapThreshold">Specifies the overlap threshold for each box.</param>
        /// <param name="bUsePriorForMatching">Specifies whether or not to use priors for matching.</param>
        /// <param name="codeType">Specifies the code type to use.</param>
        /// <param name="bEncodeVariantInTgt">Specifies whether or not to encode the variant in the target.</param>
        /// <param name="bBpInside">Specifies whether or not the BP is inside or not.</param>
        /// <param name="bIgnoreCrossBoundaryBbox">Specifies whether or not to ignore cross boundary boxes.</param>
        /// <param name="bUsePriorForNms">Specifies whether or not to use priors for NMS.</param>
        /// <param name="confLossType">Specifies the confidence loss type.</param>
        /// <param name="locLossType">Specifies the location loss type.</param>
        /// <param name="fNegPosRatio">Specifies the negative/positive ratio to use.</param>
        /// <param name="fNegOverlap">Specifies the negative overlap to use.</param>
        /// <param name="nSampleSize">Specifies the sample size.</param>
        /// <param name="bMapObjectToAgnostic">Specifies whether or not to map objects to agnostic or not.</param>
        /// <param name="bNmsParam">Specifies whether or not the NMS parameters are specified.</param>
        /// <param name="fNmsThreshold">Specifies the NMS threshold, which is only used when the 'bNmsParam' = true.</param>
        /// <param name="nNmsTopK">Specifies the NMS top-k selection, which is only used when the 'bNmsParam' = true.</param>
        /// <param name="fNmsEta">Specifies the NMS eta, which is only used when the 'bNmsParam' = true.</param>
        /// <returns>A handle to the SSD instance is returned.</returns>
        public long CreateSSD(int nNumClasses, bool bShareLocation, int nLocClasses, int nBackgroundLabelId, bool bUseDiffcultGt, SSD_MINING_TYPE miningType, SSD_MATCH_TYPE matchType, float fOverlapThreshold, bool bUsePriorForMatching, SSD_CODE_TYPE codeType, bool bEncodeVariantInTgt, bool bBpInside, bool bIgnoreCrossBoundaryBbox, bool bUsePriorForNms, SSD_CONF_LOSS_TYPE confLossType, SSD_LOC_LOSS_TYPE locLossType, float fNegPosRatio, float fNegOverlap, int nSampleSize, bool bMapObjectToAgnostic, bool bNmsParam, float? fNmsThreshold = null, int? nNmsTopK = null, float? fNmsEta = null)
        {
            int nGpuID = GetDeviceID();

            if (m_dt == DataType.DOUBLE)
            {
                List<double> rgArg = new List<double>();

                /* 0 */
                rgArg.Add(nGpuID);
                /* 1 */
                rgArg.Add(nNumClasses);
                /* 2 */
                rgArg.Add((bShareLocation) ? 1 : 0);
                /* 3 */
                rgArg.Add(nLocClasses);
                /* 4 */
                rgArg.Add(nBackgroundLabelId);
                /* 5 */
                rgArg.Add((bUseDiffcultGt) ? 1 : 0);
                /* 6 */
                rgArg.Add((int)miningType);
                /* 7 */
                rgArg.Add((int)matchType);
                /* 8 */
                rgArg.Add(fOverlapThreshold);
                /* 9 */
                rgArg.Add((bUsePriorForMatching) ? 1 : 0);
                /* 10 */
                rgArg.Add((int)codeType);
                /* 11 */
                rgArg.Add((bEncodeVariantInTgt) ? 1 : 0);
                /* 12 */
                rgArg.Add((bBpInside) ? 1 : 0);
                /* 13 */
                rgArg.Add((bIgnoreCrossBoundaryBbox) ? 1 : 0);
                /* 14 */
                rgArg.Add((bUsePriorForNms) ? 1 : 0);
                /* 15 */
                rgArg.Add((int)confLossType);
                /* 16 */
                rgArg.Add((int)locLossType);
                /* 17 */
                rgArg.Add(fNegPosRatio);
                /* 18 */
                rgArg.Add(fNegOverlap);
                /* 19 */
                rgArg.Add(nSampleSize);
                /* 20 */
                rgArg.Add((bMapObjectToAgnostic) ? 1 : 0);
                /* 21 */
                rgArg.Add((bNmsParam) ? 1 : 0);

                if (bNmsParam)
                {
                    if (!fNmsThreshold.HasValue)
                        throw new Exception("An NMS threshold must be specified when the 'bNmsParam' is true.");

                    /* 22 */
                    rgArg.Add(fNmsThreshold.GetValueOrDefault(0));
                    /* 23 */
                    rgArg.Add(nNmsTopK.GetValueOrDefault(-1));
                    /* 24 */
                    rgArg.Add(fNmsEta.GetValueOrDefault(1));
                }

                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CUDA_CREATE_SSD, rgArg.ToArray());
                return (long)rg[0];
            }
            else
            {
                List<float> rgArg = new List<float>();

                /* 0 */
                rgArg.Add(nGpuID);
                /* 1 */
                rgArg.Add(nNumClasses);
                /* 2 */
                rgArg.Add((bShareLocation) ? 1 : 0);
                /* 3 */
                rgArg.Add(nLocClasses);
                /* 4 */
                rgArg.Add(nBackgroundLabelId);
                /* 5 */
                rgArg.Add((bUseDiffcultGt) ? 1 : 0);
                /* 6 */
                rgArg.Add((int)miningType);
                /* 7 */
                rgArg.Add((int)matchType);
                /* 8 */
                rgArg.Add(fOverlapThreshold);
                /* 9 */
                rgArg.Add((bUsePriorForMatching) ? 1 : 0);
                /* 10 */
                rgArg.Add((int)codeType);
                /* 11 */
                rgArg.Add((bEncodeVariantInTgt) ? 1 : 0);
                /* 12 */
                rgArg.Add((bBpInside) ? 1 : 0);
                /* 13 */
                rgArg.Add((bIgnoreCrossBoundaryBbox) ? 1 : 0);
                /* 14 */
                rgArg.Add((bUsePriorForNms) ? 1 : 0);
                /* 15 */
                rgArg.Add((int)confLossType);
                /* 16 */
                rgArg.Add((int)locLossType);
                /* 17 */
                rgArg.Add(fNegPosRatio);
                /* 18 */
                rgArg.Add(fNegOverlap);
                /* 19 */
                rgArg.Add(nSampleSize);
                /* 20 */
                rgArg.Add((bMapObjectToAgnostic) ? 1 : 0);
                /* 21 */
                rgArg.Add((bNmsParam) ? 1 : 0);

                if (bNmsParam)
                {
                    if (!fNmsThreshold.HasValue)
                        throw new Exception("An NMS threshold must be specified when the 'bNmsParam' is true.");

                    /* 22 */
                    rgArg.Add(fNmsThreshold.GetValueOrDefault(0));
                    /* 23 */
                    rgArg.Add(nNmsTopK.GetValueOrDefault(-1));
                    /* 24 */
                    rgArg.Add(fNmsEta.GetValueOrDefault(1));
                }

                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CUDA_CREATE_SSD, rgArg.ToArray());
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Setup the SSD GPU support.
        /// </summary>
        /// <param name="hSSD">Specifies the handle to the SSD instance.</param>
        /// <param name="nNum">Specifies the number of items.</param>
        /// <param name="nNumPriors">Specifies the number of priors.</param>
        /// <param name="nNumGt">Specifies the number of ground truths.</param>
        public void SetupSSD(long hSSD, int nNum, int nNumPriors, int nNumGt)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CUDA_SETUP_SSD, m_param.AsDouble(hSSD, nNum, nNumPriors, nNumGt));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CUDA_SETUP_SSD, m_param.AsFloat(hSSD, nNum, nNumPriors, nNumGt));
        }

        /// <summary>
        /// Free the instance of SSD GPU support.
        /// </summary>
        /// <param name="hSSD">Specifies the handle to the SSD instance.</param>
        public void FreeSSD(long hSSD)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CUDA_FREE_SSD, m_param.AsDouble(hSSD));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CUDA_FREE_SSD, m_param.AsFloat(hSSD));
        }

        /// <summary>
        /// Performs the SSD MultiBoxLoss forward operation.
        /// </summary>
        /// <param name="hSSD">Specifies the handle to the SSD instance.</param>
        /// <param name="nLocDataCount">Specifies the number of location data items.</param>
        /// <param name="hLocGpuData">Specifies the handle to the location data in GPU memory.</param>
        /// <param name="nConfDataCount">Specifies the number of confidence data items.</param>
        /// <param name="hConfGpuData">Specifies the handle to the confidence data in GPU memory.</param>
        /// <param name="nPriorDataCount">Specifies the number of prior box data.</param>
        /// <param name="hPriorGpuData">Specifies the prior box data in GPU memory.</param>
        /// <param name="nGtDataCount">Specifies the number of ground truth items.</param>
        /// <param name="hGtGpuData">Specifies the ground truth data in GPU memory.</param>
        /// <param name="rgAllMatchIndices">Returns all match indices found.</param>
        /// <param name="rgrgAllNegIndices">Returns all neg indices found.</param>
        /// <param name="nNumNegs">Returns the number of negatives.</param>
        /// <returns>The number of matches is returned.</returns>
        public int SsdMultiBoxLossForward(long hSSD, int nLocDataCount, long hLocGpuData, int nConfDataCount, long hConfGpuData, int nPriorDataCount, long hPriorGpuData, int nGtDataCount, long hGtGpuData, out List<DictionaryMap<List<int>>> rgAllMatchIndices, out List<List<int>> rgrgAllNegIndices, out int nNumNegs)
        {
            int nIdx = 0;
            int nMatchCount = 0;
            rgAllMatchIndices = new List<DictionaryMap<List<int>>>();
            rgrgAllNegIndices = new List<List<int>>();

            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SSD_FWD_MULTIBOXLOSS, null, m_param.AsLong(hSSD, nLocDataCount, hLocGpuData, nConfDataCount, hConfGpuData, nPriorDataCount, hPriorGpuData, nGtDataCount, hGtGpuData));
                nMatchCount = (int)rg[nIdx];
                nIdx++;
                nNumNegs = (int)rg[nIdx];
                nIdx++;

                // Get the match indices.
                int nNumAllMatchIndices = (int)rg[nIdx];
                nIdx++;
                for (int i = 0; i < nNumAllMatchIndices; i++)
                {
                    DictionaryMap<List<int>> map = new DictionaryMap<List<int>>(null);

                    int nMapCount = (int)rg[nIdx];
                    nIdx++;
                    for (int j = 0; j < nMapCount; j++)
                    {
                        int nLabel = (int)rg[nIdx];
                        nIdx++;
                        List<int> rgIdx = new List<int>();

                        int nItemCount = (int)rg[nIdx];
                        nIdx++;
                        for (int k = 0; k < nItemCount; k++)
                        {
                            int nItemIdx = (int)rg[nIdx];
                            nIdx++;
                            rgIdx.Add(nItemIdx);
                        }

                        map[nLabel] = rgIdx;
                    }

                    rgAllMatchIndices.Add(map);
                }

                // Get the neg indices.
                int nNegListCount = (int)rg[nIdx];
                nIdx++;
                for (int i = 0; i < nNegListCount; i++)
                {
                    int nItemCount = (int)rg[nIdx];
                    nIdx++;
                    List<int> rgItems = new List<int>();

                    for (int j = 0; j < nItemCount; j++)
                    {
                        int nItemIdx = (int)rg[nIdx];
                        nIdx++;
                        rgItems.Add(nItemIdx);
                    }

                    rgrgAllNegIndices.Add(rgItems);
                }
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SSD_FWD_MULTIBOXLOSS, null, m_param.AsLong(hSSD, nLocDataCount, hLocGpuData, nConfDataCount, hConfGpuData, nPriorDataCount, hPriorGpuData, nGtDataCount, hGtGpuData));
                nMatchCount = (int)rg[nIdx];
                nIdx++;
                nNumNegs = (int)rg[nIdx];
                nIdx++;

                // Get the match indices.
                int nMapListCount = (int)rg[nIdx];
                nIdx++;
                for (int i = 0; i < nMapListCount; i++)
                {
                    DictionaryMap<List<int>> map = new DictionaryMap<List<int>>(null);

                    int nMapCount = (int)rg[nIdx];
                    nIdx++;
                    for (int j = 0; j < nMapCount; j++)
                    {
                        int nLabel = (int)rg[nIdx];
                        nIdx++;
                        List<int> rgIdx = new List<int>();

                        int nItemCount = (int)rg[nIdx];
                        nIdx++;
                        for (int k = 0; k < nItemCount; k++)
                        {
                            int nItemIdx = (int)rg[nIdx];
                            nIdx++;
                            rgIdx.Add(nItemIdx);
                        }

                        map[nLabel] = rgIdx;
                    }

                    rgAllMatchIndices.Add(map);
                }

                // Get the neg indices.
                int nNegListCount = (int)rg[nIdx];
                nIdx++;
                for (int i = 0; i < nNegListCount; i++)
                {
                    int nItemCount = (int)rg[nIdx];
                    nIdx++;
                    List<int> rgItems = new List<int>();

                    for (int j = 0; j < nItemCount; j++)
                    {
                        int nItemIdx = (int)rg[nIdx];
                        nIdx++;
                        rgItems.Add(nItemIdx);
                    }

                    rgrgAllNegIndices.Add(rgItems);
                }
            }

            return nMatchCount;
        }

        /// <summary>
        /// Encodes the SSD data into the location prediction and location ground truths.
        /// </summary>
        /// <param name="hSSD">Specifies the handle to the SSD instance.</param>
        /// <param name="nLocPredCount">Specifies the number of location prediction items.</param>
        /// <param name="hLocPred">Specifies the location prediction data in GPU memory.</param>
        /// <param name="nLocGtCount">Specifies the location ground truth items.</param>
        /// <param name="hLocGt">Specifies the location ground truth data in GPU memory.</param>
        public void SsdEncodeLocPrediction(long hSSD, int nLocPredCount, long hLocPred, int nLocGtCount, long hLocGt)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SSD_ENCODE_LOCPRED, null, m_param.AsLong(hSSD, nLocPredCount, hLocPred, nLocGtCount, hLocGt));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SSD_ENCODE_LOCPRED, null, m_param.AsLong(hSSD, nLocPredCount, hLocPred, nLocGtCount, hLocGt));
        }

        /// <summary>
        /// Encodes the SSD data into the confidence prediction and confidence ground truths.
        /// </summary>
        /// <param name="hSSD">Specifies the handle to the SSD instance.</param>
        /// <param name="nConfPredCount">Specifies the number of confidence prediction items.</param>
        /// <param name="hConfPred">Specifies the confidence prediction data in GPU memory.</param>
        /// <param name="nConfGtCount">Specifies the confidence ground truth items.</param>
        /// <param name="hConfGt">Specifies the confidence ground truth data in GPU memory.</param>
        public void SsdEncodeConfPrediction(long hSSD, int nConfPredCount, long hConfPred, int nConfGtCount, long hConfGt)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SSD_ENCODE_CONFPRED, null, m_param.AsLong(hSSD, nConfPredCount, hConfPred, nConfGtCount, hConfGt));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SSD_ENCODE_CONFPRED, null, m_param.AsLong(hSSD, nConfPredCount, hConfPred, nConfGtCount, hConfGt));
        }

        /// <summary>
        /// Create the Cuda version of LayerNorm
        /// </summary>
        /// <param name="nGpuID">Specifies the GPUID to use.</param>
        /// <param name="nCount">Specifies the total number of items in the input (and output).</param>
        /// <param name="nOuterNum">Specifies the outer number of items (e.g., num)</param>
        /// <param name="nChannels">Specifies the number of channels in the data.</param>
        /// <param name="nInnerNum">Specifies the spatial dimentions of the inner data.</param>
        /// <param name="fEps">Optionally, specifies the epsilon value to avoid numeric issues (default = 1e-10).</param>
        /// <returns>The handle to the LayerNorm configuration.  This handle is used with all other layer norm functions.</returns>
        public long CreateLayerNorm(int nGpuID, int nCount, int nOuterNum, int nChannels, int nInnerNum, float fEps = 1e-10f)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CREATE_LAYERNORM, m_param.AsDouble(fEps), m_param.AsLong(nGpuID, nCount, nOuterNum, nChannels, nInnerNum));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CREATE_LAYERNORM, m_param.AsFloat(fEps), m_param.AsLong(nGpuID, nCount, nOuterNum, nChannels, nInnerNum));
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free the instance of LayerNorm GPU support.
        /// </summary>
        /// <param name="hLayerNorm">Specifies the handle to the LayerNorm instance.</param>
        public void FreeLayerNorm(long hLayerNorm)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CUDA_FREE_LAYERNORM, m_param.AsDouble(hLayerNorm));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CUDA_FREE_LAYERNORM, m_param.AsFloat(hLayerNorm));
        }

        /// <summary>
        /// Run the LayerNorm forward pass.
        /// </summary>
        /// <param name="hLayerNorm">Specifies the handle to the LayerNorm instance.</param>
        /// <param name="hXdata">Specifies the input data to be normalized.</param>
        /// <param name="hYdata">Specifies the normalized output data.</param>
        public void LayerNormForward(long hLayerNorm, long hXdata, long hYdata)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LAYERNORM_FWD, null, m_param.AsLong(hLayerNorm, hXdata, hYdata));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LAYERNORM_FWD, null, m_param.AsLong(hLayerNorm, hXdata, hYdata));
        }

        /// <summary>
        /// Run the LayerNorm backward pass.
        /// </summary>
        /// <param name="hLayerNorm">Specifies the handle to the LayerNorm instance.</param>
        /// <param name="hYdata">Specifies the normalized output data.</param>
        /// <param name="hYdiff">Specifies the input diff to be un-normalized.</param>
        /// <param name="hXdiff">Specifies the un-normalized output diff.</param>
        public void LayerNormBackward(long hLayerNorm, long hYdata, long hYdiff, long hXdiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LAYERNORM_BWD, null, m_param.AsLong(hLayerNorm, hYdata, hYdiff, hXdiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LAYERNORM_BWD, null, m_param.AsLong(hLayerNorm, hYdata, hYdiff, hXdiff));
        }


        /// <summary>
        /// Create the Cuda version of RoPE
        /// </summary>
        /// <param name="nSharedIndex">Specifies the shared index used so that all layers use the same RoPE. To use a unique RoPE set this value to -1.</param>
        /// <param name="nGpuID">Specifies the GPUID to use.</param>
        /// <param name="nCount">Specifies the total number of items in the input (and output).</param>
        /// <param name="nBatch">Specifies the batch size of items (e.g., num)</param>
        /// <param name="nSeqLen">Specifies the sequence length used.</param>
        /// <param name="nHeads">Specifies the number of heads used.</param>
        /// <param name="nDim">Specifies the spatial dimentions of the inner data.</param>
        /// <param name="fTheta">Optionally, specifies the theta value used in the rope calculation (default = 10000f).</param>
        /// <returns>The handle to the RoPE configuration.  This handle is used with all other RoPE functions.</returns>
        public long CreateRope(int nSharedIndex, int nGpuID, int nCount, int nBatch, int nSeqLen, int nHeads, int nDim, float fTheta = 10000.0f)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CREATE_ROPE, m_param.AsDouble(fTheta), m_param.AsLong(nSharedIndex, nGpuID, nCount, nBatch, nSeqLen, nHeads, nDim));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CREATE_ROPE, m_param.AsFloat(fTheta), m_param.AsLong(nSharedIndex, nGpuID, nCount, nBatch, nSeqLen, nHeads, nDim));
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free the instance of Rope GPU support.
        /// </summary>
        /// <param name="hRope">Specifies the handle to the Rope instance.</param>
        public void FreeRope(long hRope)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CUDA_FREE_ROPE, m_param.AsDouble(hRope));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CUDA_FREE_ROPE, m_param.AsFloat(hRope));
        }

        /// <summary>
        /// Run the Rope forward pass.
        /// </summary>
        /// <param name="hRope">Specifies the handle to the Rope instance.</param>
        /// <param name="n">Specifies the number of items in hYdata, hYdiff and hXdiff.</param>
        /// <param name="hXdata">Specifies the input data.</param>
        /// <param name="hYdata">Specifies the output data with added rope positional embedding.</param>
        /// <param name="nFreqOffset">Specifies a frequency offset to use within the rope positional embedding.</param>
        public void RopeForward(long hRope, int n, long hXdata, long hYdata, int nFreqOffset = -1)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ROPE_FWD, null, m_param.AsLong(hRope, n, hXdata, hYdata, nFreqOffset));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ROPE_FWD, null, m_param.AsLong(hRope, n, hXdata, hYdata, nFreqOffset));
        }

        /// <summary>
        /// Run the Rope backward pass.
        /// </summary>
        /// <param name="hRope">Specifies the handle to the Rope instance.</param>
        /// <param name="n">Specifies the number of items in hYdata, hYdiff and hXdiff.</param>
        /// <param name="hXdata">Specifies the rope augmented X input sent to the Forward pass.</param>
        /// <param name="hYdiff">Specifies the input diff to be un-roped.</param>
        /// <param name="hXdiff">Specifies the un-roped output diff.</param>
        public void RopeBackward(long hRope, int n, long hXdata, long hYdiff, long hXdiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ROPE_BWD, null, m_param.AsLong(hRope, n, hXdata, hYdiff, hXdiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ROPE_BWD, null, m_param.AsLong(hRope, n, hXdata, hYdiff, hXdiff));
        }

        /// <summary>
        /// Create the Cuda Blob loader used to load Blobs from large weight files.
        /// </summary>
        /// <param name="strFile">Specifies the weight file to load.</param>
        /// <returns>The handle to the BlobLoader is returned.  This handle is used with all other blob loader functions.</returns>
        public long CreateBlobLoader(string strFile)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx((int)m_hKernel, (int)CUDAFN.CUDA_CREATE_BLOBLOADER, null, strFile);
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx((int)m_hKernel, (int)CUDAFN.CUDA_CREATE_BLOBLOADER, null, strFile);
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free the instance of the BlobLoader GPU support.
        /// </summary>
        /// <param name="hBlobLoader">Specifies the handle to the BlobLoader instance.</param>
        public void FreeBlobLoader(long hBlobLoader)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CUDA_FREE_BLOBLOADER, m_param.AsDouble(hBlobLoader));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CUDA_FREE_BLOBLOADER, m_param.AsFloat(hBlobLoader));
        }

        /// <summary>
        /// Load a Blob GPU memory using the BlobLoader by reading the previously loaded file starting at the lOffset number of bytes and
        /// reading nCount number of items into the GPU memory at hDst.
        /// </summary>
        /// <param name="hBlobLoader">Specifies a handle to the Blob loader.</param>
        /// <param name="lCount">Specifies the number of items to read.</param>
        /// <param name="hDst">Specifies the destination memory for the data read.</param>
        /// <param name="lLocalOffsetInBytes">Specifies the local offset in bytes that is added to the internal offset, but does not alter the internal offset index.</param>
        public void LoadBlob(long hBlobLoader, long lCount, long hDst, long lLocalOffsetInBytes)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_BLOBLOADER_LOAD, null, m_param.AsLong(hBlobLoader, lCount, hDst, lLocalOffsetInBytes));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_BLOBLOADER_LOAD, null, m_param.AsLong(hBlobLoader, lCount, hDst, lLocalOffsetInBytes));
        }

        /// <summary>
        /// Reset the starting offset used to read the underlying file int the Blob.
        /// </summary>
        /// <param name="hBlobLoader">Specifies a handle to the Blob loader.</param>
        /// <param name="lOffsetInBytes">Specifies the starting offset in bytes where the next LoadBlob will start reading from the file.</param>
        public void BlobLoaderResetOffset(long hBlobLoader, long lOffsetInBytes)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_BLOBLOADER_RESETOFFSET, null, m_param.AsLong(hBlobLoader, lOffsetInBytes));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_BLOBLOADER_RESETOFFSET, null, m_param.AsLong(hBlobLoader, lOffsetInBytes));
        }

        /// <summary>
        /// Adds to the starting offset used to read the underlying file int the Blob.
        /// </summary>
        /// <param name="hBlobLoader">Specifies a handle to the Blob loader.</param>
        /// <param name="lOffsetInItems">Specifies the amount to add to the current offset counted in items.</param>
        public void BlobLoaderAddToOffset(long hBlobLoader, long lOffsetInItems)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_BLOBLOADER_ADDTOOFFSET, null, m_param.AsLong(hBlobLoader, lOffsetInItems));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_BLOBLOADER_ADDTOOFFSET, null, m_param.AsLong(hBlobLoader, lOffsetInItems));
        }


        /// <summary>
        /// Create the Cudnn Fused Compute used to perform complex 'fused' computations.
        /// </summary>
        /// <param name="nSharedIndex">Specifies a shared global index used to share computes, or -1 to ignore.</param>
        /// <param name="hCuda">Specifies a handle to cudnn.</param>
        /// <param name="nGpuID">Specifies the GPUID on which memory is allocated.</param>
        /// <param name="dtIo">Specifies the data type used for input and output.</param>
        /// <param name="dtIntermediate">Specifies the data type used for the intermediate tensors.</param>
        /// <param name="dtCompute">Specifies the data type used for compute.</param>
        /// <param name="prebuilt">Optionally, specifies a pre-built fused compute to use.</param>
        /// <returns>The handle to the FusedCompute is returned.  This handle is used with all other fused compute functions.</returns>
        public long CreateFusedCompute(long nSharedIndex, long hCuda, int nGpuID, FUSEDCOMPUTE_DATA_TYPE dtIo, FUSEDCOMPUTE_DATA_TYPE dtIntermediate, FUSEDCOMPUTE_DATA_TYPE dtCompute, FUSEDCOMPUTE_PREBUILT_OP prebuilt = FUSEDCOMPUTE_PREBUILT_OP.NONE)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CREATE_FUSEDCOMP, null, m_param.AsLong(nSharedIndex, hCuda, nGpuID, (int)dtIo, (int)dtIntermediate, (int)dtCompute, (int)prebuilt));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CREATE_FUSEDCOMP, null, m_param.AsLong(nSharedIndex, hCuda, nGpuID, (int)dtIo, (int)dtIntermediate, (int)dtCompute, (int)prebuilt));
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Free the instance of the FusedCompute.
        /// </summary>
        /// <param name="hFusedCompute">Specifies the handle to the FusedComp instance.</param>
        public void FreeFusedCompute(long hFusedCompute)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CUDA_FREE_FUSEDCOMP, m_param.AsDouble(hFusedCompute));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CUDA_FREE_FUSEDCOMP, m_param.AsFloat(hFusedCompute));
        }

        /// <summary>
        /// Create a new fused computation tensor.
        /// </summary>
        /// <param name="hFusedCompute">Specifies the handle to the FusedComp instance.</param>
        /// <param name="dt">Specifies the data type of the tensor.</param>
        /// <param name="nN">Specifies the number of items in the N dimension.</param>
        /// <param name="nC">Specifies the number of items in the C dimension.</param>
        /// <param name="nH">Specifies the number of items in the H dimension.</param>
        /// <param name="nW">Specifies the number of items in the W dimension.</param>
        /// <param name="bTranspose">Specifies to transpose the tensor before using.  Note when true, a tensor workspace with the same size as the tensor must be allocated and sent to Execute.</param>
        /// <param name="hTensorWorkspace">Returns a handle to the tensor workspace GPU memory, or 0 if not used.  Free this memory with the FreeMemory method.</param>
        /// <returns>A handle to the new tensor is returned.</returns>
        public long FusedCompAddTensor(long hFusedCompute, FUSEDCOMPUTE_DATA_TYPE dt, int nN, int nC, int nH, int nW, bool bTranspose, out long hTensorWorkspace)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_FUSED_COMP_ADD_TENSOR, null, m_param.AsLong(hFusedCompute, (long)dt, nN, nC, nH, nW, (bTranspose) ? 1 : 0));
                hTensorWorkspace = (long)rg[1];
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_FUSED_COMP_ADD_TENSOR, null, m_param.AsLong(hFusedCompute, (long)dt, nN, nC, nH, nW, (bTranspose) ? 1 : 0));
                hTensorWorkspace = (long)rg[1];
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Get the data from a fused computation tensor and copy it to the destination.
        /// </summary>
        /// <param name="hFusedCompute">Specifies the handle to the FusedComp instance.</param>
        /// <param name="hTensor">Specifies a handle to a fused comp tensor.</param>
        /// <param name="dt">Returns the tensor data type.</param>
        /// <param name="bTranspose">Returns whether or not the tensor is to be transposed first.</param>
        /// <returns>Returns the shape of the tensor.</returns>
        public List<int> FusedCompGetTensor(long hFusedCompute, long hTensor, out FUSEDCOMPUTE_DATA_TYPE dt, out bool bTranspose)
        {
            List<int> rgShape = new List<int>();

            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_FUSED_COMP_GET_TENSOR, null, m_param.AsLong(hFusedCompute, hTensor));
                dt = (FUSEDCOMPUTE_DATA_TYPE)(int)rg[0];
                bTranspose = (rg[1] == 0) ? false : true;

                rgShape.Add((int)rg[2]);
                if ((int)rg[3] > 0)
                    rgShape.Add((int)rg[3]);
                if ((int)rg[4] > 0)
                    rgShape.Add((int)rg[4]);
                if ((int)rg[5] > 0)
                    rgShape.Add((int)rg[5]);
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_FUSED_COMP_GET_TENSOR, null, m_param.AsLong(hFusedCompute, hTensor));
                dt = (FUSEDCOMPUTE_DATA_TYPE)(int)rg[0];
                bTranspose = (rg[1] == 0) ? false : true;

                rgShape.Add((int)rg[2]);
                if ((int)rg[3] > 0)
                    rgShape.Add((int)rg[3]);
                if ((int)rg[4] > 0)
                    rgShape.Add((int)rg[4]);
                if ((int)rg[5] > 0)
                    rgShape.Add((int)rg[5]);
            }

            return rgShape;
        }

        /// <summary>
        /// Add a new fused operation to be performed on the tensors.
        /// </summary>
        /// <param name="hFusedCompute">Specifies the handle to the FusedComp instance.</param>
        /// <param name="op">Specifies the fused computation operation.</param>
        /// <param name="dtCompute">Specifies the compute of the operation.</param>
        /// <param name="dfPad">Specifies the padding to use.</param>
        /// <param name="hTensor1">Specifies the first tensor to operate on.</param>
        /// <param name="hTensor2">Specifies the second tensor to operate on or 0 if not used.</param>
        /// <param name="hTensor3">Specifies the third tensor to operate on or 0 if not used.</param>
        /// <param name="hTensor4">Specifies the fourth tensor to operate on or 0 if not used.</param>
        /// <returns>The intermediate tensor from the operation is returned.</returns>
        public long FusedCompAddOp(long hFusedCompute, FUSEDCOMPUTE_OP op, FUSEDCOMPUTE_DATA_TYPE dtCompute, double dfPad, long hTensor1, long hTensor2 = 0, long hTensor3 = 0, long hTensor4 = 0)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_FUSED_COMP_ADD_OP, m_param.AsDouble(dfPad), m_param.AsLong(hFusedCompute, (int)op, (int)dtCompute, hTensor1, hTensor2, hTensor3, hTensor4));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_FUSED_COMP_ADD_OP, m_param.AsFloat((float)dfPad), m_param.AsLong(hFusedCompute, (int)op, (int)dtCompute, hTensor1, hTensor2, hTensor3, hTensor4));
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Build a new fused computation graph.
        /// </summary>
        /// <param name="hFusedCompute">Specifies the handle to the FusedComp instance.</param>
        /// <param name="heur1">Specifies the first heuristics to use.</param>
        /// <param name="heur2">Specifies the second heuristics to use.</param>
        /// <param name="nLocalID">Optionally, specifies the local ID used for caching graphs.</param>
        /// <returns>Returns the workspace size needed (in bytes) to execute the computation.</returns>
        public long FusedCompBuild(long hFusedCompute, FUSEDCOMP_HEUR_MODE heur1, FUSEDCOMP_HEUR_MODE heur2, int nLocalID = -1)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_FUSED_COMP_BUILD, null, m_param.AsLong(hFusedCompute, nLocalID, (int)heur1, (int)heur2));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_FUSED_COMP_BUILD, null, m_param.AsLong(hFusedCompute, nLocalID, (int)heur1, (int)heur2));
                return (long)rg[0];
            }
        }

        /// <summary>
        /// Execute a fused computation graph.
        /// </summary>
        /// <param name="hFusedCompute">Specifies the handle to the FusedComp instance.</param>
        /// <param name="hWorkspace">Specifies the GPU workspace memory returned from the FusedCompBuild or Create method (when using a pre-built computation).</param>
        /// <param name="rghTensors">Specifies a list of handles to tensors created with the FusedCompAddTensor method.</param>
        /// <param name="rghTensorData">Specifies a list of handles to the tensor GPU data created with AllocMem.</param>
        /// <param name="rghTensorWorkspaceData">Specifies the handles tot he tensor GPU workspace data required on any tensors that are transformed.  Each workspace should be the size in items of the tensor to be transformed.</param>
        /// <param name="nLocalID">Optionally, specifies the local ID used for caching graphs.</param>
        /// <remarks>Note the list of tensors and list of tensor data must be of the same count.</remarks>
        public void FusedCompExecute(long hFusedCompute, long hWorkspace, List<long> rghTensors, List<long> rghTensorData, List<long> rghTensorWorkspaceData, int nLocalID = -1)
        {
            if (rghTensorData.Count != rghTensors.Count)
                throw new Exception("The number of tensors and tensor data must match.");

            if (rghTensorWorkspaceData.Count != rghTensors.Count)
                throw new Exception("The number of tensors and tensor workspace data must match.");

            if (rghTensorData.Count == 0)
                throw new Exception("At least one tensor must be provided.");

            List<long> rgArgs = new List<long>();
            rgArgs.Add(hFusedCompute);
            rgArgs.Add(nLocalID);
            rgArgs.Add(hWorkspace);
            rgArgs.Add(rghTensors.Count);
            rgArgs.AddRange(rghTensors);
            rgArgs.AddRange(rghTensorData);
            rgArgs.AddRange(rghTensorWorkspaceData);

            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_FUSED_COMP_EXECUTE, null, m_param.AsLong(rgArgs.ToArray()));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_FUSED_COMP_EXECUTE, null, m_param.AsLong(rgArgs.ToArray()));
        }

        #endregion

        //---------------------------------------------------------------------
        //  ICudaMath Methods
        //---------------------------------------------------------------------
        #region ICudaMath Methods

        /// <summary>
        /// Set the values of GPU memory to a specified value of type <code>double</code>.
        /// </summary>
        /// <param name="nCount">Specifies the number of items to set.</param>
        /// <param name="hHandle">Specifies a handle to the memory on the GPU.</param>
        /// <param name="fVal">Specifies the value to set.</param>
        /// <param name="nIdx">When -1, all values in the GPU memory are set to the <i>fVal</i> value, otherwise, only the value at the index <i>nIdx</i> is set to the value.</param>
        public void set(int nCount, long hHandle, double fVal, int nIdx = -1)
        {
            set(nCount, hHandle, (T)Convert.ChangeType(fVal, typeof(T)), nIdx);
        }

        /// <summary>
        /// Set the values of GPU memory to a specified value of type <code>float</code>.
        /// </summary>
        /// <param name="nCount">Specifies the number of items to set.</param>
        /// <param name="hHandle">Specifies a handle to the memory on the GPU.</param>
        /// <param name="fVal">Specifies the value to set.</param>
        /// <param name="nIdx">When -1, all values in the GPU memory are set to the <i>fVal</i> value, otherwise, only the value at the index <i>nIdx</i> is set to the value.</param>
        public void set(int nCount, long hHandle, float fVal, int nIdx = -1)
        {
            set(nCount, hHandle, (T)Convert.ChangeType(fVal, typeof(T)), nIdx);
        }

        /// <summary>
        /// Set the values of GPU memory to a specified value of type 'T'.
        /// </summary>
        /// <param name="nCount">Specifies the number of items to set.</param>
        /// <param name="hHandle">Specifies a handle to the memory on the GPU.</param>
        /// <param name="fVal">Specifies the value to set.</param>
        /// <param name="nIdx">When -1, all values in the GPU memory are set to the <i>fVal</i> value, otherwise, only the value at the index <i>nIdx</i> is set to the value.</param>
        /// <param name="nXOff">Optionally specifies an offset into the GPU memory where the <i>set</i> starts.</param>
        public void set(int nCount, long hHandle, T fVal, int nIdx = -1, int nXOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
            {
                if (m_rgGhostMemory == null)
                {
                    m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SET, m_param.AsDouble(convertD(fVal)), m_param.AsLong(nCount, hHandle, 0, nIdx, nXOff));
                }
                else
                {
                    if (nIdx >= 0)
                        m_rgGhostMemory[hHandle][nIdx] = fVal;
                    else
                        Utility.Set<T>(m_rgGhostMemory[hHandle], fVal);
                }
            }
            else
            {
                if (m_rgGhostMemory == null)
                {
                    m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SET, m_param.AsFloat(convertF(fVal)), m_param.AsLong(nCount, hHandle, 0, nIdx, nXOff));
                }
                else
                {
                    if (nIdx >= 0)
                        m_rgGhostMemory[hHandle][nIdx] = fVal;
                    else
                        Utility.Set<T>(m_rgGhostMemory[hHandle], fVal);
                }
            }
        }

        /// <summary>
        /// Queries the GPU memory by copying it into an array of <code>double</code>
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hHandle">Specifies a handle to GPU memory.</param>
        /// <param name="nIdx">When -1, all values in the GPU memory are queried, otherwise, only the value at the index <i>nIdx</i> is returned.</param>
        /// <returns>An array of <code>double</code> is returned.</returns>
        public double[] get_double(int nCount, long hHandle, int nIdx = -1)
        {
            return convertD(get(nCount, hHandle, nIdx));
        }

        /// <summary>
        /// Queries the GPU memory by copying it into an array of <code>float</code>
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hHandle">Specifies a handle to GPU memory.</param>
        /// <param name="nIdx">When -1, all values in the GPU memory are queried, otherwise, only the value at the index <i>nIdx</i> is returned.</param>
        /// <returns>An array of <code>float</code> is returned.</returns>
        public float[] get_float(int nCount, long hHandle, int nIdx = -1)
        {
            return convertF(get(nCount, hHandle, nIdx));
        }

        /// <summary>
        /// Queries the GPU memory by copying it into an array of type 'T'.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hHandle">Specifies a handle to GPU memory.</param>
        /// <param name="nIdx">When -1, all values in the GPU memory are queried, otherwise, only the value at the index <i>nIdx</i> is returned.</param>
        /// <returns>An array of <code>T</code> is returned.</returns>
        public T[] get(int nCount, long hHandle, int nIdx = -1)
        {
            if (m_dt == DataType.DOUBLE)
                return convert(m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_GET, null, m_param.AsLong(nCount, hHandle, nIdx)));
            else
                return convert(m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_GET, null, m_param.AsLong(nCount, hHandle, nIdx)));
        }

        /// <summary>
        /// Copy data from one block of GPU memory to another.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items (not bytes) to copy.</param>
        /// <param name="hSrc">Specifies a handle to GPU memory containing the source data.</param>
        /// <param name="hDst">Specifies a handle to GPU memory containing the destination data.</param>
        /// <param name="nSrcOffset">Optionally specifies the offset into the source data where the copying starts.</param>
        /// <param name="nDstOffset">Optionally specifies the offset into the destination data where the copying starts.</param>
        /// <param name="hStream">Optionally, specifies a handle to a stream to use for the operation.</param>
        /// <param name="bSrcHalfSizeOverride">Optionally, specifies and override for the half size state of the source (default = null, which is ignored).</param>
        /// <param name="bDstHalfSizeOverride">Optionally, specifies and override for the half size state of the destination (default = null, which is ignored).</param>
        public void copy(int nCount, long hSrc, long hDst, int nSrcOffset = 0, int nDstOffset = 0, long hStream = -1, bool? bSrcHalfSizeOverride = null, bool? bDstHalfSizeOverride = null)
        {
            int nSrcHalfSizeOverride = -1;
            int nDstHalfSizeOverride = -1;

            if (bSrcHalfSizeOverride.HasValue)
                nSrcHalfSizeOverride = (bSrcHalfSizeOverride.Value) ? 1 : 0;

            if (bDstHalfSizeOverride.HasValue)
                nDstHalfSizeOverride = (bDstHalfSizeOverride.Value) ? 1 : 0;

            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY, null, m_param.AsLong(nCount, hSrc, hDst, nSrcOffset, nDstOffset, hStream, nSrcHalfSizeOverride, nDstHalfSizeOverride));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY, null, m_param.AsLong(nCount, hSrc, hDst, nSrcOffset, nDstOffset, hStream, nSrcHalfSizeOverride, nDstHalfSizeOverride));
        }

        /// <summary>
        /// Copy similar items of length 'nDim' from hSrc1 (where hSimilar(i) = 1) and dissimilar items of length 'nDim' from hSrc2 (where hSimilar(i) = 0).
        /// </summary>
        /// <param name="nCount">Specifies the total data length of hSrc1, hSrc2 and hDst.</param>
        /// <param name="nNum">Specifis the number of outer items in hSrc1, hSrc2, hDst, and the number of elements in hSimilar.</param>
        /// <param name="nDim">Specifies the inner dimension of hSrc1, hSrc2 and hDst.</param>
        /// <param name="hSrc1">Specifies a handle to the GPU memory of source 1.</param>
        /// <param name="hSrc2">Specifies a handle to the GPU memory of source 2.</param>
        /// <param name="hDst">Specifies a handle to the GPU memory of the destination.</param>
        /// <param name="hSimilar">Specifies a handle to the GPU memory of the similar data.</param>
        /// <param name="bInvert">Optionally, specifies whether or not to invert the similar values (e.g. copy when similar = 0 instead of similar = 1)</param>
        public void copy(int nCount, int nNum, int nDim, long hSrc1, long hSrc2, long hDst, long hSimilar, bool bInvert = false)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY_SIM, null, m_param.AsLong(nCount, nNum, nDim, hSrc1, hSrc2, hDst, hSimilar, (bInvert) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY_SIM, null, m_param.AsLong(nCount, nNum, nDim, hSrc1, hSrc2, hDst, hSimilar, (bInvert) ? 1 : 0));
        }

        /// <summary>
        /// Copy a batch of labeled items into a cache organized by label where older data is removed and replaced by newer data.
        /// </summary>
        /// <param name="nCount">Specifies the total data length of hSrc.</param>
        /// <param name="nNum">Specifis the number of outer items in hSrc1, hSrc2, hDst, and the number of elements in hSimilar.</param>
        /// <param name="nDim">Specifies the inner dimension of hSrc1, hSrc2 and hDst.</param>
        /// <param name="hSrcData">Specifies a handle to the GPU memory of source data.</param>
        /// <param name="hSrcLbl">Specifies a handle to the GPU memory of source labels.</param>
        /// <param name="nDstCount">Specifies the total data length of the hDstCache</param>
        /// <param name="hDstCache">Specifies a handle to the GPU memory of the destination cache.</param>
        /// <param name="hWorkDevData">Specifies a handle to the GPU memory of the device work data that is the same size as the hDstCache.</param>
        /// <param name="nLabelStart">Specifies the first label of all possible labels.</param>
        /// <param name="nLabelCount">Specifies the total number of labels (expects labels to be sequential from 'nLabelStart').</param>
        /// <param name="nCacheSize">Specifies the size of each labeled data cache.</param>
        /// <param name="hCacheHostCursors">Specifies a handle to host memmory (allocated using AllocateHostBuffer) containing the label cursors - there should be 'nLabelCount' cursors.</param>
        /// <param name="hWorkDataHost">Specifies a handle to host memory (allocated using AllocateHostBuffer) used for work - must be nNum in item length.</param>
        /// <remarks>
        /// NOTE: The cache size must be set at a sufficient size that covers the maximum number items for any given label within a batch, otherwise cached items will be overwritten for items in the current batch.
        /// </remarks>
        public void copy_batch(int nCount, int nNum, int nDim, long hSrcData, long hSrcLbl, int nDstCount, long hDstCache, long hWorkDevData, int nLabelStart, int nLabelCount, int nCacheSize, long hCacheHostCursors, long hWorkDataHost)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY_BATCH, null, m_param.AsLong(nCount, nNum, nDim, hSrcData, hSrcLbl, nDstCount, hDstCache, hWorkDevData, nLabelStart, nLabelCount, nCacheSize, hCacheHostCursors, hWorkDataHost));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY_BATCH, null, m_param.AsLong(nCount, nNum, nDim, hSrcData, hSrcLbl, nDstCount, hDstCache, hWorkDevData, nLabelStart, nLabelCount, nCacheSize, hCacheHostCursors, hWorkDataHost));
        }

        /// <summary>
        /// Copy a sequence of cached items, organized by label, into an anchor, positive (if nK > 0), and negative blobs.
        /// </summary>
        /// <param name="nK">Specifies the output type expected where: nK = 0, outputs to 2 tops (anchor and one negative), or nK > 0, outputs to 2 + nK tops (anchor, positive, nK negatives).  The rghTop and rgnTopCount must be sized accordingly.</param>
        /// <param name="nNum">Specifis the number of outer items in hSrc1, hSrc2, hDst, and the number of elements in hSimilar.</param>
        /// <param name="nDim">Specifies the inner dimension of hSrc1, hSrc2 and hDst.</param>
        /// <param name="hSrcData">Specifies a handle to the GPU memory of source data.</param>
        /// <param name="hSrcLbl">Specifies a handle to the GPU memory of source labels.</param>
        /// <param name="nSrcCacheCount">Specifis the number of items in hSrcCache (nCacheSize * nLabelCount).</param>
        /// <param name="hSrcCache">Specifies a handle to the cached labeled data.</param>
        /// <param name="nLabelStart">Specifies the first label of all possible labels.</param>
        /// <param name="nLabelCount">Specifies the total number of labels (expects labels to be sequential from 'nLabelStart').</param>
        /// <param name="nCacheSize">Specifies the size of each labeled data cache.</param>
        /// <param name="hCacheHostCursors">Specifies a handle to host memmory containing the label cursors - there should be 'nLabelCount' cursors.</param>
        /// <param name="bOutputLabels">Specifies whether or not to output labels or not.  When true, one additional top is expected for the labels.</param>
        /// <param name="rghTop">Specifies a list of the GPU memory for each top item.  The number of top items expected depends on the 'nK' value.</param>
        /// <param name="rgnTopCount">Specifies a list of the item count for each top item.  The number of top items expected depends on the 'nK' value.</param>
        /// <param name="hWorkDataHost">Specifies a handle to host memory (allocated using AllocateHostBuffer) used for work - must be nNum in item length and must be the same hWorkDataHost passed to 'copy_batch'.</param>
        /// <param name="bCombinePositiveAndNegative">Optionally, specifies to combine the positive and negative items by alternating between each and placing both in Top[1], while also making sure the output labels reflect the alternation.</param>
        /// <param name="nSeed">Optionally, specifies a seed for the random number generator (default = 0, which igores this parameter).</param>
        /// <remarks>
        /// Receiving an error ERROR_BATCH_TOO_SMALL indicates that the batch size is too small and does not have enough labels to choose from.  Each batch should have at least two instances of each labeled item.
        /// 
        /// NOTE: When 'nK' = 1 and 'bCombinePositiveAndNegative' = true, the label output has a dimension of 2, and and the tops used are as follows: top(0) = anchor; top(1) = alternating negative/positive, top(2) = labels if 'bOutputLabels' = true.
        /// </remarks>
        public void copy_sequence(int nK, int nNum, int nDim, long hSrcData, long hSrcLbl, int nSrcCacheCount, long hSrcCache, int nLabelStart, int nLabelCount, int nCacheSize, long hCacheHostCursors, bool bOutputLabels, List<long> rghTop, List<int> rgnTopCount, long hWorkDataHost, bool bCombinePositiveAndNegative = false, int nSeed = 0)
        {
            int nTopCount = 2 + nK;

            if (bOutputLabels)
                nTopCount++;

            if (bCombinePositiveAndNegative && nK != 0)
                throw new ArgumentOutOfRangeException("nK", "When using 'bCombinePositiveAndNegative', nK should be 0.");

            if (nK < 0 || nK > 10)
                throw new ArgumentOutOfRangeException("nK", "The 'nK' parameter must be within the range [0,10]!");

            if (rghTop.Count != nTopCount)
                throw new ArgumentOutOfRangeException("rghTop", "The 'rghTop' count must equal '" + nTopCount.ToString() + "' given nK = " + nK.ToString() + " and bOutputLabels = " + bOutputLabels.ToString() + "!");

            if (rgnTopCount.Count != rghTop.Count)
                throw new ArgumentOutOfRangeException("rgnTopCount", "The 'rgnTopCount' count must equal the 'rghTop' count!");

            if (m_dt == DataType.DOUBLE)
            {
                List<long> rgarg = new List<long>() { nK, nNum, nDim, hSrcData, hSrcLbl, nSrcCacheCount, hSrcCache, nLabelStart, nLabelCount, nCacheSize, hCacheHostCursors, (bOutputLabels) ? 1 : 0, hWorkDataHost, (bCombinePositiveAndNegative) ? 1 : 0, nSeed };

                for (int i = 0; i < rghTop.Count; i++)
                {
                    rgarg.Add(rghTop[i]);
                }

                for (int i = 0; i < rgnTopCount.Count; i++)
                {
                    rgarg.Add(rgnTopCount[i]);
                }

                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY_SEQUENCE, null, rgarg.ToArray());
            }
            else
            {
                List<long> rgarg = new List<long>() { nK, nNum, nDim, hSrcData, hSrcLbl, nSrcCacheCount, hSrcCache, nLabelStart, nLabelCount, nCacheSize, hCacheHostCursors, (bOutputLabels) ? 1 : 0, hWorkDataHost, (bCombinePositiveAndNegative) ? 1 : 0, nSeed };

                for (int i = 0; i < rghTop.Count; i++)
                {
                    rgarg.Add(rghTop[i]);
                }

                for (int i = 0; i < rgnTopCount.Count; i++)
                {
                    rgarg.Add(rgnTopCount[i]);
                }

                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY_SEQUENCE, null, rgarg.ToArray());
            }
        }

        /// <summary>
        /// Copy a sequence from a source to a destination and allow for skip steps.
        /// </summary>
        /// <param name="n">Specifies the total number of items in src.</param>
        /// <param name="hSrc">Specifies a handle to the source GPU memory.</param>
        /// <param name="nSrcStep">Specifies the stepping used across the source.</param>
        /// <param name="nSrcStartIdx">Specifies the starting index into the source.</param>
        /// <param name="nCopyCount">Specifies the number of items to copy.</param>
        /// <param name="nCopyDim">Specifies the dimension to copy (which x spatial dim = total copy amount).</param>
        /// <param name="hDst">Specifies a handle to the destination GPU memory.</param>
        /// <param name="nDstStep">Specifies the steping used across the desination.</param>
        /// <param name="nDstStartIdx">Specifies the starting index where data is to be copied in the destination.</param>
        /// <param name="nSrcSpatialDim">Specifies the src spatial dim of each item copied.  Src and Dst spatial dims should be equal when nSpatialDimCount is not used.</param>
        /// <param name="nDstSpatialDim">Specifies the dst spatial dim of each item copied.  Src and Dst spatial dims should be equal when nSpatialDimCount is not used.</param>
        /// <param name="nSrcSpatialDimStartIdx">Optionally, specifies the start index within the source spatial dim to start the copy (default = 0)</param>
        /// <param name="nDstSpatialDimStartIdx">Optionally, specifies the start index within the destination spatial dim to start the copy (default = 0)</param>
        /// <param name="nSpatialDimCount">Optionally, specifies the number of items to copy from within the spatial dim (default = -1, copy all)</param>
        public void copy_sequence(int n, long hSrc, int nSrcStep, int nSrcStartIdx, int nCopyCount, int nCopyDim, long hDst, int nDstStep, int nDstStartIdx, int nSrcSpatialDim, int nDstSpatialDim, int nSrcSpatialDimStartIdx = 0, int nDstSpatialDimStartIdx = 0, int nSpatialDimCount = -1)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY_SEQUENCE2, null, m_param.AsLong(n, hSrc, nSrcStep, nSrcStartIdx, nCopyCount, nCopyDim, hDst, nDstStep, nDstStartIdx, nSrcSpatialDim, nDstSpatialDim, nSrcSpatialDimStartIdx, nDstSpatialDimStartIdx, nSpatialDimCount));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY_SEQUENCE2, null, m_param.AsLong(n, hSrc, nSrcStep, nSrcStartIdx, nCopyCount, nCopyDim, hDst, nDstStep, nDstStartIdx, nSrcSpatialDim, nDstSpatialDim, nSrcSpatialDimStartIdx, nDstSpatialDimStartIdx, nSpatialDimCount));
        }

        /// <summary>
        /// Expand a vector of length 'nNum' into a matrix of size 'nNum' x 'nDim' by copying each value of the vector
        /// into all elements of the corresponding matrix row.
        /// </summary>
        /// <param name="n">Specifies the total number of items in the matrix 'A'</param>
        /// <param name="nNum">Specifies the total number of rows in the matrix 'A' and the total number of items in the vector 'X'.</param>
        /// <param name="nDim">Specifies the total number of columns in the matrix 'A'.</param>
        /// <param name="hX">Specifies the 'nNum' length vector to expand.</param>
        /// <param name="hA">Specifies the 'nNum' x 'nDim' matrix.</param>
        public void copy_expand(int n, int nNum, int nDim, long hX, long hA)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY_EXPAND, null, m_param.AsLong(n, nNum, nDim, hX, hA));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY_EXPAND, null, m_param.AsLong(n, nNum, nDim, hX, hA));
        }

        /// <summary>
        /// Fill data from the source data 'n' times in the destination.
        /// </summary>
        /// <param name="n">Specifies the number of times to copy the source data.</param>
        /// <param name="nDim">Specifies the number of source items to copy.</param>
        /// <param name="hSrc">Specifies a handle to the GPU memory of the source data.</param>
        /// <param name="nSrcOff">Specifies an offset into the GPU memory where the source data copy starts.</param>
        /// <param name="nCount">Specifies the total number of items in the destination.  This value must be >= n * nDim.</param>
        /// <param name="hDst">Specifies the handle to the GPU memory where the data is to be copied.</param>
        public void fill(int n, int nDim, long hSrc, int nSrcOff, int nCount, long hDst)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY_FILL, null, m_param.AsLong(n, nDim, hSrc, nSrcOff, nCount, hDst));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COPY_FILL, null, m_param.AsLong(n, nDim, hSrc, nSrcOff, nCount, hDst));
        }

        /// <summary>
        /// Sort the data in the GPU memory specified.
        /// </summary>
        /// <param name="nCount">Specifies the total number of items in the memory.</param>
        /// <param name="hY">Specifies the handle to the GPU memory of data to sort.</param>
        public void sort(int nCount, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SORT, null, m_param.AsLong(nCount, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SORT, null, m_param.AsLong(nCount, hY)); ;
        }

        /// <summary>
        /// Perform a matrix-matrix multiplication operation: C = alpha transB (B) transA (A) + beta C 
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="bTransA">Specifies whether or not to transpose A.</param>
        /// <param name="bTransB">Specifies whether or not to transpose B.</param>
        /// <param name="m">Specifies the width (number of columns) of A and C.</param>
        /// <param name="n">Specifies the height (number of rows) of B and C.</param>
        /// <param name="k">Specifies the width (number of columns) of A and B.</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by the data where the scalar is of type <code>double</code></param>
        /// <param name="hA">Specifies a handle to the data for A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the data for B in GPU memory.</param>
        /// <param name="fBeta">Specifies a scalar multiplied by C where the scalar is of type <code>double</code></param>
        /// <param name="hC">Specifies a handle to the data for C in GPU memory.</param>
        public void gemm(bool bTransA, bool bTransB, int m, int n, int k, double fAlpha, long hA, long hB, double fBeta, long hC)
        {
            gemm(bTransA, bTransB, m, n, k, (T)Convert.ChangeType(fAlpha, typeof(T)), hA, hB, (T)Convert.ChangeType(fBeta, typeof(T)), hC);
        }

        /// <summary>
        /// Perform a matrix-matrix multiplication operation: C = alpha transB (B) transA (A) + beta C 
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="bTransA">Specifies whether or not to transpose A.</param>
        /// <param name="bTransB">Specifies whether or not to transpose B.</param>
        /// <param name="m">Specifies the width (number of columns) of A and C.</param>
        /// <param name="n">Specifies the height (number of rows) of B and C.</param>
        /// <param name="k">Specifies the width (number of columns) of A and B.</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by the data where the scalar is of type <code>float</code></param>
        /// <param name="hA">Specifies a handle to the data for matrix A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the data for matrix B in GPU memory.</param>
        /// <param name="fBeta">Specifies a scalar multiplied by C where the scalar is of type <code>float</code></param>
        /// <param name="hC">Specifies a handle to the data for matrix C in GPU memory.</param>
        public void gemm(bool bTransA, bool bTransB, int m, int n, int k, float fAlpha, long hA, long hB, float fBeta, long hC)
        {
            gemm(bTransA, bTransB, m, n, k, (T)Convert.ChangeType(fAlpha, typeof(T)), hA, hB, (T)Convert.ChangeType(fBeta, typeof(T)), hC);
        }

        /// <summary>
        /// Perform a matrix-matrix multiplication operation: C = alpha transB (B) transA (A) + beta C 
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="bTransA">Specifies whether or not to transpose A.</param>
        /// <param name="bTransB">Specifies whether or not to transpose B.</param>
        /// <param name="m">Specifies the width (number of columns) of A and C.</param>
        /// <param name="n">Specifies the height (number of rows) of B and C.</param>
        /// <param name="k">Specifies the width (number of columns) of A and B.</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by the data where the scalar is of type 'T'.</param>
        /// <param name="hA">Specifies a handle to the data for matrix A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the data for matrix B in GPU memory.</param>
        /// <param name="fBeta">Specifies a scalar multiplied by C where the scalar is of type 'T'.</param>
        /// <param name="hC">Specifies a handle to the data for matrix C in GPU memory.</param>
        /// <param name="nAOffset">Specifies an offset (in items, not bytes) into the memory of A.</param>
        /// <param name="nBOffset">Specifies an offset (in items, not bytes) into the memory of B.</param>
        /// <param name="nCOffset">Specifies an offset (in items, not bytes) into the memory of C.</param>
        /// <param name="nGroups">Optionally, specifies the number of groups (default = 1).</param>
        /// <param name="nGroupOffsetA">Optionally, specifies an offset multiplied by the current group 'g' and added to the AOffset (default = 0).</param>
        /// <param name="nGroupOffsetB">Optionally, specifies an offset multiplied by the current group 'g' and added to the BOffset (default = 0).</param>
        /// <param name="nGroupOffsetC">Optionally, specifies an offset multiplied by the current group 'g' and added to the COffset (default = 0).</param>
        public void gemm(bool bTransA, bool bTransB, int m, int n, int k, T fAlpha, long hA, long hB, T fBeta, long hC, int nAOffset = 0, int nBOffset = 0, int nCOffset = 0, int nGroups = 1, int nGroupOffsetA = 0, int nGroupOffsetB = 0, int nGroupOffsetC = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_GEMM, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong((bTransA) ? 1 : 0, (bTransB) ? 1 : 0, m, n, k, 0, hA, hB, 0, hC, nAOffset, nBOffset, nCOffset, nGroups, nGroupOffsetA, nGroupOffsetB, nGroupOffsetC));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_GEMM, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong((bTransA) ? 1 : 0, (bTransB) ? 1 : 0, m, n, k, 0, hA, hB, 0, hC, nAOffset, nBOffset, nCOffset, nGroups, nGroupOffsetA, nGroupOffsetB, nGroupOffsetC));
        }

        /// <summary>
        /// Perform a matrix-matrix multiplication operation: C = alpha transB (B) transA (A) + beta C 
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="bTransA">Specifies whether or not to transpose A.</param>
        /// <param name="bTransB">Specifies whether or not to transpose B.</param>
        /// <param name="m">Specifies the width (number of columns) of A and C.</param>
        /// <param name="n">Specifies the height (number of rows) of B and C.</param>
        /// <param name="k">Specifies the width (number of columns) of A and B.</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by the data where the scalar is of type 'T'.</param>
        /// <param name="hA">Specifies a handle to the data for matrix A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the data for matrix B in GPU memory.</param>
        /// <param name="fBeta">Specifies a scalar multiplied by C where the scalar is of type 'T'.</param>
        /// <param name="hC">Specifies a handle to the data for matrix C in GPU memory.</param>
        /// <param name="lda">Specifies the leading dimension of A.</param>
        /// <param name="ldb">Specifies the leading dimension of B.</param>
        /// <param name="ldc">Specifies the leading dimension of C.</param>
        public void gemm(bool bTransA, bool bTransB, int m, int n, int k, double fAlpha, long hA, long hB, double fBeta, long hC, uint lda, uint ldb, uint ldc)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_GEMM2, m_param.AsDouble(fAlpha, fBeta), m_param.AsLong((bTransA) ? 1 : 0, (bTransB) ? 1 : 0, m, n, k, 0, hA, hB, 0, hC, lda, ldb, ldc));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_GEMM2, m_param.AsFloat((float)fAlpha, (float)fBeta), m_param.AsLong((bTransA) ? 1 : 0, (bTransB) ? 1 : 0, m, n, k, 0, hA, hB, 0, hC, lda, ldb, ldc));
        }

        /// <summary>
        /// Perform a matrix-matrix multiplication operation: C = alpha transB (B) transA (A) + beta C 
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="bTransA">Specifies whether or not to transpose A.</param>
        /// <param name="bTransB">Specifies whether or not to transpose B.</param>
        /// <param name="m">Specifies the width (number of columns) of A and C.</param>
        /// <param name="n">Specifies the height (number of rows) of B and C.</param>
        /// <param name="k">Specifies the width (number of columns) of A and B.</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by the data where the scalar is of type 'T'.</param>
        /// <param name="hA">Specifies a handle to the data for matrix A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the data for matrix B in GPU memory.</param>
        /// <param name="fBeta">Specifies a scalar multiplied by C where the scalar is of type 'T'.</param>
        /// <param name="hC">Specifies a handle to the data for matrix C in GPU memory.</param>
        /// <param name="lda">Specifies the leading dimension of A.</param>
        /// <param name="ldb">Specifies the leading dimension of B.</param>
        /// <param name="ldc">Specifies the leading dimension of C.</param>
        /// <param name="stridea">Specifies the stride of matrix A</param>
        /// <param name="strideb">Specifies the stride of matrix B</param>
        /// <param name="stridec">Specifies the stride of matrix C</param>
        /// <param name="batch_count">Specifies the number of matricies.</param>
        public void gemm(bool bTransA, bool bTransB, int m, int n, int k, double fAlpha, long hA, long hB, double fBeta, long hC, uint lda, uint ldb, uint ldc, uint stridea, uint strideb, uint stridec, uint batch_count)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_GEMM2, m_param.AsDouble(fAlpha, fBeta), m_param.AsLong((bTransA) ? 1 : 0, (bTransB) ? 1 : 0, m, n, k, 0, hA, hB, 0, hC, lda, ldb, ldc, stridea, strideb, stridec, batch_count));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_GEMM2, m_param.AsFloat((float)fAlpha, (float)fBeta), m_param.AsLong((bTransA) ? 1 : 0, (bTransB) ? 1 : 0, m, n, k, 0, hA, hB, 0, hC, lda, ldb, ldc, stridea, strideb, stridec, batch_count));
        }

        /// <summary>
        /// Perform a matrix-matrix addition/transposition operation: C = alpha transA (A) + beta transB (B) 
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="bTransA">Specifies whether or not to transpose A.</param>
        /// <param name="bTransB">Specifies whether or not to transpose B.</param>
        /// <param name="m">Specifies the width (number of columns) of A, B and C.</param>
        /// <param name="n">Specifies the height (number of rows) of A, B and C.</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by the data where the scalar is of type <code>double</code></param>
        /// <param name="hA">Specifies a handle to the data for A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the data for B in GPU memory.</param>
        /// <param name="fBeta">Specifies a scalar multiplied by C where the scalar is of type <code>double</code></param>
        /// <param name="hC">Specifies a handle to the data for C in GPU memory.</param>
        public void geam(bool bTransA, bool bTransB, int m, int n, double fAlpha, long hA, long hB, double fBeta, long hC)
        {
            geam(bTransA, bTransB, m, n, (T)Convert.ChangeType(fAlpha, typeof(T)), hA, hB, (T)Convert.ChangeType(fBeta, typeof(T)), hC);
        }

        /// <summary>
        /// Perform a matrix-matrix addition/transposition operation: C = alpha transA (A) + beta transB (B) 
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="bTransA">Specifies whether or not to transpose A.</param>
        /// <param name="bTransB">Specifies whether or not to transpose B.</param>
        /// <param name="m">Specifies the width (number of columns) of A, B and C.</param>
        /// <param name="n">Specifies the height (number of rows) of A, B and C.</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by the data where the scalar is of type <code>double</code></param>
        /// <param name="hA">Specifies a handle to the data for A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the data for B in GPU memory.</param>
        /// <param name="fBeta">Specifies a scalar multiplied by C where the scalar is of type <code>double</code></param>
        /// <param name="hC">Specifies a handle to the data for C in GPU memory.</param>
        public void geam(bool bTransA, bool bTransB, int m, int n, float fAlpha, long hA, long hB, float fBeta, long hC)
        {
            geam(bTransA, bTransB, m, n, (T)Convert.ChangeType(fAlpha, typeof(T)), hA, hB, (T)Convert.ChangeType(fBeta, typeof(T)), hC);
        }

        /// <summary>
        /// Perform a matrix-matrix multiplication operation: C = alpha transB (B) transA (A) + beta C 
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="bTransA">Specifies whether or not to transpose A.</param>
        /// <param name="bTransB">Specifies whether or not to transpose B.</param>
        /// <param name="m">Specifies the width (number of columns) of A and C.</param>
        /// <param name="n">Specifies the height (number of rows) of B and C.</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by the data where the scalar is of type 'T'.</param>
        /// <param name="hA">Specifies a handle to the data for matrix A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the data for matrix B in GPU memory.</param>
        /// <param name="fBeta">Specifies a scalar multiplied by C where the scalar is of type 'T'.</param>
        /// <param name="hC">Specifies a handle to the data for matrix C in GPU memory.</param>
        /// <param name="nAOffset">Specifies an offset (in items, not bytes) into the memory of A.</param>
        /// <param name="nBOffset">Specifies an offset (in items, not bytes) into the memory of B.</param>
        /// <param name="nCOffset">Specifies an offset (in items, not bytes) into the memory of C.</param>
        public void geam(bool bTransA, bool bTransB, int m, int n, T fAlpha, long hA, long hB, T fBeta, long hC, int nAOffset = 0, int nBOffset = 0, int nCOffset = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_GEAM, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong((bTransA) ? 1 : 0, (bTransB) ? 1 : 0, m, n, 0, hA, hB, 0, hC, nAOffset, nBOffset, nCOffset));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_GEAM, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong((bTransA) ? 1 : 0, (bTransB) ? 1 : 0, m, n, 0, hA, hB, 0, hC, nAOffset, nBOffset, nCOffset));
        }

        /// <summary>
        /// Perform a matrix-vector multiplication operation: y = alpha transA (A) x + beta y (where x and y are vectors)
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="bTransA">Specifies whether or not to transpose A.</param>
        /// <param name="m">Specifies the width (number of columns) of A.</param>
        /// <param name="n">Specifies the height (number of rows) of A.</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by the data where the scalar is of type <code>double</code></param>
        /// <param name="hA">Specifies a handle to the data for matrix A in GPU memory.</param>
        /// <param name="hX">Specifies a handle to the data for vector x in GPU memory.</param>
        /// <param name="fBeta">Specifies a scalar multiplied by y where the scalar is of type <code>double</code></param>
        /// <param name="hY">Specifies a handle to the data for vectory y in GPU memory.</param>
        public void gemv(bool bTransA, int m, int n, double fAlpha, long hA, long hX, double fBeta, long hY)
        {
            gemv(bTransA, m, n, (T)Convert.ChangeType(fAlpha, typeof(T)), hA, hX, (T)Convert.ChangeType(fBeta, typeof(T)), hY);
        }

        /// <summary>
        /// Perform a matrix-vector multiplication operation: y = alpha transA (A) x + beta y (where x and y are vectors)
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="bTransA">Specifies whether or not to transpose A.</param>
        /// <param name="m">Specifies the width (number of columns) of A.</param>
        /// <param name="n">Specifies the height (number of rows) of A.</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by the data where the scalar is of type <code>float</code></param>
        /// <param name="hA">Specifies a handle to the data for matrix A in GPU memory.</param>
        /// <param name="hX">Specifies a handle to the data for vector x in GPU memory.</param>
        /// <param name="fBeta">Specifies a scalar multiplied by y where the scalar is of type <code>float</code></param>
        /// <param name="hY">Specifies a handle to the data for vectory y in GPU memory.</param>
        public void gemv(bool bTransA, int m, int n, float fAlpha, long hA, long hX, float fBeta, long hY)
        {
            gemv(bTransA, m, n, (T)Convert.ChangeType(fAlpha, typeof(T)), hA, hX, (T)Convert.ChangeType(fBeta, typeof(T)), hY);
        }

        /// <summary>
        /// Perform a matrix-vector multiplication operation: y = alpha transA (A) x + beta y (where x and y are vectors)
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="bTransA">Specifies whether or not to transpose A.</param>
        /// <param name="m">Specifies the width (number of columns) of A.</param>
        /// <param name="n">Specifies the height (number of rows) of A.</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by the data where the scalar is of type 'T'.</param>
        /// <param name="hA">Specifies a handle to the data for matrix A in GPU memory.</param>
        /// <param name="hX">Specifies a handle to the data for vector X in GPU memory.</param>
        /// <param name="fBeta">Specifies a scalar multiplied by Y where the scalar is of type 'T'</param>
        /// <param name="hY">Specifies a handle to the data for vectory y in GPU memory.</param>
        /// <param name="nAOffset">Specifies an offset (in items, not bytes) into the memory of A.</param>
        /// <param name="nXOffset">Specifies an offset (in items, not bytes) into the memory of X.</param>
        /// <param name="nYOffset">Specifies an offset (in items, not bytes) into the memory of Y.</param>
        public void gemv(bool bTransA, int m, int n, T fAlpha, long hA, long hX, T fBeta, long hY, int nAOffset = 0, int nXOffset = 0, int nYOffset = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_GEMV, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong((bTransA) ? 1 : 0, m, n, 0, hA, hX, 0, hY, nAOffset, nXOffset, nYOffset));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_GEMV, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong((bTransA) ? 1 : 0, m, n, 0, hA, hX, 0, hY, nAOffset, nXOffset, nYOffset));
        }

        /// <summary>
        /// Perform a vector-vector multiplication operation: A = x * (fAlpha * y) (where x and y are vectors and A is an m x n Matrix)
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="m">Specifies the length of X and rows in A (m x n).</param>
        /// <param name="n">Specifies the length of Y and cols in A (m x n).</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by y where the scalar is of type 'T'.</param>
        /// <param name="hX">Specifies a handle to the data for matrix X (m in length) in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the data for vector Y (n in length) in GPU memory.</param>
        /// <param name="hA">Specifies a handle to the data for matrix A (m x n) in GPU memory.</param>
        public void ger(int m, int n, double fAlpha, long hX, long hY, long hA)
        {
            ger(m, n, (T)Convert.ChangeType(fAlpha, typeof(T)), hX, hY, hA);
        }

        /// <summary>
        /// Perform a vector-vector multiplication operation: A = x * (fAlpha * y) (where x and y are vectors and A is an m x n Matrix)
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="m">Specifies the length of X and rows in A (m x n).</param>
        /// <param name="n">Specifies the length of Y and cols in A (m x n).</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by y where the scalar is of type 'T'.</param>
        /// <param name="hX">Specifies a handle to the data for matrix X (m in length) in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the data for vector Y (n in length) in GPU memory.</param>
        /// <param name="hA">Specifies a handle to the data for matrix A (m x n) in GPU memory.</param>
        public void ger(int m, int n, float fAlpha, long hX, long hY, long hA)
        {
            ger(m, n, (T)Convert.ChangeType(fAlpha, typeof(T)), hX, hY, hA);
        }

        /// <summary>
        /// Perform a vector-vector multiplication operation: A = x * (fAlpha * y) (where x and y are vectors and A is an m x n Matrix)
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas) but with a different parameter ordering.
        /// </remarks>
        /// <param name="m">Specifies the length of X and rows in A (m x n).</param>
        /// <param name="n">Specifies the length of Y and cols in A (m x n).</param>
        /// <param name="fAlpha">Specifies a scalar multiplied by y where the scalar is of type 'T'.</param>
        /// <param name="hX">Specifies a handle to the data for matrix X (m in length) in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the data for vector Y (n in length) in GPU memory.</param>
        /// <param name="hA">Specifies a handle to the data for matrix A (m x n) in GPU memory.</param>
        public void ger(int m, int n, T fAlpha, long hX, long hY, long hA)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_GER, m_param.AsDouble(convertD(fAlpha)), m_param.AsLong(m, n, 0, hX, hY, hA));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_GER, m_param.AsFloat(convertF(fAlpha)), m_param.AsLong(m, n, 0, hX, hY, hA));
        }

        /// <summary>
        /// Multiply the vector X by a scalar and add the result to the vector Y.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="fAlpha">Specifies the scalar to multiply where the scalar is of type <code>double</code></param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void axpy(int n, double fAlpha, long hX, long hY)
        {
            axpy(n, (T)Convert.ChangeType(fAlpha, typeof(T)), hX, hY);
        }

        /// <summary>
        /// Multiply the vector X by a scalar and add the result to the vector Y.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="fAlpha">Specifies the scalar to multiply where the scalar is of type <code>float</code></param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void axpy(int n, float fAlpha, long hX, long hY)
        {
            axpy(n, (T)Convert.ChangeType(fAlpha, typeof(T)), hX, hY);
        }

        /// <summary>
        /// Multiply the vector X by a scalar and add the result to the vector Y.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="fAlpha">Specifies the scalar to multiply where the scalar is of type 'T'.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nXOff">Optionally, specifies an offset (in items, not bytes) into the memory of X.</param>
        /// <param name="nYOff">Optionally, specifies an offset (in items, not bytes) into the memory of Y.</param>
        public void axpy(int n, T fAlpha, long hX, long hY, int nXOff = 0, int nYOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_AXPY, m_param.AsDouble(convertD(fAlpha)), m_param.AsLong(n, 0, hX, hY, nXOff, nYOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_AXPY, m_param.AsFloat(convertF(fAlpha)), m_param.AsLong(n, 0, hX, hY, nXOff, nYOff));
        }

        /// <summary>
        /// Scale the vector x and then multiply the vector X by a scalar and add the result to the vector Y.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="fAlpha">Specifies the scalar to multiply where the scalar is of type <code>double</code></param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="fBeta">Specifies the scaling factor to apply to vector X, where the scaling factor is of type <code>double</code></param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void axpby(int n, double fAlpha, long hX, double fBeta, long hY)
        {
            axpby(n, (T)Convert.ChangeType(fAlpha, typeof(T)), hX, (T)Convert.ChangeType(fBeta, typeof(T)), hY);
        }

        /// <summary>
        /// Scale the vector x and then multiply the vector X by a scalar and add the result to the vector Y.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="fAlpha">Specifies the scalar to multiply where the scalar is of type <code>float</code></param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="fBeta">Specifies the scaling factor to apply to vector X, where the scaling factor is of type <code>float</code></param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void axpby(int n, float fAlpha, long hX, float fBeta, long hY)
        {
            axpby(n, (T)Convert.ChangeType(fAlpha, typeof(T)), hX, (T)Convert.ChangeType(fBeta, typeof(T)), hY);
        }

        /// <summary>
        /// Scale the vector x by Alpha and scale vector y by Beta and then add both together.
        /// 
        /// Y = (X * fAlpha) + (Y * fBeta)
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="fAlpha">Specifies the scalar to multiply where the scalar is of type 'T'.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="fBeta">Specifies the scaling factor to apply to vector X, where the scaling factor is of type 'T'.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void axpby(int n, T fAlpha, long hX, T fBeta, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_AXPBY, m_param.AsDouble(convertD(fAlpha), convertD(fBeta)), m_param.AsLong(n, 0, hX, 0, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_AXPBY, m_param.AsFloat(convertF(fAlpha), convertF(fBeta)), m_param.AsLong(n, 0, hX, 0, hY));
        }

        /// <summary>
        /// Multiply a matrix with a vector.
        /// </summary>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="hA">Specifies the matrix to multiply.</param>
        /// <param name="nAOff">Specifies the offset to apply to the GPU memory of hA.</param>
        /// <param name="hX">Specifies the vector to multiply.</param>
        /// <param name="nXOff">Specifies the offset to apply to the GPU memory of hX.</param>
        /// <param name="nC">Specifies the number of channels.</param>
        /// <param name="nSpatialDim">Specifies the spatial dimension.</param>
        /// <param name="bTranspose">Specifies whether or not to transpose the matrix.</param>
        /// <param name="hB">Specifies the output matrix.</param>
        /// <param name="nBOff">Specifies the offset to apply to the GPU memory of hB.</param>
        public void mulbsx(int n, long hA, int nAOff, long hX, int nXOff, int nC, int nSpatialDim, bool bTranspose, long hB, int nBOff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MULBSX, null, m_param.AsLong(n, hA, nAOff, hX, nXOff, nC, nSpatialDim, (bTranspose) ? 1 : 0, hB, nBOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MULBSX, null, m_param.AsLong(n, hA, nAOff, hX, nXOff, nC, nSpatialDim, (bTranspose) ? 1 : 0, hB, nBOff));
        }

        /// <summary>
        /// Divide a matrix by a vector.
        /// </summary>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="hA">Specifies the matrix to divide.</param>
        /// <param name="nAOff">Specifies the offset to apply to the GPU memory of hA.</param>
        /// <param name="hX">Specifies the divisor vector.</param>
        /// <param name="nXOff">Specifies the offset to apply to the GPU memory of hX.</param>
        /// <param name="nC">Specifies the number of channels.</param>
        /// <param name="nSpatialDim">Specifies the spatial dimension.</param>
        /// <param name="bTranspose">Specifies whether or not to transpose the matrix.</param>
        /// <param name="hB">Specifies the output matrix.</param>
        /// <param name="nBOff">Specifies the offset to apply to the GPU memory of hB.</param>
        public void divbsx(int n, long hA, int nAOff, long hX, int nXOff, int nC, int nSpatialDim, bool bTranspose, long hB, int nBOff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_DIVBSX, null, m_param.AsLong(n, hA, nAOff, hX, nXOff, nC, nSpatialDim, (bTranspose) ? 1 : 0, hB, nBOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_DIVBSX, null, m_param.AsLong(n, hA, nAOff, hX, nXOff, nC, nSpatialDim, (bTranspose) ? 1 : 0, hB, nBOff));
        }

        /// <summary>
        /// Perform matmul operation hC = matmul(hA, hB), where hA, hB and hC are all in row-major format.
        /// </summary>
        /// <param name="nOuterCount">Specifies the outer count (e.g. batch * channels)</param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <param name="hA">Specifies the handle to GPU memory holding the mxk matrix A (in row-major format)</param>
        /// <param name="hB">Specifies the handle to GPU memory holding the kxn matrix B (in row-major format)</param>
        /// <param name="hC">Specifies the handle to GPU memory holding the mxn matrix C (in row-major format) where the result is placed.</param>
        /// <param name="dfScale">Specifies the scale value applied to matrix B in hB (default = 1.0)</param>
        /// <param name="bTransA">Specifies to transpose matrix A (default = false).</param>
        /// <param name="bTransB">Specifies to transpose matrix B (default = false).</param>
        /// <param name="bAccumulate">Optionally, specifies to accumulate the gradients.</param>
        /// <remarks>
        /// @see [How to transpose a matrix in CUDA/cublas](https://stackoverflow.com/questions/13782012/how-to-transpose-a-matrix-in-cuda-cublas)
        /// </remarks>
        public void matmul(uint nOuterCount, int m, int n, int k, long hA, long hB, long hC, double dfScale = 1.0, bool bTransA = false, bool bTransB = false, bool bAccumulate = false)
        {
            uint ldb = (bTransB) ? (uint)k : (uint)n;
            uint lda = (bTransA) ? (uint)m : (uint)k;
            uint ldc = (uint)n;
            uint strideb = (uint)(k * n);
            uint stridea = (uint)(m * k);
            uint stridec = (uint)(m * n);
            double dfBeta = (bAccumulate) ? 1.0 : 0.0;

            gemm(bTransB, bTransA, n, m, k, dfScale, hB, hA, dfBeta, hC, ldb, lda, ldc, strideb, stridea, stridec, nOuterCount);
        }


        /// <summary>
        /// Transpose a n*c number of matrices along the height and width dimensions.  All matrices are in row-major format.
        /// </summary>
        /// <param name="n">Specifies the number of items (e.g. batches)</param>
        /// <param name="c">Specifies the number of channels.</param>
        /// <param name="h">Specifies the height.</param>
        /// <param name="w">Specifies the width.</param>
        /// <param name="hSrc">Specifies a handle to GPU memory of shape (n,c,h,w)</param>
        /// <param name="hDst">Specifies a handle to GPU memory of shape (n,c,w,h)</param>
        public void transposeHW(int n, int c, int h, int w, long hSrc, long hDst)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TRANSPOSE_HW, null, m_param.AsLong(n, c, h, w, hSrc, hDst));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TRANSPOSE_HW, null, m_param.AsLong(n, c, h, w, hSrc, hDst));
        }


        /// <summary>
        /// Set the bounds of all items within the data to a set range of values.
        /// </summary>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="dfMin">Specifies the minimum value.</param>
        /// <param name="dfMax">Specifies the maximum value.</param>
        /// <param name="hX">Specifies a handle to the GPU data to be bound.</param>
        public void set_bounds(int n, double dfMin, double dfMax, long hX)
        {
            if (m_dt == DataType.DOUBLE)
            {
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SET_BOUNDS, m_param.AsDouble(dfMin, dfMax), m_param.AsLong(n, 0, 0, hX));
            }
            else
            {
                float fMin = -float.MaxValue;
                float fMax = float.MaxValue;

                if (dfMin > -float.MaxValue && dfMin < float.MaxValue)
                    fMin = (float)dfMin;
                else if (dfMin > float.MaxValue)
                    fMin = float.MaxValue;

                if (dfMax > -float.MaxValue && dfMax < float.MaxValue)
                    fMax = (float)dfMax;
                else if (dfMin < -float.MaxValue)
                    fMax = -float.MaxValue;

                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SET_BOUNDS, m_param.AsFloat(fMin, fMax), m_param.AsLong(n, 0, 0, hX));
            }
        }

        /// <summary>
        /// Scales the data in X by a scaling factor.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="fAlpha">Specifies the scaling factor to apply to vector X, where the scaling factor is of type <code>double</code></param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="nXOff">Specifies an offset (in items, not bytes) into the memory of X.</param>
        public void scal(int n, double fAlpha, long hX, int nXOff = 0)
        {
            scal(n, (T)Convert.ChangeType(fAlpha, typeof(T)), hX, nXOff);
        }

        /// <summary>
        /// Scales the data in X by a scaling factor.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="fAlpha">Specifies the scaling factor to apply to vector X, where the scaling factor is of type <code>float</code></param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="nXOff">Specifies an offset (in items, not bytes) into the memory of X.</param>
        public void scal(int n, float fAlpha, long hX, int nXOff = 0)
        {
            scal(n, (T)Convert.ChangeType(fAlpha, typeof(T)), hX, nXOff);
        }

        /// <summary>
        /// Scales the data in X by a scaling factor.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="fAlpha">Specifies the scaling factor to apply to vector X, where the scaling factor is of type 'T'.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="nXOff">Specifies an offset (in items, not bytes) into the memory of X.</param>
        public void scal(int n, T fAlpha, long hX, int nXOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SCAL, m_param.AsDouble(convertD(fAlpha)), m_param.AsLong(n, 0, hX, nXOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SCAL, m_param.AsFloat(convertF(fAlpha)), m_param.AsLong(n, 0, hX, nXOff));
        }

        /// <summary>
        /// Computes the dot product of X and Y.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <returns>The dot product is returned as a type <code>double</code></returns>
        public double dot_double(int n, long hX, long hY)
        {
            return (double)Convert.ChangeType(dot(n, hX, hY), typeof(double));
        }

        /// <summary>
        /// Computes the dot product of X and Y.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <returns>The dot product is returned as a type <code>float</code></returns>
        public float dot_float(int n, long hX, long hY)
        {
            return (float)Convert.ChangeType(dot(n, hX, hY), typeof(float));
        }

        /// <summary>
        /// Computes the dot product of X and Y.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nXOff">Optionally, specifies an offset (in items, not bytes) into the memory of X.</param>
        /// <param name="nYOff">Optionally, specifies an offset (in items, not bytes) into the memory of Y.</param>
        /// <returns>The dot product is returned as a type 'T'.</returns>
        public T dot(int n, long hX, long hY, int nXOff = 0, int nYOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_DOT, null, m_param.AsLong(n, hX, hY, nXOff, nYOff));
                return (T)Convert.ChangeType(rg[0], typeof(T));
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_DOT, null, m_param.AsLong(n, hX, hY, nXOff, nYOff));
                return (T)Convert.ChangeType(rg[0], typeof(T));
            }
        }

        /// <summary>
        /// Computes the sum of absolute values in X.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="nXOff">Optionally, specifies an offset (in items, not bytes) into the memory of X.</param>
        /// <returns>the absolute sum is returned as a type <code>double</code></returns>
        public double asum_double(int n, long hX, int nXOff = 0)
        {
            return (double)Convert.ChangeType(asum(n, hX, nXOff), typeof(double));
        }

        /// <summary>
        /// Computes the sum of absolute values in X.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="nXOff">Optionally, specifies an offset (in items, not bytes) into the memory of X.</param>
        /// <returns>the absolute sum is returned as a type <code>float</code></returns>
        public float asum_float(int n, long hX, int nXOff = 0)
        {
            return (float)Convert.ChangeType(asum(n, hX, nXOff), typeof(float));
        }

        /// <summary>
        /// Computes the sum of absolute values in X.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="nXOff">Optionally, specifies an offset (in items, not bytes) into the memory of X.</param>
        /// <returns>the absolute value sum is returned as a type 'T'.</returns>
        public T asum(int n, long hX, int nXOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ASUM, null, m_param.AsLong(n, hX, nXOff));
                return (T)Convert.ChangeType(rg[0], typeof(T));
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ASUM, null, m_param.AsLong(n, hX, nXOff));
                return (T)Convert.ChangeType(rg[0], typeof(T));
            }
        }

        /// <summary>
        /// Scales the values in X and places them in Y.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="fAlpha">Specifies the scale value in type <code>double</code></param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void scale(int n, double fAlpha, long hX, long hY)
        {
            scale(n, (T)Convert.ChangeType(fAlpha, typeof(T)), hX, hY);
        }

        /// <summary>
        /// Scales the values in X and places them in Y.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="fAlpha">Specifies the scale value in type <code>float</code></param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void scale(int n, float fAlpha, long hX, long hY)
        {
            scale(n, (T)Convert.ChangeType(fAlpha, typeof(T)), hX, hY);
        }

        /// <summary>
        /// Scales the values in X and places them in Y.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuBlas](https://developer.nvidia.com/cublas).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X and Y.</param>
        /// <param name="fAlpha">Specifies the scale value in type 'T'.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nXOff">Optionally, specifies an offset (in items, not bytes) into the memory of X.</param>
        /// <param name="nYOff">Optionally, specifies an offset (in items, not bytes) into the memory of Y.</param>
        public void scale(int n, T fAlpha, long hX, long hY, int nXOff = 0, int nYOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SCALE, m_param.AsDouble(convertD(fAlpha)), m_param.AsLong(n, 0, hX, hY, nXOff, nYOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SCALE, m_param.AsFloat(convertF(fAlpha)), m_param.AsLong(n, 0, hX, hY, nXOff, nYOff));
        }

        /// <summary>
        /// Scales the values in X and places the result in Y (can also run inline where X = Y).
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="fMin">Specifies the minimum of the new range.</param>
        /// <param name="fMax">Specifies the maximum of the new range.</param>
        public void scale_to_range(int n, long hX, long hY, double fMin, double fMax)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SCALE_TO_RANGE, m_param.AsDouble(fMin, fMax), m_param.AsLong(n, hX, hY, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SCALE_TO_RANGE, m_param.AsFloat((float)fMin, (float)fMax), m_param.AsLong(n, hX, hY, 0, 0));
        }

        /// <summary>
        /// Calculates the erf() function.
        /// </summary>
        /// <param name="dfVal">Specifies the input value.</param>
        /// <returns>The erf result is returned.</returns>
        public double erf(double dfVal)
        {
            return convertD(erf(convertD1(dfVal)));
        }

        /// <summary>
        /// Calculates the erf() function.
        /// </summary>
        /// <param name="fVal">Specifies the input value.</param>
        /// <returns>The erf result is returned.</returns>
        public float erf(float fVal)
        {
            return convertF(erf(convertF1(fVal)));
        }

        /// <summary>
        /// Calculates the erf() function.
        /// </summary>
        /// <param name="fVal">Specifies the input value.</param>
        /// <returns>The erf result is returned.</returns>
        public T erf(T fVal)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CUDA_ERF, m_param.AsDouble(convertD(fVal)));
                return convert(rg)[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CUDA_ERF, m_param.AsFloat(convertF(fVal)));
                return convert(rg)[0];
            }
        }

        /// <summary>
        /// Mask the mask the data in the source with the mask by replacing all values 'fSearch' found in the mask with 'fReplace' in the destination.
        /// </summary>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="nMaskDim">Specifies the number of items in the mask.</param>
        /// <param name="fSearch">Specifies the value within the mask to replace.</param>
        /// <param name="fReplace">Specifies the replacement value.</param>
        /// <param name="hX">Specifies a handle to the GPU memory of the source.</param>
        /// <param name="hMask">Specifies a handle to the GPU memory of the mask (containing the 'fSearch' values)</param>
        /// <param name="hY">Specifies a handle to the GPU memory of the destination.</param>
        public void mask(int n, int nMaskDim, T fSearch, T fReplace, long hX, long hMask, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MASK, m_param.AsDouble(convertD(fSearch), convertD(fReplace)), m_param.AsLong(n, nMaskDim, 0, 0, hX, hMask, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MASK, m_param.AsFloat(convertF(fSearch), convertF(fReplace)), m_param.AsLong(n, nMaskDim, 0, 0, hX, hMask, hY));
        }

        /// <summary>
        /// Mask the mask the data in the source with the mask by replacing all values 'fSearch' found in the mask with 'fReplace' in the destination.
        /// </summary>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="nMaskDim">Specifies the number of items in the mask.</param>
        /// <param name="fSearch">Specifies the value within the mask to replace.</param>
        /// <param name="fReplace">Specifies the replacement value.</param>
        /// <param name="hX">Specifies a handle to the GPU memory of the source.</param>
        /// <param name="hMask">Specifies a handle to the GPU memory of the mask (containing the 'fSearch' values)</param>
        /// <param name="hY">Specifies a handle to the GPU memory of the destination.</param>
        public void mask(int n, int nMaskDim, double fSearch, double fReplace, long hX, long hMask, long hY)
        {
            mask(n, nMaskDim, (T)Convert.ChangeType(fSearch, typeof(T)), (T)Convert.ChangeType(fReplace, typeof(T)), hX, hMask, hY);
        }

        /// <summary>
        /// Mask the mask the data in the source with the mask by replacing all values 'fSearch' found in the mask with 'fReplace' in the destination.
        /// </summary>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="nMaskDim">Specifies the number of items in the mask.</param>
        /// <param name="fSearch">Specifies the value within the mask to replace.</param>
        /// <param name="fReplace">Specifies the replacement value.</param>
        /// <param name="hX">Specifies a handle to the GPU memory of the source.</param>
        /// <param name="hMask">Specifies a handle to the GPU memory of the mask (containing the 'fSearch' values)</param>
        /// <param name="hY">Specifies a handle to the GPU memory of the destination.</param>
        public void mask(int n, int nMaskDim, float fSearch, float fReplace, long hX, long hMask, long hY)
        {
            mask(n, nMaskDim, (T)Convert.ChangeType(fSearch, typeof(T)), (T)Convert.ChangeType(fReplace, typeof(T)), hX, hMask, hY);
        }

        /// <summary>
        /// Mask the mask the batch of data in the source with the mask by replacing all values 'fSearch' found in the mask with 'fReplace' in the destination.
        /// </summary>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="nBatch">Specifies the batch size.</param>
        /// <param name="nMaskDim">Specifies the number of items in the mask.</param>
        /// <param name="fSearch">Specifies the value within the mask to replace.</param>
        /// <param name="fReplace">Specifies the replacement value.</param>
        /// <param name="hX">Specifies a handle to the GPU memory of the source.</param>
        /// <param name="hMask">Specifies a handle to the GPU memory of the mask (containing the 'fSearch' values)</param>
        /// <param name="hY">Specifies a handle to the GPU memory of the destination.</param>
        public void mask_batch(int n, int nBatch, int nMaskDim, T fSearch, T fReplace, long hX, long hMask, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MASK_BATCH, m_param.AsDouble(convertD(fSearch), convertD(fReplace)), m_param.AsLong(n, nBatch, nMaskDim, 0, 0, hX, hMask, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MASK_BATCH, m_param.AsFloat(convertF(fSearch), convertF(fReplace)), m_param.AsLong(n, nBatch, nMaskDim, 0, 0, hX, hMask, hY));
        }

        /// <summary>
        /// Mask the mask the batch of data in the source with the mask by replacing all values 'fSearch' found in the mask with 'fReplace' in the destination.
        /// </summary>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="nBatch">Specifies the batch size.</param>
        /// <param name="nMaskDim">Specifies the number of items in the mask.</param>
        /// <param name="fSearch">Specifies the value within the mask to replace.</param>
        /// <param name="fReplace">Specifies the replacement value.</param>
        /// <param name="hX">Specifies a handle to the GPU memory of the source.</param>
        /// <param name="hMask">Specifies a handle to the GPU memory of the mask (containing the 'fSearch' values)</param>
        /// <param name="hY">Specifies a handle to the GPU memory of the destination.</param>
        public void mask_batch(int n, int nBatch, int nMaskDim, double fSearch, double fReplace, long hX, long hMask, long hY)
        {
            mask_batch(n, nBatch, nMaskDim, (T)Convert.ChangeType(fSearch, typeof(T)), (T)Convert.ChangeType(fReplace, typeof(T)), hX, hMask, hY);
        }

        /// <summary>
        /// Mask the mask the batch of data in the source with the mask by replacing all values 'fSearch' found in the mask with 'fReplace' in the destination.
        /// </summary>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="nBatch">Specifies the batch size.</param>
        /// <param name="nMaskDim">Specifies the number of items in the mask.</param>
        /// <param name="fSearch">Specifies the value within the mask to replace.</param>
        /// <param name="fReplace">Specifies the replacement value.</param>
        /// <param name="hX">Specifies a handle to the GPU memory of the source.</param>
        /// <param name="hMask">Specifies a handle to the GPU memory of the mask (containing the 'fSearch' values)</param>
        /// <param name="hY">Specifies a handle to the GPU memory of the destination.</param>
        public void mask_batch(int n, int nBatch, int nMaskDim, float fSearch, float fReplace, long hX, long hMask, long hY)
        {
            mask_batch(n, nBatch, nMaskDim, (T)Convert.ChangeType(fSearch, typeof(T)), (T)Convert.ChangeType(fReplace, typeof(T)), hX, hMask, hY);
        }

        /// <summary>
        /// Interpolates between two sizes within the spatial dimensions.
        /// </summary>
        /// <param name="nChannels">Specifies the channels (usually num * channels)</param>
        /// <param name="hData1">Specifies the input data when bBwd=false and the output data when bBwd=true.</param>
        /// <param name="nX1">Specifies the offset along the x axis for data1.</param>
        /// <param name="nY1">Specifies the offset along the y axis for data1.</param>
        /// <param name="nHeight1">Specifies the effective height for data1.</param>
        /// <param name="nWidth1">Specifies the effective width for data1.</param>
        /// <param name="nHeight1A">Specifies the input height for data1.</param>
        /// <param name="nWidth1A">Specifies the input width for data1.</param>
        /// <param name="hData2">Specifies the output data when bBwd=false and the input data when bBwd=true.</param>
        /// <param name="nX2">Specifies the offset along the x axis for data2.</param>
        /// <param name="nY2">Specifies the offset along the y axis for data2.</param>
        /// <param name="nHeight2">Specifies the effective height for data2.</param>
        /// <param name="nWidth2">Specifies the effective width for data2.</param>
        /// <param name="nHeight2A">Specifies the output height for data2.</param>
        /// <param name="nWidth2A">Specifies the output width for data2.</param>
        /// <param name="bBwd">Optionally, specifies to perform the backward operation from data2 to data1, otherwise the operation performs on data1 to data2. (default = false).</param>
        public void interp2(int nChannels, long hData1, int nX1, int nY1, int nHeight1, int nWidth1, int nHeight1A, int nWidth1A, long hData2, int nX2, int nY2, int nHeight2, int nWidth2, int nHeight2A, int nWidth2A, bool bBwd = false)
        {
            if (!(nX1 >= 0 && nY1 >= 0 && nHeight1 > 0 && nWidth1 > 0 && nX2 >= 0 && nY2 >= 0 && nHeight2 > 0 && nWidth2 > 0))
                throw new ArgumentOutOfRangeException("interp2: Invalid arguments found.");

            if (!(nWidth1A >= nWidth1 + nX1 && nHeight1A >= nHeight1 + nY1 && nWidth2A >= nWidth2 + nX2 && nHeight2A >= nHeight2 + nY2))
                throw new ArgumentOutOfRangeException("interp2: Invalid arguments found.");

            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_INTERP2, null, m_param.AsLong(nChannels, hData1, nX1, nY1, nHeight1, nWidth1, nHeight1A, nWidth1A, hData2, nX2, nY2, nHeight2, nWidth2, nHeight2A, nWidth2A, (bBwd) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_INTERP2, null, m_param.AsLong(nChannels, hData1, nX1, nY1, nHeight1, nWidth1, nHeight1A, nWidth1A, hData2, nX2, nY2, nHeight2, nWidth2, nHeight2A, nWidth2A, (bBwd) ? 1 : 0));
        }

        /// <summary>
        /// Adds a scalar value to each element of Y.
        /// </summary>
        /// <remarks>
        /// Y = Y + alpha
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector Y.</param>
        /// <param name="fAlpha">Specifies the scalar value in type <code>double</code></param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void add_scalar(int n, double fAlpha, long hY)
        {
            add_scalar(n, (T)Convert.ChangeType(fAlpha, typeof(T)), hY);
        }

        /// <summary>
        /// Adds a scalar value to each element of Y.
        /// </summary>
        /// <remarks>
        /// Y = Y + alpha
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector Y.</param>
        /// <param name="fAlpha">Specifies the scalar value in type <code>float</code></param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void add_scalar(int n, float fAlpha, long hY)
        {
            add_scalar(n, (T)Convert.ChangeType(fAlpha, typeof(T)), hY);
        }

        /// <summary>
        /// Adds a scalar value to each element of Y.
        /// </summary>
        /// <remarks>
        /// Y = Y + alpha
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector Y.</param>
        /// <param name="fAlpha">Specifies the scalar value in type 'T'.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nYOff">Optionally, specifies an offset into Y.  The default is 0.</param>
        public void add_scalar(int n, T fAlpha, long hY, int nYOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADD_SCALAR, m_param.AsDouble(convertD(fAlpha)), m_param.AsLong(n, 0, hY, nYOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADD_SCALAR, m_param.AsFloat(convertF(fAlpha)), m_param.AsLong(n, 0, hY, nYOff));
        }

        /// <summary>
        /// Adds A, B and C and places the result in Y.
        /// </summary>
        /// <remarks>
        /// Y = A + B + C
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="hC">Specifies a handle to the vector C in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void add(int n, long hA, long hB, long hC, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADD3, null, m_param.AsLong(n, hA, hB, hC, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADD3, null, m_param.AsLong(n, hA, hB, hC, hY));
        }

        /// <summary>
        /// Adds A to B and places the result in Y.
        /// </summary>
        /// <remarks>
        /// Y = A + B
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void add(int n, long hA, long hB, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADD, null, m_param.AsLong(n, hA, hB, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADD, null, m_param.AsLong(n, hA, hB, hY));
        }

        /// <summary>
        /// Adds A to (B times scalar) and places the result in Y. 
        /// </summary>
        /// <remarks>
        /// Y = A + (B * alpha)
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="dfAlpha">Specifies a scalar int type <code>double</code></param>
        public void add(int n, long hA, long hB, long hY, double dfAlpha)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADD, m_param.AsDouble(dfAlpha), m_param.AsLong(n, hA, hB, hY, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADD, m_param.AsFloat((float)dfAlpha), m_param.AsLong(n, hA, hB, hY, 0));
        }

        /// <summary>
        /// Adds A to (B times scalar) and places the result in Y.
        /// </summary>
        /// <remarks>
        /// Y = A + (B * alpha)
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="fAlpha">Specifies a scalar int type <code>float</code></param>
        public void add(int n, long hA, long hB, long hY, float fAlpha)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADD, m_param.AsDouble(fAlpha), m_param.AsLong(n, hA, hB, hY, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADD, m_param.AsFloat(fAlpha), m_param.AsLong(n, hA, hB, hY, 0));
        }

        /// <summary>
        /// Adds A to (B times scalar) and places the result in Y.
        /// </summary>
        /// <remarks>
        /// Y = A + (B * alpha)
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="dfAlphaA">Specifies a scalar int type 'T' applied to A.</param>
        /// <param name="dfAlphaB">Specifies a scalar int type 'T' applied to B.</param>
        /// <param name="nAOff">Optionally, specifies an offset (in items, not bytes) into the memory of A.</param>
        /// <param name="nBOff">Optionally, specifies an offset (in items, not bytes) into the memory of B.</param>
        /// <param name="nYOff">Optionally, specifies an offset (in items, not bytes) into the memory of Y.</param>
        public void add(int n, long hA, long hB, long hY, double dfAlphaA, double dfAlphaB, int nAOff = 0, int nBOff = 0, int nYOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADD2, m_param.AsDouble(dfAlphaA, dfAlphaB), m_param.AsLong(n, hA, hB, hY, 0, 0, nAOff, nBOff, nYOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADD2, m_param.AsFloat((float)dfAlphaA, (float)dfAlphaB), m_param.AsLong(n, hA, hB, hY, 0, 0, nAOff, nBOff, nYOff));
        }

        /// <summary>
        /// Subtracts B from A and places the result in Y.
        /// </summary>
        /// <remarks>
        /// Y = A - B
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nAOff">Optionally, specifies an offset (in items, not bytes) into the memory of A.</param>
        /// <param name="nBOff">Optionally, specifies an offset (in items, not bytes) into the memory of B.</param>
        /// <param name="nYOff">Optionally, specifies an offset (in items, not bytes) into the memory of Y.</param>
        /// <param name="nB">Optionally, specifies a number of 'B' items to subtract (default = 0 which causes ALL items in B to be subtracted).
        /// When 'nB' > 0, it must be a factor of 'n' and causes that number of B items to be subtracted as a block from A.
        /// </param>
        public void sub(int n, long hA, long hB, long hY, int nAOff = 0, int nBOff = 0, int nYOff = 0, int nB = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SUB, null, m_param.AsLong(n, hA, hB, hY, nAOff, nBOff, nYOff, nB));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SUB, null, m_param.AsLong(n, hA, hB, hY, nAOff, nBOff, nYOff, nB));
        }


        /// <summary>
        /// Multiplies each element of A with each element of B and places the result in Y.
        /// </summary>
        /// <remarks>
        /// Y = A * B (element by element)
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nAOff">Optionally, specifies an offset (in items, not bytes) into the memory of A.</param>
        /// <param name="nBOff">Optionally, specifies an offset (in items, not bytes) into the memory of B.</param>
        /// <param name="nYOff">Optionally, specifies an offset (in items, not bytes) into the memory of Y.</param>
        public void mul(int n, long hA, long hB, long hY, int nAOff = 0, int nBOff = 0, int nYOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MUL, null, m_param.AsLong(n, hA, hB, hY, nAOff, nBOff, nYOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MUL, null, m_param.AsLong(n, hA, hB, hY, nAOff, nBOff, nYOff));
        }

        /// <summary>
        /// Multiplies each element of X with A, and adds to X and places the result in Y.
        /// </summary>
        /// <remarks>
        /// Y = X * A + X (element by element, when FWD)
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hX">Specifies a handle to the input vector X in GPU memory.</param>
        /// <param name="hA">Specifies a handle to the attention vector A in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the output vector Y in GPU memory.</param>
        /// <param name="dir">Specifies the direction where FWD outputs to hY, and BWD outputs to hX.</param>
        /// <param name="hdA">When BWD, specifies the gradient output for X, expects hY to contain the gradient for Y.</param>
        /// <param name="hdX">When BWD, specifies the gradient output for A, expects hY to contain the gradient for Y.</param>
        public void muladd(int n, long hX, long hA, long hY, DIR dir, long hdX = 0, long hdA = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MULADD, null, m_param.AsLong(n, hX, hA, hY, (int)dir, hdX, hdA));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MULADD, null, m_param.AsLong(n, hX, hA, hY, (int)dir, hdX, hdA));
        }

        /// <summary>
        /// Runs a z-score normalization on the data where the forward pass subtracts the mean and divides by the std deviation, and the backward pass computes the gradient.
        /// </summary>
        /// <param name="n">Specifies the number of items in vectors X and Y.</param>
        /// <param name="hX">Specifies the GPU memory containing the input data (FWD) and output data (BWD).</param>
        /// <param name="fMeanPos">Specifies the positive mean value.</param>
        /// <param name="fStdDevPos">Specifies the positive stdev value.</param>
        /// <param name="fMeanNeg">Specifies the negative mean value.</param>
        /// <param name="fStdDevNeg">Specifies the negative stdev value.</param>
        /// <param name="hY">Specifies the output data (FWD) and input data (BWD).</param>
        /// <param name="dir">Specifies the data flow direction.</param>
        /// <param name="method">Specifies the Z-Score method.  When using the POSNEG z-score normalization is run separately on positive and negative values.</param>
        public void z_score(int n, long hX, double fMeanPos, double fStdDevPos, double fMeanNeg, double fStdDevNeg, long hY, DIR dir, SCORE_AS_LABEL_NORMALIZATION method)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_Z_SCORE, m_param.AsDouble(fMeanPos, fStdDevPos, fMeanNeg, fStdDevNeg), m_param.AsLong(n, hX, hY, (int)dir, (int)method));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_Z_SCORE, m_param.AsFloat((float)fMeanPos, (float)fStdDevPos, (float)fMeanNeg, (float)fStdDevNeg), m_param.AsLong(n, hX, hY, (int)dir, (int)method));
        }

        /// <summary>
        /// Subtracts every <i>nInnterNum</i> element of B from A and performs a dot product on the result.
        /// </summary>
        /// <remarks>
        /// Y[i] = (A[i] - B[i%nInnerNum]) * (A[i] - B[i%nInnerNum])
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="nN">Specifies the inner count.</param>
        /// <param name="nInnerNum">Specifies the dimension.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nAOff">Optionally, specifies an offset (in items, not bytes) into the memory of A.</param>
        /// <param name="nBOff">Optionally, specifies an offset (in items, not bytes) into the memory of B.</param>
        /// <param name="nYOff">Optionally, specifies an offset (in items, not bytes) into the memory of Y.</param>
        public void sub_and_dot(int n, int nN, int nInnerNum, long hA, long hB, long hY, int nAOff, int nBOff, int nYOff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SUB_AND_DOT, null, m_param.AsLong(n, nN, nInnerNum, hA, hB, hY, nAOff, nBOff, nYOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SUB_AND_DOT, null, m_param.AsLong(n, nN, nInnerNum, hA, hB, hY, nAOff, nBOff, nYOff));
        }

        /// <summary>
        /// Mutlipy each element of Y by a scalar.
        /// </summary>
        /// <remarks>
        /// Y = Y * alpha
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors Y.</param>
        /// <param name="fAlpha">Specifies the scalar in type <code>double</code></param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void mul_scalar(int n, double fAlpha, long hY)
        {
            mul_scalar(n, (T)Convert.ChangeType(fAlpha, typeof(T)), hY);
        }

        /// <summary>
        /// Mutlipy each element of Y by a scalar.
        /// </summary>
        /// <remarks>
        /// Y = Y * alpha
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors Y.</param>
        /// <param name="fAlpha">Specifies the scalar in type <code>float</code></param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void mul_scalar(int n, float fAlpha, long hY)
        {
            mul_scalar(n, (T)Convert.ChangeType(fAlpha, typeof(T)), hY);
        }

        /// <summary>
        /// Mutlipy each element of Y by a scalar.
        /// </summary>
        /// <remarks>
        /// Y = Y * alpha
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors Y.</param>
        /// <param name="fAlpha">Specifies the scalar in type 'T'.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void mul_scalar(int n, T fAlpha, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MUL_SCALAR, m_param.AsDouble(convertD(fAlpha)), m_param.AsLong(n, 0, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MUL_SCALAR, m_param.AsFloat(convertF(fAlpha)), m_param.AsLong(n, 0, hY));
        }

        /// <summary>
        /// Divides each element of A by each element of B and places the result in Y.
        /// </summary>
        /// <remarks>
        /// Y = A / B (element by element)
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void div(int n, long hA, long hB, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_DIV, null, m_param.AsLong(n, hA, hB, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_DIV, null, m_param.AsLong(n, hA, hB, hY));
        }

        /// <summary>
        /// Calculates the absolute value of A and places the result in Y.
        /// </summary>
        /// <remarks>
        /// Y = abs(X)
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void abs(int n, long hA, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ABS, null, m_param.AsLong(n, hA, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ABS, null, m_param.AsLong(n, hA, hY));
        }

        /// <summary>
        /// Calculates the exponent value of A and places the result in Y.
        /// </summary>
        /// <remarks>
        /// @f$ f(x) = exp(x) @f$
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void exp(int n, long hA, long hY)
        {
            exp(n, hA, hY, 0, 0, 1.0);
        }

        /// <summary>
        /// Calculates the exponent value of A * beta and places the result in Y.
        /// </summary>
        /// <remarks>
        /// @f$ f(x) = exp(x * \beta) @f$
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nAOff">Specifies an offset (in items, not bytes) into the memory of A.</param>
        /// <param name="nYOff">Specifies an offset (in items, not bytes) into the memory of Y.</param>
        /// <param name="dfBeta">Specifies the scalar as type <code>double</code></param>
        public void exp(int n, long hA, long hY, int nAOff, int nYOff, double dfBeta)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_EXP, m_param.AsDouble(dfBeta), m_param.AsLong(n, hA, hY, nAOff, nYOff, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_EXP, m_param.AsFloat((float)dfBeta), m_param.AsLong(n, hA, hY, nAOff, nYOff, 0));
        }

        /// <summary>
        /// Calculates the log value of A and places the result in Y.
        /// </summary>
        /// <remarks>
        /// @f$ f(x) = log(x) @f$
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void log(int n, long hA, long hY)
        {
            log(n, hA, hY, 1.0, 0.0);
        }

        /// <summary>
        /// Calculates the log value of (A * beta) + alpha, and places the result in Y.
        /// </summary>
        /// <remarks>
        /// @f$ f(x) = \ln((x * \beta) + \alpha) @f$
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="dfBeta">Specifies the scalar as type <code>double</code> that is multiplied with the log.</param>
        /// <param name="dfAlpha">Optionally, specifies a scalar added to the value before taking the log.</param>
        public void log(int n, long hA, long hY, double dfBeta, double dfAlpha = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LOG, m_param.AsDouble(dfBeta, dfAlpha), m_param.AsLong(n, hA, hY, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LOG, m_param.AsFloat((float)dfBeta, (float)dfAlpha), m_param.AsLong(n, hA, hY, 0, 0));
        }

        /// <summary>
        /// Calculates the A raised to the power alpha and places the result in Y.
        /// </summary>
        /// <remarks>
        /// @f$ f(x) = x^\alpha @f$
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="fAlpha">Specifies the scalar in type <code>double</code></param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nAOff">Optionally, specifies the offset for hA memory (default = 0).</param>
        /// <param name="nYOff">Optionally, specifies the offset for hY memory (default = 0).</param>
        public void powx(int n, long hA, double fAlpha, long hY, int nAOff = 0, int nYOff = 0)
        {
            powx(n, hA, (T)Convert.ChangeType(fAlpha, typeof(T)), hY, nAOff, nYOff);
        }

        /// <summary>
        /// Calculates the A raised to the power alpha and places the result in Y.
        /// </summary>
        /// <remarks>
        /// @f$ f(x) = x^\alpha @f$
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="fAlpha">Specifies the scalar in type <code>float</code></param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nAOff">Optionally, specifies the offset for hA memory (default = 0).</param>
        /// <param name="nYOff">Optionally, specifies the offset for hY memory (default = 0).</param>
        public void powx(int n, long hA, float fAlpha, long hY, int nAOff = 0, int nYOff = 0)
        {
            powx(n, hA, (T)Convert.ChangeType(fAlpha, typeof(T)), hY, nAOff, nYOff);
        }

        /// <summary>
        /// Calculates the A raised to the power alpha and places the result in Y.
        /// </summary>
        /// <remarks>
        /// @f$ f(x) = x^\alpha @f$
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="fAlpha">Specifies the scalar in type 'T'.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nAOff">Optionally, specifies the offset for hA memory (default = 0).</param>
        /// <param name="nYOff">Optionally, specifies the offset for hY memory (default = 0).</param>
        public void powx(int n, long hA, T fAlpha, long hY, int nAOff = 0, int nYOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_POWX, m_param.AsDouble(convertD(fAlpha)), m_param.AsLong(n, hA, 0, hY, nAOff, nYOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_POWX, m_param.AsFloat(convertF(fAlpha)), m_param.AsLong(n, hA, 0, hY, nAOff, nYOff));
        }

        /// <summary>
        /// Computes the sign of each element of X and places the result in Y.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nXOff">Specifies an offset (in items, not bytes) into the memory of X.</param>
        /// <param name="nYOff">Specifies an offset (in items, not bytes) into the memory of Y.</param>
        public void sign(int n, long hX, long hY, int nXOff = 0, int nYOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SIGN, null, m_param.AsLong(n, hX, hY, nXOff, nYOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SIGN, null, m_param.AsLong(n, hX, hY, nXOff, nYOff));
        }

        /// <summary>
        /// Computes the invert of each element of X and places the result in Y where the inver Y = 1/X.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nXOff">Specifies an offset (in items, not bytes) into the memory of X.</param>
        /// <param name="nYOff">Specifies an offset (in items, not bytes) into the memory of Y.</param>
        /// <param name="dfScaleDenom">Optionally, scale the numerator (default = 1.)</param>
        /// <param name="dfScaleNumerator">Optionally, scale the denominator (default = 1).</param>
        public void invert(int n, long hX, long hY, int nXOff = 0, int nYOff = 0, double dfScaleNumerator = 1, double dfScaleDenom = 1)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_INVERT, m_param.AsDouble(dfScaleNumerator, dfScaleDenom), m_param.AsLong(n, hX, hY, nXOff, nYOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_INVERT, m_param.AsFloat((float)dfScaleNumerator, (float)dfScaleDenom), m_param.AsLong(n, hX, hY, nXOff, nYOff));
        }

        /// <summary>
        /// Computes the threshold, setting all values above (or below, depending on the side) the threshold value to 0.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="fThreshold">Specifies the threshold value.</param>
        /// <param name="side">Specifies the side of the threshold.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void threshold(int n, long hX, float fThreshold, SIDE side, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_THRESHOLD, m_param.AsDouble(fThreshold), m_param.AsLong(n, hX, (int)side, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_THRESHOLD, m_param.AsFloat(fThreshold), m_param.AsLong(n, hX, (int)side, hY));
        }

#pragma warning disable 1591

        public void student(int n, long hX, long hY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_STUDENT, null, m_param.AsLong(n, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_STUDENT, null, m_param.AsLong(n, hX, hY));
        }

        public void logistic1(int n, long hX, long hY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LOGISTIC1, null, m_param.AsLong(n, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LOGISTIC1, null, m_param.AsLong(n, hX, hY));
        }

        public void logistic2(int n, long hX, long hY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LOGISTIC2, null, m_param.AsLong(n, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LOGISTIC2, null, m_param.AsLong(n, hX, hY));
        }

        public void reciprocol(int n, long hX, long hY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_RECIPROCOL, null, m_param.AsLong(n, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_RECIPROCOL, null, m_param.AsLong(n, hX, hY));
        }

#pragma warning restore 1591

        /// <summary>
        /// Computes the square root of each element of X and places the result in Y.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="dfEpsilon">Optionally, add an epsilont value before performing the square root.</param>
        public void sqrt(int n, long hX, long hY, double dfEpsilon = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SQRT, m_param.AsDouble(dfEpsilon), m_param.AsLong(n, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SQRT, m_param.AsFloat((float)dfEpsilon), m_param.AsLong(n, hX, hY));
        }

        /// <summary>
        /// Computes the invers square root of each element of X and places the result in Y.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and Y.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="dfEpsilon">Optionally, add an epsilont value before performing the square root.</param>
        public void rsqrt(int n, long hX, long hY, double dfEpsilon = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_RSQRT, m_param.AsDouble(dfEpsilon), m_param.AsLong(n, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_RSQRT, m_param.AsFloat((float)dfEpsilon), m_param.AsLong(n, hX, hY));
        }

        /// <summary>
        /// Scale the data by the sqrt of the data.  y = sqrt(abs(x)) * sign(x)
        /// </summary>
        /// <param name="nCount">Specifies the number of elements.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void sqrt_scale(int nCount, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SQRT_SCALE, null, m_param.AsLong(nCount, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SQRT_SCALE, null, m_param.AsLong(nCount, hX, hY));
        }

        /// <summary>
        /// Compares the signs of each value in A and B and places the result in Y.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void compare_signs(int n, long hA, long hB, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COMPARE_SIGNS, null, m_param.AsLong(n, hA, hB, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COMPARE_SIGNS, null, m_param.AsLong(n, hA, hB, hY));
        }

        /// <summary>
        /// Calculates the max of A and B and places the result in Y.  This max is only computed on a per item basis,
        /// so the shape of Y = the shape of A and B and Y(0) contains the max of A(0) and B(0), etc.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void max(int n, long hA, long hB, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MAX, null, m_param.AsLong(n, hA, hB, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MAX, null, m_param.AsLong(n, hA, hB, hY));
        }

        /// <summary>
        /// Propagates the Y diff  back to the max of A or B and places the result in A if its data has the max, or B if its data has the max.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hAdata">Specifies a handle to the data vector A in GPU memory.</param>
        /// <param name="hBdata">Specifies a handle to the data vector B in GPU memory.</param>
        /// <param name="hYdiff">Specifies a handle to the diff vector Y in GPU memory.</param>
        /// <param name="hAdiff">Specifies a handle to the mutable diff vector A in GPU memory.</param>
        /// <param name="hBdiff">Specifies a handle to the mutable diff vector B in GPU memory.</param>
        public void max_bwd(int n, long hAdata, long hBdata, long hYdiff, long hAdiff, long hBdiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MAX_BWD2, null, m_param.AsLong(n, hAdata, hBdata, hYdiff, hAdiff, hBdiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MAX_BWD2, null, m_param.AsLong(n, hAdata, hBdata, hYdiff, hAdiff, hBdiff));
        }

        /// <summary>
        /// Calculates the min of A and B and places the result in Y.  This min is only computed on a per item basis,
        /// so the shape of Y = the shape of A and B and Y(0) contains the min of A(0) and B(0), etc.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and Y.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void min(int n, long hA, long hB, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MIN, null, m_param.AsLong(n, hA, hB, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MIN, null, m_param.AsLong(n, hA, hB, hY));
        }

        /// <summary>
        /// Finds the maximum value of A.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's Thrust](https://developer.nvidia.com/thrust).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="lPos">Returns the position of the maximum value.</param>
        /// <param name="nAOff">Optionally, specifies an offset (in items, not bytes) into the memory of A (default = 0).</param>
        /// <param name="hWork">Optionally, specifies the handle to GPU memory in the size of A, which when specified is used in the 
        /// extended version of max val.  The extended version does not use thrust, and does not calculate 'lPos', which is always
        /// returned as -1 when using the extended version. (default = 0, use non extended version)</param>
        /// <returns>The maximum value is returned as type <code>double</code></returns>
        public double max(int n, long hA, out long lPos, int nAOff = 0, long hWork = 0)
        {
            if (hWork != 0)
            {
                if (m_dt == DataType.DOUBLE)
                {
                    double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MAXVAL, null, m_param.AsLong(n, hA, nAOff, hWork));
                    lPos = (long)rg[1];
                    return rg[0];
                }
                else
                {
                    float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MAXVAL, null, m_param.AsLong(n, hA, nAOff, hWork));
                    lPos = (long)rg[1];
                    return rg[0];
                }
            }
            else
            {
                if (m_dt == DataType.DOUBLE)
                {
                    double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MAXVAL, null, m_param.AsLong(n, hA, nAOff));
                    lPos = (long)rg[1];
                    return rg[0];
                }
                else
                {
                    float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MAXVAL, null, m_param.AsLong(n, hA, nAOff));
                    lPos = (long)rg[1];
                    return rg[0];
                }
            }
        }

        /// <summary>
        /// Finds the minimum value of A.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's Thrust](https://developer.nvidia.com/thrust).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="lPos">Returns the position of the minimum value.</param>
        /// <param name="nAOff">Optionally, specifies an offset (in items, not bytes) into the memory of A (default = 0).</param>
        /// <param name="hWork">Optionally, specifies the handle to GPU memory in the size of A, which when specified is used in the 
        /// extended version of max val.  The extended version does not use thrust, and does not calculate 'lPos', which is always
        /// returned as -1 when using the extended version. (default = 0, use non extended version)</param>
        /// <returns>The minimum value is returned as type <code>double</code></returns>
        public double min(int n, long hA, out long lPos, int nAOff = 0, long hWork = 0)
        {
            if (hWork != 0)
            {
                if (m_dt == DataType.DOUBLE)
                {
                    double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MINVAL, null, m_param.AsLong(n, hA, nAOff, hWork));
                    lPos = (long)rg[1];
                    return rg[0];
                }
                else
                {
                    float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MINVAL, null, m_param.AsLong(n, hA, nAOff, hWork));
                    lPos = (long)rg[1];
                    return rg[0];
                }
            }
            else
            {
                if (m_dt == DataType.DOUBLE)
                {
                    double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MINVAL, null, m_param.AsLong(n, hA, nAOff));
                    lPos = (long)rg[1];
                    return rg[0];
                }
                else
                {
                    float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MINVAL, null, m_param.AsLong(n, hA, nAOff));
                    lPos = (long)rg[1];
                    return rg[0];
                }
            }
        }

        /// <summary>
        /// Finds the minimum and maximum values within A.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vector A.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hWork1">Specifies a handle to workspace data in GPU memory.  To get the size of the workspace memory, call this function with hA = 0.</param>
        /// <param name="hWork2">Specifies a handle to workspace data in GPU memory.  To get the size of the workspace memory, call this function with hA = 0.</param>
        /// <param name="bDetectNans">Optionally, specifies whether or not to detect Nans.</param>
        /// <param name="nAOff">Optionally, specifies an offset (in items, not bytes) into the memory of A.</param>
        /// <returns>A four element tuple is returned where the first item contains the minimum, the second item contains the maximum, the third contains the number
        /// of NaN values and the fourth contains the number of Infinity values.  
        /// When calling this function with <code>hA = 0</code> the function instead returns the required size of <i>hWork1</i>, <i>hWork2</i>, 0, 0 (in items, not bytes).</returns>
        public Tuple<double, double, double, double> minmax(int n, long hA, long hWork1, long hWork2, bool bDetectNans = false, int nAOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MINMAXVAL, null, m_param.AsLong(n, hA, hWork1, hWork2, (bDetectNans) ? 1 : 0, nAOff));
                return new Tuple<double, double, double, double>(rg[0], rg[1], rg[2], rg[3]);
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MINMAXVAL, null, m_param.AsLong(n, hA, hWork1, hWork2, (bDetectNans) ? 1 : 0, nAOff));
                return new Tuple<double, double, double, double>(rg[0], rg[1], rg[2], rg[3]);
            }
        }

        /// <summary>
        /// Finds up to 'nK' minimum and maximum values within A.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vector A.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hWork1">Specifies a handle to workspace data in GPU memory.  To get the size of the workspace memory, call this function with hA = 0.</param>
        /// <param name="hWork2">Specifies a handle to workspace data in GPU memory.  To get the size of the workspace memory, call this function with hA = 0.</param>
        /// <param name="nK">Specifies the number of min and max values to find.</param>
        /// <param name="hMin">Specifies a handle to host memory allocated with AllocHostBuffer in the length 'nK' where the min values are placed.</param>
        /// <param name="hMax">Specifies a handle to host memory allocated with AllocHostBuffer in the length 'nK' where the min values are placed.</param>
        /// <param name="bNonZeroOnly">Specifies whether or not to exclude zero from the min and max calculations.</param>
        public void minmax(int n, long hA, long hWork1, long hWork2, int nK, long hMin, long hMax, bool bNonZeroOnly)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MINMAXVEC, null, m_param.AsLong(n, hA, hWork1, hWork2, nK, hMin, hMax, (bNonZeroOnly) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MINMAXVEC, null, m_param.AsLong(n, hA, hWork1, hWork2, nK, hMin, hMax, (bNonZeroOnly) ? 1 : 0));
        }

        /// <summary>
        /// Perform a transpose on X producing Y, similar to the numpy.transpose operation.
        /// </summary>
        /// <param name="n">Specifies the number of items in both hX and hY (must be the same).</param>
        /// <param name="hX">Specifies a handle to the input data in gpu memory.</param>
        /// <param name="hY">Specifies a handle to the output data in gpu memory.</param>
        /// <param name="hXCounts">Specifies a handle to the input counts in gpu memory.</param>
        /// <param name="hYCounts">Specifies a handle to the output counts in gpu memory.</param>
        /// <param name="hMapping">Specifies a handle to the mappings of each axis.</param>
        /// <param name="nNumAxes">Specifies the number of axes.</param>
        /// <param name="hBuffer">Specifies a handle to the buffer that should have 'n' * nNumAxes number of items.</param>
        public void transpose(int n, long hX, long hY, long hXCounts, long hYCounts, long hMapping, int nNumAxes, long hBuffer)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TRANSPOSE, null, m_param.AsLong(n, hX, hY, hXCounts, hYCounts, hMapping, nNumAxes, hBuffer));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TRANSPOSE, null, m_param.AsLong(n, hX, hY, hXCounts, hYCounts, hMapping, nNumAxes, hBuffer));
        }

        /// <summary>
        /// Calculates the sum of squares of A.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A and W.</param>
        /// <param name="hW">Specifies a handle to workspace data in GPU memory.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="nAOff">Specifies an offset (in items, not bytes) into the memory of A.</param>
        /// <returns>The sum of squares of A is returned as type <code>double</code></returns>
        public double sumsq(int n, long hW, long hA, int nAOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SUMSQ, null, m_param.AsLong(n, hW, hA, nAOff));
                return rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SUMSQ, null, m_param.AsLong(n, hW, hA, nAOff));
                return rg[0];
            }
        }

        /// <summary>
        /// Calculates the sum of squares of differences between A and B
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vectors A, B and W.</param>
        /// <param name="hW">Specifies a handle to workspace data in GPU memory.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hB">Specifies a handle to the vector B in GPU memory.</param>
        /// <param name="nAOff">Specifies an offset (in items, not bytes) into the memory of A.</param>
        /// <param name="nBOff">Specifies an offset (in items, not bytes) into the memory of B.</param>
        /// <returns>The sum of squared differences between A and B are returned as type <code>double</code></returns>
        public double sumsqdiff(int n, long hW, long hA, long hB, int nAOff = 0, int nBOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SUMSQDIFF, null, m_param.AsLong(n, hW, hA, hB, nAOff, nBOff));
                return rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SUMSQDIFF, null, m_param.AsLong(n, hW, hA, hB, nAOff, nBOff));
                return rg[0];
            }
        }

        /// <summary>
        /// Calculates the width values.
        /// </summary>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="hMean">Specifies a handle to the mean values in GPU memory.</param>
        /// <param name="hMin">Specifies a handle to the min values in GPU memory.</param>
        /// <param name="hMax">Specifies a handle to the max values in GPU memory.</param>
        /// <param name="dfAlpha">Specifies the alpha value.</param>
        /// <param name="hWidth">Specifies the GPU memory where the width values are placed.</param>
        public void width(int n, long hMean, long hMin, long hMax, double dfAlpha, long hWidth)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_WIDTH, m_param.AsDouble(dfAlpha), m_param.AsLong(n, hMean, hMin, hMax, 0, hWidth));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_WIDTH, m_param.AsFloat((float)dfAlpha), m_param.AsLong(n, hMean, hMin, hMax, 0, hWidth));
        }

        /// <summary>
        /// Returns true if the point is contained within the bounds.
        /// </summary>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="hMean">Specifies a handle to the mean values in GPU memory.</param>
        /// <param name="hWidth">Specifies a handle to the width values in GPU memory.</param>
        /// <param name="hX">Specifies a handle to the X values in GPU memory.</param>
        /// <param name="hWork">Specifies a handle to the work data in GPU memory.</param>
        /// <param name="nXOff">Optionally, specifies an offset into the X vector (default = 0).</param>
        /// <returns>If the X values are within the bounds, <i>true</i> is returned, otherwise <i>false</i>.</returns>
        public bool contains_point(int n, long hMean, long hWidth, long hX, long hWork, int nXOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CONTAINS_POINT, null, m_param.AsLong(n, hMean, hWidth, hX, hWork, nXOff));
                return (rg[0] == 0) ? false : true;
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CONTAINS_POINT, null, m_param.AsLong(n, hMean, hWidth, hX, hWork, nXOff));
                return (rg[0] == 0) ? false : true;
            }
        }

        /// <summary>
        /// Replaces all NAN values witin X with a replacement value.
        /// </summary>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="dfReplacement">Specifies the replacement value.</param>
        public void denan(int n, long hX, double dfReplacement)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_DENAN, m_param.AsDouble(dfReplacement), m_param.AsLong(n, hX, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_DENAN, m_param.AsFloat((float)dfReplacement), m_param.AsLong(n, hX, 0));
        }

        /// <summary>
        /// Rearranges image blocks into columns.
        /// </summary>
        /// <param name="hDataIm">Specifies a handle to the image block in GPU memory.</param>
        /// <param name="nDataImOffset">Specifies an offset into the image block memory.</param>
        /// <param name="nChannels">Specifies the number of channels in the image.</param>
        /// <param name="nHeight">Specifies the height of the image.</param>
        /// <param name="nWidth">Specifies the width of the image.</param>
        /// <param name="nKernelH">Specifies the kernel height.</param>
        /// <param name="nKernelW">Specifies the kernel width.</param>
        /// <param name="nPadH">Specifies the pad applied to the height.</param>
        /// <param name="nPadW">Specifies the pad applied to the width.</param>
        /// <param name="nStrideH">Specifies the stride along the height.</param>
        /// <param name="nStrideW">Specifies the stride along the width.</param>
        /// <param name="nDilationH">Specifies the dilation along the height.</param>
        /// <param name="nDilationW">Specifies the dilation along the width.</param>
        /// <param name="hDataCol">Specifies a handle to the column data in GPU memory.</param>
        /// <param name="nDataColOffset">Specifies an offset into the column memory.</param>
        public void im2col(long hDataIm, int nDataImOffset, int nChannels, int nHeight, int nWidth, int nKernelH, int nKernelW, int nPadH, int nPadW, int nStrideH, int nStrideW, int nDilationH, int nDilationW, long hDataCol, int nDataColOffset)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_IM2COL, null, m_param.AsLong(hDataIm, nDataImOffset, nChannels, nHeight, nWidth, nKernelH, nKernelW, nPadH, nPadW, nStrideH, nStrideW, nDilationH, nDilationW, hDataCol, nDataColOffset));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_IM2COL, null, m_param.AsLong(hDataIm, nDataImOffset, nChannels, nHeight, nWidth, nKernelH, nKernelW, nPadH, nPadW, nStrideH, nStrideW, nDilationH, nDilationW, hDataCol, nDataColOffset));
        }

        /// <summary>
        /// Rearranges image blocks into columns.
        /// </summary>
        /// <param name="hDataIm">Specifies a handle to the image block in GPU memory.</param>
        /// <param name="nDataImOffset">Specifies an offset into the image block memory.</param>
        /// <param name="nNumSpatialAxes">Specifies the number of spatial axes.</param>
        /// <param name="nImCount">Specifies the number of kernels.</param>
        /// <param name="nChannelAxis">Specifies the axis containing the channel.</param>
        /// <param name="hImShape">Specifies a handle to the image shape data in GPU memory.</param>
        /// <param name="hColShape">Specifies a handle to the column shape data in GPU memory.</param>
        /// <param name="hKernelShape">Specifies a handle to the kernel shape data in GPU memory.</param>
        /// <param name="hPad">Specifies a handle to the pad data in GPU memory.</param>
        /// <param name="hStride">Specifies a handle to the stride data in GPU memory.</param>
        /// <param name="hDilation">Specifies a handle to the dilation data in GPU memory.</param>
        /// <param name="hDataCol">Specifies a handle to the column data in GPU memory.</param>
        /// <param name="nDataColOffset">Specifies an offset into the column memory.</param>
        public void im2col_nd(long hDataIm, int nDataImOffset, int nNumSpatialAxes, int nImCount, int nChannelAxis, long hImShape, long hColShape, long hKernelShape, long hPad, long hStride, long hDilation, long hDataCol, int nDataColOffset)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_IM2COL_ND, null, m_param.AsLong(hDataIm, nDataImOffset, nNumSpatialAxes, nImCount, nChannelAxis, hImShape, hColShape, hKernelShape, hPad, hStride, hDilation, hDataCol, nDataColOffset));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_IM2COL_ND, null, m_param.AsLong(hDataIm, nDataImOffset, nNumSpatialAxes, nImCount, nChannelAxis, hImShape, hColShape, hKernelShape, hPad, hStride, hDilation, hDataCol, nDataColOffset));
        }

        /// <summary>
        /// Rearranges the columns into image blocks.
        /// </summary>
        /// <param name="hDataCol">Specifies a handle to the column data in GPU memory.</param>
        /// <param name="nDataColOffset">Specifies an offset into the column memory.</param>
        /// <param name="nChannels">Specifies the number of channels in the image.</param>
        /// <param name="nHeight">Specifies the height of the image.</param>
        /// <param name="nWidth">Specifies the width of the image.</param>
        /// <param name="nKernelH">Specifies the kernel height.</param>
        /// <param name="nKernelW">Specifies the kernel width.</param>
        /// <param name="nPadH">Specifies the pad applied to the height.</param>
        /// <param name="nPadW">Specifies the pad applied to the width.</param>
        /// <param name="nStrideH">Specifies the stride along the height.</param>
        /// <param name="nStrideW">Specifies the stride along the width.</param>
        /// <param name="nDilationH">Specifies the dilation along the height.</param>
        /// <param name="nDilationW">Specifies the dilation along the width.</param>
        /// <param name="hDataIm">Specifies a handle to the image block in GPU memory.</param>
        /// <param name="nDataImOffset">Specifies an offset into the image block memory.</param>
        public void col2im(long hDataCol, int nDataColOffset, int nChannels, int nHeight, int nWidth, int nKernelH, int nKernelW, int nPadH, int nPadW, int nStrideH, int nStrideW, int nDilationH, int nDilationW, long hDataIm, int nDataImOffset)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COL2IM, null, m_param.AsLong(hDataCol, nDataColOffset, nChannels, nHeight, nWidth, nKernelH, nKernelW, nPadH, nPadW, nStrideH, nStrideW, nDilationH, nDilationW, hDataIm, nDataImOffset));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COL2IM, null, m_param.AsLong(hDataCol, nDataColOffset, nChannels, nHeight, nWidth, nKernelH, nKernelW, nPadH, nPadW, nStrideH, nStrideW, nDilationH, nDilationW, hDataIm, nDataImOffset));
        }

        /// <summary>
        /// Rearranges the columns into image blocks.
        /// </summary>
        /// <param name="hDataCol">Specifies a handle to the column data in GPU memory.</param>
        /// <param name="nDataColOffset">Specifies an offset into the column memory.</param>
        /// <param name="nNumSpatialAxes">Specifies the number of spatial axes.</param>
        /// <param name="nColCount">Specifies the number of kernels.</param>
        /// <param name="nChannelAxis">Specifies the axis containing the channel.</param>
        /// <param name="hImShape">Specifies a handle to the image shape data in GPU memory.</param>
        /// <param name="hColShape">Specifies a handle to the column shape data in GPU memory.</param>
        /// <param name="hKernelShape">Specifies a handle to the kernel shape data in GPU memory.</param>
        /// <param name="hPad">Specifies a handle to the pad data in GPU memory.</param>
        /// <param name="hStride">Specifies a handle to the stride data in GPU memory.</param>
        /// <param name="hDilation">Specifies a handle to the dilation data in GPU memory.</param>
        /// <param name="hDataIm">Specifies a handle to the image block in GPU memory.</param>
        /// <param name="nDataImOffset">Specifies an offset into the image block memory.</param>
        public void col2im_nd(long hDataCol, int nDataColOffset, int nNumSpatialAxes, int nColCount, int nChannelAxis, long hImShape, long hColShape, long hKernelShape, long hPad, long hStride, long hDilation, long hDataIm, int nDataImOffset)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COL2IM_ND, null, m_param.AsLong(hDataCol, nDataColOffset, nNumSpatialAxes, nColCount, nChannelAxis, hImShape, hColShape, hKernelShape, hPad, hStride, hDilation, hDataIm, nDataImOffset));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COL2IM_ND, null, m_param.AsLong(hDataCol, nDataColOffset, nNumSpatialAxes, nColCount, nChannelAxis, hImShape, hColShape, hKernelShape, hPad, hStride, hDilation, hDataIm, nDataImOffset));
        }

        /// <summary>
        /// Calculates the minimum value within each channel or across the channels of X and places the result in Y.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the number of images within X.</param>
        /// <param name="nChannels">Specifies the number of channels per image of X.</param>
        /// <param name="nInnerNum">Specifies the dimension of each image in X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="bReturnIdx">Optionally, specifies to return the index of the minimum value, otherwise the minimum value is returned.</param>
        /// <param name="bAcrossChannels">Optionally, specifies to find the min across channels as opposed to within each channel (default = true).</param>
        public void channel_min(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, bool bReturnIdx = false, bool bAcrossChannels = true)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_MIN, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, (bReturnIdx) ? 1 : 0, (bAcrossChannels) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_MIN, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, (bReturnIdx) ? 1 : 0, (bAcrossChannels) ? 1 : 0));
        }

        /// <summary>
        /// Calculates the maximum value either within each channel or across the channels of X and places the result in Y.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the number of images within X.</param>
        /// <param name="nChannels">Specifies the number of channels per image of X.</param>
        /// <param name="nInnerNum">Specifies the dimension of each image in X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="bReturnIdx">Optionally, specifies to return the index of the maximum value, otherwise the maximum value is returned.</param>
        /// <param name="bAcrossChannels">Optionally, specifies to find the max across channels as opposed to within each channel (default = true).</param>
        public void channel_max(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, bool bReturnIdx = false, bool bAcrossChannels = true)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_MAX, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, (bReturnIdx) ? 1 : 0, (bAcrossChannels) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_MAX, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, (bReturnIdx) ? 1 : 0, (bAcrossChannels) ? 1 : 0));
        }

        /// <summary>
        /// Calculates the mean value of each channel of X and places the result in Y.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the number of images within X.</param>
        /// <param name="nChannels">Specifies the number of channels per item of X.</param>
        /// <param name="nInnerNum">Specifies the dimension of each item in X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory. This vector should be of shape NxCxHxW.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory. This vector should be of shape HxCx1x1</param>
        /// <param name="nXOff">Optionally, specifies an offset into the channel within X (default = 0).</param>
        public void channel_mean(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, int nXOff = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_MEAN, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, nXOff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_MEAN, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, nXOff));
        }

        /// <summary>
        /// Calculates the stdev value of each channel of X and places the result in Y, and places the mean in Z.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the N number of items within X.</param>
        /// <param name="nChannels">Specifies the C number of channels per item of X.</param>
        /// <param name="nInnerNum">Specifies the H*W dimension of each item in X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory. This vector should be of shape NxCxHxW.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory. This vector should be of shape NxCx1x1.</param>
        /// <param name="hZ">Specifies a handle to the vector Y in GPU memory. This vector should be of shape NxCx1x1</param>
        /// <param name="fEps">Specifies the small value added before the sqrt to avoid Nan.</param>
        /// <param name="bUnbiased">Specifies to calculated the unbiased standard deviation.</param>
        public void channel_stdev(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, long hZ, float fEps, bool bUnbiased)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_STDEV, m_param.AsDouble(fEps), m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, hZ, (bUnbiased) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_STDEV, m_param.AsFloat(fEps), m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, hZ, (bUnbiased) ? 1 : 0));
        }

        /// <summary>
        /// Compares the values of the channels from X and places the result in Y where 1 is set if the values are equal otherwise 0 is set.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the number of images within X.</param>
        /// <param name="nChannels">Specifies the number of channels per image of X.</param>
        /// <param name="nInnerNum">Specifies the dimension of each image in X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory of length nOuterNum.</param>
        public void channel_compare(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_COMPARE, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_COMPARE, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY));
        }

        /// <summary>
        /// Fills each channel with the the values stored in Src data where the X data continains nOuterNum x nChannels of data,
        /// (e.g. one item per channel) that is then copied to all nInnerNum elements of each channel in Y
        /// </summary>
        /// <param name="nCount">Specifies the number of items in Y.</param>
        /// <param name="nOuterNum">Specifies the num of Y and Labels.</param>
        /// <param name="nChannels">Specifies the channel size of Y and X.</param>
        /// <param name="nInnerNum">Specifies the spatial dimension of X and Y, but is normally 1.</param>
        /// <param name="hX">Specifies the GPU memory containing the src data of shape (nOuterNum, nChannels, 1).</param>
        /// <param name="hY">Specifies the GPU memory of the output data where the X src data is copied where each item per channel is filled across all nInnerNum elements of Y.  Y should have shape (nOuterNum, nChannels, nInnerNum).</param>
        /// <param name="dir">Specifies the direction of data flow.  When FWD X->Y, when BWD Y->X</param>
        public void channel_fillfrom(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, DIR dir)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_FILLFROM, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, (int)dir));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_FILLFROM, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, (int)dir));
        }

        /// <summary>
        /// Fills each channel with the channel item of Y with the data of X matching the label index specified by hLabels.
        /// </summary>
        /// <param name="nCount">Specifies the number of items in Y.</param>
        /// <param name="nOuterNum">Specifies the num of Y and Labels.</param>
        /// <param name="nChannels">Specifies the channel size of Y and X.</param>
        /// <param name="nInnerNum">Specifies the spatial dimension of X and Y, but is normally 1.</param>
        /// <param name="hX">Specifies the GPU memory containing the encodings (usually centroids) of each label 0, ... max label.</param>
        /// <param name="nLabelDim">Specifies the dimension of the label channels.  A value > 1 indicates that more than one label are stored per channel in which case only the first label is used.</param>
        /// <param name="hLabels">Specifies the label ordering that determines how Y is filled using data from X.</param>
        /// <param name="hY">Specifies the GPU memory of the output data.</param>
        /// <remarks>
        /// This function is used to fill a blob with data matching a set of labels.  For example in a 3 item encoding based system with
        /// 4 labels:
        /// X = 4 channels of 3 items each (e.g. an encoding for each label).
        /// The values of hLabels show the ordering for which to fill hY with the labeled encodings.  So if hLabels = 0, 2, 1, 3, 1, then
        /// Y = size { 5, 3, 1, 1 }, 5 items each with encoding sizes of 3 items which are then filled with the encoding at position 0,
        /// (for label 0), followed by the encoding for label 2, then 1, 3 and ending with the encoding for 1 as specified by the labels.
        /// </remarks>
        public void channel_fill(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, int nLabelDim, long hLabels, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_FILL, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, nLabelDim, hLabels, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_FILL, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, nLabelDim, hLabels, hY));
        }

        /// <summary>
        /// Subtracts the values across the channels of X from A and places the result in Y.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the number of images within X.</param>
        /// <param name="nChannels">Specifies the number of channels per image of X.</param>
        /// <param name="nInnerNum">Specifies the dimension of each image in X.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void channel_sub(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hA, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_SUB, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, hA));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_SUB, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, hA));
        }

        /// <summary>
        /// Subtracts the values across the channels from X and places the result in Y.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the number of images within X.</param>
        /// <param name="nChannels">Specifies the number of channels per image of X.</param>
        /// <param name="nInnerNum">Specifies the dimension of each image in X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void channel_sub(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_SUB, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_SUB, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY));
        }

        /// <summary>
        /// Calculates the sum of all channels in X and places the result in Y which should be nInnerNum in length.
        /// </summary>
        /// <param name="nInnerNum">Specifies the dimension of each image in X.</param>
        /// <param name="nOuterNum">Specifies the number of images within X.</param>
        /// <param name="nChannels">Specifies the number of channels per image of X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory (with expected size nOuterNum, nChannels, nInnerNum).</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory (with expected size nInnerNum, 1, 1).</param>
        /// <param name="dfScale">Optionally, specifies the scale to apply to the result.</param>
        public void channel_sum_all(int nInnerNum, int nOuterNum, int nChannels, long hX, long hY, double dfScale = 1.0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_SUM_ALL, m_param.AsDouble(dfScale), m_param.AsLong(nInnerNum, nOuterNum, nChannels, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_SUM_ALL, m_param.AsFloat((float)dfScale), m_param.AsLong(nInnerNum, nOuterNum, nChannels, hX, hY));
        }

        /// <summary>
        /// Calculates the original sum the the values either across each channel of X and places the result in Y.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the number of images within X.</param>
        /// <param name="nChannels">Specifies the number of channels per image of X.</param>
        /// <param name="nInnerNum">Specifies the dimension of each image in X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory (with expected size nOuterNum, nChannels, nInnerNum).</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory (with expected size nOuterNum, nChannels, 1).</param>
        public void channel_sum(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_SUM, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_SUM, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY));
        }

        /// <summary>
        /// Calculates the sum the the values either across or within each channel (depending on bSumAcrossChannels setting) of X and places the result in Y.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the number of images within X.</param>
        /// <param name="nChannels">Specifies the number of channels per image of X.</param>
        /// <param name="nInnerNum">Specifies the dimension of each image in X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory (with expected size nOuterNum, nChannels, nInnerNum).</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory (with expected size nOuterNum, nChannels, 1).</param>
        /// <param name="bSumAcrossChannels">Specifies to sum across channels (true), or within each channel (false).</param>
        /// <param name="dir">Optionally, specifies the direction.  When DIR.BWD is used, data flows from Y to X where Y data 
        /// is copied to X and summed across the channels of Y.  When using bSumAcrossChannels = true, ordering is based on Y ordering Y(c1,c2,c3,c1,c2,c3,c1,c2,c3),
        /// and when using bSumAcrossChannels = false, ordering is based on X ordering Y(c1,c1,c1,c2,c2,c2,c3,c3,c3).
        /// </param>
        /// <param name="nChannelsY">Optionally, specifies the channels of Y (used in special case where Y channels = 1)</param>
        public void channel_sumEx(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, bool bSumAcrossChannels, DIR dir, int nChannelsY = -1)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_SUM2, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, (bSumAcrossChannels) ? 1 : 0, (int)dir, nChannelsY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_SUM2, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, (bSumAcrossChannels) ? 1 : 0, (int)dir, nChannelsY));
        }

        /// <summary>
        /// Divides the values of the channels from X and places the result in Y.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the number of images within X.</param>
        /// <param name="nChannels">Specifies the number of channels per image of X.</param>
        /// <param name="nInnerNum">Specifies the dimension of each image in X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nMethod">Specifies the method of traversing the channel, <i>nMethod</i> = 1 (the default) is used by the SoftmaxLayer and <i>nMethod</i> = 2 is used by the GRNLayer.</param>
        public void channel_div(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, int nMethod = 1)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_DIV, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, nMethod));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_DIV, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, nMethod));
        }

        /// <summary>
        /// Multiplies the values of the channels from X and places the result in Y.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the number of images within X.</param>
        /// <param name="nChannels">Specifies the number of channels per image of X.</param>
        /// <param name="nInnerNum">Specifies the dimension of each image in X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="nMethod">Specifies the method of traversing the channel, <i>nMethod</i> = 1 (the default) is used by the SoftmaxLayer and <i>nMethod</i> = 2 is used by the GRNLayer.</param>
        public void channel_mul(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, int nMethod = 1)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_MUL, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, nMethod));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_MUL, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY, nMethod));
        }

        /// <summary>
        /// Multiplies the values in vector X by each channel in matrix A and places the result in matrix C.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in A.</param>
        /// <param name="nOuterNum">Specifies the number of items within A.</param>
        /// <param name="nChannels">Specifies the number of channels per item of A.</param>
        /// <param name="nInnerNum">Specifies the dimension of each item in A and X.</param>
        /// <param name="hA">Specifies a handle to the matrix A in GPU memory.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory (must be of length nInnerDim).</param>
        /// <param name="hC">Specifies a handle to the matrix C in GPU memory where the results are placed (matrix A and C are the same shape).</param>
        public void channel_mulv(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hA, long hX, long hC)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_MULV, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hA, hX, hC));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_MULV, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hA, hX, hC));
        }

        /// <summary>
        /// Multiplies the values of the channels from X with the scalar values in B and places the result in Y.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the number of items within X and B.</param>
        /// <param name="nChannels">Specifies the number of channels per item of X and B.</param>
        /// <param name="nInnerNum">Specifies the dimension of each data item in X (B should have data dimension = 1).</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hA">Specifies a handle to the vector B containing the scalar values, one per num * channel.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void channel_scale(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hA, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_SCALE, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hA, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_SCALE, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hA, hY));
        }

        /// <summary>
        /// Calculates the dot product the the values within each channel of X and places the result in Y.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements.</param>
        /// <param name="nOuterNum">Specifies the number of images.</param>
        /// <param name="nChannels">Specifies the number of channels per image.</param>
        /// <param name="nInnerNum">Specifies the dimension of each image.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hA">Specifies a handle to the vector A in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void channel_dot(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hA, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_DOT, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hA, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_DOT, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hA, hY));
        }

        /// <summary>
        /// Duplicates each channel 'nInnerNum' of times in the destination.
        /// </summary>
        /// <param name="nCount">Specifies the total number of elements in Y which = count(X)*nInnerDim in length.</param>
        /// <param name="nOuterNum">Specifies the number of items.</param>
        /// <param name="nChannels">Specifies the number of channels.</param>
        /// <param name="nInnerNum">Specifies the dimension of each inner dim within the channel.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void channel_duplicate(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_DUP, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_DUP, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY));
        }

        /// <summary>
        /// Calculates the percentile along axis = 0.
        /// </summary>
        /// <param name="nCount">Specifies the total number of elements in Y which = count(X)*nInnerDim in length.</param>
        /// <param name="nOuterNum">Specifies the number of items.</param>
        /// <param name="nChannels">Specifies the number of channels.</param>
        /// <param name="nInnerNum">Specifies the dimension of each inner dim within the channel.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="dfPercentile">Specifies the percentile to calculate.</param>
        public void channel_percentile(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY, double dfPercentile)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_PERCENTILE, m_param.AsDouble(dfPercentile), m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_PERCENTILE, m_param.AsFloat((float)dfPercentile), m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY));
        }

        /// <summary>
        /// Performs a channel operation forward on the data.
        /// </summary>
        /// <param name="op">Specifies the operation to perform.</param>
        /// <param name="nCount">Specifies the number of items in Y which should equal max(nN1, nN2) x nC x max(nSD1, nSD2).</param>
        /// <param name="nC">Specifies the channels in both A, B and Y.</param>
        /// <param name="nN1">Specifies the number of items in A.</param>
        /// <param name="nSD1">Specifies the spatial dimension of each item of A.</param>
        /// <param name="nN2">Specifies the number of items in B.</param>
        /// <param name="nSD2">Specifies the spatial dimension of each item of B.</param>
        /// <param name="hA">Specifies a handle to the memory of A which has the size nN1 x nC1 x nSD1.</param>
        /// <param name="hB">Specifies a handle to the memory of B which has the size nN2 x nC2 x nSD2.</param>
        /// <param name="hY">Specifies a handle to the memory where the result is placed during FWD with size max(nN1, nN2) x nC x max(nSD1, nSD2).</param>
        public void channel_op_fwd(OP op, int nCount, int nC, int nN1, int nSD1, int nN2, int nSD2, long hA, long hB, long hY)
        {
            int nCount1 = Math.Max(nN1, nN2) * nC * Math.Max(nSD1, nSD2);
            if (nCount1 != nCount)
                throw new Exception("The nCount must equal max(nN1, nN2) x nC x max(nSD1, nSD2).");

            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int) m_hKernel, (int) CUDAFN.CUDA_CHANNEL_OP_FWD, null, m_param.AsLong((int)op, nCount, nC, nN1, nSD1, nN2, nSD2, hA, hB, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_OP_FWD, null, m_param.AsLong((int)op, nCount, nC, nN1, nSD1, nN2, nSD2, hA, hB, hY));
        }

        /// <summary>
        /// Performs a channel operation backward on the data.
        /// </summary>
        /// <param name="op">Specifies the operation to perform.</param>
        /// <param name="nCount">Specifies the number of items in Y which should equal max(nN1, nN2) x nC x max(nSD1, nSD2).</param>
        /// <param name="nC">Specifies the channels in both A, B and Y.</param>
        /// <param name="nN1">Specifies the number of items in A.</param>
        /// <param name="nSD1">Specifies the spatial dimension of each item of A.</param>
        /// <param name="nN2">Specifies the number of items in B.</param>
        /// <param name="nSD2">Specifies the spatial dimension of each item of B.</param>
        /// <param name="nCy">Specifies the channels of each item of Y.</param>
        /// <param name="nSDy">Specifies the spatial dimension of each item of Y.</param>
        /// <param name="hA">Specifies a handle to the memory of A which has the size nN1 x nC1 x nSD1.</param>
        /// <param name="hB">Specifies a handle to the memory of B which has the size nN2 x nC2 x nSD2.</param>
        /// <param name="hY">Specifies a handle to the memory where the result is placed during FWD with size max(nN1, nN2) x nC x max(nSD1, nSD2).</param>
        /// <param name="hAd">Optionally, specifies a handle to the memory of the diff for A (filled during BWD) with size nN1, nC, nSD1.</param>
        /// <param name="hBd">Optionally, specifies a handle to the memory of the diff for b (filled during BWD) with size nN2, nC, nSD2.</param>
        /// <param name="hYd">Optionally, specifies a handle to the memory of the diff for Y (used during BWD).</param>
        /// <param name="hWork">Optionally, specifies a handle to work memory with the same size as Y (used during BWD)</param>
        public void channel_op_bwd(OP op, int nCount, int nC, int nN1, int nSD1, int nN2, int nSD2, int nCy,int nSDy, long hA, long hB, long hY, long hAd, long hBd, long hYd, long hWork)
        {
            int nCount1 = Math.Max(nN1, nN2) * nC * Math.Max(nSD1, nSD2);
            if (nCount1 != nCount)
                throw new Exception("The nCount must equal max(nN1, nN2) x nC x max(nSD1, nSD2).");

            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_OP_BWD, null, m_param.AsLong((int)op, nCount, nC, nN1, nSD1, nN2, nSD2, nCy, nSDy, hA, hB, hY, hAd, hBd, hYd, hWork));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_OP_BWD, null, m_param.AsLong((int)op, nCount, nC, nN1, nSD1, nN2, nSD2, nCy, nSDy, hA, hB, hY, hAd, hBd, hYd, hWork));
        }

        /// <summary>
        /// Add data along channels similar to numpy split function but where the data is added instead of copied.
        /// </summary>
        /// <param name="nCount">Specifies the total number of elements in Y which = count(X)/nBlocks in length.</param>
        /// <param name="nOuterNum">Specifies the number of items.</param>
        /// <param name="nChannels">Specifies the number of channels.</param>
        /// <param name="nBlocks">Specifies the number of blocks in each channel.</param>
        /// <param name="nInnerNum">Specifies the dimension of each inner dim within the channel.</param>
        /// <param name="nOffset">Specifies the offset of the inner dim.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="dir">Specifies the direction of data flow (0 = fwd X->Y, 1 = bwd Y->X).</param>
        public void channel_add(int nCount, int nOuterNum, int nChannels, int nBlocks, int nInnerNum, int nOffset, long hX, long hY, DIR dir)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_ADD, null, m_param.AsLong(nCount, nOuterNum, nChannels, nBlocks, nInnerNum, nOffset, hX, hY, (int)dir));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_ADD, null, m_param.AsLong(nCount, nOuterNum, nChannels, nBlocks, nInnerNum, nOffset, hX, hY, (int)dir));
        }

        /// <summary>
        /// Copy data along channels similar to numpy split function.
        /// </summary>
        /// <param name="nCount">Specifies the total number of elements in Y which = count(X)/nBlocks in length.</param>
        /// <param name="nOuterNum">Specifies the number of items.</param>
        /// <param name="nChannels">Specifies the number of channels.</param>
        /// <param name="nBlocks">Specifies the number of blocks in each channel.</param>
        /// <param name="nInnerNum">Specifies the dimension of each inner dim within the channel.</param>
        /// <param name="nOffset">Specifies the offset of the inner dim.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        /// <param name="dir">Specifies the direction of data flow (0 = fwd X->Y, 1 = bwd Y->X).</param>
        public void channel_copy(int nCount, int nOuterNum, int nChannels, int nBlocks, int nInnerNum, int nOffset, long hX, long hY, DIR dir)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_COPY, null, m_param.AsLong(nCount, nOuterNum, nChannels, nBlocks, nInnerNum, nOffset, hX, hY, (int)dir));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_COPY, null, m_param.AsLong(nCount, nOuterNum, nChannels, nBlocks, nInnerNum, nOffset, hX, hY, (int)dir));
        }

        /// <summary>
        /// Copy all data from X (shape 1,c,sd) to each num in Y (shape n,c,sd).
        /// </summary>
        /// <param name="nCount">Specifies the full count of Y.</param>
        /// <param name="nOuterNum">Specifies the outer num of Y.</param>
        /// <param name="nChannels">Specifies the channels in X and Y.</param>
        /// <param name="nInnerNum">Specifies the spatial dimension of X and Y.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void channel_copyall(int nCount, int nOuterNum, int nChannels, int nInnerNum, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_COPYALL, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_COPYALL, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNum, hX, hY));
        }

        /// <summary>
        /// Perform linear intperpolation between the values of X and places the result in Y.  The nInnerNumDst must be greater than nInnerNumSrc, and the nInnerNumSrc must be a multiple of nInnerNumDst.
        /// </summary>
        /// <param name="nCount">Specifies the number of total number of items in the Y tensor.</param>
        /// <param name="nOuterNum">Specifies the number of outer items in both the X and Y tensors.</param>
        /// <param name="nChannels">Specifies the number of channels in both the X and Y tensors.</param>
        /// <param name="nInnerNumSrc">Specifies the inner number of items for each channel in the X tensor.</param>
        /// <param name="nInnerNumDst">Specifies the inner number of items for each channel in the Y tensor.</param>
        /// <param name="hX">Specifies a handle to the GPU memory containing the X tensor data.</param>
        /// <param name="hY">Specifies a handle to the GPU memory containing the Y tensor data.</param>
        /// <param name="dir">Specifies the direction where FWD interpolates from X to Y and BWD interpolates in reverse from Y to X.</param>
        public void channel_interpolate_linear(int nCount, int nOuterNum, int nChannels, int nInnerNumSrc, int nInnerNumDst, long hX, long hY, DIR dir = DIR.FWD)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_INTERPOLATE_LINEAR, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNumSrc, nInnerNumDst, hX, hY, (int)dir));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CHANNEL_INTERPOLATE_LINEAR, null, m_param.AsLong(nCount, nOuterNum, nChannels, nInnerNumSrc, nInnerNumDst, hX, hY, (int)dir));
        }


        /// <summary>
        /// Calculates the sum of inner values of X and places the result in Y.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in X.</param>
        /// <param name="nOuterNum">Specifies the number of outer items within X.</param>
        /// <param name="nInnerNum">Specifies the dimension of items to sum in X.</param>
        /// <param name="hX">Specifies a handle to the vector X in GPU memory.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void sum(int nCount, int nOuterNum, int nInnerNum, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SUM, null, m_param.AsLong(nCount, nOuterNum, nInnerNum, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SUM, null, m_param.AsLong(nCount, nOuterNum, nInnerNum, hX, hY));
        }

        /// <summary>
        /// Sets the random number generator seed used by random number operations.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuRand](https://developer.nvidia.com/curand)
        /// </remarks>
        /// <param name="lSeed">Specifies the random number generator seed.</param>
        public void rng_setseed(long lSeed)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CUDA_RNG_SETSEED, m_param.AsDouble(lSeed));
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CUDA_RNG_SETSEED, m_param.AsFloat(lSeed));
        }

        /// <summary>
        /// Fill Y with random numbers using a uniform random distribution.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuRand](https://developer.nvidia.com/curand).  See also [Uniform Distribution](https://en.wikipedia.org/wiki/Uniform_distribution_(continuous)).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="fMin">Specifies the minimum value of the distribution with a type of <code>double</code></param>
        /// <param name="fMax">Specifies the maximum value of the distribution with a type of <code>double</code></param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void rng_uniform(int n, double fMin, double fMax, long hY)
        {
            rng_uniform(n, (T)Convert.ChangeType(fMin, typeof(T)), (T)Convert.ChangeType(fMax, typeof(T)), hY);
        }

        /// <summary>
        /// Fill Y with random numbers using a uniform random distribution.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuRand](https://developer.nvidia.com/curand).  See also [Uniform Distribution](https://en.wikipedia.org/wiki/Uniform_distribution_(continuous)).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="fMin">Specifies the minimum value of the distribution with a type of <code>float</code></param>
        /// <param name="fMax">Specifies the maximum value of the distribution with a type of <code>float</code></param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void rng_uniform(int n, float fMin, float fMax, long hY)
        {
            rng_uniform(n, (T)Convert.ChangeType(fMin, typeof(T)), (T)Convert.ChangeType(fMax, typeof(T)), hY);
        }

        /// <summary>
        /// Fill Y with random numbers using a uniform random distribution.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuRand](https://developer.nvidia.com/curand).  See also [Uniform Distribution](https://en.wikipedia.org/wiki/Uniform_distribution_(continuous)).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="fMin">Specifies the minimum value of the distribution with a type of 'T'.</param>
        /// <param name="fMax">Specifies the maximum value of the distribution with a type of 'T'.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void rng_uniform(int n, T fMin, T fMax, long hY)
        {
            if (m_dt == DataType.DOUBLE)
            {
                if (m_rgGhostMemory == null || !m_bGhostMemoryEnabled)
                    m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_RNG_UNIFORM, m_param.AsDouble(convertD(fMin), convertD(fMax)), m_param.AsLong(n, 0, 0, hY));
            }
            else
            {
                if (m_rgGhostMemory == null || !m_bGhostMemoryEnabled)
                    m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_RNG_UNIFORM, m_param.AsFloat(convertF(fMin), convertF(fMax)), m_param.AsLong(n, 0, 0, hY));
            }
        }

        /// <summary>
        /// Fill Y with random numbers using a gaussian random distribution.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuRand](https://developer.nvidia.com/curand).  See also [Guassian Distribution](https://en.wikipedia.org/wiki/Normal_distribution).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="fMu">Specifies the mean of the distribution with a type of <code>double</code></param>
        /// <param name="fSigma">Specifies the standard deviation of the distribution with a type of <code>double</code></param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void rng_gaussian(int n, double fMu, double fSigma, long hY)
        {
            rng_gaussian(n, (T)Convert.ChangeType(fMu, typeof(T)), (T)Convert.ChangeType(fSigma, typeof(T)), hY);
        }

        /// <summary>
        /// Fill Y with random numbers using a gaussian random distribution.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuRand](https://developer.nvidia.com/curand).  See also [Guassian Distribution](https://en.wikipedia.org/wiki/Normal_distribution).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="fMu">Specifies the mean of the distribution with a type of <code>float</code></param>
        /// <param name="fSigma">Specifies the standard deviation of the distribution with a type of <code>float</code></param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void rng_gaussian(int n, float fMu, float fSigma, long hY)
        {
            rng_gaussian(n, (T)Convert.ChangeType(fMu, typeof(T)), (T)Convert.ChangeType(fSigma, typeof(T)), hY);
        }

        /// <summary>
        /// Fill Y with random numbers using a gaussian random distribution.
        /// </summary>
        /// <remarks>
        /// This function uses [NVIDIA's cuRand](https://developer.nvidia.com/curand).  See also [Guassian Distribution](https://en.wikipedia.org/wiki/Normal_distribution).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="fMu">Specifies the mean of the distribution with a type of 'T'.</param>
        /// <param name="fSigma">Specifies the standard deviation of the distribution with a type of 'T'.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void rng_gaussian(int n, T fMu, T fSigma, long hY)
        {
            if (m_dt == DataType.DOUBLE)
            {
                if (m_rgGhostMemory == null || !m_bGhostMemoryEnabled)
                    m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_RNG_GAUSSIAN, m_param.AsDouble(convertD(fMu), convertD(fSigma)), m_param.AsLong(n, 0, 0, hY));
            }
            else
            {
                if (m_rgGhostMemory == null || !m_bGhostMemoryEnabled)
                    m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_RNG_GAUSSIAN, m_param.AsFloat(convertF(fMu), convertF(fSigma)), m_param.AsLong(n, 0, 0, hY));
            }
        }

        /// <summary>
        /// Fill Y with random numbers using a bernoulli random distribution.
        /// </summary>
        /// <remarks>
        /// See [Bernoulli Distribution](https://en.wikipedia.org/wiki/Bernoulli_distribution).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="fNonZeroProb">Specifies the probability that a given value is set to non zero.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void rng_bernoulli(int n, double fNonZeroProb, long hY)
        {
            rng_bernoulli(n, (T)Convert.ChangeType(fNonZeroProb, typeof(T)), hY);
        }

        /// <summary>
        /// Fill Y with random numbers using a bernoulli random distribution.
        /// </summary>
        /// <remarks>
        /// See [Bernoulli Distribution](https://en.wikipedia.org/wiki/Bernoulli_distribution).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="fNonZeroProb">Specifies the probability that a given value is set to non zero.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void rng_bernoulli(int n, float fNonZeroProb, long hY)
        {
            rng_bernoulli(n, (T)Convert.ChangeType(fNonZeroProb, typeof(T)), hY);
        }

        /// <summary>
        /// Fill Y with random numbers using a bernoulli random distribution.
        /// </summary>
        /// <remarks>
        /// See [Bernoulli Distribution](https://en.wikipedia.org/wiki/Bernoulli_distribution).
        /// </remarks>
        /// <param name="n">Specifies the number of items (not bytes) in the vector X.</param>
        /// <param name="fNonZeroProb">Specifies the probability that a given value is set to non zero.</param>
        /// <param name="hY">Specifies a handle to the vector Y in GPU memory.</param>
        public void rng_bernoulli(int n, T fNonZeroProb, long hY)
        {
            //if (m_dt == DataType.DOUBLE)
            //    m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CUDA_RNG_BERNOULLI, new double[] { n, (double)Convert.ChangeType(fNonZeroProb, typeof(double)), hY });
            //else
            //    m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CUDA_RNG_BERNOULLI, new float[] { n, (float)Convert.ChangeType(fNonZeroProb, typeof(float)), hY });

            T[] rg = GetMemory(hY);
            fill_random(fNonZeroProb, rg);
            SetMemory(hY, rg);
        }

#pragma warning disable 1591

        public void fill_random(T fNonZeroProb, T[] rg) /** @private */
        {
            double dfNonZeroProb = Utility.ConvertVal<T>(fNonZeroProb);

            for (int i = 0; i < rg.Length; i++)
            {
                double dfRand = m_random.NextDouble();
                rg[i] = (dfRand <= dfNonZeroProb) ? m_tOne : m_tZero;
            }
        }

#pragma warning restore 1591


        /// <summary>
        /// Performs the forward pass for the accuracy layer
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="nOuterNum">Specifies the outer count.</param>
        /// <param name="nInnerNum">Specifies the inner count.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hBottomLabel">Specifies a handle to the bottom labels in GPU memory.</param>
        /// <param name="hAccData">Specifies a handle to temporary accuracy correct items in GPU memory.</param>
        /// <param name="hAccTotals">Specifies a handle to the temporary accuracy totals in GPU memory.</param>
        /// <param name="nIgnoreLabel">Optionally, specifies a label to igore.</param>
        /// <param name="bLastElementOnly">Optionally specifies to only test the last element in each set.</param>
        /// <param name="nBatch">Optionally specifies the batch size.</param>
        public void accuracy_fwd(int nCount, int nOuterNum, int nInnerNum, long hBottomData, long hBottomLabel, long hAccData, long hAccTotals, int? nIgnoreLabel, bool bLastElementOnly, int nBatch)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<long> rgArg = new List<long>() { nCount, nOuterNum, nInnerNum, hBottomData, hBottomLabel, hAccData, hAccTotals, (bLastElementOnly) ? 1 : 0, nBatch };
                if (nIgnoreLabel.HasValue)
                    rgArg.Add(nIgnoreLabel.Value);
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ACCURACY_FWD, null, rgArg.ToArray());
            }
            else
            {
                List<long> rgArg = new List<long>() { nCount, nOuterNum, nInnerNum, hBottomData, hBottomLabel, hAccData, hAccTotals, (bLastElementOnly) ? 1 : 0, nBatch };
                if (nIgnoreLabel.HasValue)
                    rgArg.Add(nIgnoreLabel.Value);
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ACCURACY_FWD, null, rgArg.ToArray());
            }
        }
        

        /// <summary>
        /// Performs the forward pass for batch re-index
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="nInnerDim">Specifies the inner dimension.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hPermutData">Specifies a handle to the permuation data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void batchreidx_fwd(int nCount, int nInnerDim, long hBottomData, long hPermutData, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_BATCHREIDX_FWD, null, m_param.AsLong(nCount, nInnerDim, hBottomData, hPermutData, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_BATCHREIDX_FWD, null, m_param.AsLong(nCount, nInnerDim, hBottomData, hPermutData, hTopData));
        }

        /// <summary>
        /// Performs the backward pass for batch re-index
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="nInnerDim">Specifies the inner dimension.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopIdx">Specifies a handle to the top indexes in GPU memory.</param>
        /// <param name="hBegins">Specifies a handle to the begin data in GPU memory.</param>
        /// <param name="hCounts">Specifies a handle to the counts in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void batchreidx_bwd(int nCount, int nInnerDim, long hTopDiff, long hTopIdx, long hBegins, long hCounts, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_BATCHREIDX_BWD, null, m_param.AsLong(nCount, nInnerDim, hTopDiff, hTopIdx, hBegins, hCounts, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_BATCHREIDX_BWD, null, m_param.AsLong(nCount, nInnerDim, hTopDiff, hTopIdx, hBegins, hCounts, hBottomDiff));
        }

        /// <summary>
        /// Performs the forward pass for embed 
        /// </summary>
        /// <param name="nCount">Specifies the number of items in the bottom data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hWeight">Specifies a handle to the weight data in GPU memory.</param>
        /// <param name="nM">Specifies the input items.</param>
        /// <param name="nN">Specifies the embedding space.</param>
        /// <param name="nK">Specifies the number of indexes per slot.  For example, when using a vocabulary of 4 elements (0-3), K = 4.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void embed_fwd(int nCount, long hBottomData, long hWeight, int nM, int nN, int nK, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_EMBED_FWD, null, m_param.AsLong(nCount, hBottomData, hWeight, nM, nN, nK, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_EMBED_FWD, null, m_param.AsLong(nCount, hBottomData, hWeight, nM, nN, nK, hTopData));
        }

        /// <summary>
        /// Performs the backward pass for embed
        /// </summary>
        /// <param name="nCount">Specifies the number of items in the bottom data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nM">Specifies the input items.</param>
        /// <param name="nN">Specifies the embedding space.</param>
        /// <param name="nK">Specifies the number of indexes per slot.  For example, when using a vocabulary of 4 elements (0-3), K = 4.</param>
        /// <param name="hWeightDiff">Specifies a handle to the weight diff in GPU memory.</param>
        /// <param name="nMajorVer">Optionally, specifies the major compute version.  Compute >= 6.0 use system-wide atomic add which is more stable (default = 0, which supports all compute levels).</param>
        public void embed_bwd(int nCount, long hBottomData, long hTopDiff, int nM, int nN, int nK, long hWeightDiff, int nMajorVer = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_EMBED_BWD, null, m_param.AsLong(nCount, hBottomData, hTopDiff, nM, nN, nK, hWeightDiff, nMajorVer));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_EMBED_BWD, null, m_param.AsLong(nCount, hBottomData, hTopDiff, nM, nN, nK, hWeightDiff, nMajorVer));
        }

        /// <summary>
        /// Performs the forward pass for pooling using Cuda
        /// </summary>
        /// <param name="method">Specifies the pooling method.</param>
        /// <param name="nCount">Specifies the number of items in the bottom data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="num">Specifies the number of inputs.</param>
        /// <param name="nChannels">Specifies the number of channels per input.</param>
        /// <param name="nHeight">Specifies the height of each input.</param>
        /// <param name="nWidth">Specifies the width of each input.</param>
        /// <param name="nPooledHeight">Specifies the height of the pooled data.</param>
        /// <param name="nPooledWidth">Specifies the width of the pooled data.</param>
        /// <param name="nKernelH">Specifies the height of the pooling kernel.</param>
        /// <param name="nKernelW">Specifies the width of the pooling kernel.</param>
        /// <param name="nStrideH">Specifies the stride along the height.</param>
        /// <param name="nStrideW">Specifies the stride along the width.</param>
        /// <param name="nPadH">Specifies the pad applied to the height.</param>
        /// <param name="nPadW">Specifies the pad applied to the width.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hMask">Specifies a handle to the mask data in GPU memory.</param>
        /// <param name="hTopMask">Specifies a handle to the top mask data in GPU memory.</param>
        public void pooling_fwd(POOLING_METHOD method, int nCount, long hBottomData, int num, int nChannels, int nHeight, int nWidth, int nPooledHeight, int nPooledWidth, int nKernelH, int nKernelW, int nStrideH, int nStrideW, int nPadH, int nPadW, long hTopData, long hMask, long hTopMask)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_POOL_FWD, null, m_param.AsLong((int)method, nCount, hBottomData, num, nChannels, nHeight, nWidth, nPooledHeight, nPooledWidth, nKernelH, nKernelW, nStrideH, nStrideW, nPadH, nPadW, hTopData, hMask, hTopMask));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_POOL_FWD, null, m_param.AsLong((int)method, nCount, hBottomData, num, nChannels, nHeight, nWidth, nPooledHeight, nPooledWidth, nKernelH, nKernelW, nStrideH, nStrideW, nPadH, nPadW, hTopData, hMask, hTopMask));
        }

        /// <summary>
        /// Performs the backward pass for pooling using Cuda
        /// </summary>
        /// <param name="method">Specifies the pooling method.</param>
        /// <param name="nCount">Specifies the number of items in the bottom data.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="num">Specifies the number of inputs.</param>
        /// <param name="nChannels">Specifies the number of channels per input.</param>
        /// <param name="nHeight">Specifies the height of each input.</param>
        /// <param name="nWidth">Specifies the width of each input.</param>
        /// <param name="nPooledHeight">Specifies the height of the pooled data.</param>
        /// <param name="nPooledWidth">Specifies the width of the pooled data.</param>
        /// <param name="nKernelH">Specifies the height of the pooling kernel.</param>
        /// <param name="nKernelW">Specifies the width of the pooling kernel.</param>
        /// <param name="nStrideH">Specifies the stride along the height.</param>
        /// <param name="nStrideW">Specifies the stride along the width.</param>
        /// <param name="nPadH">Specifies the pad applied to the height.</param>
        /// <param name="nPadW">Specifies the pad applied to the width.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="hMask">Specifies a handle to the mask data in GPU memory.</param>
        /// <param name="hTopMask">Specifies a handle to the top mask data in GPU memory.</param>
        public void pooling_bwd(POOLING_METHOD method, int nCount, long hTopDiff, int num, int nChannels, int nHeight, int nWidth, int nPooledHeight, int nPooledWidth, int nKernelH, int nKernelW, int nStrideH, int nStrideW, int nPadH, int nPadW, long hBottomDiff, long hMask, long hTopMask)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_POOL_BWD, null, m_param.AsLong((int)method, nCount, hTopDiff, num, nChannels, nHeight, nWidth, nPooledHeight, nPooledWidth, nKernelH, nKernelW, nStrideH, nStrideW, nPadH, nPadW, hBottomDiff, hMask, hTopMask));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_POOL_BWD, null, m_param.AsLong((int)method, nCount, hTopDiff, num, nChannels, nHeight, nWidth, nPooledHeight, nPooledWidth, nKernelH, nKernelW, nStrideH, nStrideW, nPadH, nPadW, hBottomDiff, hMask, hTopMask));
        }

        /// <summary>
        /// Performs the forward pass for unpooling using Cuda
        /// </summary>
        /// <param name="method">Specifies the pooling method.</param>
        /// <param name="nCount">Specifies the number of items in the bottom data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="num">Specifies the number of inputs.</param>
        /// <param name="nChannels">Specifies the number of channels per input.</param>
        /// <param name="nHeight">Specifies the height of each input.</param>
        /// <param name="nWidth">Specifies the width of each input.</param>
        /// <param name="nPooledHeight">Specifies the height of the pooled data.</param>
        /// <param name="nPooledWidth">Specifies the width of the pooled data.</param>
        /// <param name="nKernelH">Specifies the height of the pooling kernel.</param>
        /// <param name="nKernelW">Specifies the width of the pooling kernel.</param>
        /// <param name="nStrideH">Specifies the stride along the height.</param>
        /// <param name="nStrideW">Specifies the stride along the width.</param>
        /// <param name="nPadH">Specifies the pad applied to the height.</param>
        /// <param name="nPadW">Specifies the pad applied to the width.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hMask">Specifies a handle to the mask data in GPU memory.</param>
        public void unpooling_fwd(POOLING_METHOD method, int nCount, long hBottomData, int num, int nChannels, int nHeight, int nWidth, int nPooledHeight, int nPooledWidth, int nKernelH, int nKernelW, int nStrideH, int nStrideW, int nPadH, int nPadW, long hTopData, long hMask)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_UNPOOL_FWD, null, m_param.AsLong((int)method, nCount, hBottomData, num, nChannels, nHeight, nWidth, nPooledHeight, nPooledWidth, nKernelH, nKernelW, nStrideH, nStrideW, nPadH, nPadW, hTopData, hMask));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_UNPOOL_FWD, null, m_param.AsLong((int)method, nCount, hBottomData, num, nChannels, nHeight, nWidth, nPooledHeight, nPooledWidth, nKernelH, nKernelW, nStrideH, nStrideW, nPadH, nPadW, hTopData, hMask));
        }

        /// <summary>
        /// Performs the backward pass for unpooling using Cuda
        /// </summary>
        /// <param name="method">Specifies the pooling method.</param>
        /// <param name="nCount">Specifies the number of items in the bottom data.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="num">Specifies the number of inputs.</param>
        /// <param name="nChannels">Specifies the number of channels per input.</param>
        /// <param name="nHeight">Specifies the height of each input.</param>
        /// <param name="nWidth">Specifies the width of each input.</param>
        /// <param name="nPooledHeight">Specifies the height of the pooled data.</param>
        /// <param name="nPooledWidth">Specifies the width of the pooled data.</param>
        /// <param name="nKernelH">Specifies the height of the pooling kernel.</param>
        /// <param name="nKernelW">Specifies the width of the pooling kernel.</param>
        /// <param name="nStrideH">Specifies the stride along the height.</param>
        /// <param name="nStrideW">Specifies the stride along the width.</param>
        /// <param name="nPadH">Specifies the pad applied to the height.</param>
        /// <param name="nPadW">Specifies the pad applied to the width.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="hMask">Specifies a handle to the mask data in GPU memory.</param>
        public void unpooling_bwd(POOLING_METHOD method, int nCount, long hTopDiff, int num, int nChannels, int nHeight, int nWidth, int nPooledHeight, int nPooledWidth, int nKernelH, int nKernelW, int nStrideH, int nStrideW, int nPadH, int nPadW, long hBottomDiff, long hMask)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_UNPOOL_BWD, null, m_param.AsLong((int)method, nCount, hTopDiff, num, nChannels, nHeight, nWidth, nPooledHeight, nPooledWidth, nKernelH, nKernelW, nStrideH, nStrideW, nPadH, nPadW, hBottomDiff, hMask));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_UNPOOL_BWD, null, m_param.AsLong((int)method, nCount, hTopDiff, num, nChannels, nHeight, nWidth, nPooledHeight, nPooledWidth, nKernelH, nKernelW, nStrideH, nStrideW, nPadH, nPadW, hBottomDiff, hMask));
        }

        /// <summary>
        /// Performs a Clip forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Calculation @f$ Y[i] = \max(min, \min(max,X[i])) @f$
        /// </remarks>
        /// <param name="nCount">Specifies the number of items in the bottom and top data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="fMin">Specifies the bottom value to clip to.</param>
        /// <param name="fMax">Specifies the top value to clip to.</param>
        public void clip_fwd(int nCount, long hBottomData, long hTopData, T fMin, T fMax)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CLIP_FWD, m_param.AsDouble(convertD1(fMin), convertD1(fMax)), m_param.AsLong(nCount, hBottomData, hTopData, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CLIP_FWD, m_param.AsFloat(convertF1(fMin), convertF1(fMax)), m_param.AsLong(nCount, hBottomData, hTopData, 0, 0));
        }

        /// <summary>
        /// Performs a Clip backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="fMin">Specifies the bottom value to clip to.</param>
        /// <param name="fMax">Specifies the top value to clip to.</param>
        public void clip_bwd(int nCount, long hTopDiff, long hBottomData, long hBottomDiff, T fMin, T fMax)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CLIP_BWD, m_param.AsDouble(convertD1(fMin), convertD1(fMax)), m_param.AsLong(nCount, hTopDiff, hBottomData, hBottomDiff, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CLIP_BWD, m_param.AsFloat(convertF1(fMin), convertF1(fMax)), m_param.AsLong(nCount, hTopDiff, hBottomData, hBottomDiff, 0, 0));
        }

        /// <summary>
        /// Performs a Math function forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Calculation @f$ Y[i] = function(X[i]) @f$
        /// </remarks>
        /// <param name="nCount">Specifies the number of items in the bottom and top data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="function">Specifies the mathematical function to use.</param>
        public void math_fwd(int nCount, long hBottomData, long hTopData, MATH_FUNCTION function)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MATH_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData, (int)function));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MATH_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData, (int)function));
        }

        /// <summary>
        /// Performs a Math function backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="hBottomData">Specifies a handle tot he bottom data in GPU memory.</param>
        /// <param name="function">Specifies the mathematical function to use.</param>
        public void math_bwd(int nCount, long hTopDiff, long hTopData, long hBottomDiff, long hBottomData, MATH_FUNCTION function)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MATH_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData, (int)function));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MATH_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData, (int)function));
        }

        /// <summary>
        /// Performs a Mean Error Loss backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// The gradient is set to:
        ///     +1 when predicted greater than target,
        ///     -1 when predicted less than target,
        ///      0 when predicted equal to target.
        /// if propagate_down[1] == true.
        /// 
        /// @see [Mean Absolute Error (MAE) derivative](https://stats.stackexchange.com/questions/312737/mean-absolute-error-mae-derivative)
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hPredicted">Specifies a handle to the predicted data in GPU memory.</param>
        /// <param name="hTarget">Specifies a handle to the target data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="merr">Specifies the type of mean error to run.</param>
        public void mean_error_loss_bwd(int nCount, long hPredicted, long hTarget, long hBottomDiff, MEAN_ERROR merr)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MEAN_ERROR_LOSS_BWD, null, m_param.AsLong(nCount, hPredicted, hTarget, hBottomDiff, (int)merr));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MEAN_ERROR_LOSS_BWD, null, m_param.AsLong(nCount, hPredicted, hTarget, hBottomDiff, (int)merr));
        }

        /// <summary>
        /// Calculates the BCEWithLogitsLoss forward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in hX, hY, hW (optional), hPosW (optional) and hLossData.</param>
        /// <param name="nN">Specifies the number of batches in the data.</param>
        /// <param name="hX">Specifies the GPU memory containing the input data.</param>
        /// <param name="hY">Specifies the GPU memory containing the target data.</param>
        /// <param name="hW">Specifies the GPU memory optionally containing the weights, or 0 to ignore.</param>
        /// <param name="hPosW">Specifies the GPU memory optionally containing the position weights, or 0 to ignore.</param>
        /// <param name="hLossData">Specifies the GPU memory where the calculated loss is placed as a vector of same size as X and Y.</param>
        /// <remarks>
        /// @see [What does BCEWithLogitsLoss actually do?](https://kamilelukosiute.com/2022/04/14/bce-with-logits-loss/) by Kamile Lukosiute, 2022
        /// @see [How to Use PyTorch's BCEWithLogitsLoss Function](https://reason.town/pytorch-bcewithlogitsloss/) by joseph, 2022
        /// </remarks>
        public void bce_with_logits_loss_fwd(int nCount, int nN, long hX, long hY, long hW, long hPosW, long hLossData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDAFN_BCE_WITH_LOGITS_LOSS_FWD, null, m_param.AsLong(nCount, nN, hX, hY, hW, hPosW, hLossData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDAFN_BCE_WITH_LOGITS_LOSS_FWD, null, m_param.AsLong(nCount, nN, hX, hY, hW, hPosW, hLossData));
        }

        /// <summary>
        /// Calculates the BCEWithLogitsLoss backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of elements in hX, hY, hW (optional) and hPosW (optional).</param>
        /// <param name="nN">Specifies the number of batches in the data.</param>
        /// <param name="hX">Specifies the GPU memory containing the input data.</param>
        /// <param name="hY">Specifies the GPU memory containing the target data.</param>
        /// <param name="hW">Specifies the GPU memory optionally containing the weights, or 0 to ignore.</param>
        /// <param name="hPosW">Specifies the GPU memory optionally containing the position weights, or 0 to ignore.</param>
        /// <param name="bMeanReduction">Specifies that mean-reduction was used.</param>
        /// <param name="hGrad">Specifies the GPU memory where the newly calculated gradients are placed.</param>
        /// <remarks>
        /// @see [What does BCEWithLogitsLoss actually do?](https://kamilelukosiute.com/2022/04/14/bce-with-logits-loss/) by Kamile Lukosiute, 2022
        /// @see [How to Use PyTorch's BCEWithLogitsLoss Function](https://reason.town/pytorch-bcewithlogitsloss/) by joseph, 2022
        /// </remarks>
        public void bce_with_logits_loss_bwd(int nCount, int nN, long hX, long hY, long hW, long hPosW, bool bMeanReduction, long hGrad)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDAFN_BCE_WITH_LOGITS_LOSS_BWD, null, m_param.AsLong(nCount, nN, hX, hY, hW, hPosW, (bMeanReduction) ? 1 : 0, hGrad));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDAFN_BCE_WITH_LOGITS_LOSS_BWD, null, m_param.AsLong(nCount, nN, hX, hY, hW, hPosW, (bMeanReduction) ? 1 : 0, hGrad));
        }

        /// <summary>
        /// Performs a Mish forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Computes the mish non-linearity @f$ f(x)  = x * tanh(ln( 1 + e^x )) @f$.
        /// 
        /// @see [Mish: A Self Regularized Non-Monotonic Neural Activation Function](https://arxiv.org/abs/1908.08681v1) by Diganta Misra, 2019.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items in the bottom and top data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="dfThreshold">Specifies the threshold value.</param>
        public void mish_fwd(int nCount, long hBottomData, long hTopData, double dfThreshold)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MISH_FWD, m_param.AsDouble(dfThreshold), m_param.AsLong(nCount, hBottomData, hTopData, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MISH_FWD, m_param.AsFloat((float)dfThreshold), m_param.AsLong(nCount, hBottomData, hTopData, 0));
        }

        /// <summary>
        /// Performs a Mish backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Computes the mish gradient @f$ f(x)' = \frac{ exp(x) * (4*e^x * x + 4*x + 6*e^x + 4*e^2x + e^3x + 4) }{ (2*e^x + e^2x + 2)^2 } @f$
        /// Note, see Wolfram Alpha with 'derivative of x * tanh(ln(1 + e^x))'                                         
        /// 
        /// @see [Mish: A Self Regularized Non-Monotonic Neural Activation Function](https://arxiv.org/abs/1908.08681v1) by Diganta Misra, 2019.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="hBottomData">Specifies a handle tot he bottom data in GPU memory.</param>
        /// <param name="dfThreshold">Specifies the threshold value.</param>
        /// <param name="nMethod">Optionally, specifies to run the new implementation when > 0.</param>
        public void mish_bwd(int nCount, long hTopDiff, long hTopData, long hBottomDiff, long hBottomData, double dfThreshold, int nMethod = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MISH_BWD, m_param.AsDouble(dfThreshold), m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData, 0, nMethod));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MISH_BWD, m_param.AsFloat((float)dfThreshold), m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData, 0, nMethod));
        }

        /// <summary>
        /// Performs a GELU forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// When bEnableBertVersion=false (default)
        /// Computes the GELU non-linearity @f$ y = cdf + x * pdf @f$
        ///                           where @f$ cdf = 0.5 * (1.0 + erf(x / sqrt(2.0))) @f$
        ///                                 @f$ pdf = 1.0 / sqrt(2.0 * PI) * exp(-0.5 * x^2) @f$
        /// 
        /// with                            @f$ y' = cdf + x * pdf @f$
        /// @see [On the GELU Activation Function](https://alaaalatif.github.io/2019-04-11-gelu/)
        /// 
        /// When bEnableBertVersion=True
        /// Computes the GELU non-linearity @f$ f(x)  =y  = 0.5 * (1.0 + tanh(sqrt(2.0/PI) * (x + 0.044715 * x^3))) @f$.
        /// 
        /// @see [Github - Karpathy: NewGELU, line 21](https://github.com/karpathy/minGPT/blob/master/mingpt/model.py) by Karpathy, 2022.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items in the bottom and top data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="bEnableBertVersion">Specifies to use the BERT version or the default version.</param>
        public void gelu_fwd(int nCount, long hBottomData, long hTopData, bool bEnableBertVersion)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_GELU_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData, (bEnableBertVersion) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_GELU_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData, (bEnableBertVersion) ? 1 : 0));
        }

        /// <summary>
        /// Performs a GELU backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Computes the GELU gradient.
        /// When bEnableBertVersion=false (default)
        /// Computes the GELU non-linearity @f$ y = cdf + x * pdf @f$
        ///                           where @f$ cdf = 0.5 * (1.0 + erf(x / sqrt(2.0))) @f$
        ///                                 @f$ pdf = 1.0 / sqrt(2.0 * PI) * exp(-0.5 * x^2) @f$
        /// 
        /// with                            @f$ y' = cdf + x * pdf @f$
        /// @see [On the GELU Activation Function](https://alaaalatif.github.io/2019-04-11-gelu/)
        /// 
        /// When bEnableBertVersion=true, 
        ///                                 @f$ y' = 0.5 * tanh(0.797885 * (x + 0.044715 * x^3)) + 
        ///                                          (0.0535161 * x^3 + 0.398942 * x) * sech^2(0.797885 * (x + 0.044715 * x^3)) + 0.5 @f$
        /// Note, see Wolfram Alpha with 'derivative of d/dx  = 0.5 * x * (1.0 + tanh(sqrt(2.0/PI) * (x + 0.044715 * x^3)))'                                         
        /// 
        /// @see [Github - Karpathy: NewGELU, line 21](https://github.com/karpathy/minGPT/blob/master/mingpt/model.py) by Karpathy, 2022.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="hBottomData">Specifies a handle tot he bottom data in GPU memory.</param>
        /// <param name="bEnableBertVersion">Specifies to use the BERT version, or default version.</param>
        public void gelu_bwd(int nCount, long hTopDiff, long hTopData, long hBottomDiff, long hBottomData, bool bEnableBertVersion)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_GELU_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData, (bEnableBertVersion) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_GELU_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData, (bEnableBertVersion) ? 1 : 0));
        }

        /// <summary>
        /// Performs the Sigmoid-weighted Linear Unit (SiLU) activation forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Computes the SiLU non-linearity @f$ y  = x * sigmoid(x) @f$
        ///                                 @f$ y' = sigmoid(x) * (1 + x * (1 - sigmoid(x)) @f$
        /// 
        /// @see [Brief Review - SiLU: Sigmoid-weighted Linear Unit](https://sh-tsang.medium.com/review-silu-sigmoid-weighted-linear-unit-be4bc943624d) by Sik-Ho Tsang, 2022, Medium.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items in the bottom and top data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void silu_fwd(int nCount, long hBottomData, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SILU_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SILU_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData));
        }

        /// <summary>
        /// Performs the Sigmoid-weighted Linear Unit (SiLU) activation backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Computes the SiLU non-linearity @f$ y  = x * sigmoid(x) @f$
        ///                                 @f$ y' = sigmoid(x) * (1 + x * (1 - sigmoid(x)) @f$
        /// 
        /// @see [Brief Review - SiLU: Sigmoid-weighted Linear Unit](https://sh-tsang.medium.com/review-silu-sigmoid-weighted-linear-unit-be4bc943624d) by Sik-Ho Tsang, 2022, Medium.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="hBottomData">Specifies a handle tot he bottom data in GPU memory.</param>
        public void silu_bwd(int nCount, long hTopDiff, long hTopData, long hBottomDiff, long hBottomData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SILU_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SILU_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData));
        }


        /// <summary>
        /// Performs the Softplus function forward, a smooth approximation of the ReLU function
        /// </summary>
        /// <remarks>
        /// Computes the SoftPlus non-linearity @f$ y  = log(1 + e^x) @f$
        ///                                 @f$ y' = sigmoid(x) @f$
        /// 
        /// @see [Softplus function - Smooth approximation of the ReLU function](https://neuralthreads.medium.com/softplus-function-smooth-approximation-of-the-relu-function-6a85f92a98e6) by neuralthreds, 2021, Medium.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items in the bottom and top data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void softplus_fwd(int nCount, long hBottomData, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SOFTPLUS_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SOFTPLUS_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData));
        }

        /// <summary>
        /// Performs the Softplus function backward, a smooth approximation of the ReLU function
        /// </summary>
        /// <remarks>
        /// Computes the SoftPlus non-linearity @f$ y  = log(1 + e^x) @f$
        ///                                 @f$ y' = sigmoid(x) @f$
        /// 
        /// @see [Softplus function - Smooth approximation of the ReLU function](https://neuralthreads.medium.com/softplus-function-smooth-approximation-of-the-relu-function-6a85f92a98e6) by neuralthreds, 2021, Medium.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="hBottomData">Specifies a handle tot he bottom data in GPU memory.</param>
        public void softplus_bwd(int nCount, long hTopDiff, long hTopData, long hBottomDiff, long hBottomData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SOFTPLUS_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SOFTPLUS_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData));
        }

        /// <summary>
        /// Performs the LeCun's Tanh function forward
        /// </summary>
        /// <remarks>
        /// Computes the LeCun non-linearity @f$ y  = 1.7159 * tanh(2/3 * x) @f$
        ///                                  @f$ y' = 1.7159 * 2/3 * (1 - tanh(2/3 * x)^2) @f$
        /// 
        /// @see [Lecun's Tanh](https://paperswithcode.com/method/lecun-s-tanh) by PapersWithCode.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items in the bottom and top data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void lecun_fwd(int nCount, long hBottomData, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LECUN_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LECUN_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData));
        }

        /// <summary>
        /// Performs the LeCun's Tanh function backward
        /// </summary>
        /// <remarks>
        /// Computes the LeCun non-linearity @f$ y  = 1.7159 * tanh(2/3 * x) @f$
        ///                                  @f$ y' = 1.7159 * 2/3 * (1 - tanh(2/3 * x)^2) @f$
        /// 
        /// @see [Lecun's Tanh](https://paperswithcode.com/method/lecun-s-tanh) by PapersWithCode.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="hBottomData">Specifies a handle tot he bottom data in GPU memory.</param>
        public void lecun_bwd(int nCount, long hTopDiff, long hTopData, long hBottomDiff, long hBottomData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LECUN_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LECUN_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData));
        }

        /// <summary>
        /// Performs a Serf forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Computes the serf non-linearity @f$ f(x) = x erf(\ln( 1 + \exp(x) )) @f$.
        /// 
        /// @see [Serf: Towards better training of deep neural networks using log-Softplus ERror activation Function](https://arxiv.org/pdf/2108.09598.pdf) by Sayan Nag and Mayukh Bhattacharyya, 2021.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items in the bottom and top data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="dfThreshold">Specifies the threshold value.</param>
        public void serf_fwd(int nCount, long hBottomData, long hTopData, double dfThreshold)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SERF_FWD, m_param.AsDouble(dfThreshold), m_param.AsLong(nCount, hBottomData, hTopData, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SERF_FWD, m_param.AsFloat((float)dfThreshold), m_param.AsLong(nCount, hBottomData, hTopData, 0));
        }

        /// <summary>
        /// Performs a Serf backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Computes the serf gradient @f$ f(x)' = \text{erf}\left(\log \left(e^x+1\right)\right)+\frac{2 x e^{x-\log^2\left(e^x+1\right)}}{\sqrt{\pi } \left(e^x+1\right)} @f$
        /// 
        /// @see [Serf: Towards better training of deep neural networks using log-Softplus ERror activation Function](https://arxiv.org/pdf/2108.09598.pdf) by Sayan Nag and Mayukh Bhattacharyya, 2021.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="hBottomData">Specifies a handle tot he bottom data in GPU memory.</param>
        /// <param name="dfThreshold">Specifies the threshold value.</param>
        public void serf_bwd(int nCount, long hTopDiff, long hTopData, long hBottomDiff, long hBottomData, double dfThreshold)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SERF_BWD, m_param.AsDouble(dfThreshold), m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SERF_BWD, m_param.AsFloat((float)dfThreshold), m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, hBottomData, 0));
        }

        /// <summary>
        /// Performs a TanH forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Calculation @f$ f(x) = tanh(x) @f$
        /// 
        /// @see [Hyperbolic Function](https://en.wikipedia.org/wiki/Hyperbolic_function).
        /// </remarks>
        /// <param name="nCount">Specifies the number of items in the bottom and top data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void tanh_fwd(int nCount, long hBottomData, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TANH_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TANH_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData));
        }

        /// <summary>
        /// Performs a TanH backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// @see [Hyperbolic Function](https://en.wikipedia.org/wiki/Hyperbolic_function).
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void tanh_bwd(int nCount, long hTopDiff, long hTopData, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TANH_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TANH_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff));
        }

        /// <summary>
        /// Performs a Sigmoid forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Calcuation @f$ f(x) = 1.0 / (1.0 + e^-x) @f$
        /// 
        /// @see [Sigmoid Function](https://en.wikipedia.org/wiki/Sigmoid_function).
        /// </remarks>
        /// <param name="nCount">Specifies the number of items in the bottom and top data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void sigmoid_fwd(int nCount, long hBottomData, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SIGMOID_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SIGMOID_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData));
        }

        /// <summary>
        /// Performs a Sigmoid backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// @see [Sigmoid Function](https://en.wikipedia.org/wiki/Sigmoid_function).
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void sigmoid_bwd(int nCount, long hTopDiff, long hTopData, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SIGMOID_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SIGMOID_BWD, null, m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff));
        }

        /// <summary>
        /// Performs a Swish backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// @see [Activation Functions](https://arxiv.org/abs/1710.05941v2) by Prajit Ramachandran, Barret Zoph, Quoc V. Le., 2017.
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hSigmoidOutputData">Specifies a handle to the sigmoid output data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="dfBeta">Specifies the 'beta' value applied to the output.</param>
        public void swish_bwd(int nCount, long hTopDiff, long hTopData, long hSigmoidOutputData, long hBottomDiff, double dfBeta)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SWISH_BWD, m_param.AsDouble(dfBeta), m_param.AsLong(nCount, hTopDiff, hTopData, hSigmoidOutputData, hBottomDiff, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SWISH_BWD, m_param.AsFloat((float)dfBeta), m_param.AsLong(nCount, hTopDiff, hTopData, hSigmoidOutputData, hBottomDiff, 0));
        }

        /// <summary>
        /// Performs a Rectifier Linear Unit (ReLU) forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Calculation @f$ f(x) = (x > 0) ? x : x * negativeSlope @f$
        /// 
        /// @see [Rectifier](https://en.wikipedia.org/wiki/Rectifier_(neural_networks)), and
        /// @see [Understanding Deep Neural Networks with Rectified Linear Units](https://arxiv.org/abs/1611.01491) by Arora, et al., 2016,
        /// @see [Delving Deep into Rectifiers: Surpassing Human-Level Performance on ImageNet Classification](https://arxiv.org/abs/1502.01852v1) by He, et al., 2015
        /// </remarks>
        /// <param name="nCount">Specifies the number of items in the bottom and top data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="fNegativeSlope">Specifies the negative slope.</param>
        public void relu_fwd(int nCount, long hBottomData, long hTopData, T fNegativeSlope)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_RELU_FWD, m_param.AsDouble(convertD(fNegativeSlope)), m_param.AsLong(nCount, hBottomData, hTopData, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_RELU_FWD, m_param.AsFloat(convertF(fNegativeSlope)), m_param.AsLong(nCount, hBottomData, hTopData, 0));
        }

        /// <summary>
        /// Performs a Rectifier Linear Unit (ReLU) backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// @see [Rectifier](https://en.wikipedia.org/wiki/Rectifier_(neural_networks)), and
        /// @see [Understanding Deep Neural Networks with Rectified Linear Units](https://arxiv.org/abs/1611.01491) by Arora, et al., 2016,
        /// @see [Delving Deep into Rectifiers: Surpassing Human-Level Performance on ImageNet Classification](https://arxiv.org/abs/1502.01852v1) by He, et al., 2015
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="fNegativeSlope">Specifies the negative slope.</param>
        public void relu_bwd(int nCount, long hTopDiff, long hTopData, long hBottomDiff, T fNegativeSlope)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_RELU_BWD, m_param.AsDouble(convertD(fNegativeSlope)), m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_RELU_BWD, m_param.AsFloat(convertF(fNegativeSlope)), m_param.AsLong(nCount, hTopDiff, hTopData, hBottomDiff, 0));
        }

        /// <summary>
        /// Performs a Exponential Linear Unit (ELU) forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Calculates @f$ f(x) = (x > 0) ? x : \alpha * (e^x - 1) @f$
        /// 
        /// @see [Deep Residual Networks with Exponential Linear Unit](https://arxiv.org/abs/1604.04112) by Shah, et al., 2016
        /// </remarks>
        /// <param name="nCount">Specifies the number of items in the bottom and top data.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="dfAlpha">Specifies the alpha value.</param>
        public void elu_fwd(int nCount, long hBottomData, long hTopData, double dfAlpha)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ELU_FWD, m_param.AsDouble(dfAlpha), m_param.AsLong(nCount, hBottomData, hTopData, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ELU_FWD, m_param.AsFloat((float)dfAlpha), m_param.AsLong(nCount, hBottomData, hTopData, 0));
        }

        /// <summary>
        /// Performs a Exponential Linear Unit (ELU) backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// @see [Deep Residual Networks with Exponential Linear Unit](https://arxiv.org/abs/1604.04112) by Shah, et al., 2016
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="dfAlpha">Specifies the alpha value.</param>
        public void elu_bwd(int nCount, long hTopDiff, long hTopData, long hBottomData, long hBottomDiff, double dfAlpha)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ELU_BWD, m_param.AsDouble(dfAlpha), m_param.AsLong(nCount, hTopDiff, hTopData, hBottomData, hBottomDiff, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ELU_BWD, m_param.AsFloat((float)dfAlpha), m_param.AsLong(nCount, hTopDiff, hTopData, hBottomData, hBottomDiff, 0));
        }

        /// <summary>
        /// Performs a dropout forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// @see [Improving neural networks by preventing co-adaptation of feature detectors](https://arxiv.org/abs/1207.0580) by Hinton, et al., 2012
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hMask">Specifies a handle to the mask data in GPU memory.</param>
        /// <param name="uiThreshold">Specifies the threshold value: when mask value are less than the threshold, the data item is 'dropped out' by setting the data item to zero.</param>
        /// <param name="fScale">Specifies a scale value applied to each item that is not dropped out.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void dropout_fwd(int nCount, long hBottomData, long hMask, uint uiThreshold, T fScale, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_DROPOUT_FWD, m_param.AsDouble(convertD(fScale)), m_param.AsLong(nCount, hBottomData, hMask, uiThreshold, 0, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_DROPOUT_FWD, m_param.AsFloat(convertF(fScale)), m_param.AsLong(nCount, hBottomData, hMask, uiThreshold, 0, hTopData));
        }

        /// <summary>
        /// Performs a dropout backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// @see [Improving neural networks by preventing co-adaptation of feature detectors](https://arxiv.org/abs/1207.0580) by Hinton, et al., 2012
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hMask">Specifies a handle to the mask data in GPU memory.</param>
        /// <param name="uiThreshold">Specifies the threshold value: when mask value are less than the threshold, the data item is 'dropped out' by setting the data item to zero.</param>
        /// <param name="fScale">Specifies a scale value applied to each item that is not dropped out.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void dropout_bwd(int nCount, long hTopDiff, long hMask, uint uiThreshold, T fScale, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_DROPOUT_BWD, m_param.AsDouble(convertD(fScale)), m_param.AsLong(nCount, hTopDiff, hMask, uiThreshold, 0, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_DROPOUT_BWD, m_param.AsFloat(convertF(fScale)), m_param.AsLong(nCount, hTopDiff, hMask, uiThreshold, 0, hBottomDiff));
        }

        /// <summary>
        /// Performs a binomial normal log liklihod (BNLL) forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Computes @f$ f(x) = ln(1 + e^x) @f$
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void bnll_fwd(int nCount, long hBottomData, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_BNLL_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_BNLL_FWD, null, m_param.AsLong(nCount, hBottomData, hTopData));
        }

        /// <summary>
        /// Performs a binomial normal log liklihod (BNLL) backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void bnll_bwd(int nCount, long hTopDiff, long hBottomData, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_BNLL_BWD, null, m_param.AsLong(nCount, hTopDiff, hBottomData, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_BNLL_BWD, null, m_param.AsLong(nCount, hTopDiff, hBottomData, hBottomDiff));
        }

        /// <summary>
        /// Performs Parameterized Rectifier Linear Unit (ReLU) forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Calculation @f$ f(x) = (x > 0) ? x : x * slopeData @f$
        /// 
        /// @see [Understanding Deep Neural Networks with Rectified Linear Units](https://arxiv.org/abs/1611.01491) by Arora, et al., 2016,
        /// @see [Delving Deep into Rectifiers: Surpassing Human-Level Performance on ImageNet Classification](https://arxiv.org/abs/1502.01852v1) by He, et al., 2015
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="nChannels">Specifies the channels per input.</param>
        /// <param name="nDim">Specifies the dimension of each input.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hSlopeData">Specifies a handle to the slope data in GPU memory.</param>
        /// <param name="nDivFactor">Specifies the div factor applied to the channels.</param>
        public void prelu_fwd(int nCount, int nChannels, int nDim, long hBottomData, long hTopData, long hSlopeData, int nDivFactor)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_PRELU_FWD, null, m_param.AsLong(nCount, nChannels, nDim, hBottomData, hTopData, hSlopeData, nDivFactor));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_PRELU_FWD, null, m_param.AsLong(nCount, nChannels, nDim, hBottomData, hTopData, hSlopeData, nDivFactor));
        }


        /// <summary>
        /// Performs Parameterized Rectifier Linear Unit (ReLU) backward param pass in Cuda.
        /// </summary>
        /// <remarks>
        /// @see [Understanding Deep Neural Networks with Rectified Linear Units](https://arxiv.org/abs/1611.01491) by Arora, et al., 2016,
        /// @see [Delving Deep into Rectifiers: Surpassing Human-Level Performance on ImageNet Classification](https://arxiv.org/abs/1502.01852v1) by He, et al., 2015
        /// </remarks>
        /// <param name="nCDim"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nNum"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nTopOffset"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hBackBuffDiff">Specifies a handle to the back buffer diff in GPU memory.</param>
        public void prelu_bwd_param(int nCDim, int nNum, int nTopOffset, long hTopDiff, long hBottomData, long hBackBuffDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_PRELU_BWD_PARAM, null, m_param.AsLong(nCDim, nNum, nTopOffset, hTopDiff, hBottomData, hBackBuffDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_PRELU_BWD_PARAM, null, m_param.AsLong(nCDim, nNum, nTopOffset, hTopDiff, hBottomData, hBackBuffDiff));
        }

        /// <summary>
        /// Performs Parameterized Rectifier Linear Unit (ReLU) backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// @see [Understanding Deep Neural Networks with Rectified Linear Units](https://arxiv.org/abs/1611.01491) by Arora, et al., 2016,
        /// @see [Delving Deep into Rectifiers: Surpassing Human-Level Performance on ImageNet Classification](https://arxiv.org/abs/1502.01852v1) by He, et al., 2015
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="nChannels">Specifies the channels per input.</param>
        /// <param name="nDim">Specifies the dimension of each input.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="hSlopeData">Specifies a handle to the slope data in GPU memory.</param>
        /// <param name="nDivFactor">Specifies the div factor applied to the channels.</param>
        public void prelu_bwd(int nCount, int nChannels, int nDim, long hTopDiff, long hBottomData, long hBottomDiff, long hSlopeData, int nDivFactor)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_PRELU_BWD, null, m_param.AsLong(nCount, nChannels, nDim, hTopDiff, hBottomData, hBottomDiff, hSlopeData, nDivFactor));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_PRELU_BWD, null, m_param.AsLong(nCount, nChannels, nDim, hTopDiff, hBottomData, hBottomDiff, hSlopeData, nDivFactor));
        }

        /// <summary>
        /// Performs Softmax Loss forward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hProbData">Specifies a handle to the probability data in GPU memory.</param>
        /// <param name="hLabel">Specifies a handle to the label data in GPU memory.</param>
        /// <param name="hLossData">Specifies a handle to the loss data in GPU memory.</param>
        /// <param name="nOuterNum"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nDim"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nInnerNum"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hCounts">Specifies a handle to the counts in GPU memory.</param>
        /// <param name="nIgnoreLabel">Optionally, specifies a label to ignore.</param>
        public void softmaxloss_fwd(int nCount, long hProbData, long hLabel, long hLossData, int nOuterNum, int nDim, int nInnerNum, long hCounts, int? nIgnoreLabel)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<long> rg = new List<long>() { nCount, hProbData, hLabel, hLossData, nOuterNum, nDim, nInnerNum, hCounts };

                if (nIgnoreLabel.HasValue)
                    rg.Add(nIgnoreLabel.Value);

                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SOFTMAXLOSS_FWD, null, rg.ToArray());
            }
            else
            {
                List<long> rg = new List<long>() { nCount, hProbData, hLabel, hLossData, nOuterNum, nDim, nInnerNum, hCounts };

                if (nIgnoreLabel.HasValue)
                    rg.Add(nIgnoreLabel.Value);

                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SOFTMAXLOSS_FWD, null, rg.ToArray());
            }
        }

        /// <summary>
        /// Performs Softmax Loss backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hLabel">Specifies a handle to the label data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="nOuterNum"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nDim"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nInnerNum"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hCounts">Specifies a handle to the counts in GPU memory.</param>
        /// <param name="nIgnoreLabel">Optionally, specifies a label to ignore.</param>
        public void softmaxloss_bwd(int nCount, long hTopData, long hLabel, long hBottomDiff, int nOuterNum, int nDim, int nInnerNum, long hCounts, int? nIgnoreLabel)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<long> rg = new List<long>() { nCount, hTopData, hLabel, hBottomDiff, nOuterNum, nDim, nInnerNum, hCounts };

                if (nIgnoreLabel.HasValue)
                    rg.Add(nIgnoreLabel.Value);
                
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SOFTMAXLOSS_BWD, null, rg.ToArray());
            }
            else
            {
                List<long> rg = new List<long>() { nCount, hTopData, hLabel, hBottomDiff, nOuterNum, nDim, nInnerNum, hCounts };

                if (nIgnoreLabel.HasValue)
                    rg.Add(nIgnoreLabel.Value);

                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SOFTMAXLOSS_BWD, null, rg.ToArray());
            }
        }

        /// <summary>
        /// Performs NLL Loss forward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hProbData">Specifies a handle to the probability data in GPU memory.</param>
        /// <param name="hLabel">Specifies a handle to the label data in GPU memory.</param>
        /// <param name="hLossData">Specifies a handle to the loss data in GPU memory.</param>
        /// <param name="nOuterNum"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nDim"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nInnerNum"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hCounts">Specifies a handle to the counts in GPU memory.</param>
        /// <param name="nIgnoreLabel">Optionally, specifies a label to ignore.</param>
        public void nllloss_fwd(int nCount, long hProbData, long hLabel, long hLossData, int nOuterNum, int nDim, int nInnerNum, long hCounts, int? nIgnoreLabel)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<long> rg = new List<long>() { nCount, hProbData, hLabel, hLossData, nOuterNum, nDim, nInnerNum, hCounts };

                if (nIgnoreLabel.HasValue)
                    rg.Add(nIgnoreLabel.Value);

                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_NLLLOSS_FWD, null, rg.ToArray());
            }
            else
            {
                List<long> rg = new List<long>() { nCount, hProbData, hLabel, hLossData, nOuterNum, nDim, nInnerNum, hCounts };

                if (nIgnoreLabel.HasValue)
                    rg.Add(nIgnoreLabel.Value);

                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_NLLLOSS_FWD, null, rg.ToArray());
            }
        }

        /// <summary>
        /// Performs NLL Loss backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hLabel">Specifies a handle to the label data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        /// <param name="nOuterNum"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nDim"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nInnerNum"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hCounts">Specifies a handle to the counts in GPU memory.</param>
        /// <param name="nIgnoreLabel">Optionally, specifies a label to ignore.</param>
        public void nllloss_bwd(int nCount, long hTopData, long hLabel, long hBottomDiff, int nOuterNum, int nDim, int nInnerNum, long hCounts, int? nIgnoreLabel)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<long> rg = new List<long>() { nCount, hTopData, hLabel, hBottomDiff, nOuterNum, nDim, nInnerNum, hCounts };

                if (nIgnoreLabel.HasValue)
                    rg.Add(nIgnoreLabel.Value);

                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_NLLLOSS_BWD, null, rg.ToArray());
            }
            else
            {
                List<long> rg = new List<long>() { nCount, hTopData, hLabel, hBottomDiff, nOuterNum, nDim, nInnerNum, hCounts };

                if (nIgnoreLabel.HasValue)
                    rg.Add(nIgnoreLabel.Value);

                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_NLLLOSS_BWD, null, rg.ToArray());
            }
        }


        /// <summary>
        /// Performs a max forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Calculation: @f$ Y[i] = max(A[i], B[i]) @f$
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottomDataA">Specifies a handle to the Bottom A data in GPU memory.</param>
        /// <param name="hBottomDataB">Specifies a handle to the Bottom B data in GPU memory.</param>
        /// <param name="nIdx">Specifies the blob index used to set the mask.</param>
        /// <param name="hTopData">Specifies a handle to the Top data in GPU memory.</param>
        /// <param name="hMask">Specifies a handle to the mask data in GPU.</param>
        public void max_fwd(int nCount, long hBottomDataA, long hBottomDataB, int nIdx, long hTopData, long hMask)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MAX_FWD, null, m_param.AsLong(nCount, hBottomDataA, hBottomDataB, nIdx, hTopData, hMask));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MAX_FWD, null, m_param.AsLong(nCount, hBottomDataA, hBottomDataB, nIdx, hTopData, hMask));
        }

        /// <summary>
        /// Performs a max backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nIdx">Specifies the blob index used to test the mask.</param>
        /// <param name="hMask">Specifies a handle to the mask data in GPU.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void max_bwd(int nCount, long hTopDiff, int nIdx, long hMask, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MAX_BWD, null, m_param.AsLong(nCount, hTopDiff, nIdx, hMask, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MAX_BWD, null, m_param.AsLong(nCount, hTopDiff, nIdx, hMask, hBottomDiff));
        }

        /// <summary>
        /// Performs a min forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Calculation: @f$ Y[i] = min(A[i], B[i]) @f$
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottomDataA">Specifies a handle to the Bottom A data in GPU memory.</param>
        /// <param name="hBottomDataB">Specifies a handle to the Bottom B data in GPU memory.</param>
        /// <param name="nIdx">Specifies the blob index used to set the mask.</param>
        /// <param name="hTopData">Specifies a handle to the Top data in GPU memory.</param>
        /// <param name="hMask">Specifies a handle to the mask data in GPU.</param>
        public void min_fwd(int nCount, long hBottomDataA, long hBottomDataB, int nIdx, long hTopData, long hMask)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MIN_FWD, null, m_param.AsLong(nCount, hBottomDataA, hBottomDataB, nIdx, hTopData, hMask));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MIN_FWD, null, m_param.AsLong(nCount, hBottomDataA, hBottomDataB, nIdx, hTopData, hMask));
        }

        /// <summary>
        /// Performs a min backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nIdx">Specifies the blob index used to test the mask.</param>
        /// <param name="hMask">Specifies a handle to the mask data in GPU.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void min_bwd(int nCount, long hTopDiff, int nIdx, long hMask, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MIN_BWD, null, m_param.AsLong(nCount, hTopDiff, nIdx, hMask, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MIN_BWD, null, m_param.AsLong(nCount, hTopDiff, nIdx, hMask, hBottomDiff));
        }

        /// <summary>
        /// Performs the crop forward operation.
        /// </summary>
        /// <param name="nCount">Specifies the count.</param>
        /// <param name="nNumAxes">Specifies the number of axes in the bottom.</param>
        /// <param name="hSrcStrides">Specifies a handle to the GPU memory containing the source strides.</param>
        /// <param name="hDstStrides">Specifies a handle to the GPU memory containing the destination strides.</param>
        /// <param name="hOffsets">Specifies a handle to the GPU memory containing the offsets.</param>
        /// <param name="hBottomData">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void crop_fwd(int nCount, int nNumAxes, long hSrcStrides, long hDstStrides, long hOffsets, long hBottomData, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CROP_FWD, null, m_param.AsLong(nCount, nNumAxes, hSrcStrides, hDstStrides, hOffsets, hBottomData, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CROP_FWD, null, m_param.AsLong(nCount, nNumAxes, hSrcStrides, hDstStrides, hOffsets, hBottomData, hTopData));
        }

        /// <summary>
        /// Performs the crop backward operation.
        /// </summary>
        /// <param name="nCount">Specifies the count.</param>
        /// <param name="nNumAxes">Specifies the number of axes in the bottom.</param>
        /// <param name="hSrcStrides">Specifies a handle to the GPU memory containing the source strides.</param>
        /// <param name="hDstStrides">Specifies a handle to the GPU memory containing the destination strides.</param>
        /// <param name="hOffsets">Specifies a handle to the GPU memory containing the offsets.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTopDiff">Specifies a handle to the top data in GPU memory.</param>
        public void crop_bwd(int nCount, int nNumAxes, long hSrcStrides, long hDstStrides, long hOffsets, long hBottomDiff, long hTopDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CROP_BWD, null, m_param.AsLong(nCount, nNumAxes, hSrcStrides, hDstStrides, hOffsets, hBottomDiff, hTopDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CROP_BWD, null, m_param.AsLong(nCount, nNumAxes, hSrcStrides, hDstStrides, hOffsets, hBottomDiff, hTopDiff));
        }

        /// <summary>
        /// Performs a concat forward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottomData">Specifies a handle to the Bottom data in GPU memory.</param>
        /// <param name="nNumConcats">Specifies the number of concatenations.</param>
        /// <param name="nConcatInputSize">Specifies the concatenation input size.</param>
        /// <param name="nTopConcatAxis">Specifies the top axis to concatenate.</param>
        /// <param name="nBottomConcatAxis"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nOffsetConcatAxis"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void concat_fwd(int nCount, long hBottomData, int nNumConcats, int nConcatInputSize, int nTopConcatAxis, int nBottomConcatAxis, int nOffsetConcatAxis, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CONCAT_FWD, null, m_param.AsLong(nCount, hBottomData, nNumConcats, nConcatInputSize, nTopConcatAxis, nBottomConcatAxis, nOffsetConcatAxis, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CONCAT_FWD, null, m_param.AsLong(nCount, hBottomData, nNumConcats, nConcatInputSize, nTopConcatAxis, nBottomConcatAxis, nOffsetConcatAxis, hTopData));
        }


        /// <summary>
        /// Performs a concat backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nNumConcats">Specifies the number of concatenations.</param>
        /// <param name="nConcatInputSize">Specifies the concatenation input size.</param>
        /// <param name="nTopConcatAxis"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nBottomConcatAxis"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nOffsetConcatAxis"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hBottomDiff">Specifies a handle to the Bottom diff in GPU memory.</param>
        public void concat_bwd(int nCount, long hTopDiff, int nNumConcats, int nConcatInputSize, int nTopConcatAxis, int nBottomConcatAxis, int nOffsetConcatAxis, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CONCAT_BWD, null, m_param.AsLong(nCount, hTopDiff, nNumConcats, nConcatInputSize, nTopConcatAxis, nBottomConcatAxis, nOffsetConcatAxis, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CONCAT_BWD, null, m_param.AsLong(nCount, hTopDiff, nNumConcats, nConcatInputSize, nTopConcatAxis, nBottomConcatAxis, nOffsetConcatAxis, hBottomDiff));
        }

        /// <summary>
        /// Performs a slice forward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottomData">Specifies a handle to the Bottom data in GPU memory.</param>
        /// <param name="nNumSlices">Specifies the number of slices.</param>
        /// <param name="nSliceSize">Specifies the slice size.</param>
        /// <param name="nBottomSliceAxis"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nTopSliceAxis"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nOffsetSliceAxis"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void slice_fwd(int nCount, long hBottomData, int nNumSlices, int nSliceSize, int nBottomSliceAxis, int nTopSliceAxis, int nOffsetSliceAxis, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SLICE_FWD, null, m_param.AsLong(nCount, hBottomData, nNumSlices, nSliceSize, nBottomSliceAxis, nTopSliceAxis, nOffsetSliceAxis, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SLICE_FWD, null, m_param.AsLong(nCount, hBottomData, nNumSlices, nSliceSize, nBottomSliceAxis, nTopSliceAxis, nOffsetSliceAxis, hTopData));
        }

        /// <summary>
        /// Performs a slice backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nNumSlices">Specifies the number of slices.</param>
        /// <param name="nSliceSize">Specifies the slice size.</param>
        /// <param name="nBottomSliceAxis">Specifies the bottom axis to concatenate.</param>
        /// <param name="nTopSliceAxis"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nOffsetSliceAxis"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hBottomDiff">Specifies a handle to the Bottom diff in GPU memory.</param>
        public void slice_bwd(int nCount, long hTopDiff, int nNumSlices, int nSliceSize, int nBottomSliceAxis, int nTopSliceAxis, int nOffsetSliceAxis, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SLICE_BWD, null, m_param.AsLong(nCount, hTopDiff, nNumSlices, nSliceSize, nBottomSliceAxis, nTopSliceAxis, nOffsetSliceAxis, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SLICE_BWD, null, m_param.AsLong(nCount, hTopDiff, nNumSlices, nSliceSize, nBottomSliceAxis, nTopSliceAxis, nOffsetSliceAxis, hBottomDiff));
        }

        /// <summary>
        /// Performs a tile forward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottomData">Specifies a handle to the Bottom data in GPU memory.</param>
        /// <param name="nInnerDim"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nTiles">Specifies the number of tiles.</param>
        /// <param name="nBottomTileAxis"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void tile_fwd(int nCount, long hBottomData, int nInnerDim, int nTiles, int nBottomTileAxis, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TILE_FWD, null, m_param.AsLong(nCount, hBottomData, nInnerDim, nTiles, nBottomTileAxis, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TILE_FWD, null, m_param.AsLong(nCount, hBottomData, nInnerDim, nTiles, nBottomTileAxis, hTopData));
        }

        /// <summary>
        /// Performs a tile backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nTileSize">Specifies the size of each tile.</param>
        /// <param name="nTiles">Specifies the number of tiles.</param>
        /// <param name="nBottomTileAxis"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hBottomDiff">Specifies a handle to the Bottom diff in GPU memory.</param>
        public void tile_bwd(int nCount, long hTopDiff, int nTileSize, int nTiles, int nBottomTileAxis, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TILE_BWD, null, m_param.AsLong(nCount, hTopDiff, nTileSize, nTiles, nBottomTileAxis, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TILE_BWD, null, m_param.AsLong(nCount, hTopDiff, nTileSize, nTiles, nBottomTileAxis, hBottomDiff));
        }

        /// <summary>
        /// Performs a bias forward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottomData">Specifies a handle to the Bottom data in GPU memory.</param>
        /// <param name="hBiasData">Specifies a handle to the bias data in GPU memory.</param>
        /// <param name="nBiasDim">Specifies the bias dimension.</param>
        /// <param name="nInnerDim"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void bias_fwd(int nCount, long hBottomData, long hBiasData, int nBiasDim, int nInnerDim, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_BIAS_FWD, null, m_param.AsLong(nCount, hBottomData, hBiasData, nBiasDim, nInnerDim, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_BIAS_FWD, null, m_param.AsLong(nCount, hBottomData, hBiasData, nBiasDim, nInnerDim, hTopData));
        }

        /// <summary>
        /// Performs a scale forward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Calculation: @f$ f(x) = 
        /// \begin{cases}
        ///     x * scaleData[(i / nInnerDim) \mod nScaleDim], & \text{if } hBias == 0\\
        ///     x * scaleData[(i / nInnerDim) \mod nScaleDim] + biasData[(i / nInnerDim) \mod nScaleDim] & \text{otherwise}
        /// \end{cases} @f$
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hX">Specifies the input data X in GPU memory.</param>
        /// <param name="hScaleData"></param>
        /// <param name="nScaleDim"></param>
        /// <param name="nInnerDim"></param>
        /// <param name="hY">Specifies the output data Y in GPU memory.</param>
        /// <param name="hBiasData">Optionally, specifies the bias data in GPU memory.</param>
        public void scale_fwd(int nCount, long hX, long hScaleData, int nScaleDim, int nInnerDim, long hY, long hBiasData = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SCALE_FWD, null, m_param.AsLong(nCount, hX, hScaleData, nScaleDim, nInnerDim, hY, hBiasData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SCALE_FWD, null, m_param.AsLong(nCount, hX, hScaleData, nScaleDim, nInnerDim, hY, hBiasData));
        }

        /// <summary>
        /// Performs a threshold pass in Cuda.
        /// </summary>
        /// <remarks>
        /// Calculation: @f$ Y[i] = (X[i] > threshold) ? 1 : 0 @f$
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="dfThreshold">Specifies the threshold value.</param>
        /// <param name="hX">Specifies the input data X in GPU memory.</param>
        /// <param name="hY">Specifies the output data Y in GPU memory.</param>
        public void threshold_fwd(int nCount, double dfThreshold, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_THRESHOLD_FWD, m_param.AsDouble(dfThreshold), m_param.AsLong(nCount, 0, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_THRESHOLD_FWD, m_param.AsFloat((float)dfThreshold), m_param.AsLong(nCount, 0, hX, hY));
        }

        /// <summary>
        /// Performs a contrastive loss layer backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// See [Dimensionality Reduction by Learning an Invariant Mapping](http://yann.lecun.com/exdb/publis/pdf/hadsell-chopra-lecun-06.pdf) by Hadsel, et al., 2006
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="nChannels">Specifies the number of channels.</param>
        /// <param name="dfMargin"><b><i>Specifies the margin to use.  The default is 1.0.</i></b></param>
        /// <param name="bLegacyVersion">When <code>false</code> the calculation proposed by Hadsell, et al., 2006 is used where @f$ (margin - d)^2 @f$, 
        /// otherwise the legacy version is used where @f$ (margin - d^2) @f$.  The default is <code>false</code></param>
        /// <param name="dfAlpha"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hY">Specifies the Y data in GPU memory used to determine similar pairs.</param>
        /// <param name="hDiff">Specifies the diff in GPU memory.</param>
        /// <param name="hDistSq">Specifies the distance squared data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies the bottom diff in GPU memory.</param>
        public void cll_bwd(int nCount, int nChannels, double dfMargin, bool bLegacyVersion, double dfAlpha, long hY, long hDiff, long hDistSq, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CLL_BWD, m_param.AsDouble(dfMargin, dfAlpha), m_param.AsLong(nCount, nChannels, 0, (bLegacyVersion) ? 1 : 0, 0, hY, hDiff, hDistSq, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CLL_BWD, m_param.AsFloat((float)dfMargin, (float)dfAlpha), m_param.AsLong(nCount, nChannels, 0, (bLegacyVersion) ? 1 : 0, 0, hY, hDiff, hDistSq, hBottomDiff));
        }

        /// <summary>
        /// Performs the forward operation for the SmoothL1 loss.
        /// </summary>
        /// <remarks>
        /// Calculation: 
        ///     f(x) = 0.5 * x^2, if |x| lt 1
        ///          = |x| - 0.5, otherwise
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hX">Specifies the input data X in GPU memory.</param>
        /// <param name="hY">Specifies the output data Y in GPU memory.</param>
        public void smoothl1_fwd(int nCount, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SMOOTHL1_FWD, null, m_param.AsLong(nCount, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SMOOTHL1_FWD, null, m_param.AsLong( nCount, hX, hY));
        }

        /// <summary>
        /// Performs the backward operation for the SmoothL1 loss.
        /// </summary>
        /// <remarks>
        /// Calculation: 
        ///     f'(x) = x, if |x| lt 1
        ///           = sign(x), otherwise
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hX">Specifies the input data X in GPU memory.</param>
        /// <param name="hY">Specifies the output data Y in GPU memory.</param>
        public void smoothl1_bwd(int nCount, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SMOOTHL1_BWD, null, m_param.AsLong(nCount, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SMOOTHL1_BWD, null, m_param.AsLong( nCount, hX, hY));
        }

        /// <summary>
        /// Performs data permutation on the input and reorders the data which is placed in the output.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottom">Specifies the input data.</param>
        /// <param name="bFwd">Specifies whether or not this is a forward (<i>true</i>) or backwards (<i>true</i>) operation.</param>
        /// <param name="hPermuteOrder">Specifies the permuation order values in GPU memory.</param>
        /// <param name="hOldSteps">Specifies the old step values in GPU memory.</param>
        /// <param name="hNewSteps">Specifies the new step values in GPU memory.</param>
        /// <param name="nNumAxes">Specifies the number of axes.</param>
        /// <param name="hTop">Specifies the output data.</param>
        public void permute(int nCount, long hBottom, bool bFwd, long hPermuteOrder, long hOldSteps, long hNewSteps, int nNumAxes, long hTop)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_PERMUTE, null, m_param.AsLong(nCount, hBottom, (bFwd) ? 1 : 0, hPermuteOrder, hOldSteps, hNewSteps, nNumAxes, hTop));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_PERMUTE, null, m_param.AsLong( nCount, hBottom, (bFwd) ? 1 : 0, hPermuteOrder, hOldSteps, hNewSteps, nNumAxes, hTop));
        }

        /// <summary>
        /// Performs a gather forward pass where data at specifies indexes along a given axis are copied to the output data.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottom">Specifies the input data.</param>
        /// <param name="hTop">Specifies the output data.</param>
        /// <param name="nAxis">Specifies the axis along which to copy.</param>
        /// <param name="nDim">Specifies the dimension of each item at each index.</param>
        /// <param name="nDimAtAxis">Specifies the dimension at the axis.</param>
        /// <param name="nM">Specifies the M dimension.</param>
        /// <param name="nN">Specifies the M dimension.</param>
        /// <param name="hIdx">Specifies the indexes of the data to gather.</param>
        public void gather_fwd(int nCount, long hBottom, long hTop, int nAxis, int nDim, int nDimAtAxis, int nM, int nN, long hIdx)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_GATHER_FWD, null, m_param.AsLong(nCount, hBottom, hTop, nAxis, nDim, nDimAtAxis, nM, nN, hIdx));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_GATHER_FWD, null, m_param.AsLong( nCount, hBottom, hTop, nAxis, nDim, nDimAtAxis, nM, nN, hIdx));
        }

        /// <summary>
        /// Performs a gather backward pass where data at specifies indexes along a given axis are copied to the output data.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hTop">Specifies the input data.</param>
        /// <param name="hBottom">Specifies the output data.</param>
        /// <param name="nAxis">Specifies the axis along which to copy.</param>
        /// <param name="nDim">Specifies the dimension of each item at each index.</param>
        /// <param name="nDimAtAxis">Specifies the dimension at the axis.</param>
        /// <param name="nM">Specifies the M dimension.</param>
        /// <param name="nN">Specifies the M dimension.</param>
        /// <param name="hIdx">Specifies the indexes of the data to gather.</param>
        public void gather_bwd(int nCount, long hTop, long hBottom, int nAxis, int nDim, int nDimAtAxis, int nM, int nN, long hIdx)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_GATHER_BWD, null, m_param.AsLong(nCount, hTop, hBottom, nAxis, nDim, nDimAtAxis, nM, nN, hIdx));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_GATHER_BWD, null, m_param.AsLong( nCount, hTop, hBottom, nAxis, nDim, nDimAtAxis, nM, nN, hIdx));
        }

        /// <summary>
        /// Performs the fill scale operation used to calculate the LRN cross channel forward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottomData">Specifies a handle to the Bottom data in GPU memory.</param>
        /// <param name="nNum">Specifies the number of input items.</param>
        /// <param name="nChannels">Specifies the number of channels per input item.</param>
        /// <param name="nHeight">Specifies the height of each input item.</param>
        /// <param name="nWidth">Specifies the width of each input item.</param>
        /// <param name="nSize"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="fAlphaOverSize">Specifies the alpha value over the size.</param>
        /// <param name="fK">Specifies the k value.</param>
        /// <param name="hScaleData">Specifies a handle to the scale data in GPU memory.</param>
        public void lrn_fillscale(int nCount, long hBottomData, int nNum, int nChannels, int nHeight, int nWidth, int nSize, T fAlphaOverSize, T fK, long hScaleData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LRN_FILLSCALE, m_param.AsDouble(convertD(fAlphaOverSize), convertD(fK)), m_param.AsLong(nCount, hBottomData, nNum, nChannels, nHeight, nWidth, nSize, 0, 0, hScaleData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LRN_FILLSCALE, m_param.AsFloat(convertF(fAlphaOverSize), convertF(fK)), m_param.AsLong(nCount, hBottomData, nNum, nChannels, nHeight, nWidth, nSize, 0, 0, hScaleData));
        }

        /// <summary>
        /// Computes the output used to calculate the LRN cross channel forward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottomData">Specifies a handle to the Bottom data in GPU memory.</param>
        /// <param name="hScaleData">Specifies a handle to the scale data in GPU memory.</param>
        /// <param name="fNegativeBeta">Specifies the negative beta value.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        public void lrn_computeoutput(int nCount, long hBottomData, long hScaleData, T fNegativeBeta, long hTopData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LRN_COMPUTEOUTPUT, m_param.AsDouble(convertD(fNegativeBeta)), m_param.AsLong(nCount, hBottomData, hScaleData, 0, hTopData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LRN_COMPUTEOUTPUT, m_param.AsFloat(convertF(fNegativeBeta)), m_param.AsLong(nCount, hBottomData, hScaleData, 0, hTopData));
        }


        /// <summary>
        /// Computes the diff used to calculate the LRN cross channel backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hBottomData">Specifies a handle to the Bottom data in GPU memory.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="hScaleData">Specifies a handle to the scale data in GPU memory.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nNum">Specifies the number of input items.</param>
        /// <param name="nChannels">Specifies the number of channels per input item.</param>
        /// <param name="nHeight">Specifies the height of each input item.</param>
        /// <param name="nWidth">Specifies the width of each input item.</param>
        /// <param name="nSize"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="fNegativeBeta">Specifies the negative beta value.</param>
        /// <param name="fCacheRatio"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void lrn_computediff(int nCount, long hBottomData, long hTopData, long hScaleData, long hTopDiff, int nNum, int nChannels, int nHeight, int nWidth, int nSize, T fNegativeBeta, T fCacheRatio, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LRN_COMPUTEDIFF, m_param.AsDouble(convertD(fNegativeBeta), convertD(fCacheRatio)), m_param.AsLong(nCount, hBottomData, hTopData, hScaleData, hTopDiff, nNum, nChannels, nHeight, nWidth, nSize, 0, 0, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LRN_COMPUTEDIFF, m_param.AsFloat(convertF(fNegativeBeta), convertF(fCacheRatio)), m_param.AsLong(nCount, hBottomData, hTopData, hScaleData, hTopDiff, nNum, nChannels, nHeight, nWidth, nSize, 0, 0, hBottomDiff));
        }

        /// <summary>
        /// Perform the Stochastic Gradient Descent (SGD) update
        /// </summary>
        /// <remarks>
        /// See [Stochastic Gradient Descent](https://en.wikipedia.org/wiki/Stochastic_gradient_descent).
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hNetParamsDiff">Specifies a handle to the net params diff in GPU memory.</param>
        /// <param name="hHistoryData">Specifies a handle to the history data in GPU memory.</param>
        /// <param name="fMomentum">Specifies the momentum value.</param>
        /// <param name="fLocalRate">Specifies the local learning rate.</param>
        public void sgd_update(int nCount, long hNetParamsDiff, long hHistoryData, T fMomentum, T fLocalRate)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SGD_UPDATE, m_param.AsDouble(convertD(fMomentum), convertD(fLocalRate)), m_param.AsLong(nCount, hNetParamsDiff, hHistoryData, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SGD_UPDATE, m_param.AsFloat(convertF(fMomentum), convertF(fLocalRate)), m_param.AsLong(nCount, hNetParamsDiff, hHistoryData, 0, 0));
        }

        /// <summary>
        /// Perform the Nesterov update
        /// </summary>
        /// <remarks>
        /// See [Lecture 6c The momentum method](http://www.cs.toronto.edu/~tijmen/csc321/slides/lecture_slides_lec6.pdf) by Hinton, et al., 2012,
        /// and [Nesterov's Accelerated Gradient and Momentum as approximations to Regularised Update Descent](https://arxiv.org/abs/1607.01981) by Botev, et al., 2016
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hNetParamsDiff">Specifies a handle to the net params diff in GPU memory.</param>
        /// <param name="hHistoryData">Specifies a handle to the history data in GPU memory.</param>
        /// <param name="fMomentum">Specifies the momentum value.</param>
        /// <param name="fLocalRate">Specifies the local learning rate.</param>
        public void nesterov_update(int nCount, long hNetParamsDiff, long hHistoryData, T fMomentum, T fLocalRate)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_NESTEROV_UPDATE, m_param.AsDouble(convertD(fMomentum), convertD(fLocalRate)), m_param.AsLong(nCount, hNetParamsDiff, hHistoryData, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_NESTEROV_UPDATE, m_param.AsFloat(convertF(fMomentum), convertF(fLocalRate)), m_param.AsLong(nCount, hNetParamsDiff, hHistoryData, 0, 0));
        }


        /// <summary>
        /// Perform the AdaGrad update
        /// </summary>
        /// <remarks>
        /// See [Adaptive Subgradient Methods for Online Learning and Stochastic Optimization](http://www.jmlr.org/papers/volume12/duchi11a/duchi11a.pdf) by Duchi, et al., 2011
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hNetParamsDiff">Specifies a handle to the net params diff in GPU memory.</param>
        /// <param name="hHistoryData">Specifies a handle to the history data in GPU memory.</param>
        /// <param name="fDelta">Specifies the numerical stability factor.</param>
        /// <param name="fLocalRate">Specifies the local learning rate.</param>
        public void adagrad_update(int nCount, long hNetParamsDiff, long hHistoryData, T fDelta, T fLocalRate)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADAGRAD_UPDATE, m_param.AsDouble(convertD(fDelta), convertD(fLocalRate)), m_param.AsLong(nCount, hNetParamsDiff, hHistoryData, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADAGRAD_UPDATE, m_param.AsFloat(convertF(fDelta), convertF(fLocalRate)), m_param.AsLong(nCount, hNetParamsDiff, hHistoryData, 0, 0));
        }

        /// <summary>
        /// Perform the AdaDelta update
        /// </summary>
        /// <remarks>
        /// See [ADADELTA: An Adaptive Learning Rate Method](https://arxiv.org/abs/1212.5701) by Zeiler, 2012
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hNetParamsDiff">Specifies a handle to the net params diff in GPU memory.</param>
        /// <param name="hHistoryData1">Specifies a handle to history data in GPU memory.</param>
        /// <param name="hHistoryData2">Specifies a handle to history data in GPU memory.</param>
        /// <param name="fMomentum">Specifies the momentum to use.</param>
        /// <param name="fDelta">Specifies the numerical stability factor.</param>
        /// <param name="fLocalRate">Specifies the local learning rate.</param>
        public void adadelta_update(int nCount, long hNetParamsDiff, long hHistoryData1, long hHistoryData2, T fMomentum, T fDelta, T fLocalRate)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADADELTA_UPDATE, m_param.AsDouble(convertD(fMomentum), convertD(fDelta), convertD(fLocalRate)), m_param.AsLong(nCount, hNetParamsDiff, hHistoryData1, hHistoryData2, 0, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADADELTA_UPDATE, m_param.AsFloat(convertF(fMomentum), convertF(fDelta), convertF(fLocalRate)), m_param.AsLong(nCount, hNetParamsDiff, hHistoryData1, hHistoryData2, 0, 0, 0));
        }

        /// <summary>
        /// Perform the Adam update
        /// </summary>
        /// <remarks>
        /// See [Adam: A Method for Stochastic Optimization](https://arxiv.org/abs/1412.6980v9) by Kingma, et al., 2014
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hNetParamsDiff">Specifies a handle to the net params diff in GPU memory.</param>
        /// <param name="hValM"><b><i>First moment.</i></b></param>
        /// <param name="hValV"><b><i>Second moment.</i></b></param>
        /// <param name="fBeta1"><b><i>Momentum for first moment.</i></b></param>
        /// <param name="fBeta2"><b><i>Momentum for second moment.</i></b></param>
        /// <param name="fEpsHat"><b><i>Small value used to avoid Nan.</i></b></param>
        /// <param name="fLearningRate"><b><i>Learning rate.</i></b></param>
        /// <param name="fCorrection">Correction where Local Learning Rate = 'fCorrection' * 'fLearningRate'</param>
        public void adam_update(int nCount, long hNetParamsDiff, long hValM, long hValV, T fBeta1, T fBeta2, T fEpsHat, T fLearningRate, T fCorrection)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADAM_UPDATE, m_param.AsDouble(convertD(fBeta1), convertD(fBeta2), convertD(fEpsHat), convertD(fLearningRate), convertD(fCorrection)), m_param.AsLong(nCount, hNetParamsDiff, hValM, hValV, 0, 0, 0, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADAM_UPDATE, m_param.AsFloat(convertF(fBeta1), convertF(fBeta2), convertF(fEpsHat), convertF(fLearningRate), convertF(fCorrection)), m_param.AsLong(nCount, hNetParamsDiff, hValM, hValV, 0, 0, 0, 0, 0));
        }

        /// <summary>
        /// Perform the AdamW update
        /// </summary>
        /// <remarks>
        /// @see [Decoupled Weight Decay Regularization](https://arxiv.org/abs/1711.05101) by Loshchilov, I. and Hutter, F., 2019.
        /// See [Adam: A Method for Stochastic Optimization](https://arxiv.org/abs/1412.6980v9) by Kingma, et al., 2014
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hNetParamsDiff">Specifies a handle to the net params diff in GPU memory.</param>
        /// <param name="hValM"><b><i>First moment.</i></b></param>
        /// <param name="hValV"><b><i>Second moment.</i></b></param>
        /// <param name="fBeta1"><b><i>Momentum for first moment.</i></b></param>
        /// <param name="fBeta2"><b><i>Momentum for second moment.</i></b></param>
        /// <param name="fEpsHat"><b><i>Small value used to avoid Nan.</i></b></param>
        /// <param name="fLearningRate"><b><i>Learning rate.</i></b></param>
        /// <param name="fDecayRate">Optionally, enable detached weight decay for AdamW optimization using this decay rate (when 0, Adam update is used).</param>
        /// <param name="hNetParamsData">Optionally, specifies the net params weight data (used when fDecayRate != 0)</param>
        /// <param name="nStep">Optionally, specifies the current step - used with AdamW optimization updates.</param>
        public void adamw_update(int nCount, long hNetParamsDiff, long hValM, long hValV, T fBeta1, T fBeta2, T fEpsHat, T fLearningRate, T fDecayRate, long hNetParamsData, int nStep)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADAMW_UPDATE, m_param.AsDouble(convertD(fBeta1), convertD(fBeta2), convertD(fEpsHat), convertD(fLearningRate), convertD(fDecayRate)), m_param.AsLong(nCount, hNetParamsDiff, hValM, hValV, 0, 0, 0, 0, 0, hNetParamsData, nStep));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_ADAMW_UPDATE, m_param.AsFloat(convertF(fBeta1), convertF(fBeta2), convertF(fEpsHat), convertF(fLearningRate), convertF(fDecayRate)), m_param.AsLong(nCount, hNetParamsDiff, hValM, hValV, 0, 0, 0, 0, 0, hNetParamsData, nStep));
        }

        /// <summary>
        /// Perform the RMSProp update
        /// </summary>
        /// <remarks>
        /// See [Lecture 6e	rmsprop: Divide the gradient by a running average of its recent magnitude](http://www.cs.toronto.edu/~tijmen/csc321/slides/lecture_slides_lec6.pdf) by Tieleman and Hinton, 2012,
        /// and [RMSProp and equilibrated adaptive learning rates for non-convex optimization](https://arxiv.org/abs/1502.04390v1) by Dauphin, et al., 2015
        /// </remarks>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hNetParamsDiff">Specifies a handle to the net params diff in GPU memory.</param>
        /// <param name="hHistoryData">Specifies a handle to the history data in GPU memory.</param>
        /// <param name="fRmsDecay">Specifies the decay value used by the Solver.  MeanSquare(t) = 'rms_decay' * MeanSquare(t-1) + (1 - 'rms_decay') * SquareGradient(t).</param>
        /// <param name="fDelta">Specifies the numerical stability factor.</param>
        /// <param name="fLocalRate">Specifies the local learning rate.</param>
        public void rmsprop_update(int nCount, long hNetParamsDiff, long hHistoryData, T fRmsDecay, T fDelta, T fLocalRate)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_RMSPROP_UPDATE, m_param.AsDouble(convertD(fRmsDecay), convertD(fDelta), convertD(fLocalRate)), m_param.AsLong(nCount, hNetParamsDiff, hHistoryData, 0, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_RMSPROP_UPDATE, m_param.AsFloat(convertF(fRmsDecay), convertF(fDelta), convertF(fLocalRate)), m_param.AsLong(nCount, hNetParamsDiff, hHistoryData, 0, 0, 0));
        }

        /// <summary>
        /// Peforms the simple LSTM foward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// See [LSTM with Working Memory](https://arxiv.org/abs/1605.01988) by Pulver, et al., 2016
        /// </remarks>
        /// <param name="t">Specifies the step within the sequence.</param>
        /// <param name="nN">Specifies the batch size.</param>
        /// <param name="nH">Specifies the number of hidden units.</param>
        /// <param name="nI">Specifies the number the input size.</param>
        /// <param name="hWeight_h">Specifies a handle to the GPU memory holding the 'h' weights.</param>
        /// <param name="hWeight_i">Specifies a handle to the GPU memory holding the 'i' weights.</param>
        /// <param name="hClipData">Specifies a handle to the GPU memory holding the clip data.</param>
        /// <param name="nClipOffset">Specifies the clip offset for this step within the sequence.</param>
        /// <param name="hTopData">Specifies a handle to the top data in GPU memory.</param>
        /// <param name="nTopOffset">Specifies an offset into the top data memory.</param>
        /// <param name="hCellData">Specifies a handle to the GPU memory holding the 'c_t' data.</param>
        /// <param name="nCellOffset">Specifies the c_t offset for this step within the sequence.</param>
        /// <param name="hPreGateData">Specifies a handle to the GPU memory holding the pre-gate data.</param>
        /// <param name="nPreGateOffset">Specifies the pre-gate offset for this step within the sequence.</param>
        /// <param name="hGateData">Specifies a handle to the GPU memory holding the gate data.</param>
        /// <param name="nGateOffset">Specifies the gate data offset for this step within the sequence.</param>
        /// <param name="hHT1Data">Specifies a handle to the GPU memory holding the HT1 data.</param>
        /// <param name="nHT1Offset">Specifies the HT1 offset for this step within the sequence.</param>
        /// <param name="hCT1Data">Specifies a handle to the GPU memory holding the CT1 data.</param>
        /// <param name="nCT1Offset">Specifies the CT1 offset for this step within the sequence.</param>
        /// <param name="hHtoGateData">Specifies a handle to the GPU memory holding the H to Gate data.</param>
        /// <param name="hContext">Optionally, specifies the attention context, or 0 when not used.</param>
        /// <param name="hWeight_c">Optionally, specifies the attention context weights, or 0 when not used.</param>
        /// <param name="hCtoGetData">Optionally, specifies the attention context to gate data, or 0 when not used.</param>
        public void lstm_fwd(int t, int nN, int nH, int nI, long hWeight_h, long hWeight_i, long hClipData, int nClipOffset, long hTopData, int nTopOffset, long hCellData, int nCellOffset, long hPreGateData, int nPreGateOffset, long hGateData, int nGateOffset, long hHT1Data, int nHT1Offset, long hCT1Data, int nCT1Offset, long hHtoGateData, long hContext = 0, long hWeight_c = 0, long hCtoGetData = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LSTM_FWD, null, m_param.AsLong(t, nN, nH, nI, hWeight_h, hWeight_i, hClipData, nClipOffset, hTopData, nTopOffset, hCellData, nCellOffset, hPreGateData, nPreGateOffset, hGateData, nGateOffset, hHT1Data, nHT1Offset, hCT1Data, nCT1Offset, hHtoGateData, hContext, hWeight_c, hCtoGetData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LSTM_FWD, null, m_param.AsLong( t, nN, nH, nI, hWeight_h, hWeight_i, hClipData, nClipOffset, hTopData, nTopOffset, hCellData, nCellOffset, hPreGateData, nPreGateOffset, hGateData, nGateOffset, hHT1Data, nHT1Offset, hCT1Data, nCT1Offset, hHtoGateData, hContext, hWeight_c, hCtoGetData));
        }

        /// <summary>
        /// Peforms the simple LSTM backward pass in Cuda.
        /// </summary>
        /// <remarks>
        /// See [LSTM with Working Memory](https://arxiv.org/abs/1605.01988) by Pulver, et al., 2016
        /// </remarks>
        /// <param name="t">Specifies the step within the sequence.</param>
        /// <param name="nN">Specifies the batch size.</param>
        /// <param name="nH">Specifies the number of hidden units.</param>
        /// <param name="nI">Specifies the number the input size.</param>
        /// <param name="dfClippingThreshold"></param>
        /// <param name="hWeight_h">Specifies a handle to the GPU memory holding the 'h' weights.</param>
        /// <param name="hClipData">Specifies a handle to the GPU memory holding the clip data.</param>
        /// <param name="nClipOffset">Specifies the clip offset for this step within the sequence.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="nTopOffset">Specifies an offset into the top diff memory.</param>
        /// <param name="hCellData">Specifies a handle to the GPU memory holding the 'c_t' data.</param>
        /// <param name="hCellDiff">Specifies a handle to the GPU memory holding the 'c_t' gradients.</param>
        /// <param name="nCellOffset">Specifies the c_t offset for this step within the sequence.</param>
        /// <param name="hPreGateDiff">Specifies a handle to the GPU memory holding the pre-gate gradients.</param>
        /// <param name="nPreGateOffset">Specifies the pre-gate offset for this step within the sequence.</param>
        /// <param name="hGateData">Specifies a handle to the GPU memory holding the gate data.</param>
        /// <param name="hGateDiff">Specifies a handle to the GPU memory holding the gate gradients.</param>
        /// <param name="nGateOffset">Specifies the gate data offset for this step within the sequence.</param>
        /// <param name="hCT1Data">Specifies a handle to the GPU memory holding the CT1 data.</param>
        /// <param name="nCT1Offset">Specifies the CT1 offset for this step within the sequence.</param>
        /// <param name="hDHT1Diff">Specifies a handle to the GPU DHT1 gradients.</param>
        /// <param name="nDHT1Offset">Specifies the DHT1 offset for this step within the sequence.</param>
        /// <param name="hDCT1Diff">Specifies a handle to the DCT1 gradients.</param>
        /// <param name="nDCT1Offset">Specifies the DCT1 offset for this step within the sequence.</param>
        /// <param name="hHtoHData">Specifies a handle to the GPU memory holding the H to H data.</param>
        /// <param name="hContextDiff">Optionally, specifies the handle to the GPU memory holding the context diff, or 0 when not used.</param>
        /// <param name="hWeight_c">Optionally, specifies the handle to the GPU memory holding the 'c' weights, or 0 when not used.</param>
        public void lstm_bwd(int t, int nN, int nH, int nI, double dfClippingThreshold, long hWeight_h, long hClipData, int nClipOffset, long hTopDiff, int nTopOffset, long hCellData, long hCellDiff, int nCellOffset, long hPreGateDiff, int nPreGateOffset, long hGateData, long hGateDiff, int nGateOffset, long hCT1Data, int nCT1Offset, long hDHT1Diff, int nDHT1Offset, long hDCT1Diff, int nDCT1Offset, long hHtoHData, long hContextDiff = 0, long hWeight_c = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LSTM_BWD, m_param.AsDouble(dfClippingThreshold), m_param.AsLong(t, nN, nH, nI, 0, hWeight_h, hClipData, nClipOffset, hTopDiff, nTopOffset, hCellData, hCellDiff, nCellOffset, hPreGateDiff, nPreGateOffset, hGateData, hGateDiff, nGateOffset, hCT1Data, nCT1Offset, hDHT1Diff, nDHT1Offset, hDCT1Diff, nDCT1Offset, hHtoHData, hContextDiff, hWeight_c));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LSTM_BWD, m_param.AsFloat((float)dfClippingThreshold), m_param.AsLong( t, nN, nH, nI, 0, hWeight_h, hClipData, nClipOffset, hTopDiff, nTopOffset, hCellData, hCellDiff, nCellOffset, hPreGateDiff, nPreGateOffset, hGateData, hGateDiff, nGateOffset, hCT1Data, nCT1Offset, hDHT1Diff, nDHT1Offset, hDCT1Diff, nDCT1Offset, hHtoHData, hContextDiff, hWeight_c));
        }

        /// <summary>
        /// Peforms the simple LSTM foward pass in Cuda for a given LSTM unit.
        /// </summary>
        /// <remarks>
        /// See [LSTM with Working Memory](https://arxiv.org/abs/1605.01988) by Pulver, et al., 2016
        /// </remarks>
        /// <param name="nCount"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nHiddenDim"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nXCount"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hX"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hX_acts"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hC_prev"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hCont"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hC"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hH"><b><i>NEEDS REVIEW</i></b></param>
        public void lstm_unit_fwd(int nCount, int nHiddenDim, int nXCount, long hX, long hX_acts, long hC_prev, long hCont, long hC, long hH)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LSTM_UNIT_FWD, null, m_param.AsLong(nCount, nHiddenDim, nXCount, hX, hX_acts, hC_prev, hCont, hC, hH));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LSTM_UNIT_FWD, null, m_param.AsLong( nCount, nHiddenDim, nXCount, hX, hX_acts, hC_prev, hCont, hC, hH));
        }

        /// <summary>
        /// Peforms the simple LSTM backward pass in Cuda for a given LSTM unit.
        /// </summary>
        /// <remarks>
        /// See [LSTM with Working Memory](https://arxiv.org/abs/1605.01988) by Pulver, et al., 2016
        /// </remarks>
        /// <param name="nCount"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nHiddenDim"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nXCount"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hC_prev"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hX_acts"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hC"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hH"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hCont"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hC_diff"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hH_diff"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hC_prev_diff"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hX_acts_diff"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hX_diff"><b><i>NEEDS REVIEW</i></b></param>
        public void lstm_unit_bwd(int nCount, int nHiddenDim, int nXCount, long hC_prev, long hX_acts, long hC, long hH, long hCont, long hC_diff, long hH_diff, long hC_prev_diff, long hX_acts_diff, long hX_diff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_LSTM_UNIT_BWD, null, m_param.AsLong(nCount, nHiddenDim, nXCount, hC_prev, hX_acts, hC, hH, hCont, hC_diff, hH_diff, hC_prev_diff, hX_acts_diff, hX_diff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_LSTM_UNIT_BWD, null, m_param.AsLong(nCount, nHiddenDim, nXCount, hC_prev, hX_acts, hC, hH, hCont, hC_diff, hH_diff, hC_prev_diff, hX_acts_diff, hX_diff));
        }

        /// <summary>
        /// Performs a coefficient sum foward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="nDim"><b><i>Specifies the dimension of the data where the data is sized 'num' x 'dim'.</i></b></param>
        /// <param name="nNumOffset">Specifies the offset applied to the coefficent indexing.</param>
        /// <param name="dfCoeff">Specifies a primary coefficient value applied to each input before summing.</param>
        /// <param name="hCoeffData">Optionally specifies a handle to coefficient data that is applied to the primary coefficient.</param>
        /// <param name="hBottom">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTop">Specifies a handle to the top data in GPU memory.</param>
        public void coeff_sum_fwd(int nCount, int nDim, int nNumOffset, double dfCoeff, long hCoeffData, long hBottom, long hTop)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COEFF_SUM_FWD, m_param.AsDouble(dfCoeff), m_param.AsLong(nCount, nDim, nNumOffset, 0, hCoeffData, hBottom, hTop));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COEFF_SUM_FWD, m_param.AsFloat((float)dfCoeff), m_param.AsLong(nCount, nDim, nNumOffset, 0, hCoeffData, hBottom, hTop));
        }


        /// <summary>
        /// Performs a coefficient sum backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="nDim"><b><i>Specifies the dimension of the data where the data is sized 'num' x 'dim'.</i></b></param>
        /// <param name="nNumOffset">Specifies the offset applied to the coefficent indexing.</param>
        /// <param name="dfCoeff">Specifies a primary coefficient value applied to each input before summing.</param>
        /// <param name="hCoeffData">Optionally specifies a handle to coefficient data that is applied to the primary coefficient.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void coeff_sum_bwd(int nCount, int nDim, int nNumOffset, double dfCoeff, long hCoeffData, long hTopDiff, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COEFF_SUM_BWD, m_param.AsDouble(dfCoeff), m_param.AsLong(nCount, nDim, nNumOffset, 0, hCoeffData, hTopDiff, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COEFF_SUM_BWD, m_param.AsFloat((float)dfCoeff), m_param.AsLong(nCount, nDim, nNumOffset, 0, hCoeffData, hTopDiff, hBottomDiff));
        }

        /// <summary>
        /// Performs a coefficient sub foward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="nDim"><b><i>Specifies the dimension of the data where the data is sized 'num' x 'dim'.</i></b></param>
        /// <param name="nNumOffset">Specifies the offset applied to the coefficent indexing.</param>
        /// <param name="dfCoeff">Specifies a primary coefficient value applied to each input before summing.</param>
        /// <param name="hCoeffData">Optionally specifies a handle to coefficient data that is applied to the primary coefficient.</param>
        /// <param name="hBottom">Specifies a handle to the bottom data in GPU memory.</param>
        /// <param name="hTop">Specifies a handle to the top data in GPU memory.</param>
        public void coeff_sub_fwd(int nCount, int nDim, int nNumOffset, double dfCoeff, long hCoeffData, long hBottom, long hTop)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COEFF_SUB_FWD, m_param.AsDouble(dfCoeff), m_param.AsLong(nCount, nDim, nNumOffset, 0, hCoeffData, hBottom, hTop));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COEFF_SUB_FWD, m_param.AsFloat((float)dfCoeff), m_param.AsLong(nCount, nDim, nNumOffset, 0, hCoeffData, hBottom, hTop));
        }


        /// <summary>
        /// Performs a coefficient sub backward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="nDim"><b><i>Specifies the dimension of the data where the data is sized 'num' x 'dim'.</i></b></param>
        /// <param name="nNumOffset">Specifies the offset applied to the coefficent indexing.</param>
        /// <param name="dfCoeff">Specifies a primary coefficient value applied to each input before summing.</param>
        /// <param name="hCoeffData">Optionally specifies a handle to coefficient data that is applied to the primary coefficient.</param>
        /// <param name="hTopDiff">Specifies a handle to the top diff in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void coeff_sub_bwd(int nCount, int nDim, int nNumOffset, double dfCoeff, long hCoeffData, long hTopDiff, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_COEFF_SUB_BWD, m_param.AsDouble(dfCoeff), m_param.AsLong(nCount, nDim, nNumOffset, 0, hCoeffData, hTopDiff, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_COEFF_SUB_BWD, m_param.AsFloat((float)dfCoeff), m_param.AsLong(nCount, nDim, nNumOffset, 0, hCoeffData, hTopDiff, hBottomDiff));
        }


        /// <summary>
        /// Performs a sigmoid cross entropy forward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hInput">Specifies a handle to the input data in GPU memory.</param>
        /// <param name="hTarget">Specifies a handle to the target data in GPU memory.</param>
        /// <param name="hLoss">Specifies a handle to the loss data in GPU memory.</param>
        /// <param name="bHasIgnoreLabel">Specifies whether or not an ignore label is used.</param>
        /// <param name="nIgnoreLabel">Specifies the ignore label which is used when <i>bHasIgnoreLabel</i> is <code>true</code></param>
        /// <param name="hCountData">Specifies a handle to the count data in GPU memory.</param>
        public void sigmoid_cross_entropy_fwd(int nCount, long hInput, long hTarget, long hLoss, bool bHasIgnoreLabel, int nIgnoreLabel, long hCountData)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SIGMOID_CROSS_ENTROPY_FWD, null, m_param.AsLong(nCount, hInput, hTarget, hLoss, (bHasIgnoreLabel) ? 1 : 0, nIgnoreLabel, hCountData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SIGMOID_CROSS_ENTROPY_FWD, null, m_param.AsLong(nCount, hInput, hTarget, hLoss, (bHasIgnoreLabel) ? 1 : 0, nIgnoreLabel, hCountData));
        }

        /// <summary>
        /// Performs a sigmoid cross entropy backward pass in Cuda when an ignore label is specified.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="nIgnoreLabel">Specifies the label to ignore.</param>
        /// <param name="hTarget">Specifies a handle to the target data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void sigmoid_cross_entropy_bwd(int nCount, int nIgnoreLabel, long hTarget, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SIGMOID_CROSS_ENTROPY_BWD, null, m_param.AsLong(nCount, nIgnoreLabel, hTarget, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SIGMOID_CROSS_ENTROPY_BWD, null, m_param.AsLong(nCount, nIgnoreLabel, hTarget, hBottomDiff));
        }

        /// <summary>
        /// Performs a softmax cross entropy forward pass in Cuda.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="hProbData">Specifies a handle to the probability data in GPU memory.</param>
        /// <param name="hLabel">Specifies a handle to the label data in GPU memory.</param>
        /// <param name="hLossDiff">Specifies a handle to the loss diff in GPU memory that is filled with 1's at each 'active' location where loss data is placed.</param>
        /// <param name="hLossData">Specifies a handle to the loss data in GPU memory.</param>
        /// <param name="nOuterNum"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nDim"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="nInnerNum"><b><i>NEEDS REVIEW</i></b></param>
        /// <param name="hCounts">Specifies a handle to the counts in GPU memory.</param>
        /// <param name="nIgnoreLabel">Optionally, specifies a label to ignore.</param>
        /// <remarks>
        /// This forward pass is a helper to perform a part of the NLLLoss portion of the SoftmaxCrossEntropyLoss.
        /// </remarks>
        public void softmax_cross_entropy_fwd(int nCount, long hProbData, long hLabel, long hLossDiff, long hLossData, int nOuterNum, int nDim, int nInnerNum, long hCounts, int? nIgnoreLabel)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<long> rg = new List<long>() { nCount, hProbData, hLabel, hLossDiff, hLossData, nOuterNum, nDim, nInnerNum, hCounts };
                
                if (nIgnoreLabel.HasValue)
                    rg.Add(nIgnoreLabel.Value);

                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SOFTMAX_CROSS_ENTROPY_FWD, null, rg.ToArray());
            }
            else
            {
                List<long> rg = new List<long>() { nCount, hProbData, hLabel, hLossDiff, hLossData, nOuterNum, nDim, nInnerNum, hCounts };
                
                if (nIgnoreLabel.HasValue)
                    rg.Add(nIgnoreLabel.Value);

                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SOFTMAX_CROSS_ENTROPY_FWD, null, rg.ToArray());
            }
        }

        /// <summary>
        /// Performs a softmax cross entropy backward pass in Cuda when an ignore label is specified.
        /// </summary>
        /// <param name="nCount">Specifies the number of items.</param>
        /// <param name="nIgnoreLabel">Specifies the label to ignore.</param>
        /// <param name="hTarget">Specifies a handle to the target data in GPU memory.</param>
        /// <param name="hBottomDiff">Specifies a handle to the bottom diff in GPU memory.</param>
        public void softmax_cross_entropy_bwd(int nCount, int nIgnoreLabel, long hTarget, long hBottomDiff)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_SOFTMAX_CROSS_ENTROPY_BWD, null, m_param.AsLong(nCount, nIgnoreLabel, hTarget, hBottomDiff));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_SOFTMAX_CROSS_ENTROPY_BWD, null, m_param.AsLong(nCount, nIgnoreLabel, hTarget, hBottomDiff));
        }

#pragma warning disable 1591

        /// <summary>
        /// The debug function is uses only during debugging the debug version of the low-level DLL.
        /// </summary>
        public void debug()
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDouble((int)m_hKernel, (int)CUDAFN.CUDA_DEBUG, null);
            else
                m_cuda.RunFloat((int)m_hKernel, (int)CUDAFN.CUDA_DEBUG, null);
        }

        public void matrix_set_diagonal(int nCount, int nRows, double dfVal, long hData) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_SET_DIAGONAL, m_param.AsDouble(dfVal), m_param.AsLong(nCount, nRows, 0, hData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_SET_DIAGONAL, m_param.AsFloat((float)dfVal), m_param.AsLong(nCount, nRows, 0, hData));
        }

        public void matrix_set_diagonal(int nCount, int nRows, long hDiagonal, double dfScaleA, double dfScaleB, long hData) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_SET_DIAGONAL2, m_param.AsDouble(dfScaleA, dfScaleB), m_param.AsLong(nCount, nRows, hDiagonal, 0, 0, hData));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_SET_DIAGONAL2, m_param.AsFloat((float)dfScaleA, (float)dfScaleB), m_param.AsLong(nCount, nRows, hDiagonal, 0, 0, hData));
        }

        public void matrix_add_vector(ORIENTATION orientation, int nWidth, int nHeight, double dfScale, long hA, long hB, long hY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_ADD_VECTOR, m_param.AsDouble(dfScale), m_param.AsLong((int)orientation, nWidth, nHeight, 0, hA, hB, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_ADD_VECTOR, m_param.AsFloat((float)dfScale), m_param.AsLong((int)orientation, nWidth, nHeight, 0, hA, hB, hY));
        }

        public void matrix_transpose_operation(TRANSPOSE_OPERATION op, int nWidth, int nHeight, long hA, long hB, long hY, double dfScaleA = 1.0, double dfScaleB = 1.0) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_TRANSPOSE_OPERATION, m_param.AsDouble(dfScaleA, dfScaleB), m_param.AsLong((int)op, nWidth, nHeight, hA, hB, hY, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_TRANSPOSE_OPERATION, m_param.AsFloat((float)dfScaleA, (float)dfScaleB), m_param.AsLong((int)op, nWidth, nHeight, hA, hB, hY, 0, 0));
        }

        public void matrix_transpose_add(int nWidth, int nHeight, double dfScaleA, double dfScaleB, long hA, long hB, long hY) /** @private */
        {
            matrix_transpose_operation(TRANSPOSE_OPERATION.ADD, nWidth, nHeight, hA, hB, hY, dfScaleA, dfScaleB);
        }

        public void matrix_transpose_mul(int nWidth, int nHeight, long hA, long hB, long hY) /** @private */
        {
            matrix_transpose_operation(TRANSPOSE_OPERATION.MUL, nWidth, nHeight, hA, hB, hY);
        }

        public void matrix_transpose_div(int nWidth, int nHeight, long hA, long hB, long hY) /** @private */
        {
            matrix_transpose_operation(TRANSPOSE_OPERATION.DIV, nWidth, nHeight, hA, hB, hY);
        }

        public void matrix_aggregate_cols(AGGREGATIONS op, int nWidth, int nHeight, long hA, long hY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_AGGREGATE_COLS, null, m_param.AsLong((int)op, nWidth, nHeight, hA, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_AGGREGATE_COLS, null, m_param.AsLong((int)op, nWidth, nHeight, hA, hY));
        }

        public void matrix_aggregate_rows(AGGREGATIONS op, int nWidth, int nHeight, long hA, long hOnes, long hY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_AGGREGATE_ROWS, null, m_param.AsLong((int)op, nWidth, nHeight, hA, hOnes, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_AGGREGATE_ROWS, null, m_param.AsLong((int)op, nWidth, nHeight, hA, hOnes, hY));
        }

        public void matrix_transpose(int nWidth, int nHeight, long hA, long hY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_TRANSPOSE, null, m_param.AsLong(nWidth, nHeight, hA, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_TRANSPOSE, null, m_param.AsLong(nWidth, nHeight, hA, hY));
        }

        /// <summary>
        /// Mean center the data by columns, where each column is summed and then subtracted from each column value.
        /// </summary>
        /// <param name="nWidth">Number of columns in the matrix (dimension D)</param>
        /// <param name="nHeight">Number of rows in the matrix (dimension N)</param>
        /// <param name="hA">Input data matrix - N x D matrix (N rows, D columns)</param>
        /// <param name="hB">Column sums vector - D x 1 vector containing the sum of each column.</param>
        /// <param name="hY">Output data matrix - N x D matrix (N rows, D columns) containing mean centering of the input data matrix.</param>
        /// <param name="bNormalize">When true, each data item is divided by N to normalize each row item by column.</param>
        public void matrix_meancenter_by_column(int nWidth, int nHeight, long hA, long hB, long hY, bool bNormalize = false)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_MEANCENTER_BY_COL, null, m_param.AsLong(nWidth, nHeight, hA, hB, hY, (bNormalize) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_MEANCENTER_BY_COL, null, m_param.AsLong(nWidth, nHeight, hA, hB, hY, (bNormalize) ? 1 : 0));
        }

        public void matrix_euclidean_distance(long hX, long hY, long hOut, int n, int d, int nStart, int nEnd) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_EUCLIDEAN_DIST, null, m_param.AsLong(hX, hY, hOut, n, d, nStart, nEnd));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_EUCLIDEAN_DIST, null, m_param.AsLong(hX, hY, hOut, n, d, nStart, nEnd));
        }

        public void matrix_dot(int m, int n, int k, long hA, long hB, long hC) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_DOT, null, m_param.AsLong(m, n, k, hA, hB, hC));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_DOT, null, m_param.AsLong(m, n, k, hA, hB, hC));
        }

        public void matrix_mean_rows(int nWidth, int nHeight, long hA, long hOnes, double dfAlpha, long hY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_MEAN, m_param.AsDouble(dfAlpha), m_param.AsLong(nWidth, nHeight, hA, hOnes, 0, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_MEAN, m_param.AsFloat((float)dfAlpha), m_param.AsLong(nWidth, nHeight, hA, hOnes, 0, hY));
        }

        public void matrix_stdev_rows(int nWidth, int nHeight, long hA, long hOnes, long hMean, long hWork, long hY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_STDEV, null, m_param.AsLong(nWidth, nHeight, hA, hOnes, hMean, hWork, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_STDEV, null, m_param.AsLong(nWidth, nHeight, hA, hOnes, hMean, hWork, hY));
        }

        public void matrix_correlations(int nWidth, int nHeight, long hA, long hOnes, long hMean, long hStdev, long hWork, long hY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_CORRELATIONS, null, m_param.AsLong(nWidth, nHeight, hA, hOnes, hMean, hStdev, hWork, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_MTX_CORRELATIONS, null, m_param.AsLong(nWidth, nHeight, hA, hOnes, hMean, hStdev, hWork, hY));
        }

#pragma warning restore 1591

        #endregion

        #region T-SNE Methods

#pragma warning disable 1591

        public void tsne_update(int n, double dfMomentum, double dfLearningRate, long hdY, long huY, long hGains, long hY, double fGainFactor1 = 0.2, double fGainFactor2 = 0.8) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_UPDATE, m_param.AsDouble(dfMomentum, dfLearningRate, fGainFactor1, fGainFactor2), m_param.AsLong(n, 0, 0, hdY, huY, hGains, hY, 0, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_UPDATE, m_param.AsFloat((float)dfMomentum, (float)dfLearningRate, (float)fGainFactor1, (float)fGainFactor2), m_param.AsLong(n, 0, 0, hdY, huY, hGains, hY, 0, 0));
        }

        public void tsne_update_grad(int n, long hPosF, long hNegF, double dfSumQ, long hdC) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_UPDATE_GRAD, m_param.AsDouble(dfSumQ), m_param.AsLong(n, hPosF, hNegF, 0, hdC));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_UPDATE_GRAD, m_param.AsFloat((float)dfSumQ), m_param.AsLong(n, hPosF, hNegF, 0, hdC));
        }

        public void tsne_compute_exact_error(int n, long hP, long hQ, long hY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_EXACT_ERROR, null, m_param.AsLong(n, hP, hQ, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_EXACT_ERROR, null, m_param.AsLong(n, hP, hQ, hY));
        }

        public void tsne_compute_squared_euclidean_distance(int n, int d, long hWork, long hX, long hDD_on_host) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_SQUARED_EUCLIDEAN_DISTANCE, null, m_param.AsLong(n, d, hWork, hX, hDD_on_host));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_SQUARED_EUCLIDEAN_DISTANCE, null, m_param.AsLong(n, d, hWork, hX, hDD_on_host));
        }

        public double tsne_compute_q_matrix(int n, long hDD_on_host, long hQ, bool bQisHostMem) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_Q_MATRIX, null, m_param.AsLong(n, hDD_on_host, hQ, (bQisHostMem) ? 1 : 0));
                return rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_Q_MATRIX, null, m_param.AsLong(n, hDD_on_host, hQ, (bQisHostMem) ? 1 : 0));
                return rg[0];
            }
        }

        public void tsne_compute_exact_gradient(int n, int d, long hY, long hP, long hQ, bool bQonHost, long hdC, double dfSumQ) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_EXACT_GRADIENT, m_param.AsDouble(dfSumQ), m_param.AsLong(n, d, hY, hP, hQ, (bQonHost) ? 1 : 0, hdC, 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_EXACT_GRADIENT, m_param.AsFloat((float)dfSumQ), m_param.AsLong(n, d, hY, hP, hQ, (bQonHost) ? 1 : 0, hdC, 0));
        }

        public long tsne_symmetrize_matrix(int n, long hRowP, long hColP, long hValP) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_SYMMETRIZE_MATRIX, null, m_param.AsLong(n, hRowP, hColP, hValP));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_SYMMETRIZE_MATRIX, null, m_param.AsLong(n, hRowP, hColP, hValP));
                return (long)rg[0];
            }
        }

        public void tsne_compute_knn_bounds(int n, long hData, double dfCirclePct, out double dfMinX, out double dfMinY, out double dfMaxX, out double dfMaxY) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_KNN_BOUNDS, m_param.AsDouble(dfCirclePct), m_param.AsLong(n, hData, 0));
                dfMinX = rg[0];
                dfMinY = rg[1];
                dfMaxX = rg[2];
                dfMaxY = rg[3];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_KNN_BOUNDS, m_param.AsFloat((float)dfCirclePct), m_param.AsLong(n, hData, 0));
                dfMinX = rg[0];
                dfMinY = rg[1];
                dfMaxX = rg[2];
                dfMaxY = rg[3];
            }
        }

        public long CreateTsneGaussianPerplexity(int n, int d, int k, long hX, long hCurP, long hValP, long hRowPonHost, long hColPonHost, double fPerplexity) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_CREATE_GAUSSIAN_PERPLEXITY, m_param.AsDouble(fPerplexity), m_param.AsLong(n, d, k, hX, hCurP, hValP, hRowPonHost, hColPonHost, 0));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_CREATE_GAUSSIAN_PERPLEXITY, m_param.AsFloat((float)fPerplexity), m_param.AsLong(n, d, k, hX, hCurP, hValP, hRowPonHost, hColPonHost, 0));
                return (long)rg[0];
            }
        }

        public bool FindTsneGaussianPerplexity(long hTsnePerplexity, out int nCurrentIteration, out int nMaxIteration) /** @private */
        {
            bool bDone = false;

            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_FIND_GAUSSIAN_PERPLEXITY, null, m_param.AsLong(hTsnePerplexity));
                bDone = (rg[0] == 1.0) ? true : false;
                nCurrentIteration = (int)rg[1];
                nMaxIteration = (int)rg[2];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_FIND_GAUSSIAN_PERPLEXITY, null, m_param.AsLong(hTsnePerplexity));
                bDone = (rg[0] == 1.0) ? true : false;
                nCurrentIteration = (int)rg[1];
                nMaxIteration = (int)rg[2];
            }

            return bDone;
        }

        public void FreeTsneGaussianPerplexity(long hTsnePerplexity) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_FREE_GAUSSIAN_PERPLEXITY, null, m_param.AsLong(hTsnePerplexity));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_FREE_GAUSSIAN_PERPLEXITY, null, m_param.AsLong(hTsnePerplexity));
        }

        public long CreateTsne(int n, int d, long hY, long hValP, long hRowP, long hColP, long hdC, double fTheta) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_CREATE, m_param.AsDouble(fTheta), m_param.AsLong(n, d, hY, hValP, hRowP, hColP, hdC, 0));
                return (long)rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_CREATE, m_param.AsFloat((float)fTheta), m_param.AsLong(n, d, hY, hValP, hRowP, hColP, hdC, 0));
                return (long)rg[0];
            }
        }

        public void ComputeTsneGradient(long hTsne, bool bValPUpdated) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_GRADIENT1, null, m_param.AsLong(hTsne, (bValPUpdated) ? 1 : 0));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_GRADIENT1, null, m_param.AsLong(hTsne, (bValPUpdated) ? 1 : 0));
        }

        public double EvaluateTsneError(long hTsne) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
            {
                double[] rg = m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_ERROR1, null, m_param.AsLong(hTsne));
                return rg[0];
            }
            else
            {
                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_COMPUTE_ERROR1, null, m_param.AsLong(hTsne));
                return rg[0];
            }
        }

        public void FreeTsne(long hTsne) /** @private */
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_FREE, null, m_param.AsLong(hTsne));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_TSNE_FREE, null, m_param.AsLong(hTsne));
        }

#pragma warning restore 1591

        #endregion

        #region Image Processing And Misc

        /// <summary>
        /// The gaussian_blur runs a Gaussian blurring operation over each channel of the data using the sigma.
        /// </summary>
        /// <remarks>
        /// The gaussian blur operation runs a 3x3 patch, initialized with the gaussian distribution using the formula
        /// @f$
        /// G(x, y) = \frac{1}{{2\pi\sigma^2 }}e^{{{ - \left( {x^2 - y^2 } \right) } \mathord{\left/ {\vphantom {{ - \left( {x^2 - y^2 } \right) } {2\sigma ^2 }}} \right. \kern-\nulldelimiterspace} {2\sigma ^2 }}}
        /// @f$
        /// @see [Gaussian Blur](https://en.wikipedia.org/wiki/Gaussian_blur) on Wikipedia for more information.
        /// </remarks>
        /// <param name="n">Specifies the number of items in the memory of 'X'.</param>
        /// <param name="nChannels">Specifies the number of channels (i.e. 3 for RGB, 1 for B/W).</param>
        /// <param name="nHeight">Specifies the height of each item.</param>
        /// <param name="nWidth">Specifies the width of each item.</param>
        /// <param name="dfSigma">Specifies the sigma used in the gaussian blur.</param>
        /// <param name="hX">Specifies a handle to GPU memory containing the source data to blur.</param>
        /// <param name="hY">Specifies a handle to GPU memory where the blurred information is placed.</param>
        public void gaussian_blur(int n, int nChannels, int nHeight, int nWidth, double dfSigma, long hX, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_GUASSIAN_BLUR, m_param.AsDouble(dfSigma), m_param.AsLong(n, nChannels, nHeight, nWidth, 0, hX, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_GUASSIAN_BLUR, m_param.AsFloat((float)dfSigma), m_param.AsLong(n, nChannels, nHeight, nWidth, 0, hX, hY));
        }

        /// <summary>
        /// The hamming_distance calculates the Hamming Distance between X and Y both of length <i>n</i>.
        /// </summary>
        /// <remarks>
        /// To calculate the hamming distance first, X and Y are bitified where each element is converted to 1 if > than the threshold, or 0 otherwise.
        /// Next, the bitified versions of X and Y are subtracted from one another, and the Asum of the result is returned, which is the
        /// number of bits that are different, thus the Hamming distance.
        /// </remarks>
        /// <param name="n">Specifies the number of elements to compare in both X and Y.</param>
        /// <param name="dfThreshold">Specifies the threshold used to 'bitify' both X and Y</param>
        /// <param name="hA">Specifies the handle to the GPU memory containing the first vector to compare.</param>
        /// <param name="hB">Specifies the handle to the GPU memory containing the second vector to compare.</param>
        /// <param name="hY">Specifies the handle to the GPU memory where the hamming difference (bitified A - bitified B) is placed.</param>
        /// <param name="nOffA">Optionally, specifies an offset into the GPU memory of A, the default is 0.</param>
        /// <param name="nOffB">Optionally, specifies an offset into the GPU memory of B, the default is 0.</param>
        /// <param name="nOffY">Optionally, specifies an offset into the GPU memory of Y, the default is 0.</param>
        /// <returns>The hamming distance is returned.</returns>
        public double hamming_distance(int n, double dfThreshold, long hA, long hB, long hY, int nOffA = 0, int nOffB = 0, int nOffY = 0)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_HAMMING_DIFF, m_param.AsDouble(dfThreshold), m_param.AsLong(n, 0, hA, hB, hY, nOffA, nOffB, nOffY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_HAMMING_DIFF, m_param.AsFloat((float)dfThreshold), m_param.AsLong(n, 0, hA, hB, hY, nOffA, nOffB, nOffY));

            return asum_double(n, hY);
        }

        /// <summary>
        /// Calculates the discrete Fourier Transform (DFT) coefficients across the frequencies 1...n/2 (Nyquest Limit)
        /// for the array of values in host memory referred to by hA.  Return values are placed in the host memory
        /// referenced by hY.
        /// </summary>
        /// <param name="n">Specifies the number of items.</param>
        /// <param name="hX">Specifies a handle to the host memory holding the input values.</param>
        /// <param name="m">Specifies the number of items in hY, must = n/2 (Nyquest Limit)</param>
        /// <param name="hY">Specifies a handle to the host memory holding the n/2 output values (Nyquest Limit)</param>
        /// <remarks>
        /// @see [Implement the Spectrogram from scratch in python](https://fairyonice.github.io/implement-the-spectrogram-from-scratch-in-python.html) by Yumi, Yumi's Blog, 2018
        /// </remarks>
        public void calc_dft_coefficients(int n, long hX, int m, long hY)
        {
            if (m_dt == DataType.DOUBLE)
                m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CALC_DFT, null, m_param.AsLong(n, hX, m, hY));
            else
                m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CALC_DFT, null, m_param.AsLong(n, hX, m, hY));
        }

        /// <summary>
        /// The calculate_batch_distances method calculates a set of distances based on the DistanceMethod specified.
        /// </summary>
        /// <param name="distMethod">Specifies the DistanceMethod to use (i.e. HAMMING or EUCLIDEAN).</param>
        /// <param name="dfThreshold">Specifies the threshold used when binarifying the values for the HAMMING distance.  This parameter is ignored when calculating the EUCLIDEAN distance.</param>
        /// <param name="nItemDim">Specifies the dimension of a single item.</param>
        /// <param name="hSrc">Specifies the GPU memory containing the source items.</param>
        /// <param name="hTargets">Specifies the GPU memory containing the target items that are compared against the source items.</param>
        /// <param name="hWork">Specifies the GPU memory containing the work memory - this must be the same size as the maximum size of the src or targets.</param>
        /// <param name="rgOffsets">Specifies the array of offset pairs where the first offset is into the source and the second is into the target.</param>
        /// <returns>The array distances corresponding to each offset pair is returned.</returns>
        public double[] calculate_batch_distances(DistanceMethod distMethod, double dfThreshold, int nItemDim, long hSrc, long hTargets, long hWork, int[,] rgOffsets)
        {
            if (m_dt == DataType.DOUBLE)
            {
                List<long> rgArg = new List<long> { (int)distMethod, 0, nItemDim, hSrc, hTargets, hWork };
                int nDim0 = rgOffsets.GetLength(0);
                int nDim1 = rgOffsets.GetLength(1);

                rgArg.Add(nDim0);
                rgArg.Add(nDim1);

                for (int i = 0; i < nDim0; i++)
                {
                    for (int j = 0; j < nDim1; j++)
                    {
                        rgArg.Add(rgOffsets[i, j]);
                    }
                }

                return m_cuda.RunDoubleEx2((int)m_hKernel, (int)CUDAFN.CUDA_CALC_BATCH_DIST, m_param.AsDouble(dfThreshold), rgArg.ToArray());
            }
            else
            {
                List<long> rgArg = new List<long> { (int)distMethod, 0, nItemDim, hSrc, hTargets, hWork };
                int nDim0 = rgOffsets.GetLength(0);
                int nDim1 = rgOffsets.GetLength(1);

                rgArg.Add(nDim0);
                rgArg.Add(nDim1);

                for (int i = 0; i < nDim0; i++)
                {
                    for (int j = 0; j < nDim1; j++)
                    {
                        rgArg.Add(rgOffsets[i, j]);
                    }
                }

                float[] rg = m_cuda.RunFloatEx2((int)m_hKernel, (int)CUDAFN.CUDA_CALC_BATCH_DIST, m_param.AsFloat((float)dfThreshold), rgArg.ToArray());
                double[] rgD = new double[rg.Length];

                for (int i = 0; i < rg.Length; i++)
                {
                    rgD[i] = rg[i];
                }

                return rgD;
            }
        }

        #endregion

        //---------------------------------------------------------------------
        //  Conversion Methods
        //---------------------------------------------------------------------
        #region Convertion Methods

        private T[] convert(double[] rg)
        {
            if (rg == null)
                return null;

            if (typeof(T) == typeof(double))
                return (T[])Convert.ChangeType(rg, typeof(T[]));

            T[] rgt = new T[rg.Length];
            Array.Copy(Array.ConvertAll(rg, p => Convert.ToSingle(p)), rgt, rg.Length);

            return rgt;
        }

        private T[] convert(float[] rg)
        {
            if (rg == null)
                return null;

            if (typeof(T) == typeof(float))
                return (T[])Convert.ChangeType(rg, typeof(T[]));

            T[] rgt = new T[rg.Length];
            Array.Copy(rg, rgt, rg.Length);

            return rgt;
        }

        private float convertF1(T f)
        {
            return (float)Convert.ChangeType(f, typeof(float));
        }

        private T convertF1(float f)
        {
            return (T)Convert.ChangeType(f, typeof(T));
        }

        private float[] convertF(T[] rg, int nCount = -1)
        {
            if (rg == null)
                return null;

            if (nCount == -1)
                nCount = rg.Length;

            if (typeof(T) == typeof(float))
                return (float[])Convert.ChangeType(rg, typeof(float[]));

            float[] rgf = new float[rg.Length];
            Array.Copy(Array.ConvertAll(rg, p => Convert.ToSingle(p)), rgf, rg.Length);

            return rgf;
        }

        private float[] convertF(T[] rg, float[] rgDst, int nOffset = 0, int nCount = -1)
        {
            if (rg == null)
                return null;

            if (nCount == -1)
                nCount = rg.Length;

            if (typeof(T) == typeof(float))
            {
                float[] rgConv = (float[])Convert.ChangeType(rg, typeof(float[]));
                Array.Copy(rgConv, 0, rgDst, nOffset, nCount);
            }
            else
            {
                Array.Copy(rg, 0, rgDst, nOffset, nCount);
            }

            return rgDst;
        }

        private double convertD1(T df)
        {
            return (double)Convert.ChangeType(df, typeof(double));
        }

        private T convertD1(double df)
        {
            return (T)Convert.ChangeType(df, typeof(T));
        }

        private double[] convertD(T[] rg, int nCount = -1)
        {
            if (rg == null)
                return null;

            if (nCount == -1)
                nCount = rg.Length;

            if (typeof(T) == typeof(double))
                return (double[])Convert.ChangeType(rg, typeof(double[]));

            double[] rgdf = new double[rg.Length];
            Array.Copy(rg, rgdf, rg.Length);

            return rgdf;
        }

        private double[] convertD(T[] rg, double[] rgDst, int nOffset = 0, int nCount = -1)
        {
            if (rg == null)
                return null;

            if (nCount == -1)
                nCount = rg.Length;

            if (typeof(T) == typeof(double))
            {
                double[] rgConv = (double[])Convert.ChangeType(rg, typeof(double[]));
                Array.Copy(rgConv, 0, rgDst, nOffset, nCount);
            }
            else
            {
                Array.Copy(rg, 0, rgDst, nOffset, nCount);
            }

            return rgDst;
        }

        #endregion

        #region Debugging Methods
        
        /// <summary>
        /// Report the memory use on the current GPU managed by the CudaDnn object.
        /// </summary>
        /// <param name="log">Specifies the output log.</param>
        /// <param name="strLocation">Specifies the location of the memory test.</param>
        public void ReportMemory(Log log, string strLocation)
        {
            double dfFree;
            double dfUsed;
            bool bCudaCallUsed;
            int nGpuID = GetDeviceID();
            double dfMem = GetDeviceMemory(out dfFree, out dfUsed, out bCudaCallUsed);
            log.WriteLine(strLocation + " Memory (GPU " + nGpuID.ToString() + "): " + dfMem.ToString("N2") + " GB total; " + dfFree.ToString("N2") + " GB free; " + dfUsed.ToString("N2") + " GB used.", true);
        }

        #endregion
    }

#pragma warning disable 1591

    class Params /** @private */
    {
        public Params()
        {
        }
        
        public long[] AsLong(params long[] rg) /** @private */
        {
            return rg;
        }

        public double[] AsDouble(params double[] rg) /** @private */
        {
            return rg;
        }

        public float[] AsFloat(params float[] rg) /** @private */
        {
            return rg;
        }
    }

#pragma warning restore 1591
}
