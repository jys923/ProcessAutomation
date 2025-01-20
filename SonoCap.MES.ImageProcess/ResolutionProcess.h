#pragma once

namespace MyOpenCVWrapper {
    //void ResolutionProcess(System::IntPtr buffer, int width, int height);
    void ResolutionProcess(System::IntPtr buffer, int width, int height, System::IntPtr resultBuffer, System::IntPtr textBuffer);
}
