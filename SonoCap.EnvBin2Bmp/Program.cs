using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SonoCap.EnvBin2Bmp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: EnvBin2Bmp <binPath> <width> <height>");
                return;
            }

            string binPath = args[0];
            int width = int.Parse(args[1]);
            int height = int.Parse(args[2]);

            //string binPath = "20251106_183959_envRec.bin";
            //int width = 512;
            //int height = 960;


            if (!File.Exists(binPath))
            {
                Console.WriteLine($"File not found: {binPath}");
                return;
            }

            string folder = Path.Combine(
                Path.GetDirectoryName(binPath)!,
                Path.GetFileNameWithoutExtension(binPath)
            );
            Directory.CreateDirectory(folder);

            byte[] allData = File.ReadAllBytes(binPath);
            int frameSize = width * height * 2;
            int totalFrames = allData.Length / frameSize;
            Console.WriteLine($"[INFO] Total frames = {totalFrames}");

            for (int i = 0; i < totalFrames; i++)
            {
                byte[] frame = new byte[frameSize];
                Buffer.BlockCopy(allData, i * frameSize, frame, 0, frameSize);

                byte[] processed = CreateNormalizedBitmapDataFromRaw2Byte(frame, width, height, 0, 100);

                using (var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                {
                    var bmpData = bmp.LockBits(
                        new Rectangle(0, 0, width, height),
                        ImageLockMode.WriteOnly,
                        bmp.PixelFormat);

                    Marshal.Copy(processed, 0, bmpData.Scan0, processed.Length);
                    bmp.UnlockBits(bmpData);

                    string outPath = Path.Combine(folder, $"{i + 1:D6}.bmp");
                    bmp.Save(outPath, ImageFormat.Bmp);

                    if ((i + 1) % 10 == 0)
                        Console.WriteLine($"[{i + 1}/{totalFrames}] saved {outPath}");
                }
            }

            Console.WriteLine($"[DONE] Saved {totalFrames} frames to {folder}");
        }

        // ✅ MES용 변환 로직 그대로 포함
        static byte[] CreateNormalizedBitmapDataFromRaw2Byte(
            byte[] rawUint8Buffer,
            int width,
            int height,
            double drMin,
            double drMax)
        {
            if (rawUint8Buffer == null || width <= 0 || height <= 0)
                return Array.Empty<byte>();

            ushort[] ushortBuffer = new ushort[rawUint8Buffer.Length / 2];
            Buffer.BlockCopy(rawUint8Buffer, 0, ushortBuffer, 0, rawUint8Buffer.Length);

            double absolute_max_db = Math.Log10(Math.Sqrt(2.0) * 32768.0) * 20.0;
            const double ln10_inv_mul20 = 8.685890;
            double dr_min_db = absolute_max_db * drMin / 100.0;
            double dr_max_db = absolute_max_db * drMax / 100.0;

            byte[] argb_buffer = new byte[width * height * 4];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    ushort original_value = ushortBuffer[y * width + x];
                    double log_result = Math.Log(Math.Max((double)original_value, 1.0));
                    log_result = Math.Max(log_result * ln10_inv_mul20 - dr_min_db, 0.0);

                    if ((dr_max_db - dr_min_db) > 0)
                        log_result = Math.Min(log_result / (dr_max_db - dr_min_db) * 255.0, 255.0);
                    else
                        log_result = 0.0;

                    byte normalized_value = (byte)Math.Round(log_result);
                    int pixel_index = (y * width + x) * 4;
                    argb_buffer[pixel_index + 0] = normalized_value;
                    argb_buffer[pixel_index + 1] = normalized_value;
                    argb_buffer[pixel_index + 2] = normalized_value;
                    argb_buffer[pixel_index + 3] = 255;
                }
            }
            return argb_buffer;
        }
    }
}
