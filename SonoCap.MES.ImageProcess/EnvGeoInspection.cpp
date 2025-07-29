#include "EnvGeoInspection.h"

// 표준 편차 계산 헬퍼 함수
double calculateStandardDeviation(const std::vector<double>& values) {
    if (values.empty() || values.size() == 1) { // 요소가 0 또는 1개일 경우 표준 편차는 0
        return 0.0;
    }
    double sum = std::accumulate(values.begin(), values.end(), 0.0);
    double mean = sum / values.size();
    double sq_diff_sum = 0.0;
    for (double val : values) {
        sq_diff_sum += (val - mean) * (val - mean);
    }
    // 샘플 표준 편차 (N-1) 사용, 모집단 표준 편차 (N)를 원하면 values.size() 사용
    return std::sqrt(sq_diff_sum / (values.size() - 1));
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
    Logger::Information(gcnew System::String(finalText.c_str()));
    // 복사
    memcpy(textBuffer.ToPointer(), finalText.c_str(), finalText.size() + 1);
    memcpy(resultBuffer.ToPointer(), resultImage.data, resultImage.total() * resultImage.elemSize());
}

void MyOpenCVWrapper::EnvGeoInspection2(cv::Mat& roiImage, EnvGeoResult2& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) {
        return;
    }

    const ConfigManager& config = ConfigManager::getInstance();
    const InspectionParams::ResParams& resConfig = config.getInspectionParams().res;

    const int drMin = resConfig.drMin;
    const int drMax = resConfig.drMax;
    const double minContourArea = resConfig.minContourArea;

    cv::Mat gray;
    cv::cvtColor(roiImage, gray, cv::COLOR_BGRA2GRAY);

    showAndThreshold("Res_DR_Adjust", gray);

    ApplyLinearDRClip(gray, gray, drMin, drMax, true);
    showAndSaveImage("Res_DR_Clipped", gray);

    cv::Rect roi(110, 0, 40, roiImage.rows - 1);

    if (roi.x < 0 || roi.y < 0 || roi.x + roi.width > gray.cols || roi.y + roi.height > gray.rows) {
        Logger::Error("--- ROI is out of image bounds ---");
        return;
    }

    cv::Mat roiGray = gray(roi);
    cv::Mat binary;
    cv::threshold(roiGray, binary, 100, 255, cv::THRESH_BINARY);

    cv::Mat eroded, restored;
    cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));
    cv::erode(binary, eroded, kernel);
    cv::dilate(eroded, restored, kernel);
    showAndSaveImage("Res_Morphology", restored);

    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(restored, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);

    std::vector<ContourData> detectedPoints; // 모든 유효 윤곽선에서 추출된 점들을 저장
    result.pointsOnLeftEdge.clear(); // EnvGeoResult에 결과 저장 시작 전 초기화

    // ----------------------------------------------------
    // Y값 유사성 판단 기준 (오차 허용 범위)
    const float yTolerance = 5.0f;
    // ----------------------------------------------------

    // 모든 유효 윤곽선 데이터 수집 및 새로운 좌표 계산
    for (int i = 0; i < contours.size(); ++i) {
        const auto& contour = contours[i];
        double currentArea = cv::contourArea(contour);
        if (currentArea < minContourArea) {
            continue;
        }

        cv::Moments m = cv::moments(contour);
        if (m.m00 == 0) {
            continue;
        }

        // 윤곽선의 가장 왼쪽 x 좌표와 위-아래 중앙 y 좌표 계산
        float min_x_local = std::numeric_limits<float>::max();
        float max_y_local = std::numeric_limits<float>::min();
        float min_y_local = std::numeric_limits<float>::max();

        for (const auto& p : contour) {
            if (p.x < min_x_local) {
                min_x_local = static_cast<float>(p.x);
            }
            if (p.y > max_y_local) {
                max_y_local = static_cast<float>(p.y);
            }
            if (p.y < min_y_local) {
                min_y_local = static_cast<float>(p.y);
            }
        }

        float leftmost_x_abs = min_x_local + roi.x;
        float vertical_mid_y_abs = (min_y_local + max_y_local) / 2.0f + roi.y;

        // 윤곽선 추출 후 즉시, 검출된 점이 원본 윤곽선 외부에 있는지 확인 (ROI 기준)
        cv::Point2f extractedPoint_roi_relative(min_x_local, vertical_mid_y_abs);
        double distanceToContour = cv::pointPolygonTest(contour, extractedPoint_roi_relative, false);

        if (distanceToContour < 0) { // 점이 윤곽선 외부에 있는 경우
            Logger::Information("Pre-filter: Removed point (outside contour at extraction). Original Contour Index: {0}, Point: ({1}, {2})",
                i, leftmost_x_abs, vertical_mid_y_abs);
            continue; // 이 점은 건너뛰고 다음 윤곽선으로
        }

        detectedPoints.push_back({
            cv::Point2f(),          // center_abs
            0.0,                    // quality
            currentArea,            // area
            contour,                // originalContour_rel (roiGray 기준의 상대 윤곽선 데이터)
            0.0,                    // distance_from_ref
            0.0,                    // angle_from_ref_rad
            0.0,                    // angle_from_ref_deg
            leftmost_x_abs,         // leftmost_x_abs (전체 이미지 기준)
            vertical_mid_y_abs,     // vertical_mid_y_abs (전체 이미지 기준)
            i                       // originalContourIndex
            });
    }

    // ----------------------------------------------------
    // Y값이 비슷할 경우 X값이 큰 데이터를 지우고, X값이 작은 데이터만 남기기
    // ----------------------------------------------------

    // 1. Y 좌표 기준으로 정렬
    std::sort(detectedPoints.begin(), detectedPoints.end(),
        [](const ContourData& a, const ContourData& b) {
            return a.vertical_mid_y_abs < b.vertical_mid_y_abs;
        });

    std::vector<ContourData> finalFilteredPoints_temp; // 임시 저장용

    if (!detectedPoints.empty()) {
        std::vector<ContourData> currentGroup;
        currentGroup.push_back(detectedPoints[0]);

        for (size_t i = 1; i < detectedPoints.size(); ++i) {
            const auto& currentPoint = detectedPoints[i];
            const auto& firstInGroup = currentGroup[0]; // 그룹의 첫 번째 점 (Y 기준)

            // 현재 점의 Y값이 그룹의 첫 번째 점의 Y값과 yTolerance 이내인지 확인
            if (std::abs(currentPoint.vertical_mid_y_abs - firstInGroup.vertical_mid_y_abs) <= yTolerance) {
                currentGroup.push_back(currentPoint);
            }
            else {
                // 새로운 그룹이 시작되므로, 이전 그룹을 처리
                if (!currentGroup.empty()) {
                    // 그룹 내에서 leftmost_x_abs가 가장 작은 점을 찾음
                    auto minX_it = std::min_element(currentGroup.begin(), currentGroup.end(),
                        [](const ContourData& a, const ContourData& b) {
                            return a.leftmost_x_abs < b.leftmost_x_abs;
                        });
                    // 가장 X값이 작은 점을 최종 필터링된 데이터에 추가
                    finalFilteredPoints_temp.push_back(*minX_it);

                    // 나머지 점들은 제거된 것으로 로그 출력
                    for (const auto& removedPoint : currentGroup) {
                        if (&removedPoint != &(*minX_it)) {
                            Logger::Information("Removed point (Y-similar, X-larger). Original Contour Index: {0}, Point: ({1}, {2})",
                                removedPoint.originalContourIndex, removedPoint.leftmost_x_abs, removedPoint.vertical_mid_y_abs);
                        }
                    }
                }
                // 새 그룹 시작
                currentGroup.clear();
                currentGroup.push_back(currentPoint);
            }
        }
        // 마지막 그룹 처리
        if (!currentGroup.empty()) {
            auto minX_it = std::min_element(currentGroup.begin(), currentGroup.end(),
                [](const ContourData& a, const ContourData& b) {
                    return a.leftmost_x_abs < b.leftmost_x_abs;
                });
            finalFilteredPoints_temp.push_back(*minX_it);
            for (const auto& removedPoint : currentGroup) {
                if (&removedPoint != &(*minX_it)) {
                    Logger::Information("Removed point (Y-similar, X-larger). Original Contour Index: {0}, Point: ({1}, {2})",
                        removedPoint.originalContourIndex, removedPoint.leftmost_x_abs, removedPoint.vertical_mid_y_abs);
                }
            }
        }
    }

    // ★ 최종 필터링된 점들 중, 해당 점이 원본 윤곽선을 벗어나면 제거
    std::vector<ContourData> finalFilteredPoints; // 최종 결과
    for (const auto& data : finalFilteredPoints_temp) {
        // 점의 절대 좌표를 ROI 기준 상대 좌표로 변환하여 pointPolygonTest에 사용
        cv::Point2f point_roi_relative(data.leftmost_x_abs - roi.x, data.vertical_mid_y_abs - roi.y);

        // originalContour_rel 필드를 사용하여 점이 윤곽선 내부에 있는지 확인
        double distanceToOriginalContour = cv::pointPolygonTest(data.originalContour_rel, point_roi_relative, false);

        if (distanceToOriginalContour < 0) { // 점이 원본 윤곽선 외부에 있는 경우
            Logger::Information("Final Filter: Removed point (outside original contour). Original Contour Index: {0}, Point: ({1}, {2})",
                data.originalContourIndex, data.leftmost_x_abs, data.vertical_mid_y_abs);
        }
        else {
            finalFilteredPoints.push_back(data); // 유효한 점만 추가
        }
    }

    // ----------------------------------------------------
    // ★ 점 간 관계 수치화 시작 (Y 간격의 새로운 지표 반영)
    // ----------------------------------------------------
    // EnvGeoResult 필드 초기화 (필요한 경우 새 필드 추가)
    result.x_uniformity_std_dev = 0.0;
    // 이전 필드 대신 새로운 간격 오차 정보를 담을 필드를 EnvGeoResult에 추가해야 합니다.
    // 예를 들어, std::vector<IntervalErrorData> y_interval_errors; 같은 필드
    // 여기서는 임시적으로 Logger로 출력하겠습니다.

    if (finalFilteredPoints.size() > 1) {
        const double targetYInterval = 30.0; // 목표 기본 간격

        // 1. X 좌표 균일성 (X Uniformity) 계산 (기존 로직 유지)
        std::vector<double> x_coords;
        for (const auto& p : finalFilteredPoints) {
            x_coords.push_back(p.leftmost_x_abs);
        }
        if (x_coords.size() > 1) {
            result.x_uniformity_std_dev = calculateStandardDeviation(x_coords);
        }
        Logger::Information("X Uniformity (Standard Deviation): {0}", result.x_uniformity_std_dev);

        // 2. Y 좌표 간격 및 개별 오차 측정 (새로운 로직)
        std::vector<double> y_intervals;
        for (size_t i = 0; i < finalFilteredPoints.size() - 1; ++i) {
            double currentInterval = finalFilteredPoints[i + 1].vertical_mid_y_abs - finalFilteredPoints[i].vertical_mid_y_abs;

            // 가장 가까운 targetYInterval의 정수배를 찾기
            double nearest_n_double = std::round(currentInterval / targetYInterval);
            int N = std::max(1, static_cast<int>(nearest_n_double));

            double nearest_pattern_multiple = static_cast<double>(N) * targetYInterval;
            double error_to_pattern = std::fabs(currentInterval - nearest_pattern_multiple);

            // 결과 로깅
            Logger::Information(
                "Interval between Point {0} and {1}: {2}px (Nearest pattern: {3} x {4}px, Error: {5}px)",
                (int)i, (int)i + 1, currentInterval, N, targetYInterval, error_to_pattern
            );

            // EnvGeoResult에 결과 저장 (구조체 수정이 필요)
            // 예를 들어:
            result.y_interval_errors.push_back({
                (int)i,
                (int)i + 1,
                currentInterval,
                nearest_pattern_multiple,
                error_to_pattern
            });
        }
    }
    else {
        Logger::Warning("Not enough points ({0}) to calculate point relationship metrics.", (int)finalFilteredPoints.size());
    }
    // ----------------------------------------------------
    // ★ 점 간 관계 수치화 종료
    // ----------------------------------------------------

    // 필터링된 점들과 해당 윤곽선을 이미지에 그리기
    for (const auto& data : finalFilteredPoints) {
        result.pointsOnLeftEdge.push_back(cv::Point2f(data.leftmost_x_abs, data.vertical_mid_y_abs));

        // 윤곽선 그리기 (녹색)
        // originalContour_rel은 roiGray 기준으로 되어 있으므로 offset을 더해줍니다.
        std::vector<std::vector<cv::Point>> contoursToDraw;
        std::vector<cv::Point> shiftedContour;
        for (const auto& p : data.originalContour_rel) { // originalContour_rel 사용
            shiftedContour.push_back(cv::Point(p.x + roi.x, p.y + roi.y));
        }
        contoursToDraw.push_back(shiftedContour);
        cv::drawContours(roiImage, contoursToDraw, -1, GreenA, 1);

        // 점 그리기 (빨간색, 윤곽선 위에 그려져야 더 잘 보임)
        cv::circle(roiImage, cv::Point2f(data.leftmost_x_abs, data.vertical_mid_y_abs), 2, RedA, -1);
    }

    Logger::Information("--- Final Filtered Points (Y-similar, X-smallest) ---");
    for (size_t i = 0; i < finalFilteredPoints.size(); ++i) {
        const auto& data = finalFilteredPoints[i];
        Logger::Information(
            "Final Point {0}: Original Contour Index {1}, Point: ({2}, {3}), Original Area: {4}",
            (int)i,
            data.originalContourIndex,
            data.leftmost_x_abs, data.vertical_mid_y_abs,
            data.area
        );
    }
    Logger::Information("----------------------------------");

    showAndSaveImage("Geo_Env_End", roiImage);
}

// 개선된 오프셋 계산 헬퍼 함수
double calculateBestOffset(const std::vector<ContourData>& points, double targetInterval) {
    if (points.empty()) {
        return 0.0;
    }

    std::map<int, int> offsetBins; // 오프셋 후보를 1px 단위로 묶기 위한 맵
    for (const auto& p : points) {
        double offset = std::fmod(p.vertical_mid_y_abs, targetInterval);
        int bin = static_cast<int>(std::round(offset));
        offsetBins[bin]++;
    }

    int maxCount = 0;
    std::vector<int> bestBins;
    for (const auto& pair : offsetBins) {
        if (pair.second > maxCount) {
            maxCount = pair.second;
            bestBins.clear();
            bestBins.push_back(pair.first);
        }
        else if (pair.second == maxCount) {
            bestBins.push_back(pair.first);
        }
    }

    if (bestBins.empty()) {
        return 0.0;
    }

    double totalOffset = 0.0;
    for (int bin : bestBins) {
        totalOffset += bin;
    }

    return totalOffset / bestBins.size();
}

void MyOpenCVWrapper::EnvGeoInspection3(cv::Mat& roiImage, EnvGeoResult2& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) {
        return;
    }

    const ConfigManager& config = ConfigManager::getInstance();
    const InspectionParams::ResParams& resConfig = config.getInspectionParams().res;

    const int drMin = resConfig.drMin;
    const int drMax = resConfig.drMax;

    const double minContourArea = resConfig.minContourArea;

    cv::Mat gray;
    cv::cvtColor(roiImage, gray, cv::COLOR_BGRA2GRAY);

    showAndThreshold("Res_DR_Adjust", gray);

    ApplyLinearDRClip(gray, gray, drMin, drMax, true);
    showAndSaveImage("Res_DR_Clipped", gray);

    cv::Rect roi(110, 0, 40, roiImage.rows - 1);

    if (roi.x < 0 || roi.y < 0 || roi.x + roi.width > gray.cols || roi.y + roi.height > gray.rows) {
        Logger::Error("--- ROI is out of image bounds ---");
        return;
    }

    cv::Mat roiGray = gray(roi);
    cv::Mat binary;
    cv::threshold(roiGray, binary, 100, 255, cv::THRESH_BINARY);

    cv::Mat temp, restored;
    cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));

    // 1. 열림 연산으로 작은 노이즈 제거
    cv::morphologyEx(binary, temp, cv::MORPH_OPEN, kernel);
    showAndSaveImage("Res_Opening", temp);

    // 2. 닫힘 연산으로 가까운 점 연결 및 구멍 메우기
    cv::morphologyEx(temp, restored, cv::MORPH_CLOSE, kernel);
    showAndSaveImage("Res_Morphology", restored);

    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(restored, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);

    std::vector<ContourData> detectedPoints; // 모든 유효 윤곽선에서 추출된 점들을 저장
    result.pointsOnLeftEdge.clear(); // EnvGeoResult에 결과 저장 시작 전 초기화

    // ----------------------------------------------------
    // Y값 유사성 판단 기준 (오차 허용 범위)
    const float yTolerance = 5.0f;
    // ----------------------------------------------------

    // 모든 유효 윤곽선 데이터 수집 및 새로운 좌표 계산
    for (int i = 0; i < contours.size(); ++i) {
        const auto& contour = contours[i];
        double currentArea = cv::contourArea(contour);
        if (currentArea < minContourArea) {
            continue;
        }

        cv::Moments m = cv::moments(contour);
        if (m.m00 == 0) {
            continue;
        }

        // 윤곽선의 가장 왼쪽 x 좌표와 위-아래 중앙 y 좌표 계산
        float min_x_local = std::numeric_limits<float>::max();
        float max_y_local = std::numeric_limits<float>::min();
        float min_y_local = std::numeric_limits<float>::max();

        for (const auto& p : contour) {
            if (p.x < min_x_local) {
                min_x_local = static_cast<float>(p.x);
            }
            if (p.y > max_y_local) {
                max_y_local = static_cast<float>(p.y);
            }
            if (p.y < min_y_local) {
                min_y_local = static_cast<float>(p.y);
            }
        }

        float leftmost_x_abs = min_x_local + roi.x;
        float vertical_mid_y_abs = (min_y_local + max_y_local) / 2.0f + roi.y;

        // 윤곽선 추출 후 즉시, 검출된 점이 원본 윤곽선 외부에 있는지 확인 (ROI 기준)
        cv::Point2f extractedPoint_roi_relative(min_x_local, vertical_mid_y_abs);
        double distanceToContour = cv::pointPolygonTest(contour, extractedPoint_roi_relative, false);

        if (distanceToContour < 0) { // 점이 윤곽선 외부에 있는 경우
            Logger::Information("Pre-filter: Removed point (outside contour at extraction). Original Contour Index: {0}, Point: ({1}, {2})",
                i, leftmost_x_abs, vertical_mid_y_abs);
            continue; // 이 점은 건너뛰고 다음 윤곽선으로
        }

        detectedPoints.push_back({
            cv::Point2f(),            // center_abs
            0.0,                      // quality
            currentArea,              // area
            contour,                  // originalContour_rel (roiGray 기준의 상대 윤곽선 데이터)
            0.0,                      // distance_from_ref
            0.0,                      // angle_from_ref_rad
            0.0,                      // angle_from_ref_deg
            leftmost_x_abs,           // leftmost_x_abs (전체 이미지 기준)
            vertical_mid_y_abs,       // vertical_mid_y_abs (전체 이미지 기준)
            i                         // originalContourIndex
            });
    }

    // ----------------------------------------------------
    // Y값이 비슷할 경우 X값이 큰 데이터를 지우고, X값이 작은 데이터만 남기기
    // ----------------------------------------------------

    // 1. Y 좌표 기준으로 정렬
    std::sort(detectedPoints.begin(), detectedPoints.end(),
        [](const ContourData& a, const ContourData& b) {
            return a.vertical_mid_y_abs < b.vertical_mid_y_abs;
        });

    std::vector<ContourData> finalFilteredPoints_temp; // 임시 저장용

    if (!detectedPoints.empty()) {
        std::vector<ContourData> currentGroup;
        currentGroup.push_back(detectedPoints[0]);

        for (size_t i = 1; i < detectedPoints.size(); ++i) {
            const auto& currentPoint = detectedPoints[i];
            const auto& firstInGroup = currentGroup[0]; // 그룹의 첫 번째 점 (Y 기준)

            // 현재 점의 Y값이 그룹의 첫 번째 점의 Y값과 yTolerance 이내인지 확인
            if (std::abs(currentPoint.vertical_mid_y_abs - firstInGroup.vertical_mid_y_abs) <= yTolerance) {
                currentGroup.push_back(currentPoint);
            }
            else {
                // 새로운 그룹이 시작되므로, 이전 그룹을 처리
                if (!currentGroup.empty()) {
                    // 그룹 내에서 leftmost_x_abs가 가장 작은 점을 찾음
                    auto minX_it = std::min_element(currentGroup.begin(), currentGroup.end(),
                        [](const ContourData& a, const ContourData& b) {
                            return a.leftmost_x_abs < b.leftmost_x_abs;
                        });
                    // 가장 X값이 작은 점을 최종 필터링된 데이터에 추가
                    finalFilteredPoints_temp.push_back(*minX_it);

                    // 나머지 점들은 제거된 것으로 로그 출력
                    for (const auto& removedPoint : currentGroup) {
                        if (&removedPoint != &(*minX_it)) {
                            Logger::Information("Removed point (Y-similar, X-larger). Original Contour Index: {0}, Point: ({1}, {2})",
                                removedPoint.originalContourIndex, removedPoint.leftmost_x_abs, removedPoint.vertical_mid_y_abs);
                        }
                    }
                }
                // 새 그룹 시작
                currentGroup.clear();
                currentGroup.push_back(currentPoint);
            }
        }
        // 마지막 그룹 처리
        if (!currentGroup.empty()) {
            auto minX_it = std::min_element(currentGroup.begin(), currentGroup.end(),
                [](const ContourData& a, const ContourData& b) {
                    return a.leftmost_x_abs < b.leftmost_x_abs;
                });
            finalFilteredPoints_temp.push_back(*minX_it);
            for (const auto& removedPoint : currentGroup) {
                if (&removedPoint != &(*minX_it)) {
                    Logger::Information("Removed point (Y-similar, X-larger). Original Contour Index: {0}, Point: ({1}, {2})",
                        removedPoint.originalContourIndex, removedPoint.leftmost_x_abs, removedPoint.vertical_mid_y_abs);
                }
            }
        }
    }

    // ★ 최종 필터링된 점들 중, 해당 점이 원본 윤곽선을 벗어나면 제거
    std::vector<ContourData> finalFilteredPoints; // 최종 결과
    for (const auto& data : finalFilteredPoints_temp) {
        // 점의 절대 좌표를 ROI 기준 상대 좌표로 변환하여 pointPolygonTest에 사용
        cv::Point2f point_roi_relative(data.leftmost_x_abs - roi.x, data.vertical_mid_y_abs - roi.y);

        // originalContour_rel 필드를 사용하여 점이 윤곽선 내부에 있는지 확인
        double distanceToOriginalContour = cv::pointPolygonTest(data.originalContour_rel, point_roi_relative, false);

        if (distanceToOriginalContour < 0) { // 점이 원본 윤곽선 외부에 있는 경우
            Logger::Information("Final Filter: Removed point (outside original contour). Original Contour Index: {0}, Point: ({1}, {2})",
                data.originalContourIndex, data.leftmost_x_abs, data.vertical_mid_y_abs);
        }
        else {
            finalFilteredPoints.push_back(data); // 유효한 점만 추가
        }
    }

    // ----------------------------------------------------
    // ★ 점 간 관계 수치화 시작 (절대 Y좌표 기준 오차 측정)
    // ----------------------------------------------------
    result.x_uniformity_std_dev = 0.0;
    result.y_interval_errors.clear();

    if (finalFilteredPoints.size() >= 2) {
        const double targetYInterval = 30.0; // 목표 간격

        // 1. 개선된 오프셋 계산 (모든 점들을 기준으로 보정)
        double bestOffset = calculateBestOffset(finalFilteredPoints, targetYInterval);

        Logger::Information("--- 절대 Y좌표 기준 오차 계산 시작 (Best Offset: {0}px) ---", bestOffset);

        // 2. 각 점의 오차 측정 및 결과 저장
        size_t idealPointIndex = 0; // 누락된 점을 건너뛰기 위한 인덱스
        for (size_t i = 0; i < finalFilteredPoints.size(); ++i) {
            const auto& point = finalFilteredPoints[i];
            double currentY = point.vertical_mid_y_abs;

            // 현재 점의 Y 좌표에 가장 가까운 이상적인 위치를 찾음
            double idealY = (static_cast<double>(idealPointIndex) * targetYInterval) + bestOffset;
            double nextIdealY = (static_cast<double>(idealPointIndex + 1) * targetYInterval) + bestOffset;

            // 다음 이상적인 위치가 더 가까우면 idealPointIndex를 증가시킴 (점 누락 보정)
            while (std::abs(currentY - nextIdealY) < std::abs(currentY - idealY)) {
                idealY = nextIdealY;
                idealPointIndex++;
                nextIdealY = (static_cast<double>(idealPointIndex + 1) * targetYInterval) + bestOffset;
            }

            double error = std::abs(currentY - idealY);

            Logger::Information(
                "Point {0}: Actual Y={1}px, Ideal Y={2}px (Index {3}), Error={4}px",
                (int)i, currentY, idealY, idealPointIndex, error
            );

            // EnvGeoResult에 결과 저장
            result.y_interval_errors.push_back({
                (int)i,
                -1,
                currentY,
                idealY,
                error
                });
            idealPointIndex++; // 다음 점을 위해 인덱스 증가
        }

        // 3. X 좌표 균일성 (X Uniformity) 계산 (기존 로직 유지)
        std::vector<double> x_coords;
        for (const auto& p : finalFilteredPoints) {
            x_coords.push_back(p.leftmost_x_abs);
        }
        result.x_uniformity_std_dev = calculateStandardDeviation(x_coords);
        Logger::Information("X Uniformity (Standard Deviation): {0}", result.x_uniformity_std_dev);

    }
    else {
        // 점이 부족하여 패턴 분석 불가
        Logger::Warning("Not enough points ({0}) to perform pattern inspection.", (int)finalFilteredPoints.size());
    }
    // ----------------------------------------------------
    // ★ 점 간 관계 수치화 종료
    // ----------------------------------------------------

    // 필터링된 점들과 해당 윤곽선을 이미지에 그리기
    for (const auto& data : finalFilteredPoints) {
        result.pointsOnLeftEdge.push_back(cv::Point2f(data.leftmost_x_abs, data.vertical_mid_y_abs));

        // 윤곽선 그리기 (녹색)
        // originalContour_rel은 roiGray 기준으로 되어 있으므로 offset을 더해줍니다.
        std::vector<std::vector<cv::Point>> contoursToDraw;
        std::vector<cv::Point> shiftedContour;
        for (const auto& p : data.originalContour_rel) { // originalContour_rel 사용
            shiftedContour.push_back(cv::Point(p.x + roi.x, p.y + roi.y));
        }
        contoursToDraw.push_back(shiftedContour);
        cv::drawContours(roiImage, contoursToDraw, -1, GreenA, 1);

        // 점 그리기 (빨간색, 윤곽선 위에 그려져야 더 잘 보임)
        cv::circle(roiImage, cv::Point2f(data.leftmost_x_abs, data.vertical_mid_y_abs), 2, RedA, -1);
    }

    Logger::Information("--- Final Filtered Points (Y-similar, X-smallest) ---");
    for (size_t i = 0; i < finalFilteredPoints.size(); ++i) {
        const auto& data = finalFilteredPoints[i];
        Logger::Information(
            "Final Point {0}: Original Contour Index {1}, Point: ({2}, {3}), Original Area: {4}",
            (int)i,
            data.originalContourIndex,
            data.leftmost_x_abs, data.vertical_mid_y_abs,
            data.area
        );
    }
    Logger::Information("----------------------------------");

    showAndSaveImage("Geo_Env_End", roiImage);
}

void MyOpenCVWrapper::EnvGeoInspection4(cv::Mat& roiImage, EnvGeoResult2& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) {
        return;
    }

    const ConfigManager& config = ConfigManager::getInstance();
    const InspectionParams::ResParams& resConfig = config.getInspectionParams().res;

    const int drMin = 55; // resConfig.drMin;
    const int drMax = 70; // resConfig.drMax;
    const double minContourArea = resConfig.minContourArea;

    cv::Mat gray;
    cv::cvtColor(roiImage, gray, cv::COLOR_BGRA2GRAY);

    showAndThreshold("Res_DR_Adjust", gray);

    ApplyLinearDRClip(gray, gray, drMin, drMax, true);
    showAndSaveImage("Res_DR_Clipped", gray);

    cv::Rect roi(110, 0, 40, roiImage.rows);

    if (roi.x < 0 || roi.y < 0 || roi.x + roi.width > gray.cols || roi.y + roi.height > gray.rows) {
        Logger::Error("--- ROI is out of image bounds ---");
        return;
    }

    cv::Mat roiGray = gray(roi);
    cv::Mat binary;
    cv::threshold(roiGray, binary, 100, 255, cv::THRESH_BINARY);

    cv::Mat eroded, restored;
    cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));
    cv::erode(binary, eroded, kernel);
    cv::dilate(eroded, restored, kernel);
    showAndSaveImage("Res_Morphology", restored);

    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(restored, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);

    std::vector<ContourData> detectedPoints; // 모든 유효 윤곽선에서 추출된 점들을 저장
    result.pointsOnLeftEdge.clear(); // EnvGeoResult에 결과 저장 시작 전 초기화

    // ----------------------------------------------------
    // Y값 유사성 판단 기준 (오차 허용 범위)
    const float yTolerance = 5.0f;
    // ----------------------------------------------------

    // 모든 유효 윤곽선 데이터 수집 및 새로운 좌표 계산
    for (int i = 0; i < contours.size(); ++i) {
        const auto& contour = contours[i];
        double currentArea = cv::contourArea(contour);
        if (currentArea < minContourArea) {
            continue;
        }

        cv::Moments m = cv::moments(contour);
        if (m.m00 == 0) {
            continue;
        }

        // 윤곽선의 가장 왼쪽 x 좌표와 위-아래 중앙 y 좌표 계산
        float min_x_local = std::numeric_limits<float>::max();
        float max_y_local = std::numeric_limits<float>::min();
        float min_y_local = std::numeric_limits<float>::max();

        for (const auto& p : contour) {
            if (p.x < min_x_local) {
                min_x_local = static_cast<float>(p.x);
            }
            if (p.y > max_y_local) {
                max_y_local = static_cast<float>(p.y);
            }
            if (p.y < min_y_local) {
                min_y_local = static_cast<float>(p.y);
            }
        }

        float leftmost_x_abs = min_x_local + roi.x;
        float vertical_mid_y_abs = (min_y_local + max_y_local) / 2.0f + roi.y;

        // 윤곽선 추출 후 즉시, 검출된 점이 원본 윤곽선 외부에 있는지 확인 (ROI 기준)
        cv::Point2f extractedPoint_roi_relative(min_x_local, vertical_mid_y_abs);
        double distanceToContour = cv::pointPolygonTest(contour, extractedPoint_roi_relative, false);

        if (distanceToContour < 0) { // 점이 윤곽선 외부에 있는 경우
            Logger::Information("Pre-filter: Removed point (outside contour at extraction). Original Contour Index: {0}, Point: ({1}, {2})",
                i, leftmost_x_abs, vertical_mid_y_abs);
            continue; // 이 점은 건너뛰고 다음 윤곽선으로
        }

        detectedPoints.push_back({
            cv::Point2f(),            // center_abs
            0.0,                      // quality
            currentArea,              // area
            contour,                  // originalContour_rel (roiGray 기준의 상대 윤곽선 데이터)
            0.0,                      // distance_from_ref
            0.0,                      // angle_from_ref_rad
            0.0,                      // angle_from_ref_deg
            leftmost_x_abs,           // leftmost_x_abs (전체 이미지 기준)
            vertical_mid_y_abs,       // vertical_mid_y_abs (전체 이미지 기준)
            i                         // originalContourIndex
            });
    }

    // ----------------------------------------------------
    // Y값이 비슷할 경우 X값이 큰 데이터를 지우고, X값이 작은 데이터만 남기기
    // ----------------------------------------------------

    // 1. Y 좌표 기준으로 정렬
    std::sort(detectedPoints.begin(), detectedPoints.end(),
        [](const ContourData& a, const ContourData& b) {
            return a.vertical_mid_y_abs < b.vertical_mid_y_abs;
        });

    std::vector<ContourData> finalFilteredPoints_temp; // 임시 저장용

    if (!detectedPoints.empty()) {
        std::vector<ContourData> currentGroup;
        currentGroup.push_back(detectedPoints[0]);

        for (size_t i = 1; i < detectedPoints.size(); ++i) {
            const auto& currentPoint = detectedPoints[i];
            const auto& firstInGroup = currentGroup[0]; // 그룹의 첫 번째 점 (Y 기준)

            // 현재 점의 Y값이 그룹의 첫 번째 점의 Y값과 yTolerance 이내인지 확인
            if (std::abs(currentPoint.vertical_mid_y_abs - firstInGroup.vertical_mid_y_abs) <= yTolerance) {
                currentGroup.push_back(currentPoint);
            }
            else {
                // 새로운 그룹이 시작되므로, 이전 그룹을 처리
                if (!currentGroup.empty()) {
                    // 그룹 내에서 leftmost_x_abs가 가장 작은 점을 찾음
                    auto minX_it = std::min_element(currentGroup.begin(), currentGroup.end(),
                        [](const ContourData& a, const ContourData& b) {
                            return a.leftmost_x_abs < b.leftmost_x_abs;
                        });
                    // 가장 X값이 작은 점을 최종 필터링된 데이터에 추가
                    finalFilteredPoints_temp.push_back(*minX_it);

                    // 나머지 점들은 제거된 것으로 로그 출력
                    for (const auto& removedPoint : currentGroup) {
                        if (&removedPoint != &(*minX_it)) {
                            Logger::Information("Removed point (Y-similar, X-larger). Original Contour Index: {0}, Point: ({1}, {2})",
                                removedPoint.originalContourIndex, removedPoint.leftmost_x_abs, removedPoint.vertical_mid_y_abs);
                        }
                    }
                }
                // 새 그룹 시작
                currentGroup.clear();
                currentGroup.push_back(currentPoint);
            }
        }
        // 마지막 그룹 처리
        if (!currentGroup.empty()) {
            auto minX_it = std::min_element(currentGroup.begin(), currentGroup.end(),
                [](const ContourData& a, const ContourData& b) {
                    return a.leftmost_x_abs < b.leftmost_x_abs;
                });
            finalFilteredPoints_temp.push_back(*minX_it);
            for (const auto& removedPoint : currentGroup) {
                if (&removedPoint != &(*minX_it)) {
                    Logger::Information("Removed point (Y-similar, X-larger). Original Contour Index: {0}, Point: ({1}, {2})",
                        removedPoint.originalContourIndex, removedPoint.leftmost_x_abs, removedPoint.vertical_mid_y_abs);
                }
            }
        }
    }

    // ★ 최종 필터링된 점들 중, 해당 점이 원본 윤곽선을 벗어나면 제거
    std::vector<ContourData> finalFilteredPoints; // 최종 결과
    for (const auto& data : finalFilteredPoints_temp) {
        // 점의 절대 좌표를 ROI 기준 상대 좌표로 변환하여 pointPolygonTest에 사용
        cv::Point2f point_roi_relative(data.leftmost_x_abs - roi.x, data.vertical_mid_y_abs - roi.y);

        // originalContour_rel 필드를 사용하여 점이 윤곽선 내부에 있는지 확인
        double distanceToOriginalContour = cv::pointPolygonTest(data.originalContour_rel, point_roi_relative, false);

        if (distanceToOriginalContour < 0) { // 점이 원본 윤곽선 외부에 있는 경우
            Logger::Information("Final Filter: Removed point (outside original contour). Original Contour Index: {0}, Point: ({1}, {2})",
                data.originalContourIndex, data.leftmost_x_abs, data.vertical_mid_y_abs);
        }
        else {
            finalFilteredPoints.push_back(data); // 유효한 점만 추가
        }
    }

    // ----------------------------------------------------
    // ★ 점 간 관계 수치화 시작 (절대 Y좌표 기준 오차 측정)
    // ----------------------------------------------------
    result.x_uniformity_std_dev = 0.0;
    result.y_interval_errors.clear();

    if (finalFilteredPoints.size() >= 2) {
        const double targetYInterval = 30.0; // 목표 간격

        // 1. 개선된 오프셋 계산 (모든 점들을 기준으로 보정)
        double bestOffset = calculateBestOffset(finalFilteredPoints, targetYInterval);

        Logger::Information("--- 절대 Y좌표 기준 오차 계산 시작 (Best Offset: {0}px) ---", bestOffset);

        // 2. 각 점의 오차 측정 및 결과 저장
        size_t idealPointIndex = 0; // 누락된 점을 건너뛰기 위한 인덱스
        for (size_t i = 0; i < finalFilteredPoints.size(); ++i) {
            const auto& point = finalFilteredPoints[i];
            double currentY = point.vertical_mid_y_abs;

            // 현재 점의 Y 좌표에 가장 가까운 이상적인 위치를 찾음
            double idealY = (static_cast<double>(idealPointIndex) * targetYInterval) + bestOffset;
            double nextIdealY = (static_cast<double>(idealPointIndex + 1) * targetYInterval) + bestOffset;

            // 다음 이상적인 위치가 더 가까우면 idealPointIndex를 증가시킴 (점 누락 보정)
            while (std::abs(currentY - nextIdealY) < std::abs(currentY - idealY)) {
                idealY = nextIdealY;
                idealPointIndex++;
                nextIdealY = (static_cast<double>(idealPointIndex + 1) * targetYInterval) + bestOffset;
            }

            double error = std::abs(currentY - idealY);

            Logger::Information(
                "Point {0}: Actual Y={1}px, Ideal Y={2}px (Index {3}), Error={4}px",
                (int)i, currentY, idealY, idealPointIndex, error
            );

            // EnvGeoResult에 결과 저장
            result.y_interval_errors.push_back({
                (int)i,
                -1,
                currentY,
                idealY,
                error
                });
            idealPointIndex++; // 다음 점을 위해 인덱스 증가
        }

        // 3. X 좌표 균일성 (X Uniformity) 계산 (기존 로직 유지)
        std::vector<double> x_coords;
        for (const auto& p : finalFilteredPoints) {
            x_coords.push_back(p.leftmost_x_abs);
        }
        result.x_uniformity_std_dev = calculateStandardDeviation(x_coords);
        Logger::Information("X Uniformity (Standard Deviation): {0}", result.x_uniformity_std_dev);

    }
    else {
        // 점이 부족하여 패턴 분석 불가
        Logger::Warning("Not enough points ({0}) to perform pattern inspection.", (int)finalFilteredPoints.size());
    }
    // ----------------------------------------------------
    // ★ 점 간 관계 수치화 종료
    // ----------------------------------------------------

    // 필터링된 점들과 해당 윤곽선을 이미지에 그리기
    for (const auto& data : finalFilteredPoints) {
        result.pointsOnLeftEdge.push_back(cv::Point2f(data.leftmost_x_abs, data.vertical_mid_y_abs));

        // 윤곽선 그리기 (녹색)
        // originalContour_rel은 roiGray 기준으로 되어 있으므로 offset을 더해줍니다.
        std::vector<std::vector<cv::Point>> contoursToDraw;
        std::vector<cv::Point> shiftedContour;
        for (const auto& p : data.originalContour_rel) { // originalContour_rel 사용
            shiftedContour.push_back(cv::Point(p.x + roi.x, p.y + roi.y));
        }
        contoursToDraw.push_back(shiftedContour);
        cv::drawContours(roiImage, contoursToDraw, -1, GreenA, 1);

        // 점 그리기 (빨간색, 윤곽선 위에 그려져야 더 잘 보임)
        cv::circle(roiImage, cv::Point2f(data.leftmost_x_abs, data.vertical_mid_y_abs), 2, RedA, -1);
    }
	cv::rectangle(roiImage, roi, YellowA, 1); // ROI 영역 표시

    Logger::Information("--- Final Filtered Points (Y-similar, X-smallest) ---");
    for (size_t i = 0; i < finalFilteredPoints.size(); ++i) {
        const auto& data = finalFilteredPoints[i];
        Logger::Information(
            "Final Point {0}: Original Contour Index {1}, Point: ({2}, {3}), Original Area: {4}",
            (int)i,
            data.originalContourIndex,
            data.leftmost_x_abs, data.vertical_mid_y_abs,
            data.area
        );
    }
    Logger::Information("----------------------------------");

    showAndSaveImage("Geo_Env_End", roiImage);
}

void MyOpenCVWrapper::EnvGeoInspection(cv::Mat& srcImg, EnvGeoResult& result)
{
    if (srcImg.empty() || srcImg.channels() != 4) {
        return;
    }

    const EnvGeoData* bestRefPoint = nullptr;
    const ConfigManager& config = ConfigManager::getInstance();
    const InspectionParams::EnvGeoParams& envGeoConfig = config.getInspectionParams().envGeo;

    const int drMin = envGeoConfig.drMin; // resConfig.drMin;
    const int drMax = envGeoConfig.drMax; // resConfig.drMax;
    const double minContourArea = envGeoConfig.minContourArea;

    cv::Mat gray;
    cv::cvtColor(srcImg, gray, cv::COLOR_BGRA2GRAY);

    showAndThreshold("EnvGeo_DR_Adjust", gray);

    ApplyLinearDRClip(gray, gray, drMin, drMax, true);
    showAndSaveImage("EnvGeo_DR_Clipped", gray);

    cv::Rect roi(envGeoConfig.roi.x, envGeoConfig.roi.y, envGeoConfig.roi.width, srcImg.rows);

    if (roi.x < 0 || roi.y < 0 || roi.x + roi.width > gray.cols || roi.y + roi.height > gray.rows) {
        Logger::Error("--- ROI is out of image bounds ---");
        return;
    }

    cv::Mat roiGray = gray(roi);
    cv::Mat binary;
    cv::threshold(roiGray, binary, 100, 255, cv::THRESH_BINARY);

    cv::Mat eroded, restored;
    cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));
    cv::erode(binary, eroded, kernel);
    cv::dilate(eroded, restored, kernel);
    showAndSaveImage("EnvGeo_Morphology", restored);

    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(restored, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);

    std::vector<EnvGeoData> detectedPoints; // 모든 유효 윤곽선에서 추출된 점들을 저장
    //result.findPoints.clear(); // EnvGeoResult에 결과 저장 시작 전 초기화

    // ----------------------------------------------------
    // Y값 유사성 판단 기준 (오차 허용 범위)
    const float yTolerance = envGeoConfig.yTolerance;
    // ----------------------------------------------------

    // 모든 유효 윤곽선 데이터 수집 및 새로운 좌표 계산
    for (int i = 0; i < contours.size(); ++i) {
        const auto& contour = contours[i];
        double currentArea = cv::contourArea(contour);
        if (currentArea < minContourArea) {
            continue;
        }

        cv::Moments m = cv::moments(contour);
        if (m.m00 == 0) {
            continue;
        }

        // 윤곽선의 가장 왼쪽 x 좌표와 위-아래 중앙 y 좌표 계산
        float min_x_local = std::numeric_limits<float>::max();
        float max_y_local = std::numeric_limits<float>::min();
        float min_y_local = std::numeric_limits<float>::max();

        for (const auto& p : contour) {
            if (p.x < min_x_local) {
                min_x_local = static_cast<float>(p.x);
            }
            if (p.y > max_y_local) {
                max_y_local = static_cast<float>(p.y);
            }
            if (p.y < min_y_local) {
                min_y_local = static_cast<float>(p.y);
            }
        }

        float leftmost_x = min_x_local;
        float vertical_mid_y = (min_y_local + max_y_local) / 2.0f;

        // 윤곽선 추출 후 즉시, 검출된 점이 원본 윤곽선 외부에 있는지 확인 (ROI 기준)
        cv::Point2f extractedPoint_roi(min_x_local, vertical_mid_y);
        double distanceToContour = cv::pointPolygonTest(contour, extractedPoint_roi, false);

        if (distanceToContour < 0) { // 점이 윤곽선 외부에 있는 경우
            Logger::Information("Pre-filter: Removed point (outside contour at extraction). Original Contour Index: {0}, Point: ({1}, {2})",
                i, leftmost_x + roi.x, vertical_mid_y + roi.y);
            continue; // 이 점은 건너뛰고 다음 윤곽선으로
        }

        detectedPoints.push_back({
            leftmost_x,           // leftmost_x (전체 이미지 기준)
            vertical_mid_y,       // vertical_mid_y (전체 이미지 기준)
            currentArea,              // area
            contour,                  // originalContour_rel (roiGray 기준의 상대 윤곽선 데이터)
            i                         // originalContourIndex
            });
    }

    // ----------------------------------------------------
    // Y값이 비슷할 경우 X값이 큰 데이터를 지우고, X값이 작은 데이터만 남기기
    // ----------------------------------------------------

    // 1. Y 좌표 기준으로 정렬
    std::sort(detectedPoints.begin(), detectedPoints.end(),
        [](const EnvGeoData& a, const EnvGeoData& b) {
            return a.vertical_mid_y < b.vertical_mid_y;
        });

    std::vector<EnvGeoData> finalFilteredPoints_temp; // 임시 저장용
    std::vector<EnvGeoData> removedPoints_temp; // Y 유사성 필터링에서 제거된 점들 임시 저장

    if (!detectedPoints.empty()) {
        std::vector<EnvGeoData> currentGroup;
        currentGroup.push_back(detectedPoints[0]);

        for (size_t i = 1; i < detectedPoints.size(); ++i) {
            const auto& currentPoint = detectedPoints[i];
            const auto& firstInGroup = currentGroup[0]; // 그룹의 첫 번째 점 (Y 기준)

            // 현재 점의 Y값이 그룹의 첫 번째 점의 Y값과 yTolerance 이내인지 확인
            if (std::abs(currentPoint.vertical_mid_y - firstInGroup.vertical_mid_y) <= yTolerance) {
                currentGroup.push_back(currentPoint);
            }
            else {
                // 새로운 그룹이 시작되므로, 이전 그룹을 처리
                if (!currentGroup.empty()) {
                    // 그룹 내에서 leftmost_x가 가장 작은 점을 찾음
                    auto minX_it = std::min_element(currentGroup.begin(), currentGroup.end(),
                        [](const EnvGeoData& a, const EnvGeoData& b) {
                            return a.leftmost_x < b.leftmost_x;
                        });
                    // 가장 X값이 작은 점을 최종 필터링된 데이터에 추가
                    finalFilteredPoints_temp.push_back(*minX_it);

                    // 나머지 점들은 제거된 것으로 로그 출력
                    for (const auto& removedPoint : currentGroup) {
                        if (&removedPoint != &(*minX_it)) {
                            Logger::Information("Removed point (Y-similar, X-larger). Original Contour Index: {0}, Point: ({1}, {2})",
                                removedPoint.originalContourIndex, removedPoint.leftmost_x + roi.x, removedPoint.vertical_mid_y + roi.y);
                            removedPoints_temp.push_back(removedPoint);
                        }
                    }
                }
                // 새 그룹 시작
                currentGroup.clear();
                currentGroup.push_back(currentPoint);
            }
        }
        // 마지막 그룹 처리
        if (!currentGroup.empty()) {
            auto minX_it = std::min_element(currentGroup.begin(), currentGroup.end(),
                [](const EnvGeoData& a, const EnvGeoData& b) {
                    return a.leftmost_x < b.leftmost_x;
                });
            finalFilteredPoints_temp.push_back(*minX_it);
            for (const auto& removedPoint : currentGroup) {
                if (&removedPoint != &(*minX_it)) {
                    Logger::Information("Removed point (Y-similar, X-larger). Original Contour Index: {0}, Point: ({1}, {2})",
                        removedPoint.originalContourIndex, removedPoint.leftmost_x + roi.x, removedPoint.vertical_mid_y + roi.y);
                    removedPoints_temp.push_back(removedPoint);
                }
            }
        }
    }

    // ★ 최종 필터링된 점들 중, 해당 점이 원본 윤곽선을 벗어나면 제거
    std::vector<EnvGeoData> finalFilteredPoints; // 최종 결과
    for (const auto& data : finalFilteredPoints_temp) {
        // 점의 절대 좌표를 ROI 기준 상대 좌표로 변환하여 pointPolygonTest에 사용
        cv::Point2f point_roi(data.leftmost_x, data.vertical_mid_y);

        // originalContour_rel 필드를 사용하여 점이 윤곽선 내부에 있는지 확인
        double distanceToOriginalContour = cv::pointPolygonTest(data.originalContour, point_roi, false);

        if (distanceToOriginalContour < 0) { // 점이 원본 윤곽선 외부에 있는 경우
            Logger::Information("Final Filter: Removed point (outside original contour). Original Contour Index: {0}, Point: ({1}, {2})",
                data.originalContourIndex, data.leftmost_x + roi.x, data.vertical_mid_y + roi.y);
            removedPoints_temp.push_back(data);
        }
        else {
            Logger::Information("Final Filter: point (original contour). Original Contour Index: {0}, Point: ({1}, {2})",
                data.originalContourIndex, data.leftmost_x + roi.x, data.vertical_mid_y + roi.y);
            finalFilteredPoints.push_back(data); // 유효한 점만 추가
        }
    }

    // ★★★----------------------------------------------------
    // ★★★ 검출 결과 해석 단계: X값, Y값 간격 패턴에 맞지 않는 점 필터링 (수정된 로직)
    // ★★★----------------------------------------------------
    std::vector<EnvGeoData> filteredByXPoints; // X 필터링 통과 점
    std::vector<EnvGeoData> filteredByYPoints; // Y 필터링 통과 점
    std::vector<EnvGeoData> finalResultPoints; // 최종적으로 이미지에 그릴 점들

    std::vector<EnvGeoData> filteredOutXPoints; // X 필터에서 제거된 점들
    std::vector<EnvGeoData> filteredOutYPoints; // Y 필터에서 제거된 점들

    // ----------------------------------------------------
    // 1. X값 균일성 검사 (항상 실행)
    // ----------------------------------------------------
    if (finalFilteredPoints.size() > 1) {
        std::vector<double> x_coords;
        for (const auto& p : finalFilteredPoints) {
            x_coords.push_back(p.leftmost_x);
        }

        std::sort(x_coords.begin(), x_coords.end());

        double median;
        size_t size = x_coords.size();
        if (size % 2 == 0) {
            median = (x_coords[size / 2 - 1] + x_coords[size / 2]) / 2.0;
        }
        else {
            median = x_coords[size / 2];
        }

        std::vector<double> absolute_deviations;
        for (double x : x_coords) {
            absolute_deviations.push_back(std::abs(x - median));
        }

        std::sort(absolute_deviations.begin(), absolute_deviations.end());
        double mad;
        size_t ad_size = absolute_deviations.size();
        if (ad_size % 2 == 0) {
            mad = (absolute_deviations[ad_size / 2 - 1] + absolute_deviations[ad_size / 2]) / 2.0;
        }
        else {
            mad = absolute_deviations[ad_size / 2];
        }

        const double mad_constant = envGeoConfig.xMadConstant;
        const double xTolerance = 1.4826 * mad * mad_constant;

        Logger::Information("X Uniformity Check (MAD-based): Median={0}, MAD={1}, Tolerance={2}", median, mad, xTolerance);

        result.madMetrics.median_x = median;
        result.madMetrics.mad_x = mad;
        result.madMetrics.x_tolerance = xTolerance;

        for (const auto& point : finalFilteredPoints) {
            if (std::abs(point.leftmost_x - median) > xTolerance) {
                filteredOutXPoints.push_back(point);
                Logger::Information("Filtered out by X-uniformity: Original Index {0}, Point ({1}, {2})", point.originalContourIndex, point.leftmost_x + roi.x, point.vertical_mid_y + roi.x);
            }
            else {
                filteredByXPoints.push_back(point);
            }
        }
    }
    else {
        filteredByXPoints = finalFilteredPoints;
    }

    /* Y값 간격 필터링
    if (finalResultPoints.size() > 1) {
        // Y 좌표 기준으로 다시 정렬 (혹시 모를 상황 대비)
        std::sort(finalResultPoints.begin(), finalResultPoints.end(),
            [](const EnvGeoData& a, const EnvGeoData& b) {
                return a.vertical_mid_y < b.vertical_mid_y;
            });

        const double targetYInterval = 30.0; // 목표 Y 간격 (이 값은 설정에서 가져와야 함)
        const double yIntervalTolerance = 10.0; // Y 간격 허용 오차 (픽셀)

        std::vector<EnvGeoData> tempFilteredResultPoints;
        for (size_t i = 0; i < finalResultPoints.size(); ++i) {
            if (i > 0) {
                double currentInterval = finalResultPoints[i].vertical_mid_y - finalResultPoints[i - 1].vertical_mid_y;
                if (std::abs(currentInterval - targetYInterval) > yIntervalTolerance) {
                    filteredOutYPoints.push_back(finalResultPoints[i]);
                    Logger::Information("Filtered out by Y-interval: Original Index {0}, Point ({1}, {2}), Interval {3}",
                        finalResultPoints[i].originalContourIndex,
                        finalResultPoints[i].leftmost_x,
                        finalResultPoints[i].vertical_mid_y,
                        currentInterval);
                    // 간격이 맞지 않는 점은 제외하고 다음 점으로
                    continue;
                }
            }
            tempFilteredResultPoints.push_back(finalResultPoints[i]);
        }
        finalResultPoints = tempFilteredResultPoints;
    }
    */

    // ----------------------------------------------------
    // 2. Y값 간격 필터링 (항상 실행)
    // ----------------------------------------------------
    /*const double targetYInterval = 30.0;
    const double yIntervalTolerance = 5.0;
    result.yIntervalMetrics.target_y_interval = targetYInterval;
    result.yIntervalMetrics.y_tolerance = yIntervalTolerance;
    
    if (finalFilteredPoints.size() > 1) {

        std::vector<double> remainders;
        for (const auto& point : finalFilteredPoints) {
            double rem = fmod(point.vertical_mid_y, targetYInterval);
            if (rem > targetYInterval / 2) rem = rem - targetYInterval;
            remainders.push_back(rem);
        }

        double sumOfRemainders = 0.0;
        int validRemainderCount = 0;
        const double outlierThreshold = targetYInterval / 2.0;

        for (double rem : remainders) {
            if (std::abs(rem) <= outlierThreshold) {
                sumOfRemainders += rem;
                validRemainderCount++;
            }
        }

        double meanOffset = 0.0;
        if (validRemainderCount > 0) {
            meanOffset = sumOfRemainders / validRemainderCount;
            Logger::Information("Calculated Y-offset: {0}", meanOffset);
            result.yIntervalMetrics.mean_offset = meanOffset;
        }
        else {
            Logger::Information("Not enough valid points to calculate Y-offset. Skipping offset correction.");
            result.yIntervalMetrics.mean_offset = 0.0;
        }

        for (const auto& point : finalFilteredPoints) {
            double correctedY = point.vertical_mid_y - meanOffset;
            double rem = fmod(correctedY, targetYInterval);

            if (rem > targetYInterval / 2) {
                rem = rem - targetYInterval;
            }

            if (std::abs(rem) < yIntervalTolerance) {
                filteredByYPoints.push_back(point);
            }
            else {
                filteredOutYPoints.push_back(point);
                Logger::Information("Filtered out by Y-interval (after offset correction): Original Index {0}, Point ({1}, {2}), Corrected Y: {3}", point.originalContourIndex, point.leftmost_x, point.vertical_mid_y, correctedY);
                result.yIntervalMetrics.filtered_out_by_y.push_back(cv::Point(static_cast<int>(point.leftmost_x), static_cast<int>(point.vertical_mid_y)));
            }
        }
    }
    else {
        filteredByYPoints = finalFilteredPoints;
    }*/

    // ----------------------------------------------------
    // 2. Y값 간격 필터링 (가장 잘 맞는 패턴 찾기)
    // ----------------------------------------------------
    
    if (finalFilteredPoints.size() > 1) {
        const double targetYInterval = envGeoConfig.targetYInterval;
        const double yIntervalTolerance = envGeoConfig.yIntervalTolerance;
        result.yIntervalMetrics.target_y_interval = targetYInterval;
        result.yIntervalMetrics.y_tolerance = yIntervalTolerance;

        // 각 기준점의 매치 개수를 저장할 맵
        // key: 기준점의 originalContourIndex, value: 매치된 점들의 개수
        std::map<int, int> matchCounts;

        // 모든 점을 기준점(refPoint)으로 가정하여 패턴에 맞는 점들의 개수를 계산
        for (const auto& refPoint : finalFilteredPoints) {
            int currentMatchesCount = 1; // 자기 자신 포함

            for (const auto& otherPoint : finalFilteredPoints) {
                if (&refPoint == &otherPoint) {
                    continue;
                }
                double yDiff = std::abs(otherPoint.vertical_mid_y - refPoint.vertical_mid_y);
                double remainder = fmod(yDiff, targetYInterval);

                if (std::abs(remainder) < yIntervalTolerance || std::abs(remainder - targetYInterval) < yIntervalTolerance) {
                    currentMatchesCount++;
                }
            }
            // 각 기준점에 대한 매치 개수를 맵에 저장
            matchCounts[refPoint.originalContourIndex] = currentMatchesCount;
        }

        // 맵을 내림차순으로 정렬
        std::vector<std::pair<int, int>> sortedMatchCounts(matchCounts.begin(), matchCounts.end());
        std::sort(sortedMatchCounts.begin(), sortedMatchCounts.end(), [](const auto& a, const auto& b) {
            return a.second > b.second;
            });

        //const EnvGeoData* bestRefPoint = nullptr;
        // 가장 많은 매칭을 가진 기준점을 찾음
        if (!sortedMatchCounts.empty()) {
            int bestRefPointIndex = sortedMatchCounts.front().first;
            for (const auto& point : finalFilteredPoints) {
                if (point.originalContourIndex == bestRefPointIndex) {
                    bestRefPoint = &point;
                    break;
                }
            }
        }
        else {
            filteredByYPoints = finalFilteredPoints;
        }

        // 찾은 기준점을 기반으로 점들을 필터링
        if (bestRefPoint) {
            Logger::Information("Best Reference Point (Cyan): Index {0}, Point ({1}, {2})", bestRefPoint->originalContourIndex, bestRefPoint->leftmost_x + roi.x, bestRefPoint->vertical_mid_y + roi.y);
            for (const auto& point : finalFilteredPoints) {
                double yDiff = std::abs(point.vertical_mid_y - bestRefPoint->vertical_mid_y);
                double remainder = fmod(yDiff, targetYInterval);
                if (std::abs(remainder) < yIntervalTolerance || std::abs(remainder - targetYInterval) < yIntervalTolerance) {
                    filteredByYPoints.push_back(point);
                    Logger::Information("Filtered by Y-interval: Original Index {0}, Point ({1}, {2}), Diff : {3}", point.originalContourIndex, point.leftmost_x + roi.x, point.vertical_mid_y + roi.y, remainder);
                }
                else {
                    filteredOutYPoints.push_back(point);
                    Logger::Information("Filtered out by Y-interval: Original Index {0}, Point ({1}, {2}), Diff : {3}", point.originalContourIndex, point.leftmost_x + roi.x, point.vertical_mid_y + roi.y, remainder);
                }
            }
        }
    }
    else {
        filteredByYPoints = finalFilteredPoints;
    }

    // ----------------------------------------------------
    // 3. 최종 결과 도출 (두 필터링을 모두 통과한 점만 남김)
    // ----------------------------------------------------
    const bool useXFilter = envGeoConfig.enableXFilter; // X 필터 사용 여부
    const bool useYFilter = envGeoConfig.enableYFilter; // Y 필터 사용 여부

    if (useXFilter && useYFilter) {
        std::set<int> yFilteredIndices;
        for (const auto& yPoint : filteredByYPoints) {
            yFilteredIndices.insert(yPoint.originalContourIndex);
        }
        for (const auto& xPoint : filteredByXPoints) {
            if (yFilteredIndices.count(xPoint.originalContourIndex)) {
                finalResultPoints.push_back(xPoint);
            }
        }
    }
    else if (useXFilter) {
        finalResultPoints = filteredByXPoints;
    }
    else if (useYFilter) {
        finalResultPoints = filteredByYPoints;
    }
    else {
        finalResultPoints = finalFilteredPoints;
    }


    // ----------------------------------------------------
    // ★★★ 결과 해석 단계 종료
    // ----------------------------------------------------

    // ----------------------------------------------------
    // 모든 유효 윤곽선 그리기 (초록색)
    // ----------------------------------------------------
    for (const auto& data : finalFilteredPoints) { // Y 유사성 필터링을 통과한 모든 점의 윤곽선
        std::vector<std::vector<cv::Point>> contoursToDraw;
        std::vector<cv::Point> shiftedContour;
        for (const auto& p : data.originalContour) {
            shiftedContour.push_back(cv::Point(p.x + roi.x, p.y + roi.y));
        }
        contoursToDraw.push_back(shiftedContour);
        cv::drawContours(srcImg, contoursToDraw, -1, GreenA, 1);
        result.findPoints.push_back(cv::Point(data.leftmost_x + roi.x, data.vertical_mid_y + roi.y));
    }

    // ----------------------------------------------------
    // 필터링된 점들 그리기
    // ----------------------------------------------------
    // X 균일성 필터에서 제거된 점들 그리기 (주황색)
    for (const auto& data : filteredOutXPoints) {
        cv::circle(srcImg, cv::Point(data.leftmost_x + roi.x, data.vertical_mid_y + roi.y), 2, OrangeA, -1);
        result.madMetrics.filtered_out_by_x.push_back(cv::Point(data.leftmost_x + roi.x, data.vertical_mid_y + roi.y));
    }

    // Y 간격 필터에서 제거된 점들 그리기 (파란색)
    for (const auto& data : filteredOutYPoints) {
        cv::circle(srcImg, cv::Point(data.leftmost_x + roi.x, data.vertical_mid_y + roi.y), 2, BlueA, -1);
        result.yIntervalMetrics.filtered_out_by_y.push_back(cv::Point(data.leftmost_x + roi.x, data.vertical_mid_y + roi.y));
    }

    // ----------------------------------------------------
    // 최종 결과 점 그리기 (빨간색)
    // ----------------------------------------------------
    for (const auto& data : finalResultPoints) {
        cv::circle(srcImg, cv::Point(data.leftmost_x + roi.x, data.vertical_mid_y + roi.y), 2, RedA, -1);
        result.finalPoints.push_back(cv::Point(data.leftmost_x + roi.x, data.vertical_mid_y + roi.y));
    }

    if (bestRefPoint) {
        cv::Point center(bestRefPoint->leftmost_x + roi.x, bestRefPoint->vertical_mid_y + roi.y);
        cv::circle(srcImg, center, 2, CyanA, -1); // 반지름 5의 채워진 원 그리기
        result.yIntervalMetrics.best_ref_point = center;
    }
    // 필터링된 점들과 해당 윤곽선을 이미지에 그리기finalFilteredPoints finalResultPoints
    //for (const auto& data : finalResultPoints) {
    //    result.finalPoints.push_back(cv::Point(static_cast<int>(data.leftmost_x), static_cast<int>(data.vertical_mid_y)));

    //    // 윤곽선 그리기 (녹색)
    //    // originalContour_rel은 roiGray 기준으로 되어 있으므로 offset을 더해줍니다.
    //    std::vector<std::vector<cv::Point>> contoursToDraw;
    //    std::vector<cv::Point> shiftedContour;
    //    for (const auto& p : data.originalContour_rel) { // originalContour_rel 사용
    //        shiftedContour.push_back(cv::Point(p.x + roi.x, p.y + roi.y));
    //    }
    //    contoursToDraw.push_back(shiftedContour);
    //    cv::drawContours(srcImg, contoursToDraw, -1, GreenA, 1);

    //    // 점 그리기 (빨간색, 윤곽선 위에 그려져야 더 잘 보임)
    //    cv::circle(srcImg, cv::Point2f(data.leftmost_x, data.vertical_mid_y), 2, RedA, -1);
    //}

    //// 로그에 최종 제거된 점들 출력
    //if (!filteredOutXPoints.empty()) {
    //    Logger::Information("--- Final Removed X Points (by pattern filtering) ---");
    //    for (const auto& data : filteredXOutPoints) {
    //        Logger::Information("Removed: Original Index {0}, Point: ({1}, {2}), Original Area: {3}",
    //            data.originalContourIndex,
    //            data.leftmost_x, data.vertical_mid_y,
    //            data.area);
    //    }
    //    Logger::Information("----------------------------------");
    //}

    //if (!filteredOutYPoints.empty()) {
    //    Logger::Information("--- Final Removed Y Points (by pattern filtering) ---");
    //    for (const auto& data : filteredOutYPoints) {
    //        Logger::Information("Removed: Original Index {0}, Point: ({1}, {2}), Original Area: {3}",
    //            data.originalContourIndex,
    //            data.leftmost_x, data.vertical_mid_y,
    //            data.area);
    //    }
    //    Logger::Information("----------------------------------");
    //}

    cv::rectangle(srcImg, roi, YellowA, 1); // ROI 영역 표시

    Logger::Information("--- Final Processed Points (Passed all filters) ---");
    for (size_t i = 0; i < finalResultPoints.size(); ++i) {
        const auto& data = finalResultPoints[i];
        Logger::Information(
            "Final Point {0}: Original Contour Index {1}, Point: ({2}, {3}), Original Area: {4}",
            (int)i,
            data.originalContourIndex,
            data.leftmost_x + roi.x, data.vertical_mid_y + roi.y,
            data.area
        );
    }
    Logger::Information("----------------------------------");

    showAndSaveImage("EnvGeo_End", srcImg);
}
