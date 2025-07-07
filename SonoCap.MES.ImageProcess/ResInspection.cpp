#include "ResInspection.h"

#define ROI_X 165
#define ROI_Y 185
#define ROI_W 50
#define ROI_H 60

#define USE_DENSITY  // 또는 주석처리하고 평균 밝기 쓸 수도 있음
//#define USE_FAST_CENTER

void MyOpenCVWrapper::ResInspection(cv::Mat& roiImage, ResResult& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) return;

    showAndSaveImage("ResInspection", roiImage);

    cv::Mat gray;
    cv::cvtColor(roiImage, gray, cv::COLOR_BGRA2GRAY);
    showAndSaveImage("ResInspection", gray);

    // DR 클리핑
    ApplyLinearDRClip(gray, gray, 65, 69, true);
    showAndSaveImage("ResInspection", gray);

    // ROI 설정 및 검출 준비
    cv::Rect roi(ROI_X, ROI_Y, ROI_W, ROI_H);
    if (roi.x < 0 || roi.y < 0 || roi.x + roi.width > gray.cols || roi.y + roi.height > gray.rows) return;

    cv::Mat roiGray = gray(roi);
    cv::Mat binary;
    cv::threshold(roiGray, binary, 100, 255, cv::THRESH_BINARY);

    // 모폴로지 오프닝으로 곁가지 제거
    cv::Mat eroded, restored;
    cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));

    // 1. Erosion으로 살짝 줄임 (붙은 부분 끊기)
    cv::erode(binary, eroded, kernel);

    // 2. Dilation으로 원래 크기 복원
    cv::dilate(eroded, restored, kernel);
    showAndSaveImage("ResInspection", restored);

    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(restored, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);

    std::vector<cv::Point2f> centers;
    double* quality[3] = { &result.edgeDensity1, &result.edgeDensity2, &result.edgeDensity3 };
    std::vector<cv::Scalar> colors = { RedA, GreenA, BlueA };

    int count = 0;
    for (const auto& contour : contours) {
        if (count >= 3) break;
        if (cv::contourArea(contour) < 4)
            continue;

        cv::Moments m = cv::moments(contour);
        if (m.m00 == 0) continue;
        cv::Point2f center(m.m10 / m.m00, m.m01 / m.m00);
        centers.push_back(center);

        // 마스크 및 퀄리티 계산
        cv::Mat mask = cv::Mat::zeros(roiGray.size(), CV_8UC1);
        cv::drawContours(mask, std::vector<std::vector<cv::Point>>{contour}, -1, 255, cv::FILLED);

#ifdef USE_DENSITY
        cv::Mat edge, edgeMasked;
        cv::Canny(roiGray, edge, 100, 200);
        edge.copyTo(edgeMasked, mask);
        int edgeCount = cv::countNonZero(edgeMasked);
        int areaCount = cv::countNonZero(mask);
        *quality[count] = (areaCount > 0) ? static_cast<double>(edgeCount) / areaCount : -1;
#else
        cv::Scalar mean = cv::mean(roiGray, mask);
        *quality[count] = mean[0];
#endif

        // 결과 이미지에 표시
        if (count < colors.size()) {
            std::vector<std::vector<cv::Point>> shifted = { contour };
            for (auto& pt : shifted[0])
                pt += cv::Point(ROI_X, ROI_Y);
            cv::drawContours(roiImage, shifted, -1, colors[count], 1);
        }

        count++;
    }
    cv::rectangle(roiImage, roi, YellowA, 1);  // 밝은 노란색 1px 테두리
    showAndSaveImage("ResInspection", roiImage);

    if (centers.size() != 3) {
        result.verticalDist = -1;
        result.horizontalDist = -1;
        return;
    }

    std::sort(centers.begin(), centers.end(), [](const cv::Point2f& a, const cv::Point2f& b) {
        return a.y < b.y;
        });

    for (auto& pt : centers)
        pt += cv::Point2f(ROI_X, ROI_Y);

    result.verticalDist = cv::norm(centers[0] - centers[1]);
    result.horizontalDist = cv::norm(centers[1] - centers[2]);
}

// 디파인: 핀의 사각형 ROI 크기 및 위치 (좌상단 기준)
#define PIN_W  10
#define PIN_H  10

#define PIN1_X 194
#define PIN1_Y 207

#define PIN2_X 201
#define PIN2_Y 216

#define PIN3_X 193
#define PIN3_Y 231

//#define PIN1_X 0
//#define PIN1_Y 0
//
//#define PIN2_X 10
//#define PIN2_Y 10
//
//#define PIN3_X 20
//#define PIN3_Y 20


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

void MyOpenCVWrapper::ResInspection2(cv::Mat& roiImage, ResResult& result)
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