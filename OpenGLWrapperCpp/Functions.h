#pragma once
#include <GL/gl.h>
#include <GL/glu.h>
#include <GL/glut.h>
#include <windows.h>
#include <iostream>

extern "C" __declspec(dllexport) void drawPolygon();
extern "C" __declspec(dllexport) void initializeOpenGL(HWND hWnd);
extern "C" __declspec(dllexport) void cleanupOpenGL();
extern "C" __declspec(dllexport) void RunOpenGL();