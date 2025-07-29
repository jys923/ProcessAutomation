using SonoCap.MES.Models.Inspection;
using SonoCap.MES.UI.Models;
using System.Text.Json;

namespace SonoCap.MES.UI.Services.TestStrategies
{
    public class GrayTestStrategy : ITestStrategy
    {
        // 생성자에서 아무것도 받지 않습니다.
        public void Execute(TestContext context)
        {
            InspectionPartType partType = InspectionPartType.Gray;
            // 컨텍스트를 통해 함수에 접근
            context.InspectionFunction(context.ImageBufferPtr, context.SnapshotImg.PixelWidth, context.SnapshotImg.PixelHeight, context.ResultBufferPtr, context.TextBufferPtr, (int)partType);

            string resultText = System.Text.Encoding.UTF8.GetString(context.TextArray).TrimEnd('\0');
            var parsed = JsonSerializer.Deserialize<InspectionResult>(resultText);
            context.ChangedImgMetadata = JsonSerializer.Serialize(parsed.Gray);
            context.ResultScore = InspectionCalculator.CalculateGrayScore(parsed.Gray);
        }
    }
}
