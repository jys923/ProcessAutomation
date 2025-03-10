#include "MyOpenCVWrapper.h"
#include "AlignProcess.h"
#include "ResolutionProcess.h"
#include "GeometricDistortionProcess.h"

void MyOpenCVWrapper::OpenCVWrapper::AlignProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer) {
    MyOpenCVWrapper::AlignProcess(buffer, width, height, resultBuffer, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::ResolutionProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer) {
    MyOpenCVWrapper::ResolutionProcess(buffer, width, height, resultBuffer, textBuffer);
}

void MyOpenCVWrapper::OpenCVWrapper::GeometricDistortionProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer) {
    MyOpenCVWrapper::GeometricDistortionProcess(buffer, width, height, resultBuffer, textBuffer);
}
