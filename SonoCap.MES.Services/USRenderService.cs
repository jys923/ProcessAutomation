using HsnLibraryCS;
using SonoCap.WpfCommons;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SonoCap.MES.Services
{
    public class USRenderService
    {
        private HsnUltrasoundOffScreenView offScreenView;//offscreenview

        private int _width;
        private int _height;
        private int _length;
        //private byte[] _buffer;
        private int _verticalShift { get; set; } = 0;

        public float DRMin { get; set; } = 0;
        public float DRMax { get; set; } = 100;

        public USRenderService(int width, int height)
        {
            _width = width;
            _height = height;
            _length = _width * _height * 4;
            //_buffer = new byte[_length];
        }

        Action<BitmapSource>? renderToTarget = null;
        Action<BitmapSource>? renderToTargetEnv = null;

        public void connectRenderToTargetFunction(Action<BitmapSource> action, Action<BitmapSource> actionEnv)
        {
            renderToTarget = action;
            renderToTargetEnv = actionEnv;

        }
        public void RenderStart()
        {
            offScreenView = new HsnLibraryCS.HsnUltrasoundOffScreenView(_width, _height);
            offScreenView.setTargetIPFrameRate(60);
            offScreenView.Start(LoadImage, LoadEnv);
        }

        public void SetRotationAngle(double angle)
        {
            offScreenView?.SetRotationAngle(angle);
        }

        public void SetVerticalFlip(bool flip)
        {
            offScreenView?.SetVerticalFlip(flip);
        }

        public void SetScanline(int envdata_height)
        {
            offScreenView?.SetScanline(envdata_height);
        }
        public void SetVerticalShift(int currentStep)
        {
            _verticalShift = currentStep;
        }

        public void RenderEnd()
        {
            //pboxes = null;
            if (offScreenView != null)
            {
                renderToTarget = null;
                renderToTargetEnv = null;
                offScreenView.End();
            }
        }

        static double framerate_acc_val = 0;
        static DateTime prev_time = DateTime.Now;

        public BitmapSource ResizeBitmapSource(BitmapSource source, int width, int height)
        {
            // 원본 BitmapSource의 너비와 높이를 가져옵니다.
            double originalWidth = source.PixelWidth;
            double originalHeight = source.PixelHeight;

            // 스케일 비율을 계산합니다.
            double scaleX = width / originalWidth;
            double scaleY = height / originalHeight;

            // TransformedBitmap을 사용하여 스케일 변환을 적용합니다.
            TransformedBitmap resizedBitmap = new TransformedBitmap(
                source,
                new ScaleTransform(scaleX, scaleY)
            );

            return resizedBitmap;
        }

        private void LoadImage(byte[] buffer, int width, int height, int length, MetadataInfo metadata)
        {
            var curr_time = DateTime.Now;
            var elapsed_time = curr_time - prev_time;
            if (elapsed_time.TotalMilliseconds > 1000)
            {
                framerate_acc_val++;
                //Debug.WriteLine("IP Framerate : " + (framerate_acc_val * 1000.0 / elapsed_time.TotalMilliseconds).ToString());
                framerate_acc_val = 0;
                prev_time = curr_time;
            }
            else
            {
                framerate_acc_val++;
            }

            int stride = width * 4;

            // 버퍼를 복사하여 BitmapSource의 버퍼로 사용 (GC 안전)
            byte[] copy = new byte[length];
            Buffer.BlockCopy(buffer, 0, copy, 0, length);

            // byte[] 배열을 직접 BitmapSource로 변환
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                BitmapSource bitmapSource = BitmapSource.Create(
                    width, height,
                    96, 96,
                    System.Windows.Media.PixelFormats.Bgr32,
                    null,
                    copy,
                    stride
                );

                renderToTarget?.Invoke(bitmapSource);
            }));
        }
        private byte[] CreateNormalizedBitmapDataFromRaw2Byte(
                        byte[] rawUint8Buffer, // 2바이트(uint16_t) 값들이 바이트 형태로 들어오는 버퍼
                        int width,
                        int height,
                        double drMin,
                        double drMax)
        {
            if (rawUint8Buffer == null || width <= 0 || height <= 0 /*|| rawUint8Buffer.Length != (width * height * 2)*/)
            {
                // 유효성 검사: 버퍼가 null이거나 크기가 충분하지 않은 경우
                //Debug.WriteLine("Error: Invalid input buffer or dimensions for CreateNormalizedBitmapDataFromRaw2Byte.");
                return new byte[0]; // 빈 배열 반환
            }

            // 1. **2바이트씩 읽어서** 처리: byte[]를 ushort[]으로 변환하여 uint16_t 값으로 해석
            ushort[] ushortBuffer = new ushort[rawUint8Buffer.Length / 2];
            Buffer.BlockCopy(rawUint8Buffer, 0, ushortBuffer, 0, rawUint8Buffer.Length);

            // C++ 코드의 상수 값들 (log10(sqrt(2)*32768)*20, 20/ln(10))
            double absolute_max_db = Math.Log10(Math.Sqrt(2.0) * 32768.0) * 20.0;
            const double ln10_inv_mul20 = 8.685890;

            // dBm 다이나믹 레인지 계산
            double dr_min_db = absolute_max_db * drMin / 100.0;
            double dr_max_db = absolute_max_db * drMax / 100.0;

            // 결과 버퍼: 4 bytes per pixel (BGRA 순서, 윈도우 비트맵 표준)
            byte[] argb_buffer = new byte[width * height * 4];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // ushortBuffer에서 2바이트씩 1픽셀 값(uint16_t)을 읽어옴
                    ushort original_value = ushortBuffer[y * width + x];

                    // 2. **dBm 정규화(로그 스케일링) 및 스케일링** 로직
                    double log_result = Math.Log(Math.Max((double)original_value, 1.0)); // 0 또는 음수 방지
                    log_result = Math.Max(log_result * ln10_inv_mul20 - dr_min_db, 0.0);

                    if ((dr_max_db - dr_min_db) > 0)
                    {
                        log_result = Math.Min(log_result / (dr_max_db - dr_min_db) * 255.0, 255.0);
                    }
                    else
                    {
                        log_result = 0.0; // 분모가 0이거나 음수일 경우 0으로 처리
                    }

                    byte normalized_value = (byte)Math.Round(log_result); // 0-255 범위로 변환

                    // 3. **비트맵으로 변환** (4바이트 BGRA 픽셀 데이터 생성)
                    int pixel_index = (y * width + x) * 4;
                    argb_buffer[pixel_index + 0] = normalized_value; // Blue (그레이스케일이므로 R, G, B 모두 동일)
                    argb_buffer[pixel_index + 1] = normalized_value; // Green
                    argb_buffer[pixel_index + 2] = normalized_value; // Red
                    argb_buffer[pixel_index + 3] = 255;              // Alpha (불투명)
                }
            }
            return argb_buffer; // 최종 4바이트 BGRA 비트맵 데이터 반환
        }

        private double _verticalShiftOffset = 0.0;

        public void LoadEnv(byte[] buffer, int width, int height, int length, MetadataInfo metadata)
        {
            _verticalShiftOffset = height / 360.0;

            byte[] processedEnvBitmapData = CreateNormalizedBitmapDataFromRaw2Byte(
                buffer,        // 2바이트 원본 데이터
                width,
                height,
                DRMin, // MetadataInfo에서 DrMin 가져오기
                DRMax  // MetadataInfo에서 DrMax 가져오기
            );

            // 4바이트 BGRA 비트맵 데이터의 스트라이드 (width * bytes_per_pixel)
            int stride = width * 4;

            Utilities.ShiftBytesCircularly(processedEnvBitmapData, stride * (int)(_verticalShiftOffset * _verticalShift));

            // UI 스레드에서 BitmapSource 생성 및 렌더링 (LoadImage와 유사)
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (processedEnvBitmapData.Length == 0) // 변환 실패 시 처리
                {
                    Debug.WriteLine("Warning: No envelope bitmap data generated.");
                    return;
                }

                BitmapSource envBitmapSource = BitmapSource.Create(
                    width,
                    height,
                    96, 96, // DPI 값
                    System.Windows.Media.PixelFormats.Bgr32, // 4바이트 BGRA 형식
                    null, // 팔레트 (단색 이미지이므로 필요 없음)
                    processedEnvBitmapData, // 2바이트 원본이 처리된 4바이트 BGRA 비트맵 데이터
                    stride // 스트라이드
                );
                renderToTargetEnv?.Invoke(envBitmapSource); // 변환된 비트맵 소스를 UI에 전달
            }));
        }
    }
}