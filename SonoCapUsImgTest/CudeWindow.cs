using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Runtime.InteropServices;

namespace SonoCapUsImgTest
{
    public class CudeWindow : GameWindow
    {
        private int Width = 512;
        private int Height = 512;

        // A simple constructor to let us set properties like window size, title, FPS, etc. on the window.
        public CudeWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        // C++ DLL에서 제공하는 함수들
        [DllImport("CubeRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitializeOpenGLContext();

        [DllImport("CubeRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawCube();

        [DllImport("CubeRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResizeOpenGLViewport(int width, int height);

        // OpenGL 컨텍스트 초기화
        protected override void OnLoad()
        {
            base.OnLoad();

            // OpenGL 상태 초기화
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);  // 배경색을 검은색으로 설정
            GL.Enable(EnableCap.DepthTest);  // 깊이 테스트 활성화

            // C++ DLL에서 OpenGL 컨텍스트를 초기화
            InitializeOpenGLContext();

            // 초기 뷰포트 크기 설정
            ResizeOpenGLViewport(Width, Height);
        }

        // 창 크기 변경 시 뷰포트 크기 변경
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            ResizeOpenGLViewport(Width, Height);
        }

        // 매 프레임마다 큐브 그리기
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // 화면 클리어
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // C++ DLL에서 큐브 그리기
            DrawCube();

            // 화면을 표시하기 위해 SwapBuffers 호출
            SwapBuffers();
        }
    }
}
