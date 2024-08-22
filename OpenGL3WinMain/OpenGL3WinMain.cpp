#include <windows.h>
#include <gl/glut.h>
#include <stdio.h>
void DoDisplay();
void DoKeyboard(unsigned char key, int x, int y);
GLfloat xAngle, yAngle, zAngle;
int APIENTRY WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance
    , LPSTR lpszCmdParam, int nCmdShow)
{
    glutInit(&__argc, __argv);
    glutCreateWindow("OpenGL");
    glutDisplayFunc(DoDisplay);
    glutKeyboardFunc(DoKeyboard);
    glutMainLoop();
    return 0;
}
void DoKeyboard(unsigned char key, int x, int y)
{
    switch (key) {
    case 'a':yAngle += 2; break;
    case 'd':yAngle -= 2; break;
    case 'w':xAngle += 2; break;
    case 's':xAngle -= 2; break;
    case 'q':zAngle += 2; break;
    case 'e':zAngle -= 2; break;
    case 'z':xAngle = yAngle = zAngle = 0.0; break;
    }
    char info[128];
    sprintf_s(info, "x=%.1f, y=%.1f, z=%.1f", xAngle, yAngle, zAngle);
    glutSetWindowTitle(info);
    glutPostRedisplay();
}
void DoDisplay()
{
    static GLfloat vert[] = {
         0, 0, -0.8,          // 12시
         0.5, 0.5, 0,
         -0.5, 0.5, 0,
         0, 0, -0.8,          // 9시
         -0.5, 0.5, 0,
         -0.5, -0.5, 0,
         0, 0, -0.8,          // 6시
         -0.5, -0.5, 0,
         0.5, -0.5, 0,
         0, 0, -0.8,          // 3시
         0.5, -0.5, 0,
         0.5, 0.5, 0,
    };
    glEnableClientState(GL_VERTEX_ARRAY);
    glVertexPointer(3, GL_FLOAT, 0, vert);
    glClear(GL_COLOR_BUFFER_BIT);
    glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
    glMatrixMode(GL_MODELVIEW);
    glPushMatrix();
    glRotatef(xAngle, 1.0f, 0.0f, 0.0f);
    glRotatef(yAngle, 0.0f, 1.0f, 0.0f);
    glRotatef(zAngle, 0.0f, 0.0f, 1.0f);
    glRectf(-0.5, 0.5, 0.5, -0.5);
    glBegin(GL_TRIANGLES);
    for (int i = 0; i < sizeof(vert) / sizeof(vert[0]); i += 3) {
        glVertex3f(vert[i], vert[i + 1], vert[i + 2]);
    }
    /*for (int i = 0; i < sizeof(vert) / sizeof(vert[0]) / 3; i++) {
        glArrayElement(i);
    }*/
    glEnd();
    glPopMatrix();
    glFlush();
}