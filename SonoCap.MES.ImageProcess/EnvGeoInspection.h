#pragma once
#include "Util.h"

namespace MyOpenCVWrapper {
	void EnvGeoInspection(System::IntPtr inputBuffer, int imageWidth, int imageHeight,System::IntPtr resultBuffer, System::IntPtr textBuffer);
	void EnvGeoInspection(cv::Mat& roiImage, EnvGeoResult& result);
}