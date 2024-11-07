#include "pch.h"
#include "CubeRenderer.h"

#include <windows.h>
#include <GL/gl.h>
#include <GL/glu.h>

float angle = 0.0f;

// OpenGL 컨텍스트 초기화
//void InitializeOpenGLContext(HWND hwnd)
//{
//    HDC hdc = GetDC(hwnd);
//    PIXELFORMATDESCRIPTOR pfd = { 0 };
//
//    pfd.nSize = sizeof(PIXELFORMATDESCRIPTOR);
//    pfd.nVersion = 1;
//    pfd.dwFlags = PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER | PFD_DRAW_TO_WINDOW;
//    pfd.iPixelType = PFD_TYPE_RGBA;
//    pfd.cColorBits = 24;
//    pfd.cRedBits = 8;
//    pfd.cGreenBits = 8;
//    pfd.cBlueBits = 8;
//    pfd.cAlphaBits = 8;
//    pfd.cDepthBits = 24;
//
//    int pf = ChoosePixelFormat(hdc, &pfd);
//    SetPixelFormat(hdc, pf, &pfd);
//
//    HGLRC hglrc = wglCreateContext(hdc);
//    wglMakeCurrent(hdc, hglrc);
//}
void InitializeOpenGLContext()
{
    // 여기서는 OpenGL 컨텍스트를 이미 설정했으므로 추가로 핸들링할 필요가 없습니다.
    // OpenTK에서 관리된 컨텍스트를 C#에서 이미 설정해 주었기 때문에, C++는 그저 렌더링만 담당합니다.
    // 깊이 테스트 활성화
    glEnable(GL_DEPTH_TEST);  // 깊이 테스트 활성화
    glClearColor(0.0f, 0.0f, 0.0f, 1.0f);  // 배경색을 검은색으로 설정
    glViewport(0, 0, 800, 600); // 뷰포트 설정
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    //gluPerspective(45.0f, 800.0f / 600.0f, 0.1f, 100.0f);
    glOrtho(-2.0, 2.0, -2.0, 2.0, -1.0, 1.0);
    glMatrixMode(GL_MODELVIEW);
}

// OpenGL 뷰포트 크기 조정
void ResizeOpenGLViewport(int width, int height)
{
    glViewport(0, 0, width, height);
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    glOrtho(-2.0, 2.0, -2.0, 2.0, -2.0, 2.0);
    glMatrixMode(GL_MODELVIEW);
}

void DrawSquare()
{
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

    glLoadIdentity();
    glTranslatef(0.0f, 0.0f, 0.0f); // 원근 투영이 없기 때문에 z 축 변환 필요 없음
    glRotatef(angle, 0.0f, 0.0f, 1.0f); // z축을 기준으로 회전

    glBegin(GL_QUADS);

    // 2D 사각형 그리기
    glColor3f(1.0f, 0.0f, 0.0f); // Red
    glVertex2f(-1.0f, -1.0f);
    glVertex2f(1.0f, -1.0f);
    glVertex2f(1.0f, 1.0f);
    glVertex2f(-1.0f, 1.0f);

    glEnd();
    glFlush();

    angle += 1.0f; // 회전각도를 증가시켜서 회전 효과를 줍니다.
}
// 큐브 그리기
void DrawCube()
{
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

    glLoadIdentity();
    glTranslatef(0.0f, 0.0f, -5.0f);
    glRotatef(angle, 1.0f, 0.0f, 0.0f); // X축을 중심으로 회전

    glBegin(GL_QUADS);

    // Front face
    glColor3f(1.0f, 0.0f, 0.0f); // Red
    glVertex3f(-1.0f, -1.0f, 1.0f);
    glVertex3f(1.0f, -1.0f, 1.0f);
    glVertex3f(1.0f, 1.0f, 1.0f);
    glVertex3f(-1.0f, 1.0f, 1.0f);

    // Back face
    glColor3f(0.0f, 1.0f, 0.0f); // Green
    glVertex3f(-1.0f, -1.0f, -1.0f);
    glVertex3f(-1.0f, 1.0f, -1.0f);
    glVertex3f(1.0f, 1.0f, -1.0f);
    glVertex3f(1.0f, -1.0f, -1.0f);

    // Top face
    glColor3f(0.0f, 0.0f, 1.0f); // Blue
    glVertex3f(-1.0f, 1.0f, -1.0f);
    glVertex3f(-1.0f, 1.0f, 1.0f);
    glVertex3f(1.0f, 1.0f, 1.0f);
    glVertex3f(1.0f, 1.0f, -1.0f);

    // Bottom face
    glColor3f(1.0f, 1.0f, 0.0f); // Yellow
    glVertex3f(-1.0f, -1.0f, -1.0f);
    glVertex3f(1.0f, -1.0f, -1.0f);
    glVertex3f(1.0f, -1.0f, 1.0f);
    glVertex3f(-1.0f, -1.0f, 1.0f);

    // Right face
    glColor3f(0.0f, 1.0f, 1.0f); // Cyan
    glVertex3f(1.0f, -1.0f, -1.0f);
    glVertex3f(1.0f, 1.0f, -1.0f);
    glVertex3f(1.0f, 1.0f, 1.0f);
    glVertex3f(1.0f, -1.0f, 1.0f);

    // Left face
    glColor3f(1.0f, 0.0f, 1.0f); // Magenta
    glVertex3f(-1.0f, -1.0f, -1.0f);
    glVertex3f(-1.0f, -1.0f, 1.0f);
    glVertex3f(-1.0f, 1.0f, 1.0f);
    glVertex3f(-1.0f, 1.0f, -1.0f);

    glEnd();
    glFlush(); // 추가: 그리기 명령어가 즉시 실행되도록 함

    angle += 0.05f;
}
