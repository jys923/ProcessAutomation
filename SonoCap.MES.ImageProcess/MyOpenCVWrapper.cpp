#include "MyOpenCVWrapper.h"
#include "AlignProcess.h"
#include "ResolutionProcess.h"
#include "GeometricDistortionProcess.h"
#include "GrayProcess.h"
#include "UltrasoundQualityAnalyzer.h"
#include "RunInspection.h"
#include "EnvGeoInspection.h"

void MyOpenCVWrapper::OpenCVWrapper::EnvGeoInspection(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer)
{
    MyOpenCVWrapper::EnvGeoInspection(buffer, width, height, resultBuffer, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::RunInspection(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer, int testPart) {
	MyOpenCVWrapper::RunInspection(buffer, width, height, resultBuffer, textBuffer, testPart);
}

void MyOpenCVWrapper::OpenCVWrapper::AlignProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer) {
    MyOpenCVWrapper::AlignProcess(buffer, width, height, resultBuffer, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::ResolutionProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer) {
    MyOpenCVWrapper::ResolutionProcess(buffer, width, height, resultBuffer, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::GeometricDistortionProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer) {
    MyOpenCVWrapper::GeometricDistortionProcess(buffer, width, height, resultBuffer, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::GrayProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer) {
    MyOpenCVWrapper::GrayProcess(buffer, width, height, resultBuffer, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::AnalyzeSharpness(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer) {
	MyOpenCVWrapper::AnalyzeSharpness(buffer, width, height, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::AnalyzeContrast(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer) {
	MyOpenCVWrapper::AnalyzeContrast(buffer, width, height, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::AnalyzeBrightness(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer) {
	MyOpenCVWrapper::AnalyzeBrightness(buffer, width, height, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::AnalyzeSNR(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer) {
	MyOpenCVWrapper::AnalyzeSNR(buffer, width, height, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::AnalyzeSpeckleIndex(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer) {
	MyOpenCVWrapper::AnalyzeSpeckleIndex(buffer, width, height, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::AnalyzeEntropy(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer) {
	MyOpenCVWrapper::AnalyzeEntropy(buffer, width, height, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::AnalyzeEdgeDensity(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer) {
	MyOpenCVWrapper::AnalyzeEdgeDensity(buffer, width, height, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::AnalyzeLocalVariance(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer) {
	MyOpenCVWrapper::AnalyzeLocalVariance(buffer, width, height, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::AnalyzeCNR(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer) {
	MyOpenCVWrapper::AnalyzeCNR(buffer, width, height, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::AnalyzeFFT(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer) {
	MyOpenCVWrapper::AnalyzeFFT(buffer, width, height, textBuffer);
}

// ref class 바깥, 전역 공간에 위치
static cv::Mat g_referenceImage;

cv::Mat& MyOpenCVWrapper::OpenCVWrapper::GetReferenceImage() {
    return g_referenceImage;
}

void MyOpenCVWrapper::OpenCVWrapper::SetReferenceImage(System::String^ path) {
    System::IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(path);
    const char* nativePath = static_cast<const char*>(ptr.ToPointer());

    Logger::Information("레퍼런스 이미지 경로: {0}", path);

	g_referenceImage = cv::imread(nativePath, cv::IMREAD_GRAYSCALE);

    if (g_referenceImage.empty()) {
        Logger::Information("레퍼런스 이미지 로딩 실패");
    }
    else {
        Logger::Information("레퍼런스 이미지 로딩 완료");
    }

    System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr);
}
