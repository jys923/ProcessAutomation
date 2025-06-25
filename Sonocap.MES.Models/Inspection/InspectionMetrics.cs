// 파일명: InspectionMetrics.cs
using System.Text.Json.Serialization;

namespace SonoCap.MES.Models.Inspection
{
    [Flags]
    public enum InspectionPartType
    {
        None = 0,
        Geo = 1 << 0,
        Gray = 1 << 1,
        Res = 1 << 2,
        All = Geo | Gray | Res
    }

    public class Gray
    {
        [JsonPropertyName("mean1")]
        public double Mean1 { get; set; } = -1;

        [JsonPropertyName("mean2")]
        public double Mean2 { get; set; } = -1;

        [JsonPropertyName("mean3")]
        public double Mean3 { get; set; } = -1;
    }

    public class Res
    {
        [JsonPropertyName("edgeDensity1")]
        public double EdgeDensity1 { get; set; } = -1;

        [JsonPropertyName("edgeDensity2")]
        public double EdgeDensity2 { get; set; } = -1;

        [JsonPropertyName("edgeDensity3")]
        public double EdgeDensity3 { get; set; } = -1;

        [JsonPropertyName("horizontalDist")]
        public double HorizontalDist { get; set; } = -1;

        [JsonPropertyName("verticalDist")]
        public double VerticalDist { get; set; } = -1;
    }

    public class Geo
    {
        [JsonPropertyName("meanBrightness")]
        public double MeanBrightness { get; set; } = -1;

        [JsonPropertyName("stdBrightness")]
        public double StdBrightness { get; set; } = -1;

        [JsonPropertyName("brightnessContrast")]
        public double BrightnessContrast { get; set; } = -1;

        [JsonPropertyName("maxSliceMean")]
        public double MaxSliceMean { get; set; } = -1;

        [JsonPropertyName("maxSliceVariance")]
        public double MaxSliceVariance { get; set; } = -1;
    }


    public class InspectionResult
    {
        [JsonPropertyName("Gray")]
        public Gray Gray { get; set; } = new();

        [JsonPropertyName("Res")]
        public Res Res { get; set; } = new();

        [JsonPropertyName("Geo")]
        public Geo Geo { get; set; } = new();
    }
}
