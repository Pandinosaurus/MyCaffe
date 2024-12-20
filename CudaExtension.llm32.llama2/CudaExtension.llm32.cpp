// MyCaffe.test.extension.cpp : This is a Test DLL used along with the TestExtension test 
// to demonstrate how to implement a custom extension to the CudaDnnDLL.  Each custom 
// extension has access not only to the parent, but also the memory and other CUDA
// objects managed by the parent.
//

#include "stdafx.h"
#include "FunctionIDs.h"
#include "Files/LLM.h"
#include <map>
#include <mutex>

// Used to call back to the parent CudaDnnDLL
LPFNINTERNAL_INVOKEFLOAT m_pfnf = NULL;
LPFNINTERNAL_INVOKEDOUBLE m_pfndf = NULL;
LPFNINTERNAL_ALLOCHOSTFLOAT m_pfnfAllocHost = NULL;
LPFNINTERNAL_ALLOCHOSTDOUBLE m_pfndfAllocHost = NULL;
LONG m_lKernel;

const int LLM_CUDA_EXTENSION_CREATE = 1;
const int LLM_CUDA_EXTENSION_DESTROY = 2;
const int LLM_CUDA_EXTENSION_LOAD = 3;
const int LLM_CUDA_EXTENSION_QUERYSTATUS = 4;
const int LLM_CUDA_EXTENSION_GENERATE = 5;
const int LLM_CUDA_EXTENSION_QUERYRESPONSE = 6;
const int LLM_CUDA_EXTENSION_ABORT = 7;

std::map<long, LLM<float>*> m_mapLLMf;
std::map<long, LLM<double>*> m_mapLLMd;
std::mutex m_mtxFloat;
std::mutex m_mtxDouble;
float m_rgDataF[1024];
double m_rgDataD[1024];

LLM<float>* getLlmFloat(long hHandle, bool bSetBusy)
{
	std::lock_guard<std::mutex> lock(m_mtxFloat);
	std::map<long, LLM<float>*>::iterator it = m_mapLLMf.find(hHandle);
	if (it != m_mapLLMf.end())
	{
		if (bSetBusy)
			it->second->SetBusy(true);	
		return it->second;
	}

	return NULL;
}

LLM<double>* getLlmDouble(long hHandle, bool bSetBusy)
{
	std::lock_guard<std::mutex> lock(m_mtxDouble);
	std::map<long, LLM<double>*>::iterator it = m_mapLLMd.find(hHandle);
	if (it != m_mapLLMd.end())
	{
		if (bSetBusy)
			it->second->SetBusy(true);
		return it->second;
	}

	return NULL;
}

//
// The DLL_InitFloatCustomExtension function is called when first creating an extension using the
// float base type.
//
extern "C" LONG WINAPI DLL_InitFloatCustomExtension(HMODULE hParent, long lKernelIdx)
{
	// This parent export allows Invoking functions implemented by the parent, see FunctionIDs.h
	m_pfnf = (LPFNINTERNAL_INVOKEFLOAT)GetProcAddress(hParent, SZFN_INTERNAL_INVOKEFLOAT);
	if (m_pfnf == NULL)
		return ERROR_INVALID_PARAMETER;

	// This parent export allows allocating host memory in a manner that is consistent with
	// the parent's memory allocation - this must be used for all data returned.
	m_pfnfAllocHost = (LPFNINTERNAL_ALLOCHOSTFLOAT)GetProcAddress(hParent, SZFN_INTERNAL_ALLOCHOSTFLT);
	if (m_pfnfAllocHost == NULL)
		return ERROR_INVALID_PARAMETER;

	m_lKernel = lKernelIdx;

	return 0;
}

//
// The DLL_InvokeFloatCustomExtnsion function is called when running an extension using the
// float base type.  
//
// IMPORTANT: return data must be allocated using the SZFN_INTERNAL_ALLOCHOSTFLT parent export.
//
extern "C" LONG WINAPI DLL_InvokeFloatCustomExtension(LONG lfnIdx, float* pInput, LONG lInput, float** ppOutput, LONG* plOutput, LPTSTR szErr, LONG lErrMax)
{
	LONG lErr;

	if (m_pfnf == NULL)
		return PEER_E_NOT_INITIALIZED;

	switch (lfnIdx)
	{
		case LLM_CUDA_EXTENSION_CREATE:
			{
				float fTemperature = 1.0f; // 0.0 = greedy deterministic. 1.0 = original (range = 0.0 to 1.0).
				float fTopp = 0.9f;        // top-p in nucleus sampling. 1.0 = 0ff. 0.9 works well but slower (range = 0.0 to 1.0).
				long lSeed = 0;            // random seed.

				if (lInput > 0)
					fTemperature = pInput[0];
				if (lInput > 1)
					fTopp = pInput[1];
				if (lInput > 2)
					lSeed = (long)pInput[2];

				LLM<float>* pLLM = new LLM<float>(fTemperature, fTopp, lSeed);
				if (pLLM == NULL)
					return ERROR_OUTOFMEMORY;

				long hHandle = (long)m_mapLLMf.size() + 1;
				m_mtxFloat.lock();
				m_mapLLMf.insert(std::pair<long, LLM<float>*>(hHandle, pLLM));
				m_mtxFloat.unlock();

				m_rgDataF[0] = (float)hHandle;
				if (lErr = (*m_pfnfAllocHost)(m_lKernel, 1, ppOutput, m_rgDataF, false))
				{
					delete pLLM;
					return lErr;
				}
				*plOutput = 1;
			}
			break;

		case LLM_CUDA_EXTENSION_DESTROY:
			{
				long hHandle = (long)pInput[0];
				m_mtxFloat.lock();
				std::map<long, LLM<float>*>::iterator it = m_mapLLMf.find(hHandle);
				if (it != m_mapLLMf.end())
				{
					m_mapLLMf.erase(it);
					it->second->Cancel();

					int nWaitAttempts = 0;
					while (it->second->IsBusy() && nWaitAttempts < (4 * 3))
					{
						Sleep(250);
					}

					if (!it->second->IsBusy())
						delete it->second;
				}
				m_mtxFloat.unlock();
			}
			break;

		case LLM_CUDA_EXTENSION_ABORT:
			{
				long hHandle = (long)pInput[0];
				m_mtxFloat.lock();
				std::map<long, LLM<float>*>::iterator it = m_mapLLMf.find(hHandle);
				if (it != m_mapLLMf.end())
				{
					it->second->Cancel();
				}
				m_mtxFloat.unlock();
			}
			break;

		case LLM_CUDA_EXTENSION_QUERYSTATUS:
			{
				long hHandle = (long)pInput[0];

				LLM<float>* pLLM = getLlmFloat(hHandle, false);
				if (pLLM != NULL)
				{
					float fPct;
					LONG lStatus = 0;
					lErr = pLLM->QueryStatus(&fPct, &lStatus);
					if (lErr)
						return lErr;

					m_rgDataF[0] = fPct;
					m_rgDataF[1] = (float)lStatus;
					if (lErr = (*m_pfnfAllocHost)(m_lKernel, 2, ppOutput, m_rgDataF, false))
						return lErr;
					*plOutput = 2;
				}
				else
				{
					return ERROR_INVALID_PARAMETER;
				}
			}
			break;

		default:
			_tcsncpy(szErr, _T("The function specified is not supported."), lErrMax);
			return ERROR_NOT_SUPPORTED;
	}

	return 0;
}

//
// The DLL_InvokeFloatCustomExtnsionEx function is called when running an extension using the
// float base type.  
//
// IMPORTANT: return data must be allocated using the SZFN_INTERNAL_ALLOCHOSTFLT parent export.
//
extern "C" LONG WINAPI DLL_InvokeFloatCustomExtensionEx(LONG lfnIdx, float* pInput, LONG lInput, float** ppOutput, LONG * plOutput, LPTSTR szInput, LPTSTR szOutput, LONG lOutputMax, LPTSTR szErr, LONG lErrMax)
{
	LONG lErr;

	if (m_pfnf == NULL)
		return PEER_E_NOT_INITIALIZED;

	switch (lfnIdx)
	{
		case LLM_CUDA_EXTENSION_LOAD:
			{
				long hHandle = (long)pInput[0];
				LLM<float>* pLLM = getLlmFloat(hHandle, true);
				if (pLLM != NULL)
				{
					lErr = pLLM->Load(szInput);
					pLLM->SetBusy(false);
					if (lErr)
						return lErr;
				}
			}
			break;

		case LLM_CUDA_EXTENSION_GENERATE:
			{
				long hHandle = (long)pInput[0];
				LLM<float>* pLLM = getLlmFloat(hHandle, true);
				if (pLLM != NULL)
				{
					lErr = pLLM->Generate(szInput);
					pLLM->SetBusy(false);
					if (lErr)
						return lErr;
				}
			}
			break;

		case LLM_CUDA_EXTENSION_QUERYRESPONSE:
			{
				long lEnd = 1;
				long hHandle = (long)pInput[0];
				LLM<float>* pLLM = getLlmFloat(hHandle, false);
				if (pLLM != NULL)
				{
					lErr = pLLM->QueryResponse(szOutput, lOutputMax, &lEnd);
					if (lErr)
						return lErr;
				}

				if (ppOutput != NULL)
					*ppOutput = (float*)lEnd;
			}
			break;

		default:
			_tcsncpy(szErr, _T("The function specified is not supported."), lErrMax);
			return ERROR_NOT_SUPPORTED;
	}

	return 0;
}

//
// The DLL_InitFloatCustomExtension function is called when first creating an extension using the
// double base type.
//
extern "C" LONG WINAPI DLL_InitDoubleCustomExtension(HMODULE hParent, long lKernelIdx)
{
	// This parent export allows Invoking functions implemented by the parent, see FunctionIDs.h
	m_pfndf = (LPFNINTERNAL_INVOKEDOUBLE)GetProcAddress(hParent, SZFN_INTERNAL_INVOKEDOUBLE);
	if (m_pfndf == NULL)
		return ERROR_INVALID_PARAMETER;

	// This parent export allows allocating host memory in a manner that is consistent with
	// the parent's memory allocation - this must be used for all data returned.
	m_pfndfAllocHost = (LPFNINTERNAL_ALLOCHOSTDOUBLE)GetProcAddress(hParent, SZFN_INTERNAL_ALLOCHOSTDBL);
	if (m_pfndfAllocHost == NULL)
		return ERROR_INVALID_PARAMETER;

	m_lKernel = lKernelIdx;

	return 0;
}

//
// The DLL_InvokeFloatCustomExtnsion function is called when running an extension using the
// double base type.  
//
// IMPORTANT: return data must be allocated using the SZFN_INTERNAL_ALLOCHOSTFLT parent export.
//
extern "C" LONG WINAPI DLL_InvokeDoubleCustomExtension(LONG lfnIdx, double* pInput, LONG lInput, double** ppOutput, LONG* plOutput, LPTSTR szErr, LONG lErrMax)
{
	LONG lErr;

	if (m_pfndf == NULL)
		return PEER_E_NOT_INITIALIZED;

	switch (lfnIdx)
	{
		case LLM_CUDA_EXTENSION_CREATE:
			{
				double fTemperature = 1.0; // 0.0 = greedy deterministic. 1.0 = original (range = 0.0 to 1.0).
				double fTopp = 0.9;        // top-p in nucleus sampling. 1.0 = 0ff. 0.9 works well but slower (range = 0.0 to 1.0).
				long lSeed = 0;            // random seed.

				if (lInput > 0)
					fTemperature = pInput[0];
				if (lInput > 1)
					fTopp = pInput[1];
				if (lInput > 2)
					lSeed = (long)pInput[2];

				LLM<double>* pLLM = new LLM<double>(fTemperature, fTopp, lSeed);
				if (pLLM == NULL)
					return ERROR_OUTOFMEMORY;

				long hHandle = (long)m_mapLLMd.size() + 1;
				m_mapLLMd.insert(std::pair<long, LLM<double>*>(hHandle, pLLM));

				m_rgDataD[0] = (double)hHandle;
				if (lErr = (*m_pfndfAllocHost)(m_lKernel, 1, ppOutput, m_rgDataD, false))
				{
					delete pLLM;
					return lErr;
				}
				*plOutput = 1;
			}
			break;

		case LLM_CUDA_EXTENSION_DESTROY:
			{
				long hHandle = (long)pInput[0];
				std::map<long, LLM<double>*>::iterator it = m_mapLLMd.find(hHandle);
				if (it != m_mapLLMd.end())
				{
					m_mapLLMd.erase(it);
					it->second->Cancel();

					int nWaitAttempts = 0;
					while (it->second->IsBusy() && nWaitAttempts < (4 * 3))
					{
						Sleep(250);
					}

					if (!it->second->IsBusy())
						delete it->second;
				}
			}
			break;

		case LLM_CUDA_EXTENSION_QUERYSTATUS:
			{
				long hHandle = (long)pInput[0];
				LLM<double>* pLLM = getLlmDouble(hHandle, false);
				if (pLLM != NULL)
				{
					float fPct;
					LONG lStatus = 0;
					lErr = pLLM->QueryStatus(&fPct, &lStatus);
					if (lErr)
						return lErr;

					m_rgDataD[0] = fPct;
					m_rgDataD[1] = (float)lStatus;
					if (lErr = (*m_pfndfAllocHost)(m_lKernel, 1, ppOutput, m_rgDataD, false))
						return lErr;
					*plOutput = 2;
				}
				else
				{
					return ERROR_INVALID_PARAMETER;
				}
			}
			break;

		default:
			_tcsncpy(szErr, _T("The function specified is not supported."), lErrMax);
			return ERROR_NOT_SUPPORTED;
	}

	return 0;
}

//
// The DLL_InvokeFloatCustomExtnsionEx function is called when running an extension using the
// float base type.  
//
// IMPORTANT: return data must be allocated using the SZFN_INTERNAL_ALLOCHOSTFLT parent export.
//
extern "C" LONG WINAPI DLL_InvokeDoubleCustomExtensionEx(LONG lfnIdx, double* pInput, LONG lInput, double** ppOutput, LONG * plOutput, LPTSTR szInput, LPTSTR szOutput, LONG lOutputMax, LPTSTR szErr, LONG lErrMax)
{
	LONG lErr;

	if (m_pfndf == NULL)
		return PEER_E_NOT_INITIALIZED;

	switch (lfnIdx)
	{
		case LLM_CUDA_EXTENSION_LOAD:
			{
				long hHandle = (long)pInput[0];
				LLM<double>* pLLM = getLlmDouble(hHandle, true);
				if (pLLM != NULL)
				{
					lErr = pLLM->Load(szInput);
					pLLM->SetBusy(false);
					if (lErr)
						return lErr;
				}
			}
			break;

		case LLM_CUDA_EXTENSION_GENERATE:
			{
				long hHandle = (long)pInput[0];
				LLM<double>* pLLM = getLlmDouble(hHandle, true);
				if (pLLM != NULL)
				{
					lErr = pLLM->Generate(szInput);
					pLLM->SetBusy(false);
					if (lErr)
						return lErr;
				}
			}
			break;

		case LLM_CUDA_EXTENSION_QUERYRESPONSE:
			{
				long lEnd = 1;
				long hHandle = (long)pInput[0];
				LLM<double>* pLLM = getLlmDouble(hHandle, false);
				if (pLLM != NULL)
				{
					lErr = pLLM->QueryResponse(szOutput, lOutputMax, &lEnd);
					if (lErr)
						return lErr;
				}

				if (ppOutput != NULL)
					*ppOutput = (double*)lEnd;
			}
			break;

		default:
			_tcsncpy(szErr, _T("The function specified is not supported."), lErrMax);
			return ERROR_NOT_SUPPORTED;
	}

	return 0;
}
