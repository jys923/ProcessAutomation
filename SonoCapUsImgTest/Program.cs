using Serilog;
using SonoCap.Commons;
using System.Runtime.InteropServices;
using System.Timers;

namespace SonoCapUsImgTest
{
    class Program
    {
        [DllImport("SonoCapUsImgD.dll")]
        public static extern double ForTest(int length);

        static void Main(string[] args)
        {
            LoggingConfigurator.Configure();

            Console.WriteLine("Hello, World!");
            //var cppResult = ForTest(100000000);
            //Console.WriteLine("CPP 결과 : " + cppResult + "ms");

            //var csResult = CSharpForTest(100000000);
            //Console.WriteLine("CS 결과 : " + csResult + "ms");

            Init();

            while (true) {

            }
        }

        public static double CSharpForTest(int length)
        {
            DateTime startTime = DateTime.Now; // 시작 시간 기록
            int j = 0;
            for (int i = 0; i < length; i++)
            {
                j++;
            }
            DateTime endTime = DateTime.Now; // 종료 시간 기록
            
            TimeSpan elapsedTime = endTime - startTime; // 경과 시간 계산

            double elapsedMilliseconds = elapsedTime.TotalMilliseconds; // 경과 시간을 밀리초로 얻음

            return elapsedMilliseconds;
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
                SonoCapUsImgService.HsnBufferCreatorGetBitmapBuffer();
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

            SonoCapUsImgService.HsnBufferCreatorInitialize();

            //SonoCapUsImgService.HsnBufferCreatorGetBitmapBuffer();

            //if (!SonoCapUsImgService.IpInitialize())
            //{
            //    Log.Information("ipInitialize Fail");
            //    return;
            //}

            //int width = 512;
            //int height = 512;
            //if (!SonoCapUsImgService.IpResize(width, height))
            //{
            //    //exception
            //    Log.Information("ipResize Fail");
            //    return;
            //}
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
                Log.Information($"{nameof(ProbeState)}:{value}");
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
