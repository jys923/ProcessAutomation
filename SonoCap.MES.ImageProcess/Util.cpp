#include "Util.h"
#include "RandomUtilities.h"

#include <opencv2/opencv.hpp>
#include <vector>
#include <cmath>

using namespace cv;
using namespace std;

// RANSAC 기반 타원 근사 함수
RotatedRect fitRotatedEllipseRANSAC(const  vector<Point>& points, int iter = 30, int sample_num = 10, double offset = 80.0) {
    int count_max = 0;
    vector<Point> effective_sample;

    random_device rd;
    mt19937 rng(rd());

    for (int i = 0; i < iter; i++) {
        vector<Point> sample;
        sample.reserve(sample_num);

        // 랜덤 샘플 선택
        for (int j = 0; j < sample_num; j++) {
            int idx = rng() % points.size();
            sample.push_back(points[idx]);
        }

        // 임시 타원 근사
        if (sample.size() >= 5) {
            RotatedRect ellipse = fitEllipse(sample);

            vector<Point> inliers;
            for (const Point& pt : points) {
                double dist = pointPolygonTest(sample, pt, true);
                if (fabs(dist) < offset) {
                    inliers.push_back(pt);
                }
            }

            if (inliers.size() > count_max) {
                count_max = inliers.size();
                effective_sample = inliers;
            }
        }
    }

    if (effective_sample.size() >= 5) {
        return fitEllipse(effective_sample);
    }
    else {
        return RotatedRect();
    }
}

// 일반적인 타원 근사 함수
RotatedRect fitRotatedEllipse(const vector<Point>& points) {
    if (points.size() >= 5) {
        return fitEllipse(points);
    }
    return RotatedRect();
}

// 메인 함수: 이미지 처리 및 타원 검출
void processImage(const string& imagePath) {
    Mat src = imread(imagePath, IMREAD_COLOR);
    if (src.empty()) {
        cerr << "이미지를 로드할 수 없습니다!" << endl;
        return;
    }

    Mat gray;
    cvtColor(src, gray, COLOR_BGR2GRAY);

    Mat thresh;
    threshold(gray, thresh, 0, 255, THRESH_BINARY + THRESH_OTSU);

    vector<vector<Point>> contours;
    findContours(thresh, contours, RETR_EXTERNAL, CHAIN_APPROX_SIMPLE);

    for (const auto& contour : contours) {
        double area = contourArea(contour);
        if (contour.size() > 10 && area > 30000) {
            // 일반 타원 근사 (보라색)
            RotatedRect ellipse1 = fitRotatedEllipse(contour);
            ellipse(src, ellipse1, Scalar(142, 56, 142), 2);

            // RANSAC을 이용한 타원 근사 (빨간색)
            RotatedRect ellipse2 = fitRotatedEllipseRANSAC(contour);
            ellipse(src, ellipse2, Scalar(0, 0, 255), 2);
        }
    }

    // 결과 저장
    imwrite("out.jpg", src);
    cout << "결과가 out.jpg에 저장되었습니다!" << endl;
}

//int main(int argc, char** argv) {
//    if (argc != 2) {
//        cerr << "사용법: " << argv[0] << " <이미지 파일 경로>" << endl;
//        return -1;
//    }
//
//    processImage(argv[1]);
//    return 0;
//}


// 📌 3️⃣ RMSE 기반 거리 오차 계산
double calculateRMSE(const vector<Point>& contour, const Vec4f& bestLine) {
    if (contour.empty()) return -1;

    // 📌 fitLine 결과에서 방향 벡터 및 기준점 추출
    double vx = bestLine[0], vy = bestLine[1];
    double x0 = bestLine[2], y0 = bestLine[3];

    // 📌 직선 방정식 설정: Ax + By + C = 0
    double A = -vy, B = vx, C = vy * x0 - vx * y0;

    // 📌 각 점이 직선으로부터 얼마나 떨어지는지 측정 (MSE 계산)
    double totalError = 0.0;
    for (const Point& pt : contour) {
        double distance = abs(A * pt.x + B * pt.y + C) / sqrt(A * A + B * B);
        totalError += distance * distance;
    }

    double mse = totalError / contour.size();  // MSE 계산
    return sqrt(mse);  // RMSE 반환
}

double evaluateContourStraightness(const vector<Point>& contour, Mat& resultImage) {
    if (contour.empty()) return -1;

    // 📌 `cv::fitLine()`을 사용하여 최적 직선 구하기
    Vec4f lineParams;
    fitLine(contour, lineParams, DIST_L2, 0, 0.01, 0.01);

    // 📌 fitLine() 결과를 이용하여 RMSE 계산
    double rmse = calculateRMSE(contour, lineParams);

    // 📌 직선 시각화 (빨간색)
    float vx = lineParams[0], vy = lineParams[1], x0 = lineParams[2], y0 = lineParams[3];
    int height = resultImage.rows;
    int y1 = 0, y2 = height;
    int x1 = int(x0 + (y1 - y0) * (vx / vy));
    int x2 = int(x0 + (y2 - y0) * (vx / vy));
    line(resultImage, Point(x1, y1), Point(x2, y2), Scalar(0, 0, 255), 2);

    // 컨투어 최대 길이 계산 (최대 RMSE 기준)
    double max_rmse = cv::arcLength(contour, false) / 10.0;  // 적절한 스케일 조정

    // 📌 새로운 직선성 평가 공식 (RMSE 기반)
    double straightnessScore = max(0.0, 1.0 - (rmse / max_rmse));

    return straightnessScore; // 값이 1에 가까울수록 직선
}

double calculateContourStraightnessMSE(const vector<Point>& contour, Mat& resultImage) {
    if (contour.empty()) return -1;

    // 📌 `cv::fitLine()`을 사용하여 최적 직선 구하기
    Vec4f lineParams;
    fitLine(contour, lineParams, DIST_L2, 0, 0.01, 0.01);

    // 📌 fitLine() 결과를 이용하여 RMSE 계산
    double rmse = calculateRMSE(contour, lineParams);

    // 📌 직선 시각화 (빨간색)
    float vx = lineParams[0], vy = lineParams[1], x0 = lineParams[2], y0 = lineParams[3];
    int height = resultImage.rows;
    int y1 = 0, y2 = height;
    int x1 = int(x0 + (y1 - y0) * (vx / vy));
    int x2 = int(x0 + (y2 - y0) * (vx / vy));
    line(resultImage, Point(x1, y1), Point(x2, y2), Scalar(0, 0, 255), 2);

    return rmse;  // RMSE 값 반환
}

double calculateContourStraightnessRANSAC(const vector<Point>& contour, Mat& resultImage, int iterations, double threshold) {
    if (contour.empty()) return -1;

    random_device rd;
    mt19937 rng(rd());
    uniform_int_distribution<int> dist(0, contour.size() - 1);

    Vec4f bestLine;
    int maxInliers = 0;

    // 1. RANSAC 반복
    for (int i = 0; i < iterations; i++) {
        // 랜덤하게 두 개의 점 선택
        Point p1 = contour[dist(rng)];
        Point p2 = contour[dist(rng)];

        // 두 점을 이용해 직선의 방정식 계산
        double A = p2.y - p1.y;
        double B = p1.x - p2.x;
        double C = p2.x * p1.y - p1.x * p2.y;

        // 인라이어 개수 계산
        int inliers = 0;
        for (const Point& pt : contour) {
            double distance = abs(A * pt.x + B * pt.y + C) / sqrt(A * A + B * B);
            if (distance < threshold) inliers++;
        }

        // 가장 많은 인라이어를 포함하는 직선 업데이트
        if (inliers > maxInliers) {
            maxInliers = inliers;
            bestLine = Vec4f(A, B, p1.x, p1.y);
        }
    }

    // 2. 최적 직선 계산 완료
    if (maxInliers == 0) return -1;  // 직선을 찾지 못한 경우

    double A = bestLine[0], B = bestLine[1], x0 = bestLine[2], y0 = bestLine[3];

    // **기울기 벡터 계산 (vx, vy)**
    double vx = -B; // x 방향
    double vy = A;  // y 방향

    // 3. 최적 직선과의 거리 계산 (MSE 또는 RMSE)
    double totalError = 0.0;
    for (const Point& pt : contour) {
        double distance = abs(A * pt.x + B * pt.y - A * x0 - B * y0) / sqrt(A * A + B * B);
        totalError += distance * distance;
    }

    double mse = totalError / contour.size();
    double rmse = sqrt(mse);

    // 4. 올바른 직선 시각화 (빨간색)
    int height = resultImage.rows;
    int width = resultImage.cols;

    // y1, y2를 화면 위아래 끝으로 설정하고, x1, x2를 계산
    int y1 = 0, y2 = height;
    int x1 = int(x0 + (y1 - y0) * (vx / vy));  // 직선의 방향을 고려하여 x 계산
    int x2 = int(x0 + (y2 - y0) * (vx / vy));

    // 직선 그리기
    line(resultImage, Point(x1, y1), Point(x2, y2), Scalar(0, 0, 255), 2);

    return rmse;  // RMSE 값을 반환 (값이 작을수록 직선에 가까움)
}

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