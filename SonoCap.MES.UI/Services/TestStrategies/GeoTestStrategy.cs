using SonoCap.MES.Models.Inspection;
using SonoCap.MES.UI.Models;
using System.Text.Json;

namespace SonoCap.MES.UI.Services.TestStrategies
{
    public class GeoTestStrategy : ITestStrategy
    {
        public void Execute(TestContext context)
        {
            InspectionPartType partType = InspectionPartType.Geo;
            context.InspectionFunction(context.ImageBufferPtr, context.SnapshotImg.PixelWidth, context.SnapshotImg.PixelHeight, context.ResultBufferPtr, context.TextBufferPtr, (int)partType);

            string resultText = System.Text.Encoding.UTF8.GetString(context.TextArray).TrimEnd('\0');
            var parsed = JsonSerializer.Deserialize<InspectionResult>(resultText);
            context.ChangedImgMetadata = JsonSerializer.Serialize(parsed.Geo);
            context.ResultScore = InspectionCalculator.CalculateGeoScore(parsed.Geo);
        }
    }
}
