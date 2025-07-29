#pragma once  
#include "util.h"  
#include "ConfigManager.h"

using namespace System;  

namespace MyOpenCVWrapper {  

   public ref class OpenCVWrapper {  
   public:  
       /*OpenCVWrapper() {  
           setOpenCVLogLevel();  
           Console::WriteLine("Instance Constructor Called");  
       }*/
       static void EnvGeoInspection(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer);

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

   private:  
       //static cv::Mat* referenceImage; // 포인터로 변경하여 비 관리 형식을 허용  
       // 정적 생성자 추가 (클래스 로드 시 한 번만 실행됨)  
       static OpenCVWrapper() {  
           //setOpenCVLogLevel();  
           //referenceImage = new cv::Mat(); // 포인터 초기화  
           Logger::Information("Static Constructor Called");

           ConfigManager& config = ConfigManager::getInstance();
           config.loadConfigFromFile("./SonoCap.MES.ImageProcess.json"); // 설정 파일 경로 지정

           // 로드 성공 여부 확인 (옵션)
           if (config.isConfigLoaded()) {
               Logger::Information("ConfigManager: 설정 파일 로드 완료.");
           }
           else {
               Logger::Information("ConfigManager: 설정 파일 로드 실패 또는 기본값 사용.");
           }

           setOpenCVLogLevelFromConfig(config.getGeneralSettings().logLevel);
       }

       static void setOpenCVLogLevelFromConfig(const std::string& logLevelStr) {
           cv::utils::logging::LogLevel level = cv::utils::logging::LOG_LEVEL_SILENT; // 기본값은 출력 안 함

           if (logLevelStr == "Fatal") {
               level = cv::utils::logging::LOG_LEVEL_FATAL;
           }
           else if (logLevelStr == "Error") {
               level = cv::utils::logging::LOG_LEVEL_ERROR;
           }
           else if (logLevelStr == "Warn") {
               level = cv::utils::logging::LOG_LEVEL_WARNING;
           }
           else if (logLevelStr == "Info") {
               level = cv::utils::logging::LOG_LEVEL_INFO;
           }
           else if (logLevelStr == "Debug") {
               level = cv::utils::logging::LOG_LEVEL_DEBUG;
           }
           // else "Silent" 또는 알 수 없는 값은 LOG_LEVEL_SILENT로 유지

           cv::utils::logging::setLogLevel(level);
       }

       // 내부 함수로 OpenCV 로그 레벨 설정  
       static void setOpenCVLogLevel() {  
           // OpenCV 로깅 레벨 설정  
           cv::utils::logging::setLogLevel(cv::utils::logging::LOG_LEVEL_ERROR);  
       }  
   };  
}
