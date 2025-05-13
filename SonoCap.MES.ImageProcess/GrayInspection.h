#pragma once
#include "Util.h"

namespace MyOpenCVWrapper {
    //void GrayInspection(const cv::Mat& input, cv::Mat& output, std::string& resultText);
    void GrayInspection(cv::Mat& roiImage, std::string& resultText);
    void GrayInspection(cv::Mat& roiImage, GrayResult& result);
}
