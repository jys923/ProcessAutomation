#pragma once
#include "util.h"

using namespace System;

namespace MyOpenCVWrapper {

    public ref class OpenCVWrapper {
    public:
        /*OpenCVWrapper() {
            setOpenCVLogLevel();
            Console::WriteLine("Instance Constructor Called");
        }*/
        // AlignProcess 함수 정의
        static void AlignProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer);

        // ResolutionProcess 함수 정의
        static void ResolutionProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer);
        
        static void GeometricDistortionProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer);

        static void GrayProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer);
        
        static void AnalyzeSharpness(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
        static void AnalyzeContrast(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
        static void AnalyzeBrightness(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
        static void AnalyzeSNR(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
        static void AnalyzeSpeckleIndex(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
        static void AnalyzeEntropy(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
        static void AnalyzeEdgeDensity(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
        static void AnalyzeLocalVariance(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
        static void AnalyzeCNR(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
        static void AnalyzeFFT(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);

    private:
        // 정적 생성자 추가 (클래스 로드 시 한 번만 실행됨)
        static OpenCVWrapper() {
            setOpenCVLogLevel();
            Console::WriteLine("Static Constructor Called");
        }
        // 내부 함수로 OpenCV 로그 레벨 설정
        static void setOpenCVLogLevel() {
            // OpenCV 로깅 레벨 설정
            cv::utils::logging::setLogLevel(cv::utils::logging::LOG_LEVEL_ERROR);
        }
    };
}

