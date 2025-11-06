using SonoCap.MES.Models.Inspection;
using SonoCap.MES.UI.Models;
using System;
using System.Text.Json;

namespace SonoCap.MES.UI.Services.TestStrategies
{
    public class GeoTestStrategy : BaseTestStrategy<Geo>
    {
        protected override void ExecuteTestLoop(TestContext c)
        {
            var type = (int)InspectionPartType.Geo;
            for (int i = 0; i < c.InputBufferPtrs.Length; i++)
            {
                c.InspectionFunction(c.InputBufferPtrs[i], c.SnapshotImg.PixelWidth, c.SnapshotImg.PixelHeight,
                    c.ResultBufferPtrs[i], c.ResultTextBufferPtrs[i], type);
            }
        }

        protected override int GetFinalResultIndex(TestContext c)
        {
            double maxScore = double.MinValue;
            int best = 0;

            for (int i = 0; i < c.InputBufferPtrs.Length; i++)
            {
                string json = System.Text.Encoding.UTF8.GetString(c.ResultTextArrays[i]).TrimEnd('\0');
                var result = JsonSerializer.Deserialize<InspectionResult>(json);
                double score = result?.Geo?.BrightnessContrast ?? 0;
                if (score > maxScore) { maxScore = score; best = i; }
            }

            return best;
        }

        protected override void UpdateContextMetadataAndScore(TestContext c, Geo parsed)
        {
            c.ChangedImgMetadata = JsonSerializer.Serialize(parsed);
            c.ResultScore = InspectionCalculator.CalculateGeoScore(parsed);
        }
    }
}
