#include "pch.h"

#include "IUAlnspector.h"

#include "UAInspectorBasedBlob_TH.h"

#include "UAIController.h"


CUAIController::CUAIController()
{
	m_pInsp = NULL;
}


CUAIController::~CUAIController()
{
	if (NULL != m_pInsp)
	{
		delete m_pInsp;
		m_pInsp = NULL;
	}
}

int CUAIController::Do(const CGrayImg& grayImg, const cv::Mat& maskImg, const cv::Mat& maskFImg, const IRecipe * rRecipe, CResult& res)
{
	int ret = 0;
	if (NULL != m_pInsp)
	{
		ret = m_pInsp->Do(grayImg, maskImg, maskFImg, res, rRecipe, 0);
	}
	else
	{
		Create();
		ret = m_pInsp->Do(grayImg, maskImg, maskFImg, res, rRecipe, 0);
	}

	return 1;
}


int CUAIController::Create()
{
	m_pInsp = new CUAInspectorBasedBlob_TH;
	if (NULL == m_pInsp) return -1;
	return 1;
}