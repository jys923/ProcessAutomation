#pragma once
#include "Util.h"

namespace MyOpenCVWrapper {
	void EnvGeoInspection(System::IntPtr inputBuffer, int imageWidth, int imageHeight,System::IntPtr resultBuffer, System::IntPtr textBuffer);
	void EnvGeoInspection2(cv::Mat& roiImage, EnvGeoResult2& result);
	void EnvGeoInspection3(cv::Mat& roiImage, EnvGeoResult2& result);
	void EnvGeoInspection4(cv::Mat& roiImage, EnvGeoResult2& result);
	void EnvGeoInspection(cv::Mat& roiImage, EnvGeoResult& result);
}