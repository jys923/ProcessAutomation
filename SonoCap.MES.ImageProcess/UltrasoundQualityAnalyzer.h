#pragma once

namespace MyOpenCVWrapper
{
    void AnalyzeSharpness(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
    void AnalyzeContrast(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
    void AnalyzeBrightness(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
    void AnalyzeSNR(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
    void AnalyzeSpeckleIndex(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
    void AnalyzeEntropy(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
    void AnalyzeEdgeDensity(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
    void AnalyzeLocalVariance(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
    void AnalyzeCNR(System::IntPtr buffer, int width, int height, System::IntPtr textBuffer);
    void AnalyzeFFT(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr textBuffer);
}