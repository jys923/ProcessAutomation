using System.Text.Json;
using System.Text.Json.Serialization;

namespace SonoCap.MES.Models.Process
{
    public static class QualityResultManager
    {
        public static void MergeAndSave(QualityMetricsRoot newResult, string resultJsonPath)
        {
            QualityMetricsRoot existingResult = LoadOrCreate(resultJsonPath);

            if (!string.IsNullOrEmpty(existingResult.ImageName) &&
                existingResult.ImageName != newResult.ImageName)
            {
                // 기존 파일과 다른 이미지다 → 덮어쓰기
                Save(newResult, resultJsonPath);
            }
            else
            {
                // 같은 이미지다 → Merge
                existingResult.ImageName = newResult.ImageName;
                Merge(existingResult.Analysis, newResult.Analysis);
                Save(existingResult, resultJsonPath);
            }
        }

        private static QualityMetricsRoot LoadOrCreate(string path)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<QualityMetricsRoot>(json) ?? new QualityMetricsRoot();
            }
            else
            {
                return new QualityMetricsRoot();
            }
        }

        private static void Save(QualityMetricsRoot data, string path)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull  // ★ 추가
            };

            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(path, json);
        }


        private static void Merge(QualityMetrics target, QualityMetrics source)
        {
            if (source.Sharpness != null)
            {
                if (target.Sharpness == null)
                    target.Sharpness = new SharpnessMetrics();

                if (source.Sharpness.Tenengrad.HasValue)
                    target.Sharpness.Tenengrad = source.Sharpness.Tenengrad;
                if (source.Sharpness.Laplacian.HasValue)
                    target.Sharpness.Laplacian = source.Sharpness.Laplacian;
            }

            if (source.Brightness.HasValue)
                target.Brightness = source.Brightness;
            if (source.Contrast.HasValue)
                target.Contrast = source.Contrast;
            if (source.SNR.HasValue)
                target.SNR = source.SNR;
            if (source.SpeckleIndex.HasValue)
                target.SpeckleIndex = source.SpeckleIndex;
            if (source.Entropy.HasValue)
                target.Entropy = source.Entropy;
            if (source.EdgeDensity.HasValue)
                target.EdgeDensity = source.EdgeDensity;
            if (source.LocalVariance.HasValue)
                target.LocalVariance = source.LocalVariance;
            if (source.CNR.HasValue)
                target.CNR = source.CNR;
        }

        private static void Merge(QualityMetricsRoot baseRoot, QualityMetricsRoot newRoot)
        {
            if (newRoot.Analysis.Sharpness != null)
            {
                if (baseRoot.Analysis.Sharpness == null)
                    baseRoot.Analysis.Sharpness = new SharpnessMetrics();

                if (newRoot.Analysis.Sharpness.Tenengrad != -1)
                    baseRoot.Analysis.Sharpness.Tenengrad = newRoot.Analysis.Sharpness.Tenengrad;

                if (newRoot.Analysis.Sharpness.Laplacian != -1)
                    baseRoot.Analysis.Sharpness.Laplacian = newRoot.Analysis.Sharpness.Laplacian;
            }

            if (newRoot.Analysis.Brightness != -1)
                baseRoot.Analysis.Brightness = newRoot.Analysis.Brightness;
            if (newRoot.Analysis.Contrast != -1)
                baseRoot.Analysis.Contrast = newRoot.Analysis.Contrast;
            if (newRoot.Analysis.SNR != -1)
                baseRoot.Analysis.SNR = newRoot.Analysis.SNR;
            if (newRoot.Analysis.SpeckleIndex != -1)
                baseRoot.Analysis.SpeckleIndex = newRoot.Analysis.SpeckleIndex;
            if (newRoot.Analysis.Entropy != -1)
                baseRoot.Analysis.Entropy = newRoot.Analysis.Entropy;
            if (newRoot.Analysis.EdgeDensity != -1)
                baseRoot.Analysis.EdgeDensity = newRoot.Analysis.EdgeDensity;
            if (newRoot.Analysis.LocalVariance != -1)
                baseRoot.Analysis.LocalVariance = newRoot.Analysis.LocalVariance;
            if (newRoot.Analysis.CNR != -1)
                baseRoot.Analysis.CNR = newRoot.Analysis.CNR;
        }
    }
}
