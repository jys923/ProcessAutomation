#include "Util.h"
#include "RandomUtilities.h"

void processLogNormalization(const cv::Mat& input, cv::Mat& output, double dr_min, double dr_max) {
    // 상수값 설정
    const double ln10_inv_mul20 = 8.685890; // 20 / ln(10)
    const double absolute_max_db = std::log10(std::sqrt(2.0) * 32768) * 20;

    // 사용자 정의 DR min/max를 dB로 변환
    double dr_min_db = absolute_max_db * (dr_min / 100.0);
    double dr_max_db = absolute_max_db * (dr_max / 100.0);

    // 입력 데이터를 float으로 변환
    cv::Mat floatInput;
    input.convertTo(floatInput, CV_32F);

    // 로그 변환 적용 (log(1.0) 이상으로 보정)
    cv::Mat logResult;
    cv::log(cv::max(floatInput, 1.0), logResult);

    // dB 변환 및 동적 범위 조정
    logResult = logResult * ln10_inv_mul20 - dr_min_db;
    cv::max(logResult, 0.0, logResult); // 최소값 보정

    // 0~255 정규화
    logResult = (logResult / (dr_max_db - dr_min_db)) * 255.0;
    cv::min(logResult, 255.0, logResult); // 최대값 보정
    logResult.convertTo(output, CV_8U); // uint8 변환
}

//void processLogNormalization(const cv::Mat& input, cv::Mat& output, double dr_min, double dr_max) {
//    // 상수값 설정
//    const double ln10_inv_mul20 = 8.685890; // 20 / ln(10)
//    const double absolute_max_db = std::log10(std::sqrt(2.0) * 32768) * 20;
//
//    // 사용자 정의 DR min/max를 dB로 변환
//    double dr_min_db = absolute_max_db * (dr_min / 100.0);
//    double dr_max_db = absolute_max_db * (dr_max / 100.0);
//
//    // RGBA → Grayscale 변환 (모든 채널이 동일한 값을 가지므로, 하나만 사용)
//    cv::Mat grayInput;
//    cv::cvtColor(input, grayInput, cv::COLOR_BGRA2GRAY);
//
//    // float 변환
//    cv::Mat floatInput;
//    grayInput.convertTo(floatInput, CV_32F);
//
//    // 로그 변환 (log(1.0) 이상으로 보정)
//    floatInput += 1.0f; // 최소값 보정
//    cv::log(floatInput, floatInput);
//
//    // dB 변환 및 동적 범위 조정
//    cv::Mat logResult = (floatInput * ln10_inv_mul20) - dr_min_db;
//    cv::max(logResult, 0.0, logResult); // 최소값 보정
//
//    // 0~255 정규화
//    logResult = (logResult / (dr_max_db - dr_min_db)) * 255.0;
//    cv::min(logResult, 255.0, logResult); // 최대값 보정
//    logResult.convertTo(logResult, CV_8U); // uint8 변환
//
//    // 다시 RGBA로 변환
//    cv::Mat channels[] = { logResult, logResult, logResult, cv::Mat::ones(logResult.size(), CV_8U) * 255 };
//    cv::merge(channels, 4, output);
//}

#if ENABLE_IMAGE_DISPLAY
void showAndSaveImage(const std::string& windowName, const cv::Mat& image) {
    if (image.empty()) {
        std::cerr << "Error: Image is empty!" << std::endl;
        return;
    }

    cv::imshow(windowName, image);
    cv::waitKey(0);

    std::string filename = windowName + ".bmp";
    cv::imwrite(filename, image);
}
#else
// 빈 함수 정의 (호출은 남아있지만 아무 동작 안 함)
void showAndSaveImage(const std::string&, const cv::Mat&) {}
#endif


// 랜덤 색상 생성 함수 (OpenCV Scalar 반환)
cv::Scalar getRandomColor() {
    RandomUtilities randomUtil;
    return cv::Scalar(randomUtil.getRandomInt(), randomUtil.getRandomInt(), randomUtil.getRandomInt());
}

bool compareCircularity(const ContourInfo& a, const ContourInfo& b) {
    return a.circularity > b.circularity;
}

cv::Mat createCircularMask(const cv::Size& size, int innerRadius, int outerRadius) {
    cv::Mat mask = cv::Mat::zeros(size, CV_8U);
    cv::Point center(size.width / 2, size.height / 2);
    cv::circle(mask, center, outerRadius, cv::Scalar(255), -1);
    cv::circle(mask, center, innerRadius, cv::Scalar(0), -1);
    cv::bitwise_not(mask, mask);
    return mask;
}

double calculateAngle(const cv::Point& center, const cv::Point& point) {
    double radian = std::atan2(center.y - point.y, point.x - center.x);
    if (radian < 0)
    {
        radian += 2 * CV_PI;
    }
    double degree = radian * 180 / CV_PI; // 라디안에서 도로 변환
    return degree;
}

double calculateArcLength(const cv::Point& center, const cv::Point& p1, const cv::Point& p2, double radius) {
    double radian = std::atan2(center.y - p2.y, p2.x - center.x) - std::atan2(center.y - p1.y, p1.x - center.x);
    if (radian < 0)
    {
        radian += 2 * CV_PI;
    }
    return radius * radian;
}

double calculateArcLength(double angle1, double angle2, double radius) {
    double degreeDiff = std::abs(angle2 - angle1); // 각도 차이 (도 단위)
    if (degreeDiff > 180) {
        degreeDiff = 360 - degreeDiff;  // 더 짧은 방향으로 계산
    }

    double radian = degreeDiff * CV_PI / 180.0; // 도 → 라디안 변환
    return radius * radian;
}


double calculateCircularity(const cv::RotatedRect& ellipse) {
    double a = ellipse.size.width / 2.0; // 장축의 반지름
    double b = ellipse.size.height / 2.0; // 단축의 반지름
    double area = CV_PI * a * b; // 타원의 면적
    double perimeter = CV_PI * (3 * (a + b) - sqrt((3 * a + b) * (a + 3 * b))); // 타원의 둘레
    return (4 * CV_PI * area) / (perimeter * perimeter);
}

double calculateCircularity(const std::vector<cv::Point>& contour) {
    double area = cv::contourArea(contour);
    double perimeter = cv::arcLength(contour, true);
    return (4 * CV_PI * area) / (perimeter * perimeter);
}

void calculateAndDisplayHistogram(const cv::Mat& inputImage, cv::Mat& outputImage) {
    int histSize = 256;  // 빈(bin)의 수
    float range[] = { 0, 256 };  // 히스토그램 범위
    const float* histRange = { range };
    cv::Mat hist;

    // 히스토그램 계산
    cv::calcHist(&inputImage, 1, 0, cv::Mat(), hist, 1, &histSize, &histRange);

    // 히스토그램을 정규화하여 보여주기 쉽게 함
    int histHeight = 400; // 히스토그램 높이
    int histWidth = 512; // 히스토그램 너비
    int binWidth = std::round((double)histWidth / histSize);
    outputImage.create(histHeight, histWidth, CV_8UC1);
    outputImage = cv::Scalar(255);

    // 히스토그램을 정규화합니다.
    cv::normalize(hist, hist, 0, outputImage.rows, cv::NORM_MINMAX);

    // 히스토그램 그리기
    for (int i = 1; i < histSize; i++) {
        cv::line(outputImage,
            cv::Point(binWidth * (i - 1), histHeight - std::round(hist.at<float>(i - 1))),
            cv::Point(binWidth * i, histHeight - std::round(hist.at<float>(i))),
            cv::Scalar(0), 2, 8, 0);
    }

    // 히스토그램 이미지 출력
    //cv::imshow("Histogram", outputImage);
}

std::vector<int> calculateRadii(const std::vector<std::vector<cv::Point>>& contours) {
    std::vector<int> radii;
    for (const auto& contour : contours) {
        cv::Rect boundingRect = cv::boundingRect(contour);
        int radius = (boundingRect.width + boundingRect.height) / 4;
        radii.push_back(radius);
    }
    return radii;
}

bool isNearCType(const std::vector<cv::Point>& cType, const cv::Point& oPt, int range) {
    for (const auto& cPt : cType) {
        if (norm(oPt - cPt) < range) {
            return true;
        }
    }
    return false;
}

bool isOpenCShape(const std::vector<cv::Point>& contour) {
    // 윤곽선의 둘레 계산
    double contourLength = cv::arcLength(contour, true);

    // 윤곽선을 둘러싸는 최소 원을 찾기
    cv::Point2f center;
    float radius;
    cv::minEnclosingCircle(contour, center, radius);

    // 최소 원의 둘레 계산
    double circlePerimeter = 2 * CV_PI * radius;

    // 윤곽선의 둘레와 최소 원의 둘레 비교
    // 예를 들어, 윤곽선의 둘레가 최소 원의 둘레보다 큰 경우 열려 있을 가능성이 높다고 판단
    return contourLength > circlePerimeter;
}

bool isOpenCShape(const std::vector<cv::Point>& contour, float radius) {
    // 윤곽선의 둘레 계산
    double contourLength = cv::arcLength(contour, true);

    // 최소 원의 둘레 계산
    double circlePerimeter = 2 * CV_PI * radius;

    // 윤곽선의 둘레와 최소 원의 둘레 비교
    // 예를 들어, 윤곽선의 둘레가 최소 원의 둘레보다 큰 경우 열려 있을 가능성이 높다고 판단
    return contourLength > circlePerimeter;
}

bool isOpenCShape(const std::vector<cv::Point>& contour, double minRadius, double maxRadius, const cv::Point2f& imageCenter, const cv::Point2f& center, float radius) {
    double minLength = minRadius * 2 * CV_PI - 10; // minRadius와 원주율을 사용하여 minLength 계산

    if (radius < minRadius || radius > maxRadius) {
        return true;
    }

    double contourLength = cv::arcLength(contour, true);
    if (contourLength > minLength) {
        return true;
    }

    if (cv::norm(center - imageCenter) >= 1.0) {
        return true;
    }

    return false;
}

std::vector<cv::Point> extractCirclePoints(int radius, const cv::Point& center) {
    std::vector<cv::Point> circlePoints; // 원을 그리기 위한 점들을 저장할 벡터

    // 라디안을 직접 사용하여 각도를 작은 증가량으로 세분화
    for (double angle = 0; angle < 2 * CV_PI; angle += 0.01) {
        double x = center.x + radius * cos(angle);
        double y = center.y + radius * sin(angle);
        cv::Point newPoint(static_cast<int>(x), static_cast<int>(y));
        circlePoints.push_back(newPoint); // 원을 그리기 위한 점 추가
    }

    return circlePoints;
}

void drawCircleUsingOpenCV() {
    cv::Mat image = cv::Mat::zeros(512, 512, CV_8UC3); // 검정 바탕 생성
    cv::Point imageCenter(256, 256); // 이미지 중심점 설정 (512x512이므로 중심은 256, 256)
    int radius = 27; // 원의 반지름 설정

    // OpenCV 함수로 원 그리기
    cv::circle(image, imageCenter, static_cast<int>(radius), cv::Scalar(0, 255, 255), 1); // 노란색 원으로 채우기

    // 원 이미지 표시
    cv::imshow("Circle Using OpenCV", image);
    cv::waitKey(0);
}

void drawExtractedCirclePoints() {
    cv::Point imageCenter(256, 256); // 이미지 중심점 설정 (512x512이므로 중심은 256, 256)
    int radius = 27; // 원의 반지름 설정

    // 원의 점들 추출
    std::vector<cv::Point> circlePoints = extractCirclePoints(radius, imageCenter);

    cv::Mat image = cv::Mat::zeros(512, 512, CV_8UC4); // 검정 바탕 생성
    // 추출된 점들 시각화
    for (const auto& point : circlePoints) {
        image.at<cv::Vec4b>(point) = cv::Vec4b(0, 255, 255, 255); // 노란색 점으로 표시
    }

    // 원 이미지 표시
    cv::imshow("Extracted Circle Points", image);
    cv::waitKey(0);
}

// 점들을 이미지에 표시하는 함수
cv::Mat drawExtractedPoints(const cv::Mat& image, const std::vector<cv::Point>& points) {
    cv::Mat result = image.clone(); // 입력 이미지를 복사하여 새로운 Mat 생성

    for (const auto& point : points) {
        if (point.x >= 0 && point.x < result.cols && point.y >= 0 && point.y < result.rows) {
            result.at<cv::Vec4b>(point) = cv::Vec4b(0, 255, 255, 255); // 노란색 점
        }
    }
    return result; // 수정된 이미지 반환
}


void drawPreciseCirclePoints() {
    cv::Mat image = cv::Mat::zeros(512, 512, CV_8UC3); // 검정 바탕 생성
    cv::Point imageCenter(256, 256); // 이미지 중심점 설정 (512x512이므로 중심은 256, 256)
    int radius = 27; // 원의 반지름 설정

    // 라디안을 직접 사용하여 각도를 작은 증가량으로 세분화 0.001 같음
    for (double angle = 0; angle < 2 * CV_PI; angle += 0.01) {
        double x = imageCenter.x + radius * cos(angle);
        double y = imageCenter.y + radius * sin(angle);
        cv::Point newPoint(static_cast<int>(x), static_cast<int>(y));
        image.at<cv::Vec3b>(newPoint) = cv::Vec3b(0, 255, 255); // 노란색 점으로 표시
    }

    // 원 이미지 표시
    cv::imshow("Yellow Circle on Black Background", image);
    cv::waitKey(0);
}

void drawPoints(const cv::Mat& inputImage, cv::Mat& outputImage, const std::vector<cv::Point>& points, const cv::Scalar& color) {
    // 입력 이미지를 복사하여 작업할 이미지 생성
    outputImage = inputImage.clone();

    // 추출된 점들 시각화
    for (const auto& point : points) {
        // 이미지를 넘어서는 점을 그리지 않도록 범위 체크
        if (point.x >= 0 && point.x < outputImage.cols && point.y >= 0 && point.y < outputImage.rows) {
            outputImage.at<cv::Vec4b>(point) = cv::Vec4b(color[0], color[1], color[2], 255); // 지정된 색상으로 점 표시
        }
    }
}

cv::Mat rotateImage(const cv::Mat& image, double angle)
{
    // 이미지의 중심 점 계산
    cv::Point2f center(image.cols / 2.0F, image.rows / 2.0F);

    // 회전 변환 행렬 생성
    cv::Mat rotMat = cv::getRotationMatrix2D(center, angle, 1.0);

    // 원형 회전 변환 수행
    cv::Mat rotatedImage;
    cv::warpAffine(image, rotatedImage, rotMat, image.size(), cv::INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));

    return rotatedImage;
}