using System.Text.Json;

namespace SonoCap.MES.Models
{
    public class HansonoSettings
    {
        public int density { get; set; }
        public double depth_in_cm { get; set; }
        public double dr_max { get; set; }
        public double dr_min { get; set; }
        public bool frameaverage_enable { get; set; }
        public double frameaverage_weight { get; set; }
        public int gain { get; set; }
        public double image_offset_x_in_pixel { get; set; }
        public double image_offset_y_in_pixel { get; set; }
        public int probe_frame_index { get; set; }
        public int rawdata_byte_per_sample { get; set; }
        public int rawdata_sample_no { get; set; }
        public int rawdata_scanline_no { get; set; }
        public double tx_power { get; set; }
        public double unit_mm_per_pixel { get; set; }

        public string ToJson()
        {
            try
            {
                return JsonSerializer.Serialize(this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting to JSON: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
