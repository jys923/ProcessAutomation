using System.Text.Json.Serialization;

namespace SonoCap.MES.Models.Process
{
    public class SharpnessMetrics
    {
        [JsonPropertyName("Tenengrad")]
        public double? Tenengrad { get; set; }

        [JsonPropertyName("Laplacian")]
        public double? Laplacian { get; set; }
    }

    public class QualityMetrics
    {
        [JsonPropertyName("Sharpness")]
        public SharpnessMetrics? Sharpness { get; set; } = new SharpnessMetrics();

        [JsonPropertyName("Brightness")]
        public double? Brightness { get; set; }

        [JsonPropertyName("Contrast")]
        public double? Contrast { get; set; }

        [JsonPropertyName("SNR")]
        public double? SNR { get; set; }

        [JsonPropertyName("SpeckleIndex")]
        public double? SpeckleIndex { get; set; }

        [JsonPropertyName("Entropy")]
        public double? Entropy { get; set; }

        [JsonPropertyName("EdgeDensity")]
        public double? EdgeDensity { get; set; }

        [JsonPropertyName("LocalVariance")]
        public double? LocalVariance { get; set; }

        [JsonPropertyName("CNR")]
        public double? CNR { get; set; }
    }

    public class QualityMetricsRoot
    {
        [JsonPropertyName("image")]
        public string ImageName { get; set; } = string.Empty;

        [JsonPropertyName("analysis")]
        public QualityMetrics Analysis { get; set; } = new QualityMetrics();
    }
}
