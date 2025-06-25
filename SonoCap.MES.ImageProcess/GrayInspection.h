#pragma once
#include "Util.h"

namespace MyOpenCVWrapper {
    //void GrayInspection(const cv::Mat& input, cv::Mat& output, std::string& resultText);
    void GrayInspection(cv::Mat& roiImage, std::string& resultText);
    void GrayInspection2(cv::Mat& roiImage, GrayResult& result);
    void GrayInspection(cv::Mat& roiImage, GrayResult& result);
}
