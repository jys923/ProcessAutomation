//#define USE_ALIGN_PROCESS
//#define USE_RESOLUTION_PROCESS
#define USE_GEOMETRIC_DISTORTION_PROCESS

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

#elif USE_GEOMETRIC_DISTORTION_PROCESS
            imagePath = "../../../../TestImg/1741655984932_ori.bmp";
            //imagePath = "../../../../TestImg/1741245670186_ori.bmp";
            processFunction = MyOpenCVWrapper.OpenCVWrapper.GeometricDistortionProcess;
            processFolder = ".\\GeometricDistortion\\";

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
