using SonoCap.MES.Models.Inspection;
using SonoCap.MES.UI.Models;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;

namespace SonoCap.MES.UI.Services.TestStrategies
{
    public abstract class BaseTestStrategy<TMetrics> : ITestStrategy
    {
        public void Execute(TestContext context)
        {
            // 1️⃣ 공통 실행 순서: (은닉형 템플릿 구조)
            ExecuteTestLoop(context);                      // 각 전략별 DLL 호출
            int finalIndex = GetFinalResultIndex(context); // 결과 선택 (전략별)
            FinalizeImageResults(context, finalIndex);     // 공통 이미지 포장
            UpdateFinalTextResult(context, finalIndex);    // 공통 텍스트 파싱 및 점수 반영
        }

        // -----------------------------
        // 🔸 추상 메서드 (전략별 구현)
        // -----------------------------
        protected abstract void ExecuteTestLoop(TestContext context);
        protected abstract int GetFinalResultIndex(TestContext context);
        protected abstract void UpdateContextMetadataAndScore(TestContext context, TMetrics parsedMetrics);

        // -----------------------------
        // 🔹 공통 로직
        // -----------------------------
        protected virtual void FinalizeImageResults(TestContext context, int finalIndex)
        {
            context.SnapshotImg = BitmapSource.Create(
                context.SnapshotImg.PixelWidth, context.SnapshotImg.PixelHeight, 96, 96,
                PixelFormats.Bgr32, null,
                context.InputSnapshots[finalIndex], context.SnapshotImg.PixelWidth * 4
            );

            context.ResultImg = BitmapSource.Create(
                context.SnapshotImg.PixelWidth, context.SnapshotImg.PixelHeight, 96, 96,
                PixelFormats.Bgr32, null,
                context.ResultSnapshots[finalIndex], context.SnapshotImg.PixelWidth * 4
            );
        }

        private void UpdateFinalTextResult(TestContext context, int finalIndex)
        {
            context.TextArray = context.ResultTextArrays[finalIndex];
            context.ResultText = System.Text.Encoding.UTF8.GetString(context.TextArray).TrimEnd('\0');

            // EnvGeoMetrics는 독립 구조
            if (typeof(TMetrics) == typeof(EnvGeoMetrics))
            {
                var parsed = JsonSerializer.Deserialize<TMetrics>(context.ResultText);
                UpdateContextMetadataAndScore(context, parsed!);
            }
            // Geo/Gray/Res는 InspectionResult 내부 Metrics 사용
            else
            {
                var parsedInspection = JsonSerializer.Deserialize<InspectionResult>(context.ResultText);
                var property = typeof(InspectionResult).GetProperty(typeof(TMetrics).Name);
                if (property != null)
                {
                    var parsedMetrics = (TMetrics)property.GetValue(parsedInspection)!;
                    UpdateContextMetadataAndScore(context, parsedMetrics);
                }
            }
        }
    }
}
