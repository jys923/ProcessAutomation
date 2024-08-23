#include <GL/glut.h>

// 카메라 파라미터
GLfloat eyeX = 0.0f, eyeY = 0.0f, eyeZ = 5.0f;
GLfloat centerX = 0.0f, centerY = 0.0f, centerZ = 0.0f;
GLfloat upX = 0.0f, upY = 1.0f, upZ = 0.0f;

// 카메라 설정 함수
void setupCamera() {
    glMatrixMode(GL_MODELVIEW);
    glLoadIdentity();
    gluLookAt(eyeX, eyeY, eyeZ,  // 카메라 위치
        centerX, centerY, centerZ, // 카메라가 바라보는 점
        upX, upY, upZ); // 업 벡터
}

void display() {
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
    setupCamera();  // 카메라 설정
    glutWireTeapot(0.3);  // 와이어프레임 찻주전자를 그리기
    glFlush();
}

void reshape(int w, int h) {
    glViewport(0, 0, w, h);
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    gluPerspective(45.0, (GLfloat)w / (GLfloat)h, 1.0, 100.0);  // 원근 투영 설정
}

void keyboard(unsigned char key, int x, int y) {
    GLfloat step = 0.1f; // 카메라 이동 스텝

    switch (key) {
        // 카메라 위치 조절
    case 'w': // 카메라를 앞쪽으로 이동
        eyeZ -= step;
        break;
    case 's': // 카메라를 뒤쪽으로 이동
        eyeZ += step;
        break;
    case 'a': // 카메라를 왼쪽으로 이동
        eyeX -= step;
        break;
    case 'd': // 카메라를 오른쪽으로 이동
        eyeX += step;
        break;
    case 'r': // 카메라를 위쪽으로 이동
        eyeY += step;
        break;
    case 'f': // 카메라를 아래쪽으로 이동
        eyeY -= step;
        break;
        // 카메라 시점 조절
    case 'i': // 카메라의 시점을 앞쪽으로 이동
        centerZ -= step;
        break;
    case 'k': // 카메라의 시점을 뒤쪽으로 이동
        centerZ += step;
        break;
    case 'j': // 카메라의 시점을 왼쪽으로 이동
        centerX -= step;
        break;
    case 'l': // 카메라의 시점을 오른쪽으로 이동
        centerX += step;
        break;
    case 'u': // 카메라의 시점을 위쪽으로 이동
        centerY += step;
        break;
    case 'n': // 카메라의 시점을 아래쪽으로 이동
        centerY -= step;
        break;
        // 업 벡터 조절
    case 'o': // 업 벡터의 Y 값을 증가
        upY += step;
        break;
    case 'p': // 업 벡터의 Y 값을 감소
        upY -= step;
        break;
    case 'm': // 업 벡터의 X 값을 증가
        upX += step;
        break;
    case ',': // 업 벡터의 X 값을 감소
        upX -= step;
        break;
    case '.': // 업 벡터의 Z 값을 증가
        upZ += step;
        break;
    case '/': // 업 벡터의 Z 값을 감소
        upZ -= step;
        break;
        // 원점으로 돌아가기
    case '0': // 카메라 위치와 시점을 원점으로 리셋
        eyeX = 0.0f; eyeY = 0.0f; eyeZ = 5.0f;
        centerX = 0.0f; centerY = 0.0f; centerZ = 0.0f;
        upX = 0.0f; upY = 1.0f; upZ = 0.0f;
        break;
    case 27: // ESC 키를 눌러서 종료
        exit(0);
        break;
    }

    glutPostRedisplay(); // 화면을 다시 그립니다
}

int APIENTRY WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance
    , LPSTR lpszCmdParam, int nCmdShow)
{
    glutInit(&__argc, __argv);
    glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB | GLUT_DEPTH);
    glutInitWindowSize(800, 600);
    glutInitWindowPosition(100, 100);
    glutCreateWindow("GLU LookAt with Keyboard Controls");

    glEnable(GL_DEPTH_TEST);

    glutDisplayFunc(display);
    glutReshapeFunc(reshape);
    glutKeyboardFunc(keyboard); // 키보드 입력 처리 함수 등록

    glutMainLoop();

    return 0;
}