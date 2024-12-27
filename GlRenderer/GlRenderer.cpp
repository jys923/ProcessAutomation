#include "pch.h"

#include "GlRenderer.h"

#include <gl/glut.h>

void GlRenderer::RendererInterface::DrawSquare()
{
	//throw gcnew System::NotImplementedException();
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

    GlRenderer::RendererInterface::angle += 1.0f; // 회전각도를 증가시켜서 회전 효과를 줍니다.
}

void GlRenderer::RendererInterface::DrawCube()
{
	//throw gcnew System::NotImplementedException();
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
