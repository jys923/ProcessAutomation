#include "GeoInspection.h"

// ROI 정의 - 비율 기반으로 이미지 중앙에 위치한 가로 직선 형태
#define ROI_WIDTH_RATIO  0.7   // 이미지 너비의 80%
#define ROI_HEIGHT_RATIO 0.05  // 이미지 높이의 5%

void MyOpenCVWrapper::GeoInspection(cv::Mat& roiImage, std::string& resultText)
{
    if (roiImage.empty() || roiImage.channels() != 4) {
        resultText = R"({"error": "invalid input"})";
        return;
    }

    int imageWidth = roiImage.cols;
    int imageHeight = roiImage.rows;

    // ROI 설정 (중앙 수평 직선 형태)
    int roiWidth = static_cast<int>(imageWidth * ROI_WIDTH_RATIO);
    int roiHeight = static_cast<int>(imageHeight * ROI_HEIGHT_RATIO);
    int roiX = (imageWidth - roiWidth) / 2;
    int roiY = (imageHeight - roiHeight) / 2;

    cv::Rect roi(roiX, roiY, roiWidth, roiHeight);

    // 회색조 변환
    cv::Mat grayImage;
    cv::cvtColor(roiImage, grayImage, cv::COLOR_BGRA2GRAY);

    // ROI 내 통계 분석
    if (roi.x >= 0 && roi.y >= 0 &&
        roi.x + roi.width <= grayImage.cols &&
        roi.y + roi.height <= grayImage.rows)
    {
        cv::Mat roiMat = grayImage(roi);
        cv::Scalar mean, stddev;
        cv::meanStdDev(roiMat, mean, stddev);

        // 결과 시각화
        cv::rectangle(roiImage, roi, GreenA, 1);  // 초록색 사각형

        // 결과 JSON 작성
        std::ostringstream oss;
        oss << "{"
            << R"("roi":{"x":)" << roiX
            << R"(,"y":)" << roiY
            << R"(,"w":)" << roiWidth
            << R"(,"h":)" << roiHeight << "},"
            << R"("mean":)" << mean[0] << ","
            << R"("stddev":)" << stddev[0]
            << "}";

        resultText = oss.str();
    }
    else {
        resultText = R"({"error":"roi out of bounds"})";
    }
}

void MyOpenCVWrapper::GeoInspection2(cv::Mat& roiImage, GeoResult& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) return;

    int imageWidth = roiImage.cols;
    int imageHeight = roiImage.rows;

    // ROI 설정 (중앙 수평 직선 형태)
    int roiWidth = static_cast<int>(imageWidth * ROI_WIDTH_RATIO);
    int roiHeight = static_cast<int>(imageHeight * ROI_HEIGHT_RATIO);
    int roiX = (imageWidth - roiWidth) / 2;
    int roiY = (imageHeight - roiHeight) / 2;

    cv::Rect roi(roiX, roiY, roiWidth, roiHeight);

    // 회색조 변환
    cv::Mat grayImage;
    cv::cvtColor(roiImage, grayImage, cv::COLOR_BGRA2GRAY);

    // ROI 내 통계 분석
    if (roi.x >= 0 && roi.y >= 0 &&
        roi.x + roi.width <= grayImage.cols &&
        roi.y + roi.height <= grayImage.rows)
    {
        cv::Mat roiMat = grayImage(roi);
        cv::Scalar mean, stddev;
        cv::meanStdDev(roiMat, mean, stddev);

        // 결과 저장
        result.meanBrightness = mean[0];
        result.stdBrightness = stddev[0];

        // 시각화
        cv::rectangle(roiImage, roi, GreenA, 1);
    }
    else {
        result.meanBrightness = -1;
        result.stdBrightness = -1;
    }

}

#define NUM_SLICES 5
#define ROI_X  60
#define ROI_Y  (330 - 256)
#define ROI_W  400
#define ROI_H  (NUM_SLICES * 10)
#define MASK_OFFSET_Y    15  // ROI_Y에서 아래로 얼마나 떨어져 있는지 (조정 필요)
#define MASK_HEIGHT      40  // 마스크 타원의 높이 (조정 필요)

void MyOpenCVWrapper::GeoInspection(cv::Mat& roiImage, GeoResult& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) return;

    cv::Mat grayImage;
    cv::cvtColor(roiImage, grayImage, cv::COLOR_BGRA2GRAY);

    showAndSaveImage("Geo_Gray", grayImage);

    showAndThreshold("Geo_DR_Adjust", grayImage);

    ApplyLinearDRClip(grayImage, grayImage, 55, 60, true);
    showAndSaveImage("Geo_DR_Clipped", grayImage); // DR 클리핑된 이진화 이미지

    // --- 2. 마스크로 grayImage에 마스킹 ---
    cv::Mat mask = cv::Mat::zeros(grayImage.size(), CV_8UC1);

    // 마스크 타원의 중심 및 축 길이 계산
    cv::Point ellipse_center(ROI_X + ROI_W / 2, ROI_Y + MASK_OFFSET_Y + MASK_HEIGHT / 2);
    cv::Size ellipse_axes(ROI_W / 4, MASK_HEIGHT / 2); // ROI_W/4로 폭을 줄여 빨간 타원 너비에 맞춤

    // 마스크에 타원 그리기 (흰색으로 채움)
    cv::ellipse(mask, ellipse_center, ellipse_axes, 0, 0, 360, cv::Scalar(255), -1); // -1은 채우기

    // 마스크를 반전 (타원 영역은 0, 나머지는 255)
    cv::Mat mask_inv;
    // E0415: "void"에서 "cv::debug_build_guard::__InputArray" (으)로 변환하기 위한 적절한 생성자가 없습니다. 오류 해결
    // cv::bitwise_not 함수의 첫 번째 인자는 `InputArray` 타입이므로, `mask`를 `cv::InputArray` 타입으로 명시적으로 변환하거나,
    // `mask`가 이미 `cv::Mat` 타입이라면 바로 사용 가능합니다.
    // 기존 코드는 `mask`가 cv::Mat이므로 별도 변환 없이 사용할 수 있습니다.
    cv::bitwise_not(mask, mask_inv);
    cv::bitwise_and(grayImage, mask_inv, grayImage);

    cv::Mat fullImageMask = cv::Mat::zeros(grayImage.size(), CV_8UC1);

    // 마스킹에서 제외할 ROI 영역 정의
    // #define 으로 정의된 값들을 사용합니다.
    cv::Rect roiToKeep(ROI_X, ROI_Y, ROI_W, ROI_H);

    // 유효한 ROI인지 확인 (이미지 경계를 벗어나지 않도록)
    roiToKeep = roiToKeep & cv::Rect(0, 0, grayImage.cols, grayImage.rows);

    // ROI 영역만 흰색(255)으로 설정
    if (!roiToKeep.empty()) {
        fullImageMask(roiToKeep).setTo(cv::Scalar(255));
    }

    // grayImage에 이 마스크를 적용 (ROI 영역만 남기고 나머지는 검은색으로)
    cv::bitwise_and(grayImage, fullImageMask, grayImage);

    showAndSaveImage("Geo_Masked", grayImage); // 마스크 적용 결과 확인

    // 커널 정의: 연결 강도를 조절 (작을수록 약하게 연결, 클수록 강하게 연결)
    cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3)); // 3x3 사각형 커널 예시
    cv::dilate(grayImage, grayImage, kernel, cv::Point(-1, -1), 2); // 2번 팽창 (조정 필요)
    cv::erode(grayImage, grayImage, kernel, cv::Point(-1, -1), 2);  // 2번 침식 (조정 필요)
    showAndSaveImage("Geo_Dilated_Eroded", grayImage); // 중간 결과 확인

    // --- 3. 컨투어 찾아 너무 작은 거 빼고 모든 컨투어 1픽셀로 둘레 그리기 ---
    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(grayImage, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);

    // 가장 길쭉한 윤곽선을 찾기 위한 변수
    std::vector<cv::Point> bestContour;
    double maxAspectRatio = 0.0;
    result.isElongatedObjectFound = false; // 기본값은 찾지 못함으로 설정

    for (const auto& contour : contours)
    {
        double area = cv::contourArea(contour);

        // 면적 필터링: 너무 작은 윤곽선 제외 (임계값은 조정 필요)
        if (area < 1000) { // 예를 들어 50픽셀 미만은 무시. 이 값은 이미지에 맞춰 조정하세요.
            continue;
        }

        // 모든 컨투어를 1픽셀 두께, 랜덤 색상으로 그리기 (시각화용)
        //cv::drawContours(roiImage, std::vector<std::vector<cv::Point>>{contour}, -1, getRandomColor(), 1);

        // 윤곽선의 최소 사각형 및 종횡비 계산
        cv::RotatedRect minRect = cv::minAreaRect(contour);
        float width = minRect.size.width;
        float height = minRect.size.height;

        // 길이를 항상 더 큰 값으로, 너비를 더 작은 값으로 정렬하여 종횡비를 계산
        if (height > width) {
            std::swap(width, height);
        }

        double aspectRatio = (height > 0) ? (double)width / height : 0.0;

        // 가장 길쭉한 윤곽선 선택 (종횡비 기준)
        // 종횡비가 일정 값 이상이고, 현재까지의 최대 종횡비보다 크다면 업데이트
        if (aspectRatio > 5.0 && aspectRatio > maxAspectRatio) { // 10.0은 '길쭉함'을 판단하는 임계값, 조정 가능
            maxAspectRatio = aspectRatio;
            bestContour = contour;
            result.isElongatedObjectFound = true;
        }
    }

    // --- 4. 선택된 윤곽선에 대한 피팅 및 시각화 ---
    if (result.isElongatedObjectFound && bestContour.size() > 1)
    {
        int start_x = ROI_X + 1;
        int end_x = ROI_X + ROI_W - 1;

        // 1. 직선 피팅 (기존 방식)
        // fitLineAndDraw(roiImage, bestContour, start_x, end_x, YellowA);

        // 2.2.3.1.2.3. 다항식 피팅 (`fitPolynomialAndDraw` 수정됨)
        // 1.1 다항식 차수 증가 (3차 또는 4차) 및 1.2 데이터 정규화, 1.3 이상치 필터링 (간접 적용), 2.3 approxPolyDP 적용
        fitPolynomialAndDraw(roiImage, bestContour, start_x, end_x, CyanA, result);
    }

    // --- 기존 ROI 및 슬라이스 분석/시각화 로직 (유지) ---
    cv::Rect roi(ROI_X, ROI_Y, ROI_W, ROI_H);
    if (roi.x < 0 || roi.y < 0 ||
        roi.x + roi.width > grayImage.cols ||
        roi.y + roi.height > grayImage.rows)
    {
        result.meanBrightness = -1;
        result.stdBrightness = -1;
        result.maxSliceMean = -1;
        result.brightnessContrast = -1;
        result.maxSliceVariance = -1;
        return;
    }

    cv::Mat roiMat = grayImage(roi); // 마스크 및 모폴로지 적용된 grayImage에서 ROI 추출
    cv::Scalar mean, stddev;
    cv::meanStdDev(roiMat, mean, stddev);

    int sliceHeight = roiMat.rows / NUM_SLICES;
    std::vector<double> sliceMeans;
    std::vector<double> sliceVariances;

    for (int i = 0; i < NUM_SLICES; ++i)
    {
        int y0 = i * sliceHeight;
        int h = (i == NUM_SLICES - 1) ? roiMat.rows - y0 : sliceHeight;
        cv::Mat slice = roiMat(cv::Rect(0, y0, roiMat.cols, h));

        cv::Scalar sliceMean, sliceStddev;
        cv::meanStdDev(slice, sliceMean, sliceStddev);

        sliceMeans.push_back(sliceMean[0]);
        sliceVariances.push_back(sliceStddev[0] * sliceStddev[0]);
    }

    int brightestSliceIdx = std::distance(
        sliceMeans.begin(),
        std::max_element(sliceMeans.begin(), sliceMeans.end())
    );

    double maxSliceMean = sliceMeans[brightestSliceIdx];
    double contrastRatio = (maxSliceMean - mean[0]) / std::max(mean[0], 1.0);
    double maxSliceVariance = sliceVariances[brightestSliceIdx];

    result.meanBrightness = mean[0];
    result.stdBrightness = stddev[0];
    result.maxSliceMean = maxSliceMean;
    result.brightnessContrast = contrastRatio;
    result.maxSliceVariance = maxSliceVariance;
    result.elongatedObjectAspectRatio = maxAspectRatio; // 결과 구조체에 종횡비 저장

    cv::rectangle(roiImage, roi, RedA, 1);

    if (!sliceMeans.empty())
    {
        int y0 = ROI_Y + brightestSliceIdx * sliceHeight;
        int h = sliceHeight;

        cv::Rect brightSliceRect(ROI_X, y0, ROI_W, h);
        cv::rectangle(roiImage, brightSliceRect, GreenA, 1);
    }
}

void MyOpenCVWrapper::GeoInspection3(cv::Mat& roiImage, GeoResult& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) return;

    cv::Mat grayImage;
    cv::cvtColor(roiImage, grayImage, cv::COLOR_BGRA2GRAY);

    // 디버깅 및 시각화를 위한 이미지 저장/표시 (실제 배포 시에는 제거하거나 조건부 실행)
    showAndSaveImage("ResInspection_Gray", grayImage);

    // showAndThreshold 함수를 통해 사용자 대화형으로 DR 값 조절 (개발/테스트용)
    showAndThreshold("ResInspection_DR_Adjust", grayImage);

    // DR 클리핑 적용 (고정된 값으로 다시 처리)
    // 이 단계에서 grayImage는 사실상 이진화된 상태가 됩니다.
    ApplyLinearDRClip(grayImage, grayImage, 55, 60, true);

    cv::Point ellipse_center(ROI_X + ROI_W / 2, ROI_Y + MASK_OFFSET_Y + MASK_HEIGHT / 2);
    cv::Size ellipse_axes(ROI_W / 4, MASK_HEIGHT / 2); // ROI 너비를 장축, MASK_HEIGHT를 단축으로

    // roiImage에 노란색 타원 둘레 그리기
    // BGRa: Yellow = (0, 255, 255, 255), 두께 1
    cv::ellipse(roiImage, ellipse_center, ellipse_axes, 0, 0, 360, YellowA, 1);
    showAndSaveImage("ResInspection_DR_Adjust2", roiImage);

    showAndSaveImage("ResInspection_DR_Clipped", grayImage); // 이 이미지를 기반으로 덩어리 검출

    cv::Rect roi(ROI_X, ROI_Y, ROI_W, ROI_H);
    if (roi.x < 0 || roi.y < 0 ||
        roi.x + roi.width > grayImage.cols ||
        roi.y + roi.height > grayImage.rows)
    {
        // ROI가 유효하지 않으면 기본값 설정 후 반환
        result.meanBrightness = -1;
        result.stdBrightness = -1;
        result.maxSliceMean = -1;
        result.brightnessContrast = -1;
        result.maxSliceVariance = -1;
        result.elongatedObjectAspectRatio = -1; // 초기화
        result.isElongatedObjectFound = false;  // 초기화
        return;
    }

    cv::Mat roiMat = grayImage(roi);
    cv::Scalar mean, stddev;
    cv::meanStdDev(roiMat, mean, stddev);

    // 슬라이스 분할 (기존 로직 유지)
    int sliceHeight = roiMat.rows / NUM_SLICES;
    std::vector<double> sliceMeans;
    std::vector<double> sliceVariances;

    for (int i = 0; i < NUM_SLICES; ++i)
    {
        int y0 = i * sliceHeight;
        int h = (i == NUM_SLICES - 1) ? roiMat.rows - y0 : sliceHeight;
        cv::Mat slice = roiMat(cv::Rect(0, y0, roiMat.cols, h));

        cv::Scalar sliceMean, sliceStddev;
        cv::meanStdDev(slice, sliceMean, sliceStddev);

        sliceMeans.push_back(sliceMean[0]);
        sliceVariances.push_back(sliceStddev[0] * sliceStddev[0]);
    }

    int brightestSliceIdx = std::distance(
        sliceMeans.begin(),
        std::max_element(sliceMeans.begin(), sliceMeans.end())
    );

    double maxSliceMean = sliceMeans[brightestSliceIdx];
    double contrastRatio = (maxSliceMean - mean[0]) / std::max(mean[0], 1.0);
    double maxSliceVariance = sliceVariances[brightestSliceIdx];

    // 결과 저장 (기존 로직 유지)
    result.meanBrightness = mean[0];
    result.stdBrightness = stddev[0];
    result.maxSliceMean = maxSliceMean;
    result.brightnessContrast = contrastRatio;
    result.maxSliceVariance = maxSliceVariance;

    // --- 길쭉한 덩어리 검출 및 직선성 평가 로직 추가 ---
    result.elongatedObjectAspectRatio = -1; // 기본값으로 초기화
    result.isElongatedObjectFound = false;

    // DR 클리핑된 grayImage에서 윤곽선 찾기
    // findContours는 원본 이미지를 변경하므로, 복사본을 사용하는 것이 안전합니다.
    cv::Mat contouredImage = grayImage.clone();
    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(contouredImage, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);

    double bestAspectRatio = 0.0;
    std::vector<cv::Point> bestContour;
    cv::RotatedRect bestMinAreaRect; // 직선성 시각화를 위해 저장

    // 이미지의 대략적인 중앙 Y 좌표 범위 (필터링 기준)
    int imgHeight = grayImage.rows;
    int imgWidth = grayImage.cols;
    int center_y_range_min = static_cast<int>(imgHeight * 0.3); // 이미지 높이의 30% 지점부터
    int center_y_range_max = static_cast<int>(imgHeight * 0.4); // 70% 지점까지

    for (const auto& contour : contours)
    {
        double area = cv::contourArea(contour);

        // 1. 면적 필터링: 너무 작거나 큰 노이즈 제거 (임계값 조정 필요)
        // 예시 값: 1000 ~ 100000. 실제 이미지에 맞춰 미세 조정 필요.
        if (area < 1000 || area > 50000) { // 좀 더 넓은 범위로 시작
            continue;
        }

        // 윤곽선에 대해 최소 면적 사각형 계산
        cv::RotatedRect minRect = cv::minAreaRect(contour);
        float width = minRect.size.width;
        float height = minRect.size.height;

        // 길쭉한 덩어리는 가로로 길므로, 항상 width를 긴 쪽, height를 짧은 쪽으로 정규화
        if (height > width) {
            std::swap(width, height);
        }

        double aspectRatio = (height > 0) ? (double)width / height : 0.0;

        // 2. 종횡비 필터링: 가로로 길쭉한 형태 (예: 5 이상). 이 값도 조정 필요.
        if (aspectRatio < 5.0) { // 예시 값
            continue;
        }

        // 3. Y 위치 필터링: 이미지 중앙에 위치하는지 확인
        // minRect.center.y는 RotatedRect의 중심 Y 좌표
        if (!(minRect.center.y > center_y_range_min && minRect.center.y < center_y_range_max)) {
            continue;
        }

        // 모든 필터를 통과한 윤곽선 중 가장 종횡비가 큰 것을 선택 (가장 길쭉한 객체)
        if (aspectRatio > bestAspectRatio) {
            bestAspectRatio = aspectRatio;
            bestContour = contour;
            bestMinAreaRect = minRect;
            result.isElongatedObjectFound = true;
        }
    }

    if (result.isElongatedObjectFound) {
        result.elongatedObjectAspectRatio = bestAspectRatio;

        // 시각화: 가장 길쭉한 덩어리에 빨간색 테두리 그리기
        cv::drawContours(roiImage, std::vector<std::vector<cv::Point>>{bestContour}, -1, cv::Scalar(0, 0, 255, 255), 2); // RedA 대신 cv::Scalar 사용 예시

        // 시각화: 최소 면적 사각형 그리기 (녹색)
        cv::Point2f rect_points[4];
        bestMinAreaRect.points(rect_points);
        for (int j = 0; j < 4; j++) {
            cv::line(roiImage, rect_points[j], rect_points[(j + 1) % 4], cv::Scalar(0, 255, 0, 255), 2); // GreenA 대신 cv::Scalar 사용 예시
        }
    }

    // --- 기존 시각화 로직 (ROI 및 가장 밝은 슬라이스) ---
    cv::rectangle(roiImage, roi, cv::Scalar(0, 0, 255, 255), 1); // RedA 대신 cv::Scalar 사용 예시

    if (!sliceMeans.empty())
    {
        int y0 = ROI_Y + brightestSliceIdx * sliceHeight;
        int h = sliceHeight;

        cv::Rect brightSliceRect(ROI_X, y0, ROI_W, h);
        cv::rectangle(roiImage, brightSliceRect, cv::Scalar(0, 255, 0, 255), 1); // GreenA 대신 cv::Scalar 사용 예시
    }
}