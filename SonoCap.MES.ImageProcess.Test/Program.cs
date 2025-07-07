// 파일명: Program.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SonoCap.MES.Models.Process; // QualityMetricsRoot, QualityResultManager 등

namespace SonoCap.MES.ImageProcess.Test
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

    enum ImageProcessType
    {
        Align,
        Resolution,
        GeometricDistortion,
        Gray
    }

    enum ImageAnalyzeType
    {
        Sharpness,
        Brightness,
        Contrast,
        SNR,
        SpeckleIndex,
        Entropy,
        EdgeDensity,
        LocalVariance,
        CNR,
        FFT
    }

    class Program
    {
        static readonly List<InspectionPartType> SelectedInspectionParts = new()
        {
            //InspectionPartType.All,
            //InspectionPartType.Geo,
            //InspectionPartType.Gray,
            InspectionPartType.Res
        };

        static readonly List<ImageProcessType> SelectedProcesses = new()
        {
            //ImageProcessType.Align,
            //ImageProcessType.Resolution,
            //ImageProcessType.GeometricDistortion,
            //ImageProcessType.Gray
        };

        static readonly List<ImageAnalyzeType> SelectedAnalyzes = new()
        {
            //ImageAnalyzeType.Sharpness,
            //ImageAnalyzeType.Brightness,
            //ImageAnalyzeType.Contrast,
            //ImageAnalyzeType.SNR,
            //ImageAnalyzeType.SpeckleIndex,
            //ImageAnalyzeType.Entropy,
            //ImageAnalyzeType.EdgeDensity,
            //ImageAnalyzeType.LocalVariance,
            //ImageAnalyzeType.CNR,
            //ImageAnalyzeType.FFT
        };

        static void Main()
        {
            string refImgPath = "./referenceImage.bmp";
            MyOpenCVWrapper.OpenCVWrapper.SetReferenceImage(refImgPath);

            string imagePath = "../../../../TestImg/images/1751434018403_ori.bmp";
            //string imagePath = "./rotate2.bmp";
            string processFolder = MakeResultFolder(imagePath);

            BitmapSource bitmapSource = LoadBitmap(imagePath);
            GCHandle imageHandle;
            IntPtr imageBufferPtr = BitmapSourceToByteArray(bitmapSource, out imageHandle);

            int resultImageSize = bitmapSource.PixelWidth * bitmapSource.PixelHeight * 4;
            byte[] resultImageArray = new byte[resultImageSize];
            GCHandle resultHandle = GCHandle.Alloc(resultImageArray, GCHandleType.Pinned);
            IntPtr resultBufferPtr = resultHandle.AddrOfPinnedObject();

            byte[] textArray = new byte[4096];
            GCHandle textHandle = GCHandle.Alloc(textArray, GCHandleType.Pinned);
            IntPtr textBufferPtr = textHandle.AddrOfPinnedObject();

            foreach (var part in SelectedInspectionParts)
            {
                Array.Clear(textArray, 0, textArray.Length); // ← 이거 추가

                var inspectFn = GetInspectionFunction(part);
                inspectFn(imageBufferPtr, bitmapSource.PixelWidth, bitmapSource.PixelHeight, resultBufferPtr, textBufferPtr);

                string resultText = System.Text.Encoding.UTF8.GetString(textArray).TrimEnd('\0');
                string partName = part.ToString();

                Console.WriteLine($"[Inspection: {partName}]\n{resultText}\n");

                SaveBitmap(
                    BitmapSource.Create(bitmapSource.PixelWidth, bitmapSource.PixelHeight,
                                        96, 96, PixelFormats.Bgr32, null,
                                        resultImageArray, bitmapSource.PixelWidth * 4),
                    $".\\DebugOutput\\RunInspection_{partName}_result.bmp");

                File.WriteAllText($".\\DebugOutput\\RunInspection_{partName}_result.json", resultText);
            }

            foreach (var process in SelectedProcesses)
            {
                var processFunction = GetProcessFunction(process);
                if (processFunction != null)
                {
                    ProcessImage(processFunction, imageBufferPtr, bitmapSource.PixelWidth, bitmapSource.PixelHeight, resultBufferPtr, textBufferPtr);

                    string resultImagePath = Path.Combine(processFolder, $"{process}_result.bmp");
                    SaveBitmap(BitmapSource.Create(
                        bitmapSource.PixelWidth,
                        bitmapSource.PixelHeight,
                        512, 512,
                        PixelFormats.Bgr32,
                        null,
                        resultImageArray,
                        bitmapSource.PixelWidth * 4
                    ), resultImagePath);

                    string resultText = System.Text.Encoding.UTF8.GetString(textArray).TrimEnd('\0');
                    File.WriteAllText(Path.Combine(processFolder, $"{process}_result.json"), resultText);
                }
            }

            foreach (var analyze in SelectedAnalyzes)
            {
                var analyzeFunction = GetAnalyzeFunction(analyze);
                if (analyzeFunction != null)
                {
                    Array.Clear(textArray, 0, textArray.Length);
                    AnalyzeImage(analyzeFunction, imageBufferPtr, bitmapSource.PixelWidth, bitmapSource.PixelHeight, textBufferPtr);

                    string resultText = System.Text.Encoding.UTF8.GetString(textArray).TrimEnd('\0');

                    if (!string.IsNullOrWhiteSpace(resultText) && resultText.StartsWith("{"))
                    {
                        var analysis = JsonSerializer.Deserialize<QualityMetrics>(resultText);
                        if (analysis != null)
                        {
                            var newRoot = new QualityMetricsRoot
                            {
                                ImageName = Path.GetFileName(imagePath),
                                Analysis = analysis
                            };
                            QualityResultManager.MergeAndSave(newRoot, Path.Combine(processFolder, "analyze_result.json"));
                        }
                    }
                }
            }

            imageHandle.Free();
            resultHandle.Free();
            textHandle.Free();
        }

        static string MakeResultFolder(string imagePath)
        {
            string fileStem = Path.GetFileNameWithoutExtension(imagePath);
            string resultFolder = Path.Combine(".\\DebugOutput\\", fileStem);
            Directory.CreateDirectory(resultFolder);
            return resultFolder;
        }

        static Action<IntPtr, int, int, IntPtr, IntPtr> GetInspectionFunction(InspectionPartType part)
        {
            return (buf, w, h, res, txt) =>
                MyOpenCVWrapper.OpenCVWrapper.RunInspection(buf, w, h, res, txt, (int)part);
        }

        static Action<IntPtr, int, int, IntPtr, IntPtr> GetProcessFunction(ImageProcessType process)
        {
            return process switch
            {
                ImageProcessType.Align => MyOpenCVWrapper.OpenCVWrapper.AlignProcess,
                ImageProcessType.Resolution => MyOpenCVWrapper.OpenCVWrapper.ResolutionProcess,
                ImageProcessType.GeometricDistortion => MyOpenCVWrapper.OpenCVWrapper.GeometricDistortionProcess,
                ImageProcessType.Gray => MyOpenCVWrapper.OpenCVWrapper.GrayProcess,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        static Action<IntPtr, int, int, IntPtr> GetAnalyzeFunction(ImageAnalyzeType analyze)
        {
            return analyze switch
            {
                ImageAnalyzeType.Sharpness => MyOpenCVWrapper.OpenCVWrapper.AnalyzeSharpness,
                ImageAnalyzeType.Brightness => MyOpenCVWrapper.OpenCVWrapper.AnalyzeBrightness,
                ImageAnalyzeType.Contrast => MyOpenCVWrapper.OpenCVWrapper.AnalyzeContrast,
                ImageAnalyzeType.SNR => MyOpenCVWrapper.OpenCVWrapper.AnalyzeSNR,
                ImageAnalyzeType.SpeckleIndex => MyOpenCVWrapper.OpenCVWrapper.AnalyzeSpeckleIndex,
                ImageAnalyzeType.Entropy => MyOpenCVWrapper.OpenCVWrapper.AnalyzeEntropy,
                ImageAnalyzeType.EdgeDensity => MyOpenCVWrapper.OpenCVWrapper.AnalyzeEdgeDensity,
                ImageAnalyzeType.LocalVariance => MyOpenCVWrapper.OpenCVWrapper.AnalyzeLocalVariance,
                ImageAnalyzeType.CNR => MyOpenCVWrapper.OpenCVWrapper.AnalyzeCNR,
                ImageAnalyzeType.FFT => MyOpenCVWrapper.OpenCVWrapper.AnalyzeFFT,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        static void ProcessImage(Action<IntPtr, int, int, IntPtr, IntPtr> processFunction,
                                 IntPtr imageBufferPtr, int width, int height,
                                 IntPtr resultBufferPtr, IntPtr textBufferPtr)
        {
            processFunction(imageBufferPtr, width, height, resultBufferPtr, textBufferPtr);
        }

        static void AnalyzeImage(Action<IntPtr, int, int, IntPtr> analyzeFunction,
                                 IntPtr imageBufferPtr, int width, int height,
                                 IntPtr textBufferPtr)
        {
            analyzeFunction(imageBufferPtr, width, height, textBufferPtr);
        }

        static void SaveBitmap(BitmapSource bitmapSource, string filePath)
        {
            using FileStream stream = new FileStream(filePath, FileMode.Create);
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(stream);
        }

        static IntPtr BitmapSourceToByteArray(BitmapSource bitmapSource, out GCHandle handle)
        {
            int stride = bitmapSource.PixelWidth * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            byte[] byteArray = new byte[stride * bitmapSource.PixelHeight];
            bitmapSource.CopyPixels(byteArray, stride, 0);
            handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            return handle.AddrOfPinnedObject();
        }

        static BitmapSource LoadBitmap(string filePath)
        {
            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();

            FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
            convertedBitmap.BeginInit();
            convertedBitmap.Source = bitmap;
            convertedBitmap.DestinationFormat = PixelFormats.Bgr32;
            convertedBitmap.EndInit();
            convertedBitmap.Freeze();

            return convertedBitmap;
        }
    }
}
