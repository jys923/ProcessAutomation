//#define SET_MOTOR

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Services;
using SonoCap.MES.Services.Interfaces;
using SonoCap.WpfCommons;
using SonoCap.MES.UI.Services;
using SonoCap.MES.UI.Services.Interfaces;
using SonoCap.MES.UI.Validation;
using SonoCap.MES.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using SonoCap.MES.UI.Messages;
using SonoCap.MES.UI.Models;
using SonoCap.MES.UI.Services.TestStrategies;

namespace SonoCap.MES.UI.ViewModels
{
    public class SubSetting { public int Id { get; set; } public string Name { get; set; } }

    public partial class TestingViewModel : ViewModelBase, IParameterReceiver
    {
        private Transducer? _transducer { get; set; } = default!;
        private TransducerModule? _transducerModule { get; set; } = default!;
        private MotorModule? _motorModule { get; set; } = default!;
        private Probe? _probe { get; set; } = default!;
        private PTRView? _pTRView { get; set; } = default!;
        private TestCategories _testCategory { get; set; } = default!;
        private TestTypes _testType { get; set; } = default!;
        private Tester? _tester { get; set; } = default!;
        private Test? _test { get; set; } = default!;

        private MES.Services.Model.GlobalModel _model;
        private readonly TestingManagementService _testingManagementService;
        private readonly IMotorService _motorService;
        private readonly ICellStatusService _cellStatusService;
        private readonly ImageService _imageService;
        private readonly IViewService _viewService;
        private readonly ImageBufferService _imageBufferService;
        private readonly ITestMemoryService _testMemoryService;
        private readonly ISnFlowService _snFlowService;

        public TestingViewModel(
            ITestMemoryService testMemoryService,
            ISnFlowService snFlowService,
            USRenderService usRenderer,
            ImageBufferService imageBufferService,
            ImageService imageService,
            IViewService viewService,
            TestingManagementService testingManagementService,
            MES.Services.Model.GlobalModel model,
            IMotorService motorService,
            ICellStatusService cellStatusService)
        {
            _testMemoryService = testMemoryService;
            _snFlowService = snFlowService;
            _usRenderer = usRenderer;
            _imageBufferService = imageBufferService;
            _imageService = imageService;
            _viewService = viewService;
            _testingManagementService = testingManagementService;
            _model = model;
            _motorService = motorService;
            _cellStatusService = cellStatusService;

            Title = this.GetType().Name;

            Init();
            LogIn();
        }

        private void Init()
        {
            InitHsn();
            //InitUI();
            InitValidation();
            InitImg();
            InitTimer();
            string refImgPath = "Resources/refImg.bmp";
            MyOpenCVWrapper.OpenCVWrapper.SetReferenceImage(refImgPath);
            processFunction = MyOpenCVWrapper.OpenCVWrapper.EnvGeoInspection;
            inspectionFunction = MyOpenCVWrapper.OpenCVWrapper.RunInspection;
        }

        private void InitUI()
        {
            ApplicationList = new ObservableCollection<Tuple<int, string>>(_model.Applications);
            SelectedApplication = ApplicationList.FirstOrDefault(x => x.Item1 == _model.Application)
                                 ?? ApplicationList.FirstOrDefault()
                                 ?? new Tuple<int, string>(0, string.Empty);
            Log.Information($"ApplicationList 초기화 완료, SelectedApplication: {SelectedApplication?.Item2 ?? "null"}");

            PresetList = new ObservableCollection<Tuple<int, string>>(_model.Presets);
            SelectedPreset = PresetList.FirstOrDefault(x => x.Item1 == _model.Setting)
                             ?? PresetList.FirstOrDefault()
                             ?? new Tuple<int, string>(0, string.Empty);
            Log.Information($"PresetList 초기화 완료, SelectedPreset: {SelectedPreset?.Item2 ?? "null"}");

            // 허용할 파일명 목록
            var allowList = new HashSet<string>
            {
                "pen.json", "gen.json", "res.json",
                "pen_fh.json", "gen_fh.json", "res_fh.json"
            };

            // 필터링 적용
            var filtered = _model.SubSettings
                .Where(x => allowList.Contains(x.Item2));

            // ObservableCollection 생성
            SubSettingList = new ObservableCollection<Tuple<int, string>>(filtered);
            SelectedSubSetting = SubSettingList.FirstOrDefault(x => x.Item1 == _model.Subsetting)
                                 ?? SubSettingList.FirstOrDefault()
                                 ?? new Tuple<int, string>(0, string.Empty);

            //SrcImg = Utilities.GetFileToImageSource("Resources/usImg.bmp") ?? Utilities.LoadBitmapFromResource("usImg.bmp");

            if (_depthToScanlineMap.TryGetValue(_model.ViewDepthCm, out int scanlineValue))
            {
                _usRenderer?.SetScanline(scanlineValue);
            }

            SelectedViewDepth = _model.ViewDepthCm;
            SelectedLineDensity = _model.LineDensity;
            IPGain = _model.IPGain;
            DRMin = _model.DRMin;
            DRMax = _model.DRMax;
            SelectedPower = _model.TxPower;
        }

        private void InitValidation()
        {
            DepthIsEnabled.Add(0, new ValidationItem { IsEnabled = true });
            DepthIsEnabled.Add(1, new ValidationItem { IsEnabled = true });
            DepthIsEnabled.Add(2, new ValidationItem { IsEnabled = true });
            DepthIsEnabled.Add(3, new ValidationItem { IsEnabled = true });
            DepthIsEnabled.Add(4, new ValidationItem { IsEnabled = true });

            LineDensityIsEnabled.Add(0, new ValidationItem { IsEnabled = true });
            LineDensityIsEnabled.Add(1, new ValidationItem { IsEnabled = true });
            LineDensityIsEnabled.Add(2, new ValidationItem { IsEnabled = true });
            LineDensityIsEnabled.Add(3, new ValidationItem { IsEnabled = true });

            ValidationDict[nameof(TDSn)] = new ValidationItem { WaterMarkText = $"{nameof(TDSn)}을 입력 하세요.", IsEnabled = true };
            ValidationDict[nameof(TDMdSn)] = new ValidationItem { WaterMarkText = $"{nameof(TDMdSn)}을 입력 하세요." };
            ValidationDict[nameof(MTMdSn)] = new ValidationItem { WaterMarkText = $"{nameof(MTMdSn)}을 입력 하세요." };
            ValidationDict[nameof(ProbeSn)] = new ValidationItem { WaterMarkText = $"{nameof(ProbeSn)}을 입력 하세요." };
            ValidationDict[nameof(TestResult)] = new ValidationItem { };

            TestResult = -2;

            //SetCellBackgrounds(TestCategories.All, Brushes.LightGray);
            _cellStatusService.SetCategoryDefault(
                TestCategories.All,
                (pos, brush) => BorderBackgrounds[(int)pos] = new ObservableBrush { Value = brush }
            );
        }

        private void InitTimer()
        {
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var timer = new System.Timers.Timer(1000);//1s
            timer.Elapsed += (s, e) => CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            timer.Start();
        }
        private void InitImg()
        {
            _defaultImg = Utilities.LoadBitmapFromResource("refImg.bmp");

            // 이미지 파일 경로 설정
            string imagePath = "Resources/refImg.bmp";

            // 이미지 로드
            //SrcImg = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            //ResImg = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            SrcImg = Utilities.GetFileToImageSource(imagePath) ?? _defaultImg;
            ResImg = Utilities.GetFileToImageSource(imagePath) ?? _defaultImg;
        }
        private void InitHsn()
        {
            //_model = new GlobalModel();
            if (!_model.InitializeLibrary())
            {
                MessageBox.Show("initialize library failure");
                //this.Close();
                return;
            }
            _model.MotorStateChanged += _motorService.OnMotorStateChanged;
            _model.IpCapsuleIsInnerVisible = true;

            _model.OnInitUI += InitUI;

            //_selectedSubSetting = _model.Subsetting;
            //List<Tuple<int, string>> subSettingList = _model.SubSettings;

            //SubSettingList = new ObservableCollection<Tuple<int, string>>(subSettingList);

            //SelectedSubSetting = subSettingList.Find(tuple => tuple.Item1 == _model.Subsetting);

            //string targetString = "res_fh";
            //Tuple<int, string>? result = subSettingList.Find(tuple => tuple.Item2.Contains(targetString));
            //_model.Subsetting = result.Item1;

            RenderStart();
        }

        private async void LogIn()
        {
            _tester = await _testingManagementService.InsertAndRetrieveTesterAsync(CreateTester());
        }

        private Tester CreateTester()
        {
            return new Tester
            {
                Name = App.appSettings.TesterName,
                PcId = App.appSettings.PcId
            };
        }

        private bool InitMotor()
        {
            //_motorService.CloseViewRequested += CloseViewRequestedHandler;
            if (_motorService.InitializeMotor())
            {
                //_motorService.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceivedHandler);
                Log.Information($"Succ:{nameof(InitMotor)}");
                return true;
            }
            else
            {
                Log.Error($"{nameof(InitMotor)}");
                return false;
            }
        }

        private Action<IntPtr, int, int, IntPtr, IntPtr> processFunction = default!;
        private Action<IntPtr, int, int, IntPtr, IntPtr, int> inspectionFunction = default!;

        [ObservableProperty]
        private string _title = default!;

        [ObservableProperty]
        private string _message = default!;

        [ObservableProperty]
        private bool _messageIsPopupOpen = false;


        [ObservableProperty]
        private string _currentTime = DateTime.Today.ToString("yyyy-MM-dd");

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TestCommand))]
        private ObservableDictionary<string, ValidationItem> _validationDict = new();

        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(TestCommand))]
        private bool _tdCellIsEnabled = false;

        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(TestCommand))]
        private bool _tdMdCellIsEnabled = false;

        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(TestCommand))]
        private bool _probeCellIsEnabled = false;

        [ObservableProperty]
        private ObservableDictionary<int, ObservableBrush> _borderBackgrounds = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TestCommand))]
        private int _blinkingCellIndex = (int)CellPositions.Row0_Column0;

        partial void OnBlinkingCellIndexChanged(int value)
        {
            Log.Information($"[DEBUG] BlinkingCellIndex changed to {value}");
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TestCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private string _tDSn = string.Empty;

        [ObservableProperty]
        private bool _tDSnIsPopupOpen = false;

        [ObservableProperty]
        private int _tDSnSelectedIndex = -1;

        [ObservableProperty]
        private ObservableCollection<string> _tDSnFilteredItems = new();

        private void TDSnFilterItems()
        {
            if (string.IsNullOrWhiteSpace(TDSn))
            {

                TDSnFilteredItems.Clear();
            }
            else
            {
                List<string> items = _testingManagementService.GetFilteredSn(SnType.Transducer, TDSn);

                TDSnFilteredItems = new ObservableCollection<string>(items);
            }
        }

        [RelayCommand]
        private void TDSnKeyDown(KeyEventArgs e)
        {
            if (e == null) return;

            //Log.Information($"TDSnKeyDown : {e.Key}");
            if (e.Key == Key.Down)
            {
                if (TDSnFilteredItems.Count > 0)
                {
                    TDSnSelectedIndex = (TDSnSelectedIndex + 1) % TDSnFilteredItems.Count;
                }
            }
            else if (e.Key == Key.Up)
            {
                if (TDSnFilteredItems.Count > 0)
                {
                    TDSnSelectedIndex = (TDSnSelectedIndex - 1 + TDSnFilteredItems.Count) % TDSnFilteredItems.Count;
                }
            }
            else if (e.Key == Key.Enter)
            {
                if (TDSnSelectedIndex >= 0 && TDSnSelectedIndex < TDSnFilteredItems.Count)
                {
                    TDSn = TDSnFilteredItems[TDSnSelectedIndex];
                    TDSnIsPopupOpen = false;
                }
            }
            else if (e.Key == Key.Tab)
            {
                TDSnIsPopupOpen = false;
            }
        }

        [RelayCommand]
        private void TDSnFilteredItemsMouseDoubleClick(string selectedItem)
        {
            Log.Information($"TDSnFilteredItemsMouseDoubleClick : {selectedItem}");
            TDSn = selectedItem;
            TDSnIsPopupOpen = false;
        }

        //[NotifyCanExecuteChangedFor(nameof(TestCommand))]
        //[NotifyCanExecuteChangedFor(nameof(NextCommand))]
        [ObservableProperty]
        private string _tDMdSn = string.Empty;

        [ObservableProperty]
        private bool _tDMdSnIsPopupOpen = false;

        [ObservableProperty]
        private int _tDMdSnSelectedIndex = -1;

        [ObservableProperty]
        private ObservableCollection<string> _tDMdSnFilteredItems = new();


        [RelayCommand]
        private void TDMdSnKeyDown(KeyEventArgs e)
        {
            if (e == null) return;

            Log.Information($"TDMdSnKeyDown : {e.Key}");
            if (e.Key == Key.Down)
            {
                if (TDMdSnFilteredItems.Count > 0)
                {
                    TDMdSnSelectedIndex = (TDMdSnSelectedIndex + 1) % TDMdSnFilteredItems.Count;
                }
            }
            else if (e.Key == Key.Up)
            {
                if (TDMdSnFilteredItems.Count > 0)
                {
                    TDMdSnSelectedIndex = (TDMdSnSelectedIndex - 1 + TDMdSnFilteredItems.Count) % TDSnFilteredItems.Count;
                }
            }
            else if (e.Key == Key.Enter)
            {
                if (TDMdSnSelectedIndex >= 0 && TDMdSnSelectedIndex < TDMdSnFilteredItems.Count)
                {
                    TDMdSn = TDMdSnFilteredItems[TDMdSnSelectedIndex];
                    TDMdSnIsPopupOpen = false;
                }
            }
            else if (e.Key == Key.Tab)
            {
                TDMdSnIsPopupOpen = false;
            }
        }

        [ObservableProperty]
        private string _mTMdSn = string.Empty;

        [ObservableProperty]
        private bool _mTMdSnIsPopupOpen = false;

        [ObservableProperty]
        private int _mTMdSnSelectedIndex = -1;

        [ObservableProperty]
        private ObservableCollection<string> _mTMdSnFilteredItems = new();

        [RelayCommand]
        private void MTMdSnKeyDown(KeyEventArgs e)
        {
            if (e == null) return;

            Log.Information($"MTMdSnKeyDown : {e.Key}");
            if (e.Key == Key.Down)
            {
                if (MTMdSnFilteredItems.Count > 0)
                {
                    MTMdSnSelectedIndex = (MTMdSnSelectedIndex + 1) % MTMdSnFilteredItems.Count;
                }
            }
            else if (e.Key == Key.Up)
            {
                if (MTMdSnFilteredItems.Count > 0)
                {
                    MTMdSnSelectedIndex = (MTMdSnSelectedIndex - 1 + MTMdSnFilteredItems.Count) % MTMdSnFilteredItems.Count;
                }
            }
            else if (e.Key == Key.Enter)
            {
                if (MTMdSnSelectedIndex >= 0 && MTMdSnSelectedIndex < MTMdSnFilteredItems.Count)
                {
                    MTMdSn = MTMdSnFilteredItems[MTMdSnSelectedIndex];
                    MTMdSnIsPopupOpen = false;
                }
            }
            else if (e.Key == Key.Tab)
            {
                MTMdSnIsPopupOpen = false;
            }
        }

        //[NotifyCanExecuteChangedFor(nameof(TestCommand))]
        //[NotifyCanExecuteChangedFor(nameof(NextCommand))]
        [ObservableProperty]
        private string _probeSn = string.Empty;

        [ObservableProperty]
        private bool _probeSnIsPopupOpen = false;

        [ObservableProperty]
        private int _probeSnSelectedIndex = -1;

        [ObservableProperty]
        private ObservableCollection<string> _probeSnFilteredItems = new();

        [RelayCommand]
        private void ProbeSnKeyDown(KeyEventArgs e)
        {
            if (e == null) return;

            Log.Information($"ProbeSnKeyDown : {e.Key}");
            if (e.Key == Key.Down)
            {
                if (ProbeSnFilteredItems.Count > 0)
                {
                    ProbeSnSelectedIndex = (ProbeSnSelectedIndex + 1) % ProbeSnFilteredItems.Count;
                }
            }
            else if (e.Key == Key.Up)
            {
                if (ProbeSnFilteredItems.Count > 0)
                {
                    ProbeSnSelectedIndex = (ProbeSnSelectedIndex - 1 + ProbeSnFilteredItems.Count) % ProbeSnFilteredItems.Count;
                }
            }
            else if (e.Key == Key.Enter)
            {
                if (ProbeSnSelectedIndex >= 0 && ProbeSnSelectedIndex < ProbeSnFilteredItems.Count)
                {
                    ProbeSn = ProbeSnFilteredItems[ProbeSnSelectedIndex];
                    ProbeSnIsPopupOpen = false;
                }
            }
            else if (e.Key == Key.Tab)
            {
                ProbeSnIsPopupOpen = false;
            }
        }

        [ObservableProperty]
        private ObservableDictionary<int, ValidationItem> _depthIsEnabled = new();

        // ListViewDepth 초기화
        //[ObservableProperty]
        //public IList<double> _listViewDepth = new List<double> { 3, 4, 5, 6, 7 };

        private static readonly Dictionary<double, int> _depthToScanlineMap = new Dictionary<double, int>
        {
            { 7, 480 },
            { 6, 576 },
            { 5, 720 },
            { 4, 768 },
            { 3, 960 }
        };

        public double SelectedViewDepth
        {
            get => _model.ViewDepthCm;
            set
            {
                _model.ViewDepthCm = value;
                if (_depthToScanlineMap.TryGetValue(value, out int scanlineValue))
                {
                    _usRenderer?.SetScanline(scanlineValue);
                }
                else
                {
                    Log.Warning($"Warning: No scanline mapping found for depth: {value}");
                    // _usRenderer?.SetScanline(기본_값_또는_계산된_값);
                }
                OnPropertyChanged();
            }
        }

        [ObservableProperty]
        private ObservableDictionary<int, ValidationItem> _lineDensityIsEnabled = new();
        // ListViewDepth 초기화
        //[ObservableProperty]
        //public IList<int> _listLineDensity = new List<int> { 1, 2, 3, 4};

        //[ObservableProperty]
        //private double _selectedLineDensity;
        public int SelectedLineDensity
        {
            get { return _model.LineDensity; }
            set
            {
                _model.LineDensity = value;
                OnPropertyChanged(nameof(SelectedLineDensity));
                //OnSelectedLineDensityChanged(value);
            }
        }

        public double SelectedPower
        {
            get { return _model.TxPower; }
            set
            {
                _model.TxPower = value;
                OnPropertyChanged(nameof(SelectedPower));
            }
        }

        public float DRMin
        {
            get { return _model.DRMin; }
            set
            {
                if (value <= DRMax - 2)
                {
                    _model.DRMin = value;
                    _usRenderer.DRMin = value;
                    OnPropertyChanged(nameof(DRMin));
                }
            }
        }

        public float DRMax
        {
            get { return _model.DRMax; }
            set
            {
                if (value >= DRMin + 2)
                {
                    _model.DRMax = value;
                    _usRenderer.DRMax = value;
                    OnPropertyChanged(nameof(DRMax));
                }
            }
        }

        public int IPGain
        {
            get { return _model.IPGain; }
            set
            {
                _model.IPGain = value;
                OnPropertyChanged(nameof(IPGain));
            }
        }

        [ObservableProperty]
        private int _calibrationOffset = 0;

        [RelayCommand]
        private void CalibrateButton()
        {
            Log.Information($"{nameof(CalibrateButton)}");
            //_model.CalibrateScanline(CalibrationOffset);
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private int _testResult = -2;

        private BitmapImage _defaultImg = default!;

        [ObservableProperty] private ObservableCollection<Tuple<int, string>> _applicationList = default!;
        [ObservableProperty] private Tuple<int, string> _selectedApplication = default!;
        [ObservableProperty] private ObservableCollection<Tuple<int, string>> _presetList = default!;
        [ObservableProperty] private Tuple<int, string> _selectedPreset = default!;
        [ObservableProperty] private ObservableCollection<Tuple<int, string>> _subSettingList = default!;
        [ObservableProperty] private Tuple<int, string> _selectedSubSetting = default!;

        partial void OnSelectedApplicationChanged(Tuple<int, string> value)
        {
            if (value == null) return;
            if (_model.Application != value.Item1)
            {
                _model.Application = value.Item1;
                InitUI();
            }
        }

        partial void OnSelectedPresetChanged(Tuple<int, string> value)
        {
            if (value == null) return;
            if (_model.Setting != value.Item1)
            {
                _model.Setting = value.Item1;
                InitUI();
            }
        }

        // Events
        partial void OnSelectedSubSettingChanged(Tuple<int, string> value)
        {
            if (value == null) return;
            if (_model.Subsetting != value.Item1)
            {
                _model.Subsetting = value.Item1;
                InitUI();
            }
        }

        [ObservableProperty]
        private ImageSource _snapshotImg = default!;

        [ObservableProperty]
        private ImageSource _srcImg = default!;

        [ObservableProperty]
        private ImageSource _envImg = default!;

        [ObservableProperty]
        private bool _isEnvImgVisible = false;

        [ObservableProperty]
        private ImageSource _resImg = default!;

        //[ObservableProperty]
        //private string _resTxt = default!;

        private string _resTxt = default!;
        public string ResTxt
        {
            get => _resTxt;
            set
            {
                _resTxt = Utilities.FormatJson(value);
                OnPropertyChanged(nameof(ResTxt));
            }
        }

        [ObservableProperty]
        private TimeStampedObservableCollection<string> _resLogs = new TimeStampedObservableCollection<string>();

        [ObservableProperty]
        private string _selectedLogItem = default!;

        [RelayCommand]
        public void KeyDown(KeyEventArgs keyEventArgs)
        {
            Key key = keyEventArgs.Key == Key.System ? keyEventArgs.SystemKey : keyEventArgs.Key;
            Log.Information($"{nameof(KeyDown)} key: {key}");

            if (key == Key.Left)
            {
                _rotationAngle = (_rotationAngle - 1 + 360) % 360;
                _usRenderer?.SetRotationAngle(_rotationAngle);
                Log.Information($"[Rotate] angle → {_rotationAngle}° (←)");
                keyEventArgs.Handled = true;
            }
            else if (key == Key.Right)
            {
                _rotationAngle = (_rotationAngle + 1) % 360;
                _usRenderer?.SetRotationAngle(_rotationAngle);
                Log.Information($"[Rotate] angle → {_rotationAngle}° (→)");
                keyEventArgs.Handled = true;
            }
            else if (key == Key.Up)
            {
                _usRenderer?.SetVerticalFlip(true);
                Log.Information($"[Flip Vertical] → true (↑)");
                keyEventArgs.Handled = true;
            }
            else if (key == Key.Down)
            {
                _usRenderer?.SetVerticalFlip(false);
                Log.Information($"[Flip Vertical] → false (↓)");
                keyEventArgs.Handled = true;
            }
        }

        [RelayCommand]
        private Task CellClickAsync(CellPositions position)
        {
            var cell = TestCellMap.Items.FirstOrDefault(x => x.Position == position);
            if (cell == null)
                return Task.CompletedTask;

            _testCategory = cell.Category;
            _testType = cell.TestType;

            IsEnvImgVisible = cell.UsesEnvImage;
            BlinkingCellIndex = (int)position;

            // Reset basic UI
            ResImg = default!;
            TestResult = -2;
            ValidationDict[nameof(TestResult)].IsEnabled = false;
            TDSnIsPopupOpen = false;

            (TestCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
            return Task.CompletedTask;
        }

        [RelayCommand]
        private void ForcePass(CellPositions position)
        {
            Log.Information($"{nameof(ForcePass)} click {position}");
            int row = (int)position / 10;
            _testCategory = (TestCategories)row;
            bool proceed = Controls.AlertBox.Show("통합 검사", "통합 검사 미지원");
            if (!proceed)
            {
                ResLogs.Add("통합 검사 취소");
                return;
            }
        }

        async partial void OnTDSnChanged(string value)
        {
            TDSnFilterItems();
            TDSnIsPopupOpen = !string.IsNullOrEmpty(value) && TDSnFilteredItems.Any();

            ValidationService.ClearValidatingWaterMark(ValidationDict);

            var result = await _snFlowService.ProcessAsync(value);

            // 초기화
            _probe = result.Probe;
            _transducerModule = result.TransducerModule;
            _transducer = result.Transducer;
            _motorModule = result.MotorModule;
            _pTRView = null; // 필요 시 서비스에서 제공 가능

            TdCellIsEnabled = result.TdEnabled;
            TdMdCellIsEnabled = result.TdMdEnabled;
            ProbeCellIsEnabled = result.ProbeEnabled;

            // 전체 초기 색깔 리셋
            _cellStatusService.SetCategoryDefault(
                TestCategories.All,
                (pos, brush) => BorderBackgrounds[(int)pos] = new ObservableBrush { Value = brush }
            );

            if (!result.IsValid)
            {
                ValidationService.ValidateField(ValidationDict, nameof(TDSn), "TDSn Is Not Exist");
                return;
            } else{                
                ValidationService.ValidateField(ValidationDict, nameof(TDSn));
            }   

            // 1) Transducer Tests
            foreach (var t in result.TransducerTests)
                _cellStatusService.SetCellPassFail(t, (pos, b)
                    => BorderBackgrounds[(int)pos] = new ObservableBrush { Value = b });

            // 2) TransducerModule Tests
            foreach (var t in result.TransducerModuleTests)
                _cellStatusService.SetCellPassFail(t, (pos, b)
                    => BorderBackgrounds[(int)pos] = new ObservableBrush { Value = b });

            // 3) Probe Tests
            foreach (var t in result.ProbeTests)
                _cellStatusService.SetCellPassFail(t, (pos, b)
                    => BorderBackgrounds[(int)pos] = new ObservableBrush { Value = b });

            // Watermark
            if (result.TDMdWatermark != null)
            {
                ValidationDict[nameof(TDMdSn)].IsEnabled = false;
                ValidationDict[nameof(TDMdSn)].WaterMarkText = result.TDMdWatermark;
            }

            if (result.ProbeSnWatermark != null)
            {
                ValidationDict[nameof(ProbeSn)].IsEnabled = false;
                ValidationDict[nameof(ProbeSn)].WaterMarkText = result.ProbeSnWatermark;
            }

            if (result.MotorMdWatermark != null)
            {
                ValidationDict[nameof(MTMdSn)].IsEnabled = false;
                ValidationDict[nameof(MTMdSn)].WaterMarkText = result.MotorMdWatermark;
            }
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
            List<int> validIndices = new List<int>
            {
                (int)CellPositions.Row1_Column1, (int)CellPositions.Row1_Column2, (int)CellPositions.Row1_Column3,
                (int)CellPositions.Row2_Column1, (int)CellPositions.Row2_Column2, (int)CellPositions.Row2_Column3,
                (int)CellPositions.Row3_Column1, (int)CellPositions.Row3_Column2, (int)CellPositions.Row3_Column3
            };
            return validIndices.Contains(BlinkingCellIndex) && ValidationService.GetValidating(ValidationDict, nameof(TDSn));
        }

        // ViewModel (예: TestingViewModel.cs)
        [RelayCommand(CanExecute = nameof(CanTest))]
        private Task TestAsync()
        {
            Log.Information($"TestAsync response");

            // 1. 공통 준비 단계
            TestContext context = PrepareTestContext();

            if (context == null)
            {
                Log.Error("Test context preparation failed.");
                ResLogs.Add("Test context preparation failed.");
                return Task.CompletedTask;
            }

            // 2. 전략 패턴 실행 (핵심 로직)
            try
            {
                // ViewModel에 의존하지 않는 전략 객체 생성
                ITestStrategy strategy = GetTestStrategy(context.TestType);
                strategy.Execute(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred during test execution.");
                ResLogs.Add($"Error: {ex.Message}");
                FinalizeTestContext(context);
                return Task.CompletedTask;
            }

            // 3. 공통 정리 단계
            FinalizeTestContext(context);

            ValidationDict[nameof(TestResult)].IsEnabled = true;
            return Task.CompletedTask;
        }

        // 공통 준비 함수: 이미지 선택 및 메모리 할당까지 완료하여 완전한 컨텍스트를 반환합니다.
        private TestContext PrepareTestContext()
        {
            if (!Utilities.EnsureFolderExists(App.appSettings.Path.ExportImg))
                return null;

            var context = new TestContext();
            context.TestType = _testType;
            context.TestCategory = _testCategory;
            context.Tester = _tester;
            context.ResLogs = ResLogs;
            context.InspectionFunction = inspectionFunction;
            context.ProcessFunction = processFunction;

            ImageSource imageSourceToUse = (_testType == TestTypes.EnvGeo)
                ? EnvImg
                : SrcImg;

            App.Current.Dispatcher.Invoke(() =>
            {
                context.SnapshotImg = Utilities.CopyBitmapSource((BitmapSource)imageSourceToUse);
            });

            try
            {
                _testMemoryService.Prepare(context, _testType);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "메모리 준비 실패");
                return null;
            }

            return context;
        }

        // 공통 정리 함수
        private void FinalizeTestContext(TestContext context)
        {
            if (context == null) return;

            ResTxt = context.ChangedImgMetadata;
            Log.Information($"metadataOnly: {context.ChangedImgMetadata}");
            context.ResLogs.Add(context.ChangedImgMetadata);

            var epoch = Utilities.GetCurrentUnixTimestampMilliseconds();
            string originalImgName = Path.Combine(App.appTempDir, $"{epoch}_ori.bmp");
            string resultImagePath = Path.Combine(App.appTempDir, $"{epoch}_det.png");

            Utilities.SaveBitmap(context.SnapshotImg, originalImgName);
            Utilities.SavePng(context.ResultImg, resultImagePath);

            App.Current.Dispatcher.Invoke(() =>
            {
                ResImg = context.ResultImg;
            });

            _test = new Test
            {
                TestCategoryId = (int)context.TestCategory,
                TestTypeId = (int)context.TestType,
                TesterId = context.Tester.Id,
                Result = context.ResultScore,
                Method = 1,
                ChangedImgMetadata = context.ChangedImgMetadata,
                OriginalImg = Path.GetFileName(originalImgName),
                ChangedImg = Path.GetFileName(resultImagePath),
            };
            PrepareTest(context.TestCategory, _test);

            _testMemoryService.Release(context);
        }


        // 테스트 타입에 맞는 전략 객체 반환 (더 이상 ViewModel이나 이미지를 전달하지 않습니다)
        private ITestStrategy GetTestStrategy(TestTypes type)
        {
            return type switch
            {
                TestTypes.Gray => new GrayTestStrategy(),
                TestTypes.Res => new ResTestStrategy(),
                TestTypes.Geo => new GeoTestStrategy(),
                TestTypes.EnvGeo => new EnvGeoTestStrategy(),
                _ => throw new ArgumentException($"Invalid test type: {type}")
            };
        }

        private bool CanNext()
        {
            Log.Information(nameof(CanNext));

            return (TestResult != -2 && ValidationService.GetValidating(ValidationDict, nameof(TDSn)));
        }

        [RelayCommand(CanExecute = nameof(CanNext))]
        private async Task NextAsync()
        {
            // 1) 저장
            await SaveTestAndExportImagesAsync();

            // 2) PASS/FAIL 색칠
            _cellStatusService.SetCellPassFail(
                _test,
                (pos, brush) => BorderBackgrounds[(int)pos] = new ObservableBrush { Value = brush }
            );

            // 3) 이제 다음 셀로 이동
            await ActivateNextCellAsync();
        }

        private async Task SaveTestAndExportImagesAsync()
        {
            _test.Result = TestResult;

            var basePath = App.appSettings.Path.ExportImg;
            var phaseMap = App.appSettings.Path.ExportImgPhase;
            string sn = GetSnByCategory(_testCategory);

            string exportPath = Utilities.GetExportImgPath(basePath, phaseMap, (int)_testCategory);

            string typeSuffix = _test.TestTypeId switch
            {
                1 => "gray",
                2 => "res",
                3 => "align",
                _ => "unk"
            };
            string prefix = $"{sn}_{typeSuffix}";
            string baseName = await _imageService.GenNextImgNameAsync(prefix);

            string finalOriginalName = $"{baseName}_{_tester.PcId:D3}.bmp";
            string finalChangedName = $"{baseName}_{_tester.PcId:D3}.png";

            Utilities.MoveTempImageToExport(_test.OriginalImg, App.appTempDir, exportPath, finalOriginalName);
            Utilities.MoveTempImageToExport(_test.ChangedImg, App.appTempDir, exportPath, finalChangedName);

            _test.OriginalImg = finalOriginalName;
            _test.ChangedImg = finalChangedName;

            await _testingManagementService.SaveAsync(_test);
        }

        private async Task ActivateNextCellAsync()
        {
            // 현재 셀 찾기
            var current = TestCellMap.Items
                .FirstOrDefault(x => x.Category == _testCategory && x.TestType == _testType);

            if (current == null)
                return;

            // 다음 셀 인덱스
            int idx = TestCellMap.Items.IndexOf(current);

            // 마지막 셀이면 종료
            if (idx == -1 || idx == TestCellMap.Items.Count - 1)
                return;

            // 다음 셀
            var next = TestCellMap.Items[idx + 1];

            // 다음 셀을 UI에 적용 (기존 CellClickAsync 그대로 활용)
            await CellClickAsync(next.Position);
        }

        private string GetSnByCategory(TestCategories category)
        {
            return category switch
            {
                TestCategories.Processing => _transducer!.Sn,
                TestCategories.Process => _transducerModule!.Sn,
                TestCategories.Dispatch => _probe!.Sn,
                _ => throw new ArgumentOutOfRangeException(nameof(category))
            };
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

        private readonly USRenderService _usRenderer;
        private double _rotationAngle = 0.0;

        public void RenderStart()
        {
            //usRenderer = new USRenderService(512, 512, _imageBufferService);
            _usRenderer.connectRenderToTargetFunction(UpdateImageSource, UpdateEnvImageSource);
            _usRenderer.RenderStart();
        }

        public void RenderEnd()
        {
            if (_usRenderer != null)
            {
                _usRenderer.RenderEnd();
            }
        }

        public void UpdateImageSource(BitmapSource bitmapSource)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                //SrcImg = Utilities.BitmapToImageSource(m_bmpRes);
                SrcImg = bitmapSource;
            });
        }

        public void UpdateEnvImageSource(BitmapSource bitmapSource)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                EnvImg = bitmapSource;
            });
        }

        // 메시지를 표시할 메서드 예시
        public async Task ShowMessageAsync(string message)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Message = message;
                MessageIsPopupOpen = true; // Popup을 열어 메시지를 표시합니다.
            });

            await Task.Delay(3000); // 3초 대기

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageIsPopupOpen = false; // Popup을 닫습니다.
            });
        }

        public SubData SubData { get; set; } = default!;

        public void ReceiveParameter(object parameter)
        {
            if (parameter is SubData subData)
            {
                SubData = subData;

                Log.Information($"Received parameter {SubData.stringData}");
            }

        }

        private void CloseWindow()
        {
            //App.Current.MainWindow.Close();
            //Application.Current.Shutdown()
            //Environment.Exit(1);

            Application.Current.Dispatcher.Invoke(() =>
            {
                //Window? focusedWindow = System.Windows.Input.Keyboard.FocusedElement as Window;
                //focusedWindow?.Close();
                Controls.MessageBox.Show("Get Video Fail", $"Open Video App First");
                var windows = Application.Current.Windows.OfType<Window>();
                var window = windows.FirstOrDefault(w => w.DataContext == this);
                window?.Close();
            });
        }

        protected override void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            base.OnWindowLoaded(sender, e);
            Log.Information($"{nameof(OnWindowLoaded)}");
            //Init();
            //LogIn();

            if (!InitMotor())
            {
                CloseWindow();
            }
            else
            {
                _motorService.StartMotor();
                Task.Delay(100);
            }
        }

        protected override void AddMsg()
        {
            var currentViewModelTypeName = GetType().Name;

            RegisterMessageHandler<ViewModelActionMessage>(msg =>
            {
                if (msg.Value.TargetViewModel == currentViewModelTypeName && msg.Value.Action == "Refresh")
                {
                    Log.Information($"currentViewModelTypeName Refresh");
                    //_ = SearchAsync()
                    TDSn = string.Empty;
                    OnTDSnChanged(TDSn);
                }
            });
        }

        protected override void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            base.OnWindowClosing(sender, e);
            Log.Information($"{nameof(OnWindowClosing)}");
            _motorService.StopMotor();
            //Task.Delay(100);
            //_model.DeactivateProbe();
            e.Cancel = true;
            if (sender is Window window)
            {
                window.Hide();
            }
        }

        protected override void OnWindowActivated(object? sender, EventArgs e)
        {
            base.OnWindowActivated(sender, e);
            Log.Information($"{nameof(OnWindowActivated)}");

            _motorService.StartMotor();
            //Task.Delay(100);
            //_model.ActivateProbe();
        }

        [RelayCommand]
        private async Task AutoTestAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                Log.Information($"===== Loop {i + 1} Start =====");

                await CellClickAsync(CellPositions.Row1_Column3);
                await Task.Delay(100);
                
                await TestAsync();
                await Task.Delay(2000);

                TestResult = 100;
                await Task.Delay(100);

                await NextAsync();
                await Task.Delay(1000);
            }

            Log.Information("===== Loop Finished =====");
            ResLogs.Add("100회 반복 테스트 완료");
        }
    }
}