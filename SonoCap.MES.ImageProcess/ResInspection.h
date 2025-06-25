#pragma once
#include "Util.h"

namespace MyOpenCVWrapper {
    void ResInspection(cv::Mat& roiImage, std::string& resultText);
    void ResInspection2(cv::Mat& roiImage, ResResult& result);
    void ResInspection(cv::Mat& roiImage, ResResult& result);
}