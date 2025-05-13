#pragma once
#include "Util.h"

namespace MyOpenCVWrapper {
    void ResInspection(cv::Mat& roiImage, std::string& resultText);
    void ResInspection(cv::Mat& roiImage, ResResult& result);
}