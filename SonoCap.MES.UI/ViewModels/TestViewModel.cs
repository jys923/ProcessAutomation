using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models.Enums;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VILib;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class TestViewModel : ObservableObject
    {
        
        [ObservableProperty]
        private string _title = default!;

        [ObservableProperty]
        private string _probeSn = default!;

        [ObservableProperty]
        private bool _probeSnIsReadOnly = default!;

        [ObservableProperty]
        private string _tDMdSn = default!;

        [ObservableProperty]
        private bool _tDMdSnIsReadOnly = default!;

        [ObservableProperty]
        private string _tDSn = default!;

        [ObservableProperty]
        private bool _tDSnIsReadOnly = default!;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        [ObservableProperty]
        private bool _selectedDateIsReadOnly = default!;

        [ObservableProperty]
        private ObservableCollection<string> _pCs;

        [ObservableProperty]
        private int _pCIndex = default!;

        [ObservableProperty]
        private string _seqNo = default!;

        [ObservableProperty]
        private string _mTMdSn = default!;

        [ObservableProperty]
        private bool _mTMdSnIsReadOnly = default!;

        [ObservableProperty]
        private int _blinkingCellIndex = -1;

        int oldRow = -1;

        [RelayCommand]
        private void CellClick(CellPositions position)
        {
            int row = (int)position / 10;
            Log.Information("row:"+row);
            
            if (oldRow != row)
            {
                oldRow = row;
                ClearAll();
                ChangeReadOnly((TestCategories)row);
            }

            SrcImg = default!;
            ResImg = default!;

            switch (position)
            {
                case CellPositions.Row1_Column1:
                    BlinkingCellIndex = (int)CellPositions.Row1_Column1;
                    break;
                case CellPositions.Row1_Column2:
                    BlinkingCellIndex = (int)CellPositions.Row1_Column2;
                    break;
                case CellPositions.Row1_Column3:
                    BlinkingCellIndex = (int)CellPositions.Row1_Column3;
                    break;
                case CellPositions.Row1_Column4:
                    Log.Information($"click {CellPositions.Row1_Column4}");
                    break;
                case CellPositions.Row2_Column1:
                    BlinkingCellIndex = (int)CellPositions.Row2_Column1;
                    break;
                case CellPositions.Row2_Column2:
                    BlinkingCellIndex = (int)CellPositions.Row2_Column2;
                    break;
                case CellPositions.Row2_Column3:
                    BlinkingCellIndex = (int)CellPositions.Row2_Column3;
                    break;
                case CellPositions.Row2_Column4:
                    Log.Information($"click {CellPositions.Row2_Column4}");
                    break;
                case CellPositions.Row3_Column1:
                    BlinkingCellIndex = (int)CellPositions.Row3_Column1;
                    break;
                case CellPositions.Row3_Column2:
                    BlinkingCellIndex = (int)CellPositions.Row3_Column2;
                    break;
                case CellPositions.Row3_Column3:
                    BlinkingCellIndex = (int)CellPositions.Row3_Column3;
                    break;
                case CellPositions.Row3_Column4:
                    Log.Information($"click {CellPositions.Row3_Column4}");
                    break;
                default:
                    break;
            }
        }

        [ObservableProperty]
        private BitmapImage _srcImg = default!;

        [ObservableProperty]
        private BitmapImage _resImg = default!;

        [ObservableProperty]
        private string _resTxt = default!;

        [ObservableProperty]
        private ObservableCollection<string> _resLogs = default!;

        [ObservableProperty]
        private int _testResultTypeIndex = default!;

        private BitmapImage _loadBmp = default!;
        private BitmapImage _resultBmp = default!;

        VILibWrapper VI = new VILibWrapper();
        VIRes vIResults = new VIRes();

        int ret = 0;

        private Bitmap m_OrgBmp = default!;
        private Bitmap m_bmpRes = default!;
        private Bitmap m_DispMap = default!;

        private byte[] bmp_data = default!;
        private byte[] res_data = default!;

        ImageSource srcImg = default!;
        ImageSource resImg = default!;

        int w = 0;
        int h = 0;
        int chs = 4;
        uint size = 0;
        int opt = 0;
        string sModel = "SC-GP5";

        private void ClearAll()
        {
            ProbeSn = "";
            ProbeSnIsReadOnly = true;
            TDMdSn = "";
            TDMdSnIsReadOnly = true;
            TDSn = "";
            TDSnIsReadOnly = true;
            SeqNo = "";
            MTMdSn = "";
            MTMdSnIsReadOnly = true;
            SrcImg = default!;
            ResImg = default!;
            TestResultTypeIndex = 0;
            TestIsEnabled = true;
            NextIsEnabled = true;
        }

        private void ChangeReadOnly(TestCategories categories)
        {
            //ProbeSnIsReadOnly = false;
            //MTMdSnIsReadOnly = false;
            //TDMdSnIsReadOnly = false;
            //TDSnIsReadOnly = false;
            switch (categories)
            {
                case TestCategories.Process:
                    TDSnIsReadOnly = false;
                    break;
                case TestCategories.Processing:
                    TDMdSnIsReadOnly = false;
                    break;
                case TestCategories.Dispatch:
                    ProbeSnIsReadOnly = false;
                    MTMdSnIsReadOnly = false;
                    break;
            }
        }

        public TestViewModel()
        {
            PCs = new ObservableCollection<string>
            {
                "Left",
                "Middle",
                "Right"
            };
            PCIndex = 0;

            ResLogs = new ObservableCollection<string>();

            ret = VI.Load(0);

            Title = this.GetType().Name;
            // 이미지 파일 경로 설정
            string imagePath = "/Resources/sc_ori_img_512.bmp";

            // 이미지 로드
            SrcImg = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            ResImg = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));

            ProbeSn = "";
            ProbeSnIsReadOnly = true;
            MTMdSnIsReadOnly = true;
            TDMdSnIsReadOnly = true;
            TDSnIsReadOnly = true;

        }

        partial void OnPCIndexChanged(int value)
        {
            Log.Information(value.ToString());
        }

        partial void OnTestResultTypeIndexChanged(int value)
        {
            Log.Information(value.ToString());
        }

        [ObservableProperty]
        private bool _testIsEnabled;

        [RelayCommand]
        private void Test()
        {
            //이미지 불러오기
            //이미지 불러오기
            m_OrgBmp = new Bitmap(@"img\\4.bmp");
            srcImg = ConvertBitmapToImageSource(m_OrgBmp);
            SrcImg = (BitmapImage)srcImg;
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

            ResTxt = sb.ToString();

            ResLogs.Add("succ");

            resImg = ConvertBitmapToImageSource(m_bmpRes);
            ResImg = (BitmapImage)resImg;
        }

        [ObservableProperty]
        private bool _nextIsEnabled;

        [RelayCommand]
        private void Next() 
        {
            MessageBoxResult mbr = MessageBox.Show("Content:save??","Title:save", MessageBoxButton.YesNo);

            switch (mbr)
            {
                case MessageBoxResult.None:
                    break;
                case MessageBoxResult.OK:
                    break;
                case MessageBoxResult.Cancel:
                    break;
                case MessageBoxResult.Yes:
                    Log.Information("mbr yes");
                    //디비 저장
                    //화면 클리어
                    break;
                case MessageBoxResult.No:
                    Log.Information("mbr no");
                    break;
                default:
                    break;
            }
        }

        private BitmapImage convertByteArrToBitmap2(byte[] res_data)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream stream = new MemoryStream(res_data))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        private byte[] ConvertBitmapToImageSource2(BitmapImage bitmapImage)
        {
            byte[] data;
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
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

    }
}
