#include "Util.h"

using namespace cv;
using namespace std;

std::vector<cv::Point> removeOutliersIQR(const std::vector<cv::Point>& points, double k_factor)
{
    if (points.empty()) {
        return {};
    }

    // 1. Y 좌표만 추출하여 정렬
    std::vector<int> y_coords;
    for (const auto& p : points) {
        y_coords.push_back(p.y);
    }
    std::sort(y_coords.begin(), y_coords.end());

    // 2. Q1 (1사분위수) 및 Q3 (3사분위수) 계산
    // 배열 크기가 짝수일 경우 중앙값 계산 방식에 따라 다를 수 있으나,
    // 여기서는 간단히 인덱스를 기반으로 합니다.
    int q1_idx = y_coords.size() / 4;
    int q3_idx = (y_coords.size() * 3) / 4;

    double Q1 = y_coords[q1_idx];
    double Q3 = y_coords[q3_idx];

    // 3. IQR (사분위 범위) 계산
    double IQR = Q3 - Q1;

    // 4. 이상치 경계 계산
    double lower_bound = Q1 - k_factor * IQR;
    double upper_bound = Q3 + k_factor * IQR;

    // 5. 이상치 제거 (경계 내에 있는 점들만 포함)
    std::vector<cv::Point> filtered_points;
    for (const auto& p : points) {
        if (p.y >= lower_bound && p.y <= upper_bound) {
            filtered_points.push_back(p);
        }
    }
    return filtered_points;
}

// 스플라인 근사 함수 (OpenCV에 직접적인 스플라인 피팅 없음, 근사로 대체)
void fitSplineApproximationAndDraw(cv::Mat& roiImage, const std::vector<cv::Point>& contour,
    const cv::Scalar& color)
{
    if (contour.size() < 2) return;

    // 윤곽선을 근사하여 제어점 수를 줄임 (스플라인에 사용할 제어점)
    // epsilon 값 조정으로 근사 정도 조절
    double epsilon = cv::arcLength(contour, false) * 0.01; // 윤곽선 길이의 1%
    std::vector<cv::Point> approxCurve;
    cv::approxPolyDP(contour, approxCurve, epsilon, false);

    // 근사된 점들을 연결하여 곡선처럼 그리기
    // 실제 스플라인 보간은 아니지만, 시각적으로 곡선 형태를 보여줌
    for (size_t i = 0; i < approxCurve.size() - 1; ++i) {
        cv::line(roiImage, approxCurve[i], approxCurve[i + 1], color, 2, cv::LINE_AA);
    }
    // 스플라인의 경우 곡률 계산이 다소 복잡하므로, 여기서는 생략하거나
    // 필요 시 다항식 피팅 후 곡률 계산 로직을 여기에 통합해야 합니다.
    // 현재 함수에서는 result 구조체에 곡률 정보를 업데이트하지 않습니다.
}

void fitPolynomialAndDraw(cv::Mat& roiImage, const std::vector<cv::Point>& contour,
    int start_x, int end_x, const cv::Scalar& color, GeoResult& result)
{
    // 1.1. 상단 윤곽선 추출 및 분리 (핵심 개선)
    // 파란색 윤곽선 전체에서 '위쪽' 라인만 분리하여 피팅에 사용합니다.
    // 이는 x_min에서 x_max까지 각 x 값에 대해 최소 y (가장 위쪽 점)를 찾는 방식입니다.
    std::map<int, int> topPointsMap; // x -> min_y

    for (const auto& p : contour) {
        if (topPointsMap.find(p.x) == topPointsMap.end() || p.y < topPointsMap[p.x]) {
            topPointsMap[p.x] = p.y;
        }
    }

    std::vector<cv::Point> topContour;
    for (const auto& pair : topPointsMap) {
        topContour.push_back(cv::Point(pair.first, pair.second));
    }

    // 추출된 상단 윤곽선 점들을 x 좌표 기준으로 정렬 (피팅을 위해 필요)
    std::sort(topContour.begin(), topContour.end(), [](const cv::Point& a, const cv::Point& b) {
        return a.x < b.x;
        });

    // ✨✨✨ 새로 추가된 부분 ✨✨✨
    // 추출된 topContour에서 Y 좌표 기준 이상치 제거
    // k_factor는 필요에 따라 조절 (예: 1.5, 2.0 등)
    std::vector<cv::Point> filteredTopContour = removeOutliersIQR(topContour, 1.5);

    // 이제부터 filteredTopContour를 사용합니다.
    // 충분한 점이 없으면 종료 (topContour 대신 filteredTopContour 사용)
    if (filteredTopContour.size() < 3) {
        result.avgCurvature = 0.0;
        result.maxCurvature = 0.0;
        result.isCurvedObjectFound = false;
        return;
    }

    // 2.3. cv::approxPolyDP를 이용한 윤곽선 단순화 적용 (필요시 epsilon 값 조정)
    // 상단 윤곽선에 대해서만 단순화 적용
    std::vector<cv::Point> processedContour = filteredTopContour;
    if (filteredTopContour.size() > 3) {
        // epsilon 값은 컨투어 길이에 비례하여 조정 (0.01은 예시, 테스트 필요)
        double epsilon = cv::arcLength(filteredTopContour, false) * 0.01;
        cv::approxPolyDP(filteredTopContour, processedContour, epsilon, false);
    }

    // 단순화 후에도 점이 부족하면 종료
    if (processedContour.size() < 3) {
        result.avgCurvature = 0.0;
        result.maxCurvature = 0.0;
        result.isCurvedObjectFound = false;
        return;
    }

    // 1.2. 데이터 정규화(Normalization) 적용
    // 이제 x를 독립 변수로 사용하므로 x의 min/max를 기준으로 정규화합니다.
    double min_x = processedContour[0].x, max_x = processedContour[0].x;
    double min_y = processedContour[0].y, max_y = processedContour[0].y; // y도 범위 확인용
    for (const auto& p : processedContour) {
        if (p.x < min_x) min_x = p.x;
        if (p.x > max_x) max_x = p.x;
        if (p.y < min_y) min_y = p.y; // Y 범위도 필요
        if (p.y > max_y) max_y = p.y;
    }

    // 정규화 스케일 계산 (분모가 0이 되는 경우 방지)
    double scale_x = (max_x - min_x > 0) ? (max_x - min_x) : 1.0;
    double scale_y = (max_y - min_y > 0) ? (max_y - min_y) : 1.0; // Y 스케일도 필요

    // 2.1. 독립 변수/종속 변수 변경 (y = Ax^3 + Bx^2 + Cx + D 형태로 변경) (핵심 개선)
    // 1.1. 다항식 차수 증가 검토 (3차 다항식으로 변경)
    // M * P = Y  ->  [x^3 x^2 x 1] * [A B C D]' = [y]
    int poly_order = 3; // 3차 다항식 (y = Ax^3 + Bx^2 + Cx + D)
    cv::Mat M(processedContour.size(), poly_order + 1, CV_64F); // [x^3, x^2, x, 1]
    cv::Mat Y_mat(processedContour.size(), 1, CV_64F); // [y]

    for (int i = 0; i < processedContour.size(); ++i) {
        // 정규화된 x 값 사용
        double x_norm = (static_cast<double>(processedContour[i].x) - min_x) / scale_x;

        M.at<double>(i, 0) = x_norm * x_norm * x_norm; // x^3
        M.at<double>(i, 1) = x_norm * x_norm;           // x^2
        M.at<double>(i, 2) = x_norm;                   // x
        M.at<double>(i, 3) = 1.0;                      // 1

        // 정규화된 y 값 사용
        Y_mat.at<double>(i, 0) = (static_cast<double>(processedContour[i].y) - min_y) / scale_y;
    }

    cv::Mat coefficients; // A, B, C, D
    cv::solve(M, Y_mat, coefficients, cv::DECOMP_SVD);

    double A = coefficients.at<double>(0, 0);
    double B = coefficients.at<double>(1, 0);
    double C = coefficients.at<double>(2, 0);
    double D = coefficients.at<double>(3, 0); // 3차 항의 상수

    // 1.3. 이상치 필터링 (간접 적용):
    // RANSAC과 같은 명시적인 이상치 필터링은 직접 구현이 복잡하므로,
    // 여기서는 cv::solve의 DECOMP_SVD를 통해 어느 정도 안정성을 확보하고,
    // 전처리(`approxPolyDP`와 모폴로지)로 이상치를 줄이는 데 집중합니다.

    // 피팅된 곡선 그리기
    std::vector<cv::Point> curve_points;
    // x 값 범위는 이미지의 ROI_X에서 ROI_X + ROI_W까지 또는 컨투어의 min_x, max_x를 기준으로 합니다.
    // 여기서는 컨투어의 x_min, x_max 범위를 사용합니다.
    for (int x_pixel = static_cast<int>(min_x); x_pixel <= static_cast<int>(max_x); ++x_pixel) {
        // 정규화된 x 값으로 변환
        double x_norm = (static_cast<double>(x_pixel) - min_x) / scale_x;

        // 정규화된 y 값 계산
        double y_norm_calculated = A * x_norm * x_norm * x_norm + B * x_norm * x_norm + C * x_norm + D;

        // 역정규화하여 실제 픽셀 y 값으로 변환
        double y_pixel_calculated = y_norm_calculated * scale_y + min_y;

        // 이미지 경계 내에서만 점을 추가
        if (x_pixel >= 0 && x_pixel < roiImage.cols && y_pixel_calculated >= 0 && y_pixel_calculated < roiImage.rows) {
            curve_points.push_back(cv::Point(x_pixel, static_cast<int>(y_pixel_calculated)));
        }
    }

    // 곡률 계산 (3차 다항식 y = Ax^3 + Bx^2 + Cx + D 에 대한 곡률 공식)
    // 1차 미분: y' = 3Ax^2 + 2Bx + C
    // 2차 미분: y'' = 6Ax + 2B
    // 곡률 K = |y''| / (1 + (y')^2)^(3/2)
    std::vector<double> curvatures;
    for (const auto& p : processedContour) { // 처리된 윤곽선 내의 각 점에서 곡률 계산
        double x_pixel = static_cast<double>(p.x);
        // 정규화된 x 값 사용
        double x_norm = (x_pixel - min_x) / scale_x;

        double first_derivative_norm = 3 * A * x_norm * x_norm + 2 * B * x_norm + C;
        double second_derivative_norm = 6 * A * x_norm + 2 * B;

        // 역정규화 스케일 반영
        // y' = (dy_norm/dx_norm) * (scale_y / scale_x)
        // y'' = (d2y_norm/dx_norm^2) * (scale_y / (scale_x * scale_x))

        double first_derivative = first_derivative_norm * (scale_y / scale_x);
        double second_derivative = second_derivative_norm * (scale_y / (scale_x * scale_x));

        double curvature = std::abs(second_derivative) / std::pow((1 + first_derivative * first_derivative), 1.5);

        // 무한대나 NaN 값 방지 및 유효한 곡률 값만 저장
        if (!std::isnan(curvature) && !std::isinf(curvature) && curvature < 1e5) { // 과도하게 큰 값 필터링
            curvatures.push_back(curvature);
        }
    }

    if (!curvatures.empty()) {
        double sum_curvature = 0;
        double max_curv = 0;
        for (double c : curvatures) {
            sum_curvature += c;
            if (c > max_curv) max_curv = c;
        }
        result.avgCurvature = sum_curvature / curvatures.size();
        result.maxCurvature = max_curv;

        // 곡률을 기반으로 곡선 객체 여부 판단 (임계값은 이미지 특성에 따라 조정 필요)
        // '직선이다'를 판단하기 위한 임계값은 0.001 (이전 제안)보다 약간 더 유연하게 조정될 수 있습니다.
        // 예를 들어, 더 작은 값으로 설정하거나, 평균 곡률도 함께 고려합니다.
        if (result.maxCurvature > 0.00005) { // 곡률이 매우 작으면 직선으로 간주, 특정 임계값 이상이면 곡선
            result.isCurvedObjectFound = true;
        }
        else {
            result.isCurvedObjectFound = false;
        }
    }
    else {
        result.avgCurvature = 0.0;
        result.maxCurvature = 0.0;
        result.isCurvedObjectFound = false;
    }

    // 곡선 점들을 연결하여 그리기
    // curve_points가 정렬되어 있지 않을 수 있으므로, 그리기 전에 정렬
    std::sort(curve_points.begin(), curve_points.end(), [](const cv::Point& a, const cv::Point& b) {
        return a.x < b.x;
        });

    /*for (size_t i = 0; i < curve_points.size() - 1; ++i) {
        cv::line(roiImage, curve_points[i], curve_points[i + 1], color, 2, cv::LINE_AA);
    }*/
}

void fitLineAndDraw(cv::Mat& roiImage, const std::vector<cv::Point>& contour,
    int start_x, int end_x, const cv::Scalar& color)
{
    cv::Vec4f line_params; // [vx, vy, x0, y0] - 방향 벡터 (vx, vy), 직선 위의 한 점 (x0, y0)
    cv::fitLine(contour, line_params, cv::DIST_L12, 0, 0.01, 0.01);

    cv::Point p1, p2;

    if (std::abs(line_params[0]) < 1e-6) { // 수직선에 가까운 경우
        p1.x = static_cast<int>(line_params[2]);
        p1.y = 0;
        p2.x = static_cast<int>(line_params[2]);
        p2.y = roiImage.rows - 1;
    }
    else {
        p1.x = start_x;
        p1.y = static_cast<int>(line_params[3] + (line_params[1] / line_params[0]) * (start_x - line_params[2]));
        p2.x = end_x;
        p2.y = static_cast<int>(line_params[3] + (line_params[1] / line_params[0]) * (end_x - line_params[2]));
    }

    cv::line(roiImage, p1, p2, color, 2, cv::LINE_AA);
}

// 전역 변수 정의 (값을 여기서 초기화)
cv::Mat g_srcImage;
cv::Mat g_dstImage;
std::string g_windowName;
int g_drmin = 0; // 초기 dr min 값
int g_drmax = 100; // 초기 dr max 값

// 공통 처리 함수 (중복 코드를 줄이기 위해)
void processAndDisplayImage() {
    if (g_drmin >= g_drmax) {
        return;
    }
    ApplyLinearDRClip(g_srcImage, g_dstImage, g_drmin, g_drmax, true);
    cv::imshow(g_windowName, g_dstImage);
}

// DR Min 트랙바 콜백 함수
void onTrackbarMin(int, void*) {
    if (g_drmin >= g_drmax) {
        g_drmax = g_drmin + 1; // DR Min이 DR Max보다 커지면, DR Max를 DR Min + 1로 설정
        if (g_drmax > 100) { // DR Max가 100을 넘지 않도록
            g_drmax = 100;
            g_drmin = g_drmax - 1; // DR Max가 100이 되면 DR Min은 99로 제한
            if (g_drmin < 0) g_drmin = 0; // 최소값 0 보장
            cv::setTrackbarPos("DR Min", g_windowName, g_drmin);
        }
        cv::setTrackbarPos("DR Max", g_windowName, g_drmax);
    }
    processAndDisplayImage();
}

// DR Max 트랙바 콜백 함수
void onTrackbarMax(int, void*) {
    if (g_drmax <= g_drmin) {
        g_drmin = g_drmax - 1; // DR Max가 DR Min보다 작아지면, DR Min을 DR Max - 1로 설정
        if (g_drmin < 0) { // DR Min이 0보다 작아지지 않도록
            g_drmin = 0;
            g_drmax = g_drmin + 1; // DR Min이 0이 되면 DR Max는 1로 제한
            if (g_drmax > 100) g_drmax = 100; // 최대값 100 보장
            cv::setTrackbarPos("DR Max", g_windowName, g_drmax);
        }
        cv::setTrackbarPos("DR Min", g_windowName, g_drmin);
    }
    processAndDisplayImage();
}

void showAndThreshold(const std::string& windowName, const cv::Mat& image) {

    if (!(MyOpenCVWrapper::ConfigManager::getInstance().getGeneralSettings().debugImg))
    {
        return;
    }

    if (image.empty()) {
        Logger::Error("Error: Image is empty!");
        return;
    }

    g_windowName = windowName;
    g_srcImage = image.clone();

    if (g_srcImage.channels() == 3) {
        cv::cvtColor(g_srcImage, g_srcImage, cv::COLOR_BGR2GRAY);
    }

    cv::namedWindow(g_windowName, cv::WINDOW_AUTOSIZE);

    cv::createTrackbar("DR Min", g_windowName, &g_drmin, 100, onTrackbarMin);
    cv::createTrackbar("DR Max", g_windowName, &g_drmax, 100, onTrackbarMax);

    processAndDisplayImage();

    cv::waitKey(0);

    cv::destroyWindow(windowName);
}

// 트랙바 콜백 함수 정의
void onTrackbar(int, void*) {
    // 임계값 조정: g_drmin이 g_drmax보다 커지지 않도록 합니다.
    if (g_drmin >= g_drmax) {
        //g_drmin = g_drmax - 1;
        //cv::setTrackbarPos("DR Min", g_windowName, g_drmin);
		return; // 트랙바 위치를 조정한 후 함수 종료
    }

    // 이진화(Thresholding) 수행
    //cv::threshold(g_srcImage, g_dstImage, g_drmin, g_drmax, cv::THRESH_BINARY);
    ApplyLinearDRClip(g_srcImage, g_dstImage, g_drmin, g_drmax, true);

    // 결과를 화면에 표시
    cv::imshow(g_windowName, g_dstImage);
}

// 이미지 표시 및 임계값 조절 함수 정의
//void showAndThreshold(const std::string& windowName, const cv::Mat& image) {
//    if (image.empty()) {
//        std::cerr << "Error: Image is empty!" << std::endl;
//        return;
//    }
//
//    g_windowName = windowName;
//    g_srcImage = image.clone(); // 원본 이미지를 복사하여 전역 변수에 저장
//
//    // 임계값 처리를 위해 이미지를 그레이스케일로 변환 (필요한 경우)
//    if (g_srcImage.channels() == 3) {
//        cv::cvtColor(g_srcImage, g_srcImage, cv::COLOR_BGR2GRAY);
//    }
//
//    cv::namedWindow(g_windowName, cv::WINDOW_AUTOSIZE); // 창 생성
//
//    // 트랙바 생성 (이름 변경: "DR Min", "DR Max")
//    cv::createTrackbar("DR Min", g_windowName, &g_drmin, 100, onTrackbar);
//    cv::createTrackbar("DR Max", g_windowName, &g_drmax, 100, onTrackbar);
//
//    // 초기 트랙바 위치에 따라 한 번 콜백 함수 호출
//    onTrackbar(0, 0);
//
//    // 사용자가 키를 누를 때까지 대기
//    cv::waitKey(0);
//
//    // 창 닫기 (선택 사항)
//    //cv::destroyWindow(g_windowName);
//}

// ORB 기반 회전 추정 함수
float estimateRotationByORB(const cv::Mat& reference, const cv::Mat& rotated) {
    cv::Ptr<cv::ORB> orb = cv::ORB::create(500);
    std::vector<cv::KeyPoint> kp1, kp2;
    cv::Mat des1, des2;
    orb->detectAndCompute(reference, cv::noArray(), kp1, des1);
    orb->detectAndCompute(rotated, cv::noArray(), kp2, des2);

    if (des1.empty() || des2.empty()) return 0.0f;

    std::vector<cv::DMatch> matches;
    cv::BFMatcher matcher(cv::NORM_HAMMING);
    matcher.match(des1, des2, matches);

    std::sort(matches.begin(), matches.end(), [](const auto& a, const auto& b) {
        return a.distance < b.distance;
        });

    if (matches.size() < 10) return 0.0f;

    std::vector<cv::Point2f> pts1, pts2;
    for (int i = 0; i < std::min(50, (int)matches.size()); ++i) {
        pts1.push_back(kp1[matches[i].queryIdx].pt);
        pts2.push_back(kp2[matches[i].trainIdx].pt);
    }

    cv::Mat affine = cv::estimateAffinePartial2D(pts2, pts1);
    if (affine.empty()) return 0.0f;

    float angleRad = std::atan2(affine.at<double>(0, 1), affine.at<double>(0, 0));
    return angleRad * 180.0f / CV_PI;
}

float estimateRotationByPhaseCorrelation(const cv::Mat& reference, const cv::Mat& rotated) {
    int radius = std::min(reference.cols, reference.rows) / 2;
    cv::Point2f center(reference.cols / 2.0f, reference.rows / 2.0f);

    // 해상도 설정 (각도 방향이 세로, 고정)
    int angleResolution = 1024;

    // Polar 변환
    cv::Mat refPolar, rotPolar;
    cv::warpPolar(reference, refPolar, cv::Size(radius, angleResolution), center, radius, cv::WARP_POLAR_LINEAR);
    cv::warpPolar(rotated, rotPolar, cv::Size(radius, angleResolution), center, radius, cv::WARP_POLAR_LINEAR);

    // 회전 정합: phase correlation (Y 방향 이동량 측정)
    cv::Point2d shift = cv::phaseCorrelate(refPolar, rotPolar);
    double yShift = shift.y;

    // 회전 각도 계산
    float angle = -360.0f * static_cast<float>(yShift) / angleResolution;
    if (angle < 0) angle += 360.0f; // 0~360으로 보정

    return -angle; // 외부 사용 시 CCW 기준 음수 반환
}

float estimateVerticalShiftByFFT(const cv::Mat& ref, const cv::Mat& target, int angleResolution)
{
    // 1. float 변환 + 평균 제거
    cv::Mat fRef, fTarget;
    ref.convertTo(fRef, CV_32F);
    target.convertTo(fTarget, CV_32F);
    fRef -= cv::mean(fRef);
    fTarget -= cv::mean(fTarget);

    // 2. 행 방향(=Y축) FFT
    cv::dft(fRef, fRef, cv::DFT_ROWS | cv::DFT_COMPLEX_OUTPUT);
    cv::dft(fTarget, fTarget, cv::DFT_ROWS | cv::DFT_COMPLEX_OUTPUT);

    // 3. Cross Power Spectrum 계산
    cv::Mat crossPower;
    cv::mulSpectrums(fTarget, fRef, crossPower, 0, true);
    cv::normalize(crossPower, crossPower);

    // 4. 역 DFT → correlation peak 추정
    cv::Mat corr;
    cv::dft(crossPower, corr, cv::DFT_INVERSE | cv::DFT_ROWS | cv::DFT_REAL_OUTPUT | cv::DFT_SCALE);

    // 5. 최대 상관 위치 → shift
    cv::Point maxLoc;
    cv::minMaxLoc(corr, nullptr, nullptr, nullptr, &maxLoc);
    int shift = maxLoc.y;

    // 6. shift → angle 변환 (Polar 이미지의 세로축 = 각도)
    float angle = -360.0f * shift / angleResolution;
    if (angle < 0) angle += 360.0f;
    return -angle; // 다른 알고리즘과 부호 맞추기
}



cv::Mat circShiftX(const cv::Mat& src, int shift) {
    int w = src.cols;
    shift = ((shift % w) + w) % w; // 음수 대응 안전 모듈러

    // 예외 처리
    if (w == 0 || shift < 0 || shift >= w) {
        std::cerr << "[CircularShift] Invalid shift value: " << shift << ", src.cols: " << w << std::endl;
        return src.clone();
    }

    if (shift == 0)
        return src.clone();  // 0이면 그대로 반환

    // 안전하게 split 후 concat
    cv::Mat part1 = src.colRange(shift, w).clone(); // clone으로 안전 확보
    cv::Mat part2 = src.colRange(0, shift).clone();

    cv::Mat result;
    cv::hconcat(part1, part2, result);
    return result;
}

cv::Mat circShiftY(const cv::Mat& src, int shift) {
    int h = src.rows;
    shift = ((shift % h) + h) % h; // 음수 대응 안전 모듈러

    if (h == 0 || shift < 0 || shift >= h) {
        std::cerr << "[CircularShiftY] Invalid shift value: " << shift << ", src.rows: " << h << std::endl;
        return src.clone();
    }

    if (shift == 0)
        return src.clone();

    // 위쪽과 아래쪽을 분리해서 붙이기
    cv::Mat part1 = src.rowRange(shift, h).clone();
    cv::Mat part2 = src.rowRange(0, shift).clone();

    cv::Mat result;
    cv::vconcat(part1, part2, result); // ← 가로 말고 세로 연결
    return result;
}
 
// 정규화 상관계수 계산
float computeNormalizedCorrelation(const cv::Mat& a, const cv::Mat& b) {
    cv::Mat a32f, b32f;
    a.convertTo(a32f, CV_32F);
    b.convertTo(b32f, CV_32F);

    cv::Scalar meanA = cv::mean(a32f);
    cv::Scalar meanB = cv::mean(b32f);
    a32f -= meanA;
    b32f -= meanB;

    double num = cv::sum(a32f.mul(b32f))[0];
    double denom = std::sqrt(cv::sum(a32f.mul(a32f))[0] * cv::sum(b32f.mul(b32f))[0]);
    if (denom == 0) return 0;
    return static_cast<float>(num / denom);
}

// 회전 추정 함수 (Polar 변환 + Y 방향 순환 시프트)
float estimateRotationByCircularShift(const cv::Mat& reference, const cv::Mat& rotated) {
    int radius = std::min(reference.cols, reference.rows) / 2;
    cv::Point2f center(reference.cols / 2.0f, reference.rows / 2.0f);

    int angleResolution = 1024;
    cv::Mat refPolar, rotPolar;
    cv::warpPolar(reference, refPolar, cv::Size(radius, angleResolution), center, radius, cv::WARP_POLAR_LINEAR);
    cv::warpPolar(rotated, rotPolar, cv::Size(radius, angleResolution), center, radius, cv::WARP_POLAR_LINEAR);

    // 상관계수 리스트 저장
    std::vector<float> corrList(angleResolution);
    float bestCorr = -1.0f;
    int bestShift = 0;

    for (int shift = 0; shift < angleResolution; ++shift) {
        cv::Mat shifted = circShiftY(rotPolar, shift);
        float corr = computeNormalizedCorrelation(refPolar, shifted);
        corrList[shift] = corr;
        if (corr > bestCorr) {
            bestCorr = corr;
            bestShift = shift;
        }
    }

    // 서브픽셀 보정 (parabolic interpolation)
    float subpixelShift = static_cast<float>(bestShift);
    if (bestShift > 0 && bestShift < angleResolution - 1) {
        float y1 = corrList[bestShift - 1];
        float y2 = corrList[bestShift];
        float y3 = corrList[bestShift + 1];
        float denom = 2 * (2 * y2 - y1 - y3);
        if (denom != 0.0f) {
            float delta = (y1 - y3) / denom;
            subpixelShift += delta;
        }
    }

    // 시각화 (원하는 경우)
    cv::imshow("refPolar", refPolar);
    cv::imshow("rotPolar", rotPolar);
    cv::imshow("shifted best", circShiftY(rotPolar, static_cast<int>(subpixelShift + 0.5f)));
    cv::waitKey();

    // 각도 계산 (음수: 반시계)
    float angle = -360.0f * subpixelShift / angleResolution;
    if (angle < 0) angle += 360.0f;  // 항상 0~360 범위로 보정
    return -angle;
}

void ApplyLinearDRClip(const cv::Mat& inputGray, cv::Mat& outputUint8, double dr_min_percent, double dr_max_percent, bool normalize)
{
    CV_Assert(inputGray.type() == CV_8UC1);
    CV_Assert(0 <= dr_min_percent && dr_min_percent < dr_max_percent && dr_max_percent <= 100);

    // 퍼센트 → 값 변환
    int min_val = static_cast<int>(dr_min_percent * 255.0 / 100.0);
    int max_val = static_cast<int>(dr_max_percent * 255.0 / 100.0);
    if (max_val <= min_val)
        max_val = min_val + 1;

    // 클리핑
    cv::Mat clipped;
    cv::threshold(inputGray, clipped, max_val, max_val, cv::THRESH_TRUNC);
    cv::threshold(clipped, clipped, min_val, min_val, cv::THRESH_TOZERO);

    if (normalize)
    {
        // 정규화: (x - min_val) / (max_val - min_val) * 255
        cv::Mat float_img;
        clipped.convertTo(float_img, CV_32F);
        float_img = (float_img - min_val) / (max_val - min_val) * 255.0f;
        float_img.convertTo(outputUint8, CV_8U);
    }
    else
    {
        // 정규화 없이 그대로 출력
        outputUint8 = clipped.clone();
    }
}

void ApplyLogNormalization(const cv::Mat& inputGray, cv::Mat& outputUint8, double dr_min_percent, double dr_max_percent)
{
    CV_Assert(inputGray.type() == CV_8UC1 || inputGray.type() == CV_16UC1);
    CV_Assert(0 <= dr_min_percent && dr_min_percent < dr_max_percent && dr_max_percent <= 100);

    // 1. 입력을 float32로 변환
    cv::Mat floatInput;
    inputGray.convertTo(floatInput, CV_32F);

    // 2. log 변환 전에 0 방지
    cv::Mat logInput = cv::max(floatInput, 1.0f);

    // 3. log 변환 (자연로그 사용)
    cv::Mat logResult;
    cv::log(logInput, logResult);

    // 4. log -> dB 변환
    const float ln10_inv_mul20 = 8.68589f;
    logResult *= ln10_inv_mul20;

    // 5. DR 범위 설정
    const float absolute_max_db = std::log10(std::sqrt(2.0f) * 32768.0f) * 20.0f;
    float dr_min_db = absolute_max_db * dr_min_percent / 100.0f;
    float dr_max_db = absolute_max_db * dr_max_percent / 100.0f;

    // 6. DR min 조정
    logResult -= dr_min_db;
    cv::threshold(logResult, logResult, 0, 0, cv::THRESH_TOZERO); // max(x, 0)

    // 7. 정규화 to 0~255
    logResult = logResult / (dr_max_db - dr_min_db) * 255.0f;
    cv::threshold(logResult, logResult, 255.0, 255.0, cv::THRESH_TRUNC); // min(x, 255)

    // 8. 반올림 후 uint8로 변환
    cv::Mat rounded;
    cv::add(logResult, 0.5, logResult);            // +0.5
    logResult.convertTo(outputUint8, CV_8U);       // 정수형으로 내림 (truncation → 반올림 효과)
}

// 원 내부의 모든 픽셀값 평균을 계산하는 함수
double calculateCircleMean(const Mat& grayImage, Point center, int radius) {
    if (grayImage.empty()) {
        cerr << "Error: Empty image." << endl;
        return -1;
    }
    if (grayImage.channels() != 1) {
        cerr << "Error: Image must be grayscale." << endl;
        return -1;
    }

    double sum = 0;
    int count = 0;

    for (int y = max(center.y - radius, 0); y <= min(center.y + radius, grayImage.rows - 1); y++) {
        for (int x = max(center.x - radius, 0); x <= min(center.x + radius, grayImage.cols - 1); x++) {
            // 원의 방정식 (x - center.x)^2 + (y - center.y)^2 <= radius^2 체크
            if ((x - center.x) * (x - center.x) + (y - center.y) * (y - center.y) <= radius * radius) {
                sum += grayImage.at<uchar>(y, x);
                count++;
            }
        }
    }

    return (count > 0) ? sum / count : 0;
}

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

void showAndSaveImage(const std::string& windowName, const cv::Mat& image) {
    if (!(MyOpenCVWrapper::ConfigManager::getInstance().getGeneralSettings().debugImg))
    {
        return;
    }

    if (image.empty()) {
        std::cerr << "Error: Image is empty!" << std::endl;
        return;
    }

    cv::imshow(windowName, image);
    cv::waitKey(0);

    cv::destroyWindow(windowName);

    std::string filename = windowName + ".bmp";
    cv::imwrite(filename, image);
}

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

// 특정 각도에서의 새로운 점 계산 함수 (currentAngle을 인자로 추가하여 재사용)
Point calculateNewPoint(const Point& center, const Point& point, double currentAngle, double angleOffset) {
    double newAngle = currentAngle + angleOffset;
    double radian = newAngle * CV_PI / 180.0;

    double radius = std::sqrt((point.x - center.x) * (point.x - center.x) + (point.y - center.y) * (point.y - center.y));
    int newX = center.x + static_cast<int>(radius * std::cos(radian));
    int newY = center.y - static_cast<int>(radius * std::sin(radian));

    return Point(newX, newY);
}

// 특정 각도에서의 새로운 점 계산 함수 (currentAngle과 radiusOffset 추가하여 재사용 및 확장)
Point calculateNewPoint(const Point& center, const Point& point, double currentAngle, double angleOffset, double radiusOffset) {
    double newAngle = currentAngle + angleOffset;
    double radian = newAngle * CV_PI / 180.0;

    double radius = std::sqrt((point.x - center.x) * (point.x - center.x) + (point.y - center.y) * (point.y - center.y)) + radiusOffset;
    int newX = center.x + static_cast<int>(radius * std::cos(radian));
    int newY = center.y - static_cast<int>(radius * std::sin(radian));

    return Point(newX, newY);
}

// 특정 각도에서의 새로운 점 계산 함수
Point calculateNewPoint(const Point& center, const Point& point, double angleOffset) {
    double currentAngle = calculateAngle(center, point);
    double newAngle = currentAngle + angleOffset;
    double radian = newAngle * CV_PI / 180.0;

    double radius = std::sqrt((point.x - center.x) * (point.x - center.x) + (point.y - center.y) * (point.y - center.y));
    int newX = center.x + static_cast<int>(radius * std::cos(radian));
    int newY = center.y - static_cast<int>(radius * std::sin(radian));

    return Point(newX, newY);
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

cv::Mat rotateImage(const cv::Mat& image, float angle)
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