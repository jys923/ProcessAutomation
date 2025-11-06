using SonoCap.MES.Models.Inspection;
using SonoCap.MES.UI.Models;
using System.Text.Json;

namespace SonoCap.MES.UI.Services.TestStrategies
{
    public class GrayTestStrategy : BaseTestStrategy<Gray>
    {
        protected override void ExecuteTestLoop(TestContext c)
        {
            var type = (int)InspectionPartType.Gray;

            // Gray는 단일 프레임만 검사하므로 첫 번째 버퍼만 사용
            c.InspectionFunction(
                c.InputBufferPtrs[0],
                c.SnapshotImg.PixelWidth,
                c.SnapshotImg.PixelHeight,
                c.ResultBufferPtrs[0],
                c.ResultTextBufferPtrs[0],
                type
            );
        }

        protected override int GetFinalResultIndex(TestContext c)
            => 0; // 중앙 프레임

        protected override void UpdateContextMetadataAndScore(TestContext c, Gray parsed)
        {
            c.ChangedImgMetadata = JsonSerializer.Serialize(parsed);
            c.ResultScore = InspectionCalculator.CalculateGrayScore(parsed);
        }
    }

}
