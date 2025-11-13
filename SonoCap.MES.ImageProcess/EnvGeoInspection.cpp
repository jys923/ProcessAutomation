#include "EnvGeoInspection.h"

// =========================================================
// 데이터 구조
// =========================================================
struct TrialOutcome {
    double threshold;
    MorphMode morphMode;
    std::vector<EnvGeoData> detectedPoints;
    EnvGeoResult result;
    //std::vector<EnvGeoData> finalResultPoints;
    //std::vector<EnvGeoData> filteredOutXPoints;
    //std::vector<EnvGeoData> filteredOutYPoints;
};

// =========================================================
// Trial 단위 처리 함수 (원본 로직 완전 복원)
// =========================================================
bool RunEnvGeoTrial(TrialOutcome& out, const cv::Mat& roiGray, const cv::Rect& roi, double perThreshold, const MyOpenCVWrapper::InspectionParams::EnvGeoParams& cfg, MorphMode morphMode)
{
    AxisFilter xF, yF;
    std::vector<EnvGeoData> detectedPoints;

    std::vector<cv::Mat> stageImages;
    
    double bestThresh;
    int maxVal = 255;

    const double minContourArea = cfg.minContourArea;
    //const float yTolerance = cfg.yTolerance;

    const double BASE_DIST_TOL = cfg.clusterDistTol;   // 기존 BASE_DIST_TOL 대체
    const double X_TOL = cfg.clusterXTol;      // 기존 X_TOL 대체
    //const double clusterYTol = cfg.clusterYTol;      // 새 항목 (Y 방향 병합 간격)
    const double xTol = cfg.xTol;             // 기존 하드코딩 2.0 대체
    const double targetY = cfg.yInterval;        // 기존 targetYInterval 대체
    const double yTol = cfg.yTol;             // 기존 yIntervalTolerance 대체

    // 1. Binary + Morphology
    cv::Mat binary;
    cv::threshold(roiGray, binary, perThreshold, 255, cv::THRESH_BINARY);
    cv::Mat eroded, dilated, restored;
    cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));

    switch (morphMode)
    {
        case MorphMode::Open11:
            cv::erode(binary, eroded, kernel, cv::Point(-1, -1), 1);
            cv::dilate(eroded, restored, kernel, cv::Point(-1, -1), 1); break; // Open(1,1)
        case MorphMode::Close11:
            cv::dilate(binary, dilated, kernel, cv::Point(-1, -1), 1);
            cv::erode(dilated, restored, kernel, cv::Point(-1, -1), 1); break; // Close(1,1)
        case MorphMode::Open12:
            cv::erode(binary, eroded, kernel, cv::Point(-1, -1), 1);
            cv::dilate(eroded, restored, kernel, cv::Point(-1, -1), 2); break; // Open(1,2)
        case MorphMode::Close12:
            cv::dilate(binary, dilated, kernel, cv::Point(-1, -1), 1);
            cv::erode(dilated, restored, kernel, cv::Point(-1, -1), 2); break; // Close(1,2)
        case MorphMode::HybridOpenClose:
            cv::erode(binary, eroded, kernel, cv::Point(-1, -1), 1);
            cv::dilate(eroded, dilated, kernel, cv::Point(-1, -1), 2);
            cv::erode(dilated, restored, kernel, cv::Point(-1, -1), 1); break; // Hybrid Open→Close
        case MorphMode::HybridCloseOpen:
            cv::dilate(binary, dilated, kernel, cv::Point(-1, -1), 1);
            cv::erode(dilated, eroded, kernel, cv::Point(-1, -1), 2);
            cv::dilate(eroded, restored, kernel, cv::Point(-1, -1), 1); break; // Hybrid Close→Open
    }

    cv::Mat grayBGR, binaryBGR, restoredBGR;
    cv::cvtColor(roiGray, grayBGR, cv::COLOR_GRAY2BGR);
    cv::cvtColor(binary, binaryBGR, cv::COLOR_GRAY2BGR);
    cv::cvtColor(restored, restoredBGR, cv::COLOR_GRAY2BGR);

    stageImages.push_back(grayBGR.clone());
    stageImages.push_back(binaryBGR.clone());
    stageImages.push_back(restoredBGR.clone());

    // 2. Contour 추출
    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(restored, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);
    if (contours.empty()) return false;

    cv::Mat dbgContours;
    cv::cvtColor(restored, dbgContours, cv::COLOR_GRAY2BGR);

    for (size_t i = 0; i < contours.size(); ++i)
    {
        cv::Scalar color = getRandomColor();
        cv::drawContours(dbgContours, contours, (int)i, color, 1.5);
    }
    stageImages.push_back(dbgContours.clone());

    // 3. Contour → EnvGeoData 변환
    detectedPoints.clear();
    for (int i = 0; i < contours.size(); ++i) {
        const auto& contour = contours[i];
        double area = cv::contourArea(contour);
        if (area < minContourArea)
            continue;

        /*float min_x = std::numeric_limits<float>::max();
        float min_y = std::numeric_limits<float>::max();
        float max_y = std::numeric_limits<float>::lowest();

        for (auto& p : contour) {
            if (p.x < min_x) min_x = p.x;
            if (p.y < min_y) min_y = p.y;
            if (p.y > max_y) max_y = p.y;
        }

        float vertical_mid_y = (min_y + max_y) / 2.0f;
        cv::Point2f extracted(min_x, vertical_mid_y);
        double dist = cv::pointPolygonTest(contour, extracted, false);
        if (dist < 0) continue;
        detectedPoints.push_back({ min_x, vertical_mid_y, area, contour, i });*/

        // ---- 컨투어 중심점(centroid) 계산 ----
        cv::Moments m = cv::moments(contour);
        if (m.m00 == 0) continue;  // 면적 0은 무시 (divide by zero 방지)

        cv::Point2f center(
            static_cast<float>(m.m10 / m.m00),
            static_cast<float>(m.m01 / m.m00)
        );

        // ---- 컨투어 내부 여부 검사 (필요하면 완화) ----
        double dist = cv::pointPolygonTest(contour, center, true);
        if (dist < -1.0) continue;  // 바깥 1픽셀 이상만 제외 (0 또는 살짝 음수는 허용)
        detectedPoints.push_back({ center.x, center.y, area, contour, i });
    }

    if (detectedPoints.empty()) return false;
#define Y_GROUP_IMG
#ifdef Y_GROUP_IMG
    cv::Mat dbgBefore;
    cv::cvtColor(roiGray, dbgBefore, cv::COLOR_GRAY2BGR);

    for (auto& d : detectedPoints)
    {
        // 녹색 컨투어
        std::vector<std::vector<cv::Point>> c = { d.originalContour };
        cv::drawContours(dbgBefore, c, -1, green, 1);

        // 하늘색 점: 클러스터링 전 원시 중심점
        cv::circle(dbgBefore,
            { static_cast<int>(d.leftmost_x), static_cast<int>(d.vertical_mid_y) },
            3, teal, -1);
    }

    stageImages.push_back(dbgBefore);
#endif
#define USE_CONTOUR_CLUSTER_NEARBY

#ifdef USE_CONTOUR_CLUSTER_NEARBY
    //------------------------------------------------------------
    // 컨투어 간 근접성 기반 클러스터링 (윤곽선 거리 기준, 형태학적 병합)
    //------------------------------------------------------------
    //const double BASE_DIST_TOL = 5.0;   // 기본 컨투어 경계 간 허용 거리
    //const double X_TOL = 3.0;           // X 간 거리 필터

    std::vector<std::vector<EnvGeoData>> clusters;
    std::vector<int> clusterIndex(detectedPoints.size(), -1);
    int clusterCount = 0;

    //------------------------------------------------------------
    // (1) BFS/Union-Find 스타일 클러스터링
    //------------------------------------------------------------
    if (!detectedPoints.empty())
    {
        for (size_t i = 0; i < detectedPoints.size(); ++i)
        {
            if (clusterIndex[i] != -1) continue; // 이미 속한 클러스터

            clusters.emplace_back();
            std::queue<size_t> q;
            q.push(i);
            clusterIndex[i] = clusterCount;

            while (!q.empty())
            {
                size_t baseIdx = q.front(); q.pop();
                clusters.back().push_back(detectedPoints[baseIdx]);

                const auto& baseContour = detectedPoints[baseIdx].originalContour;
                cv::Rect baseRect = cv::boundingRect(baseContour);

                for (size_t j = 0; j < detectedPoints.size(); ++j)
                {
                    if (clusterIndex[j] != -1) continue;

                    const auto& curContour = detectedPoints[j].originalContour;
                    cv::Rect curRect = cv::boundingRect(curContour);

                    // X 필터 (세로열 분리)
                    double dx = std::abs((baseRect.x + baseRect.width / 2.0) -
                        (curRect.x + curRect.width / 2.0));
                    if (dx > X_TOL)
                        continue;

                    // 거리 허용치 (상대적 보정)
                    double hMean = (baseRect.height + curRect.height) * 0.5;
                    double distTol = std::max(BASE_DIST_TOL, hMean * 0.2);

                    // 바운딩박스 거리
                    double dyGap = std::max(0.0,
                        std::max(
                            static_cast<double>(baseRect.y) - (static_cast<double>(curRect.y) + curRect.height),
                            static_cast<double>(curRect.y) - (static_cast<double>(baseRect.y) + baseRect.height)
                        )
                    );
                    double dxGap = std::max(0.0,
                        std::max(
                            static_cast<double>(baseRect.x) - (static_cast<double>(curRect.x) + curRect.width),
                            static_cast<double>(curRect.x) - (static_cast<double>(baseRect.x) + baseRect.width)
                        )
                    );
                    double bboxDist = std::sqrt(dxGap * dxGap + dyGap * dyGap);
                    if (bboxDist > distTol * 2) continue; // 빠른 배제

                    // 컨투어 실제 최소 거리
                    double minDist = std::numeric_limits<double>::max();
                    for (const auto& p : baseContour)
                        minDist = std::min(minDist, std::abs(cv::pointPolygonTest(curContour, p, true)));
                    for (const auto& p : curContour)
                        minDist = std::min(minDist, std::abs(cv::pointPolygonTest(baseContour, p, true)));

                    if (minDist <= distTol)
                    {
                        clusterIndex[j] = clusterCount;
                        q.push(j);
                    }
                }
            }

            clusterCount++;
        }
    }

    //------------------------------------------------------------
    // (2) 각 클러스터에서 대표 컨투어 계산 (형태학적 병합)
    //------------------------------------------------------------
    std::vector<EnvGeoData> tempFiltered;

    for (const auto& cl : clusters)
    {
        if (cl.empty()) continue;

        // ⚠️ 클러스터에 컨투어가 1개뿐이라면 그대로 사용
        if (cl.size() == 1)
        {
            tempFiltered.push_back(cl.front());
            continue;
        }

        // --- 2개 이상일 경우에만 병합 수행 ---
        // 전체 ROI
        cv::Rect mergedROI = cv::boundingRect(cl[0].originalContour);
        for (size_t k = 1; k < cl.size(); ++k)
            mergedROI |= cv::boundingRect(cl[k].originalContour);

        // ROI 크기의 마스크 생성
        cv::Mat mask = cv::Mat::zeros(mergedROI.height, mergedROI.width, CV_8UC1);

        // 컨투어 그리기
        for (const auto& d : cl)
        {
            std::vector<std::vector<cv::Point>> contourShifted = { d.originalContour };
            for (auto& p : contourShifted[0])
                p -= mergedROI.tl();
            cv::drawContours(mask, contourShifted, -1, cv::Scalar(255), cv::FILLED);
        }

        // 형태학적 병합
        cv::Mat mergedMask;
        cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(5, 5)); //3,3
        cv::morphologyEx(mask, mergedMask, cv::MORPH_CLOSE, kernel, cv::Point(-1, -1), 2);//1

        // 병합된 컨투어 추출
        std::vector<std::vector<cv::Point>> mergedContours;
        cv::findContours(mergedMask, mergedContours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);

        // 형태학 병합 후 centroid 계산 (mask 전체 기준)
        cv::Moments m = cv::moments(mergedMask, true);
        if (m.m00 == 0.0) continue;

        cv::Point2f centroid(
            static_cast<float>(m.m10 / m.m00),
            static_cast<float>(m.m01 / m.m00)
        );

        // 대표 데이터 구성
        EnvGeoData merged;
        merged.leftmost_x = centroid.x + mergedROI.x;
        merged.vertical_mid_y = centroid.y + mergedROI.y;
        merged.area = m.m00;
        for (auto& p : mergedContours[0])
            p += mergedROI.tl();
        merged.originalContour = mergedContours[0];
        merged.originalContourIndex = -1;

        tempFiltered.push_back(merged);

        //if (mergedContours.empty()) continue;

        //// 가장 큰 컨투어 선택
        //size_t bestIdx = 0;
        //double bestArea = cv::contourArea(mergedContours[0]);
        //for (size_t i = 1; i < mergedContours.size(); ++i)
        //{
        //    double area = cv::contourArea(mergedContours[i]);
        //    if (area > bestArea) { bestIdx = i; bestArea = area; }
        //}

        //// 중심점 계산
        //cv::Moments m = cv::moments(mergedContours[bestIdx]);
        //if (m.m00 == 0.0) continue;

        //cv::Point2f centroid(
        //    static_cast<float>(m.m10 / m.m00),
        //    static_cast<float>(m.m01 / m.m00)
        //);

        //// 대표 EnvGeoData 구성
        //EnvGeoData merged;
        //merged.leftmost_x = centroid.x + mergedROI.x;
        //merged.vertical_mid_y = centroid.y + mergedROI.y;
        //merged.area = bestArea;

        //for (auto& p : mergedContours[bestIdx])
        //    p += mergedROI.tl();

        //merged.originalContour = mergedContours[bestIdx];
        //merged.originalContourIndex = -1;

        //tempFiltered.push_back(merged);
    }

    detectedPoints = tempFiltered;
#else
    std::vector<EnvGeoData> tempFiltered;
    tempFiltered = detectedPoints;
#endif

#ifdef Y_GROUP_IMG
    cv::Mat dbgAfter;
    cv::cvtColor(roiGray, dbgAfter, cv::COLOR_GRAY2BGR);

    for (auto& d : detectedPoints)
    {
        // 주황색 컨투어
        std::vector<std::vector<cv::Point>> c = { d.originalContour };
        cv::drawContours(dbgAfter, c, -1, green, 1);

        // 빨간 점: 클러스터링 후 대표점
        cv::circle(dbgAfter,
            { static_cast<int>(d.leftmost_x), static_cast<int>(d.vertical_mid_y) },
            3, purple, -1);
    }

	stageImages.push_back(dbgAfter);
#endif

    // 5. Contour 외부 점 제거
    std::vector<EnvGeoData> finalFiltered;
    for (auto& d : tempFiltered) {
        cv::Point2f p(d.leftmost_x, d.vertical_mid_y);
        double dist = cv::pointPolygonTest(d.originalContour, p, false);
        if (dist >= 0)
            finalFiltered.push_back(d);
    }

    if (finalFiltered.empty()) return false;

    {
        cv::Mat dbgFinal;
        cv::cvtColor(roiGray, dbgFinal, cv::COLOR_GRAY2BGR);
        for (auto& d : finalFiltered)
        {
            std::vector<std::vector<cv::Point>> c = { d.originalContour };
            cv::drawContours(dbgFinal, c, -1, yellow, 1);
            cv::circle(dbgFinal,
                { static_cast<int>(d.leftmost_x), static_cast<int>(d.vertical_mid_y) },
                3, yellow, -1);
        }
        stageImages.push_back(dbgFinal);
    }

    std::vector<double> xs;
    xs.reserve(finalFiltered.size()); // 성능 최적화 (선택)
#define USE_X_FILTER_BEST_K

    for (auto& d : finalFiltered) {
        d.leftmost_x = std::round(d.leftmost_x);  // float 오차 제거
        xs.push_back(d.leftmost_x);                // MAD 계산용 수집
    }

    if (xs.empty()) return false;
#ifdef USE_X_FILTER_BEST_K
    // 기존 min/max 구문 유지 (호환용)
    //double xTol = 2.0;// cfg.xTolerance;  // 새 파라미터 (예: 2~4 픽셀 권장)
    double bestK = 0.0;
    int bestCount = 0;

    if (xs.empty()) return false;
    // 1) 실제 점들만 대상으로 최빈 X값 탐색
    for (double xi : xs)
    {
        int count = 0;
        for (double xj : xs)
            if (std::abs(xj - xi) <= xTol)
                count++;

        if (count > bestCount) {
            bestCount = count;
            bestK = xi; // 실제 존재하는 점의 X값을 그대로 사용
        }
    }

    // 2) 최적 직선(bestK) 기준으로 필터링
    std::vector<EnvGeoData> filteredByX, filteredOutX;
    for (auto& d : finalFiltered)
    {
        if (std::abs(d.leftmost_x - bestK) <= xTol)
            filteredByX.push_back(d);
        else
            filteredOutX.push_back(d);
    }

    // 3) 대표 ref_point 설정
    for each (auto& d in filteredByX)
    {
        if (std::abs(d.leftmost_x - bestK) <= 1.0) {
            xF.ref_point =
                cv::Point(static_cast<int>(d.leftmost_x + roi.x),
                    static_cast<int>(d.vertical_mid_y + roi.y));
            break;
        }
    }

    xF.interval = 0.0;     // 사용하지 않음 (유지용)
    xF.tolerance = xTol;
    xF.filtered_out.clear();
    for (const auto& d : filteredOutX) {
        xF.filtered_out.push_back(
            cv::Point(static_cast<int>(d.leftmost_x + roi.x),
                static_cast<int>(d.vertical_mid_y + roi.y)));
    }
#else
#   error "Define one of USE_X_FILTER_MAD or USE_X_FILTER_BEST_K"
#endif
    
    {
        cv::Mat dbgMad;
        cv::cvtColor(roiGray, dbgMad, cv::COLOR_GRAY2BGR);
        for (auto& d : filteredByX)
            cv::circle(dbgMad, { (int)d.leftmost_x, (int)d.vertical_mid_y }, 3, orange, -1);
        for (auto& d : filteredOutX)
            cv::circle(dbgMad, { (int)d.leftmost_x, (int)d.vertical_mid_y }, 3, orange, 1);
        cv::circle(dbgMad, xF.ref_point - cv::Point(roi.x, roi.y), 5, HotPink, 1);
        
        stageImages.push_back(dbgMad);
    }
    // 7. Y 간격 필터링
    std::vector<EnvGeoData> filteredByY, filteredOutY;
    //const double targetY = cfg.targetYInterval;
    //const double yTol2 = cfg.yIntervalTolerance;

#define USE_Y_FILTER_PHASE_BEST

#ifdef USE_Y_FILTER_PHASE_BEST
    //------------------------------------------------------------
    // Y Phase 기반 최빈값 필터링 (X_BEST_K 구조 동일)
    //------------------------------------------------------------
    double bestPhase = 0.0;
    int bestCountY = 0;

    if (finalFiltered.empty()) return false;

    // 1) phase 배열 생성 (mod 적용)
    std::vector<double> phases;
    phases.reserve(finalFiltered.size());
    for (auto& d : finalFiltered) {
        double p = std::fmod(d.vertical_mid_y, targetY);
        if (p < 0) p += targetY;
        phases.push_back(p);
    }

    // 2) 실제 점들만 대상으로 최빈 phase 탐색 (X 구조 동일)
    for (double pi : phases)
    {
        int count = 0;
        for (double pj : phases)
            if (std::abs(pj - pi) <= yTol)
                count++;

        if (count > bestCountY) {
            bestCountY = count;
            bestPhase = pi;  // 실제 존재하는 phase값 사용
        }
    }

    // 3) 필터링
    for (size_t i = 0; i < finalFiltered.size(); ++i)
    {
        double phase = phases[i];
        if (std::abs(phase - bestPhase) <= yTol)
            filteredByY.push_back(finalFiltered[i]);
        else
            filteredOutY.push_back(finalFiltered[i]);
    }

    // 4) 대표 ref_point 설정
    for each (auto& d in filteredByY)
    {
        double p = std::fmod(d.vertical_mid_y, targetY);
        if (p < 0) p += targetY;
        if (std::abs(p - bestPhase) <= 0.5) {
            yF.ref_point =
                cv::Point(static_cast<int>(d.leftmost_x + roi.x),
                    static_cast<int>(d.vertical_mid_y + roi.y));
            break;
        }
    }

    // 5) 결과 저장
    yF.interval = targetY;
    yF.tolerance = yTol;
    yF.filtered_out.clear();
    for (const auto& d : filteredOutY) {
        yF.filtered_out.push_back(
            cv::Point(static_cast<int>(d.leftmost_x + roi.x),
                static_cast<int>(d.vertical_mid_y + roi.y)));
    }
#else
#   error "Define one of USE_Y_FILTER_PAIRWISE or USE_Y_FILTER_PHASE_HIST"
#endif

    {
        cv::Mat dbgPhase;
        cv::cvtColor(roiGray, dbgPhase, cv::COLOR_GRAY2BGR);
        for (auto& d : filteredByY)
            cv::circle(dbgPhase, { (int)d.leftmost_x, (int)d.vertical_mid_y }, 3, blue , -1);
        for (auto& d : filteredOutY)
            cv::circle(dbgPhase, { (int)d.leftmost_x, (int)d.vertical_mid_y }, 3, blue, 2);
        cv::circle(dbgPhase, yF.ref_point - cv::Point(roi.x, roi.y), 5, cyan, 1);
        stageImages.push_back(dbgPhase);
    }
    // 8. X/Y 필터 조합
    std::vector<EnvGeoData> finalResult;
    bool useX = cfg.enableXFilter;
    bool useY = cfg.enableYFilter;

    if (useX && useY) {
        for (auto& dx : filteredByX)
        {
            for (auto& dy : filteredByY)
            {
                if (std::abs(dx.leftmost_x - dy.leftmost_x) <= 1.0 &&
                    std::abs(dx.vertical_mid_y - dy.vertical_mid_y) <= 1.0)
                {
                    finalResult.push_back(dx);
                    break; // 중복 방지
                }
            }
        }
    }
    else if (useX) {
        finalResult = filteredByX;
    }
    else if (useY) {
        finalResult = filteredByY;
    }
    else {
        finalResult = finalFiltered;
    }

    /*out.result.yFilter.interval = targetY;
    out.result.yFilter.tolerance = yTol;*/

    // Y 필터 관련 결과 저장 추가 (여기 삽입)
    /*out.result.yFilter.filtered_out.clear();
    for (const auto& d : filteredOutY) {
        out.result.yFilter.filtered_out.push_back(
            cv::Point(static_cast<int>(d.leftmost_x + roi.x),
                static_cast<int>(d.vertical_mid_y + roi.y)));
    }*/

    // 9. 결과 구성
    if (finalResult.empty()) return false;

    {
        cv::Mat dbgFinalResult;
        cv::cvtColor(roiGray, dbgFinalResult, cv::COLOR_GRAY2BGR);
        for (auto& d : finalResult)
            cv::circle(dbgFinalResult, { (int)d.leftmost_x, (int)d.vertical_mid_y }, 3, red, -1);
        stageImages.push_back(dbgFinalResult);
    }

    for (auto& img : stageImages)
    {
        if (img.channels() == 1)
            cv::cvtColor(img, img, cv::COLOR_GRAY2BGR);
        cv::resize(img, img, stageImages[0].size());
    }
    cv::Mat combined;
    cv::hconcat(stageImages, combined);

    showAndSaveImage("EnvGeo_AllStages_Threshold:" + std::to_string((int)perThreshold)+"," + to_string(morphMode), combined);

	// 최종 결과 저장
    out.threshold = perThreshold;
	out.morphMode = morphMode;
    out.detectedPoints = detectedPoints;

    out.result.xFilter = xF;
    out.result.yFilter = yF;

    out.result.findPoints.clear();
    out.result.finalPoints.clear();

    for (auto& d : detectedPoints)
        out.result.findPoints.emplace_back(d.leftmost_x + roi.x, d.vertical_mid_y + roi.y);
    for (auto& d : finalResult)
        out.result.finalPoints.emplace_back(d.leftmost_x + roi.x, d.vertical_mid_y + roi.y);

    return true;
}

// =========================================================
// 드로잉 함수 (최종 1회만)
// =========================================================
static void DrawEnvGeoTrial(cv::Mat& srcImg, const cv::Rect& roi, const TrialOutcome& t)
{
    for (auto& d : t.detectedPoints) {
        std::vector<std::vector<cv::Point>> c = { d.originalContour };
        for (auto& p : c[0]) p += cv::Point(roi.x, roi.y);
        cv::drawContours(srcImg, c, -1, GreenA, 1);
    }

    /*
    for (auto& d : t.filteredOutXPoints)
        cv::circle(srcImg, { int(d.leftmost_x + roi.x), int(d.vertical_mid_y + roi.y) }, 2, OrangeA, -1);

    for (auto& d : t.filteredOutYPoints)
        cv::circle(srcImg, { int(d.leftmost_x + roi.x), int(d.vertical_mid_y + roi.y) }, 2, BlueA, -1);

    for (auto& d : t.finalResultPoints)
        cv::circle(srcImg, { int(d.leftmost_x + roi.x), int(d.vertical_mid_y + roi.y) }, 2, RedA, -1);
        
    if (t.bestRefPoint.area > 0)
        cv::circle(srcImg, { int(t.bestRefPoint.leftmost_x + roi.x), int(t.bestRefPoint.vertical_mid_y + roi.y) }, 3, CyanA, -1);
    */

    for (auto& d : t.result.xFilter.filtered_out)
        cv::circle(srcImg, { d.x, d.y }, 2, OrangeA, -1);

    for (auto& d : t.result.yFilter.filtered_out)
        cv::circle(srcImg, { d.x, d.y }, 2, BlueA, -1);

    for (auto& d : t.result.finalPoints)
        cv::circle(srcImg, { d.x, d.y }, 2, RedA, -1);

    //if (t.bestRefPoint.area > 0)
    cv::circle(srcImg, { t.result.xFilter.ref_point.x, t.result.xFilter.ref_point.y }, 4, BrightOrangeA, 2);
    cv::circle(srcImg, { t.result.yFilter.ref_point.x, t.result.yFilter.ref_point.y }, 4, CyanA, 2);

    cv::rectangle(srcImg, roi, YellowA, 1);

    // --- 간단한 cnt 표시 (512x512 기준, 오른쪽 아래 구석) ---
    int cnt = static_cast<int>(t.result.finalPoints.size());
    std::string text = "cnt:" + std::to_string(cnt);
    cv::putText(srcImg, text, cv::Point(srcImg.cols - 100 , srcImg.rows - 10), cv::FONT_HERSHEY_SIMPLEX, 0.8, red, 2);

    showAndSaveImage("EnvGeo_End", srcImg);
}

void LogEnvGeoHeader()
{
    Logger::Information("---- EnvGeoResult Summary ----");
    Logger::Information("Idx | findPts | finalPts | median_x | mad_x | target_y_interval | y_tolerance");
    Logger::Information("--------------------------------------------------------------------------");
}

// 2) 인덱스 포함 단일 행 출력
void LogEnvGeoRow(const TrialOutcome& t, size_t idx)
{
    const auto& e = t.result;

    Logger::Information(
        "{Idx,3} | {FindPts,7} | {FinalPts,9} | {Median,9:F3} | {MAD,6:F3} | {Target,17:F2} | {Tol,11:F2}",
        static_cast<int>(idx),
        e.findPoints.size(),
        e.finalPoints.size(),
        e.xFilter.ref_point.x,
        e.xFilter.tolerance,
        e.yFilter.interval,
        e.yFilter.tolerance
    );
}

void LogEnvGeoSummary(const std::vector<TrialOutcome>& trials)
{
    if (trials.empty()) {
        Logger::Information("No EnvGeoResult trials to log.");
        return;
    }

    using namespace System;
    using namespace System::Text;

    for (int tIndex = 0; tIndex < static_cast<int>(trials.size()); ++tIndex)
    {
        const auto& r = trials[tIndex].result;
        const double refX = r.xFilter.ref_point.x;
        const double xTol = r.xFilter.tolerance;
        const double refY = r.yFilter.ref_point.y;
        const double interval = r.yFilter.interval;
        const double yTol = r.yFilter.tolerance;

        const auto& pts = r.findPoints;
        if (pts.empty()) continue;

        StringBuilder^ sb = gcnew StringBuilder(2048);
        sb->AppendLine("");
        sb->AppendLine("---- EnvGeoResult Summary ----");
        sb->AppendFormat("\nThreshold :{0},  ({1})\n", trials[tIndex].threshold, gcnew String(to_string(trials[tIndex].morphMode).c_str()));
        sb->AppendFormat("X-Filter : refX={0:F1}, tol={1:F1}\n", refX, xTol);
        sb->AppendFormat("Y-Filter : refY={0:F1}, interval={1:F1}, tol={2:F1}\n", refY, interval, yTol);
        sb->AppendLine("--------------------------------------------------------------------");
        sb->AppendLine(" No |    X     | ΔX(refX) | X-OK |    Y     | Phase(rem) |  Y-OK");
        sb->AppendLine("--------------------------------------------------------------------");

        int idx = 1;
        for (const auto& p : pts)
        {
            // ΔX 계산
            double dX = p.x - refX;
            bool xOK = (std::abs(dX) <= xTol);

            // ΔY 및 Phase 계산
            double diffY = std::fabs(p.y - refY);
            double phase = std::fmod(diffY, interval);
            if (phase < 0) phase += interval;
            double delta = std::min(phase, interval - phase);
            bool yOK = (delta <= yTol);

            // 기준점 여부 표시
            bool isRefX = (std::fabs(p.x - refX) < 0.5);
            bool isRefY = (std::fabs(p.y - refY) < 0.5);

            sb->AppendFormat("{0,3} | {1,7:F1}{2} | {3,8:F1} | {4,4} | {5,7:F1}{6} | {7,10:F2} | {8,4}\n",
                idx++,
                p.x, (isRefX ? " *" : "  "),
                dX,
                xOK ? "OK" : "NG",
                p.y, (isRefY ? " *" : "  "),
                delta,
                yOK ? "OK" : "NG");
        }

        sb->AppendLine("------------------------------------------------------------");
        Logger::Information("{0}", sb->ToString());
    }
}

void LogEnvGeoSummary2(const std::vector<TrialOutcome>& trials)
{
    if (trials.empty())
    {
        Logger::Information("No EnvGeoResult trials to log.");
        return;
    }

    using namespace System;
    using namespace System::Text;

    StringBuilder^ sb = gcnew StringBuilder(1024);

    sb->AppendLine("");
    sb->AppendLine("---- EnvGeoResult Summary ----");
    sb->AppendLine("Idx | findPts | finalPts | median_x | mad_x | target_y_interval | y_tolerance");
    sb->AppendLine("--------------------------------------------------------------------------");

    for (int i = 0; i < static_cast<int>(trials.size()); ++i)
    {
        const auto& e = trials[i].result;

        sb->AppendFormat(
            "{0,3} | {1,7} | {2,9} | {3,9:F3} | {4,6:F3} | {5,17:F2} | {6,11:F2}\n",
            i,
            static_cast<int>(e.findPoints.size()),
            static_cast<int>(e.finalPoints.size()),
            e.xFilter.ref_point.x,
            e.xFilter.tolerance,
            e.yFilter.interval,
            e.yFilter.tolerance
        );
    }

    sb->AppendLine("--------------------------------------------------------------------------");

    // 한 번만 출력 (여러 줄을 한 이벤트로)
    Logger::Information("{0}", sb->ToString());
}

inline void LogYIntervalSummary(const std::vector<EnvGeoData>& allPoints,
    double refY, double targetY, double tol)
{
    if (allPoints.empty())
    {
        Logger::Information("No Y-interval points to display.");
        return;
    }

    using namespace System;
    using namespace System::Text;

    StringBuilder^ sb = gcnew StringBuilder(1024);

    sb->AppendFormat("\nY-Interval Verification (refY={0:F1}, target={1:F1}, tol={2:F1})\n",
        refY, targetY, tol);
    sb->AppendLine("-------------------------------------------------------------");
    sb->AppendLine(" No |    X   |    Y   | ΔY(refY) | Remainder |  Status");
    sb->AppendLine("-------------------------------------------------------------");

    int idx = 1;
    for (const auto& d : allPoints)
    {
        double dY = d.vertical_mid_y - refY;
        double rem = std::fmod(std::abs(dY), targetY);
        if (rem < 0) rem += targetY;
        bool ok = (std::abs(rem) < tol) || (std::abs(rem - targetY) < tol);

        sb->AppendFormat("{0,3} | {1,6:F1} | {2,6:F1} | {3,8:F1} | {4,10:F2} | {5}\n",
            idx++,
            d.leftmost_x,
            d.vertical_mid_y,
            dY,
            rem,
            ok ? "OK" : "OUT");
    }

    sb->AppendLine("-------------------------------------------------------------");

    // 🚀 한 번만 출력 (전체 블록)
    Logger::Information("{0}", sb->ToString());
}

enum class AxisMode { X, Y };

double ComputePhaseError(const EnvGeoResult& r, AxisMode mode)
{
    const auto& points = r.finalPoints;
    if (points.empty())
        return std::numeric_limits<double>::infinity();

    double ref = 0.0;
    double interval = 0.0;

    if (mode == AxisMode::X) {
        ref = r.xFilter.ref_point.x;
        interval = r.xFilter.interval;
    }
    else {
        ref = r.yFilter.ref_point.y;
        interval = r.yFilter.interval;
    }

    if (interval <= 0.0)
        return std::numeric_limits<double>::infinity();

    double sumErr = 0.0;
    for (const auto& p : points)
    {
        double coord = (mode == AxisMode::X) ? p.x : p.y;
        double diff = std::fabs(coord - ref);
        double rem = std::fmod(diff, interval);
        double err = std::min(rem, interval - rem);  // 1과 29 동일 취급
        sumErr += err;
    }

    return sumErr / points.size();  // 평균 오차
}


TrialOutcome SelectBestTrial(const std::vector<TrialOutcome>& trials)
{
    TrialOutcome best = trials[0];

    for (size_t i = 1; i < trials.size(); ++i)
    {
        const auto& t = trials[i];

        int currCount = static_cast<int>(t.result.finalPoints.size());
        int bestCount = static_cast<int>(best.result.finalPoints.size());

        if (currCount > bestCount) {
            best = t;
            continue;
        }

        if (currCount == bestCount)
        {
            double bestPhaseErrY = ComputePhaseError(best.result, AxisMode::Y);
            double currPhaseErrY = ComputePhaseError(t.result, AxisMode::Y);

            if (currPhaseErrY < bestPhaseErrY) {
                best = t;
                continue;
            }

            if (std::fabs(currPhaseErrY - bestPhaseErrY) < 1e-6)
            {
                double bestPhaseErrX = ComputePhaseError(best.result, AxisMode::X);
                double currPhaseErrX = ComputePhaseError(t.result, AxisMode::X);

                if (currPhaseErrX < bestPhaseErrX) {
                    best = t;
                    continue;
                }
            }
        }
    }

    return best;
}

void MyOpenCVWrapper::EnvGeoInspection(cv::Mat& srcImg, EnvGeoResult& result)
{
    const auto& cfg = ConfigManager::getInstance().getInspectionParams().envGeo;

    auto prep = CalcHistBasedThreshold(srcImg, cfg.roi);
    auto thresholds = GenerateThresholds(prep.baseThreshold, 10.0, 5);

    std::vector<TrialOutcome> trials;

    for (double t : thresholds) {
        for (MorphMode mode : AllMorphModes)
        {
            TrialOutcome temp;
            if (RunEnvGeoTrial(temp, prep.roiGray, prep.roi, t, cfg, mode))  // mode 전달
            {
                temp.morphMode = mode; // (선택사항) 모드 기록용
                trials.push_back(temp);
            }
        }
    }

    if (trials.empty()) {
        Logger::Warning("No valid EnvGeo trials found.");
        return;
    }
    else
    {
		LogEnvGeoSummary(trials);
    }

    const TrialOutcome& chosen = SelectBestTrial(trials);//trials.at(5);// trials.front();
    
    Logger::Information(
        "Morphology = {0}, Threshold = {1}, Final Points = {2}",
        gcnew System::String(to_string(chosen.morphMode).c_str()),
        chosen.threshold,
        chosen.result.finalPoints.size()
    );

    LogEnvGeoSummary({ chosen });

    DrawEnvGeoTrial(srcImg, prep.roi, chosen);
    result = chosen.result;

    if (result.xFilter.ref_point == cv::Point(0, 0))
    {
        int temp = 1;
    }
}

void MyOpenCVWrapper::EnvGeoInspection(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr resultBuffer, System::IntPtr textBuffer)
{
    // 초기화
    EnvGeoResult geoResult;
    memset(resultBuffer.ToPointer(), 0, imageWidth * imageHeight * 4);

    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) {
        Logger::Error("Image loading failed: Input image is empty. Image Size: {0}x{1}", imageHeight, imageWidth);
        return;
    }

    Logger::Information(
        "Image size: {0}x{1}, Channels: {2}",
        imageWidth, imageHeight, inputImage.channels()
    );

    cv::Mat resultImage = inputImage.clone();

    cv::Mat gray;
    cv::cvtColor(resultImage, gray, cv::COLOR_BGRA2GRAY);

    // 검사 실행 (여기서 resultImage가 업데이트 됨)
    EnvGeoInspection(resultImage, geoResult);

    // JSON 결과 직렬화
    nlohmann::json j = geoResult;
    std::string finalText = j.dump(2); // 가독성을 위해 2칸 들여쓰기 추가
    Logger::Information("\n{0}",gcnew System::String(finalText.c_str()));
    // 복사
    memcpy(textBuffer.ToPointer(), finalText.c_str(), finalText.size() + 1);
    memcpy(resultBuffer.ToPointer(), resultImage.data, resultImage.total() * resultImage.elemSize());
}