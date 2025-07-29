using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SonoCap.MES.Models.Inspection
{
    public class MesPoint
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }
    }

    public class MadMetrics
    {
        [JsonPropertyName("filtered_out_by_x")]
        public List<MesPoint>? FilteredOutByX { get; set; }

        [JsonPropertyName("mad_x")]
        public double? MadX { get; set; }

        [JsonPropertyName("median_x")]
        public double? MedianX { get; set; }

        [JsonPropertyName("x_tolerance")]
        public double? XTolerance { get; set; }
    }

    public class YIntervalMetrics
    {
        [JsonPropertyName("best_ref_point")]
        public MesPoint? BestRefPoint { get; set; }

        [JsonPropertyName("filtered_out_by_y")]
        public List<MesPoint>? FilteredOutByY { get; set; }

        [JsonPropertyName("target_y_interval")]
        public double? TargetYInterval { get; set; }

        [JsonPropertyName("y_tolerance")]
        public double? YTolerance { get; set; }
    }

    public class EnvGeoMetrics
    {
        [JsonPropertyName("finalPoints")]
        public List<MesPoint>? FinalPoints { get; set; }

        [JsonPropertyName("findPoints")]
        public List<MesPoint>? FindPoints { get; set; }

        [JsonPropertyName("madMetrics")]
        public MadMetrics? MadMetrics { get; set; }

        [JsonPropertyName("yIntervalMetrics")]
        public YIntervalMetrics? YIntervalMetrics { get; set; }
    }
}