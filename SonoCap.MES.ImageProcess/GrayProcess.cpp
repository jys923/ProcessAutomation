#include "GrayProcess.h"
#include "Util.h"

#define MASK_MIN 70
#define MASK_MAX 95
#define THRESHOLD 150
#define GAMMA 2.0 //(1.2~2.0 recommended)
#define ROI_HEIGHT 260
#define ROI_WIDTH 40
#define ROI_OFFSET_X 140 // Distance from center in X-axis
#define ROI_OFFSET_Y 0   // Distance from center in Y-axis

void MyOpenCVWrapper::GrayProcess(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr resultBuffer, System::IntPtr textBuffer)
{
    std::string resultText = "FAIL";
    memcpy(textBuffer.ToPointer(), resultText.c_str(), resultText.size() + 1); // Include null terminator
    memset(resultBuffer.ToPointer(), 0, imageWidth * imageHeight * 4);
    // Basic parameters
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) {
        std::cerr << "Error: Image not found!" << std::endl;
        return;
    }

    memset(resultBuffer.ToPointer(), 0, imageWidth * imageHeight * 4);
    showAndSaveImage(".\\Gray\\inputImage", inputImage);

    cv::Point imageCenter(inputImage.cols / 2, inputImage.rows / 2);
    std::vector<ArcData> detectedArcs;
	std::vector<GrayData> grayResults;

    // Image processing variables
    cv::Mat grayImage, maskGrayImage, gammaCorrectedImage, binaryImage, finalEdgeImage, resultImage;

    // === 2. Create and Apply Mask ===
    maskGrayImage = inputImage.clone();
    cv::cvtColor(maskGrayImage, maskGrayImage, cv::COLOR_BGRA2GRAY);

    cv::Mat mask = createCircularMask(inputImage.size(), MASK_MIN, MASK_MAX);
    showAndSaveImage(".\\Gray\\mask", mask);
    maskGrayImage.setTo(cv::Scalar(0, 0, 0), mask);
    showAndSaveImage(".\\Gray\\gray_mask", maskGrayImage);

    // === 3. Gamma Correction ===
    cv::Mat lookupTable(1, 256, CV_8U);
    for (int i = 0; i < 256; i++) {
        lookupTable.at<uchar>(i) = cv::saturate_cast<uchar>(pow(i / 255.0, GAMMA) * 255.0);
    }
    cv::LUT(maskGrayImage, lookupTable, gammaCorrectedImage);
    showAndSaveImage(".\\Gray\\gammaCorrected", gammaCorrectedImage);

    // === 4. Binarization ===
    cv::threshold(gammaCorrectedImage, binaryImage, THRESHOLD, 255, cv::THRESH_BINARY);
    showAndSaveImage(".\\Gray\\binaryImage", binaryImage);

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

    resultImage = inputImage.clone();
    cv::cvtColor(resultImage, grayImage, cv::COLOR_BGRA2GRAY);

	double currentAngle = detectedArcs.front().angle;
    cv::Point add40(calculateNewPoint(imageCenter, detectedArcs.front().center, currentAngle, 40, 10 ));
    cv::Point add80(calculateNewPoint(imageCenter, detectedArcs.front().center, currentAngle, 80, 20));
    cv::Point add120(calculateNewPoint(imageCenter, detectedArcs.front().center, currentAngle, 120, 30));

	cv::circle(resultImage, add40, 20 , red, 2);
	double pixel40 = calculateCircleMean(grayImage, add40, 20);
	grayResults.push_back({ add40, pixel40, 20 });
	
    cv::circle(resultImage, add80, 20 , green, 2);
    double pixel80 = calculateCircleMean(grayImage, add80, 25);
    grayResults.push_back({ add80, pixel80, 25 });
	
    cv::circle(resultImage, add120, 20, blue, 2);
    double pixel120 = calculateCircleMean(grayImage, add120, 30);
    grayResults.push_back({ add120, pixel120, 30 });

	showAndSaveImage(".\\Gray\\resultImage", resultImage);
	
    resultText = vectorToJsonString(grayResults);
	memcpy(textBuffer.ToPointer(), resultText.c_str(), resultText.size() + 1);
	memcpy(resultBuffer.ToPointer(), resultImage.data, resultImage.total() * resultImage.elemSize());
	return;

}
