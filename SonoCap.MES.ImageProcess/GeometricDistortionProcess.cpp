#include "GeometricDistortionProcess.h"
#include "Util.h"

void MyOpenCVWrapper::GeometricDistortionProcess(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr outputBuffer, System::IntPtr textBuffer)
{
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

    showAndSaveImage(".\\GeometricDistortion\\inputImage", inputImage);

    drawImage = inputImage.clone();
    drawImage2 = inputImage.clone();
	resultImage = inputImage.clone();

    // 그레이스케일 변환
    cv::Mat gray;
    cv::cvtColor(inputImage, gray, cv::COLOR_BGRA2GRAY);

    //cv::GaussianBlur(gray, gray, cv::Size(3, 3), 1.5);
    cv::GaussianBlur(gray, gray, cv::Size(5, 5), 1.5);

    // Canny 엣지 검출
    cv::Mat edges;
    //cv::Canny(gray, edges, 50, 150, 3);
    cv::Canny(gray, edges, 80, 200, 3);
    showAndSaveImage(".\\GeometricDistortion\\edges", edges);

    //cv::morphologyEx(edges, finalEdges, cv::MORPH_CLOSE, cv::Mat::ones(3, 3, CV_8U));
    cv::morphologyEx(edges, finalEdges, cv::MORPH_CLOSE, cv::Mat::ones(5, 5, CV_8U));

    showAndSaveImage(".\\GeometricDistortion\\finalEdges", finalEdges);
    // Hough 변환을 이용한 선 검출
    std::vector<cv::Vec4i> lines;
    //cv::HoughLinesP(edges, lines, 1, CV_PI / 180, 120, 100, 30);
    cv::HoughLinesP(finalEdges, lines, 1, 2 * CV_PI / 180, 120, 100, 50);
    
    std::cout << "Line Cnt: " << lines.size() << std::endl;
    for (size_t i = 0; i < lines.size(); i++) {
        cv::Vec4i l = lines[i];
        std::cout << "Line: (" << l[0] << ", " << l[1] << ") -> (" << l[2] << ", " << l[3] << ")" << std::endl;
    }
    // 검출된 선을 원본 이미지에 그리기
    for (const auto& line : lines) {
        cv::line(drawImage, cv::Point(line[0], line[1]), cv::Point(line[2], line[3]), getRandomColor(), 1);
    }
    showAndSaveImage(".\\GeometricDistortion\\drawImage", drawImage);

    for (size_t i = 0; i < lines.size(); i++) {
        // 선마다 고유한 색을 생성
        cv::Scalar color = getRandomColor();

        // 두 점을 같은 색으로 그림 (점 크기 = 5, 채우기 = -1)
        cv::circle(drawImage2, cv::Point(lines[i][0], lines[i][1]), 5, color, 1);
        cv::circle(drawImage2, cv::Point(lines[i][2], lines[i][3]), 5, color, 1);
    }

    showAndSaveImage(".\\GeometricDistortion\\drawImage2", drawImage2);

}