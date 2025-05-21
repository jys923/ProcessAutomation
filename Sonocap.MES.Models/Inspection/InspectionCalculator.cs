using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoCap.MES.Models.Inspection
{
    public class InspectionCalculator
    {
        private static readonly Gray grayThreshold = new Gray
        {
            Mean1 = 125.0,
            Mean2 = 150.0,
            Mean3 = 175.0
        };
        public static bool IsGrayPass(Gray score)
        {
            bool p1 = Math.Abs(score.Mean1 - grayThreshold.Mean1) <= 10;
            bool p2 = Math.Abs(score.Mean2 - grayThreshold.Mean2) <= 10;
            bool p3 = Math.Abs(score.Mean3 - grayThreshold.Mean3) <= 10;

            int passCount = new[] { p1, p2, p3 }.Count(p => p);

            return passCount >= 2;
        }

        public static int CalculateResScore(double edge1, double edge2, double edge3, double hDist, double vDist)
        {
            // 점수 정책 미정 - 임시 0 출력
            return 0;
        }

        public static int CalculateGeoScore(double meanBrightness, double stdBrightness)
        {
            // 점수 정책 미정 - 임시 0 출력
            return 0;
        }

        // 예시: 최종 점수 집계
        public static void AnalyzeSample()
        {
            //double mean1 = 123.4, mean2 = 127.8, mean3 = 125.1;

            //int grayScore = CalculateGrayScore(mean1, mean2, mean3);

            //Console.WriteLine($"GrayScore (packed): {grayScore}");

            //// 분해 출력
            //int g1 = grayScore / 10000;
            //int g2 = (grayScore / 100) % 100;
            //int g3 = grayScore % 100;

            //Console.WriteLine($"  mean1: {g1}");
            //Console.WriteLine($"  mean2: {g2}");
            //Console.WriteLine($"  mean3: {g3}");
        }
    }
}
