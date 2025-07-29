using SonoCap.MES.Models.Inspection;
using SonoCap.MES.UI.Models;
using System.Text.Json;

namespace SonoCap.MES.UI.Services.TestStrategies
{
    public class EnvGeoTestStrategy : ITestStrategy
    {
        public void Execute(TestContext context)
        {
            context.ProcessFunction(context.ImageBufferPtr, context.SnapshotImg.PixelWidth, context.SnapshotImg.PixelHeight, context.ResultBufferPtr, context.TextBufferPtr);

            string resultText = System.Text.Encoding.UTF8.GetString(context.TextArray).TrimEnd('\0');
            var parsed = JsonSerializer.Deserialize<EnvGeoMetrics>(resultText);
            context.ChangedImgMetadata = JsonSerializer.Serialize(parsed);
            context.ResultScore = EnvGeoCalculator.CalculateScore(parsed);
        }
    }
}
