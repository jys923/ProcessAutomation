using System.Runtime.InteropServices;

namespace FunctionsDllTest
{
    class Program
    {
        [DllImport("FunctionsDll.dll")]
        public static extern double ForTest(int length);

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var cppResult = ForTest(100000000);
            Console.WriteLine("CPP 결과 : " + cppResult + "ms");

            var csResult = CSharpForTest(100000000);
            Console.WriteLine("CS 결과 : " + csResult + "ms");
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
    }
}
