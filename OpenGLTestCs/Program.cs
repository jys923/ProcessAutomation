using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace OpenGLTestCs
{
    // This is where all OpenGL code will be written.
    // OpenToolkit allows for several functions to be overriden to extend functionality; this is how we'll be writing code.
    public class Window : GameWindow
    {
        // A simple constructor to let us set properties like window size, title, FPS, etc. on the window.
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
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

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        var gameWindowSettings = new GameWindowSettings();
//        var nativeWindowSettings = new NativeWindowSettings()
//        {
//            Size = new Vector2i(800, 600),
//            Profile = ContextProfile.Compatability,
//            Title = "OpenTK Immediate Mode 3D Example"
//        };

//        using (var window = new Simple3DWindow(gameWindowSettings, nativeWindowSettings))
//        {
//            window.Run();
//        }
//    }
//}

//public class Simple3DWindow : GameWindow
//{
//    private float _rotation = 0.0f;

//    public Simple3DWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
//        : base(gameWindowSettings, nativeWindowSettings)
//    {
//    }

//    protected override void OnLoad()
//    {
//        base.OnLoad();
//        GL.ClearColor(Color4.CornflowerBlue);
//        GL.Enable(EnableCap.DepthTest); // 깊이 테스트 활성화
//    }

//    protected override void OnResize(ResizeEventArgs e)
//    {
//        base.OnResize(e);
//        GL.Viewport(0, 0, Size.X, Size.Y);
//        GL.MatrixMode(MatrixMode.Projection);
//        GL.LoadIdentity();
//        Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Size.X / (float)Size.Y, 0.1f, 100.0f);
//        GL.LoadMatrix(ref perspective);
//    }

//    protected override void OnUpdateFrame(FrameEventArgs e)
//    {
//        base.OnUpdateFrame(e);
//        _rotation += 0.01f;
//    }

//    protected override void OnRenderFrame(FrameEventArgs e)
//    {
//        base.OnRenderFrame(e);
//        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

//        GL.MatrixMode(MatrixMode.Modelview);
//        GL.LoadIdentity();
//        GL.Translate(0.0f, 0.0f, -5.0f);
//        GL.Rotate(_rotation, 0.0f, 1.0f, 0.0f);

//        RenderCube();

//        SwapBuffers();
//    }

//    private void RenderCube()
//    {
//        GL.Begin(PrimitiveType.Quads);

//        GL.Color3(1.0, 0.0, 0.0); // 빨간색
//        GL.Vertex3(-1.0, -1.0, -1.0);
//        GL.Vertex3(1.0, -1.0, -1.0);
//        GL.Vertex3(1.0, 1.0, -1.0);
//        GL.Vertex3(-1.0, 1.0, -1.0);

//        GL.Color3(0.0, 1.0, 0.0); // 초록색
//        GL.Vertex3(-1.0, -1.0, 1.0);
//        GL.Vertex3(1.0, -1.0, 1.0);
//        GL.Vertex3(1.0, 1.0, 1.0);
//        GL.Vertex3(-1.0, 1.0, 1.0);

//        GL.Color3(0.0, 0.0, 1.0); // 파란색
//        GL.Vertex3(-1.0, -1.0, -1.0);
//        GL.Vertex3(-1.0, -1.0, 1.0);
//        GL.Vertex3(-1.0, 1.0, 1.0);
//        GL.Vertex3(-1.0, 1.0, -1.0);

//        GL.Color3(1.0, 1.0, 0.0); // 노란색
//        GL.Vertex3(1.0, -1.0, -1.0);
//        GL.Vertex3(1.0, -1.0, 1.0);
//        GL.Vertex3(1.0, 1.0, 1.0);
//        GL.Vertex3(1.0, 1.0, -1.0);

//        GL.Color3(1.0, 0.0, 1.0); // 보라색
//        GL.Vertex3(-1.0, -1.0, -1.0);
//        GL.Vertex3(1.0, -1.0, -1.0);
//        GL.Vertex3(1.0, -1.0, 1.0);
//        GL.Vertex3(-1.0, -1.0, 1.0);

//        GL.Color3(0.0, 1.0, 1.0); // 청록색
//        GL.Vertex3(-1.0, 1.0, -1.0);
//        GL.Vertex3(1.0, 1.0, -1.0);
//        GL.Vertex3(1.0, 1.0, 1.0);
//        GL.Vertex3(-1.0, 1.0, 1.0);

//        GL.End();
//    }
//}
