#pragma once


class CResult;
class CGrayImg;
class IRecipe;

class IUAlnspector
{
public:
	virtual int Do(const CGrayImg & rImg, const cv::Mat& maskImg, const cv::Mat& maskFImg, CResult& pRes, const IRecipe * re, int mode) = 0;
};

