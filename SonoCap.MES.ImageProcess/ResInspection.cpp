#include "ResInspection.h"

#define ROI_X 165
#define ROI_Y 185
#define ROI_W 50
#define ROI_H 60

#define USE_DENSITY // 또는 주석처리하고 평균 밝기 쓸 수도 있음
//#define USE_FAST_CENTER

void MyOpenCVWrapper::ResInspection(cv::Mat& roiImage, ResResult& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) {
        result.verticalDist = -1;
        result.horizontalDist = -1;
        result.edgeDensity1 = -1;
        result.edgeDensity2 = -1;
        result.edgeDensity3 = -1;
        return;
    }

    cv::Mat gray;
    cv::cvtColor(roiImage, gray, cv::COLOR_BGRA2GRAY);
    showAndSaveImage("ResInspection_Gray", gray); // 이름 변경 (겹치지 않게)

    // showAndThreshold 함수를 통해 사용자 대화형으로 DR 값 조절
    showAndThreshold("ResInspection_DR_Adjust", gray); // 이름 변경

    // DR 클리핑 적용 (고정된 값 65, 70으로 다시 처리)
    ApplyLinearDRClip(gray, gray, 65, 70, true);
    
    showAndSaveImage("ResInspection_DR_Clipped", gray); // 이름 변경

    // ROI 설정
    cv::Rect roi(ROI_X, ROI_Y, ROI_W, ROI_H);
    if (roi.x < 0 || roi.y < 0 || roi.x + roi.width > gray.cols || roi.y + roi.height > gray.rows) {
        std::cerr << "Error: ROI is out of image bounds!" << std::endl;
        result.verticalDist = -1;
        result.horizontalDist = -1;
        result.edgeDensity1 = -1;
        result.edgeDensity2 = -1;
        result.edgeDensity3 = -1;
        return;
    }

    cv::Mat roiGray = gray(roi);
    cv::Mat binary;
    cv::threshold(roiGray, binary, 100, 255, cv::THRESH_BINARY);
    showAndSaveImage("ResInspection_BinaryROI", binary); // 이름 변경

    // 모폴로지 오프닝 (침식 후 팽창)
    cv::Mat eroded, restored;
    cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));
    cv::erode(binary, eroded, kernel);
    cv::dilate(eroded, restored, kernel);
    showAndSaveImage("ResInspection_Morphology", restored); // 이름 변경

    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(restored, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);

    std::vector<ContourData> allContourData;

    // 기준점 (roiImage의 우하단 모서리 픽셀) 정의
    cv::Point2f fixedRefPoint(static_cast<float>(roiImage.cols - 1), static_cast<float>(roiImage.rows - 1));

    // 모든 유효 윤곽선 데이터 수집
    for (const auto& contour : contours) {
        double currentArea = cv::contourArea(contour);
        if (currentArea < 4) { // 면적 임계값: 4 (필요시 조정)
            continue;
        }

        cv::Moments m = cv::moments(contour);
        if (m.m00 == 0) { // 면적이 0인 윤곽선 무시
            continue;
        }

        cv::Point2f relativeCenter(m.m10 / m.m00, m.m01 / m.m00); // roiGray 기준 상대 좌표
        cv::Point2f absoluteCenter = relativeCenter + cv::Point2f(ROI_X, ROI_Y); // roiImage 기준 절대 좌표

        double currentQuality;
        cv::Mat mask = cv::Mat::zeros(roiGray.size(), CV_8UC1);
        cv::drawContours(mask, std::vector<std::vector<cv::Point>>{contour}, -1, 255, cv::FILLED);

#ifdef USE_DENSITY
        cv::Mat edge, edgeMasked;
        cv::Canny(roiGray, edge, 100, 200); // Canny 임계값 (필요시 조정)
        edge.copyTo(edgeMasked, mask);
        int edgeCount = cv::countNonZero(edgeMasked);
        int areaCount = cv::countNonZero(mask);
        currentQuality = (areaCount > 0) ? static_cast<double>(edgeCount) / areaCount : -1;
#else
        cv::Scalar mean = cv::mean(roiGray, mask);
        currentQuality = mean[0]; // 평균 밝기 사용
#endif

        double distance_from_ref = cv::norm(absoluteCenter - fixedRefPoint);
        double angle_from_ref_rad = std::atan2(absoluteCenter.y - fixedRefPoint.y, absoluteCenter.x - fixedRefPoint.x);
        double angle_from_ref_deg = angle_from_ref_rad * 180.0 / CV_PI;

        allContourData.push_back({ absoluteCenter, currentQuality, currentArea, contour, distance_from_ref, angle_from_ref_rad, angle_from_ref_deg });
    }

    // 디버깅 목적: 모든 유효 윤곽선 데이터 출력
    std::cout << "--- All Contour Data Collected ---" << std::endl;
    for (size_t i = 0; i < allContourData.size(); ++i) {
        const auto& data = allContourData[i];
        std::cout << "Contour " << i << ": Center(" << data.center_abs.x << ", " << data.center_abs.y << ")"
            << ", Dist: " << data.distance_from_ref
            << ", Angle(deg): " << data.angle_from_ref_deg
            << ", Area: " << data.area
            << ", Quality: " << data.quality << std::endl;
    }
    std::cout << "----------------------------------" << std::endl;

    ContourData p1_data, p2_data, p3_data;
    bool p1_found = false, p2_found = false, p3_found = false;

    // ⭐ 요구 조건에 따른 P1, P2, P3 찾기 로직 ⭐
    //const double TARGET_DIST_MIN = 72.0; // 75 - 3
    //const double TARGET_DIST_MAX = 78.0; // 75 + 3

    const double TARGET_DISTANCE = 75.0; // 75 - 3
    const double DISTANCE_TOLERANCE = 3; // 75 + 3

    // 1. P1 찾기: 거리가 75 근처이고, 면적이 가장 큰 점
    double max_p1_area = -1.0;
    double max_p1_angle_rad = std::numeric_limits<double>::lowest(); // 가장 작은 double 값으로 초기화 (가장 큰 값 찾기 위함)

    for (const auto& data : allContourData) {
        if (std::abs(data.distance_from_ref - TARGET_DISTANCE) <= DISTANCE_TOLERANCE) {
        //if (data.distance_from_ref >= TARGET_DIST_MIN && data.distance_from_ref <= TARGET_DIST_MAX) {
            //if (data.area > max_p1_area) {
            if (data.angle_from_ref_rad > max_p1_angle_rad) {
                p1_data = data;
                max_p1_area = data.area;
                p1_found = true;
            }
        }
    }

    if (p1_found) {
        // 2. P2 찾기: P1을 제외하고, P1보다 거리가 크며, 면적이 가장 큰 점
        double max_p2_area = -1.0;
        for (const auto& data : allContourData) {
            if (data.center_abs == p1_data.center_abs) continue; // P1 제외

            if (data.distance_from_ref > p1_data.distance_from_ref) {
                if (data.area > max_p2_area) {
                    p2_data = data;
                    max_p2_area = data.area;
                    p2_found = true;
                }
            }
        }

        // 3. P3 찾기: P1, P2를 제외하고, 거리가 75 근처이며, P2보다 각도가 작고, 면적이 가장 큰 점
        double max_p3_area = -1.0;
        for (const auto& data : allContourData) {
            if (data.center_abs == p1_data.center_abs || (p2_found && data.center_abs == p2_data.center_abs)) continue; // P1, P2 제외

            //if (data.distance_from_ref >= TARGET_DIST_MIN && data.distance_from_ref <= TARGET_DIST_MAX && // 거리 75 근처
            if (std::abs(data.distance_from_ref - TARGET_DISTANCE) <= DISTANCE_TOLERANCE && // 거리 75 근처
                (p2_found && data.angle_from_ref_deg < p2_data.angle_from_ref_deg)) // P2보다 각도가 작음 (P2_found 조건 필요)
            {
                if (data.area > max_p3_area) {
                    p3_data = data;
                    max_p3_area = data.area;
                    p3_found = true;
                }
            }
        }
    }

    // 최종 결과 할당 및 시각화
    if (p1_found && p2_found && p3_found) {
        std::vector<cv::Scalar> selected_colors = { RedA, GreenA, BlueA }; // P1(Red), P2(Green), P3(Blue)

        // 모든 윤곽선 그리기 (P1, P2, P3은 특별한 색으로, 나머지는 회색)
        for (const auto& data : allContourData) {
            std::vector<std::vector<cv::Point>> shifted = { data.originalContour_rel };
            for (auto& pt : shifted[0])
                pt += cv::Point(ROI_X, ROI_Y); // ROI 오프셋 적용

            cv::Scalar color_to_draw = GrayA; // 기본은 회색
            if (data.center_abs == p1_data.center_abs) {
                color_to_draw = selected_colors[0]; // P1은 Red
            }
            else if (data.center_abs == p2_data.center_abs) {
                color_to_draw = selected_colors[1]; // P2는 Green
            }
            else if (data.center_abs == p3_data.center_abs) {
                color_to_draw = selected_colors[2]; // P3은 Blue
            }
            cv::drawContours(roiImage, shifted, -1, color_to_draw, 1);
        }

        // P1, P2, P3의 중심에 작은 원 표시
        cv::circle(roiImage, p1_data.center_abs, 2, selected_colors[0], -1);
        cv::circle(roiImage, p2_data.center_abs, 2, selected_colors[1], -1);
        cv::circle(roiImage, p3_data.center_abs, 2, selected_colors[2], -1);

        // 선택된 P1, P2, P3의 정보 출력
        std::cout << "\n--- Selected Points (P1, P2, P3) ---" << std::endl;
        std::cout << "P1: Center(" << p1_data.center_abs.x << ", " << p1_data.center_abs.y << ")"
            << ", Dist: " << p1_data.distance_from_ref
            << ", Angle(deg): " << p1_data.angle_from_ref_deg << ", Area: " << p1_data.area << ", Quality: " << p1_data.quality << std::endl;
        std::cout << "P2: Center(" << p2_data.center_abs.x << ", " << p2_data.center_abs.y << ")"
            << ", Dist: " << p2_data.distance_from_ref
            << ", Angle(deg): " << p2_data.angle_from_ref_deg << ", Area: " << p2_data.area << ", Quality: " << p2_data.quality << std::endl;
        std::cout << "P3: Center(" << p3_data.center_abs.x << ", " << p3_data.center_abs.y << ")"
            << ", Dist: " << p3_data.distance_from_ref
            << ", Angle(deg): " << p3_data.angle_from_ref_deg << ", Area: " << p3_data.area << ", Quality: " << p3_data.quality << std::endl;
        std::cout << "-------------------------------------" << std::endl;

        // 결과 할당
        result.verticalDist = cv::norm(p1_data.center_abs - p2_data.center_abs);
        result.horizontalDist = cv::norm(p1_data.center_abs - p3_data.center_abs);
        result.edgeDensity1 = p1_data.quality;
        result.edgeDensity2 = p2_data.quality;
        result.edgeDensity3 = p3_data.quality;

    }
    else {
        std::cerr << "Warning: Could not find all 3 required points (P1, P2, P3) matching criteria. Returning -1 for distances and qualities." << std::endl;
        result.verticalDist = -1;
        result.horizontalDist = -1;
        result.edgeDensity1 = -1;
        result.edgeDensity2 = -1;
        result.edgeDensity3 = -1;
    }

    cv::rectangle(roiImage, roi, YellowA, 1); // ROI 테두리 그리기
}

void MyOpenCVWrapper::ResInspection4(cv::Mat& roiImage, ResResult& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) return;

    cv::Mat gray;
    cv::cvtColor(roiImage, gray, cv::COLOR_BGRA2GRAY);
    showAndSaveImage("ResInspection", gray);

    // showAndThreshold 함수를 통해 사용자 대화형으로 DR 값 조절
    showAndThreshold("ResInspection", gray);

    // showAndThreshold에서 조절된 g_drmin, g_drmax 값을 ApplyLinearDRClip에 바로 사용하지 않고,
    // 이 코드에서는 여전히 65, 70으로 고정된 DR 클리핑을 수행하고 있습니다.
    // 만약 showAndThreshold로 조절한 값을 여기서도 쓰고 싶다면, 전역 변수 g_drmin, g_drmax를 여기에 전달해야 합니다.
    // 현재는 고정된 값 (65, 70)으로 한 번 더 처리하는 형태입니다.
    ApplyLinearDRClip(gray, gray, 65, 70, true);
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

    // 모든 유효한 윤곽선의 중심점, 품질, 면적을 저장할 벡터
    struct ContourData {
        cv::Point2f center_abs;         // roiImage 기준의 절대 좌표 중심점
        double quality;
        double area;                    // 면적을 저장하여 정렬에 사용
        std::vector<cv::Point> originalContour_rel; // roiGray 기준의 상대 윤곽선
        double distance_from_ref;       // 기준점 (W-1, H-1)까지의 거리 ⭐추가⭐
        double angle_from_ref;          // 기준점 (W-1, H-1)까지의 각도 (라디안) ⭐추가⭐
    };
    std::vector<ContourData> allContourData;

    // ⭐ 기준점 (roiImage의 우하단 모서리 픽셀) 정의
    cv::Point2f fixedRefPoint(static_cast<float>(roiImage.cols - 1), static_cast<float>(roiImage.rows - 1));

    for (const auto& contour : contours) {
        // 면적이 너무 작은 윤곽선 무시
        double currentArea = cv::contourArea(contour);
        if (currentArea < 4) {
            continue;
        }

        cv::Moments m = cv::moments(contour);
        // 면적이 0인 윤곽선 무시 (방어적 코드)
        if (m.m00 == 0) {
            continue;
        }

        cv::Point2f relativeCenter(m.m10 / m.m00, m.m01 / m.m00); // roiGray 기준의 상대 좌표 중심
        cv::Point2f absoluteCenter = relativeCenter + cv::Point2f(ROI_X, ROI_Y); // roiImage 기준의 절대 좌표 중심

        double currentQuality;
        // 마스크 및 퀄리티 계산
        cv::Mat mask = cv::Mat::zeros(roiGray.size(), CV_8UC1);
        cv::drawContours(mask, std::vector<std::vector<cv::Point>>{contour}, -1, 255, cv::FILLED);

#ifdef USE_DENSITY
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

        // ⭐ 기준점 (fixedRefPoint)까지의 거리 계산
        double distance_from_ref = cv::norm(absoluteCenter - fixedRefPoint);

        // ⭐ 기준점 (fixedRefPoint)까지의 각도 계산
        // atan2(y, x)에서 y는 (점의 y - 기준점의 y), x는 (점의 x - 기준점의 x)
        // atan2는 라디안 값을 반환하며, -PI에서 PI 사이의 범위 (Y축이 아래로 증가하는 OpenCV 좌표계)
        double angle_from_ref = std::atan2(absoluteCenter.y - fixedRefPoint.y, absoluteCenter.x - fixedRefPoint.x);

        allContourData.push_back({ absoluteCenter, currentQuality, currentArea, contour, distance_from_ref, angle_from_ref });
    }

    // 면적 기준으로 내림차순 정렬 (가장 큰 윤곽선이 앞으로 오도록)
    std::sort(allContourData.begin(), allContourData.end(), [](const ContourData& a, const ContourData& b) {
        return a.area > b.area; // 면적이 큰 순서대로 정렬
        });

    // ⭐ 현재 단계에서는 거리/각도에 따른 필터링을 하지 않고, 모든 윤곽선 데이터가 allContourData에 있습니다.
    // 이전 코드에서 상위 3개 윤곽선을 선택하던 부분은 주석 처리하거나 목적에 맞게 수정해야 합니다.
    // "몇개가 들어올지 몰라"라는 요구사항에 따라, 이제 allContourData에는 필터링되지 않은 (면적만으로 정렬된) 모든 유효 윤곽선이 있습니다.
    // 다음 단계에서 거리/각도 조건을 사용하여 필터링해야 합니다.

    // 최종 결과에 사용할 윤곽선 데이터 (여기서는 아직 필터링되지 않음)
    // 이전 코드는 finalCenters에 3개만 넣었지만, 여기서는 모든 데이터를 시각화할 수 있도록 변경합니다.
    // 거리 계산 및 결과 할당 부분도 다음 단계에서 필터링된 데이터에 따라 수정되어야 합니다.

    std::vector<cv::Scalar> colors = { RedA, GreenA, BlueA, YellowA, MagentaA, CyanA }; // 더 많은 색상 준비

    // 선택된 윤곽선을 원본 이미지에 그리기 (모든 유효 윤곽선을 그립니다)
    // ⭐ 시각화 목적: 각 점의 거리/각도 정보는 콘솔 출력 등으로 확인 가능
    for (int i = 0; i < allContourData.size(); ++i) {
        std::vector<std::vector<cv::Point>> shifted = { allContourData[i].originalContour_rel };
        for (auto& pt : shifted[0])
            pt += cv::Point(ROI_X, ROI_Y); // ROI 오프셋 적용하여 roiImage에 그릴 수 있도록 좌표 변환

        // 각 윤곽선에 다른 색상 부여 (없으면 회색)
        cv::Scalar color_to_draw = (i < colors.size()) ? colors[i] : cv::Scalar(128, 128, 128, 255);
        cv::drawContours(roiImage, shifted, -1, color_to_draw, 1);

        // ⭐ 각 점의 중심, 거리, 각도를 콘솔에 출력 (디버깅 목적)
        std::cout << "Contour " << i << ": Center(" << allContourData[i].center_abs.x << ", " << allContourData[i].center_abs.y << ")"
            << ", Distance from Ref: " << allContourData[i].distance_from_ref
            << ", Angle from Ref (rad): " << allContourData[i].angle_from_ref
            << ", Angle from Ref (deg): " << allContourData[i].angle_from_ref * 180.0 / CV_PI << std::endl;
    }

    // ⭐ 기준점 (우하단 모서리)을 작은 원으로 표시 (시각화 목적)
    //cv::circle(roiImage, fixedRefPoint, 3, GreenA, -1); // 초록색 채워진 원
    //cv::putText(roiImage, "Ref", cv::Point(fixedRefPoint.x - 20, fixedRefPoint.y - 10), cv::FONT_HERSHEY_SIMPLEX, 0.5, GreenA, 1);

    cv::rectangle(roiImage, roi, YellowA, 1);  // 밝은 노란색 1px 테두리
    showAndSaveImage("ResInspection", roiImage);

    // ⭐ 이 아래의 result 할당 및 거리 계산 로직은 현재 단계에서는 의미가 없습니다.
    // 왜냐하면 아직 거리/각도 조건에 따른 필터링이 이루어지지 않았고,
    // "몇개가 들어올지 몰라"라는 요구사항에 따라 3개의 점만 선택하는 로직을 제거해야 하기 때문입니다.
    // 이 부분은 다음 단계에서 "분류하자"는 요구사항에 맞춰 수정되어야 합니다.

    // 임시로 result 초기화 (추후 변경 필요)
    result.verticalDist = -1;
    result.horizontalDist = -1;
    result.edgeDensity1 = -1;
    result.edgeDensity2 = -1;
    result.edgeDensity3 = -1;
}


void MyOpenCVWrapper::ResInspection2(cv::Mat& roiImage, ResResult& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) return;

    cv::Mat gray;
    cv::cvtColor(roiImage, gray, cv::COLOR_BGRA2GRAY);
    showAndSaveImage("ResInspection", gray);

    // showAndThreshold 함수를 통해 사용자 대화형으로 DR 값 조절
    showAndThreshold("ResInspection", gray);

    // showAndThreshold에서 조절된 g_drmin, g_drmax 값을 ApplyLinearDRClip에 바로 사용하지 않고,
    // 이 코드에서는 여전히 65, 70으로 고정된 DR 클리핑을 수행하고 있습니다.
    // 만약 showAndThreshold로 조절한 값을 여기서도 쓰고 싶다면, 전역 변수 g_drmin, g_drmax를 여기에 전달해야 합니다.
    // 현재는 고정된 값 (65, 70)으로 한 번 더 처리하는 형태입니다.
    ApplyLinearDRClip(gray, gray, 65, 70, true);
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

    // 모든 유효한 윤곽선의 중심점, 품질, 면적을 저장할 벡터
    struct ContourData {
        cv::Point2f center;
        double quality;
        double area; // 면적을 저장하여 정렬에 사용
        std::vector<cv::Point> originalContour; // 원래 윤곽선을 저장하여 나중에 그릴 때 사용
    };
    std::vector<ContourData> allContourData;

    for (const auto& contour : contours) {
        // 면적이 너무 작은 윤곽선 무시
        double currentArea = cv::contourArea(contour);
        if (currentArea < 4) {
            continue;
        }

        cv::Moments m = cv::moments(contour);
        // 면적이 0인 윤곽선 무시 (방어적 코드)
        if (m.m00 == 0) {
            continue;
        }

        cv::Point2f center(m.m10 / m.m00, m.m01 / m.m00);

        double currentQuality;
        // 마스크 및 퀄리티 계산
        cv::Mat mask = cv::Mat::zeros(roiGray.size(), CV_8UC1);
        cv::drawContours(mask, std::vector<std::vector<cv::Point>>{contour}, -1, 255, cv::FILLED);

#ifdef USE_DENSITY
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

        allContourData.push_back({ center, currentQuality, currentArea, contour });
    }

    // 면적 기준으로 내림차순 정렬 (가장 큰 윤곽선이 앞으로 오도록)
    std::sort(allContourData.begin(), allContourData.end(), [](const ContourData& a, const ContourData& b) {
        return a.area > b.area; // 면적이 큰 순서대로 정렬
        });

    // 최종 결과에 사용할 상위 3개 윤곽선 데이터 선택
    std::vector<cv::Point2f> finalCenters;
    double finalQualities[3];
    std::vector<cv::Scalar> colors = { RedA, GreenA, BlueA }; // RedA, GreenA, BlueA는 정의되어 있어야 합니다.

    // 선택된 윤곽선을 원본 이미지에 그리기
    for (int i = 0; i < allContourData.size(); ++i) {
        // 모든 윤곽선을 그리는 대신, 상위 3개만 그리고 품질도 3개만 저장
        if (i < 3) {
            finalCenters.push_back(allContourData[i].center);
            finalQualities[i] = allContourData[i].quality;
        }

        // 결과 이미지에 표시 (상위 3개 윤곽선만 색깔을 다르게 표시)
        if (i < colors.size()) { // 상위 3개 (0, 1, 2 인덱스)는 색깔 입힘
            std::vector<std::vector<cv::Point>> shifted = { allContourData[i].originalContour };
            for (auto& pt : shifted[0])
                pt += cv::Point(ROI_X, ROI_Y);
            cv::drawContours(roiImage, shifted, -1, colors[i], 1);
        }
        else { // 4번째 윤곽선부터는 회색으로 그림 (선택 사항, 안그려도 됨)
            std::vector<std::vector<cv::Point>> shifted = { allContourData[i].originalContour };
            for (auto& pt : shifted[0])
                pt += cv::Point(ROI_X, ROI_Y);
            cv::drawContours(roiImage, shifted, -1, cv::Scalar(128, 128, 128, 255), 1); // 회색으로 그림
        }
    }
    cv::rectangle(roiImage, roi, YellowA, 1);  // 밝은 노란색 1px 테두리
    showAndSaveImage("ResInspection", roiImage);

    // 최종 결과 할당
    if (finalCenters.size() != 3) {
        result.verticalDist = -1;
        result.horizontalDist = -1;
        result.edgeDensity1 = -1;
        result.edgeDensity2 = -1;
        result.edgeDensity3 = -1;
        return;
    }

    // result.edgeDensity에 최종 품질 값 할당
    result.edgeDensity1 = finalQualities[0];
    result.edgeDensity2 = finalQualities[1];
    result.edgeDensity3 = finalQualities[2];

    // 중심점을 y축 기준으로 정렬하여 거리 계산 (여전히 y축 정렬은 필요)
    std::sort(finalCenters.begin(), finalCenters.end(), [](const cv::Point2f& a, const cv::Point2f& b) {
        return a.y < b.y;
        });

    for (auto& pt : finalCenters)
        pt += cv::Point2f(ROI_X, ROI_Y);

    result.verticalDist = cv::norm(finalCenters[0] - finalCenters[1]);
    result.horizontalDist = cv::norm(finalCenters[1] - finalCenters[2]);
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

void MyOpenCVWrapper::ResInspection3(cv::Mat& roiImage, ResResult& result)
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