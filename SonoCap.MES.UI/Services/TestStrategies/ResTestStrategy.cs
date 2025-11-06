using SonoCap.MES.Models.Inspection;
using SonoCap.MES.UI.Models;
using System.Text.Json;

namespace SonoCap.MES.UI.Services.TestStrategies
{
    public class ResTestStrategy : BaseTestStrategy<Res>
    {
        protected override void ExecuteTestLoop(TestContext c)
        {
            var type = (int)InspectionPartType.Res;
            for (int i = 0; i < c.InputBufferPtrs.Length; i++)
            {
                c.InspectionFunction(c.InputBufferPtrs[i], c.SnapshotImg.PixelWidth, c.SnapshotImg.PixelHeight,
                    c.ResultBufferPtrs[i], c.ResultTextBufferPtrs[i], type);
            }
        }

        protected override int GetFinalResultIndex(TestContext c)
        {
            // C++ SelectBestTrial 대응
            // trials == c.ResultTextArrays
            // 각 프레임마다 JSON 문자열에 Res 결과가 들어 있음
            const double targetH = 14.63;
            const double targetV = 10.97;

            int bestIndex = 0;
            double bestDistErr = double.MaxValue;
            double bestEdgeSum = double.MinValue;

            for (int i = 0; i < c.ResultTextArrays.Length; i++)
            {
                string json = System.Text.Encoding.UTF8.GetString(c.ResultTextArrays[i]).TrimEnd('\0');
                if (string.IsNullOrWhiteSpace(json))
                    continue;

                var result = System.Text.Json.JsonSerializer.Deserialize<InspectionResult>(json);
                if (result?.Res == null)
                    continue;

                var r = result.Res;
                double distErr =
                    Math.Pow(r.HorizontalDist - targetH, 2) +
                    Math.Pow(r.VerticalDist - targetV, 2);

                double edgeSum = r.EdgeDensity1 + r.EdgeDensity2 + r.EdgeDensity3;

                if (distErr < bestDistErr)
                {
                    bestIndex = i;
                    bestDistErr = distErr;
                    bestEdgeSum = edgeSum;
                }
                else if (Math.Abs(distErr - bestDistErr) < 1e-6)
                {
                    if (edgeSum > bestEdgeSum)
                    {
                        bestIndex = i;
                        bestEdgeSum = edgeSum;
                    }
                }
            }

            return bestIndex;
        }

        protected override void UpdateContextMetadataAndScore(TestContext c, Res parsed)
        {
            c.ChangedImgMetadata = JsonSerializer.Serialize(parsed);
            c.ResultScore = InspectionCalculator.CalculateResScore(parsed);
        }
    }
}
