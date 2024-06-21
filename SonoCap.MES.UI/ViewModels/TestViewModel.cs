using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.Validation;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VILib;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class TestViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = default!;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TestCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private string _probeSn = default!;

        [ObservableProperty]
        private bool _probeSnIsEnabled = default!;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TestCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private string _tDMdSn = default!;

        [ObservableProperty]
        private bool _tDMdSnIsEnabled = default!;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TestCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private string _tDSn = default!;

        [ObservableProperty]
        private bool _tDSnIsEnabled = default!;

        [ObservableProperty]
        private bool _addTDSnIsEnabled = default!;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        [ObservableProperty]
        private bool _selectedDateIsEnabled = default!;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TestCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private string _seqNo = default!;

        [ObservableProperty]
        private bool _seqNoIsEnabled = default!;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddMTMdSnCommand))]
        [NotifyCanExecuteChangedFor(nameof(TestCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private string _mTMdSn = default!;

        [ObservableProperty]
        private bool _mTMdSnIsEnabled = default!;

        [ObservableProperty]
        private bool _addMTMdSnIsEnabled = default!;


        [ObservableProperty]
        private int _blinkingCellIndex = -1;

        private int _oldRow = -1;

        [ObservableProperty]
        private BitmapImage _srcImg = default!;

        [ObservableProperty]
        private BitmapImage _resImg = default!;

        [ObservableProperty]
        private string _resTxt = default!;

        [ObservableProperty]
        private ObservableCollection<string> _resLogs = default!;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private int _testResult = default!;

        [ObservableProperty]
        private string _selectedLogItem = default!;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TestCommand))]
        private Dictionary<string, ValidationItem> _validationDict = new();

        [ObservableProperty]
        private Dictionary<int, Brush> _borderBackgrounds = new();
        private Probe? _probe { get; set; }
        private TransducerModule? _transducerModule { get; set; }
        private Transducer? _transducer { get; set; }
        private MotorModule? _motorModule { get; set; }
        private TestCategories _testCategory { get; set; }
        private TestTypes _testType { get; set; }
        private Test? _test { get; set; }
        private Tester? _tester { get; set; }
        private PTRView? _pTRView { get; set; }

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
        private readonly IPTRViewRepository _pTRViewRepository;

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
            ITransducerTypeRepository transducerTypeRepository,
            IPTRViewRepository pTRViewRepository)
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
            _pTRViewRepository = pTRViewRepository;

            Title = this.GetType().Name;

            Init();
            LogIn();
            TestResult = -3;
        }

        //public Dictionary<string, ValidationItem> ValidationDict
        //{
        //    get => _validationDict;
        //    set
        //    {
        //        if (SetProperty(ref _validationDict, value))
        //        {
        //            // Notify that individual keys might have changed
        //            foreach (var key in _validationDict.Keys)
        //            {
        //                OnPropertyChanged($"ValidationDict[{key}]");
        //            }

        //            // If there are specific properties dependent on ValidationDict, notify them as well
        //            //OnPropertyChanged(nameof(CanTest));
        //            //SubmitCommand.RaiseCanExecuteChanged();
        //        }
        //    }
        //}

        //public Dictionary<int, Brush> BorderBackgrounds
        //{
        //    get => _borderBackgrounds;
        //    //set => SetProperty(ref _borderBackgrounds, value);
        //    set
        //    {
        //        if(SetProperty(ref _borderBackgrounds, value))
        //        {
        //            OnPropertyChanged(nameof(BorderBackgrounds));
        //            //foreach (var key in _borderBackgrounds.Keys)
        //            //{
        //            //    OnPropertyChanged($"BorderBackgrounds[{key}]");
        //            //}
        //        }
        //    }
        //}

        partial void OnProbeSnChanged(string value)
        {
            List<Test> tests;
            //정규 표현식 검증 추가
            if (value.Length > 10)
            {
                Log.Information($"Probe sn {value}");
                if (!IsExistsBySn(SnType.Probe, value))
                {
                    //ProbeSn Is Not Exist
                    //SetValidating(nameof(ProbeSn), "ProbeSn Is Not Exist");
                    ValidateField(nameof(ProbeSn), "ProbeSn Is Not Exist");
                    SetCellBackgrounds(TestCategories.All, Brushes.LightBlue);
                }
                else
                {
                    //ClearValidating(nameof(ProbeSn));
                    SetBySn(SnType.Probe, value);
                    ValidateField(nameof(ProbeSn));

                    tests = GetTestById(SnType.Probe, _probe!.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        SetCellPassFail(item, cellPosition);
                    }

                    tests = GetTestById(SnType.TransducerModule, _probe.TransducerModule.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        SetCellPassFail(item, cellPosition);
                    }

                    tests = GetTestById(SnType.Transducer, _probe.TransducerModule.Transducer.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        SetCellPassFail(item, cellPosition);
                    }
                }
            }
            else
            {
                ValidateField(nameof(ProbeSn), "ProbeSn Is Not Valid");
                SetCellBackgrounds(TestCategories.All, Brushes.LightBlue);
            }
        }
        
        partial void OnTDMdSnChanged(string value)
        {
            List<Test> tests;
            //정규 표현식 검증 추가
            if (value.Length > 10)
            {
                Log.Information($"TransducerModule sn {value}");
                if (!IsExistsBySn(SnType.TransducerModule, value))
                {
                    ValidateField(nameof(TDMdSn), "TDMdSn Is Not Exist");
                    SetCellBackgrounds(TestCategories.All, Brushes.LightBlue);
                }
                else
                {
                    SetBySn(SnType.TransducerModule, value);
                    ValidateField(nameof(TDMdSn));

                    tests = GetTestById(SnType.TransducerModule, _transducerModule!.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        SetCellPassFail(item, cellPosition);
                    }

                    tests = GetTestById(SnType.Transducer, _transducerModule.Transducer.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        SetCellPassFail(item, cellPosition);
                    }

                    IQueryable<Probe> query = _probeRepository.GetQueryable();
                    query = from probes in query
                            where probes.TransducerModuleId == _transducerModule.Id
                            orderby probes.Id descending
                            select probes;

                    Probe? probe = query.FirstOrDefault();

                    if (probe is null)
                        return;

                    //probe.Sn 에서 seqNo 파싱
                    string pattern = @"^.{0,11}([0-9]{3})$";
                    string getSeq = ExtractReqExr(probe.Sn, pattern);
                    //int seq = !string.IsNullOrEmpty(getSeq) ? int.Parse(getSeq) : 0;
                    if (!string.IsNullOrEmpty(getSeq))
                    {
                        SeqNo = getSeq;
                        ValidateField(nameof(SeqNo));
                    }

                    tests = GetTestById(SnType.Probe, probe.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        SetCellPassFail(item, cellPosition);
                    }
                }
            }
            else
            {
                ValidateField(nameof(TDMdSn), "TDMdSn Is Not Valid");
                SetCellBackgrounds(TestCategories.All, Brushes.LightBlue);
            }
        }

        partial void OnTDSnChanged(string value)
        {
            AddTDSnIsEnabled = false;
            ClearValidatingWaterMark();
            List<Test> tests;
            //정규 표현식 검증 추가
            if (value.Length > 10)
            {
                Log.Information($"Transducer sn {value}");
                if (!IsExistsBySn(SnType.Transducer, value))
                {
                    ValidateField(nameof(TDSn), "TDSn Is Not Exist");
                    AddTDSnIsEnabled = true;
                    SetCellBackgrounds(TestCategories.All, Brushes.LightBlue);
                }
                else
                {
                    SetBySn(SnType.Transducer, value);
                    ValidateField(nameof(TDSn));

                    tests = GetTestById(SnType.Transducer, _transducer!.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        SetCellPassFail(item, cellPosition);
                    }

                    IQueryable<TransducerModule> query = _transducerModuleRepository.GetQueryable();
                    query = from transducerModules in query
                            where transducerModules.TransducerId == _transducer.Id
                            orderby transducerModules.Id descending
                            select transducerModules;

                    TransducerModule? transducerModule = query.FirstOrDefault();

                    if (transducerModule is null)
                        return;

                    //transducerModule.Sn 에서 seqNo 파싱
                    //숫자 3자리로 끝남
                    string pattern = @"^.*(\d{3})$";
                    string getSeq = ExtractReqExr(transducerModule.Sn, pattern);
                    //int seq = !string.IsNullOrEmpty(getSeq) ? int.Parse(getSeq) : 0;
                    if (!string.IsNullOrEmpty(getSeq))
                    {
                        SeqNo = getSeq;
                        ValidateField(nameof(SeqNo));
                    }

                    tests = GetTestById(SnType.TransducerModule, transducerModule.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        SetCellPassFail(item, cellPosition);
                    }

                    IQueryable<Probe> queryProbe = _probeRepository.GetQueryable();
                    queryProbe = from probes in queryProbe
                                 where probes.TransducerModuleId == transducerModule.Id
                                 orderby probes.Id descending
                                 select probes;

                    Probe? probe = queryProbe.FirstOrDefault();

                    if (probe is null)
                        return;

                    tests = GetTestById(SnType.Probe, probe.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        SetCellPassFail(item, cellPosition);
                    }

                    ValidationDict[nameof(ProbeSn)].WaterMarkText = probe.Sn;
                    ValidationDict[nameof(TDMdSn)].WaterMarkText = transducerModule.Sn;
                    ValidationDict[nameof(TDSn)].WaterMarkText = value;
                    ValidationDict[nameof(MTMdSn)].WaterMarkText = probe.MotorModule.Sn;
                }
            }
            else
            {
                ValidateField(nameof(TDSn), "TDSn Is Not Valid");
                SetCellBackgrounds(TestCategories.All, Brushes.LightBlue);
            }


        }

        private bool CanAddTDSn()
        {
            Log.Information($"{nameof(CanAddTDSn)}");
            //return ValidationDict[nameof(TDSn)].IsValid!;
            if (ValidationDict[nameof(TDSn)].Message == "TDSn Is Not Exist")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanAddTDSn))]
        //[RelayCommand]
        private async Task AddTDSnAsync(string TDSn)
        {
            Log.Information($"{nameof(AddTDSnAsync)} TDSn:{TDSn}");
            if (TDSn.Length > 10)
            {
                //todo TDSn으로 타입 찾아내기
                Transducer td = new Transducer { Sn = TDSn, TransducerTypeId = 1 };
                bool res = await _transducerRepository.InsertAsync(td);
                if (res)
                {
                    AddTDSnIsEnabled = false;
                    ResLogs.Add($"Succ Add {TDSn}");
                    ValidateField("TDSn");
                    SeqNo = "";
                }
            }
        }
        
        partial void OnSeqNoChanged(string value)
        {
            string pattern = @"^[0-9]{3}$";
            bool isMatch = IsMatchReqExr(value, pattern);

            if (!isMatch)
            {
                ValidateField(nameof(SeqNo), "SeqNo Need 3 Digit");
            }
            else
            {
                ValidateField(nameof(SeqNo));
                switch (_testCategory)
                {
                    case TestCategories.Processing:
                        if (IsExistsBySn(SnType.TransducerModule, $"tdm-sn {SelectedDate.ToString("yyMMdd")} {SeqNo}"))
                        {
                            ValidateField(nameof(SeqNo), "SeqNo Need Is Duplicate");
                        }
                        break;
                    case TestCategories.Process:
                        if (IsExistsBySn(SnType.Probe, $"UPAG1{SelectedDate.ToString("yyMMdd")}{SeqNo}"))
                        {
                            ValidateField(nameof(SeqNo), "SeqNo Need Is Duplicate");
                        }
                        break;
                }
            }
        }

        partial void OnMTMdSnChanged(string value)
        {
            //정규 표현식 검증 추가
            if (value.Length > 10)
            {
                Log.Information($"MTMdSn sn {value}");
                if (!IsExistsBySn(SnType.MotorModule, value))
                {
                    ValidateField(nameof(MTMdSn), "MTMdSn Is Not Exist");
                }
                else
                {
                    SetBySn(SnType.MotorModule, value);
                    ValidateField(nameof(MTMdSn));
                }
            }
            else
            {
                ValidateField(nameof(MTMdSn), "MTMdSn Is Not Valid");
            }
        }

        private bool CanAddMTMdSn()
        {
            Log.Information($"{nameof(CanAddMTMdSn)}");

            if (ValidationDict[nameof(MTMdSn)].IsValid)
            {
                return true;
            }
            else
            {
                if (ValidationDict[nameof(MTMdSn)].Message == "MTMdSn Is Not Exist")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanAddMTMdSn))]
        //[RelayCommand]
        private async Task AddMTMdSnAsync(string MTMdSn)
        {
            Log.Information($"{nameof(AddMTMdSnAsync)} TDSn:{MTMdSn}");
            if (MTMdSn.Length > 10)
            {
                MotorModule motor = new MotorModule { Sn = MTMdSn };
                bool res = await _motorModuleRepository.InsertAsync(motor);
                if (res)
                {
                    ResLogs.Add($"Succ Add {MTMdSn}");
                }
            }
        }

        [RelayCommand]
        private void CellClick(CellPositions position)
        {

            int row = (int)position / 10;
            int col = (int)position % 10;
            Log.Information($"row:{row} col:{col}");
            _testCategory = (TestCategories)row;
            _testType = (TestTypes)col;
            Log.Information($"_testCategory : {_testCategory} _testType : {_testType}");
            if (_oldRow != row)
            {
                _oldRow = row;
                ClearAll();
                ClearValidatingWaterMark();
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

        //셀더블클릭
        [RelayCommand]
        private void CellDoubleClick(CellPositions position)
        {
            int row = (int)position / 10;
            int col = (int)position % 10;
            Log.Information($"row:{row} col:{col}");
            _testCategory = (TestCategories)row;
            _testType = (TestTypes)col;
            Test();
        }

        partial void OnTestResultChanged(int value)
        {
            Log.Information($"OnTestResultChanged : {value}");
        }

        [RelayCommand]
        private void CopyLog(string resLog)
        {
            Log.Information(nameof(CopyLog));
            if (!string.IsNullOrEmpty(resLog))
            {
                Clipboard.SetText(resLog);
            }
        }

        private bool CanTest()
        {
            Log.Information(nameof(CanTest));
            bool res = false;
            switch (_testCategory)
            {
                case TestCategories.Processing:
                    if (GetValidating(nameof(SeqNo)) &&
                        GetValidating(nameof(TDSn)))
                    {
                        res = true;
                    }
                    break;
                case TestCategories.Process:

                    if (GetValidating(nameof(SeqNo)) &&
                        GetValidating(nameof(TDMdSn)) &&
                        GetValidating(nameof(MTMdSn)))
                    {
                        res = true;
                    }
                    break;
                case TestCategories.Dispatch:
                    if (GetValidating(nameof(ProbeSn)))
                    {
                        res = true;
                    }
                    break;
            }

            return res;
        }

        [RelayCommand(CanExecute = nameof(CanTest))]
        private void Test()
        {
            //이미지 불러오기
            //이미지 불러오기
            //NextIsEnabled = true;
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

                    string s;
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

            ResLogs.Add($"Succ Test {ResTxt}");

            resImg = ConvertBitmapToImageSource(m_bmpRes);
            ResImg = (BitmapImage)resImg;

            TestResult = -2;
        }

        private bool CanNext()
        {
            Log.Information(nameof(CanNext));
            if (TestResult == -2)
            {
                return false;
            }
            
            bool res = false;
            switch (_testCategory)
            {
                case TestCategories.Processing:
                    if (GetValidating(nameof(SeqNo)) &&
                        GetValidating(nameof(TDSn)))
                    {
                        res = true;
                    }
                    break;
                case TestCategories.Process:

                    if (GetValidating(nameof(SeqNo)) &&
                        GetValidating(nameof(TDMdSn)) &&
                        GetValidating(nameof(MTMdSn)))
                    {
                        res = true;
                    }
                    break;
                case TestCategories.Dispatch:
                    if (GetValidating(nameof(ProbeSn)))
                    {
                        res = true;
                    }
                    break;
            }

            return res;
        }

        [RelayCommand(CanExecute = nameof(CanNext))]
        //[RelayCommand]
        private async Task NextAsync() 
        {
            Log.Information($"ValidateAll(_testCategory) : {ValidateAll(_testCategory)}");
            PTRView? tmpPTR;
            if (!ValidateAll(_testCategory)) return;

            //검사 결과 "" 체크
            
            Test insertTest = new Test
            {
                TestCategoryId = (int)_testCategory,
                TestTypeId = (int)_testType,
                TesterId = _tester.Id,
                OriginalImg = "오리지널 이미지",
                ChangedImg = "체인지 이미지",
                ChangedImgMetadata = ResTxt,
                Result = TestResult,
                Method = 1,
                CreatedDate = SelectedDate,
            };

            PrepareTest(_testCategory, insertTest);

            await SaveAsync(_testRepository, insertTest);
            
            //검사 결과 삭제
            ResTxt = "";

            CellPositions cellPosition = (CellPositions)((int)_testCategory * 10 + (int)_testType);
            SetCellPassFail(insertTest, cellPosition);

            bool existNext = false;
            //다음 공정에서 생성 할 sn 있는지 만 조회 하위 모듈 재사용 가능성 있음
            switch (_testCategory)
            {
                case TestCategories.Processing:
                    existNext = await IsExistsBySnAsync(SnType.TransducerModule, $"tdm-sn {SelectedDate.ToString("yyMMdd")} {SeqNo}");
                    break;
                case TestCategories.Process:
                    existNext = await IsExistsBySnAsync(SnType.Probe, $"UPAG1{SelectedDate.ToString("yyMMdd")}{SeqNo}");
                    break;
            }
            if (existNext)
                return;

            //sn으로 id 가져오기
            int id = 0;
            switch (_testCategory)
            {
                case TestCategories.Processing:
                    id = await GetBySnAsync(_testCategory, TDSn);
                    break;
                case TestCategories.Process:
                    id = await GetBySnAsync(_testCategory, TDMdSn);
                    break;
                case TestCategories.Dispatch:
                    id = await GetBySnAsync(_testCategory, ProbeSn);
                    break;
            }

            if (id == 0)
                return;

            //검사 결과 3개 생성& PASS 확인
            bool passAll = await PassTestCategoryAsync(_testRepository, _testCategory, id);
            if (!passAll && _pTRView == null)
                return;
            //다음 공정 시리얼 생성
            //tdmd, probe, 프로브 뷰에 삽입
            switch (_testCategory)
            {
                case TestCategories.Processing:
                    //tdmd 삽입 생성
                    TransducerModule tdMd = new TransducerModule { Sn = $"tdm-sn {SelectedDate.ToString("yyMMdd")} {SeqNo}",TransducerId = id};
                    if(await _transducerModuleRepository.InsertAsync(tdMd))
                    {
                        ResLogs.Add($"Add TDMd Sn : {tdMd.Sn}");
                    }
                    break;
                case TestCategories.Process:
                    //probe 삽입 생성
                    Probe probe = new Probe { Sn = $"UPAG1{SelectedDate.ToString("yyMMdd")}{SeqNo}", TransducerModuleId = id,MotorModuleId= _motorModule.Id };
                    if (await _probeRepository.InsertAsync(probe))
                    {
                        ResLogs.Add($"Add Probe Sn : {probe.Sn}");
                    }
                    // todo PTRView 뷰 아이디 조회 해서 추가
                    tmpPTR = await _probeRepository.GetPTRViewAsync(probe.Sn);
                    if (_pTRView is not null)
                        tmpPTR.Id = _pTRView.Id;
                    await _pTRViewRepository.UpsertAsync(tmpPTR);
                    break;
                case TestCategories.Dispatch:
                    //probeview 삽입 생성
                    tmpPTR = await _probeRepository.GetPTRViewAsync(ProbeSn);
                    if (_pTRView is not null)
                        tmpPTR.Id = _pTRView.Id;

                    await _pTRViewRepository.UpsertAsync(tmpPTR);
                    break;
            }

            //생성된 시리얼 알림


            //MessageBoxResult mbr = MessageBox.Show("Content:save??","Title:save", MessageBoxButton.YesNo);

            //switch (mbr)
            //{
            //    case MessageBoxResult.None:
            //        break;
            //    case MessageBoxResult.OK:
            //        break;
            //    case MessageBoxResult.Cancel:
            //        break;
            //    case MessageBoxResult.Yes:
            //        Log.Information("mbr yes");
            //        //디비 저장
            //        //화면 클리어
            //        NextIsEnabled = false;
            //        break;
            //    case MessageBoxResult.No:
            //        Log.Information("mbr no");
            //        break;
            //    default:
            //        break;
            //}
        }

        //public void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        [RelayCommand]
        public async Task KeyDownAsync(KeyEventArgs keyEventArgs)
        {
            Log.Information(keyEventArgs.Key.ToString());
            if (keyEventArgs.Key == Key.F10)
            {
                await NextAsync();
            }
        }

        private void Init()
        {
            ClearAll();

            ValidationDict[nameof(ProbeSn)] = new ValidationItem { WaterMarkText = $"{nameof(ProbeSn)}을 입력 하세요." };
            ValidationDict[nameof(TDMdSn)] = new ValidationItem { WaterMarkText = $"{nameof(TDMdSn)}을 입력 하세요." };
            ValidationDict[nameof(TDSn)] = new ValidationItem { WaterMarkText = $"{nameof(TDSn)}을 입력 하세요." };
            ValidationDict[nameof(SeqNo)] = new ValidationItem { WaterMarkText = $"{nameof(SeqNo)}을 입력 하세요." };
            ValidationDict[nameof(MTMdSn)] = new ValidationItem { WaterMarkText = $"{nameof(MTMdSn)}을 입력 하세요." };

            ResLogs = new ObservableCollection<string>();

            ret = VI.Load(0);

            // 이미지 파일 경로 설정
            string imagePath = "/Resources/sc_ori_img_512.bmp";

            // 이미지 로드
            SrcImg = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            ResImg = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
        }

        private async void LogIn()
        {
            Tester tester = new Tester { Name = "yoon", PcId = 1 };
            if (await _testerRepository.InsertAsync(tester))
            {
                IQueryable<Tester> query = _testerRepository.GetQueryable();
                query = from testers in query
                        where testers.PcId == 1 && testers.Name == "yoon"
                        orderby testers.Id descending
                        select testers;

                _tester = query.FirstOrDefault();
            }

            //꺼짐
        }

        // UI
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
                    SeqNoIsEnabled = true;
                    break;
                case TestCategories.Process:
                    TDMdSnIsEnabled = true;
                    MTMdSnIsEnabled = true;
                    SeqNoIsEnabled = true;
                    break;
                case TestCategories.Dispatch:
                    ProbeSnIsEnabled = true;
                    break;
            }
        }

        private void ClearAll()
        {
            ProbeSn = "";
            ProbeSnIsEnabled = false;
            TDMdSn = "";
            TDMdSnIsEnabled = false;
            TDSn = "";
            TDSnIsEnabled = false;
            AddTDSnIsEnabled = false;
            SeqNo = "";
            SeqNoIsEnabled = false;
            MTMdSn = "";
            MTMdSnIsEnabled = false;
            AddMTMdSnIsEnabled = false;
            SrcImg = default!;
            ResImg = default!;
            TestResult = -2;
            //TestIsEnabled = false;
            //NextIsEnabled = false;
            _probe = null;
            _transducerModule = null;
            _transducer = null;
            _motorModule = null;
            SetCellBackgrounds(TestCategories.All, Brushes.LightBlue);
        }

        private void ClearCellBackgrounds()
        {
            BorderBackgrounds[11] = Brushes.LightBlue;
            BorderBackgrounds[12] = Brushes.LightBlue;
            BorderBackgrounds[13] = Brushes.LightBlue;

            BorderBackgrounds[21] = Brushes.LightBlue;
            BorderBackgrounds[22] = Brushes.LightBlue;
            BorderBackgrounds[23] = Brushes.LightBlue;

            BorderBackgrounds[31] = Brushes.LightBlue;
            BorderBackgrounds[32] = Brushes.LightBlue;
            BorderBackgrounds[33] = Brushes.LightBlue;

            OnPropertyChanged(nameof(BorderBackgrounds));
        }

        private void SetCellPassFail(Test item, CellPositions cellPosition)
        {
            switch (item.TestTypeId)
            {
                case 1:
                    if (item.Result > App._testThresholdDict[11])
                    {
                        SetCellBackgrounds(Brushes.LightGreen, cellPosition);
                    }
                    else
                    {
                        SetCellBackgrounds(Brushes.Tomato, cellPosition);
                    }
                    break;
                case 2:
                    if (item.Result > App._testThresholdDict[12])
                    {
                        SetCellBackgrounds(Brushes.LightGreen, cellPosition);
                    }
                    else
                    {
                        SetCellBackgrounds(Brushes.Tomato, cellPosition);
                    }
                    break;
                case 3:
                    if (item.Result > App._testThresholdDict[13])
                    {
                        SetCellBackgrounds(Brushes.LightGreen, cellPosition);
                    }
                    else
                    {
                        SetCellBackgrounds(Brushes.Tomato, cellPosition);
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetCellBackgrounds(Brush brush, params CellPositions[] cellPositions)
        {
            foreach (CellPositions item in cellPositions)
            {
                _borderBackgrounds[(int)item] = brush;
            }

            OnPropertyChanged(nameof(BorderBackgrounds));
        }

        private void SetCellBackgrounds(TestCategories category, Brush brush)
        {
            switch (category)
            {
                case TestCategories.All:
                    BorderBackgrounds[(int)CellPositions.Row1_Column1] = brush;
                    BorderBackgrounds[(int)CellPositions.Row1_Column2] = brush;
                    BorderBackgrounds[(int)CellPositions.Row1_Column3] = brush;

                    BorderBackgrounds[(int)CellPositions.Row2_Column1] = brush;
                    BorderBackgrounds[(int)CellPositions.Row2_Column2] = brush;
                    BorderBackgrounds[(int)CellPositions.Row2_Column3] = brush;

                    BorderBackgrounds[(int)CellPositions.Row3_Column1] = brush;
                    BorderBackgrounds[(int)CellPositions.Row3_Column2] = brush;
                    BorderBackgrounds[(int)CellPositions.Row3_Column3] = brush;
                    break;
                case TestCategories.Processing:
                    BorderBackgrounds[(int)CellPositions.Row1_Column1] = brush;
                    BorderBackgrounds[(int)CellPositions.Row1_Column2] = brush;
                    BorderBackgrounds[(int)CellPositions.Row1_Column3] = brush;
                    break;
                case TestCategories.Process:
                    BorderBackgrounds[(int)CellPositions.Row2_Column1] = brush;
                    BorderBackgrounds[(int)CellPositions.Row2_Column2] = brush;
                    BorderBackgrounds[(int)CellPositions.Row2_Column3] = brush;
                    break;
                case TestCategories.Dispatch:
                    BorderBackgrounds[(int)CellPositions.Row3_Column1] = brush;
                    BorderBackgrounds[(int)CellPositions.Row3_Column2] = brush;
                    BorderBackgrounds[(int)CellPositions.Row3_Column3] = brush;
                    break;
                default:
                    break;
            }
        }

        // DB 관련

        private void SetBySn(SnType snType, string sn)// => snType switch
        {
            var query = _pTRViewRepository.GetQueryable();
            switch (snType)
            {
                case SnType.Probe:
                    _probe = _probeRepository.GetBySn(sn).OrderByDescending(x => x.Id).First();
                    query = from ptr in query
                            where ptr.ProbeSn == sn
                            orderby ptr.Id descending
                            select ptr;
                    break;
                case SnType.TransducerModule:
                    _transducerModule = _transducerModuleRepository.GetBySn(sn).OrderByDescending(x => x.Id).First();
                    query = from ptr in query
                            where ptr.TransducerModuleSn == sn
                            orderby ptr.Id descending
                            select ptr;
                    break;
                case SnType.Transducer:
                    _transducer = _transducerRepository.GetBySn(sn).OrderByDescending(x => x.Id).First();
                    query = from ptr in query
                            where ptr.TransducerSn == sn
                            orderby ptr.Id descending
                            select ptr;
                    break;
                case SnType.MotorModule:
                    _motorModule = _motorModuleRepository.GetBySn(sn).OrderByDescending(x => x.Id).First();
                    break;
                default:
                    break;
            }
            _pTRView = query.FirstOrDefault();
        }

        private bool IsExistsBySn(SnType snType, string sn)// => snType switch
        {
            return snType switch
            {
                SnType.Probe => _probeRepository.GetBySn(sn).Any(),
                SnType.TransducerModule => _transducerModuleRepository.GetBySn(sn).Any(),
                SnType.Transducer => _transducerRepository.GetBySn(sn).Any(),
                SnType.MotorModule => _motorModuleRepository.GetBySn(sn).Any(),
                _ => false
            };
        }

        private async Task<bool> IsExistsBySnAsync(SnType snType, string sn)
        {
            switch (snType)
            {
                case SnType.Probe:
                    var probes = await _probeRepository.GetBySn(sn).ToListAsync();
                    return probes.Any(); // Any() 메서드는 리스트에 요소가 있는지 여부를 반환합니다.
                case SnType.TransducerModule:
                    var transducerModules = await _transducerModuleRepository.GetBySn(sn).ToListAsync();
                    return transducerModules.Any();
                case SnType.Transducer:
                    var transducers = await _transducerRepository.GetBySn(sn).ToListAsync();
                    return transducers.Any();
                case SnType.MotorModule:
                    var motorModules = await _motorModuleRepository.GetBySn(sn).ToListAsync();
                    return motorModules.Any();
                default:
                    return false;
            }
        }
        
        private int GetBySn(TestCategories testCategory, string sn)
        {
            int res = 0;
            //IQueryable<Test> query = _testRepository.GetQueryable();
            switch (testCategory)
            {
                case TestCategories.Processing:
                    res = _transducerRepository.GetBySn(sn).FirstOrDefault()?.Id ?? 0;
                    break;
                case TestCategories.Process:
                    res = _transducerModuleRepository.GetBySn(sn).FirstOrDefault()?.Id ?? 0;
                    break;
                case TestCategories.Dispatch:
                    res = _probeRepository.GetBySn(sn).FirstOrDefault()?.Id ?? 0;
                    break;
            }
            return res;
        }

        private async Task<int> GetBySnAsync(TestCategories testCategory, string sn)
        {
            int res = 0;

            switch (testCategory)
            {
                case TestCategories.Processing:
                    var transducer = await _transducerRepository.GetBySn(sn).FirstOrDefaultAsync();
                    res = transducer?.Id ?? 0;
                    break;
                case TestCategories.Process:
                    var transducerModule = await _transducerModuleRepository.GetBySn(sn).FirstOrDefaultAsync();
                    res = transducerModule?.Id ?? 0;
                    break;
                case TestCategories.Dispatch:
                    var probe = await _probeRepository.GetBySn(sn).FirstOrDefaultAsync();
                    res = probe?.Id ?? 0;
                    break;
            }

            return res;
        }

        private bool PassTestCategory(ITestRepository testRepository, TestCategories testCategory, int id)
        {
            bool res = false;
            IQueryable<Test> query;
            switch (testCategory)
            {
                case TestCategories.Processing:
                    for (int i = 1; i < 4; i++)
                    {
                        query = from tests in _testRepository.GetQueryable()
                                where
                                        tests.TransducerId == id &&
                                        tests.TestCategoryId == 1 &&
                                        tests.TestTypeId == i &&
                                        tests.Result > App._testThresholdDict[10 + i]
                                orderby tests.Id descending
                                select tests;
                        if (!query.Any())
                        {
                            return false;
                        }
                    }
                    return true;
                case TestCategories.Process:
                    for (int i = 1; i < 4; i++)
                    {
                        query = from tests in _testRepository.GetQueryable()
                                where
                                        tests.TransducerId == id &&
                                        tests.TestCategoryId == 2 &&
                                        tests.TestTypeId == i &&
                                        tests.Result > App._testThresholdDict[20 + i]
                                orderby tests.Id descending
                                select tests;
                        if (!query.Any())
                        {
                            return false;
                        }
                    }
                    return true;
                case TestCategories.Dispatch:
                    for (int i = 1; i < 4; i++)
                    {
                        query = from tests in _testRepository.GetQueryable()
                                where
                                        tests.TransducerId == id &&
                                        tests.TestCategoryId == 2 &&
                                        tests.TestTypeId == i &&
                                        tests.Result > App._testThresholdDict[30 + i]
                                orderby tests.Id descending
                                select tests;
                        if (!query.Any())
                        {
                            return false;
                        }
                    }
                    return true;
                    //default:
                    //    break;
            }
            return res;
        }

        private async Task<bool> PassTestCategoryAsync(ITestRepository testRepository, TestCategories testCategory, int id)
        {
            IQueryable<Test> query;

            switch (testCategory)
            {
                case TestCategories.Processing:
                    for (int i = 1; i < 4; i++)
                    {
                        query = from tests in _testRepository.GetQueryable()
                                where tests.TransducerId == id &&
                                      tests.TestCategoryId == 1 &&
                                      tests.TestTypeId == i &&
                                      tests.Result > App._testThresholdDict[10 + i]
                                orderby tests.Id descending
                                select tests;

                        if (!await query.AnyAsync())
                        {
                            return false;
                        }
                    }
                    return true;

                case TestCategories.Process:
                    for (int i = 1; i < 4; i++)
                    {
                        query = from tests in _testRepository.GetQueryable()
                                where tests.TransducerModuleId == id &&
                                      tests.TestCategoryId == 2 &&
                                      tests.TestTypeId == i &&
                                      tests.Result > App._testThresholdDict[20 + i]
                                orderby tests.Id descending
                                select tests;

                        if (!await query.AnyAsync())
                        {
                            return false;
                        }
                    }
                    return true;

                case TestCategories.Dispatch:
                    for (int i = 1; i < 4; i++)
                    {
                        query = from tests in _testRepository.GetQueryable()
                                where tests.ProbeId == id &&
                                      tests.TestCategoryId == 3 &&
                                      tests.TestTypeId == i &&
                                      tests.Result > App._testThresholdDict[30 + i]
                                orderby tests.Id descending
                                select tests;

                        if (!await query.AnyAsync())
                        {
                            return false;
                        }
                    }
                    return true;

                default:
                    return false;
            }
        }

        // Save 메서드
        private async Task SaveAsync(ITestRepository testRepository, Test insertTest)
        {
            await testRepository.InsertAsync(insertTest);
        }

        /// <summary>
        /// 값이 없을떄 반환 값 체크
        /// </summary>
        /// <param name="snType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<Test> GetTestById(SnType snType, int id)
        {
            IQueryable<Test> query = _testRepository.GetQueryable();

            switch (snType)
            {
                case SnType.Probe:
                    query = from test in query
                            where test.ProbeId == id
                            orderby test.Id ascending
                            select test;
                    break;
                case SnType.TransducerModule:
                    query = from test in query
                            where test.TransducerModuleId == id
                            orderby test.Id ascending
                            select test;
                    break;
                case SnType.Transducer:
                    query = from test in query
                            where test.TransducerId == id
                            orderby test.Id ascending
                            select test;
                    break;
            }
            return query.ToList();
        }

        private static bool IsMatchReqExr(string value, string pattern)
        {
            // 정규식 패턴
            //string pattern = @"^[0-9]{4}$";
            // 정규식 객체 생성 및 컴파일 옵션 사용
            Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // 패턴 매칭 확인
            bool isMatch = regex.IsMatch(value);
            return isMatch;
        }

        private static string ExtractReqExr(string input, string pattern)
        {
            //string pattern = "^.{0,11}([0-9]{3})$";
            Match match = Regex.Match(input, pattern);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private void PrepareTest(TestCategories testCategory, Test insertTest)
        {
            switch (testCategory)
            {
                case TestCategories.Processing:
                    insertTest.TransducerId = _transducer!.Id;
                    break;
                case TestCategories.Process:
                    insertTest.TransducerModuleId = _transducerModule!.Id;
                    break;
                case TestCategories.Dispatch:
                    insertTest.ProbeId = _probe!.Id;
                    break;
            }
        }


        //private bool isNullText(string key, string value)
        //{
        //    if (string.IsNullOrWhiteSpace(value))
        //    {
        //        SetValidating(key, $"{key}: Is Required.");
        //        return true;
        //    }
        //    return false;
        //}

        // Validate
        private bool ValidateAll(TestCategories test)
        {
            // 필드 검증 로직 (검증 실패 시 즉시 종료)
            bool ValidateField(string key, string value)
            {
                // 필드 값이 비어있는지 검증하는 델리게이트
                var isNullText = (string key, string value) =>
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        SetValidating(key, $"{key}: Is Required.");
                        return true;
                    }
                    return false;
                };

                if (isNullText(key, value))
                    return false;

                if (ValidationDict.ContainsKey(key) && !ValidationDict[key].IsValid)
                    return false;

                return true;
            }

            // SelectedDate 필드 검증
            if (!ValidateField(nameof(SelectedDate), SelectedDate.ToString()))
                return false;

            // 각 TestCategory에 대한 필드 검증
            switch (test)
            {
                case TestCategories.Processing:
                    if (!ValidateField(nameof(TDSn), TDSn))
                        return false;
                    if (!ValidateField(nameof(SeqNo), SeqNo))
                        return false;
                    break;
                case TestCategories.Process:
                    if (!ValidateField(nameof(TDMdSn), TDMdSn))
                        return false;
                    if (!ValidateField(nameof(MTMdSn), MTMdSn))
                        return false;
                    if (!ValidateField(nameof(SeqNo), SeqNo))
                        return false;
                    break;
                case TestCategories.Dispatch:
                    if (!ValidateField(nameof(ProbeSn), ProbeSn))
                        return false;
                    break;
                default:
                    break;
            }

            // 모든 검증이 통과되었을 때만 true 반환
            return true;
        }

        private void ClearValidating(string key)
        {
            if (ValidationDict.ContainsKey(key))
            {
                ValidationDict[key].IsValid = true;
                ValidationDict[key].Message = string.Empty;
            }
            else
            {
                ValidationDict[key] = new ValidationItem { IsValid = true, Message = string.Empty };
            }
            OnPropertyChanged(nameof(ValidationDict));
        }

        private void SetValidating(string key, string message)
        {
            if (ValidationDict.ContainsKey(key))
            {
                ValidationDict[key].IsValid = false;
                ValidationDict[key].Message = message;
            }
            else
            {
                ValidationDict[key] = new ValidationItem { IsValid = false, Message = message };
            }
            OnPropertyChanged(nameof(ValidationDict));
        }

        private void SetValidatingWaterMark(string key, string waterMarkText)
        {
            if (ValidationDict.ContainsKey(key))
            {
                ValidationDict[key].WaterMarkText = waterMarkText;
            }
            else
            {
                ValidationDict[key] = new ValidationItem { WaterMarkText = waterMarkText };
            }
        }

        private void ClearValidatingWaterMark()
        {
            foreach (var item in ValidationDict)
            {
                item.Value.WaterMarkText = $"{item.Key}를 입력하세요.";
            }
            OnPropertyChanged(nameof(ValidationDict));
        }

        // Example of using the validation methods
        public bool GetValidating(string key)
        {
            if (ValidationDict.ContainsKey(key))
            {
                OnPropertyChanged(nameof(ValidationDict));
                return ValidationDict[key].IsValid;
            }
            OnPropertyChanged(nameof(ValidationDict));
            return false;
        }

        public void ValidateField(string key, string value = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ClearValidating(key);
            }
            else
            {
                SetValidating(key, value);
            }
        }

        // Example method to set specific validation messages
        private void SetSpecificValidationText(string key)
        {
            switch (key)
            {
                case nameof(ProbeSn):
                    SetValidating(key, "ProbeSn Is Not Exist.");
                    break;
                case "Email":
                    SetValidating(key, "Email을 입력하세요.");
                    break;
                case "ExistEmail":
                    SetValidating(key, "이미 존재하는 Email입니다.");
                    break;
                case "Nickname":
                    SetValidating(key, "닉네임을 입력하세요.");
                    break;
                case "CellPhone":
                    SetValidating(key, "휴대전화번호를 입력하세요.");
                    break;
                case "Password":
                    SetValidating(key, "비밀번호를 입력하세요.");
                    break;
                case "PasswordConfirm":
                    SetValidating(key, "비밀번호 확인를 입력하세요.");
                    break;
                case "DifferentPassword":
                    SetValidating(key, "비밀번호와 재입력 값이 일치하지 않습니다.");
                    break;
            }
        }

        // 이미지 프로세싱 관련
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

        private ImageSource ConvertBitmapToImageSource(Bitmap bitmap)
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

        private int convertBitmapToByteArr(byte[] result, Bitmap bitmap)
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

        private int convertByteArrToBitmap(byte[] raw_img, Bitmap m_bmp)
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
