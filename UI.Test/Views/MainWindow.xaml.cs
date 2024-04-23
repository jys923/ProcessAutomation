using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VILib;

namespace UI.Test.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VILibWrapper VI = new VILibWrapper();
        VIRes vIResults = new VIRes();
        
        int ret = 0;

        private Bitmap m_OrgBmp;
        private Bitmap m_bmpRes;
        private Bitmap m_DispMap;

        private byte[] bmp_data;
        private byte[] res_data;

        ImageSource srcImg;
        ImageSource resImg;

        int w = 0;
        int h = 0;
        int chs = 4;
        uint size = 0;
        int opt = 0;
        string sModel = "SC-GP5";

        public MainWindow()
        {
            InitializeComponent();
            ret = VI.Load(0);
        }

        public int convertByteArrToBitmap(byte[] raw_img, Bitmap m_bmp)
        {
            BitmapData bmpData = null;
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

        public int convertBitmapToByteArr(byte[] result, Bitmap bitmap)
        {
            BitmapData bmpData = null;
            try
            {
                bmpData = bitmap.LockBits(new Rectangle(0, 0,
                                                    bitmap.Width,
                                                    bitmap.Height),
                                                    ImageLockMode.ReadOnly,
                                                    bitmap.PixelFormat);

                IntPtr pNative = bmpData.Scan0;
                int numbytes = bmpData.Stride * bitmap.Height;
                Marshal.Copy(pNative, result, 0, numbytes);
                bitmap.UnlockBits(bmpData);

                return 1;
            }
            catch (Exception ex)
            {
                bitmap.UnlockBits(bmpData);
                Trace.WriteLine(ex.ToString());
                return -1;
            }
        }

        public BitmapImage ConvertByteArrToBitmap(byte[] byteArray)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                stream.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        public byte[] ConvertBitmapToByteArr(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        public byte[] ImageSourceToByteArray(ImageSource imageSource)
        {
            if (imageSource == null)
                return null;

            BitmapSource bitmapSource = (BitmapSource)imageSource;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder(); // Bitmap으로 변환할 것이므로 BMP 형식을 사용
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public ImageSource ByteArrayToImageSource(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return null;

            try
            {
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    BitmapImage imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.CacheOption = BitmapCacheOption.OnLoad;
                    imageSource.StreamSource = stream;
                    imageSource.EndInit();
                    return imageSource;
                }
            }
            catch (Exception ex)
            {
                // 오류 처리
                Console.WriteLine("Failed to convert byte array to ImageSource: " + ex.Message);
                return null;
            }
        }

        public ImageSource ConvertBitmapToImageSource(Bitmap bitmap)
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

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            //이미지 불러오기
            m_OrgBmp = new Bitmap(@"img\\4.bmp");
            srcImg = ConvertBitmapToImageSource(m_OrgBmp);
            SrcImg.Source = srcImg;
            //각종 정보 셋팅
            w = m_OrgBmp.Width;
            h = m_OrgBmp.Height;
            chs = 4;
            size = (uint)(w * h * chs);
            
            m_bmpRes = new Bitmap(m_OrgBmp.Width, m_OrgBmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            bmp_data = new byte[size];
            res_data = new byte[size];
            //검사
            Array.Clear(bmp_data, 0, (int)size);
            Array.Clear(res_data, 0, (int)size);

            convertBitmapToByteArr(bmp_data, m_OrgBmp);

            vIResults = new VIRes();
            ret = VI.DoInspection(sModel, bmp_data, w, h, chs, res_data, ref vIResults, opt);
            convertByteArrToBitmap(res_data, m_bmpRes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < vIResults.nCnt; i++)
            {
                Trace.Write(i + " : ");
                Trace.Write("cen_x : " + vIResults.list[i].cen_x);
                Trace.Write(", cen_y : " + vIResults.list[i].cen_y);
                Trace.Write(", Lum : " + vIResults.list[i].nval);

                if (vIResults.list[i].nState > 0)
                {

                    String s;
                    int index = i + 1;
                    s = index.ToString() + " : "
                        + "cen_x : " + vIResults.list[i].cen_x.ToString()
                        + ", cen_y : " + vIResults.list[i].cen_y.ToString()
                        + ", Lum : " + vIResults.list[i].nval.ToString()
                        + " ";

                    sb.Append(s);
                }
            }

            TestResTb.Text = sb.ToString();

            resImg = ConvertBitmapToImageSource(m_bmpRes);
            ResImg.Source = resImg;

            //textBox1.Text = sb.ToString();

            //SrcImg.Source = new BitmapImage(new Uri("newImage.jpg", UriKind.Relative));
            //m_bmpRes = new Bitmap(m_OrgBmp.Width, m_OrgBmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //w = m_OrgBmp.Width;
            //h = m_OrgBmp.Height;


            //bmp_data = new byte[size];
            //res_data = new byte[size];
            //byte[] bmp_data = ImageSourceToByteArray(SrcImg.Source);
            //ret = VI.DoInspection(sModel, bmp_data, w, h, chs, res_data, ref vIResults, opt);
            //ImageSource resImg = ByteArrayToImageSource(res_data);

        }
    }
}