#pragma once

class CResult;
class IRecipe;
class CGrayImg;
class IUAlnspector;

class IUAIController
{
public:
	virtual int Do(const CGrayImg & rgbImg, const cv::Mat& maskTImg, const cv::Mat& maskFImg, const IRecipe * rRecipe, CResult& res) = 0;
	virtual int Create() = 0;
};

 