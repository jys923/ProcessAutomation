// 파일명: InspectionCalculator.cs

namespace SonoCap.MES.Models.Inspection
{
    public class InspectionCalculator
    {
        // 기준값 외부 주입
        public static Gray GrayThreshold { get; set; } = new Gray { Mean1 = 125.0, Mean2 = 150.0, Mean3 = 175.0 };
        public static Geo GeoThreshold { get; set; } = new Geo
        {
            MeanBrightness = 180.0,
            StdBrightness = 15.0,
            BrightnessContrast = 0.25,
            MaxSliceMean = 200.0,
            MaxSliceVariance = 500.0   // 최대 허용 분산
        };
        public static Res ResThreshold { get; set; } = new Res { HorizontalDist = 20.0, VerticalDist = 20.0 };

        // 통과 기준 점수 (DB에서 외부 주입 가정)
        public static int PassThresholdGray { get; set; } = 70;
        public static int PassThresholdGeo { get; set; } = 70;
        public static int PassThresholdRes { get; set; } = 70;

        // Gray 검사 점수
        public static int CalculateGrayScore(Gray score)
        {
            double s1 = ScoreByDeviation(score.Mean1, GrayThreshold.Mean1, 20);
            double s2 = ScoreByDeviation(score.Mean2, GrayThreshold.Mean2, 20);
            double s3 = ScoreByDeviation(score.Mean3, GrayThreshold.Mean3, 20);

            return (int)Math.Round((s1 + s2 + s3) / 3.0);
        }

        public static bool IsGrayPass(Gray score) =>
            CalculateGrayScore(score) >= PassThresholdGray;

        // Geo 검사 점수

        public static int CalculateGeoScore(Geo score)
        {
            double s1 = ScoreByDeviation(score.MeanBrightness, GeoThreshold.MeanBrightness, 30) * 0.1;
            double s2 = ScoreByDeviation(score.StdBrightness, GeoThreshold.StdBrightness, 20) * 0.15;
            double s3 = ScoreByDeviation(score.BrightnessContrast, GeoThreshold.BrightnessContrast, 0.1) * 0.15;

            double s4 = score.MaxSliceMean <= GeoThreshold.MaxSliceMean
                ? 100.0
                : Math.Max(0.0, 100.0 * (1.0 - (score.MaxSliceMean - GeoThreshold.MaxSliceMean) / 50.0));
            s4 *= 0.1;

            double s5 = Math.Max(0.0, 100.0 * (1.0 - score.MaxSliceVariance / (GeoThreshold.MaxSliceVariance * 1.5)));
            s5 *= 0.5;

            return (int)Math.Round(s1 + s2 + s3 + s4 + s5);
        }

        public static bool IsGeoPass(Geo score) =>
            CalculateGeoScore(score) >= PassThresholdGeo;

        // Res 검사 점수
        public static int CalculateResScore(Res score)
        {
            double s1 = ScoreByDeviation(score.HorizontalDist, ResThreshold.HorizontalDist, 10);
            double s2 = ScoreByDeviation(score.VerticalDist, ResThreshold.VerticalDist, 10);

            return (int)Math.Round((s1 + s2) / 2.0);
        }

        public static bool IsResPass(Res score) =>
            CalculateResScore(score) >= PassThresholdRes;

        // 오차 기반 점수 (0~100), 최대 허용 오차 = maxDeviation
        private static double ScoreByDeviation(double actual, double expected, double maxDeviation)
        {
            double diff = Math.Abs(actual - expected);
            return Math.Max(0.0, 100.0 * (1.0 - diff / maxDeviation));
        }
    }
}
