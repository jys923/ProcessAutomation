#pragma once

#include <opencv2/opencv.hpp>
#include <vector>
#include <utility>
#include <opencv2/core/utils/logger.hpp>
#include <iostream>
#include <vector>
#include <numeric>
#include <nlohmann/json.hpp>

#define ENABLE_IMAGE_DISPLAY false

// 상수 정의
const cv::Scalar red(0, 0, 255);
const cv::Scalar green(0, 255, 0);
const cv::Scalar blue(255, 0, 0);
const cv::Scalar yellow(0, 255, 255);
const cv::Scalar cyan(255, 255, 0);
const cv::Scalar magenta(255, 0, 255);
const cv::Scalar white(255, 255, 255);
const cv::Scalar black(0, 0, 0);

const cv::Scalar RedA = cv::Scalar(0, 0, 255, 255);
const cv::Scalar GreenA = cv::Scalar(0, 255, 0, 255);
const cv::Scalar BlueA = cv::Scalar(255, 0, 0, 255);
const cv::Scalar YellowA = cv::Scalar(0, 255, 255, 255);
const cv::Scalar CyanA = cv::Scalar(255, 255, 0, 255);
const cv::Scalar MagentaA = cv::Scalar(255, 0, 255, 255);
const cv::Scalar WhiteA = cv::Scalar(255, 255, 255, 255);
const cv::Scalar BlackA = cv::Scalar(0, 0, 0, 255);

// Gray 검사 결과
struct GrayResult {
    double mean1 = -1;
    double mean2 = -1;
    double mean3 = -1;
};

inline void to_json(nlohmann::json& j, const GrayResult& g) {
    j = nlohmann::json::object();
    if (g.mean1 >= 0) j["mean1"] = g.mean1;
    if (g.mean2 >= 0) j["mean2"] = g.mean2;
    if (g.mean3 >= 0) j["mean3"] = g.mean3;
}

// Res 검사 결과
struct ResResult {
    double edgeDensity1 = -1;
    double edgeDensity2 = -1;
    double edgeDensity3 = -1;
    double horizontalDist = -1;
    double verticalDist = -1;
};

inline void to_json(nlohmann::json& j, const ResResult& r) {
    j = nlohmann::json::object();
    if (r.edgeDensity1 >= 0) j["edgeDensity1"] = r.edgeDensity1;
    if (r.edgeDensity2 >= 0) j["edgeDensity2"] = r.edgeDensity2;
    if (r.edgeDensity3 >= 0) j["edgeDensity3"] = r.edgeDensity3;
    if (r.horizontalDist >= 0) j["horizontalDist"] = r.horizontalDist;
    if (r.verticalDist >= 0) j["verticalDist"] = r.verticalDist;
}

// Geo 검사 결과
struct GeoResult {
    double meanBrightness = -1;
    double stdBrightness = -1;
};

inline void to_json(nlohmann::json& j, const GeoResult& g) {
    j = nlohmann::json::object();
    if (g.meanBrightness >= 0) j["meanBrightness"] = g.meanBrightness;
    if (g.stdBrightness >= 0) j["stdBrightness"] = g.stdBrightness;
}

// 전체 검사 결과
struct InspectionResult {
    GrayResult Gray;
    ResResult Res;
    GeoResult Geo;
};

inline void to_json(nlohmann::json& j, const InspectionResult& r) {
    j = nlohmann::json::object();
    nlohmann::json jGray, jRes, jGeo;
    to_json(jGray, r.Gray);
    to_json(jRes, r.Res);
    to_json(jGeo, r.Geo);

    if (!jGray.empty()) j["Gray"] = jGray;
    if (!jRes.empty())  j["Res"] = jRes;
    if (!jGeo.empty())  j["Geo"] = jGeo;
}

struct SharpnessMetrics {
    double tenengrad = -1;
    double laplacian = -1;
};

struct QualityMetrics {
    SharpnessMetrics sharpness;
    double brightness = -1;
    double contrast = -1;
    double snr = -1;
    double speckleIndex = -1;
    double entropy = -1;
    double edgeDensity = -1;
    double localVariance = -1;
    double cnr = -1;
    double fourierNoise = -1;
};

inline void to_json(nlohmann::json& j, const SharpnessMetrics& s) {
    j = nlohmann::json::object();
    if (s.tenengrad >= 0)
        j["Tenengrad"] = s.tenengrad;
    if (s.laplacian >= 0)
        j["Laplacian"] = s.laplacian;
}

inline void to_json(nlohmann::json& j, const QualityMetrics& q) {
    j = nlohmann::json::object();

    if (q.sharpness.tenengrad >= 0 || q.sharpness.laplacian >= 0)
        j["Sharpness"] = q.sharpness;
    if (q.brightness >= 0)
        j["Brightness"] = q.brightness;
    if (q.contrast >= 0)
        j["Contrast"] = q.contrast;
    if (q.snr >= 0)
        j["SNR"] = q.snr;
    if (q.speckleIndex >= 0)
        j["SpeckleIndex"] = q.speckleIndex;
    if (q.entropy >= 0)
        j["Entropy"] = q.entropy;
    if (q.edgeDensity >= 0)
        j["EdgeDensity"] = q.edgeDensity;
    if (q.localVariance >= 0)
        j["LocalVariance"] = q.localVariance;
    if (q.cnr >= 0)
        j["CNR"] = q.cnr;
    if (q.fourierNoise >= 0)
        j["FourierNoise"] = q.fourierNoise;
}

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
