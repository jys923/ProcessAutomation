#include "ResInspection.h"

// 디파인: 핀의 사각형 ROI 크기 및 위치 (좌상단 기준)
#define PIN_W  20
#define PIN_H  20

#define PIN1_X 30
#define PIN1_Y 40

#define PIN2_X 80
#define PIN2_Y 60

#define PIN3_X 130
#define PIN3_Y 50

void MyOpenCVWrapper::ResInspection(cv::Mat& roiImage, std::string& resultText)
{
    if (roiImage.empty() || roiImage.channels() != 4) {
        resultText = R"({"error": "invalid input"})";
        return;
    }

    // Grayscale 변환
    cv::Mat gray;
    cv::cvtColor(roiImage, gray, cv::COLOR_BGRA2GRAY);

    std::vector<cv::Rect> pinRects = {
        { PIN1_X, PIN1_Y, PIN_W, PIN_H },
        { PIN2_X, PIN2_Y, PIN_W, PIN_H },
        { PIN3_X, PIN3_Y, PIN_W, PIN_H }
    };

    std::vector<double> densities;
    std::vector<cv::Scalar> colors = { RedA, GreenA, BlueA };

    for (int i = 0; i < pinRects.size(); ++i) {
        const auto& rect = pinRects[i];
        if (rect.x >= 0 && rect.y >= 0 &&
            rect.x + rect.width <= gray.cols &&
            rect.y + rect.height <= gray.rows) {

            cv::Mat pinROI = gray(rect);
            cv::Mat edge;
            cv::Canny(pinROI, edge, 100, 200);

            double density = cv::countNonZero(edge) / (double)(rect.area());
            densities.push_back(density);

            // 시각화
            cv::rectangle(roiImage, rect, colors[i], 2);
        }
        else {
            densities.push_back(-1);
        }
    }

    // 통계값 계산
    double avg = 0, minVal = DBL_MAX, maxVal = DBL_MIN;
    int validCount = 0;
    for (double d : densities) {
        if (d >= 0) {
            avg += d;
            minVal = std::min(minVal, d);
            maxVal = std::max(maxVal, d);
            ++validCount;
        }
    }
    avg = (validCount > 0) ? avg / validCount : -1;

    // JSON 결과 생성
    std::ostringstream oss;
    oss << "{";
    for (int i = 0; i < densities.size(); ++i) {
        oss << R"("pin)" << (i + 1) << R"(": )" << densities[i];
        if (i < densities.size() - 1) oss << ", ";
    }
    oss << ", \"average\": " << avg
        << ", \"min\": " << minVal
        << ", \"max\": " << maxVal
        << "}";

    resultText = oss.str();
}

void MyOpenCVWrapper::ResInspection(cv::Mat& roiImage, ResResult& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) return;

    // Grayscale 변환
    cv::Mat gray;
    cv::cvtColor(roiImage, gray, cv::COLOR_BGRA2GRAY);

    std::vector<cv::Rect> pinRects = {
        { PIN1_X, PIN1_Y, PIN_W, PIN_H },
        { PIN2_X, PIN2_Y, PIN_W, PIN_H },
        { PIN3_X, PIN3_Y, PIN_W, PIN_H }
    };

    std::vector<cv::Point> centers;
    std::vector<cv::Scalar> colors = { RedA, GreenA, BlueA };
    double* densityTargets[3] = { &result.edgeDensity1, &result.edgeDensity2, &result.edgeDensity3 };

    for (int i = 0; i < pinRects.size(); ++i) {
        const auto& rect = pinRects[i];
        if (rect.x >= 0 && rect.y >= 0 &&
            rect.x + rect.width <= gray.cols &&
            rect.y + rect.height <= gray.rows) {

            cv::Mat pinROI = gray(rect);
            cv::Mat edge;
            cv::Canny(pinROI, edge, 100, 200);

            double density = static_cast<double>(cv::countNonZero(edge)) / rect.area();
            *densityTargets[i] = density;

            cv::rectangle(roiImage, rect, colors[i], 2);
            centers.push_back(cv::Point(rect.x + rect.width / 2, rect.y + rect.height / 2));
        }
        else {
            *densityTargets[i] = -1;
            centers.push_back(cv::Point(-1, -1));
        }
    }

    // 거리 계산: horizontal (1-2), vertical (1-3)
    if (centers[0].x >= 0 && centers[1].x >= 0)
        result.horizontalDist = cv::norm(centers[0] - centers[1]);
    if (centers[0].x >= 0 && centers[2].x >= 0)
        result.verticalDist = cv::norm(centers[0] - centers[2]);
}