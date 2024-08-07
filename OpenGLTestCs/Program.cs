using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Drawing;

class Program
{
    private static GameWindow window = default!;

    static void Main()
    {
        var gameWindowSettings = new GameWindowSettings()
        {
            UpdateFrequency = 60.0
        };

        var nativeWindowSettings = new NativeWindowSettings()
        {
            //ClientSize = new System.Drawing.Size(800, 600),
            Title = "OpenTK Sample",
            APIVersion = new Version(4, 6),
            // 필요에 따라 OpenGL 기능 버전과 컨텍스트를 설정
        };

        window = new GameWindow(gameWindowSettings, nativeWindowSettings);

        //window = new GameWindow(GameWindowSettings.Default, NativeWindowSettings.Default);

        window.Load += OnLoad;
        window.RenderFrame += OnRenderFrame;
        window.UpdateFrame += OnUpdateFrame;
        window.KeyDown += OnKeyDown;

        window.Run();
    }

    private static void OnLoad()
    {
        GL.ClearColor(Color.CornflowerBlue); // 배경색을 검정색으로 설정
        GL.ClearDepth(1.0);// '깊이 버퍼에대한 값을 지정합니다.
        GL.MatrixMode(MatrixMode.Projection);
        GL.ShadeModel(ShadingModel.Smooth);//
        GL.LoadIdentity();
        GL.Ortho(0, 800, 0, 600, -1, 1); // 2D 투영 설정
    }

    private static void OnRenderFrame(FrameEventArgs frame)
    {
        GL.LoadIdentity();
        //OnRenderFrame(frame);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.Color3(Color.Tomato); // 사각형 색상 설정 (빨간색)
        GL.Begin(PrimitiveType.Quads); // 사각형을 그리기 위한 시작
        GL.Vertex2(100, 100); // 왼쪽 아래
        GL.Vertex2(200, 100); // 오른쪽 아래
        GL.Vertex2(200, 200); // 오른쪽 위
        GL.Vertex2(100, 200); // 왼쪽 위
        GL.End(); // 사각형 그리기 종료

        window.SwapBuffers(); // 화면 업데이트
    }

    private static void OnUpdateFrame(FrameEventArgs frame)
    {
        // 입력 처리나 애니메이션 업데이트 등이 필요한 경우 여기서 처리
    }

    private static void OnKeyDown(KeyboardKeyEventArgs e)
    {
        Console.WriteLine($"{e.Key}");
        if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape)
        {
            window.Close();
        }
    }
}

