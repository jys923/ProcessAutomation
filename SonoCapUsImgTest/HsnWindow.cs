using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using Serilog;
using System.Runtime.InteropServices;
using System.Text;
using HsnLibraryCS;

namespace SonoCapUsImgTest
{
    // This is where all OpenGL code will be written.
    // OpenToolkit allows for several functions to be overriden to extend functionality; this is how we'll be writing code.
    public class HsnWindow : GameWindow
    {
        // A simple constructor to let us set properties like window size, title, FPS, etc. on the window.
        public HsnWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        static int byte_per_sample = 2;
        static int max_no_sample = 512;
        static int max_scanline = 960;

        // 환경 데이터 버퍼 준비
        static int envdata_buffer_size = max_scanline * max_no_sample * byte_per_sample;
        static byte[] envdata_buffer = new byte[envdata_buffer_size];

        // 최종 이미지 버퍼 준비
        static int width = 512; // 예시 값
        static int height = 512; // 예시 값
        static int buffer_size = width * height * 4;
        static byte[] buffer = new byte[buffer_size];

        // GCHandle로 고정하여 IntPtr로 변환
        static GCHandle finalImageHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        static IntPtr finalImagePtr = finalImageHandle.AddrOfPinnedObject();

        static GCHandle rawDataHandle = GCHandle.Alloc(envdata_buffer, GCHandleType.Pinned);
        static IntPtr rawDataPtr = rawDataHandle.AddrOfPinnedObject();

        // 메타데이터 버퍼 준비
        static StringBuilder outputMetadata = new StringBuilder(10240);

        // Now, we start initializing OpenGL.
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest); // 깊이 테스트 활성화
            GL.Viewport(0, 0, 512, 512);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            //Perspective(45.0f, 800.0f / 600.0f, 0.1f, 100.0f);
            GL.Ortho(-2.0, 2.0, -2.0, 2.0, -1.0, 1.0);
            GL.MatrixMode(MatrixMode.Modelview);
            Init();
        }

        // Now that initialization is done, let's create our render loop.
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            //GL.DrawPixels(512, 512, PixelFormat.Rgba, PixelType.Byte, buffer);
            GL.DrawPixels(512, 512, PixelFormat.Bgra, PixelType.UnsignedByte, buffer);

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

        static HsnUltrasoundOffScreenView offscrrenView;
        private static void Init()
        {
            registerCallbackBeforeInitialize();

            if (!HsnlibraryCS.HsnInterface.initialize())
            {
                Log.Information("initialize Fail");
                return;
            }

            registerCallbackAfterInitialize();

            HsnlibraryCS.HsnInterface.startProbeDetection();

            offscrrenView = new HsnUltrasoundOffScreenView(512, 512);
            offscrrenView.setTargetIPFrameRate(60);
            offscrrenView.Start(UpdateImgSource);
        }



        private static void UpdateImgSource(byte[] hsnBuffer, int width, int height, int length, MetadataInfo metadata)
        {
            Log.Information("updateimgSource");
            buffer = hsnBuffer;
        }

        static int probe = 0;

        private static void registerCallbackBeforeInitialize()
        {
            HsnlibraryCS.Callback.registerLoadingCallback(OnLoadingCallback);
            HsnlibraryCS.Callback.registerErrorStateCallback(OnErrorCallback);
        }

        private static void OnErrorCallback(string err_str, int err_num)
        {
            Log.Error(err_str, err_num);
        }

        private static void OnLoadingCallback(bool val)
        {
            Log.Information($"Loading callback executed! {val}");
        }

        private static void registerCallbackAfterInitialize()
        {
            HsnlibraryCS.HsnInterface.DeviceAttached += OnDeviceAttached;
            HsnlibraryCS.HsnInterface.DeviceDetached += OnDeviceDetached;
            HsnlibraryCS.Callback.registerENDMotorCallback(OnMotorCallback);
            HsnlibraryCS.Callback.registerProbeStateCallback(OnProbeStateCallback);
        }

        private static void OnProbeStateCallback(int val)
        {
            probe = val;
        }

        private static void OnMotorCallback(int prf_hz, int density)
        {
            Log.Information($"prf:{prf_hz}, depth:{density}");
        }

        private static void OnDeviceDetached(object? sender, EventArgs e)
        {
            HsnlibraryCS.HsnInterface.disactivateProbe();
        }

        private static void OnDeviceAttached(object? sender, EventArgs e)
        {
            HsnlibraryCS.HsnInterface.activateProbe();
        }
    }
}
