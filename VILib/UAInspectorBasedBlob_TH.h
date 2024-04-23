#pragma once

#include "IUAlnspector.h"

using namespace cv;
using namespace std;

class CBlobDetector;
class CGrayImg;
class IRecipe;

class CUAInspectorBasedBlob_TH : public IUAlnspector
{

public:
	CUAInspectorBasedBlob_TH();
	~CUAInspectorBasedBlob_TH();

public:
	int Do(const CGrayImg & rSrc, const cv::Mat& maskImg, const cv::Mat& maskFImg, CResult & pRes, const IRecipe * rRecipe, int mode);

	virtual int ProcessBinarization(const Mat& Src, Mat& Res,const cv::Mat& maskImg, const cv::Mat& maskFImg, int nTh);

	virtual CBlobDetector * ProcessBlobDetection(const Mat& Src, Mat& Res, const cv::Mat& maskImg, uint r, uint rmarg);

private:
	int CalMean(const cv::Mat& OrgImg, const cv::Mat& BinImg, const cv::Rect& rect, int nCnt);
	int CalMean(const cv::Mat& OrgImg, const cv::Mat& BinImg, int x, int y, int win_size);
	int MakeResult(const Mat& rOrgImg, const Mat& rBinImg, CBlobDetector* pBD, CResult& rRes);
};

