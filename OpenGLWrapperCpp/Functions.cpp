#include "pch.h"
#include "Functions.h"

HDC hDC = NULL; // Device context
HGLRC hRC = NULL; // Rendering context

// 예시: OpenGL 초기화 함수
void initializeOpenGL(HWND hWnd)
{
    PIXELFORMATDESCRIPTOR pfd;
    int format;

    // Get the device context (DC)
    hDC = GetDC(hWnd);

    // Set the pixel format for the DC
    ZeroMemory(&pfd, sizeof(pfd));
    pfd.nSize = sizeof(pfd);
    pfd.nVersion = 1;
    pfd.dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER;
    pfd.iPixelType = PFD_TYPE_RGBA;
    pfd.cColorBits = 24;
    pfd.cDepthBits = 16;
    pfd.iLayerType = PFD_MAIN_PLANE;

    format = ChoosePixelFormat(hDC, &pfd);
    SetPixelFormat(hDC, format, &pfd);

    // Create and enable the OpenGL rendering context (RC)
    hRC = wglCreateContext(hDC);
    wglMakeCurrent(hDC, hRC);

    // Enable depth testing and smooth shading
    glEnable(GL_DEPTH_TEST);
    glShadeModel(GL_SMOOTH);

    // Optional: Set clear color and viewport
    glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
    glViewport(0, 0, 800, 600); // Adjust the viewport size as needed
}

void cleanupOpenGL()
{
    if (hRC)
    {
        wglMakeCurrent(NULL, NULL);
        wglDeleteContext(hRC);
        hRC = NULL;
    }

    if (hDC)
    {
        ReleaseDC(GetDesktopWindow(), hDC);
        hDC = NULL;
    }
}

// 예시: OpenGL을 이용한 그리기 함수
void drawPolygon()
{
    glBegin(GL_POLYGON);
    glVertex2f(-0.2f, -0.2f);
    glVertex2f(0.2f, -0.2f);
    glVertex2f(0.2f, 0.2f);
    glVertex2f(-0.2f, 0.2f);
    glEnd();
    glFinish();
}

void RunOpenGL()
{
    int argc;
    char** argv = nullptr;
    glutInit(&argc, argv);
    glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB);
    glutCreateWindow("OpenGL Test");

    // 그리기 함수 등록
    glutDisplayFunc(drawPolygon);

    // 메시지 루프 실행
    glutMainLoop();
}
