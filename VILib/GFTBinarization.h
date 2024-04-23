#pragma once

#include "IBinarization.h"

//#include "opencv2/opencv.hpp"
using namespace cv;
using namespace std;

// Global fixed thresholding

class CGFTBinarization : public IBinarization
{
public:
	CGFTBinarization();
	virtual ~CGFTBinarization();

public:

	const cv::Mat& GetResImg()
	{
		return m_Img;
	}

	int Do(const cv::Mat& src, cv::Mat& res, const cv::Mat& maskImg, const cv::Mat& maskFImg, map<string, double>& rParam);


public:
	unsigned int m_uiTh;

private:
	cv::Mat m_Img;
};

