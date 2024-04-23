#pragma once

//#include "opencv2/opencv.hpp"
using namespace cv;
using namespace std;

class CLabelingRes;

class ILabeling
{
public:
	virtual int Do(const cv::Mat& src, cv::Mat & res, map<string, string>& rParam) = 0;
	virtual const CLabelingRes& GetResult() = 0;
	virtual const cv::Mat & GetLabeledImg() = 0;

};

