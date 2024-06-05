using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using VILib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class TestViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMotorModuleRepository _motorModuleRepository;
        private readonly IPcRepository _pcRepository;
        private readonly IProbeRepository _probeRepository;
        private readonly ITestCategoryRepository _testCategoryRepository;
        private readonly ITesterRepository _testerRepository;
        private readonly ITestRepository _testRepository;
        private readonly ITestTypeRepository _testTypeRepository;
        private readonly ITransducerRepository _transducerRepository;
        private readonly ITransducerModuleRepository _transducerModuleRepository;
        private readonly ITransducerTypeRepository _transducerTypeRepository;

        public TestViewModel(
            IServiceProvider serviceProvider,
            IMotorModuleRepository motorModuleRepository,
            IPcRepository pcRepository,
            IProbeRepository probeRepository,
            ITestCategoryRepository testCategoryRepository,
            ITesterRepository testerRepository,
            ITestRepository testRepository,
            ITestTypeRepository testTypeRepository,
            ITransducerRepository transducerRepository,
            ITransducerModuleRepository transducerModuleRepository,
            ITransducerTypeRepository transducerTypeRepository)
        {
            _serviceProvider = serviceProvider;
            _motorModuleRepository = motorModuleRepository;
            _pcRepository = pcRepository;
            _probeRepository = probeRepository;
            _testCategoryRepository = testCategoryRepository;
            _testerRepository = testerRepository;
            _testRepository = testRepository;
            _testTypeRepository = testTypeRepository;
            _transducerRepository = transducerRepository;
            _transducerModuleRepository = transducerModuleRepository;
            _transducerTypeRepository = transducerTypeRepository;

            Title = this.GetType().Name;

            PCs = new ObservableCollection<string>
            {
                "Left",
                "Middle",
                "Right"
            };

            Init();
        }

        private void Init()
        {
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
            ProbeSnIsEnabled = false;
            MTMdSnIsEnabled = false;
            TDMdSnIsEnabled = false;
            TDSnIsEnabled = false;
        }

        [ObservableProperty]
        private string _title = default!;

        public static ValidationResult ValidateSn(object value, ValidationContext context)
        {
            //bool isValid = ((TestViewModel)context.ObjectInstance).service.Validate(name);
            string? propertyName = context.MemberName; // 프로퍼티 이름
            Log.Information("propertyName:" + propertyName);
            int resCnt = 0;
            switch (propertyName)
            {
                case nameof(ProbeSn):
                    resCnt = ((TestViewModel)context.ObjectInstance)._probeRepository.GetBySn(((TestViewModel)context.ObjectInstance).ProbeSn).ToList().Count;
                    break;
                case nameof(TDMdSn):
                    resCnt = ((TestViewModel)context.ObjectInstance)._transducerModuleRepository.GetBySn(((TestViewModel)context.ObjectInstance).TDMdSn).ToList().Count;
                    break;
                case nameof(TDSn):
                    resCnt = ((TestViewModel)context.ObjectInstance)._transducerRepository.GetBySn(((TestViewModel)context.ObjectInstance).TDSn).ToList().Count;
                    break;
                case nameof(MTMdSn):
                    resCnt = ((TestViewModel)context.ObjectInstance)._motorModuleRepository.GetBySn(((TestViewModel)context.ObjectInstance).MTMdSn).ToList().Count;
                    break;
                default:
                    break;
            }

            Log.Information($"resCnt : {resCnt}");

            if (resCnt == 0)
            {
                return new ValidationResult("시리얼 조회 실패");
            }
            
            string? _value = value as string;
            
            if (_value!.Length > 5 )
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("The name contains invalid characters");
            }
        }

        private bool GetBySn(SnType snType, string sn)
        {
            return snType switch
            {
                SnType.Probe => _probeRepository.GetBySn(sn).ToList().Count > 0,
                SnType.Transducer => _transducerRepository.GetBySn(sn).ToList().Count > 0,
                SnType.MotorModule => _motorModuleRepository.GetBySn(sn).ToList().Count > 0,
                _ => false
            };
        }

        [ObservableProperty]
        private string _probeSn = default!;

        partial void OnProbeSnChanged(string value)
        {
            //정규 표현식 검증 추가
            if (value.Length > 10)
            {
                if (GetBySn(SnType.Probe, value))
                {
                    //ProbeSn Is Not Exist
                    Log.Information($"Probe sn {value}");
                }
            }
        }

        [ObservableProperty]
        private bool _probeSnIsEnabled = default!;

        [ObservableProperty]
        private string _tDMdSn = default!;

        partial void OnTDMdSnChanged(string value)
        {
            //정규 표현식 검증 추가
            if (value.Length > 10)
            {
                if (GetBySn(SnType.TransducerModule, value))
                {
                    //ProbeSn Is Not Exist
                    Log.Information($"TransducerModule sn {value}");
                }
            }
        }

        [ObservableProperty]
        private bool _tDMdSnIsEnabled = default!;

        [ObservableProperty]
        private string _tDSn = default!;

        partial void OnTDSnChanged(string value)
        {
            //정규 표현식 검증 추가
            if (value.Length > 10)
            {
                if (GetBySn(SnType.Transducer, value))
                {
                    //ProbeSn Is Not Exist
                    Log.Information($"Transducer sn {value}");
                }
            }
        }

        [ObservableProperty]
        private bool _tDSnIsEnabled = default!;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        partial void OnSelectedDateChanged(DateTime value)
        {
            throw new NotImplementedException();
        }

        [ObservableProperty]
        private bool _selectedDateIsEnabled = default!;

        [ObservableProperty]
        private ObservableCollection<string> _pCs;

        [ObservableProperty]
        private int _pCIndex = default!;

        [ObservableProperty]
        private string _seqNo = default!;

        [ObservableProperty]
        private string _mTMdSn = default!;

        partial void OnMTMdSnChanged(string value)
        {
            //정규 표현식 검증 추가
            if (value.Length > 10)
            {
                if (GetBySn(SnType.MotorModule, value))
                {
                    //ProbeSn Is Not Exist
                    Log.Information($"MotorModule sn {value}");
                }
            }
        }

        [ObservableProperty]
        private bool _mTMdSnIsEnabled = default!;

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
                ChangeIsEnabled((TestCategories)row);
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
            ProbeSnIsEnabled = false;
            TDMdSn = "";
            TDMdSnIsEnabled = false;
            TDSn = "";
            TDSnIsEnabled = false;
            SeqNo = "";
            MTMdSn = "";
            MTMdSnIsEnabled = false;
            SrcImg = default!;
            ResImg = default!;
            TestResultTypeIndex = 0;
            TestIsEnabled = false;
            NextIsEnabled = false;
        }

        private void ChangeIsEnabled(TestCategories categories)
        {
            //ProbeSnIsEnabled = false;
            //MTMdSnIsEnabled = false;
            //TDMdSnIsEnabled = false;
            //TDSnIsEnabled = false;
            switch (categories)
            {
                case TestCategories.Processing:
                    TDSnIsEnabled = true;
                    break;
                case TestCategories.Process:
                    TDMdSnIsEnabled = true;
                    MTMdSnIsEnabled = true;
                    break;
                case TestCategories.Dispatch:
                    ProbeSnIsEnabled = true;
                    break;
            }
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
            if (!IsValidTesting()) return;

            Save();

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

        private void Save()
        {
            throw new NotImplementedException();
        }

        private bool IsValidTesting()
        {
            var isNullText = delegate (string key, string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    SetValidating(key);
                    return true;
                }
                return false;
            };

            if (isNullText(nameof(ProbeSn), ProbeSn)) return false;

            throw new NotImplementedException();
        }

        [ObservableProperty]
        private string _validationText = "";

        private Dictionary<string, bool> _validatingDict = default!;

        public Dictionary<string, bool> ValidatingDict
        {
            get
            {
                if (_validatingDict == null)
                {
                    _validatingDict = new Dictionary<string, bool>();
                }
                return _validatingDict;
            }
        }

        private void ClearValidating()
        {
            ValidatingDict.Clear();
            ValidationText = "";
        }

        private void SetValidating(string key)
        {
            ValidatingDict[key] = true;
            switch (key)
            {
                case "Email":
                    ValidationText = "Email을 입력하세요.";
                    break;
                case "ExistEmail":
                    ValidationText = "이미 존재하는 Email입니다.";
                    break;
                case "Nickname":
                    ValidationText = "닉네임을 입력하세요.";
                    break;
                case "CellPhone":
                    ValidationText = "휴대전화번호를 입력하세요.";
                    break;
                case "Password":
                    ValidationText = "비밀번호를 입력하세요.";
                    break;
                case "PasswordConfirm":
                    ValidationText = "비밀번호 확인를 입력하세요.";
                    break;
                case "DifferentPassword":
                    ValidatingDict["Password"] = true;
                    ValidatingDict["PasswordConfirm"] = true;
                    ValidationText = "비밀번호와 재입력 값이 일치하지 않습니다.";
                    break;
            }
            OnPropertyChanged(nameof(ValidatingDict));
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
