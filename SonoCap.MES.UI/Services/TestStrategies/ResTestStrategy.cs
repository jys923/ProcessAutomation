using SonoCap.MES.Models.Inspection;
using SonoCap.MES.UI.Models;
using System.Text.Json;

namespace SonoCap.MES.UI.Services.TestStrategies
{
    public class ResTestStrategy : ITestStrategy
    {
        public void Execute(TestContext context)
        {
            InspectionPartType partType = InspectionPartType.Res;
            // 컨텍스트를 통해 함수에 접근
            context.InspectionFunction(context.ImageBufferPtr, context.SnapshotImg.PixelWidth, context.SnapshotImg.PixelHeight, context.ResultBufferPtr, context.TextBufferPtr, (int)partType);

            string resultText = System.Text.Encoding.UTF8.GetString(context.TextArray).TrimEnd('\0');
            var parsed = JsonSerializer.Deserialize<InspectionResult>(resultText);
            context.ChangedImgMetadata = JsonSerializer.Serialize(parsed.Res);
            context.ResultScore = InspectionCalculator.CalculateResScore(parsed.Res);
        }
    }
}
