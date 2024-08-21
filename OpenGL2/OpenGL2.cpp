#include <gl/glut.h>
void DoInit()
{
    glClearColor(1.0, 0.0, 0.0, 1.0);
}
void DoDisplay2()
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
void DoDisplay3()
{
    glClear(GL_COLOR_BUFFER_BIT);
    GLfloat y;
    GLfloat w = 1;
    for (y = 0.8; y > -0.8; y -= 0.2) {
        glLineWidth(w++);
        glBegin(GL_LINES);
        glVertex2f(-0.8, y);
        glVertex2f(0.8, y);
        glEnd();
    }
    glFlush();
}
void DoDisplay4()
{
    glClear(GL_COLOR_BUFFER_BIT);
    glBegin(GL_TRIANGLES);
    GLfloat x = -0.8;
    GLfloat y = 0.4;
    for (int i = 0; i < 6; i++) {
        glVertex2f(x, y);
        x += 0.3;
        y *= -1;
    }
    glEnd();
    glFlush();
}
void DoDisplay5()
{
    glClear(GL_COLOR_BUFFER_BIT);
    glShadeModel(GL_FLAT);
    glBegin(GL_TRIANGLE_STRIP);
    GLfloat x = -0.8;
    GLfloat y = 0.4;
    for (int i = 0; i < 6; i++) {
        if (i % 2 == 0) {
            glColor3f(1.0, 0.0, 0.0);
        }
        else {
            glColor3f(0.0, 1.0, 0.0);
        }
        glVertex2f(x, y);
        x += 0.3;
        y *= -1;
    }
    glEnd();
    glFlush();
}
void DoDisplay6()

{

    glClear(GL_COLOR_BUFFER_BIT);

    glShadeModel(GL_FLAT);



    glBegin(GL_TRIANGLE_FAN);

    glColor3f(1.0, 0.0, 0.0);

    glVertex2f(0.0, 0.0);

    glVertex2f(0.0, 0.5);

    glVertex2f(-0.35, 0.35);



    glColor3f(0.0, 1.0, 0.0);

    glVertex2f(-0.5, 0.0);
    


    glColor3f(1.0, 0.0, 0.0);

    glVertex2f(-0.35, -0.35);



    glColor3f(0.0, 1.0, 0.0);

    glVertex2f(0.0, -0.5);

    glEnd();

    glFlush();

}
void DoDisplay()

{

    glClear(GL_COLOR_BUFFER_BIT);



    glRectf(-0.8, 0.8, 0.8, -0.8);

    glFlush();

}

int main(int argc, char* argv[]) {
    glutInit(&argc, argv);
    glutCreateWindow("OpenGL");
    //DoInit();
    glutDisplayFunc(DoDisplay);
    glutMainLoop();
}