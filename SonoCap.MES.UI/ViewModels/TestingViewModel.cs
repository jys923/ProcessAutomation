//#define SET_MOTOR

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SonoCap.Commons;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Models.Inspection;
using SonoCap.MES.Services;
using SonoCap.MES.Services.Interfaces;
using SonoCap.WpfCommons;
using SonoCap.MES.UI.Services;
using SonoCap.MES.UI.Services.Interfaces;
using SonoCap.MES.UI.Validation;
using SonoCap.MES.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
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

        public TestingViewModel(
            USRenderService usRenderer,
            ImageBufferService imageBufferService,
            ImageService imageService,
            IViewService viewService,
            TestingManagementService testingManagementService,
            MES.Services.Model.GlobalModel model,
            IMotorService motorService,
            ICellStatusService cellStatusService)
        {
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

        private int _oldRow = -1;
        private int _oldCol = -1;
        private CellPositions _oldCell = (int)CellPositions.Row0_Column0;

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

        private void TDMdSnFilterItems()
        {
            if (string.IsNullOrWhiteSpace(TDMdSn))
            {

                TDMdSnFilteredItems.Clear();
            }
            else
            {
                List<string> items = _testingManagementService.GetFilteredSn(SnType.TransducerModule, TDMdSn);

                TDMdSnFilteredItems = new ObservableCollection<string>(items);
            }
        }

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

        private void MTMdSnFilterItems()
        {
            if (string.IsNullOrWhiteSpace(MTMdSn))
            {

                MTMdSnFilteredItems.Clear();
            }
            else
            {
                List<string> items = _testingManagementService.GetFilteredSn(SnType.MotorModule, MTMdSn);

                MTMdSnFilteredItems = new ObservableCollection<string>(items);
            }
        }

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

        private void ProbeSnFilterItems()
        {
            if (string.IsNullOrWhiteSpace(ProbeSn))
            {

                ProbeSnFilteredItems.Clear();
            }
            else
            {
                List<string> items = _testingManagementService.GetFilteredSn(SnType.Probe, ProbeSn);

                ProbeSnFilteredItems = new ObservableCollection<string>(items);
            }
        }

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

        private void OnSelectedViewDepthChanged(double value)
        {
            LineDensityIsEnabled.Keys.ToList().ForEach(key => LineDensityIsEnabled[key].IsEnabled = false);
            switch (value)
            {
                case 3:
                    LineDensityIsEnabled[0].IsEnabled = true;
                    LineDensityIsEnabled[1].IsEnabled = true;
                    LineDensityIsEnabled[2].IsEnabled = true;
                    LineDensityIsEnabled[3].IsEnabled = true;
                    break;
                case 4:
                    LineDensityIsEnabled[0].IsEnabled = false;
                    LineDensityIsEnabled[1].IsEnabled = true;
                    LineDensityIsEnabled[2].IsEnabled = true;
                    LineDensityIsEnabled[3].IsEnabled = true;
                    break;
                case 5:
                    LineDensityIsEnabled[0].IsEnabled = false;
                    LineDensityIsEnabled[1].IsEnabled = false;
                    LineDensityIsEnabled[2].IsEnabled = true;
                    LineDensityIsEnabled[3].IsEnabled = true;
                    break;
                case 6:
                    LineDensityIsEnabled[0].IsEnabled = false;
                    LineDensityIsEnabled[1].IsEnabled = false;
                    LineDensityIsEnabled[2].IsEnabled = false;
                    LineDensityIsEnabled[3].IsEnabled = true;
                    break;
                case 7:
                    LineDensityIsEnabled[0].IsEnabled = false;
                    LineDensityIsEnabled[1].IsEnabled = false;
                    LineDensityIsEnabled[2].IsEnabled = false;
                    LineDensityIsEnabled[3].IsEnabled = true;
                    break;
                default:
                    break;
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

        private void OnSelectedLineDensityChanged(int value)
        {
            DepthIsEnabled.Keys.ToList().ForEach(key => DepthIsEnabled[key].IsEnabled = false);
            switch (value)
            {
                case 4:
                    DepthIsEnabled[0].IsEnabled = true;
                    DepthIsEnabled[1].IsEnabled = true;
                    DepthIsEnabled[2].IsEnabled = true;
                    DepthIsEnabled[3].IsEnabled = true;
                    DepthIsEnabled[4].IsEnabled = true;
                    break;
                case 3:
                    DepthIsEnabled[0].IsEnabled = true;
                    DepthIsEnabled[1].IsEnabled = true;
                    DepthIsEnabled[2].IsEnabled = true;
                    DepthIsEnabled[3].IsEnabled = false;
                    DepthIsEnabled[4].IsEnabled = false;
                    break;
                case 2:
                    DepthIsEnabled[0].IsEnabled = true;
                    DepthIsEnabled[1].IsEnabled = true;
                    DepthIsEnabled[2].IsEnabled = false;
                    DepthIsEnabled[3].IsEnabled = false;
                    DepthIsEnabled[4].IsEnabled = false;
                    break;
                case 1:
                    DepthIsEnabled[0].IsEnabled = true;
                    DepthIsEnabled[1].IsEnabled = false;
                    DepthIsEnabled[2].IsEnabled = false;
                    DepthIsEnabled[3].IsEnabled = false;
                    DepthIsEnabled[4].IsEnabled = false;
                    break;
                default:
                    break;
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

        private TestTypes ColumnToTestType(int col)
        {
            // col 값에 따라 TestTypes를 반환하는 로직 구현
            switch (col)
            {
                case 1: return TestTypes.Gray;
                case 2: return TestTypes.Res;
                case 3: return TestTypes.EnvGeo;
                case 4: return TestTypes.Align;
                case 5: return TestTypes.Axial;
                case 6: return TestTypes.Lateral;
                case 7: return TestTypes.Geo;
                default: return TestTypes.None;
            }
        }

        [RelayCommand]
        private Task CellClickAsync(CellPositions position)
        {
            _oldCell = position;
            int row = (int)position / 10;
            int col = (int)position % 10;
            Log.Information($"CellClick row:{row} col:{col}");
            _testCategory = (TestCategories)row;
            _testType = ColumnToTestType(col);
            bool isRowChanged = _oldRow != row;
            bool isColChanged = _oldCol != col;

            if (isRowChanged || isColChanged)
            {
                _oldRow = row;
                _oldCol = col;
                // 로직
            }

            //SrcImg = default!;
            //ResImg = default!;
            //TestResult = -2;
            //ValidationDict[nameof(TestResult)].IsEnabled = false;
            IsEnvImgVisible = false;
            switch (position)
            {
                case CellPositions.Row1_Column1:
                    BlinkingCellIndex = (int)CellPositions.Row1_Column1;
                    //processFunction = MyOpenCVWrapper.OpenCVWrapper.ResolutionProcess;
                    break;
                case CellPositions.Row1_Column2:
                    BlinkingCellIndex = (int)CellPositions.Row1_Column2;
                    //processFunction = MyOpenCVWrapper.OpenCVWrapper.GeometricDistortionProcess;
                    break;
                case CellPositions.Row1_Column3:
                    BlinkingCellIndex = (int)CellPositions.Row1_Column3;
                    //processFunction = MyOpenCVWrapper.OpenCVWrapper.GrayProcess;
                    IsEnvImgVisible = true;
                    break;
                case CellPositions.Row2_Column1:
                    BlinkingCellIndex = (int)CellPositions.Row2_Column1;
                    //processFunction = MyOpenCVWrapper.OpenCVWrapper.ResolutionProcess;
                    break;
                case CellPositions.Row2_Column2:
                    BlinkingCellIndex = (int)CellPositions.Row2_Column2;
                    //processFunction = MyOpenCVWrapper.OpenCVWrapper.GeometricDistortionProcess;
                    break;
                case CellPositions.Row2_Column3:
                    BlinkingCellIndex = (int)CellPositions.Row2_Column3;
                    //processFunction = MyOpenCVWrapper.OpenCVWrapper.GrayProcess;
                    IsEnvImgVisible = true;
                    break;
                case CellPositions.Row3_Column1:
                    BlinkingCellIndex = (int)CellPositions.Row3_Column1;
                    //processFunction = MyOpenCVWrapper.OpenCVWrapper.ResolutionProcess;
                    break;
                case CellPositions.Row3_Column2:
                    BlinkingCellIndex = (int)CellPositions.Row3_Column2;
                    //processFunction = MyOpenCVWrapper.OpenCVWrapper.GeometricDistortionProcess;
                    break;
                case CellPositions.Row3_Column3:
                    BlinkingCellIndex = (int)CellPositions.Row3_Column3;
                    //processFunction = MyOpenCVWrapper.OpenCVWrapper.GrayProcess;
                    IsEnvImgVisible = true;
                    break;
                default:
                    break;
            }

            ResImg = default!;
            TestResult = -2;
            ValidationDict[nameof(TestResult)].IsEnabled = false;
            //OnTDSnChanged(TDSn);
            TDSnIsPopupOpen = false;

            //// TestCommand의 CanExecute 상태를 갱신합니다.
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

        [RelayCommand]
        private async Task ForcePassAsync3(CellPositions position)
        {
            Log.Information($"{nameof(ForcePassAsync3)} click {position}");
            int row = (int)position / 10;
            _testCategory = (TestCategories)row;

            bool proceed = Controls.MessageBox.Show("통합 검사", "통합 검사 실행?");
            if (!proceed)
            {
                ResLogs.Add("통합 검사 취소");
                return;
            }

            //// 검사 이력 확인
            //var allTypeIds = new[] { 1, 2, 3 };
            //var existingTypes = _testingManagementService.GetExistingTestTypeIds(
            //    _testCategory, _transducer, _transducerModule, _probe
            //);
            //var missingTypes = allTypeIds.Except(existingTypes).ToList();

            //if (missingTypes.Count == 0)
            //{
            //    Controls.MessageBox.Show("검사 생략", "모든 항목이 이미 검사되어\n추가로 저장할 항목이 없습니다.");
            //    ResLogs.Add("검사 생략 - 저장할 항목 없음");
            //    return;
            //}

            // 이미지 캡처 및 RunInspection 실행
            App.Current.Dispatcher.Invoke(() =>
            {
                SnapshotImg = Utilities.CopyBitmapSource((BitmapSource)SrcImg);
                //ResImg = SrcImg;
            });

            // BitmapSource를 byte array로 변환하고 IntPtr로 전달
            BitmapSource bitmapSource = (BitmapSource)SnapshotImg;
            GCHandle imageHandle;
            IntPtr imageBufferPtr = Utilities.BitmapSourceToByteArray(bitmapSource, out imageHandle);

            // 결과 이미지 저장 배열
            int resultImageSize = bitmapSource.PixelWidth * bitmapSource.PixelHeight * 4;
            byte[] resultImageArray = new byte[resultImageSize];
            GCHandle resultHandle = GCHandle.Alloc(resultImageArray, GCHandleType.Pinned);
            IntPtr resultBufferPtr = resultHandle.AddrOfPinnedObject();

            // 텍스트 데이터 저장 배열
            byte[] textArray = new byte[1024];
            GCHandle textHandle = GCHandle.Alloc(textArray, GCHandleType.Pinned);
            IntPtr textBufferPtr = textHandle.AddrOfPinnedObject();

            Utilities.InspectionImage(inspectionFunction, imageBufferPtr, bitmapSource.PixelWidth, bitmapSource.PixelHeight, resultBufferPtr, textBufferPtr, (int)InspectionPartType.All);

            var epoch = Utilities.GetCurrentUnixTimestampMilliseconds();
            //string OriginalImgName = $"{App.appSettings.Path.ExportImg}{epoch}_ori.bmp";
            //string resultImagePath = $"{App.appSettings.Path.ExportImg}{epoch}_det.png";

            string OriginalImgName = Path.Combine(App.appTempDir, $"{epoch}_ori.bmp");
            string resultImagePath = Path.Combine(App.appTempDir, $"{epoch}_det.png");

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
            //Utilities.ImageSourceToGrayBmp(SrcImg, OriginalImgName);
            Utilities.SaveBitmap((BitmapImage)SnapshotImg, OriginalImgName);
            Utilities.SavePng(resultBitmapSource, resultImagePath);
            App.Current.Dispatcher.Invoke(() =>
            {
                //SnapshotImg = Utilities.CopyBitmapSource((BitmapSource)SrcImg);
                ResImg = resultBitmapSource;
            });
            // 결과 텍스트 출력
            string resultText = System.Text.Encoding.UTF8.GetString(textArray).TrimEnd('\0');
            Log.Information($"resultText: {resultText}");
            ResLogs.Add(resultText);
            ResTxt = resultText;
            // 메모리 해제
            imageHandle.Free();
            resultHandle.Free();
            textHandle.Free();

            ////ResImg = default!;
            //TestResult = -2;
            //ValidationDict[nameof(TestResult)].IsEnabled = false;
            //OnTDSnChanged(TDSn);
            //TDSnIsPopupOpen = false;

            var parsed = JsonSerializer.Deserialize<InspectionResult>(resultText);
            if (parsed != null)
            {
                await SaveForceTestResultsAsync(parsed, Path.GetFileName(OriginalImgName), Path.GetFileName(resultImagePath));
                Log.Information("Calling TryActivateNextCategoryAsync");
                await TryActivateNextCategoryAsync();

                TestResult = -2;
                ValidationDict[nameof(TestResult)].IsEnabled = false;
                OnTDSnChanged(TDSn);
                TDSnIsPopupOpen = false;
            }
        }

        private async Task SaveForceTestResultsAsync(InspectionResult parsed, string originalImg, string changedImg)
        {
            var allTypeIds = new[] { 1, 2, 3 };

            var basePath = App.appSettings.Path.ExportImg;
            var phaseMap = App.appSettings.Path.ExportImgPhase;
            var sn = GetSnByCategory(_testCategory);

            string exportPath = Utilities.GetExportImgPath(basePath, phaseMap, (int)_testCategory);

            //var missingTypes = allTypeIds.Except(existingTypes).ToList();
            //Log.Information("missingTypes = {Missing}", string.Join(", ", missingTypes));

            // prefix = 예: "probeSn_gray"
            string prefix = $"{sn}_all";
            string finalName = await _imageService.GenNextImgNameAsync(prefix);
            //string finalName = Utilities.GenImgName(prefix, exportPath);

            //string finalOriginalName = finalName + ".bmp";
            //string finalChangedName = finalName + ".png";
            string finalOriginalName = $"{finalName}_{_tester.PcId.ToString("D3")}.bmp"; // 보간 문자열
            string finalChangedName = $"{finalName}_{_tester.PcId.ToString("D3")}.png"; // 보간 문자열

            bool anySaved = false;
            foreach (int typeId in allTypeIds)
            {
                var resultScore = typeId switch
                {
                    1 => InspectionCalculator.CalculateGrayScore(parsed.Gray),
                    2 => InspectionCalculator.CalculateResScore(parsed.Res),
                    3 => InspectionCalculator.CalculateGeoScore(parsed.Geo),
                    _ => 0
                };

                var newTest = new Test
                {
                    TestCategoryId = (int)_testCategory,
                    TesterId = _tester.Id,
                    OriginalImg = finalOriginalName,
                    ChangedImg = finalChangedName,
                    Result = 100,
                    Method = 0,
                    TestTypeId = typeId,
                    ChangedImgMetadata = typeId switch
                    {
                        1 => parsed.Gray.ToJson(),
                        2 => parsed.Res.ToJson(),
                        3 => parsed.Geo.ToJson(),
                        _ => "{}"
                    },
                };

                PrepareTest(_testCategory, newTest);

                if (await _testingManagementService.SaveAsync(newTest))
                {
                    anySaved = true;
                    ResLogs.Add($"통합 검사 저장: TestTypeId = {typeId}");
                }
            }

            if (anySaved)
            {
                Utilities.MoveTempImageToExport(originalImg, App.appTempDir, exportPath, finalOriginalName);
                Utilities.MoveTempImageToExport(changedImg, App.appTempDir, exportPath, finalChangedName);
            }
            Log.Information("SaveForceTestResultsAsync done");
        }

        // 강제 패스 저장
        private async Task SaveForceTestResultsAsync2(InspectionResult parsed, string originalImg, string changedImg)
        {
            // 저장 대상 리스트 (1=Gray, 2=Res, 3=Geo)
            var testTypeIds = new[] { 1, 2, 3 };

            foreach (int typeId in testTypeIds)
            {
                var newTest = new Test
                {
                    TestCategoryId = (int)_testCategory,
                    TesterId = _tester.Id,
                    OriginalImg = originalImg,
                    ChangedImg = changedImg,
                    Result = 100,
                    Method = 2,
                    TestTypeId = typeId,
                    ChangedImgMetadata = typeId switch
                    {
                        1 => parsed.Gray.ToJson(),
                        2 => parsed.Res.ToJson(),
                        3 => parsed.Geo.ToJson(),
                        _ => "{}"
                    }
                };

                PrepareTest(_testCategory, newTest);

                if (await _testingManagementService.SaveAsync(newTest))
                {
                    ResLogs.Add($"검사 저장: TestTypeId = {typeId}");
                }
            }

            //ResImg = default!;
            TestResult = -2;
            ValidationDict[nameof(TestResult)].IsEnabled = false;
            OnTDSnChanged(TDSn);
            TDSnIsPopupOpen = false;

        }

        [RelayCommand]
        private async Task ForcePassAsync2(CellPositions position)
        {
            Log.Information($"{nameof(ForcePassAsync2)} click {position}");
            int row = (int)position / 10;
            _testCategory = (TestCategories)row;
            bool proceed = Controls.MessageBox.Show("강제 검사", "강제 검사 실행?");
            if (!proceed)
            {
                ResLogs.Add("강제 검사 취소");
                return;
            }
            switch (position)
            {
                case CellPositions.Row1_Column4:
                    //BlinkingCellIndex = (int)CellPositions.Row1_Column1;
                    if (_transducer != null)
                    {
                        await ForceAllPassAsync2(TestCategories.Processing);
                    }
                    else
                    {
                        ResLogs.Add("TD Sn 없음");
                        //await ShowMessageAsync("TD Sn 없음");
                    }
                    break;
                case CellPositions.Row2_Column4:
                    //BlinkingCellIndex = (int)CellPositions.Row2_Column1;
                    if (_transducerModule != null)
                    {
                        await ForceAllPassAsync2(TestCategories.Process);
                    }
                    else
                    {
                        ResLogs.Add("TDMd Sn 없음");
                    }
                    break;
                case CellPositions.Row3_Column4:
                    //BlinkingCellIndex = (int)CellPositions.Row3_Column1;
                    if (_probe != null)
                    {
                        await ForceAllPassAsync2(TestCategories.Dispatch);
                    }
                    else
                    {
                        ResLogs.Add("Probe Sn 없음");
                    }
                    break;
                default:
                    break;
            }
            ResImg = default!;
            TestResult = -2;
            ValidationDict[nameof(TestResult)].IsEnabled = false;
            OnTDSnChanged(TDSn);
            TDSnIsPopupOpen = false;

            // TestCommand의 CanExecute 상태를 갱신합니다.
            //(TestCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();

            //if (TestCommand.CanExecute(null))
            //{
            //    await TestCommand.ExecuteAsync(null);
            //}
        }

        //td tdmd probe 입력 값이 달라서 곤란
        private async Task ForceAllPassAsync2(TestCategories testCategory)
        {
            //throw new NotImplementedException();
            //검사하고 없는 것만 검사 데이터 추가
            Test insertTest = new Test
            {
                TestCategoryId = (int)testCategory,
                TesterId = _tester.Id,
                ChangedImgMetadata = "Force Pass",
                Result = 100,
                Method = 2,
            };

            PrepareTest(testCategory, insertTest);

            IEnumerable<Test> testRes = new List<Test>();

            switch (testCategory)
            {
                case TestCategories.Processing:
                    testRes = _testingManagementService.GetLatestTests(transducer: _transducer);
                    break;
                case TestCategories.Process:
                    testRes = _testingManagementService.GetLatestTests(transducerModule: _transducerModule);
                    break;
                case TestCategories.Dispatch:
                    testRes = _testingManagementService.GetLatestTests(probe: _probe);
                    break;
                default:
                    break;
            }

            int GetTestThreshold(TestCategories testCategory, int testType)
            {
                int aa = (int)testCategory * 10 + testType;
                return App.TestThresholdDict[aa];
            }

            foreach (Test test in testRes)
            {
                if (test.Result < GetTestThreshold(testCategory, test.TestTypeId))
                {
                    insertTest.Id = 0;
                    insertTest.TestTypeId = test.TestTypeId;

                    if (await _testingManagementService.SaveAsync(insertTest))
                    {
                        ResLogs.Add($"Fail > Pass test : {insertTest.ToString()}");
                    }
                }
            }

            var testList = testRes.ToList();

            // 현재 존재하는 TestTypeId 확인
            var existingTypes = testList.Select(t => t.TestTypeId).Distinct().ToList();

            // 필요한 TestTypeId (예: 1, 2, 3)
            var requiredTypes = new[] { 1, 2, 3 };

            // 누락된 TestTypeId 찾기
            List<int> missingTypes = requiredTypes.Except(existingTypes).ToList();

            foreach (int type in missingTypes)
            {
                insertTest.Id = 0;
                insertTest.TestTypeId = type;

                if (await _testingManagementService.SaveAsync(insertTest))
                {
                    ResLogs.Add($"Add test : {insertTest.ToString()}");
                }
            }

            SharedSeqNo? seqNo = await _testingManagementService.GetSeqNoAsync();
            bool existNext = false;
            bool passAll = false;
            int id = 0;

            switch (testCategory)
            {
                case TestCategories.Processing:
                    id = _transducer.Id;
                    existNext = _transducerModule is not null ? true : false;
                    passAll = await _testingManagementService.PassTestCategoryAsync(_testCategory, transducer: _transducer);
                    if (!existNext && id > 0 && passAll)
                    {
                        TransducerModule tdMd = new TransducerModule { Sn = $"tdm-sn{DateTime.Today.ToString("yyMMdd")}{seqNo.TDMdNo.ToString().PadLeft(3, '0')}", TransducerId = id };
                        if (await _testingManagementService.InsertTdMdAsync(tdMd))
                        {
                            _transducerModule = tdMd;
                            await _testingManagementService.SetSeqNoAsync(SnType.TransducerModule);
                            ResLogs.Add($"Add TDMd Sn : {tdMd.Sn}");
                        }
                    }
                    break;
                case TestCategories.Process:
                    id = _transducerModule.Id;
                    existNext = _probe is not null ? true : false;
                    passAll = await _testingManagementService.PassTestCategoryAsync(_testCategory, transducerModule: _transducerModule);
                    if (!existNext && id > 0 && passAll)
                    {
                        //_motorModule = Controls.InputBoxMotor.Show("Motor Module", "Input Motor Module Lot", _motorModuleRepository);
                        _motorModule = _viewService.ShowInputBoxMotorView("Motor Module", "Input Motor Module Lot");

                        if (_motorModule is null) break;

                        Probe probe = new Probe { Sn = $"UPAG1{DateTime.Today.ToString("yyMMdd")}{seqNo.ProbeNo.ToString().PadLeft(3, '0')}", TransducerModuleId = id, MotorModuleId = _motorModule.Id };
                        if (await _testingManagementService.InsertProbeAsync(probe))
                        {
                            _probe = probe;
                            await _testingManagementService.SetSeqNoAsync(SnType.Probe);
                            ResLogs.Add($"Add Probe Sn : {probe.Sn}");
                        }
                    }
                    break;
                case TestCategories.Dispatch:
                    break;
                default:
                    break;
            }
            await _testingManagementService.PTRViewUpsertAsync(_probe, _pTRView);
        }

        [RelayCommand]
        private void CellRightClick(CellPositions position)
        {
            if (_oldCell != position)
            {
                ResLogs.Add($"fail start test {position}");
                return;
            }

            //// TestCommand의 CanExecute 상태를 갱신합니다.
            //(TestCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();

            //// Test 메서드를 호출합니다.
            //if (TestCommand.CanExecute(null))
            //{
            //    TestCommand.Execute(null);
            //}
        }

        async partial void OnTDSnChanged(string value)
        {
            TDSnFilterItems();
            TDSnIsPopupOpen = !string.IsNullOrEmpty(value) && TDSnFilteredItems.Any();

            //ChangeIsEnabled(TestCategories.Processing);
            ValidationService.ClearValidatingWaterMark(ValidationDict);

            _probe = null;
            _transducerModule = null;
            _transducer = null;
            _motorModule = null;
            _pTRView = null;

            TdCellIsEnabled = false;
            TdMdCellIsEnabled = false;
            ProbeCellIsEnabled = false;

            BlinkingCellIndex = (int)CellPositions.Row0_Column0;

            List<Test> tests;
            //정규 표현식 검증 추가
            if (value.Length > 1)
            {
                Log.Information($"Transducer sn {value}");
                if (!await _testingManagementService.IsExistsBySnAsync(SnType.Transducer, value))
                {
                    ValidationService.ValidateField(ValidationDict, nameof(TDSn), "TDSn Is Not Exist");
                    //SetCellBackgrounds(TestCategories.All, Brushes.LightGray);
                    _cellStatusService.SetCategoryDefault(
                        TestCategories.All,
                        (pos, brush) => BorderBackgrounds[(int)pos] = new ObservableBrush { Value = brush }
                    );
                }
                else
                {
                    //BlinkingCellIndex = (int)_oldCell;
                    TdCellIsEnabled = true;
                    //셀버튼 _td _tdMd _p 각각 널이면 가로로 한줄을 끔
                    await SetBySnAsync(SnType.Transducer, value);
                    ValidationService.ValidateField(ValidationDict, nameof(TDSn));

                    Log.Information("Calling GetTestByIdAsync");
                    tests = await _testingManagementService.GetTestByIdAsync(SnType.Transducer, _transducer!.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        //SetCellPassFail(item, cellPosition);
                        _cellStatusService.SetCellPassFail(
                            item,
                            (pos, brush) => BorderBackgrounds[(int)pos] = new ObservableBrush { Value = brush }
                        );
                    }
                    //BlinkingCellIndex = 10 + Math.Max(1, Math.Min(tests.Count, 3));

                    _transducerModule = await _testingManagementService.GetLatestTransducerModuleAsync(_transducer.Id);

                    if (_transducerModule is null)
                        return;

                    TdMdCellIsEnabled = true;

                    ValidationDict[nameof(TDMdSn)].IsEnabled = false;
                    ValidationDict[nameof(TDMdSn)].WaterMarkText = _transducerModule.Sn;

                    tests = await _testingManagementService.GetTestByIdAsync(SnType.TransducerModule, _transducerModule.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        //SetCellPassFail(item, cellPosition);
                        _cellStatusService.SetCellPassFail(
                            item,
                            (pos, brush) => BorderBackgrounds[(int)pos] = new ObservableBrush { Value = brush }
                        );
                    }

                    //BlinkingCellIndex = 20 + Math.Max(1, Math.Min(tests.Count, 3));

                    _probe = await _testingManagementService.GetLatestProbeWithMotorAsync(_transducerModule.Id);

                    if (_probe is null)
                        return;

                    _motorModule = _probe.MotorModule;

                    ValidationDict[nameof(MTMdSn)].IsEnabled = false;
                    ValidationDict[nameof(MTMdSn)].WaterMarkText = _motorModule.Sn;

                    ProbeCellIsEnabled = true;
                    ValidationDict[nameof(ProbeSn)].IsEnabled = false;
                    ValidationDict[nameof(ProbeSn)].WaterMarkText = _probe.Sn;

                    tests = await _testingManagementService.GetTestByIdAsync(SnType.Probe, _probe.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        //SetCellPassFail(item, cellPosition);
                        _cellStatusService.SetCellPassFail(
                            item,
                            (pos, brush) => BorderBackgrounds[(int)pos] = new ObservableBrush { Value = brush }
                        );

                    }
                    //BlinkingCellIndex = 30 + Math.Max(1, Math.Min(tests.Count, 3));
                }
            }
            else
            {
                ValidationService.ValidateField(ValidationDict, nameof(TDSn), "TDSn Is Not Valid");
                //SetCellBackgrounds(TestCategories.All, Brushes.LightGray);
                _cellStatusService.SetCategoryDefault(
                    TestCategories.All,
                    (pos, brush) => BorderBackgrounds[(int)pos] = new ObservableBrush { Value = brush }
                );

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

            // 어떤 이미지를 사용할지 ViewModel에서 결정합니다.
            ImageSource imageSourceToUse = null;
            byte[][] imageSnapshotsToUse = null;

            // 테스트 타입에 따라 _envImageBuffer 또는 _srcImageBuffer (10개 스냅샷 배열)를 선택합니다.
            if (_testType == TestTypes.EnvGeo)
            {
                imageSourceToUse = EnvImg;
                imageSnapshotsToUse = _imageBufferService.GetEnvSnapshot(); // _envImageBuffer는 byte[][] 타입이라고 가정
            }
            else
            {
                imageSourceToUse = SrcImg;
                imageSnapshotsToUse = _imageBufferService.GetSnapshot(); // _srcImageBuffer는 byte[][] 타입이라고 가정
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                context.SnapshotImg = Utilities.CopyBitmapSource((BitmapSource)imageSourceToUse);
            });

            // 공통 함수를 호출하여 이미지 복사 및 메모리를 준비합니다.
            //PrepareImageAndMemory(context, imageSourceToUse);
            try
            {
                PrepareSnapshotsAndMemory(context, imageSnapshotsToUse);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, "10개 스냅샷 버퍼 준비 중 오류 발생.");
                return null;
            }

            return context;
        }

        // TestingViewModel.cs (추가되는 함수)

        private void PrepareSnapshotsAndMemory(TestContext context, byte[][] snapshotSource)
        {
            // 1. 10개 원본 스냅샷 가져오기
            context.InputSnapshots = snapshotSource;

            if (context.InputSnapshots == null || context.InputSnapshots.Length != 10)
            {
                throw new InvalidOperationException("선택된 버퍼 소스에서 10개의 스냅샷을 가져오지 못했습니다.");
            }

            // 이미지 길이 및 텍스트 길이 설정
            int imageLength = context.InputSnapshots[0].Length;
            int textLength = 4096;

            // 10개 결과 배열 및 포인터 배열 초기화
            context.ResultSnapshots = new byte[10][];
            context.ResultTextArrays = new byte[10][];
            context.InputBufferPtrs = new IntPtr[10];
            context.ResultBufferPtrs = new IntPtr[10];
            context.ResultTextBufferPtrs = new IntPtr[10];

            // 10쌍의 메모리를 모두 할당하고 고정(Pinning)합니다.
            for (int i = 0; i < 10; i++)
            {
                // 1. 원본 이미지 배열 고정 (Pinning)
                GCHandle inputHandle = GCHandle.Alloc(context.InputSnapshots[i], GCHandleType.Pinned);
                context.PinnedHandles.Add(inputHandle);
                context.InputBufferPtrs[i] = inputHandle.AddrOfPinnedObject();

                // 2. 결과 이미지 배열 할당 및 고정
                context.ResultSnapshots[i] = new byte[imageLength];
                GCHandle resultImgHandle = GCHandle.Alloc(context.ResultSnapshots[i], GCHandleType.Pinned);
                context.PinnedHandles.Add(resultImgHandle);
                context.ResultBufferPtrs[i] = resultImgHandle.AddrOfPinnedObject();

                // 3. 결과 텍스트 배열 할당 및 고정
                context.ResultTextArrays[i] = new byte[textLength];
                GCHandle resultTextHandle = GCHandle.Alloc(context.ResultTextArrays[i], GCHandleType.Pinned);
                context.PinnedHandles.Add(resultTextHandle);
                context.ResultTextBufferPtrs[i] = resultTextHandle.AddrOfPinnedObject();
            }

            Log.Information($"Snapshot Test: 10쌍의 메모리 ({context.PinnedHandles.Count}개 GCHandle) 준비 완료.");
        }
        // 공통 정리 함수
        private void FinalizeTestContext(TestContext context)
        {
            if (context == null) return;

            // 결과 텍스트 출력
            //context.ResultText = System.Text.Encoding.UTF8.GetString(context.TextArray).TrimEnd('\0');

            // ViewModel의 속성 업데이트
            ResTxt = context.ChangedImgMetadata;
            Log.Information($"metadataOnly: {context.ChangedImgMetadata}");
            context.ResLogs.Add(context.ChangedImgMetadata);

            // 결과 이미지 변환 및 저장
            var epoch = Utilities.GetCurrentUnixTimestampMilliseconds();
            string originalImgName = Path.Combine(App.appTempDir, $"{epoch}_ori.bmp");
            string resultImagePath = Path.Combine(App.appTempDir, $"{epoch}_det.png");
            Utilities.SaveBitmap(context.SnapshotImg, originalImgName);
            Utilities.SavePng(context.ResultImg, resultImagePath);

            // UI 업데이트
            App.Current.Dispatcher.Invoke(() =>
            {
                ResImg = context.ResultImg;
            });

            // DB 저장을 위한 Test 객체 생성
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

            // 메모리 해제
            foreach (var handle in context.PinnedHandles)
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
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
            Log.Information($"{nameof(NextAsync)}");
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
            //string baseName = Utilities.GenImgName(prefix, exportPath);
            //string finalOriginalName = baseName + ".bmp";
            //string finalChangedName = baseName + ".png";
            //string finalChangedName = baseName + ".png";
            string finalOriginalName = $"{baseName}_{_tester.PcId.ToString("D3")}.bmp"; // 보간 문자열
            string finalChangedName = $"{baseName}_{_tester.PcId.ToString("D3")}.png"; // 보간 문자열

            // --- 파일 이동 및 이름 변경 ---
            Utilities.MoveTempImageToExport(_test.OriginalImg, App.appTempDir, exportPath, finalOriginalName);
            Utilities.MoveTempImageToExport(_test.ChangedImg, App.appTempDir, exportPath, finalChangedName);

            // --- 이동 후 이름을 저장용 객체에 반영 ---
            _test.OriginalImg = finalOriginalName;
            _test.ChangedImg = finalChangedName;

            // --- DB 저장 ---
            if (await _testingManagementService.SaveAsync(_test))
            {
                string tmp = _test.ToString();
                Log.Information(tmp);
                ResLogs.Add($"Add test : {tmp}");

                CellPositions cellPosition = (CellPositions)((int)_testCategory * 10 + (int)_testType);
                //SetCellPassFail(_test, cellPosition);
                _cellStatusService.SetCellPassFail(
                    _test,
                    (pos, brush) => BorderBackgrounds[(int)pos] = new ObservableBrush { Value = brush }
                );

                await TryActivateNextCategoryAsync();
            }

            TestResult = -2;
            ValidationDict[nameof(TestResult)].IsEnabled = false;
            OnTDSnChanged(TDSn);
            TDSnIsPopupOpen = false;
            IsEnvImgVisible = false;
        }


        private async Task TryActivateNextCategoryAsync()
        {
            SharedSeqNo? seqNo = await _testingManagementService.GetSeqNoAsync();

            bool existNext = false;
            bool passAll = false;

            //sn으로 id 가져오기
            int id = 0;

            switch (_testCategory)
            {
                case TestCategories.Processing:
                    //id = await GetBySnAsync(_testCategory, TDSn);
                    id = _transducer.Id;
                    existNext = _transducerModule is not null ? true : false;

                    passAll = await _testingManagementService.PassTestCategoryAsync(_testCategory, transducer: _transducer);
                    if (!existNext && id > 0 && passAll)
                    {
                        TransducerModule tdMd = new TransducerModule { Sn = $"tdm-sn{DateTime.Today.ToString("yyMMdd")}{seqNo.TDMdNo.ToString().PadLeft(3, '0')}", TransducerId = id };
                        if (await _testingManagementService.InsertTdMdAsync(tdMd))
                        {
                            await _testingManagementService.SetSeqNoAsync(SnType.TransducerModule);
                            ResLogs.Add($"Add TDMd Sn : {tdMd.Sn}");
                        }
                    }
                    else if (existNext)
                    {
                        //조회 해서 넣을까?
                        //ResLogs.Add($"Exist TDMd Sn: {_transducerModule.Sn}");
                    }

                    await _testingManagementService.PTRViewUpsertAsync(_probe, _pTRView);
                    break;
                case TestCategories.Process:

                    //id = await GetBySnAsync(_testCategory, _transducerModule.Id);
                    id = _transducerModule.Id;
                    existNext = _probe is not null ? true : false;

                    passAll = await _testingManagementService.PassTestCategoryAsync(_testCategory, transducerModule: _transducerModule);
                    if (!existNext && id > 0 && passAll)
                    {
                        //_motorModule = Controls.InputBoxMotor.Show("Motor Module", "Input Motor Module Lot", _motorModuleRepository);
                        _motorModule = _viewService.ShowInputBoxMotorView("Motor Module", "Input Motor Module Lot");
                        if (_motorModule is null) break;

                        string? probeSn = _viewService.ShowInputBoxProbeView("Probe List", "Input Probe");
                        if (probeSn is null) break;

                        Probe probe = new Probe { Sn = probeSn, TransducerModuleId = id, MotorModuleId = _motorModule.Id };
                        if (await _testingManagementService.InsertProbeAsync(probe))
                        {
                            //_motorModule = null;
                            await _testingManagementService.SetSeqNoAsync(SnType.Probe);
                            ResLogs.Add($"Add Probe Sn : {probe.Sn}");
                        }
                    }
                    else if (existNext)
                    {
                        //ResLogs.Add($"Exist Probe Sn: {_probe.Sn}");
                    }
                    await _testingManagementService.PTRViewUpsertAsync(_probe, _pTRView);
                    break;
                case TestCategories.Dispatch:
                    if (_probe is not null)
                    {
                        if (_pTRView is null)
                            await _testingManagementService.IsExistsBySnAsync(SnType.Probe, _probe.Sn);

                        await _testingManagementService.PTRViewUpsertAsync(_probe, _pTRView);
                    }
                    break;
            }
        }

        private async Task SetBySnAsync(SnType snType, string sn)
        {
            var testingData = await _testingManagementService.GetTestingDataBySnAsync(snType, sn);

            _probe = testingData.Probe;
            _transducerModule = testingData.TransducerModule;
            _transducer = testingData.Transducer;
            _pTRView = testingData.PTRView;

            Log.Information("SetBySnAsync done");
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