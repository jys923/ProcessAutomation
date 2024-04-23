#pragma once
#include "IUAIController.h"
class CUAIController : public IUAIController
{

public:
	CUAIController();
	~CUAIController();

	int Do(const CGrayImg& rgbImg, const cv::Mat& maskImg, const cv::Mat& maskFImg, const IRecipe * rRecipe, CResult& res);
	int Create();

public:
	IUAlnspector* m_pInsp;
};

