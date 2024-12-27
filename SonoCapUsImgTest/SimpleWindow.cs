using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using ErrorCode = OpenTK.Graphics.OpenGL.ErrorCode;
using GlRenderer;

namespace SonoCapUsImgTest
{
    // This is where all OpenGL code will be written.
    // OpenToolkit allows for several functions to be overriden to extend functionality; this is how we'll be writing code.
    public class SimpleWindow : GameWindow
    {

        // A simple constructor to let us set properties like window size, title, FPS, etc. on the window.
        public SimpleWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        // Now, we start initializing OpenGL.
        //protected override void OnLoad()
        //{
        //    base.OnLoad();
        //    GL.ClearColor(Color4.CornflowerBlue);
        //    GL.Enable(EnableCap.DepthTest); // 깊이 테스트 활성화
        //    InitializeOpenGLContext();
        //}
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(OpenTK.Mathematics.Color4.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Viewport(0, 0, 800, 600);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            //GL.Ortho(-2.0, 2.0, -2.0, 2.0, -1.0, 1.0);
            Perspective(45.0f, 800.0f / 600.0f, 0.1f, 100.0f);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        private void Perspective(double fovy, double aspect, double zNear, double zFar)
        {
            double fH = Math.Tan(fovy / 360.0 * Math.PI) * zNear;
            double fW = fH * aspect;
            GL.Frustum(-fW, fW, -fH, fH, zNear, zFar);
        }

        // Now that initialization is done, let's create our render loop.
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            //DrawSquare();
            RendererInterface.DrawCube();

            var error = GL.GetError(); 
            if (error != ErrorCode.NoError) 
            { 
                Console.WriteLine($"OpenGL Error: {error}"); 
            }

            SwapBuffers();
        }

        // This function runs on every update frame.
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Check if the Escape button is currently being pressed.
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                // If it is, close the window.
                Close();
            }

            base.OnUpdateFrame(e);
        }
    }
}
