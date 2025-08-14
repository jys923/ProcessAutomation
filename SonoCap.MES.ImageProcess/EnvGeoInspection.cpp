#include "EnvGeoInspection.h"

// 표준 편차 계산 헬퍼 함수
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
