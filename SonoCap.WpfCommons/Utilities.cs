using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Serilog;
using System.Drawing.Imaging;
using System.Drawing;
using System.Security.Cryptography;
using SonoCap.MES.Models;
using System.Text.Json;

namespace SonoCap.WpfCommons
{
    public static class Utilities
    {

        public static void ShiftBytesCircularly(byte[] data, int shiftLength)
        {
            if (data == null || data.Length == 0)
            {
                // 데이터가 없으면 아무것도 하지 않습니다.
                return;
            }

            int totalLength = data.Length;

            // 실제 이동시킬 바이트 길이 계산 (배열 길이를 넘어가지 않도록 모듈로 연산)
            int actualShift = shiftLength % totalLength;
            if (actualShift < 0)
            {
                actualShift += totalLength; // 음수 결과를 양수로 보정하여 항상 0 이상 totalLength 미만의 값으로 만듭니다.
            }

            // 이동할 필요가 없으면 바로 종료합니다.
            if (actualShift == 0)
            {
                return;
            }

            // 임시 저장 공간 (이동시킬 블록의 크기만큼만 필요)
            byte[] temp = new byte[actualShift];

            // --- 순환 이동 로직 ---
            // 1. 이동시킬 앞부분(또는 뒷부분) 데이터를 임시 공간에 복사
            Buffer.BlockCopy(data, 0, temp, 0, actualShift);

            // 2. 나머지 데이터를 이동시킬 방향으로 옮김
            Buffer.BlockCopy(data, actualShift, data, 0, totalLength - actualShift);

            // 3. 임시 공간에 있던 데이터를 나머지 부분에 붙여넣음
            Buffer.BlockCopy(temp, 0, data, totalLength - actualShift, actualShift);
        }

        public static string FormatJson(string json)
        {
            try
            {
                using var jdoc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(jdoc, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch
            {
                // 실패 시 원본 그대로 반환 (비정상 JSON 등)
                return json;
            }
        }

        public static string GenImgName(string prefix, string exportDirectory)
        {
            if (!Directory.Exists(exportDirectory))
                Directory.CreateDirectory(exportDirectory);

            var existingFiles = Directory.EnumerateFiles(exportDirectory, $"{prefix}_*.bmp")
                .Concat(Directory.EnumerateFiles(exportDirectory, $"{prefix}_*.png"))
                .Select(path => Path.GetFileNameWithoutExtension(path))
                .Where(name => name.StartsWith(prefix + "_"))
                .Select(name =>
                {
                    string[] parts = name.Split('_');
                    if (parts.Length >= 2 && int.TryParse(parts.Last(), out int num))
                        return num;
                    return 0;
                });

            int maxIndex = existingFiles.Any() ? existingFiles.Max() : 0;
            int newIndex = maxIndex + 1;

            return $"{prefix}_{newIndex:D3}";
        }

        public static ImageSource LoadOrDefault(string basePath, string? fileName, ImageSource defaultImage)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return defaultImage;

            string path = Path.Combine(basePath, fileName);
            return GetFileToImageSource(path) ?? defaultImage;
        }

        public static string GetExportImgPath(string basePath, Dictionary<string, string> phases, int testCategory, string sn)
        {
            string phaseKey = testCategory switch
            {
                1 => "Process",
                2 => "Product",
                3 => "Final",
                _ => "Unknown"
            };

            if (phases.TryGetValue(phaseKey, out string? phaseFolder))
            {
                return Path.Combine(basePath, phaseFolder, sn);
            }

            return basePath;
        }

        public static string MoveTempImageToExport(string fileName, string tempPath, string exportPath, string newFileName)
        {
            string sourcePath = Path.Combine(tempPath, fileName);
            string destPath = Path.Combine(exportPath, newFileName);

            try
            {
                if (!Directory.Exists(exportPath))
                    Directory.CreateDirectory(exportPath);

                // 기존 파일이 있으면 삭제
                if (File.Exists(destPath))
                    File.Delete(destPath);

                File.Move(sourcePath, destPath);
                return destPath;
            }
            catch (Exception ex)
            {
                Log.Error($"파일 이동 실패: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 경로 + 파일명(.확장자)을 안전하게 생성해줍니다.
        /// 폴더가 없으면 자동 생성되며, 슬래시 문제도 자동 처리됩니다.
        /// </summary>
        /// <param name="baseDir">기준 디렉터리 (예: ExportImg)</param>
        /// <param name="prefix">파일명 접두어 (예: capture, screen 등)</param>
        /// <param name="ext">확장자 (예: png, mp4)</param>
        /// <returns>전체 경로</returns>
        public static string BuildPath(string baseDir, string fileNameWithPrefix, string ext)
        {
            Directory.CreateDirectory(baseDir);
            return Path.Combine(baseDir, $"{fileNameWithPrefix}.{ext}");
        }
        public static bool ResetFolder(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, recursive: true);

                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ex)
            {
                Log.Warning($"ResetFolder failed for {path}: {ex.Message}");
                return false;
            }
        }

        public static bool EnsureFolderExists(string folderName)
        {
            try
            {
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                return true; // 폴더 생성 성공
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false; // 폴더 생성 실패
            }
        }

        public static bool IsMatchReqExr(string value, string pattern)
        {
            // 정규식 패턴
            //string pattern = @"^[0-9]{4}$";
            // 정규식 객체 생성 및 컴파일 옵션 사용
            Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // 패턴 매칭 확인
            bool isMatch = regex.IsMatch(value);
            return isMatch;
        }

        public static string ExtractReqExr(string input, string pattern)
        {
            //string pattern = "^.{0,11}([0-9]{3})$";
            Match match = Regex.Match(input, pattern);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        public static string[]? ExtractReqExrSn(string input)
        {
            string pattern = @"^.{5}(2[0-9]|19|20)(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])([0-9]{3})$";
            Match match = Regex.Match(input, pattern);
            if (match.Success)
            {
                string extractedDate = match.Groups[2].Value + match.Groups[3].Value + match.Groups[4].Value;
                string extractedNumber = match.Groups[5].Value;
                return new string[] { extractedDate, extractedNumber };
            }

            return null;
        }

        public static bool ImageSourceToPng(ImageSource imageSource, string fileName)
        {
            try
            {
                var bitmapSource = imageSource as BitmapSource;
                if (bitmapSource == null)
                    throw new ArgumentException("ImageSource must be of type BitmapSource", nameof(imageSource));

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }

                return true; // 성공
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
                return false; // 실패
            }
        }

        public static bool ImageSourceToBmp(ImageSource imageSource, string fileName)
        {
            try
            {
                var bitmapSource = imageSource as BitmapSource;
                if (bitmapSource == null)
                    throw new ArgumentException("ImageSource must be of type BitmapSource", nameof(imageSource));

                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }

                return true; // 성공
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
                return false; // 실패
            }
        }

        public static bool ImageSourceToGrayBmp(ImageSource imageSource, string fileName)
        {
            try
            {
                var bitmapSource = imageSource as BitmapSource;
                if (bitmapSource == null)
                    throw new ArgumentException("ImageSource must be of type BitmapSource", nameof(imageSource));

                // 그레이스케일 포맷으로 변환
                var formatConvertedBitmap = new FormatConvertedBitmap(bitmapSource, PixelFormats.Gray8, null, 0);

                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(formatConvertedBitmap));

                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }

                return true; // 성공
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
                return false; // 실패
            }
        }

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

        public static string WhatImageFormat(byte[] rawImage)
        {
            int length = rawImage.Length;

            if (length % 4 == 0)
            {
                return "ARGB";
            }
            else if (length % 3 == 0)
            {
                return "RGB";
            }
            else if (length % 1 == 0)
            {
                return "Grayscale";
            }
            else
            {
                return "Unknown";
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
        public static void DrawCircle(Bitmap bitmap, Point center, int radius, System.Drawing.Color color, int thickness)
        {
            // Graphics 객체 생성
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // 원을 그리기 위한 사각형 영역 계산
                Rectangle rectangle = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);

                // 원 외곽선 그리기
                using (System.Drawing.Pen pen = new System.Drawing.Pen(color, thickness))
                {
                    graphics.DrawEllipse(pen, rectangle);
                }
            }
        }

        // 직선을 그리는 함수
        public static void DrawLine(Bitmap bitmap, Point start, Point end, System.Drawing.Color color, int thickness)
        {
            // Graphics 객체 생성
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // 직선 그리기
                using (System.Drawing.Pen pen = new System.Drawing.Pen(color, thickness))
                {
                    graphics.DrawLine(pen, start, end);
                }
            }
        }

        // 호를 그리는 함수
        public static void DrawArc(Bitmap bitmap, Point center, int radius, float startAngle, float sweepAngle, System.Drawing.Color color, int thickness)
        {
            // Graphics 객체 생성
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // 호를 그리기 위한 사각형 영역 계산
                Rectangle rectangle = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);

                // 호 그리기
                using (System.Drawing.Pen pen = new System.Drawing.Pen(color, thickness))
                {
                    graphics.DrawArc(pen, rectangle, startAngle, sweepAngle);
                }
            }
        }


        /// <summary>
        /// epoch
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentUnixTimestampSeconds()
        {
            DateTimeOffset epochTime = DateTimeOffset.UtcNow;
            return epochTime.ToUnixTimeSeconds();
        }

        public static long GetCurrentUnixTimestampMilliseconds()
        {
            DateTimeOffset epochTime = DateTimeOffset.UtcNow;
            return epochTime.ToUnixTimeMilliseconds();
        }

        public static ImageSource? GetFileToImageSource(string? filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return null;
                }

                //string uriPath = $"pack://application:,,,{filePath}";

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                return image;
            }
            catch (Exception ex)
            {
                // 예외 처리: 파일 로드 실패 시 null 반환 또는 로그 작성
                Log.Information($"Error loading image from file: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 어셈블리에서 리소스 파일을 로드하여 BitmapImage 객체를 반환합니다.
        /// </summary>
        /// <param name="fileName">리소스 파일 이름 (예: sc_ori_img_512.bmp)</param>
        /// <returns>BitmapImage 객체, 파일을 찾을 수 없으면 null</returns>
        public static BitmapImage LoadBitmapFromResource(string fileName)
        {
            // 네임스페이스와 폴더 경로를 포함한 리소스 파일 이름 생성
            string resourceName = $"SonoCap.MES.UI.Resources.{fileName}";

            // 현재 실행 중인 어셈블리 가져오기
            Assembly assembly = Assembly.GetExecutingAssembly();

            // 리소스 스트림 가져오기
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    // BitmapImage 객체 생성 및 초기화
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    return image;
                }
                else
                {
                    // 리소스 스트림을 찾을 수 없을 때
                    return null;
                }
            }
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

        public static ImageSource CopyImageSource(ImageSource source)
        {
            if (source == null)
            {
                // 빈 이미지 생성
                var emptyImage = new RenderTargetBitmap(1, 1, 96, 96, PixelFormats.Pbgra32);
                return emptyImage;
            }

            if (source is BitmapSource bitmapSource)
            {
                var encoder = new BmpBitmapEncoder(); // BmpBitmapEncoder 사용
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    var newBitmap = new BitmapImage();
                    newBitmap.BeginInit();
                    newBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    newBitmap.StreamSource = stream;
                    newBitmap.EndInit();

                    return newBitmap;
                }
            }

            return new RenderTargetBitmap(1, 1, 96, 96, PixelFormats.Pbgra32);
        }

        public static async Task<ImageSource> CopyImageSourceAsync(ImageSource source)
        {
            return await Task.Run(() =>
            {
                if (source == null)
                {
                    // 빈 이미지 생성
                    var emptyImage = new RenderTargetBitmap(1, 1, 96, 96, PixelFormats.Pbgra32);
                    return (ImageSource)emptyImage;
                }

                var bitmapSource = source as BitmapSource;
                if (bitmapSource != null)
                {
                    var encoder = new BmpBitmapEncoder(); // BmpBitmapEncoder 사용
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                    using (var stream = new MemoryStream())
                    {
                        encoder.Save(stream);
                        stream.Seek(0, SeekOrigin.Begin);

                        var newBitmap = new BitmapImage();
                        newBitmap.BeginInit();
                        newBitmap.CacheOption = BitmapCacheOption.OnLoad;
                        newBitmap.StreamSource = stream;
                        newBitmap.EndInit();

                        return (ImageSource)newBitmap;
                    }
                }

                // source가 BitmapSource가 아닌 경우 빈 이미지 반환
                return new RenderTargetBitmap(1, 1, 96, 96, PixelFormats.Pbgra32);
            });
        }

        public static void ProcessImage(Action<IntPtr, int, int, IntPtr, IntPtr> processFunction,
                                 IntPtr imageBufferPtr, int width, int height,
                                 IntPtr resultBufferPtr, IntPtr textBufferPtr)
        {
            processFunction(imageBufferPtr, width, height, resultBufferPtr, textBufferPtr);
        }

        public static void InspectionImage(Action<IntPtr, int, int, IntPtr, IntPtr, int> inspectionFunction,
                                 IntPtr imageBufferPtr, int width, int height,
                                 IntPtr resultBufferPtr, IntPtr textBufferPtr, int testPartFlags)
        {
            inspectionFunction(imageBufferPtr, width, height, resultBufferPtr, textBufferPtr, testPartFlags);
        }

        public static IntPtr BitmapSourceToByteArray(BitmapSource bitmapSource, out GCHandle handle)
        {
            int stride = bitmapSource.PixelWidth * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            byte[] byteArray = new byte[stride * bitmapSource.PixelHeight];
            bitmapSource.CopyPixels(byteArray, stride, 0);
            handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            return handle.AddrOfPinnedObject();
        }

        public static void SaveBitmap(BitmapSource bitmapSource, string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
            }
        }

        public static void SavePng(BitmapSource bitmapSource, string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
            }
        }
        public static BitmapSource ConvertToGray8(BitmapSource source)
        {
            if (source.Format == PixelFormats.Gray8)
                return source;

            return new FormatConvertedBitmap(source, PixelFormats.Gray8, null, 0);
        }

        public enum ImageFormatType
        {
            Bmp,
            Png,
            Jpeg,
            Tiff
        }

        public static void SaveImage(BitmapSource bitmapSource, string filePath, ImageFormatType format)
        {
            BitmapEncoder encoder = format switch
            {
                ImageFormatType.Bmp => new BmpBitmapEncoder(),
                ImageFormatType.Png => new PngBitmapEncoder(),
                ImageFormatType.Jpeg => new JpegBitmapEncoder(),
                ImageFormatType.Tiff => new TiffBitmapEncoder(),
                _ => throw new ArgumentOutOfRangeException(nameof(format), $"지원되지 않는 포맷: {format}")
            };

            using FileStream stream = new FileStream(filePath, FileMode.Create);
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(stream);
        }


        public static BitmapSource CopyBitmapSource(BitmapSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var encoder = new BmpBitmapEncoder();
            //var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var newBitmap = new BitmapImage();
                newBitmap.BeginInit();
                newBitmap.CacheOption = BitmapCacheOption.OnLoad;
                newBitmap.StreamSource = stream;
                newBitmap.EndInit();

                return newBitmap;
            }
        }

        public static int GetPixelValue(BitmapSource bitmapSource, int x, int y)
        {
            if (x < 0 || x >= bitmapSource.PixelWidth || y < 0 || y >= bitmapSource.PixelHeight)
            {
                throw new ArgumentOutOfRangeException("x 또는 y 좌표가 이미지의 범위를 벗어났습니다.");
            }

            // 픽셀 데이터를 저장할 배열
            byte[] pixels = new byte[4]; // Bgr32 포맷은 픽셀당 4바이트 (B, G, R, A)

            // 이미지의 픽셀을 추출
            bitmapSource.CopyPixels(new System.Windows.Int32Rect(x, y, 1, 1), pixels, 4, 0);

            // 그레이스케일 이미지에서 모든 채널은 동일한 값을 가짐
            return pixels[2]; // R, G, B 중 하나의 값을 반환 (여기서는 R 채널 선택)
        }

        public static double GetCirclePixelMean(BitmapSource bitmapSource, int centerX, int centerY, double radius, double strokeThickness)
        {
            if (bitmapSource == null) throw new ArgumentNullException(nameof(bitmapSource));

            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;
            int stride = width * 4; // Bgr32 포맷의 경우, 1픽셀 당 4바이트

            byte[] pixels = new byte[height * stride];
            bitmapSource.CopyPixels(pixels, stride, 0);

            int pixelCount = 0;
            long pixelSum = 0;

            double innerRadius = radius - strokeThickness;
            double innerRadiusSquared = innerRadius * innerRadius;

            for (double y = centerY - innerRadius; y <= centerY + innerRadius; y++)
            {
                if (y < 0 || y >= height) continue;

                for (double x = centerX - innerRadius; x <= centerX + innerRadius; x++)
                {
                    if (x < 0 || x >= width) continue;

                    double dx = x - centerX;
                    double dy = y - centerY;

                    if (dx * dx + dy * dy < innerRadiusSquared)
                    {
                        int ix = (int)Math.Round(x);
                        int iy = (int)Math.Round(y);

                        int index = (iy * stride) + (ix * 4);
                        if (index < 0 || index >= pixels.Length) continue;

                        byte pixelValue = pixels[index + 2]; // R, G, B 채널은 모두 같은 값이므로 하나의 채널 값만 사용
                        pixelSum += pixelValue;
                        pixelCount++;
                    }
                }
            }

            if (pixelCount > 0)
            {
                //Log.Information($"pixelSum : {pixelSum} pixelCount : {pixelCount}");
                return (double)pixelSum / pixelCount;
            }
            else
            {
                return double.NaN; // 원 내부에 픽셀이 없는 경우
            }
        }

        public static void OpenFolder(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;

            try
            {
                var fullPath = Path.GetFullPath(filePath);
                Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"파일 선택 열기 실패: {filePath}");
            }
        }
    }
}
