#pragma once
//extern "C" __declspec(dllexport) void InitializeOpenGLContext(HWND hwnd);
extern "C" __declspec(dllexport) void InitializeOpenGLContext();
extern "C" __declspec(dllexport) void DrawCube();
extern "C" __declspec(dllexport) void DrawSquare();
extern "C" __declspec(dllexport) void ResizeOpenGLViewport(int width, int height);