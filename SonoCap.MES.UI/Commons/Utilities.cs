using Serilog;
using SonoCap.MES.Models;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SonoCap.MES.UI.Commons
{
    public class Utilities
    {
        public static bool IsValidImageFormat(Stream stream)
        {
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                BitmapImage tempImage = new BitmapImage();
                tempImage.BeginInit();
                tempImage.CacheOption = BitmapCacheOption.None;
                tempImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                tempImage.StreamSource = stream;
                tempImage.EndInit();
                return true;
            }
            catch (Exception ex)
            {
                Log.Information(ex.ToString());
                return false;
            }
        }

        public static int ByteArrToBitmap(byte[] raw_img, Bitmap m_bmp)
        {
            BitmapData? bmpData = null;
            try
            {
                bmpData = m_bmp.LockBits(new Rectangle(0, 0,
                                                    m_bmp.Width,
                                                    m_bmp.Height),
                                                    ImageLockMode.WriteOnly,
                                                    m_bmp.PixelFormat);

                IntPtr pNative = bmpData.Scan0;
                Marshal.Copy(raw_img, 0, pNative, raw_img.Length);
                m_bmp.UnlockBits(bmpData);

                return 1;
            }
            catch (Exception ex)
            {
                m_bmp.UnlockBits(bmpData);
                Trace.WriteLine(ex.ToString());
                return -1;
            }
        }

        public static ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            using (MemoryStream stream = new MemoryStream())
            {
                // 비트맵을 MemoryStream에 복사합니다.
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);

                stream.Position = 0; // 스트림 포지션을 처음으로 되돌립니다.

                // 비트맵 이미지를 생성하고 반환합니다.
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.StreamSource = stream;
                imageSource.EndInit();

                return imageSource;
            }
        }

        // 원의 외곽선을 그리는 함수
        public static void DrawCircle(Bitmap bitmap, int centerX, int centerY, int radius, System.Drawing.Color color, int thickness)
        {
            // Graphics 객체 생성
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // 원을 그리기 위한 사각형 영역 계산
                Rectangle rectangle = new Rectangle(centerX - radius, centerY - radius, radius * 2, radius * 2);

                // 원 외곽선 그리기
                using (System.Drawing.Pen pen = new System.Drawing.Pen(color, thickness))
                {
                    graphics.DrawEllipse(pen, rectangle);
                }
            }
        }

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
