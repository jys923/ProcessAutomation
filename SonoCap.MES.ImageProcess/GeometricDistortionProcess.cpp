#include "GeometricDistortionProcess.h"
#include "Util.h"

#define MASK_MIN 70
#define MASK_MAX 95
#define THRESHOLD 150
#define GAMMA 2.0 //(1.2~2.0 recommended)
#define ROI_HEIGHT 260
#define ROI_WIDTH 40
#define ROI_OFFSET_X 140 // Distance from center in X-axis
#define ROI_OFFSET_Y 0   // Distance from center in Y-axis

void MyOpenCVWrapper::GeometricDistortionProcess(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr outputBuffer, System::IntPtr textBuffer) {
    std::string resultText = "FAIL";
    memcpy(textBuffer.ToPointer(), resultText.c_str(), resultText.size() + 1); // Include null terminator
    memset(outputBuffer.ToPointer(), 0, imageWidth * imageHeight * 4);

    // Basic parameters
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) {
        std::cerr << "Error: Image not found!" << std::endl;
        return;
    }
    showAndSaveImage(".\\GeometricDistortion\\inputImage", inputImage);

    cv::Point imageCenter(inputImage.cols / 2, inputImage.rows / 2);
    std::vector<ArcData> detectedArcs;
    std::vector<StraightnessData> straightnessResults;

    // Image processing variables
    cv::Mat maskGrayImage, gammaCorrectedImage, binaryImage, finalEdgeImage, rotatedImage, roiExtractedImage;

    // === 2. Create and Apply Mask ===
    maskGrayImage = inputImage.clone();
    cv::cvtColor(maskGrayImage, maskGrayImage, cv::COLOR_BGRA2GRAY);

    cv::Mat mask = createCircularMask(inputImage.size(), MASK_MIN, MASK_MAX);
    showAndSaveImage(".\\GeometricDistortion\\mask", mask);
    maskGrayImage.setTo(cv::Scalar(0, 0, 0), mask);
    showAndSaveImage(".\\GeometricDistortion\\gray_mask", maskGrayImage);

    // === 3. Gamma Correction ===
    cv::Mat lookupTable(1, 256, CV_8U);
    for (int i = 0; i < 256; i++) {
        lookupTable.at<uchar>(i) = cv::saturate_cast<uchar>(pow(i / 255.0, GAMMA) * 255.0);
    }
    cv::LUT(maskGrayImage, lookupTable, gammaCorrectedImage);
    showAndSaveImage(".\\GeometricDistortion\\gammaCorrected", gammaCorrectedImage);

    // === 4. Binarization ===
    cv::threshold(gammaCorrectedImage, binaryImage, THRESHOLD, 255, cv::THRESH_BINARY);
    showAndSaveImage(".\\GeometricDistortion\\binaryImage", binaryImage);

    // === 5. Contour Detection ===
    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(binaryImage, contours, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);
    if (contours.empty()) return;

    for (const auto& contour : contours) {
        cv::Rect boundingBox = cv::boundingRect(contour);
        if (boundingBox.width <= inputImage.cols - 10 && boundingBox.height <= inputImage.rows - 10) {
            cv::Point contourCenter(boundingBox.x + boundingBox.width / 2, boundingBox.y + boundingBox.height / 2);
            double angle = calculateAngle(imageCenter, contourCenter);
            double area = cv::contourArea(contour);
            detectedArcs.push_back({ contourCenter, angle, area });
        }
    }
    std::sort(detectedArcs.begin(), detectedArcs.end(),
        [](const ArcData& a, const ArcData& b) { return a.area > b.area; });

    rotatedImage = rotateImage(inputImage, 180 - detectedArcs.front().angle);
    cv::Rect roi(cv::Point(imageCenter.x + ROI_OFFSET_X, imageCenter.y - ROI_HEIGHT / 2),
        cv::Point(imageCenter.x + ROI_OFFSET_X + ROI_WIDTH, imageCenter.y + ROI_HEIGHT / 2));
    showAndSaveImage(".\\GeometricDistortion\\rotatedImage", rotatedImage);

    // === 6. ROI Extraction ===
    roiExtractedImage = rotatedImage(roi);
    showAndSaveImage(".\\GeometricDistortion\\roiExtractedImage", roiExtractedImage);

    // === 7. Final Edge Detection ===
    cv::Mat detectedEdges;
    cv::cvtColor(roiExtractedImage, gammaCorrectedImage, cv::COLOR_BGRA2GRAY);
    cv::LUT(gammaCorrectedImage, lookupTable, gammaCorrectedImage);
    cv::threshold(gammaCorrectedImage, binaryImage, 135, 255, cv::THRESH_BINARY);
    cv::morphologyEx(binaryImage, finalEdgeImage, cv::MORPH_CLOSE, cv::Mat::ones(5, 5, CV_8U));
    showAndSaveImage(".\\GeometricDistortion\\finalEdges", finalEdgeImage);

    cv::Canny(finalEdgeImage, detectedEdges, 50, 200, 3);
    showAndSaveImage(".\\GeometricDistortion\\edges", detectedEdges);

    cv::findContours(detectedEdges, contours, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);
    if (contours.empty()) return;

    std::vector<cv::Point> longestContour;
    double maxContourLength = 0;
    for (const auto& contour : contours) {
        double length = cv::arcLength(contour, false);
        if (length > maxContourLength) {
            maxContourLength = length;
            longestContour = contour;
        }
    }
    if (longestContour.empty()) return;

    double straightnessScore = -1;
    if (!longestContour.empty()) {
        double length = cv::arcLength(longestContour, false);
        if (length < ROI_HEIGHT - 20) {
            return;
        }
        straightnessScore = evaluateContourStraightness(longestContour, roiExtractedImage);
        drawContours(roiExtractedImage, std::vector<std::vector<cv::Point>>{longestContour}, -1, green, 1);
    }
    std::cout << "straightnessScore: " << straightnessScore << std::endl;

    straightnessResults.push_back({ straightnessScore });
    showAndSaveImage(".\\GeometricDistortion\\roiExtractedImage2", roiExtractedImage);
    showAndSaveImage(".\\GeometricDistortion\\rotatedImage2", rotatedImage);

    memcpy(outputBuffer.ToPointer(), rotatedImage.data, rotatedImage.total() * rotatedImage.elemSize());
    resultText = objectToJsonString(straightnessResults[0]);
    memcpy(textBuffer.ToPointer(), resultText.c_str(), resultText.size() + 1);

    return;
}

#if false
    // Hough 변환을 이용한 선 검출
    std::vector<cv::Vec4i> lines;
    //cv::HoughLinesP(edges, lines, 1, CV_PI / 180, 120, 100, 30);
    //cv::HoughLinesP(edges, lines, 1, 2 * CV_PI / 180, 120, 250, 50);
    //cv::HoughLinesP(edges, lines, 1, 2 * CV_PI / 180, 80, 150, 50);
    cv::HoughLinesP(edges, lines, 1, CV_PI / 180, 50, 150, 50);
    
    std::cout << "Line Cnt: " << lines.size() << std::endl;
    for (size_t i = 0; i < lines.size(); i++) {
        cv::Vec4i l = lines[i];
        std::cout << "Line: (" << l[0] << ", " << l[1] << ") -> (" << l[2] << ", " << l[3] << ")" << std::endl;
    }
    // 검출된 선을 원본 이미지에 그리기
    int cnt = 0;
    for (const auto& line : lines) {
		cv::Mat roiImage3 = roiImage2.clone();
        cv::line(roiImage3, cv::Point(line[0], line[1]), cv::Point(line[2], line[3]), red, 1);
        showAndSaveImage(".\\GeometricDistortion\\"+ std::to_string(cnt), roiImage3);
        cnt++;
    }

    for (size_t i = 0; i < lines.size(); i++) {
        // 선마다 고유한 색을 생성
        cv::Scalar color = getRandomColor();

        // 두 점을 같은 색으로 그림 (점 크기 = 5, 채우기 = -1)
        cv::circle(roiImage, cv::Point(lines[i][0], lines[i][1]), 5, color, 1);
        cv::circle(roiImage, cv::Point(lines[i][2], lines[i][3]), 5, color, 1);
    }

    showAndSaveImage(".\\GeometricDistortion\\drawImage2", roiImage);

    return;
}
#endif // false
