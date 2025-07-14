#pragma once
#include "Util.h"

namespace MyOpenCVWrapper {
	void GeoInspection(cv::Mat& roiImage, std::string& resultText);
	void GeoInspection2(cv::Mat& roiImage, GeoResult& result);
	void GeoInspection(cv::Mat& roiImage, GeoResult& result);
	void GeoInspection3(cv::Mat& roiImage, GeoResult& result);
}