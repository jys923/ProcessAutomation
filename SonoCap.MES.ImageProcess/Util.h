#pragma once

#include "ConfigManager.h"
#include "RandomUtilities.h"
#include <opencv2/opencv.hpp>
#include <opencv2/core/utils/logger.hpp>
#include <utility>
#include <iostream>
#include <vector>
#include <numeric>
#include <nlohmann/json.hpp>
#include <algorithm>

using namespace SonoCap::Commons::Logging;

struct PreprocessResult {
    cv::Mat gray;
    cv::Mat roiGray;
    cv::Rect roi;
    cv::Mat hist;
    double baseThreshold;
};

std::vector<double> GenerateThresholds(double base, double range, int count);
PreprocessResult CalcHistBasedThreshold(const cv::Mat& srcImg, const MyOpenCVWrapper::RoiParams& roi);

struct EnvGeoData {
    float leftmost_x;
    float vertical_mid_y;
    double area;
    std::vector<cv::Point> originalContour;
    int originalContourIndex;
};

// ContourData 구조체 정의 (Util.h 또는 ResInspection.h에 정의되어 있어야 함)
 struct ContourData {
     cv::Point2f center_abs;           // roiImage 기준의 절대 좌표 중심점
     double quality;
     double area;                      // 면적을 저장하여 정렬에 사용
     std::vector<cv::Point> originalContour_rel; // roiGray 기준의 상대 윤곽선
     double distance_from_ref;         // 기준점 (W-1, H-1)까지의 거리
     double angle_from_ref_rad;        // 기준점 (W-1, H-1)까지의 각도 (라디안)
     double angle_from_ref_deg;        // 기준점 (W-1, H-1)까지의 각도 (도)

     float leftmost_x_abs;    // 전체 이미지 기준, 윤곽선의 가장 왼쪽 x 좌표
     float vertical_mid_y_abs; // 전체 이미지 기준, 윤곽선의 위-아래 중앙 y 좌표
     int originalContourIndex; // ★ 새로 추가된 필드: 원본 윤곽선의 인덱스
 };

// 전역 변수 선언 (extern 키워드 사용)
extern cv::Mat g_srcImage;
extern cv::Mat g_dstImage;
extern std::string g_windowName;
extern int g_drmin; // dr min 값
extern int g_drmax; // dr max 값

void showAndDRClip(const std::string& windowName, const cv::Mat& image, int& outDrMin, int& outDrMax);
void showAndThreshold(const std::string& windowName, const cv::Mat& image, double& outThreshold, int& outMaxval);
void showAndThreshold(const std::string& windowName, const cv::Mat& image);
void onTrackbar(int, void*);

void calcHist(const cv::Mat& grayImage, cv::Mat& hist);
std::string matToString(const cv::Mat& hist);
void plotHist(const cv::Mat& hist, cv::Mat& imgHist);
double calcPerThreshold(const cv::Mat& hist, const cv::Mat& grayImage, double targetPercentile);

// 상수 정의
const cv::Scalar red(0, 0, 255);
const cv::Scalar orange(0, 165, 255);
const cv::Scalar green(0, 255, 0);
const cv::Scalar blue(255, 0, 0);
const cv::Scalar yellow(0, 255, 255);
const cv::Scalar cyan(255, 255, 0);
const cv::Scalar magenta(255, 0, 255);
const cv::Scalar white(255, 255, 255);
const cv::Scalar black(0, 0, 0);

const cv::Scalar RedA = cv::Scalar(0, 0, 255, 255);
const cv::Scalar OrangeA = cv::Scalar(0, 165, 255, 255);
const cv::Scalar GreenA = cv::Scalar(0, 255, 0, 255);
const cv::Scalar BlueA = cv::Scalar(255, 0, 0, 255);
const cv::Scalar YellowA = cv::Scalar(0, 255, 255, 255);
const cv::Scalar CyanA = cv::Scalar(255, 255, 0, 255);
const cv::Scalar MagentaA = cv::Scalar(255, 0, 255, 255);
const cv::Scalar WhiteA = cv::Scalar(255, 255, 255, 255);
const cv::Scalar GrayA = cv::Scalar(128, 128, 128, 255);
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

namespace nlohmann {
    template <>
    struct adl_serializer<cv::Point2f> {
        static void to_json(json& j, const cv::Point2f& p) {
            j = { {"x", p.x}, {"y", p.y} };
        }

        static void from_json(const json& j, cv::Point2f& p) {
            p.x = j.at("x").get<float>();
            p.y = j.at("y").get<float>();
        }
    };
}

namespace nlohmann {
    template <>
    struct adl_serializer<cv::Point> {
        static void to_json(json& j, const cv::Point& p) {
            j = { {"x", p.x}, {"y", p.y} };
        }

        static void from_json(const json& j, cv::Point& p) {
            p.x = j.at("x").get<int>(); // cv::Point는 int 멤버를 가집니다.
            p.y = j.at("y").get<int>(); // cv::Point는 int 멤버를 가집니다.
        }
    };
}

// 개별 Y 간격 오차 정보를 담을 구조체
struct YIntervalError {
    int point1_index;       // 첫 번째 점의 인덱스
    int point2_index;       // 두 번째 점의 인덱스
    double actual_interval; // 실제 Y 간격
    double target_multiple; // 가장 가까운 목표 간격의 배수
    double error;           // 오차

    // JSON 직렬화를 위한 to_json 함수

};

inline void to_json(nlohmann::json& j, const YIntervalError& e) {
    j = nlohmann::json{
        {"Index1", e.point1_index},
        {"Index2", e.point2_index},
        {"ActualInterval", e.actual_interval},
        {"TargetMultiple", e.target_multiple},
        {"Error", e.error}
    };
}

struct EnvGeoResult2 {
    std::vector<cv::Point2f> pointsOnLeftEdge;
    double x_uniformity_std_dev;

    // 기존의 요약 정보 필드는 삭제하거나 유지
    // 예를 들어, 최대 오차와 같은 요약 정보는 유지할 수 있습니다.
    double y_max_abs_error_to_pattern;

    // 개별 간격 오차 정보를 담을 새로운 필드 추가
    std::vector<YIntervalError> y_interval_errors;

    // ... 기존 필드들은 삭제 (y_matched_pattern_intervals_count 등) ...
};
    inline void to_json(nlohmann::json& j, const EnvGeoResult2& r) {
        j = nlohmann::json::object();
        j["PointsOnLeftEdge"] = r.pointsOnLeftEdge;
        j["XUniformityStdDev"] = r.x_uniformity_std_dev;

        // 새로운 필드를 JSON에 추가
        j["YIntervalErrors"] = r.y_interval_errors;

        // 만약 요약 정보가 필요하다면 추가
        j["YMaxAbsErrorToPattern"] = r.y_max_abs_error_to_pattern;
    }


//struct EnvGeoResult {
//    std::vector<cv::Point2f> pointsOnLeftEdge;
//    friend void to_json(nlohmann::json& j, const EnvGeoResult& r) {
//        j = nlohmann::json::object();
//        j["pointsOnLeftEdge"] = r.pointsOnLeftEdge;
//    }
//};

struct MadMetrics {
    double median_x;
    double mad_x;
    double x_tolerance;
    std::vector<cv::Point> filtered_out_by_x;
};
    inline void to_json(nlohmann::json& j, const MadMetrics& m) {
        j = nlohmann::json{
            {"median_x", m.median_x},
            {"mad_x", m.mad_x},
            {"x_tolerance", m.x_tolerance},
            {"filtered_out_by_x", m.filtered_out_by_x}
        };
    }


// YIntervalMetrics 정보를 담는 구조체
struct YIntervalMetrics {
    double target_y_interval;
    double y_tolerance;
    cv::Point best_ref_point;
    std::vector<cv::Point> filtered_out_by_y;
};
    inline void to_json(nlohmann::json& j, const YIntervalMetrics& y) {
        j = nlohmann::json{
            {"target_y_interval", y.target_y_interval},
            {"y_tolerance", y.y_tolerance},
            {"best_ref_point", y.best_ref_point},
            {"filtered_out_by_y", y.filtered_out_by_y}
        };
    }


struct EnvGeoResult {
    std::vector<cv::Point> findPoints;
    MadMetrics madMetrics;
    YIntervalMetrics yIntervalMetrics;
    std::vector<cv::Point> finalPoints;
};

inline void to_json(nlohmann::json& j, const EnvGeoResult& r) {
    j = nlohmann::json{
        {"findPoints", r.findPoints},
        {"madMetrics", r.madMetrics},
        {"yIntervalMetrics", r.yIntervalMetrics},
        {"finalPoints", r.finalPoints}
    };
}


// Geo 검사 결과
struct GeoResult {
    double meanBrightness = -1;
    double stdBrightness = -1;
    double maxSliceMean = -1;
    double maxSliceVariance = -1;
    double brightnessContrast = -1;
    double elongatedObjectAspectRatio = -1; // 길쭉한 객체의 종횡비 (직선성 지표)
    bool isElongatedObjectFound = -1;       // 길쭉한 객체 발견 여부
    double maxCurvature = -1; // 최대 곡률
    double avgCurvature = -1; // 평균 곡률
    bool isCurvedObjectFound = -1; // 곡선 객체 찾음 여부
};

inline void to_json(nlohmann::json& j, const GeoResult& g) {
    j = nlohmann::json::object();
    if (g.meanBrightness >= 0) j["meanBrightness"] = g.meanBrightness;
    if (g.stdBrightness >= 0) j["stdBrightness"] = g.stdBrightness;
    if (g.maxSliceMean >= 0) j["maxSliceMean"] = g.maxSliceMean;
    if (g.maxSliceVariance >= 0) j["maxSliceVariance"] = g.maxSliceVariance;
    if (g.brightnessContrast >= 0) j["brightnessContrast"] = g.brightnessContrast;
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

inline void to_json(nlohmann::json& j, const SharpnessMetrics& s) {
    j = nlohmann::json::object();
    if (s.tenengrad >= 0)
        j["Tenengrad"] = s.tenengrad;
    if (s.laplacian >= 0)
        j["Laplacian"] = s.laplacian;
}

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
        j = nlohmann::json::object(); // 빈 객체로 초기화

        // cv::Point에 대한 adl_serializer가 정의되었으므로 이제 이 한 줄로 충분합니다.
        j["center"] = data.center;

        j["radius"] = data.radius;
        j["pixelMean"] = data.pixelMean;
    }


//inline void to_json(nlohmann::json& j, const GrayData& data) {
//    j = {
//        {"center", {{"x", data.center.x}, {"y", data.center.y}}},
//        {"radius", data.radius},
//        {"pixelMean", data.pixelMean}
//    };
//}

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

// 함수 선언
float estimateRotationByORB(const cv::Mat& reference, const cv::Mat& rotated);
float estimateRotationByCircularShift(const cv::Mat& reference, const cv::Mat& rotated);
float estimateRotationByPhaseCorrelation(const cv::Mat& reference, const cv::Mat& rotated);
float estimateVerticalShiftByFFT(const cv::Mat& ref, const cv::Mat& target, int angleResolution = 1024);
double calculateCircleMean(const cv::Mat& grayImage, cv::Point center, int radius);
double evaluateContourStraightness(const std::vector<cv::Point>& contour, cv::Mat& resultImage);
double calculateContourStraightnessMSE(const std::vector<cv::Point>& contour, cv::Mat& resultImage);
double calculateContourStraightnessRANSAC(const std::vector<cv::Point>& contour, cv::Mat& resultImage, int iterations = 100, double threshold = 2.0);

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

cv::Mat rotateImage(const cv::Mat& image, float angle);

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

void ApplyLogNormalization(const cv::Mat& inputGray, cv::Mat& outputUint8, double dr_min_percent, double dr_max_percent);

void ApplyLinearDRClip(const cv::Mat& inputGray, cv::Mat& outputUint8, double dr_min_percent, double dr_max_percent, bool normalize = true);

inline double clamp(double val, double min_val, double max_val) {
    return std::max(min_val, std::min(val, max_val));
}

void fitSplineApproximationAndDraw(cv::Mat& roiImage, const std::vector<cv::Point>& contour,
    const cv::Scalar& color);

void fitLineAndDraw(cv::Mat& roiImage, const std::vector<cv::Point>& contour,
    int start_x, int end_x, const cv::Scalar& color);

void fitPolynomialAndDraw(cv::Mat& roiImage, const std::vector<cv::Point>& contour,
    int start_x, int end_x, const cv::Scalar& color, GeoResult& result);

std::vector<cv::Point> removeOutliersIQR(const std::vector<cv::Point>& points, double k_factor);