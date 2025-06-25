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

       static void RunInspection(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer, int testPart);  
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
       static void SetAnalyzeConfigJson(System::String^ json)
       {
           System::IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(json);
           const char* nativeStr = static_cast<const char*>(ptr.ToPointer());
           //::SetAnalyzeConfigJson(nativeStr); // native C++ 함수 호출
           System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr);
       };
       static cv::Mat& GetReferenceImage();
       static void SetReferenceImage(System::String^ path);
       //static void SetReferenceImage(String^ path) {
       //    System::IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(path);
       //    const char* nativePath = static_cast<const char*>(ptr.ToPointer());

       //    Console::WriteLine("레퍼런스 이미지 경로: {0}", path);

       //    *referenceImage = cv::imread(nativePath, cv::IMREAD_GRAYSCALE);
       //    if (referenceImage->empty()) {
       //        Console::WriteLine("레퍼런스 이미지 로딩 실패");
       //    }
       //    else {
       //        Console::WriteLine("레퍼런스 이미지 로딩 완료");
       //    }

       //    System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr); // 💡 메모리 누수 방지
       //};

   private:  
       //static cv::Mat* referenceImage; // 포인터로 변경하여 비 관리 형식을 허용  
       // 정적 생성자 추가 (클래스 로드 시 한 번만 실행됨)  
       static OpenCVWrapper() {  
           setOpenCVLogLevel();  
           //referenceImage = new cv::Mat(); // 포인터 초기화  
           Console::WriteLine("Static Constructor Called");  
       }  
       // 내부 함수로 OpenCV 로그 레벨 설정  
       static void setOpenCVLogLevel() {  
           // OpenCV 로깅 레벨 설정  
           cv::utils::logging::setLogLevel(cv::utils::logging::LOG_LEVEL_ERROR);  
       }  
   };  
}
