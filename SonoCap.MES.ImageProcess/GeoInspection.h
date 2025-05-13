#pragma once
#include "Util.h"

namespace MyOpenCVWrapper {
	void GeoInspection(cv::Mat& roiImage, std::string& resultText);
	void GeoInspection(cv::Mat& roiImage, GeoResult& result);
}