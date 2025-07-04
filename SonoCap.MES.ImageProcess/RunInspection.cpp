#include "RunInspection.h"
#include "Util.h"
#include "GeoInspection.h"
#include "GrayInspection.h"
#include "ResInspection.h"
#include "MyOpenCVWrapper.h"

#define INSPECT_GEO  (1 << 0)
#define INSPECT_GRAY (1 << 1)
#define INSPECT_RES  (1 << 2)
#define INSPECT_ALL  (INSPECT_GEO | INSPECT_GRAY | INSPECT_RES)

void MyOpenCVWrapper::RunInspection(System::IntPtr inputBuffer, int imageWidth, int imageHeight,
    System::IntPtr resultBuffer, System::IntPtr textBuffer, int testPartFlags)
{
    // 초기화
    InspectionResult inspection;
    memset(resultBuffer.ToPointer(), 0, imageWidth * imageHeight * 4);

    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) {
        std::string errorText = R"({"error":"invalid image"})";
        memcpy(textBuffer.ToPointer(), errorText.c_str(), errorText.size() + 1);
        return;
    }
    
    cv::Mat resultImage = inputImage.clone();

    showAndSaveImage("origin", resultImage);

    cv::Mat gray;
    cv::cvtColor(resultImage, gray, cv::COLOR_BGRA2GRAY);

	//cv::Mat referenceImage = MyOpenCVWrapper::OpenCVWrapper::GetReferenceImage();
 //   //float angle = estimateRotationByPhaseCorrelation(referenceImage, gray); //오류
 //   //float angle = estimateVerticalShiftByFFT(referenceImage, gray); // 검출 이상 안됨
 //   //float angle = estimateRotationByCircularShift(referenceImage, gray); //느림
 //   float angle = estimateRotationByORB(referenceImage, gray);

	//Console::WriteLine("Estimated angle: {0}", angle);

 //   resultImage = rotateImage(resultImage, angle); // or -anglePolar

 //   showAndSaveImage("rotate", resultImage);

    int halfW = imageWidth / 2;
    int halfH = imageHeight / 2;

    cv::Rect roiGray(halfW, 0, halfW, halfH);    // 1사분면
    cv::Rect roiRes(0, 0, halfW, halfH);         // 2사분면
    cv::Rect roiGeo(0, halfH, imageWidth, halfH);// 3,4사분면

    // 검사 실행
    if (testPartFlags & INSPECT_GRAY) {
        GrayInspection(resultImage(roiGray), inspection.Gray);
    }

    if (testPartFlags & INSPECT_RES) {
        ResInspection(resultImage(roiRes), inspection.Res);
    }

    if (testPartFlags & INSPECT_GEO) {
        GeoInspection(resultImage(roiGeo), inspection.Geo);
    }

	showAndSaveImage("Result", resultImage);

    // JSON 결과 직렬화
    nlohmann::json j = inspection;
    std::string finalText = j.dump();

    // 복사
    memcpy(textBuffer.ToPointer(), finalText.c_str(), finalText.size() + 1);
    memcpy(resultBuffer.ToPointer(), resultImage.data, resultImage.total() * resultImage.elemSize());
}
