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
            InspectionPartType.Res // 예시로 Res만 활성화
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

        // --- 상수 설정 ---
        private const string ImageDirectory = "../../../../TestImg/images/"; // 처리할 이미지들이 있는 폴더 경로
        private const string ImageSearchPattern = "*.bmp"; // 처리할 이미지 파일 확장자 (예: *.bmp, *.png, *.* 등)
        private const string OutputRootDirectory = ".\\DebugOutput\\"; // 결과 파일이 저장될 기본 루트 폴더
        // --- 상수 설정 끝 ---


        static void Main()
        {
            // 출력 디렉토리 생성 (이미 존재하면 아무것도 안 함)
            Directory.CreateDirectory(OutputRootDirectory);

            // 레퍼런스 이미지 설정은 한 번만 필요합니다.
            string refImgPath = "./referenceImage.bmp";
            MyOpenCVWrapper.OpenCVWrapper.SetReferenceImage(refImgPath);

            // 지정된 디렉토리에서 모든 이미지 파일을 가져옵니다.
            string[] imagePaths = Directory.GetFiles(ImageDirectory, ImageSearchPattern);

            foreach (string imagePath in imagePaths)
            {
                Console.WriteLine($"\n--- Processing image: {Path.GetFileName(imagePath)} ---");

                // 이미지 로드 (예외 처리 없음 - 실패 시 프로그램 중단)
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

                string baseFileName = Path.GetFileNameWithoutExtension(imagePath);

                // 1. Inspection Part 처리
                foreach (var part in SelectedInspectionParts)
                {
                    string suffix = part.ToString();
                    Array.Clear(textArray, 0, textArray.Length); // 매번 textArray 초기화

                    bool isSuccessful = true; // 성공 여부를 추적할 플래그

                    // Inspection 함수 호출 (예외 처리 없음 - 실패 시 프로그램 중단)
                    var inspectFn = GetInspectionFunction(part);
                    inspectFn(imageBufferPtr, bitmapSource.PixelWidth, bitmapSource.PixelHeight, resultBufferPtr, textBufferPtr);

                    string resultText = System.Text.Encoding.UTF8.GetString(textArray).TrimEnd('\0');

                    // JSON 결과 내용 검사: 빈 값, -1 포함 여부, JSON 파싱 오류
                    if (string.IsNullOrWhiteSpace(resultText) || resultText.Contains("-1"))
                    {
                        isSuccessful = false;
                        Console.WriteLine($"[Inspection: {suffix}] 결과가 비어있거나 -1을 포함하여 실패로 간주합니다.");
                    }
                    else
                    {
                        try // JSON 파싱 시도 중 발생하는 예외는 여기서 처리
                        {
                            var inspectionResult = JsonSerializer.Deserialize<QualityMetrics>(resultText);
                            if (inspectionResult == null)
                            {
                                isSuccessful = false;
                                Console.WriteLine($"[Inspection: {suffix}] JSON 역직렬화 실패 또는 null 결과.");
                            }
                            // TODO: QualityMetrics 내부의 어떤 필드가 -1일 때 실패로 간주할지 구체적인 로직 추가
                            // 예: if (inspectionResult.SomeMetric == -1) { isSuccessful = false; Console.WriteLine("[Inspection: {suffix}] 특정 메트릭 값이 -1 입니다."); }
                        }
                        catch (JsonException) // JSON 파싱 오류
                        {
                            isSuccessful = false;
                            Console.WriteLine($"[Inspection: {suffix}] JSON 파싱 오류로 실패.");
                        }
                    }

                    string outputFileName = isSuccessful ? $"{baseFileName}_{suffix}.json" : $"{baseFileName}_{suffix}_Ng.json";
                    File.WriteAllText(Path.Combine(OutputRootDirectory, outputFileName), resultText);

                    if (isSuccessful)
                    {
                        Console.WriteLine($"[Inspection: {suffix}] 결과 저장 완료: {outputFileName}");
                        // 결과 BMP 저장 (원한다면 성공 시에만)
                        SaveBitmap(
                           BitmapSource.Create(bitmapSource.PixelWidth, bitmapSource.PixelHeight,
                                               96, 96, PixelFormats.Bgr32, null,
                                               resultImageArray, bitmapSource.PixelWidth * 4),
                           Path.Combine(OutputRootDirectory, $"{baseFileName}_{suffix}.bmp"));
                    }
                    else
                    {
                        Console.WriteLine($"[Inspection: {suffix}] 실패 저장 완료: {outputFileName}");
                    }
                }

                // 2. Image Process 처리 (이전 요청과 동일하게 유지 - 함수 실행 중 예외는 프로그램 중단으로 이어짐)
                foreach (var process in SelectedProcesses)
                {
                    string suffix = process.ToString();
                    Array.Clear(textArray, 0, textArray.Length);

                    var processFunction = GetProcessFunction(process);
                    if (processFunction != null)
                    {
                        // Process 함수 호출 (예외 처리 없음 - 실패 시 프로그램 중단)
                        ProcessImage(processFunction, imageBufferPtr, bitmapSource.PixelWidth, bitmapSource.PixelHeight, resultBufferPtr, textBufferPtr);

                        string resultImagePath = Path.Combine(OutputRootDirectory, $"{baseFileName}_{suffix}.bmp");
                        SaveBitmap(BitmapSource.Create(
                                bitmapSource.PixelWidth,
                                bitmapSource.PixelHeight,
                                96, 96,
                                PixelFormats.Bgr32,
                                null,
                                resultImageArray,
                                bitmapSource.PixelWidth * 4
                            ), resultImagePath);

                        string resultText = System.Text.Encoding.UTF8.GetString(textArray).TrimEnd('\0');
                        File.WriteAllText(Path.Combine(OutputRootDirectory, $"{baseFileName}_{suffix}.json"), resultText);
                        Console.WriteLine($"[Process: {suffix}] 결과 저장 완료: {baseFileName}_{suffix}.json");
                    }
                }

                // 3. Image Analyze 처리 (JSON 결과만 저장)
                foreach (var analyze in SelectedAnalyzes)
                {
                    string suffix = analyze.ToString();
                    Array.Clear(textArray, 0, textArray.Length);

                    bool isSuccessful = true; // 성공 여부를 추적할 플래그
                    string resultText = string.Empty; // resultText 변수 스코프 확장

                    var analyzeFunction = GetAnalyzeFunction(analyze);
                    if (analyzeFunction != null)
                    {
                        // Analyze 함수 호출 (예외 처리 없음 - 실패 시 프로그램 중단)
                        AnalyzeImage(analyzeFunction, imageBufferPtr, bitmapSource.PixelWidth, bitmapSource.PixelHeight, textBufferPtr);

                        resultText = System.Text.Encoding.UTF8.GetString(textArray).TrimEnd('\0');

                        // JSON 결과 내용 검사: 빈 값, -1 포함 여부, JSON 파싱 오류
                        if (string.IsNullOrWhiteSpace(resultText) || resultText.Contains("-1"))
                        {
                            isSuccessful = false;
                            Console.WriteLine($"[Analyze: {suffix}] 결과가 비어있거나 -1을 포함하여 실패로 간주합니다.");
                        }
                        else
                        {
                            if (resultText.StartsWith("{"))
                            {
                                try // JSON 파싱 시도 중 발생하는 예외는 여기서 처리
                                {
                                    var analysis = JsonSerializer.Deserialize<QualityMetrics>(resultText);
                                    if (analysis == null)
                                    {
                                        isSuccessful = false;
                                        Console.WriteLine($"[Analyze: {suffix}] JSON 역직렬화 실패 또는 null 결과.");
                                    }
                                    else
                                    {
                                        // TODO: QualityMetrics 내부의 어떤 필드가 -1일 때 실패로 간주할지 구체적인 로직 추가
                                        // 예: if (analysis.SomeSpecificMetricValue == -1) { isSuccessful = false; Console.WriteLine("[Analyze: {suffix}] 특정 메트릭 값이 -1 입니다."); }
                                    }
                                }
                                catch (JsonException) // JSON 파싱 오류
                                {
                                    isSuccessful = false;
                                    Console.WriteLine($"[Analyze: {suffix}] JSON 파싱 오류로 실패.");
                                }
                            }
                            else // JSON 형식이 아닌 경우도 실패로 간주
                            {
                                isSuccessful = false;
                                Console.WriteLine($"[Analyze: {suffix}] 결과가 JSON 형식이 아닙니다.");
                            }
                        }
                    }

                    string outputFileName = isSuccessful ? $"{baseFileName}_{suffix}.json" : $"{baseFileName}_{suffix}_Ng.json";
                    File.WriteAllText(Path.Combine(OutputRootDirectory, outputFileName), resultText);

                    if (isSuccessful)
                    {
                        Console.WriteLine($"[Analyze: {suffix}] 결과 저장 완료: {outputFileName}");
                        // QualityResultManager.MergeAndSave 호출 (성공 시)
                        if (!string.IsNullOrWhiteSpace(resultText) && resultText.StartsWith("{"))
                        {
                            var analysis = JsonSerializer.Deserialize<QualityMetrics>(resultText); // 다시 역직렬화
                            if (analysis != null)
                            {
                                var newRoot = new QualityMetricsRoot
                                {
                                    ImageName = Path.GetFileName(imagePath),
                                    Analysis = analysis
                                };
                                QualityResultManager.MergeAndSave(newRoot, Path.Combine(OutputRootDirectory, $"{baseFileName}_analyze_summary.json"));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[Analyze: {suffix}] 실패 저장 완료: {outputFileName}");
                    }
                }

                // 각 이미지 처리 후 핸들을 해제합니다.
                if (imageHandle.IsAllocated) imageHandle.Free();
                if (resultHandle.IsAllocated) resultHandle.Free();
                if (textHandle.IsAllocated) textHandle.Free();
            } // foreach (string imagePath in imagePaths) 끝
        }

        // MakeResultFolder 함수는 더 이상 사용되지 않습니다.
        static string MakeResultFolder(string imagePath)
        {
            return OutputRootDirectory;
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
            convertedBitmap.DestinationFormat = PixelFormats.Bgr32; // 32bpp BGR (알파 채널 포함)
            convertedBitmap.EndInit();
            convertedBitmap.Freeze();

            return convertedBitmap;
        }
    }
}