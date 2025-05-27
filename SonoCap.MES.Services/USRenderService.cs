using HsnLibraryCS;
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
        
        public USRenderService(int width, int height)
        {
            _width = width;
            _height = height;
            _length = _width * _height * 4;
            //_buffer = new byte[_length];
        }

        Action<BitmapSource>? renderToTarget = null;
        public void connectRenderToTargetFunction(Action<BitmapSource> action)
        {
            renderToTarget = action;
        }
        public void RenderStart()
        {
            offScreenView = new HsnLibraryCS.HsnUltrasoundOffScreenView(_width, _height);
            offScreenView.setTargetIPFrameRate(60);
            offScreenView.Start(LoadImage);
        }

        public void SetRotationAngle(double angle)
        {
            offScreenView?.SetRotationAngle(angle);
        }

        public void SetVerticalFlip(bool flip)
        {
            offScreenView?.SetVerticalFlip(flip);
        }

        public void RenderEnd()
        {
            //pboxes = null;
            if (offScreenView != null)
            {
                renderToTarget = null;
                offScreenView.End();
            }
        }

        static double framerate_acc_val = 0;
        static DateTime prev_time = DateTime.Now;
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
    }
}

