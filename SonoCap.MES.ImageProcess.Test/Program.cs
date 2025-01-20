//#define USE_ALIGN_PROCESS
#define USE_RESOLUTION_PROCESS

using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SonoCap.MES.ImageProcess.Test
{
    class Program
    {
        static void Main()
        {
            // 입력 이미지 경로 및 결과 이미지 경로 설정
            // 공통 변수 선언
            string imagePath;
            string processFolder;
            string resultImagePath;
            Action<IntPtr, int, int, IntPtr, IntPtr> processFunction;

#if USE_ALIGN_PROCESS
            imagePath = "../../../../TestImg/1737345113242_.bmp";
            processFunction = MyOpenCVWrapper.OpenCVWrapper.AlignProcess;
            processFolder = ".\\Align\\";

#elif USE_RESOLUTION_PROCESS
            imagePath = "../../../../TestImg/1737097822739.bmp";
            processFunction = MyOpenCVWrapper.OpenCVWrapper.ResolutionProcess;
            processFolder = ".\\Resolution\\";

#else
    Console.WriteLine("Error: No process defined.");
    return;
#endif

            // 폴더 생성 (중복 제거)
            if (!Directory.Exists(processFolder))
            {
                Directory.CreateDirectory(processFolder);
                //Console.WriteLine($"{processFolder} 폴더 생성 완료");
            }

            // 결과 이미지 경로 설정
            resultImagePath = processFolder + "result_image.bmp";

            // 이미지 로드
            BitmapSource bitmapSource = LoadBitmap(imagePath);
            GCHandle imageHandle;
            IntPtr imageBufferPtr = BitmapSourceToByteArray(bitmapSource, out imageHandle);

            // 결과 이미지 저장 배열
            int resultImageSize = bitmapSource.PixelWidth * bitmapSource.PixelHeight * 4;
            byte[] resultImageArray = new byte[resultImageSize];
            GCHandle resultHandle = GCHandle.Alloc(resultImageArray, GCHandleType.Pinned);
            IntPtr resultBufferPtr = resultHandle.AddrOfPinnedObject();

            // 텍스트 데이터 저장 배열
            byte[] textArray = new byte[1024];
            GCHandle textHandle = GCHandle.Alloc(textArray, GCHandleType.Pinned);
            IntPtr textBufferPtr = textHandle.AddrOfPinnedObject();

            // OpenCV 처리 함수 실행 (ProcessImage 내부에는 오직 이 한 줄만 있음)
            ProcessImage(processFunction, imageBufferPtr, bitmapSource.PixelWidth, bitmapSource.PixelHeight, resultBufferPtr, textBufferPtr);

            // 결과 이미지 변환 및 저장
            BitmapSource resultBitmapSource = BitmapSource.Create(
                bitmapSource.PixelWidth,
                bitmapSource.PixelHeight,
                512, 512,
                PixelFormats.Bgr32,
                null,
                resultImageArray,
                bitmapSource.PixelWidth * 4
            );
            SaveBitmap(resultBitmapSource, resultImagePath);

            // 결과 텍스트 출력
            string resultText = System.Text.Encoding.UTF8.GetString(textArray).TrimEnd('\0');
            Console.WriteLine(resultText);

            // 메모리 해제
            imageHandle.Free();
            resultHandle.Free();
            textHandle.Free();
        }

        /// <summary>
        /// OpenCV 처리 함수 호출 (이제 오직 OpenCV 함수 실행만 담당)
        /// </summary>
        /// <param name="processFunction">OpenCV 처리 함수</param>
        static void ProcessImage(Action<IntPtr, int, int, IntPtr, IntPtr> processFunction,
                                 IntPtr imageBufferPtr, int width, int height,
                                 IntPtr resultBufferPtr, IntPtr textBufferPtr)
        {
            processFunction(imageBufferPtr, width, height, resultBufferPtr, textBufferPtr);
        }

        static void Main7(string[] args)
        {
#if USE_ALIGN_PROCESS
            string inputImagePath = "../../../../TestImg/1737345113242_.bmp";
#elif USE_RESOLUTION_PROCESS
            string inputImagePath = "../../../../TestImg/1737097822739.bmp";
#else
            Console.WriteLine("Error: No process defined.");
            return;
#endif
            // 이미지 로드
            BitmapSource inputBitmap = LoadBitmap(inputImagePath);
            GCHandle inputHandle;
            IntPtr inputBufferPtr = BitmapSourceToByteArray(inputBitmap, out inputHandle);

            // 결과 이미지를 저장할 배열
            int outputImageSize = inputBitmap.PixelWidth * inputBitmap.PixelHeight * 4; // 4는 CV_8UC4를 가정
            byte[] outputImageArray = new byte[outputImageSize];
            GCHandle outputHandle = GCHandle.Alloc(outputImageArray, GCHandleType.Pinned);
            IntPtr outputBufferPtr = outputHandle.AddrOfPinnedObject();

            // 텍스트 데이터를 저장할 배열
            byte[] outputTextArray = new byte[1024]; // 적절한 크기 설정
            GCHandle outputTextHandle = GCHandle.Alloc(outputTextArray, GCHandleType.Pinned);
            IntPtr outputTextBufferPtr = outputTextHandle.AddrOfPinnedObject();

            // OpenCV 이미지 처리
#if USE_ALIGN_PROCESS
            MyOpenCVWrapper.OpenCVWrapper.AlignProcess(inputBufferPtr, inputBitmap.PixelWidth, inputBitmap.PixelHeight, outputBufferPtr, outputTextBufferPtr);
#elif USE_RESOLUTION_PROCESS
            MyOpenCVWrapper.OpenCVWrapper.ResolutionProcess(inputBufferPtr, inputBitmap.PixelWidth, inputBitmap.PixelHeight, outputBufferPtr, outputTextBufferPtr);
#else
#endif
        }
#if USE_OLD
        static void Main4(string[] args)
        {
            string alignImgPath = "../../../../TestImg/1737345113242.bmp";
            //BitmapSource bitmapSource = new BitmapImage(new Uri(imagePath));
            BitmapSource alignBitmapSource = LoadBitmap(alignImgPath);

            // PixelFormat을 통해 bits_per_pixel 값 얻기
            int bitsPerPixel = alignBitmapSource.Format.BitsPerPixel;
            Console.WriteLine("Bits per pixel: " + bitsPerPixel);

            // BitmapSource를 IntPtr로 변환
            GCHandle handle;
            IntPtr alignBufferPtr = BitmapSourceToByteArray(alignBitmapSource, out handle);

            // OpenCV 함수 호출 (예: AlignProcess)
            MyOpenCVWrapper.OpenCVWrapper.AlignProcess(alignBufferPtr, alignBitmapSource.PixelWidth, alignBitmapSource.PixelHeight);

            handle.Free();


            string resolutionImgPath = "../../../../TestImg/1737097822739.bmp";
            BitmapSource resolutionBitmapSource = LoadBitmap(resolutionImgPath);

            // PixelFormat을 통해 bits_per_pixel 값 얻기
            int resolutionBitsPerPixel = resolutionBitmapSource.Format.BitsPerPixel;
            Console.WriteLine("Bits per pixel: " + resolutionBitsPerPixel);

            // BitmapSource를 IntPtr로 변환
            //GCHandle handle;
            IntPtr resolutionBufferPtr = BitmapSourceToByteArray(resolutionBitmapSource, out handle);

            //MyOpenCVWrapper.OpenCVWrapper.ResolutionProcess(resolutionBufferPtr, resolutionBitmapSource.PixelWidth, resolutionBitmapSource.PixelHeight);

            // 결과 처리 (예: 결과 저장 또는 출력)
            handle.Free();

            //Console.WriteLine("wait .....");
            //Console.ReadLine();
        }

        static void Main2(string[] args)
        {
            string imagePath = "../../../../TestImg/1737345113242.bmp";

            // 비트맵 파일을 읽고 BitmapSource로 변환
            BitmapSource bitmapSource = LoadBitmap(imagePath);

            // BitmapSource를 바이트 배열로 변환
            byte[] byteArray = BitmapSourceToByteArray(bitmapSource);

            // C++ DLL 호출하여 바이트 배열 전달
            //ConvertToMat(byteArray, byteArray.Length, bitmapSource.PixelWidth, bitmapSource.PixelHeight);

            Console.WriteLine("BitmapSource가 성공적으로 전달되었습니다!");
            Console.ReadLine();
        }

        static void Main3(string[] args)
        {
            // RGBA 비트맵 파일 로드
            string imagePath = "../../../../TestImg/1737345113242.bmp";
            Bitmap bitmap = new Bitmap(imagePath);

            // 이미지 크기와 채널 수 얻기
            int width = bitmap.Width;
            int height = bitmap.Height;
            int channels = 4; // RGBA

            // Bitmap 데이터를 byte[]로 변환
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            int byteCount = bmpData.Stride * bmpData.Height;
            byte[] buffer = new byte[byteCount];
            Marshal.Copy(bmpData.Scan0, buffer, 0, byteCount);
            bitmap.UnlockBits(bmpData);

            // byte[]를 IntPtr로 변환
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr bufferPtr = handle.AddrOfPinnedObject();

            // OpenCV 함수 호출 (예: AlignProcess)
            MyOpenCVWrapper.OpenCVWrapper.AlignProcess(bufferPtr, width, height);
            //MyOpenCVWrapper.OpenCVWrapper.ResolutionProcess(bufferPtr, width, height);

            // 결과 처리 (예: 결과 저장 또는 출력)
            handle.Free();

            Console.WriteLine("wait .....");
            Console.ReadLine();
            while (true) { }
        }
#else
#endif
        // BitmapSource를 BMP 파일로 저장하는 함수
        static void SaveBitmap(BitmapSource bitmapSource, string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
            }
        }

        // BitmapSource를 바이트 배열로 변환하는 함수 (CV_8UC3 포맷 지원)
        public static IntPtr BitmapSourceToByteArray(BitmapSource bitmapSource, out GCHandle handle, System.Windows.Media.PixelFormat pixelFormat)
        {
            int stride = (bitmapSource.PixelWidth * pixelFormat.BitsPerPixel + 7) / 8;
            byte[] pixels = new byte[bitmapSource.PixelHeight * stride];
            bitmapSource.CopyPixels(pixels, stride, 0);
            handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            return handle.AddrOfPinnedObject();
        }

            static IntPtr BitmapSourceToByteArray(BitmapSource bitmapSource, out GCHandle handle)
        {
            int stride = bitmapSource.PixelWidth * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            byte[] byteArray = new byte[stride * bitmapSource.PixelHeight];
            bitmapSource.CopyPixels(byteArray, stride, 0);
            handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            return handle.AddrOfPinnedObject();
        }

        static byte[] BitmapSourceToByteArray(BitmapSource bitmapSource)
        {
            int stride = bitmapSource.PixelWidth * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            byte[] byteArray = new byte[stride * bitmapSource.PixelHeight];
            bitmapSource.CopyPixels(byteArray, stride, 0);
            return byteArray;
        }

        static BitmapSource LoadBitmap(string filePath)
        {
            BitmapSource bitmapSource = null;

            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    // Convert to Bgr32 format
                    FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
                    convertedBitmap.BeginInit();
                    convertedBitmap.Source = bitmap;
                    convertedBitmap.DestinationFormat = PixelFormats.Bgr32;
                    convertedBitmap.EndInit();
                    convertedBitmap.Freeze();

                    bitmapSource = convertedBitmap;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("이미지 로드 중 오류 발생: " + ex.Message);
            }

            return bitmapSource;
        }
    }
}
