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

void MyOpenCVWrapper::GeoInspection2(cv::Mat& roiImage, GeoResult& result)
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

#define NUM_SLICES 5
#define ROI_X  60
#define ROI_Y  (330 - 256)
#define ROI_W  400
#define ROI_H  (NUM_SLICES * 10)

void MyOpenCVWrapper::GeoInspection(cv::Mat& roiImage, GeoResult& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) return;

    cv::Mat grayImage;
    cv::cvtColor(roiImage, grayImage, cv::COLOR_BGRA2GRAY);

    cv::Rect roi(ROI_X, ROI_Y, ROI_W, ROI_H);
    if (roi.x < 0 || roi.y < 0 ||
        roi.x + roi.width > grayImage.cols ||
        roi.y + roi.height > grayImage.rows)
    {
        result.meanBrightness = -1;
        result.stdBrightness = -1;
        result.maxSliceMean = -1;
        result.brightnessContrast = -1;
        result.maxSliceVariance = -1;  // 추가
        return;
    }

    cv::Mat roiMat = grayImage(roi);
    cv::Scalar mean, stddev;
    cv::meanStdDev(roiMat, mean, stddev);

    // 슬라이스 분할
    int sliceHeight = roiMat.rows / NUM_SLICES;
    std::vector<double> sliceMeans;
    std::vector<double> sliceVariances;

    for (int i = 0; i < NUM_SLICES; ++i)
    {
        int y0 = i * sliceHeight;
        int h = (i == NUM_SLICES - 1) ? roiMat.rows - y0 : sliceHeight;
        cv::Mat slice = roiMat(cv::Rect(0, y0, roiMat.cols, h));

        cv::Scalar sliceMean, sliceStddev;
        cv::meanStdDev(slice, sliceMean, sliceStddev);

        sliceMeans.push_back(sliceMean[0]);
        sliceVariances.push_back(sliceStddev[0] * sliceStddev[0]);  // variance
    }

    // 가장 밝은 슬라이스의 index 및 관련 수치
    int brightestSliceIdx = std::distance(
        sliceMeans.begin(),
        std::max_element(sliceMeans.begin(), sliceMeans.end())
    );

    double maxSliceMean = sliceMeans[brightestSliceIdx];
    double contrastRatio = (maxSliceMean - mean[0]) / std::max(mean[0], 1.0);
    double maxSliceVariance = sliceVariances[brightestSliceIdx];  // 추가

    // 결과 저장
    result.meanBrightness = mean[0];
    result.stdBrightness = stddev[0];
    result.maxSliceMean = maxSliceMean;
    result.brightnessContrast = contrastRatio;
    result.maxSliceVariance = maxSliceVariance;  // 추가

    // 시각화
    cv::rectangle(roiImage, roi, RedA, 1);

    if (!sliceMeans.empty())
    {
        int y0 = ROI_Y + brightestSliceIdx * sliceHeight;
        int h = sliceHeight;

        cv::Rect brightSliceRect(ROI_X, y0, ROI_W, h);
        cv::rectangle(roiImage, brightSliceRect, GreenA, 1);
    }
}

