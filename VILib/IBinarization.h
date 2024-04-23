#pragma once
//#include "opencv2/opencv.hpp"
using namespace cv;
using namespace std;

class IBinarization
{
public:
	virtual int Do(const cv::Mat& src, cv::Mat & res, const cv::Mat& maskImg, const cv::Mat& maskFImg, map<string, double> & rParam) = 0;
	virtual const cv::Mat& GetResImg() = 0;
};

