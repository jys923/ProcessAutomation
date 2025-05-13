// 파일명: GeoInspection.cpp
#include "GeoInspection.h"

//void MyOpenCVWrapper::GeoInspection(const cv::Mat& input, cv::Mat& output, std::string& resultText)
//{
//	output.setTo(cv::Scalar(0, 0, 255)); // 🔴 Red
//	resultText = "Geo OK";
//}

// ROI 정의 - 비율 기반으로 이미지 중앙에 위치한 가로 직선 형태
#define ROI_WIDTH_RATIO  0.7   // 이미지 너비의 80%
#define ROI_HEIGHT_RATIO 0.05  // 이미지 높이의 5%

void MyOpenCVWrapper::GeoInspection(cv::Mat& roiImage, std::string& resultText)
{
    if (roiImage.empty() || roiImage.channels() != 4) {
        resultText = R"({"error": "invalid input"})";
        return;
    }

    int imageWidth = roiImage.cols;
    int imageHeight = roiImage.rows;

    // ROI 설정 (중앙 수평 직선 형태)
    int roiWidth = static_cast<int>(imageWidth * ROI_WIDTH_RATIO);
    int roiHeight = static_cast<int>(imageHeight * ROI_HEIGHT_RATIO);
    int roiX = (imageWidth - roiWidth) / 2;
    int roiY = (imageHeight - roiHeight) / 2;

    cv::Rect roi(roiX, roiY, roiWidth, roiHeight);

    // 회색조 변환
    cv::Mat grayImage;
    cv::cvtColor(roiImage, grayImage, cv::COLOR_BGRA2GRAY);

    // ROI 내 통계 분석
    if (roi.x >= 0 && roi.y >= 0 &&
        roi.x + roi.width <= grayImage.cols &&
        roi.y + roi.height <= grayImage.rows)
    {
        cv::Mat roiMat = grayImage(roi);
        cv::Scalar mean, stddev;
        cv::meanStdDev(roiMat, mean, stddev);

        // 결과 시각화
        cv::rectangle(roiImage, roi, GreenA, 1);  // 초록색 사각형

        // 결과 JSON 작성
        std::ostringstream oss;
        oss << "{"
            << R"("roi":{"x":)" << roiX
            << R"(,"y":)" << roiY
            << R"(,"w":)" << roiWidth
            << R"(,"h":)" << roiHeight << "},"
            << R"("mean":)" << mean[0] << ","
            << R"("stddev":)" << stddev[0]
            << "}";

        resultText = oss.str();
    }
    else {
        resultText = R"({"error":"roi out of bounds"})";
    }
}

void MyOpenCVWrapper::GeoInspection(cv::Mat& roiImage, GeoResult& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) return;

    int imageWidth = roiImage.cols;
    int imageHeight = roiImage.rows;

    // ROI 설정 (중앙 수평 직선 형태)
    int roiWidth = static_cast<int>(imageWidth * ROI_WIDTH_RATIO);
    int roiHeight = static_cast<int>(imageHeight * ROI_HEIGHT_RATIO);
    int roiX = (imageWidth - roiWidth) / 2;
    int roiY = (imageHeight - roiHeight) / 2;

    cv::Rect roi(roiX, roiY, roiWidth, roiHeight);

    // 회색조 변환
    cv::Mat grayImage;
    cv::cvtColor(roiImage, grayImage, cv::COLOR_BGRA2GRAY);

    // ROI 내 통계 분석
    if (roi.x >= 0 && roi.y >= 0 &&
        roi.x + roi.width <= grayImage.cols &&
        roi.y + roi.height <= grayImage.rows)
    {
        cv::Mat roiMat = grayImage(roi);
        cv::Scalar mean, stddev;
        cv::meanStdDev(roiMat, mean, stddev);

        // 결과 저장
        result.meanBrightness = mean[0];
        result.stdBrightness = stddev[0];

        // 시각화
        cv::rectangle(roiImage, roi, GreenA, 1);
    }
    else {
        result.meanBrightness = -1;
        result.stdBrightness = -1;
    }
}
