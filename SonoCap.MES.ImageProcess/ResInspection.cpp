#include "ResInspection.h"

#define ROI_X 165
#define ROI_Y 175
#define ROI_W 60
#define ROI_H 70

#define USE_DENSITY false// 또는 주석처리하고 평균 밝기 쓸 수도 있음
//#define USE_FAST_CENTER
// =========================================================
// 데이터 구조
// =========================================================
struct ResTrialOutcome {
    double threshold;
    ContourData p1;
    ContourData p2;
    ContourData p3;
    ResResult result;
};

// =========================================================
// 단일 Trial 수행 (Res 전용 로직)
// =========================================================
static bool RunResTrial(ResTrialOutcome& out,
    const cv::Mat& fullImg,
    const cv::Rect& roi,
    double perThreshold,
    const MyOpenCVWrapper::InspectionParams::ResParams& cfg)
{
    cv::Mat roiGray = fullImg(roi);

    out.threshold = perThreshold;

    const double minContourArea = cfg.minContourArea;
    const double targetDistance = cfg.targetDistance;
    const double distanceTolerance = cfg.distanceTolerance;
    const double angleToleranceDeg = cfg.angleToleranceDeg;

    // 1. Binary + Morphology
    cv::Mat binary;
    cv::threshold(roiGray, binary, perThreshold, 255, cv::THRESH_BINARY);

    cv::Mat eroded, restored;
    cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));
    cv::erode(binary, eroded, kernel);
    cv::dilate(eroded, restored, kernel);

    // 2. Contour 추출
    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(restored, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);
    if (contours.empty()) return false;

    // 기준점 (ROI 우하단 모서리)
    //cv::Point2f fixedRefPoint(256, 256);
    cv::Point2f fixedRefPoint(static_cast<float>(fullImg.cols - 1), static_cast<float>(fullImg.rows - 1));

    std::vector<ContourData> allContourData;

    // 3. contour 분석
    for (const auto& contour : contours) {
        double currentArea = cv::contourArea(contour);
        if (currentArea < minContourArea)
            continue;

        cv::Moments m = cv::moments(contour);
        if (m.m00 == 0)
            continue;

        cv::Point2f relativeCenter(m.m10 / m.m00, m.m01 / m.m00);
        cv::Point2f absoluteCenter = relativeCenter + cv::Point2f(roi.x, roi.y);

        //Logger::Information(
        //    "relativeCenter = ({0}, {1}), absoluteCenter = ({2}, {3}), fixedRefPoint = ({4}, {5})",
        //    relativeCenter.x,
        //    relativeCenter.y,
        //    absoluteCenter.x,
        //    absoluteCenter.y,
        //    fixedRefPoint.x,
        //    fixedRefPoint.y
        //);

        double currentQuality;
        cv::Mat mask = cv::Mat::zeros(roiGray.size(), CV_8UC1);
        cv::drawContours(mask, std::vector<std::vector<cv::Point>>{contour}, -1, 255, cv::FILLED);

#if USE_DENSITY
        cv::Mat edge, edgeMasked;
        cv::Canny(roiGray, edge, 100, 200);
        edge.copyTo(edgeMasked, mask);
        int edgeCount = cv::countNonZero(edgeMasked);
        int areaCount = cv::countNonZero(mask);
        currentQuality = (areaCount > 0) ? static_cast<double>(edgeCount) / areaCount : -1;
#else
        cv::Scalar mean = cv::mean(roiGray, mask);
        currentQuality = mean[0];
#endif

        double distance_from_ref = cv::norm(absoluteCenter - fixedRefPoint);
        double angle_from_ref_rad = std::atan2(absoluteCenter.y - fixedRefPoint.y, absoluteCenter.x - fixedRefPoint.x);
        double angle_from_ref_deg = angle_from_ref_rad * 180.0 / CV_PI;

        ContourData data;
        data.center_abs = absoluteCenter;
        data.quality = currentQuality;
        data.area = currentArea;
        data.originalContour_rel = contour;
        data.distance_from_ref = distance_from_ref;
        data.angle_from_ref_rad = angle_from_ref_rad;
        data.angle_from_ref_deg = angle_from_ref_deg;
        allContourData.push_back(data);

        //showAndSaveImage("Res_Mask", mask);
    }

    if (allContourData.empty()) return false;

    // 로그 — 원본 그대로
    Logger::Debug("--- All Contour Data Collected ---");
    //for (const auto& contourData : allContourData) {
    for (size_t i = 0; i < allContourData.size(); ++i) {
        const auto& data = allContourData[i];
        Logger::Debug(
            "Contour {0}: Center({1}, {2}), Dist: {3}, Angle(deg): {4}, Area: {5}, Quality: {6}",
            (int)i,
            data.center_abs.x, data.center_abs.y,
            data.distance_from_ref, data.angle_from_ref_deg,
            data.area, data.quality
        );
    }
    Logger::Debug("----------------------------------");

    // 4. P1 / P2 / P3 찾기
    ContourData p1_data, p2_data, p3_data;
    bool p1_found = false, p2_found = false, p3_found = false;

    double max_p1_angle_rad = std::numeric_limits<double>::lowest();

    for (const auto& data : allContourData) {
        if (std::abs(data.distance_from_ref - targetDistance) <= distanceTolerance) {
            if (data.angle_from_ref_rad > max_p1_angle_rad) {
                p1_data = data;
                max_p1_angle_rad = data.angle_from_ref_rad;
                p1_found = true;
            }
        }
    }

    if (p1_found) {
        const double ANGLE_TOLERANCE_RAD = angleToleranceDeg * CV_PI / 180.0;
        double max_p2_area = -1.0;

        for (const auto& data : allContourData) {
            if (data.center_abs == p1_data.center_abs) continue;
            if (data.distance_from_ref > p1_data.distance_from_ref) {
                double angle_diff = std::abs(data.angle_from_ref_rad - p1_data.angle_from_ref_rad);
                if (angle_diff <= ANGLE_TOLERANCE_RAD && data.area > max_p2_area) {
                    p2_data = data;
                    max_p2_area = data.area;
                    p2_found = true;
                }
            }
        }

        double max_p3_area = -1.0;
        for (const auto& data : allContourData) {
            if (data.center_abs == p1_data.center_abs || (p2_found && data.center_abs == p2_data.center_abs)) continue;
            if (std::abs(data.distance_from_ref - targetDistance) <= distanceTolerance &&
                (p2_found && data.angle_from_ref_deg < p2_data.angle_from_ref_deg)) {
                if (data.area > max_p3_area) {
                    p3_data = data;
                    max_p3_area = data.area;
                    p3_found = true;
                }
            }
        }
    }

    if (!(p1_found && p2_found && p3_found)) {
        Logger::Information("Warning: Could not find all 3 required points (P1, P2, P3).");
        return false;
    }

    // 5. 결과 구성
    out.p1 = p1_data;
    out.p2 = p2_data;
    out.p3 = p3_data;
    out.result.verticalDist = cv::norm(p1_data.center_abs - p2_data.center_abs);
    out.result.horizontalDist = cv::norm(p1_data.center_abs - p3_data.center_abs);
    out.result.edgeDensity1 = p1_data.quality;
    out.result.edgeDensity2 = p2_data.quality;
    out.result.edgeDensity3 = p3_data.quality;

    //Logger::Information("--- Selected Points (P1, P2, P3) ---");
    //Logger::Information("P1: Center({0}, {1}), Dist: {2}, Angle(deg): {3}, Area: {4}, Quality: {5}",
    //    p1_data.center_abs.x, p1_data.center_abs.y,
    //    p1_data.distance_from_ref, p1_data.angle_from_ref_deg, p1_data.area, p1_data.quality);
    //Logger::Information("P2: Center({0}, {1}), Dist: {2}, Angle(deg): {3}, Area: {4}, Quality: {5}",
    //    p2_data.center_abs.x, p2_data.center_abs.y,
    //    p2_data.distance_from_ref, p2_data.angle_from_ref_deg, p2_data.area, p2_data.quality);
    //Logger::Information("P3: Center({0}, {1}), Dist: {2}, Angle(deg): {3}, Area: {4}, Quality: {5}",
    //    p3_data.center_abs.x, p3_data.center_abs.y,
    //    p3_data.distance_from_ref, p3_data.angle_from_ref_deg, p3_data.area, p3_data.quality);
    //Logger::Information("-------------------------------------");

    return true;
}

// =========================================================
// 시각화
// =========================================================
static void DrawResTrial(cv::Mat& srcImg, const cv::Rect& roi, const ResTrialOutcome& t)
{
    // contour (P1, P2, P3)
    if (t.p1.area > 0) {
        std::vector<std::vector<cv::Point>> c = { t.p1.originalContour_rel };
        for (auto& p : c[0]) p += cv::Point(roi.x, roi.y);
        cv::drawContours(srcImg, c, -1, RedA, 1);
        cv::circle(srcImg, t.p1.center_abs, 2, RedA, -1);
    }

    if (t.p2.area > 0) {
        std::vector<std::vector<cv::Point>> c = { t.p2.originalContour_rel };
        for (auto& p : c[0]) p += cv::Point(roi.x, roi.y);
        cv::drawContours(srcImg, c, -1, GreenA, 1);
        cv::circle(srcImg, t.p2.center_abs, 2, GreenA, -1);
    }

    if (t.p3.area > 0) {
        std::vector<std::vector<cv::Point>> c = { t.p3.originalContour_rel };
        for (auto& p : c[0]) p += cv::Point(roi.x, roi.y);
        cv::drawContours(srcImg, c, -1, BlueA, 1);
        cv::circle(srcImg, t.p3.center_abs, 2, BlueA, -1);
    }

    cv::rectangle(srcImg, roi, YellowA, 1);
    showAndSaveImage("Res_End", srcImg);
}   

ResTrialOutcome SelectBestTrial(const std::vector<ResTrialOutcome>& trials)
{
    const double targetH = 14.63;
    const double targetV = 10.97;

    // trials가 비어있지 않다고 가정 (항상 출력 있음)
    int bestIdx = 0;
    double bestDistErr = std::numeric_limits<double>::infinity();
    double bestEdgeSum = -std::numeric_limits<double>::infinity();

    for (size_t i = 0; i < trials.size(); ++i) {
        const auto& r = trials[i].result;

        // 거리 오차 (목표값과의 차이 제곱합)
        double distErr = std::pow(r.horizontalDist - targetH, 2) +
            std::pow(r.verticalDist - targetV, 2);

        // 에지 밀도 합 (클수록 좋음)
        double edgeSum = r.edgeDensity1 + r.edgeDensity2 + r.edgeDensity3;

        if (distErr < bestDistErr) {
            bestIdx = static_cast<int>(i);
            bestDistErr = distErr;
            bestEdgeSum = edgeSum;
        }
        else if (std::fabs(distErr - bestDistErr) < 1e-6) {
            // 거리 거의 같으면 edgeSum이 큰 쪽 선택
            if (edgeSum > bestEdgeSum) {
                bestIdx = static_cast<int>(i);
                bestEdgeSum = edgeSum;
            }
        }
    }

    return trials[bestIdx];
}

void LogEnvGeoSummary(const EnvGeoResult& e)
{
    Logger::Information(
        "EnvGeoResult | findPts=%3zu | finalPts=%3zu | MAD(mean=%.3f,std=%.3f) | YInt(min=%.2f,max=%.2f)",
        e.findPoints.size(),
        e.finalPoints.size(),
        e.xFilter.ref_point.x,
        e.xFilter.interval,
        e.yFilter.interval,
        e.yFilter.tolerance
    );
}

inline void LogResHeader()
{
    Logger::Debug("---- Res Trials Summary ----");
    Logger::Debug("Idx | Thresh | Hor(px) | Ver(px) | Edge1 | Edge2 | Edge3 | EdgeAvg");
    Logger::Debug("-----------------------------------------------------------------");
}

inline void LogResRow(const ResTrialOutcome& t, size_t idx)
{
    const auto& r = t.result;
    double avgEdge = (r.edgeDensity1 + r.edgeDensity2 + r.edgeDensity3) / 3.0;

    Logger::Debug(
        "%3zu | %7.3f | %7.2f | %7.2f | %6.3f | %6.3f | %6.3f | %7.3f",
        idx,
        t.threshold,
        r.horizontalDist,
        r.verticalDist,
        r.edgeDensity1,
        r.edgeDensity2,
        r.edgeDensity3,
        avgEdge
    );
}

inline void LogResSummary(const std::vector<ResTrialOutcome>& trials)
{
    if (trials.empty())
    {
        Logger::Information("No Res trials to log.");
        return;
    }

    using namespace System;
    using namespace System::Text;

    StringBuilder^ sb = gcnew StringBuilder(1024);

    sb->AppendLine("");
    sb->AppendLine("---- Res Trials Summary ----");
    sb->AppendLine("Idx | Thresh | Hor(px) | Ver(px) | Edge1 | Edge2 | Edge3 | EdgeAvg");
    sb->AppendLine("-----------------------------------------------------------------");

    for (int i = 0; i < static_cast<int>(trials.size()); ++i)
    {
        const auto& t = trials[i];
        const auto& r = t.result;
        double avgEdge = (r.edgeDensity1 + r.edgeDensity2 + r.edgeDensity3) / 3.0;

        sb->AppendFormat(
            "{0,3} | {1,7:F3} | {2,7:F2} | {3,7:F2} | {4,6:F3} | {5,6:F3} | {6,6:F3} | {7,7:F3}\n",
            i,
            t.threshold,
            r.horizontalDist,
            r.verticalDist,
            r.edgeDensity1,
            r.edgeDensity2,
            r.edgeDensity3,
            avgEdge
        );
    }

    sb->AppendLine("-----------------------------------------------------------------");

    //한 번에 전체 출력
    Logger::Information("{0}", sb->ToString());
}


void MyOpenCVWrapper::ResInspection(cv::Mat& srcImg, ResResult& result)
{
    const auto& cfg = ConfigManager::getInstance().getInspectionParams().res;

    auto prep = CalcHistBasedThreshold(srcImg, cfg.roi);
    auto thresholds = GenerateThresholds(prep.baseThreshold, 20.0, 40);

    std::vector<ResTrialOutcome> trials;

    for (double t : thresholds) {
    ResTrialOutcome temp;
        if (RunResTrial(temp, prep.gray, prep.roi, t, cfg))
            trials.push_back(temp);
    }

    if (trials.empty()) {
        Logger::Warning("No valid Res trials found.");
        return;
    }
    else
    {
        //LogResHeader();
        //for (size_t i = 0; i < trials.size(); ++i) {
        //    LogResRow(trials[i], i);
        //}
		LogResSummary(trials);
    }

    const ResTrialOutcome& chosen = SelectBestTrial(trials); //trials.front();
    
    DrawResTrial(srcImg, prep.roi, chosen);
    result = chosen.result;

    Logger::Information("Final result threshold = {0}, verticalDist = {1}, horizontalDist = {2}",
        chosen.threshold, result.verticalDist, result.horizontalDist);
}