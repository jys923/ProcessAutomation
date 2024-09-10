using System.Text.Json;

namespace SonoCap.MES.Models
{
    public class HansonoSettings
    {
        public HansonoSettings(int density, double depth_in_cm, double dr_max, double dr_min, bool frameaverage_enable, double frameaverage_weight, int gain, double image_offset_x_in_pixel, double image_offset_y_in_pixel, int probe_frame_index, int rawdata_byte_per_sample, int rawdata_sample_no, int rawdata_scanline_no, double tx_power, double unit_mm_per_pixel)
        {
            this.density = density;
            this.depth_in_cm = depth_in_cm;
            this.dr_max = dr_max;
            this.dr_min = dr_min;
            this.frameaverage_enable = frameaverage_enable;
            this.frameaverage_weight = frameaverage_weight;
            this.gain = gain;
            this.image_offset_x_in_pixel = image_offset_x_in_pixel;
            this.image_offset_y_in_pixel = image_offset_y_in_pixel;
            this.probe_frame_index = probe_frame_index;
            this.rawdata_byte_per_sample = rawdata_byte_per_sample;
            this.rawdata_sample_no = rawdata_sample_no;
            this.rawdata_scanline_no = rawdata_scanline_no;
            this.tx_power = tx_power;
            this.unit_mm_per_pixel = unit_mm_per_pixel;
        }

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
    }
}
