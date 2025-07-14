#include "GrayInspection.h"

// 디파인된 ROI 위치와 크기
#define ROI_WIDTH  40
#define ROI_HEIGHT 40

#define ROI1_X  100
#define ROI1_Y  80

#define ROI2_X  160
#define ROI2_Y  120

#define ROI3_X  200
#define ROI3_Y  160

//void MyOpenCVWrapper::GrayInspection(const cv::Mat& input, cv::Mat& output, std::string& resultText)
//{
//    output.setTo(cv::Scalar(0, 255, 0));
//    resultText = "Res OK";
//}

void MyOpenCVWrapper::GrayInspection(cv::Mat& roiImage, std::string& resultText)
{
    if (roiImage.empty() || roiImage.channels() != 4) {
        resultText = R"({"error": "invalid input"})";
        return;
    }

    cv::Mat grayImage;
    cv::cvtColor(roiImage, grayImage, cv::COLOR_BGRA2GRAY);

    std::vector<cv::Point> roiPoints = {
        {ROI1_X, ROI1_Y},
        {ROI2_X, ROI2_Y},
        {ROI3_X, ROI3_Y}
    };

    std::vector<cv::Scalar> colors = { RedA, GreenA, BlueA };
    std::ostringstream oss;
    oss << "[";

    for (int i = 0; i < roiPoints.size(); ++i) {
        cv::Rect roi(roiPoints[i].x, roiPoints[i].y, ROI_WIDTH, ROI_HEIGHT);
        if (roi.x >= 0 && roi.y >= 0 &&
            roi.x + roi.width <= grayImage.cols &&
            roi.y + roi.height <= grayImage.rows) {

            double meanVal = cv::mean(grayImage(roi))[0];
            cv::rectangle(roiImage, roi, colors[i], 2);  // 직접 그리기

            oss << R"({"x":)" << roiPoints[i].x
                << R"(,"y":)" << roiPoints[i].y
                << R"(,"mean":)" << meanVal
                << R"(})";
        }
        else {
            oss << R"({"x":)" << roiPoints[i].x
                << R"(,"y":)" << roiPoints[i].y
                << R"(,"mean": -1})";
        }

        if (i < roiPoints.size() - 1) oss << ",";
    }

    oss << "]";
    resultText = oss.str();
}

void MyOpenCVWrapper::GrayInspection2(cv::Mat& roiImage, GrayResult& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) return;

    cv::Mat grayImage;
    cv::cvtColor(roiImage, grayImage, cv::COLOR_BGRA2GRAY);

    std::vector<cv::Point> roiPoints = {
        {ROI1_X, ROI1_Y},
        {ROI2_X, ROI2_Y},
        {ROI3_X, ROI3_Y}
    };

    std::vector<cv::Scalar> colors = { RedA, GreenA, BlueA };

    std::vector<double*> targets = { &result.mean1, &result.mean2, &result.mean3 };

    for (int i = 0; i < roiPoints.size(); ++i) {
        cv::Rect roi(roiPoints[i].x, roiPoints[i].y, ROI_WIDTH, ROI_HEIGHT);
        if (roi.x >= 0 && roi.y >= 0 &&
            roi.x + roi.width <= grayImage.cols &&
            roi.y + roi.height <= grayImage.rows) {

            *targets[i] = cv::mean(grayImage(roi))[0];
            cv::rectangle(roiImage, roi, colors[i], 2);
        }
    }
}

#define ROI_WIDTH     14
#define ROI_HEIGHT    14

#define RADIUS        75           // 적절한 반지름
#define ANGLE_CENTER  (CV_PI / 8)   // 22.5도
#define ANGLE_OFFSET  (CV_PI / 12)  // ±15도

void MyOpenCVWrapper::GrayInspection(cv::Mat& roiImage, GrayResult& result)
{
    if (roiImage.empty() || roiImage.channels() != 4) return;

    cv::Mat grayImage;
    cv::cvtColor(roiImage, grayImage, cv::COLOR_BGRA2GRAY);

    double centerX = 0;
    double centerY = grayImage.rows;

    std::vector<double> angles = {
        ANGLE_CENTER - ANGLE_OFFSET,  // 7.5도
        //ANGLE_CENTER,                 // 22.5도
        ANGLE_CENTER + ANGLE_OFFSET   // 37.5도
    };

    std::vector<cv::Scalar> colors = { RedA, GreenA, BlueA };
    std::vector<double*> targets = { &result.mean1, &result.mean2, &result.mean3 };

    for (int i = 0; i < angles.size(); ++i)
    {
        double angle = angles[i];


        // 중심점에서 라디안 각도로 좌표 계산
        double cx = centerX + RADIUS * cos(angle);
        double cy = centerY - RADIUS * sin(angle);  // 이미지 Y축은 아래로 향하므로 -

        int x = static_cast<int>(cx - ROI_WIDTH / 2);
        int y = static_cast<int>(cy - ROI_HEIGHT / 2);

        cv::Rect roi(x, y, ROI_WIDTH, ROI_HEIGHT);
        if (roi.x >= 0 && roi.y >= 0 &&
            roi.x + roi.width <= grayImage.cols &&
            roi.y + roi.height <= grayImage.rows)
        {
            *targets[i] = cv::mean(grayImage(roi))[0];
            cv::rectangle(roiImage, roi, colors[i], 1);
        }
    }
}
