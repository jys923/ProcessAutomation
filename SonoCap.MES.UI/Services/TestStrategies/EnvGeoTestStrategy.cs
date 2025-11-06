using SonoCap.MES.Models.Inspection;
using SonoCap.MES.UI.Models;
using System;
using System.Text.Json;

namespace SonoCap.MES.UI.Services.TestStrategies
{
    public class EnvGeoTestStrategy : BaseTestStrategy<EnvGeoMetrics>
    {
        protected override void ExecuteTestLoop(TestContext c)
        {
            // EnvGeo는 ProcessFunction 사용
            for (int i = 0; i < c.InputBufferPtrs.Length; i++)
            {
                c.ProcessFunction(
                    c.InputBufferPtrs[i],
                    c.SnapshotImg.PixelWidth,
                    c.SnapshotImg.PixelHeight,
                    c.ResultBufferPtrs[i],
                    c.ResultTextBufferPtrs[i]
                );
            }
        }
        protected override int GetFinalResultIndex(TestContext c)
        {
            int bestIndex = 0;

            for (int i = 1; i < c.ResultTextArrays.Length; i++)
            {
                string jsonBest = System.Text.Encoding.UTF8.GetString(c.ResultTextArrays[bestIndex]).TrimEnd('\0');
                string jsonCurr = System.Text.Encoding.UTF8.GetString(c.ResultTextArrays[i]).TrimEnd('\0');

                if (string.IsNullOrWhiteSpace(jsonCurr))
                    continue;

                var best = System.Text.Json.JsonSerializer.Deserialize<EnvGeoMetrics>(jsonBest);
                var curr = System.Text.Json.JsonSerializer.Deserialize<EnvGeoMetrics>(jsonCurr);

                if (curr == null || best == null)
                    continue;

                int currCount = curr.FinalPoints?.Count ?? 0;
                int bestCount = best.FinalPoints?.Count ?? 0;

                // ① finalPoints가 많은 Trial 우선
                if (currCount > bestCount)
                {
                    bestIndex = i;
                    continue;
                }

                // ② finalPoints 수가 같으면 Y축 Phase Error 비교
                if (currCount == bestCount)
                {
                    double bestPhaseErrY = ComputePhaseError(best, AxisMode.Y);
                    double currPhaseErrY = ComputePhaseError(curr, AxisMode.Y);

                    if (currPhaseErrY < bestPhaseErrY)
                    {
                        bestIndex = i;
                        continue;
                    }

                    // ③ Y오차가 거의 같으면 X축 Phase Error로 tie-break
                    if (Math.Abs(currPhaseErrY - bestPhaseErrY) < 1e-6)
                    {
                        double bestPhaseErrX = ComputePhaseError(best, AxisMode.X);
                        double currPhaseErrX = ComputePhaseError(curr, AxisMode.X);

                        if (currPhaseErrX < bestPhaseErrX)
                        {
                            bestIndex = i;
                            continue;
                        }
                    }
                }
            }

            return bestIndex;
        }


        // 동일 파일 내 보조 함수
        private enum AxisMode { X, Y }

        private double ComputePhaseError(EnvGeoMetrics m, AxisMode mode)
        {
            if (m.FinalPoints == null || m.FinalPoints.Count == 0)
                return double.MaxValue;

            double refCoord = 0;
            double interval = 0;

            if (mode == AxisMode.X)
            {
                refCoord = m.XFilter.RefPoint.X;
                interval = m.XFilter.Interval;   // 일반적으로 0 또는 1
            }
            else
            {
                refCoord = m.YFilter.RefPoint.Y;
                interval = m.YFilter.Interval;
            }

            if (interval <= 0)
                return double.MaxValue;

            double sumErr = 0;
            foreach (var p in m.FinalPoints)
            {
                double coord = (mode == AxisMode.X) ? p.X : p.Y;
                double diff = Math.Abs(coord - refCoord);
                double rem = diff % interval;
                double err = Math.Min(rem, interval - rem);
                sumErr += err;
            }

            return sumErr / m.FinalPoints.Count;
        }



        protected override void UpdateContextMetadataAndScore(TestContext c, EnvGeoMetrics parsed)
        {
            // EnvGeo 전용 점수 계산기 사용
            //c.ChangedImgMetadata = $"Cnt:{parsed.FinalPoints.Count} {JsonSerializer.Serialize(parsed)}";
            c.ChangedImgMetadata = JsonSerializer.Serialize(parsed);
            c.ResultScore = EnvGeoCalculator.CalculateScore(parsed);
        }
    }
}
