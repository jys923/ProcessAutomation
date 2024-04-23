#pragma once
#include "ILabeling.h"

class CLabelingRes;

class CLabeling :   public ILabeling
{

public:
	CLabeling();
	~CLabeling();

public:
	int Do(const cv::Mat& src, cv::Mat & res,  map<string, string>& rParam);

private:
	int labeling(const Mat& src, Mat& res);

	const CLabelingRes& GetResult()
	{
		return m_Res;
	}

	const cv::Mat & GetLabeledImg()
	{
		return m_Img;
	}

public:
	cv::Mat m_Img;


private:
	vector< vector<cv::Point> > m_vRes;
	vector<cv::Rect>   m_vBoundingRect;
	vector<cv::RotatedRect>   m_vRotatedBoundingRect;

public:
	CLabelingRes m_Res;
};

