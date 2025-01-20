#include "ResolutionProcess.h"
#include "Util.h"

#define inRadius 75
#define inRadiusMin inRadius - 5
#define inRadiusMax inRadius + 5
#define outRadius inRadius + 10
#define outRadiusMin outRadius - 5
#define outRadiusMax outRadius + 5
#define minArea 5.0
#define maxArea 30.0

#define MASK_MIN 70
#define MASK_MAX 95
#define IGNORE_BELOW 20

#define TEST_V1 false
#define THRESHOLD 150
#define GAMMA 2.0 //(1.2~2.0 추천)

void MyOpenCVWrapper::ResolutionProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer) {

    cv::Point imageCenter;
    std::vector<std::vector<cv::Point>> contours, filteredContours;
    std::vector<ArcData> arcDataInners;
    std::vector<ArcData> arcDataOutters;
    cv::Mat image, resultImage, equalizedImage, binaryImage, mask, drawImage;

    std::ostringstream textStream;

    uchar* data = static_cast<uchar*>(buffer.ToPointer());
    image = cv::Mat(height, width, CV_8UC4, data);
    if (image.empty()) {
        std::cerr << "Error: Image not found!" << std::endl;
        return;
    }
    showAndSaveImage(".\\Resolution\\inputImage", image);

    imageCenter = cv::Point(image.cols / 2, image.rows / 2);
    
    resultImage = image.clone();
    drawImage = image.clone();
    
    cv::cvtColor(image, image, cv::COLOR_BGRA2GRAY);

    //image = rotateImage(image, 300);
    //resultImage = rotateImage(resultImage, 300);

    mask = createCircularMask(image.size(), MASK_MIN, MASK_MAX);
    showAndSaveImage(".\\Resolution\\mask", mask);
    image.setTo(cv::Scalar(0, 0, 0), mask);
    showAndSaveImage(".\\Resolution\\gray_mask", image);

#if TEST_V1
    cv::Mat grad_x, grad_y, grad_mag, grad_dir;

    // 2️⃣ X 방향 그레이디언트 (소벨 필터)
    cv::Sobel(image, grad_x, CV_32F, 1, 0, 3);
    cv::Sobel(image, grad_y, CV_32F, 0, 1, 3);

    // 3️⃣ 그레이디언트 크기(Magnitude) 계산
    cv::magnitude(grad_x, grad_y, grad_mag);

    // 4️⃣ 그레이디언트 방향(Direction) 계산
    cv::phase(grad_x, grad_y, grad_dir, true);

    // 5️⃣ 0~255 범위로 변환 및 정규화
    cv::Mat abs_grad_x, abs_grad_y, abs_grad_mag, abs_grad_dir;
    cv::convertScaleAbs(grad_x, abs_grad_x);
    cv::convertScaleAbs(grad_y, abs_grad_y);
    cv::convertScaleAbs(grad_mag, abs_grad_mag);
    cv::convertScaleAbs(grad_dir, abs_grad_dir);

    cv::normalize(abs_grad_x, abs_grad_x, 0, 255, cv::NORM_MINMAX, CV_8U);
    cv::normalize(abs_grad_y, abs_grad_y, 0, 255, cv::NORM_MINMAX, CV_8U);
    cv::normalize(abs_grad_mag, abs_grad_mag, 0, 255, cv::NORM_MINMAX, CV_8U);
    cv::normalize(abs_grad_dir, abs_grad_dir, 0, 255, cv::NORM_MINMAX, CV_8U);

    // 6️⃣ 컬러맵 적용 (보기 좋게 시각화)
    cv::Mat colored_grad_x, colored_grad_y, colored_grad_mag, colored_grad_dir;
    cv::applyColorMap(abs_grad_x, colored_grad_x, cv::COLORMAP_JET);
    cv::applyColorMap(abs_grad_y, colored_grad_y, cv::COLORMAP_JET);
    cv::applyColorMap(abs_grad_mag, colored_grad_mag, cv::COLORMAP_JET);
    cv::applyColorMap(abs_grad_dir, colored_grad_dir, cv::COLORMAP_HSV);

    // 7️⃣ HSV 기반 방향 시각화 추가
    cv::Mat hsv_image(image.size(), CV_8UC3);
    for (int y = 0; y < image.rows; y++) {
        for (int x = 0; x < image.cols; x++) {
            float angle = grad_dir.at<float>(y, x);  // 방향값 (0~360도)
            float magnitude = grad_mag.at<float>(y, x);  // 크기값

            uchar hue = static_cast<uchar>((angle / 360.0) * 179);  // Hue (0~179 범위)
            uchar saturation = 255;  // 채도 최대
            uchar value = static_cast<uchar>(std::min(magnitude * 2, 255.0f));  // 크기에 따라 명도 조정

            hsv_image.at<cv::Vec3b>(y, x) = cv::Vec3b(hue, saturation, value);
        }
    }

    // HSV → BGR 변환
    cv::Mat direction_visualization;
    cv::cvtColor(hsv_image, direction_visualization, cv::COLOR_HSV2BGR);

    // 8️⃣ 이미지 저장
    showAndSaveImage(".\\Resolution\\grad_x", colored_grad_x);
    showAndSaveImage(".\\Resolution\\grad_y", colored_grad_y);
    showAndSaveImage(".\\Resolution\\grad_mag", colored_grad_mag);
    showAndSaveImage(".\\Resolution\\grad_dir", colored_grad_dir);
    showAndSaveImage(".\\Resolution\\grad_direction_visualization", direction_visualization);

    // 히스토그램 초기화 (IGNORE_BELOW 이상, 10단위 그룹화)
    int binCount = (255 - IGNORE_BELOW) / 10 + 1;  // 총 bin 개수
    std::vector<int> histogram(binCount, 0);
    std::vector<int> nonZeroValues;  // Otsu 계산을 위한 픽셀 저장

    // 픽셀 값 분석 (사용자 입력값 이하 제외)
    for (int y = 0; y < image.rows; y++) {
        for (int x = 0; x < image.cols; x++) {
            int pixel = image.at<uchar>(y, x);
            if (pixel >= IGNORE_BELOW) {  // 사용자가 입력한 값 이하 무시
                int binIndex = (pixel - IGNORE_BELOW) / 10;
                binIndex = std::min(binIndex, binCount - 1);  // 마지막 bin 처리
                histogram[binIndex]++;
                nonZeroValues.push_back(pixel);  // Otsu 임계값 계산을 위해 저장
            }
        }
    }
    // 히스토그램 출력 (터미널)
    std::cout << "Pixel Intensity Histogram (10-range bins, ignoring below " << IGNORE_BELOW << "):\n";
    for (int i = 0; i < binCount; i++) {
        std::cout << "[" << (IGNORE_BELOW + i * 10) << "-" << (IGNORE_BELOW + (i + 1) * 10 - 1) << "]: " << histogram[i] << "\n";
    }

    // 그래프 크기 설정
    int graphWidth = 600, graphHeight = 400;
    int barWidth = graphWidth / binCount;  // 막대 너비
    cv::Mat histogramImage = cv::Mat::ones(graphHeight, graphWidth, CV_8UC3) * 255;  // 흰색 배경

    // 히스토그램 정규화 (그래프 크기에 맞게 조정)
    int maxCount = *std::max_element(histogram.begin(), histogram.end());
    for (int i = 0; i < binCount; i++) {
        int barHeight = (histogram[i] * (graphHeight - 50)) / maxCount;  // 50은 여백 고려
        cv::rectangle(histogramImage,
            cv::Point(i * barWidth, graphHeight - barHeight),  // 왼쪽 위
            cv::Point((i + 1) * barWidth - 2, graphHeight),  // 오른쪽 아래
            cv::Scalar(0, 0, 255),  // 빨간색 막대
            cv::FILLED);
    }

    // 그래프 표시
    showAndSaveImage(".\\Resolution\\histogramImage", histogramImage);
    // 총 픽셀 개수 계산
    int totalPixels = std::accumulate(histogram.begin(), histogram.end(), 0);

    // 중위수(누적 개수 기준)로 임계값 찾기
    int cumulativeSum = 0;
    int medianThreshold = IGNORE_BELOW;
    for (int i = 0; i < binCount; i++) {
        cumulativeSum += histogram[i];
        if (cumulativeSum >= totalPixels * 0.7) {
            medianThreshold = IGNORE_BELOW + (i * 10);
            break;
        }
    }
    std::cout << "Median-based Threshold: " << medianThreshold << "\n";

    int weightedSum = 0;
    for (int i = 0; i < binCount; i++) {
        weightedSum += histogram[i] * (IGNORE_BELOW + i * 10);
    }
    int meanThreshold = weightedSum / totalPixels;
    std::cout << "Mean-based Threshold: " << meanThreshold << "\n";
    
    cv::Mat gammaCorrected, binaryImage4;
    double gamma = 2.0;  // 감마 값 조정 (1.2~2.0 추천)

    // 감마 보정 함수
    cv::Mat lut(1, 256, CV_8U);
    for (int i = 0; i < 256; i++) {
        lut.at<uchar>(i) = cv::saturate_cast<uchar>(pow(i / 255.0, gamma) * 255.0);
        if (i <= medianThreshold) {
            lut.at<uchar>(i) = cv::saturate_cast<uchar>(i * 0.5);  // 150 이하 밝기 줄이기
        }
        else {
            lut.at<uchar>(i) = i;  // 150 이상(255 포함) 그대로 유지
        }
    }

    cv::LUT(image, lut, gammaCorrected);

    // 이진화 적용
    cv::threshold(gammaCorrected, binaryImage4, medianThreshold, 255, cv::THRESH_BINARY);
    showAndSaveImage(".\\Resolution\\binaryImage4", binaryImage4);

    cv::Mat gradient;
    // Morphology Gradient로 경계 강조
    cv::morphologyEx(binaryImage4, gradient, cv::MORPH_OPEN, cv::Mat::ones(3, 3, CV_8U), cv::Point(-1, -1), 1);  // 작은 노이즈 제거
    showAndSaveImage(".\\Resolution\\gradient", gradient);

    cv::Mat enhanced, binaryImage3;
    // CLAHE 적용 (대비 향상)
    cv::Ptr<cv::CLAHE> clahe = cv::createCLAHE(2.0, cv::Size(8, 8));
    clahe->apply(image, enhanced);

    // Otsu 이진화 적용 (최적 임계값 자동 결정)
    cv::threshold(enhanced, binaryImage3, 0, 255, cv::THRESH_BINARY | cv::THRESH_OTSU);
    showAndSaveImage(".\\Resolution\\binaryImage3", binaryImage3);

    cv::Mat blurred;
    cv::bilateralFilter(image, blurred, 9, 75, 75);
    showAndSaveImage(".\\Resolution\\blurred", blurred);

    cv::equalizeHist(image, equalizedImage);
    showAndSaveImage(".\\Resolution\\equalizedImage", equalizedImage);

    //cv::threshold(image, binaryImage, 45, 255, cv::THRESH_BINARY);
    cv::threshold(image, binaryImage, 100, 255, cv::THRESH_BINARY);
    showAndSaveImage(".\\Resolution\\binaryImage", binaryImage);


    cv::Mat binaryImage2;
    cv::adaptiveThreshold(image, binaryImage2, 255,
        cv::ADAPTIVE_THRESH_GAUSSIAN_C,
        cv::THRESH_BINARY_INV, 11, 2);
    showAndSaveImage(".\\Resolution\\binaryImage2", binaryImage2);
#else
#endif
    cv::Mat gammaCorrected;
    double gamma = 2.0;  // 감마 값 조정 (1.2~2.0 추천)
    
    // 감마 보정 함수
    cv::Mat lut(1, 256, CV_8U);
    for (int i = 0; i < 256; i++) {
        lut.at<uchar>(i) = cv::saturate_cast<uchar>(pow(i / 255.0, gamma) * 255.0);
    }

    cv::LUT(image, lut, gammaCorrected);
    showAndSaveImage(".\\Resolution\\gammaCorrected", gammaCorrected);

    cv::threshold(gammaCorrected, binaryImage, THRESHOLD, 255, cv::THRESH_BINARY);
    showAndSaveImage(".\\Resolution\\binaryImage", binaryImage);
    cv::findContours(binaryImage, contours, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);

    if (contours.size() < 1 )
    {
        return;
    }

    // 디버깅용 윤곽선 표시
    for (size_t i = 0; i < contours.size(); i++) {
        cv::drawContours(drawImage, contours, (int)i, getRandomColor(), 2);
    }
    showAndSaveImage(".\\Resolution\\drawImage", drawImage);
    
    // 거리(distance) 표시
    cv::Mat drawImageDistance = drawImage.clone();
    
    for (size_t i = 0; i < contours.size(); i++) {
        cv::Rect boundingRect = cv::boundingRect(contours[i]);
        if (boundingRect.width <= image.cols - 10 && boundingRect.height <= image.rows - 10) {
            cv::drawContours(resultImage, contours, static_cast<int>(i), red, 1);

            cv::Point contourCenter(boundingRect.x + boundingRect.width / 2, boundingRect.y + boundingRect.height / 2);
            double distance = cv::norm(imageCenter - contourCenter);

            cv::line(drawImageDistance, imageCenter, contourCenter, getRandomColor(), 1);
            cv::putText(drawImageDistance, std::to_string(static_cast<int>(distance)),
                contourCenter, cv::FONT_HERSHEY_SIMPLEX, 0.4, getRandomColor(), 1);

            double angle = calculateAngle(imageCenter, contourCenter);
            double area = cv::contourArea(contours[i]);
            if (distance >= inRadiusMin && distance < inRadiusMax) {
                arcDataInners.push_back({ contourCenter, angle, area });
            }
            else if (distance >= outRadiusMin && distance < outRadiusMax) {
                arcDataOutters.push_back({ contourCenter, angle, area });
            }
        }
    }

    showAndSaveImage(".\\Resolution\\drawImageDistance", drawImageDistance);

    if (!arcDataOutters.empty()) {
        double minDistance = std::numeric_limits<double>::max();
        ArcData closestOutter;
        ArcData closestInner;

        for (const auto& arcCenterOutter : arcDataOutters) {
            for (const auto& arcCenterInner : arcDataInners) {
                double distance = cv::norm(arcCenterOutter.center - arcCenterInner.center);
                if (distance < minDistance) {
                    minDistance = distance;
                    closestOutter = arcCenterOutter;
                    closestInner = arcCenterInner;
                }
            }
        }

        // 여기에서 각도를 보정합니다
        double baseAngle = closestInner.angle;

        for (auto& arc : arcDataInners) {
            arc.angle -= baseAngle;
            if (arc.angle < 0) {
                arc.angle += 360;
            }
        }

        cv::line(resultImage, closestInner.center, closestOutter.center, cyan, 1);
        cv::putText(resultImage, "D : " + std::to_string(static_cast<int>(minDistance)), closestOutter.center, cv::FONT_HERSHEY_SIMPLEX, 0.3, yellow, 1);
        
        arcDataInners.erase(std::remove_if(arcDataInners.begin(), arcDataInners.end(),
            [](const ArcData& arc) {
                return arc.angle < 10 || arc.angle > 70;
            }), arcDataInners.end());
    }
    else
    {
        std::string textOutput = "fail";
        memcpy(textBuffer.ToPointer(), textOutput.c_str(), textOutput.size() + 1); // +1은 널 종료 문자를 포함하기 위해
        return;
    }

    // 각도(angle) 표시
    cv::Mat drawImageAngle = drawImage.clone();
    for (const auto& arc : arcDataInners) {
        cv::putText(drawImageAngle, std::to_string(static_cast<int>(arc.angle)),
            arc.center, cv::FONT_HERSHEY_SIMPLEX, 0.4, getRandomColor(), 1);
    }
    showAndSaveImage(".\\Resolution\\drawImageAngle", drawImageAngle);

    // 면적(area) 표시
    cv::Mat drawImageArea = drawImage.clone();
    for (const auto& arc : arcDataInners) {
        cv::putText(drawImageArea, std::to_string(static_cast<int>(arc.area)),
            arc.center, cv::FONT_HERSHEY_SIMPLEX, 0.4, getRandomColor(), 1);
    }
    showAndSaveImage(".\\Resolution\\drawImageArea", drawImageArea);

    std::sort(arcDataInners.begin(), arcDataInners.end(),
        [](const ArcData& a, const ArcData& b) {
            return a.angle < b.angle;
        });

    std::cout << "arcCenterInners:" << std::endl;
    for (const auto& pair : arcDataInners) {
        std::cout << "Point: (" << pair.center.x << ", " << pair.center.y << "), Angle: " << pair.angle << ", Area: " << pair.area << std::endl;
    }

    for (size_t i = 0; i < arcDataInners.size() - 1; ++i) {
        //double arcLength = calculateArcLength(imageCenter, arcCenterInners[i].first, arcCenterInners[i + 1].first, inRadius);
        double arcLength = calculateArcLength(arcDataInners[i].angle, arcDataInners[i + 1].angle, inRadius);
		arcDataInners[i].distanceToNext = arcLength;

        std::string label = "L(" + std::to_string(i + 1) + "," + std::to_string(i + 2) + "): " + std::to_string(static_cast<int>(arcLength));

        std::cout << label << std::endl;

        cv::putText(resultImage, label, (arcDataInners[i].center + arcDataInners[i + 1].center) / 2, cv::FONT_HERSHEY_SIMPLEX, 0.3, yellow, 1);
        //cv::line(resultImage, arcCenterInners[i].first, arcCenterInners[i + 1].first, green, 1);
    }

    cv::circle(resultImage, imageCenter, inRadius, green, 1);
    cv::circle(resultImage, imageCenter, outRadius, blue, 1);

    // resultImage와 텍스트 데이터를 버퍼로 복사합니다.
    memcpy(resultBuffer.ToPointer(), resultImage.data, resultImage.total() * resultImage.elemSize());
    //std::string textOutput = textStream.str();
    std::string textOutput = vectorToJsonString(arcDataInners);
    memcpy(textBuffer.ToPointer(), textOutput.c_str(), textOutput.size() + 1); // +1은 널 종료 문자를 포함하기 위해

    // 메모리 할당 상태에 따라 다양한 접근 방식을 사용할 수 있습니다.
    // 결과 이미지 표시 및 저장
    showAndSaveImage(".\\Resolution\\resultImage", resultImage);
}

