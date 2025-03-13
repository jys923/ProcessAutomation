#include "AlignProcess.h"
#include "Util.h"

#define USE_CTYPE_CHECK true  // 1로 설정하면 C형 윤곽선 검사, 0으로 설정하면 타원 피팅 사용
// === 1. 변수 초기화 ===
#define BASE_RADIUS 29          // 기준 반지름 (29)
#define MASK_TOLERANCE 6          // 기준 반지름 (29)
#define RADIUS_TOLERANCE 4      // 반지름 허용 오차 (±4)
#define SEARCH_RANGE 3.0            // 점 검사 범위 (5x5)
#define CENTER_TOLERANCE 5.0        // 중심점 허용 오차
#define SIZE_TOLERANCE 5.0          // 크기 허용 오차

void MyOpenCVWrapper::AlignProcess(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr outputBuffer, System::IntPtr textBuffer) {

    // === 1. 변수 초기화 ===
#if USE_CTYPE_CHECK
#else
    double ellipseCircularity;               // 타원의 원형도
    cv::RotatedRect ellipse;                 // 타원 객체
    cv::Point2f ellipseCenter;               // 타원의 중심점
    cv::Size2f ellipseSize;                  // 타원의 크기
    bool isEllipseDuplicate;                 // 타원의 중복 여부
    cv::Point2f existingEllipseCenter;       // 기존 타원의 중심점
    cv::Size2f existingEllipseSize;          // 기존 타원의 크기
#endif
    std::string resultText = "FAIL";
    memcpy(textBuffer.ToPointer(), resultText.c_str(), resultText.size() + 1); // Include null terminator
    memset(outputBuffer.ToPointer(), 0, imageWidth * imageHeight * 4);
    // 기본 파라미터
    float contourRadius;
    double contourCircularity;  // 윤곽선의 원형도

    // 좌표 관련 변수
    cv::Point imageCenter, contourCenter;
    cv::Point2f contourCenterF;

    // 윤곽선과 점 데이터
    std::vector<cv::Point> circleContourPoints, openContourPoints, nearbyCirclePoints, filteredOpenPoints, mergedContourPoints;
    std::vector<std::vector<cv::Point>> filledContours, openContourTypes;
    std::vector<ContourInfo> contourInfoList;
    std::vector<cv::RotatedRect> filteredEllipses;

    // 이미지 처리용 Mat 객체들
    cv::Mat inputImage, circleImage, grayscaleImage, filledEdges;
    cv::Mat maskImage, equalizedImage, drawImage, drawImage2, resultImage;
    cv::Mat secondFilledEdges, finalEdges, openContourImage, closedContourImage;

    // === 2. 이미지 로드 ===
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    inputImage = cv::Mat(imageHeight, imageWidth, CV_8UC4, imageData);

    if (inputImage.empty()) {
        std::cerr << "Error: Image not found!" << std::endl;
        return;
    }
    showAndSaveImage(".\\Align\\inputImage", inputImage);

    // === 3. 이미지 및 초기 윤곽선 설정 ===
    imageCenter = cv::Point(inputImage.cols / 2.0f, inputImage.rows / 2.0f);

    // 기준 원형 포인트 생성
    circleImage = cv::Mat::zeros(inputImage.size(), inputImage.type());
    openContourPoints = extractCirclePoints(27, imageCenter);
    showAndSaveImage("circle", drawExtractedPoints(circleImage, openContourPoints));

    // 결과 및 작업용 이미지 준비
    resultImage = inputImage.clone();
    drawImage = inputImage.clone();
    drawImage2 = inputImage.clone();
    openContourImage = cv::Mat::zeros(inputImage.size(), inputImage.type());
    closedContourImage = cv::Mat::zeros(inputImage.size(), inputImage.type());

    // === 4. 이미지 전처리 ===
    cv::cvtColor(inputImage, grayscaleImage, cv::COLOR_BGRA2GRAY);

    // 도넛 형태 마스크 적용
    maskImage = createCircularMask(inputImage.size(), BASE_RADIUS - MASK_TOLERANCE, BASE_RADIUS + MASK_TOLERANCE);
    showAndSaveImage(".\\Align\\mask", maskImage);
    grayscaleImage.setTo(cv::Scalar(0, 0, 0), maskImage);
    showAndSaveImage(".\\Align\\gray_mask", grayscaleImage);

    // 이진화 및 엣지 검출
    cv::threshold(grayscaleImage, grayscaleImage, 70, 255, cv::THRESH_BINARY);
    showAndSaveImage(".\\Align\\threshold", grayscaleImage);
    
    // 이진화를 제거 하면
    //cv::Canny(grayscaleImage, filledEdges, 30, 255);
    cv::Canny(grayscaleImage, filledEdges, 50, 150);
    showAndSaveImage(".\\Align\\Canny", filledEdges);

    // === 5. 윤곽선 검출 및 필터링 ===
    cv::findContours(filledEdges, filledContours, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);

    if (filledContours.empty())
    {
        return;
    }

    // 디버깅용 윤곽선 표시
    for (size_t i = 0; i < filledContours.size(); i++) {
        cv::drawContours(drawImage, filledContours, (int)i, getRandomColor(), 2);
    }
    showAndSaveImage(".\\Align\\drawImage", drawImage);

    // 윤곽선 분석
    for (size_t i = 0; i < filledContours.size(); ++i) {
        cv::minEnclosingCircle(filledContours[i], contourCenterF, contourRadius);
        contourCenter = cv::Point(contourCenterF.x, contourCenterF.y);

        // 반지름과 중심 위치 조건 확인
        if (contourRadius >= (BASE_RADIUS - RADIUS_TOLERANCE) && contourRadius <= (BASE_RADIUS + RADIUS_TOLERANCE + 5) && cv::norm(contourCenter - imageCenter) < CENTER_TOLERANCE) {
            // O형 윤곽선 처리
            if (cv::isContourConvex(filledContours[i])) {
            //if (!isOpenCShape(filledContours[i], contourRadius)) {
                double circularity = calculateCircularity(filledContours[i]);
                contourInfoList.push_back({ filledContours[i], circularity, contourRadius });
            }
            // C형 윤곽선 처리
            else {
#if USE_CTYPE_CHECK
                // === C형 윤곽선 검사 모드 ===
                circleContourPoints = filledContours[i];

                // 검사 준비
                nearbyCirclePoints.clear();
                filteredOpenPoints.clear();
                mergedContourPoints.clear();

                // 이미지 초기화
                closedContourImage = cv::Mat::zeros(closedContourImage.size(), closedContourImage.type());
                openContourImage = cv::Mat::zeros(openContourImage.size(), openContourImage.type());

                // O형 윤곽선을 기준으로 C형 윤곽선 검사
                for (const auto& openPoint : openContourPoints) {
                    if (isNearCType(circleContourPoints, openPoint, SEARCH_RANGE)) {
                        // C형 픽셀이 근처에 있는 경우
                        for (const auto& circlePoint : circleContourPoints) {
                            if (norm(openPoint - circlePoint) < SEARCH_RANGE) {
                                nearbyCirclePoints.push_back(circlePoint);
                                mergedContourPoints.push_back(circlePoint);
                            }
                        }
                    }
                    else {
                        // C형 픽셀이 근처에 없는 경우
                        filteredOpenPoints.push_back(openPoint);
                        mergedContourPoints.push_back(openPoint);
                    }
                }

                // 디버깅용 이미지 생성
                drawPoints(closedContourImage, closedContourImage, nearbyCirclePoints, blue);
                drawPoints(closedContourImage, closedContourImage, filteredOpenPoints, green);
                showAndSaveImage(".\\Align\\closedContourImage", closedContourImage);

                // 병합된 윤곽선 그리기
                cv::drawContours(openContourImage, std::vector<std::vector<cv::Point>>{mergedContourPoints}, -1, green, 1);
                showAndSaveImage(".\\Align\\openContourImage", openContourImage);

                // 추가 엣지 검출 및 모폴로지 연산
                cv::Canny(openContourImage, secondFilledEdges, 30, 150);
                showAndSaveImage(".\\Align\\secondFilledEdges", secondFilledEdges);
                cv::morphologyEx(secondFilledEdges, finalEdges, cv::MORPH_CLOSE, cv::Mat::ones(3, 3, CV_8U));
                showAndSaveImage(".\\Align\\morphology", finalEdges);

                // 최종 윤곽선 검출 및 분석
                cv::findContours(finalEdges, openContourTypes, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);

                for (size_t i = 0; i < openContourTypes.size(); i++) {
                    cv::drawContours(drawImage2, openContourTypes, (int)i, getRandomColor(), 2);
                }
                showAndSaveImage(".\\Align\\drawImage2", drawImage2);

                for (size_t j = 0; j < openContourTypes.size(); ++j) {
                    if (openContourTypes[j].size() > 10) {
                        contourCircularity = calculateCircularity(openContourTypes[j]);
                        if (contourCircularity > 0.7) {
                            cv::minEnclosingCircle(openContourTypes[j], contourCenterF, contourRadius);
                            contourInfoList.push_back({ openContourTypes[j], contourCircularity, contourRadius });
                        }
                    }
                }
#else
                // === 타원 피팅 모드 ===
                ellipse = cv::fitEllipse(filledContours[i]);
                ellipseCenter = ellipse.center;
                ellipseSize = ellipse.size;

                // 중복 타원 검사
                isEllipseDuplicate = false;
                for (const auto& existingEllipse : filteredEllipses) {
                    existingEllipseCenter = existingEllipse.center;
                    existingEllipseSize = existingEllipse.size;

                    if (std::abs(ellipseCenter.x - existingEllipseCenter.x) < CENTER_TOLERANCE &&
                        std::abs(ellipseCenter.y - existingEllipseCenter.y) < CENTER_TOLERANCE &&
                        std::abs(ellipseSize.width - existingEllipseSize.width) < SIZE_TOLERANCE &&
                        std::abs(ellipseSize.height - existingEllipseSize.height) < SIZE_TOLERANCE) {
                        isEllipseDuplicate = true;
                        break;
                    }
                }

                if (!isEllipseDuplicate) {
                    filteredEllipses.push_back(ellipse);
                    cv::ellipse(resultImage, ellipse, cv::Scalar(0, 255, 0), 1);
                    cv::imshow("result", resultImage);
                    cv::waitKey(0);
                    ellipseCircularity = calculateCircularity(ellipse);

                    if (ellipseCircularity > 0.7) {
                        contourInfoList.push_back({ filledContours[i], ellipseCircularity, ellipse });
                        std::cout << "Ellipse Circularity: " << ellipseCircularity << ", Radius: " << contourRadius << std::endl;
                    }
                }
#endif
            }
        }
    }

    // === 6. 최적 윤곽선 선택 및 표시 ===
    std::sort(contourInfoList.begin(), contourInfoList.end(), compareCircularity);

    if (contourInfoList.empty()) {
        return;
    }

    const auto& bestContour = contourInfoList.front();
    std::cout << "Best Circularity: " << bestContour.circularity << std::endl;

    // 결과 이미지에 윤곽선 표시
    if (bestContour.ellipse.size.width > 0 && bestContour.ellipse.size.height > 0) {
        cv::ellipse(resultImage, bestContour.ellipse, red, 1);
    }
    else {
        cv::drawContours(resultImage, std::vector<std::vector<cv::Point>>{bestContour.contour}, -1, red, 1);
    }
    std::cout << "Circularity : " << bestContour.circularity << std::endl;

    // === 7. 결과 처리 ===
    showAndSaveImage(".\\Align\\resultImage", resultImage);

    // 결과 이미지 버퍼에 복사
    memcpy(outputBuffer.ToPointer(), resultImage.data, resultImage.total() * resultImage.elemSize());

    // 텍스트 결과 생성 및 버퍼에 복사
    //std::string textOutput = vectorToJsonString(contourInfoList);
    resultText = objectToJsonString(contourInfoList[0]);
    memcpy(textBuffer.ToPointer(), resultText.c_str(), resultText.size() + 1);
}
