#pragma once

#include <opencv2/opencv.hpp>
#include <vector>
#include <utility>
#include <opencv2/core/utils/logger.hpp>
#include <iostream>
#include <vector>
#include <numeric>
#include <nlohmann/json.hpp>

#define ENABLE_IMAGE_DISPLAY true

// 상수 정의
const cv::Scalar red(0, 0, 255);
const cv::Scalar green(0, 255, 0);
const cv::Scalar blue(255, 0, 0);
const cv::Scalar yellow(0, 255, 255);
const cv::Scalar cyan(255, 255, 0);
const cv::Scalar magenta(255, 0, 255);
const cv::Scalar white(255, 255, 255);
const cv::Scalar black(0, 0, 0);

struct GrayData {
    cv::Point center;
    double pixelMean;
	int radius;
};

inline void to_json(nlohmann::json& j, const GrayData& data) {
    j = {
        {"center", {{"x", data.center.x}, {"y", data.center.y}}},
        {"radius", data.radius},
        {"pixelMean", data.pixelMean}
    };
}

struct StraightnessData {
    double score = 0.0;
};

inline void to_json(nlohmann::json& j, const StraightnessData& data) {
    j = {
        {"score", data.score}
    };
}

struct ArcData {
    cv::Point center;
    double angle;
    double area;
    double distanceToNext = 0.0;
    // JSON 변환을 위한 함수
    /*nlohmann::json toJson() const {
        return {
            {"center", {{"x", center.x}, {"y", center.y}}},
            {"angle", angle},
            {"area", area}
        };
    }*/
};

inline void to_json(nlohmann::json& j, const ArcData& data) {
    j = {
        {"center", {{"x", data.center.x}, {"y", data.center.y}}},
        {"angle", data.angle},
        {"area", data.area},
		{"distanceToNext", data.distanceToNext}
    };
}
// 구조체 정의
struct ContourInfo {
    std::vector<cv::Point> contour;
    double circularity;
    float radius;
    cv::RotatedRect ellipse;

    // 생성자
    ContourInfo(const std::vector<cv::Point>& c, double circ, float ra)
        : contour(c), circularity(circ), radius(ra), ellipse(cv::RotatedRect(cv::Point2f(0, 0), cv::Size2f(0, 0), 0)) {
    }

    ContourInfo(const std::vector<cv::Point>& c, double circ, float ra, const cv::RotatedRect& e)
        : contour(c), circularity(circ), radius(ra), ellipse(e) {
    }
};
// `nlohmann::json` 직렬화 지원을 위한 `to_json()` 오버로드
inline void to_json(nlohmann::json& j, const ContourInfo& data) {
    j = {
		{"circularity", data.circularity},
		{"radius", data.radius}
    };
}

// 범용 벡터 -> JSON 변환 함수 (템플릿 활용)
template <typename T>
std::string vectorToJsonString(const std::vector<T>& dataVector) {
    return nlohmann::json(dataVector).dump(-1); // 4 = 들여쓰기
}

template <typename T>
std::string objectToJsonString(const T& dataObject) {
    return nlohmann::json(dataObject).dump(-1); // 4 = 들여쓰기
}

// 함수 선언 (알파벳 순으로 정렬)
double calculateCircleMean(const cv::Mat& grayImage, cv::Point center, int radius);
double evaluateContourStraightness(const std::vector<cv::Point>& contour, cv::Mat& resultImage);
double calculateContourStraightnessMSE(const std::vector<cv::Point>& contour, cv::Mat& resultImage);
double calculateContourStraightnessRANSAC(const std::vector<cv::Point>& contour, cv::Mat& resultImage, int iterations = 100, double threshold = 2.0);
void processLogNormalization(const cv::Mat& input, cv::Mat& output, double dr_min, double dr_max);

void showAndSaveImage(const std::string& windowName, const cv::Mat& image);

cv::Scalar getRandomColor();

bool compareCircularity(const ContourInfo& a, const ContourInfo& b);

void calculateAndDisplayHistogram(const cv::Mat& inputImage, cv::Mat& outputImage);

cv::Point calculateNewPoint(const cv::Point& center, const cv::Point& point, double currentAngle, double angleOffset, double radiusOffset);

cv::Point calculateNewPoint(const cv::Point& center, const cv::Point& point, double currentAngle, double angleOffset);

cv::Point calculateNewPoint(const cv::Point& center, const cv::Point& point, double angleOffset);

double calculateAngle(const cv::Point& center, const cv::Point& point);

double calculateArcLength(const cv::Point& center, const cv::Point& p1, const cv::Point& p2, double radius);

double calculateArcLength(double angle1, double angle2, double radius);

double calculateCircularity(const cv::RotatedRect& ellipse);

double calculateCircularity(const std::vector<cv::Point>& contour);

std::vector<int> calculateRadii(const std::vector<std::vector<cv::Point>>& contours);

cv::Mat createCircularMask(const cv::Size& size, int innerRadius, int outerRadius);

cv::Mat rotateImage(const cv::Mat& image, double angle);

std::vector<cv::Point> extractCirclePoints(int radius, const cv::Point& center);

bool isNearCType(const std::vector<cv::Point>& cType, const cv::Point& oPt, int range);

bool isOpenCShape(const std::vector<cv::Point>& contour);

bool isOpenCShape(const std::vector<cv::Point>& contour, float radius);

bool isOpenCShape(const std::vector<cv::Point>& contour, double minRadius, double maxRadius, const cv::Point2f& imageCenter, const cv::Point2f& center, float radius);

cv::Mat drawExtractedPoints(const cv::Mat& image, const std::vector<cv::Point>& points);

    void drawPoints(const cv::Mat& inputImage, cv::Mat& outputImage, const std::vector<cv::Point>& points, const cv::Scalar& color = cv::Scalar(0, 255, 255));

void drawPreciseCirclePoints();

void drawCircleUsingOpenCV();

void drawExtractedCirclePoints();
