#include <gl/glut.h>

void DoInit()
{
    glClearColor(1.0, 0.0, 0.0, 1.0);
}

void DoDisplay()
{
    //glClearColor(0.0, 0.0, 0.0, 1.0);
    glClear(GL_COLOR_BUFFER_BIT);
    //glColor3f(1.0, 0.0, 0.0);
    /*glBegin(GL_TRIANGLES);
    glVertex2f(0.0, 0.5);
    glVertex2f(-0.5, -0.5);
    glVertex2f(0.5, -0.5);
    glEnd();*/

    glBegin(GL_POLYGON);

    //glColor3f(1.0, 1.0, 1.0);
    glVertex2f(0.0, 0.6);
    glColor3f(1.0, 0.0, 0.0);
    glVertex2f(-0.6, 0.0);
    //glColor3f(1.0, 0.0, 0.0);
    glVertex2f(-0.4, -0.6);
    glColor3f(0.0, 1.0, 0.0);
    glVertex2f(0.4, -0.6);
    glColor3f(0.0, 0.0, 1.0);
    glVertex2f(0.6, 0.0);
    glEnd();
    glFlush();
    /*
    * 더블 버퍼 사용시
    * 프론트 버퍼(front buffer)에서 백 버퍼(back buffer)로 스왑
    */
    //glutSwapBuffers(); 
}

int main(int argc, char* argv[]) {
    glutInit(&argc, argv);
    glutCreateWindow("OpenGL");
    //DoInit();
    glutDisplayFunc(DoDisplay);
    glutMainLoop();
}