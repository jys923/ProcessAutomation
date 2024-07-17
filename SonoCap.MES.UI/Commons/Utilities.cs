using Serilog;
using SonoCap.MES.Models;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace SonoCap.MES.UI.Commons
{
    public class Utilities
    {
        public static long GetCurrentUnixTimestampSeconds()
        {
            DateTimeOffset epochTime = DateTimeOffset.UtcNow;
            return epochTime.ToUnixTimeSeconds();
        }

        public static bool SaveImageSourceToFile(ImageSource imageSource, string filePath)
        {
            try
            {
                // Convert ImageSource to Bitmap
                Bitmap bitmap = ConvertImageSourceToBitmap(imageSource);

                // Save the bitmap to the specified file path
                bitmap.Save(filePath, ImageFormat.Bmp);

                // Dispose the bitmap
                bitmap.Dispose();

                // Return success (true)
                return true;
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., file access errors)
                Log.Information($"Error saving bitmap: {ex.Message}");

                // Return failure (false)
                return false;
            }
        }

        private static Bitmap ConvertImageSourceToBitmap(ImageSource imageSource)
        {
            var bitmapSource = imageSource as BitmapSource;
            if (bitmapSource == null)
                throw new ArgumentException("Invalid ImageSource type. Expected BitmapSource.");

            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;
            int stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            byte[] bits = new byte[height * stride];
            bitmapSource.CopyPixels(bits, stride, 0);

            unsafe
            {
                fixed (byte* pB = bits)
                {
                    IntPtr ptr = new IntPtr(pB);
                    return new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
                }
            }
        }

        public static bool SaveBitmapToFile(Bitmap bitmap, string filePath)
        {
            try
            {
                // Save the bitmap to the specified file path
                bitmap.Save(filePath, ImageFormat.Bmp);

                // Return success (true)
                return true;
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., file access errors)
                Log.Information($"Error saving bitmap: {ex.Message}");

                // Return failure (false)
                return false;
            }
        }

        public static bool SaveBitmapToFile(byte[] bitmap, string filePath)
        {
            try
            {
                // Create a BitmapDecoder from the byte array
                using (var stream = new MemoryStream(bitmap))
                {
                    var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                    var frame = decoder.Frames[0]; // Assuming a single frame

                    // Create a BitmapEncoder (e.g., PNG or JPEG)
                    BitmapEncoder encoder = new BmpBitmapEncoder(); // You can choose other formats like JpegBitmapEncoder

                    // Add the bitmap frame to the encoder
                    encoder.Frames.Add(frame);

                    // Save the encoded image to the specified file path
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }

                // Return success (true)
                return true;
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., file access errors)
                Log.Information($"Error saving bitmap: {ex.Message}");

                return false;
            }
        }

        public static void RemoveDuplicateSnDates(ref List<SnDate> snDates)
        {
            snDates = snDates
                .GroupBy(snDate => snDate.Sn)
                .Select(group => group.First())
                .ToList();
        }

        public static string MKRandom(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // 랜덤 숫자 생성기 인스턴스 생성
            Random random = new Random();

            // 랜덤한 10자리 문자열 생성
            char[] stringChars = new char[length];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            string randomString = new string(stringChars);

            //Console.WriteLine("랜덤한 10자리 문자열: " + randomString);

            return randomString;
        }

        public static string MKSHA256()
        {
            string input = MKRandom(20);
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // 해시 값을 문자열로 변환하여 출력
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // 각 바이트를 16진수로 변환하여 추가
                }
                string hashString = sb.ToString();
                //Console.WriteLine("SHA-256 해시 값: " + hashString);
                return hashString;
            }
        }

        public static void Shuffle<T>(IList<T> list)
        {
            Random random = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
