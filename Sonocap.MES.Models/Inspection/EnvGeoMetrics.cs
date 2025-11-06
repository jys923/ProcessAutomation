using System.Text.Json.Serialization;

namespace SonoCap.MES.Models.Inspection
{
    public class MesPoint
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }
    }

    public class AxisFilter
    {
        [JsonPropertyName("interval")]
        public double Interval { get; set; }   // Y: target interval / X: 0

        [JsonPropertyName("tolerance")]
        public double Tolerance { get; set; }  // 공통 허용 오차

        [JsonPropertyName("ref_point")]
        public MesPoint? RefPoint { get; set; } // X: (bestK,0) / Y: 기준점

        [JsonPropertyName("filtered_out")]
        public List<MesPoint>? FilteredOut { get; set; } // 제외된 점들
    }

    public class EnvGeoMetrics
    {
        [JsonPropertyName("finalPoints")]
        public List<MesPoint>? FinalPoints { get; set; }

        [JsonPropertyName("findPoints")]
        public List<MesPoint>? FindPoints { get; set; }

        [JsonPropertyName("xFilter")]
        public AxisFilter? XFilter { get; set; }

        [JsonPropertyName("yFilter")]
        public AxisFilter? YFilter { get; set; }
    }
}