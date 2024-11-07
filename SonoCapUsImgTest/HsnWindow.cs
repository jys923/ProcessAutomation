using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using Serilog;
using System.Runtime.InteropServices;
using System.Text;

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
        static int width = 1920; // 예시 값
        static int height = 1080; // 예시 값
        static int buffer_size = width * height * 4;
        static byte[] buffer = new byte[buffer_size];

        // GCHandle로 고정하여 IntPtr로 변환
        static GCHandle finalImageHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        static IntPtr finalImagePtr = finalImageHandle.AddrOfPinnedObject();

        static GCHandle rawDataHandle = GCHandle.Alloc(envdata_buffer, GCHandleType.Pinned);
        static IntPtr rawDataPtr = rawDataHandle.AddrOfPinnedObject();

        // 메타데이터 버퍼 준비
        static StringBuilder outputMetadata = new StringBuilder(10240);

        //// 함수 호출
        //UIntPtr result = IpRenderWithCapture(
        //    finalImagePtr,
        //    (UIntPtr)buffer.Length,
        //    rawDataPtr,
        //    (UIntPtr)envdata_buffer.Length,
        //    outputMetadata
        //);

        //// 결과 확인
        //if (result != UIntPtr.Zero)
        //{
        //    Console.WriteLine("메서드 호출 성공!");
        //    // 추가 처리 코드
        //}
        //else
        //{
        //    Console.WriteLine("메서드 호출 실패.");
        //}

        //// GCHandle 해제
        //finalImageHandle.Free();
        //rawDataHandle.Free();


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
            //UIntPtr result = SonoCapUsImgService.IpRenderWithCapture(
            //    finalImagePtr,
            //    (UIntPtr)buffer.Length,
            //    rawDataPtr,
            //    (UIntPtr)envdata_buffer.Length,
            //    outputMetadata
            //);

            //if (result != UIntPtr.Zero)
            //{
            //    Log.Information("메서드 호출 성공!");
            //    // 추가 처리 코드
            //}
            //else
            //{
            //    Log.Information("메서드 호출 실패.");
            //}
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

        private static System.Timers.Timer timer;

        private static void StartTimer()
        {
            if (timer != null) return;

            timer = new System.Timers.Timer(1000); // 100ms 간격으로 설정하여 1초에 10회 호출
            timer.Elapsed += (s, e) =>
            {
                //Log.Information($"Sender: {s}");
                Log.Information($"Timer elapsed event triggered at {e.SignalTime}");
                //SonoCapUsImgService.HsnBufferCreatorGetBitmapBuffer();
                UIntPtr result = SonoCapUsImgService.IpRenderWithCapture(
                    finalImagePtr,
                    (UIntPtr)buffer.Length,
                    rawDataPtr,
                    (UIntPtr)envdata_buffer.Length,
                    outputMetadata
                );

                if (result != UIntPtr.Zero)
                {
                    Log.Information("메서드 호출 성공!");
                    // 추가 처리 코드
                }
                else
                {
                    Log.Information("메서드 호출 실패.");
                }
            };
            timer.AutoReset = true; // 타이머가 주기적으로 실행되도록 설정
            timer.Start();
        }

        private static void Init()
        {
            registerCallbackBeforeInitialize();

            if (!SonoCapUsImgService.Initialize())
            {
                Log.Information("initialize Fail");
                return;
            }

            registerCallbackAfterInitialize();

            try
            {
                SonoCapUsImgService.StartProbeDetection();
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}");
                throw;
            }

            //SonoCapUsImgService.HsnBufferCreatorInitialize();

            //SonoCapUsImgService.HsnBufferCreatorGetBitmapBuffer();

            if (!SonoCapUsImgService.IpInitialize())
            {
                Log.Information("ipInitialize Fail");
                return;
            }

            int width = 512;
            int height = 512;
            if (!SonoCapUsImgService.IpResize(width, height))
            {
                //exception
                Log.Information("ipResize Fail");
                return;
            }
        }

        static int probe = 0;

        private static void registerCallbackBeforeInitialize()
        {
            Loading loading = (bool value) =>
            {
                Log.Information($"Loading callback executed! {value}");
                if (!value && probe == 5)
                {
                    StartTimer();
                }
            };

            Error error = (string message, int value) =>
            {
                Log.Error(message, value);
            };

            bool result = SonoCapUsImgService.RegisterCallbackLoading(loading); //loading_status
            result = SonoCapUsImgService.RegisterCallbackError(error); //error
        }

        private static void mLoadingCallback(bool value)
        {
            Log.Information($"Loading {value}");
        }

        private static void myErrorCallback(string value, int value2)
        {
            Log.Error($"Error {value} {value2}");
        }

        private static void registerCallbackAfterInitialize()
        {
            DeviceAttached deviceAttached = () =>
            {
                Log.Information("DeviceAttached callback executed!");
                SonoCapUsImgService.ActivateProbe();
            };

            DeviceRemoved deviceRemoved = () =>
            {
                Log.Information("DeviceRemoved callback executed!");
                SonoCapUsImgService.DeactivateProbe();
            };

            MotorSpeed motorSpeed = (int value, int value2) =>
            {
                Log.Information($"{nameof(motorSpeed)}: prf:{value}, depth:{value2}");
            };

            ProbeState probeState = (int value) =>
            {
                //Log.Information($"{nameof(ProbeState)}:{value}");
                Log.Information($"ProbeState:{value}");
                probe = value;
            };

            SonoCapUsImgService.RegisterCallbackDeviceAttached(deviceAttached);
            SonoCapUsImgService.RegisterCallbackDeviceRemoved(deviceRemoved);
            SonoCapUsImgService.RegisterCallbackMotorSpeed(motorSpeed);
            SonoCapUsImgService.RegisterCallbackProbeState(probeState);
        }

        private static void DeviceAttachedCallback()
        {
            SonoCapUsImgService.ActivateProbe();
        }

        private static void mDeviceAttachedCallback()
        {
            SonoCapUsImgService.ActivateProbe();
        }

        private static void mDeviceRemovedCallback()
        {
            SonoCapUsImgService.DeactivateProbe();
        }

        private static void mMotorSpeedCallback(int value, int value2)
        {
            Log.Information($"MotorSpeed prf_hz : {value}, density : {value2}");
        }

        private static void mProbeStateCallback(int value)
        {
            Log.Information($"ProbeState {value}");
        }
    }
}
