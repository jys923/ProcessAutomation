//#define SET_MOTOR

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HsnlibraryCS;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Utilities;
using Serilog;
using SonoCap.Commons;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories.Base;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.Services.Interfaces;
using SonoCap.MES.UI.Commons;
using SonoCap.MES.UI.Model;
using SonoCap.MES.UI.Properties;
using SonoCap.MES.UI.Services;
using SonoCap.MES.UI.Validation;
using SonoCap.MES.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static OpenTK.Windowing.GraphicsLibraryFramework.GLFWCallbacks;
using static SonoCap.MES.UI.Services.MotorService;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace SonoCap.MES.UI.ViewModels
{
    public class SubSetting { public int Id { get; set; } public string Name { get; set; } }

    public partial class TestingViewModel : ViewModelBase, IParameterReceiver
    {
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
        private int _blinkingCellIndex = (int)CellPositions.Row0_Column0;

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
                List<string> items = _transducerRepository.GetFilterItems(TDSn).Select(m => m.Sn).ToList();

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
                List<string> items = _transducerModuleRepository.GetFilterItems(TDMdSn).Select(m => m.Sn).ToList();

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
                List<string> items = _probeRepository.GetFilterItems(ProbeSn).Select(m => m.Sn).ToList();

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

        private void SetMotor2()
        {
            if (_motorService.IsOpen == true)
            {
                byte[] bytesToSend = _motorService.GenerateCommand(CMD.CMD_MODE_SEL, _motorService.GetRPMFromDensity(Convert.ToInt32(SelectedLineDensity)), _motorService.GetPRFFromDepth(Convert.ToInt32(SelectedViewDepth)));
                _motorService.Write(bytesToSend, 0, bytesToSend.Length);
            }
        }

        [ObservableProperty]
        private ObservableDictionary<int, ValidationItem> _depthIsEnabled = new();

        // ListViewDepth 초기화
        //[ObservableProperty]
        //public IList<double> _listViewDepth = new List<double> { 3, 4, 5, 6, 7 };

        //[ObservableProperty]
        //private double _selectedViewDepth;
        public double SelectedViewDepth
        {
            get { return _model.ViewDepthCm; }
            set
            {
                _model.ViewDepthCm = value;
                OnPropertyChanged(nameof(SelectedViewDepth));
                //OnSelectedViewDepthChanged(value);
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
            //Log.Information($"{nameof(CalibrateButton)} key: {}");
            _model.CalibrateScanline(CalibrationOffset);
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private int _testResult = -2;

        private BitmapImage _defaultImg = default!;

        [ObservableProperty]
        private ObservableCollection<Tuple<int, string>> _subSettingList;

        [ObservableProperty]
        private Tuple<int,string> _selectedSubSetting;

        partial void OnSelectedSubSettingChanged(Tuple<int, string> value)
        {
            if (_model.Subsetting != value.Item1)
            {
                _model.Subsetting = value.Item1;
            }
        }

        [ObservableProperty]
        private ImageSource _srcImg = default!;

        [ObservableProperty]
        private ImageSource _resImg = default!;

        [ObservableProperty]
        private string _resTxt = default!;

        [ObservableProperty]
        private TimeStampedObservableCollection<string> _resLogs = new TimeStampedObservableCollection<string>();

        [ObservableProperty]
        private string _selectedLogItem = default!;

        private Transducer? _transducer { get; set; } = default!;
        private TransducerModule? _transducerModule { get; set; } = default!;
        private MotorModule? _motorModule { get; set; } = default!;
        private Probe? _probe { get; set; } = default!;
        private PTRView? _pTRView { get; set; } = default!;
        private TestCategories _testCategory { get; set; } = default!;
        private TestTypes _testType { get; set; } = default!;
        private Test? _test { get; set; } = default!;
        private Tester? _tester { get; set; } = default!;
        private MotorState _motorState = MotorState.disconnect;

        private GlobalModel _model;
        private readonly MotorService _motorService;
        private readonly ISocketService _socketService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMotorModuleRepository _motorModuleRepository;
        private readonly IPcRepository _pcRepository;
        private readonly IProbeRepository _probeRepository;
        private readonly ISharedSeqNoRepository _sharedSeqNoRepository;
        private readonly ITestCategoryRepository _testCategoryRepository;
        private readonly ITesterRepository _testerRepository;
        private readonly ITestRepository _testRepository;
        private readonly ITestTypeRepository _testTypeRepository;
        private readonly ITransducerRepository _transducerRepository;
        private readonly ITransducerModuleRepository _transducerModuleRepository;
        private readonly ITransducerTypeRepository _transducerTypeRepository;
        private readonly IPTRViewRepository _pTRViewRepository;

        public TestingViewModel(
            GlobalModel model,
            MotorService motorService,
            ISocketService socketService,
            IServiceProvider serviceProvider,
            IMotorModuleRepository motorModuleRepository,
            IPcRepository pcRepository,
            IProbeRepository probeRepository,
            ISharedSeqNoRepository sharedSeqNoRepository,
            ITestCategoryRepository testCategoryRepository,
            ITesterRepository testerRepository,
            ITestRepository testRepository,
            ITestTypeRepository testTypeRepository,
            ITransducerRepository transducerRepository,
            ITransducerModuleRepository transducerModuleRepository,
            ITransducerTypeRepository transducerTypeRepository,
            IPTRViewRepository pTRViewRepository)
        {
            _model = model;
            _motorService = motorService;
            _socketService = socketService;
            _serviceProvider = serviceProvider;
            _motorModuleRepository = motorModuleRepository;
            _pcRepository = pcRepository;
            _probeRepository = probeRepository;
            _sharedSeqNoRepository = sharedSeqNoRepository;
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
        }

        //public void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        [RelayCommand]
        public async Task KeyDownAsync(KeyEventArgs keyEventArgs)
        {
            Key key = keyEventArgs.Key == Key.System ? keyEventArgs.SystemKey : keyEventArgs.Key;
            Log.Information($"{nameof(KeyDownAsync)} key: {key}");
            if (key == Key.F10)
            {
                // NextCommand CanExecute 상태를 갱신합니다.
                (NextCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();

                // Next 메서드를 호출합니다.
                if (NextCommand.CanExecute(null))
                {
                    await NextCommand.ExecuteAsync(null);
                }
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
            _testType = (TestTypes)col;
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
                case CellPositions.Row2_Column1:
                    BlinkingCellIndex = (int)CellPositions.Row2_Column1;
                    break;
                case CellPositions.Row2_Column2:
                    BlinkingCellIndex = (int)CellPositions.Row2_Column2;
                    break;
                case CellPositions.Row2_Column3:
                    BlinkingCellIndex = (int)CellPositions.Row2_Column3;
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
                default:
                    break;
            }

            ClearCanvas = true;
            ResImg = default!;
            TestResult = -2;
            ValidationDict[nameof(TestResult)].IsEnabled = false;
            OnTDSnChanged(TDSn);
            TDSnIsPopupOpen = false;

            //// TestCommand의 CanExecute 상태를 갱신합니다.
            (TestCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task ForcePassAsync(CellPositions position)
        {
            Log.Information($"{nameof(ForcePassAsync)} click {position}");
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
                        await ForceAllPassAsync(TestCategories.Processing);
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
                        await ForceAllPassAsync(TestCategories.Process);
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
                        await ForceAllPassAsync(TestCategories.Dispatch);
                    }
                    else
                    {
                        ResLogs.Add("Probe Sn 없음");
                    }
                    break;
                default:
                    break;
            }
            ClearCanvas = true;
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
        private async Task ForceAllPassAsync(TestCategories testCategory)
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
                    testRes = _testRepository.GetLatestTests(transducer: _transducer);
                    break;
                case TestCategories.Process:
                    testRes = _testRepository.GetLatestTests(transducerModule: _transducerModule);
                    break;
                case TestCategories.Dispatch:
                    testRes = _testRepository.GetLatestTests(probe: _probe);
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

                    if (await SaveAsync(_testRepository, insertTest))
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

                if (await SaveAsync(_testRepository, insertTest))
                {
                    ResLogs.Add($"Add test : {insertTest.ToString()}");
                }
            }

            SharedSeqNo? seqNo = await _sharedSeqNoRepository.GetSeqNoAsync();
            bool existNext = false;
            bool passAll = false;
            int id = 0;

            switch (testCategory)
            {
                case TestCategories.Processing:
                    id = _transducer.Id;
                    existNext = _transducerModule is not null ? true : false;
                    passAll = await PassTestCategoryAsync(_testRepository, _testCategory, transducer: _transducer);
                    if (!existNext && id > 0 && passAll)
                    {
                        TransducerModule tdMd = new TransducerModule { Sn = $"tdm-sn{DateTime.Today.ToString("yyMMdd")}{seqNo.TDMdNo.ToString().PadLeft(3, '0')}", TransducerId = id };
                        if (await _transducerModuleRepository.InsertAsync(tdMd))
                        {
                            _transducerModule = tdMd;
                            await _sharedSeqNoRepository.SetSeqNoAsync(SnType.TransducerModule);
                            ResLogs.Add($"Add TDMd Sn : {tdMd.Sn}");
                        }
                    }
                    break;
                case TestCategories.Process:
                    id = _transducerModule.Id;
                    existNext = _probe is not null ? true : false;
                    passAll = await PassTestCategoryAsync(_testRepository, _testCategory, transducerModule: _transducerModule);
                    if (!existNext && id > 0 && passAll)
                    {
                        _motorModule = Controls.InputBoxMotor.Show("Motor Module", "Input Motor Module Lot", _motorModuleRepository);
                        if (_motorModule is null) break;

                        Probe probe = new Probe { Sn = $"UPAG1{DateTime.Today.ToString("yyMMdd")}{seqNo.ProbeNo.ToString().PadLeft(3, '0')}", TransducerModuleId = id, MotorModuleId = _motorModule.Id };
                        if (await _probeRepository.InsertAsync(probe))
                        {
                            _probe = probe;
                            await _sharedSeqNoRepository.SetSeqNoAsync(SnType.Probe);
                            ResLogs.Add($"Add Probe Sn : {probe.Sn}");
                        }
                    }
                    break;
                case TestCategories.Dispatch:
                    break;
                default:
                    break;
            }
            await PTRViewUpsert();
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

        partial void OnTDSnChanged(string value)
        {
            TDSnFilterItems();
            TDSnIsPopupOpen = !string.IsNullOrEmpty(value) && TDSnFilteredItems.Any();

            //ChangeIsEnabled(TestCategories.Processing);
            ClearValidatingWaterMark();

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
                if (!IsExistsBySn(SnType.Transducer, value))
                {
                    ValidateField(nameof(TDSn), "TDSn Is Not Exist");
                    SetCellBackgrounds(TestCategories.All, Brushes.LightGray);
                }
                else
                {
                    BlinkingCellIndex = (int)_oldCell;
                    TdCellIsEnabled = true;
                    //셀버튼 _td _tdMd _p 각각 널이면 가로로 한줄을 끔
                    SetBySn(SnType.Transducer, value);
                    ValidateField(nameof(TDSn));

                    tests = GetTestById(SnType.Transducer, _transducer!.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        SetCellPassFail(item, cellPosition);
                    }
                    //BlinkingCellIndex = 10 + Math.Max(1, Math.Min(tests.Count, 3));

                    IQueryable<TransducerModule> query = _transducerModuleRepository.GetQueryable();
                    query = from transducerModules in query
                            where transducerModules.TransducerId == _transducer.Id
                            orderby transducerModules.Id descending
                            select transducerModules;

                    _transducerModule = query.FirstOrDefault();

                    if (_transducerModule is null)
                        return;

                    TdMdCellIsEnabled = true;

                    ValidationDict[nameof(TDMdSn)].IsEnabled = false;
                    ValidationDict[nameof(TDMdSn)].WaterMarkText = _transducerModule.Sn;

                    tests = GetTestById(SnType.TransducerModule, _transducerModule.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        SetCellPassFail(item, cellPosition);
                    }

                    //BlinkingCellIndex = 20 + Math.Max(1, Math.Min(tests.Count, 3));

                    IQueryable<Probe> queryProbe = _probeRepository.GetQueryable();
                    queryProbe = from probes in queryProbe
                                 where probes.TransducerModuleId == _transducerModule.Id
                                 orderby probes.Id descending
                                 select probes;

                    //_probe = queryProbe.FirstOrDefault();

                    _probe = queryProbe.Include(p => p.MotorModule)
                                       .FirstOrDefault();

                    if (_probe is null)
                        return;

                    _motorModule = _probe.MotorModule;

                    ProbeCellIsEnabled = true;
                    ValidationDict[nameof(ProbeSn)].IsEnabled = false;
                    ValidationDict[nameof(ProbeSn)].WaterMarkText = _probe.Sn;

                    tests = GetTestById(SnType.Probe, _probe.Id);
                    foreach (var item in tests)
                    {
                        CellPositions cellPosition = (CellPositions)(item.TestCategoryId * 10 + item.TestTypeId);
                        SetCellPassFail(item, cellPosition);
                    }
                    //BlinkingCellIndex = 30 + Math.Max(1, Math.Min(tests.Count, 3));
                }
            }
            else
            {
                ValidateField(nameof(TDSn), "TDSn Is Not Valid");
                SetCellBackgrounds(TestCategories.All, Brushes.LightGray);
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
            return validIndices.Contains(BlinkingCellIndex) && GetValidating(nameof(TDSn));

            //bool res = false;
            //switch (_testCategory)
            //{
            //    case TestCategories.Processing:
            //        if (GetValidating(nameof(TDSn)))
            //        {
            //            res = true;
            //        }
            //        break;
            //    case TestCategories.Process:

            //        if (GetValidating(nameof(TDMdSn)))
            //        {
            //            res = true;
            //        }
            //        break;
            //    case TestCategories.Dispatch:
            //        if (GetValidating(nameof(ProbeSn)))
            //        {
            //            res = true;
            //        }
            //        break;
            //}

            //return res;
        }

        [RelayCommand(CanExecute = nameof(CanTest))]
        private Task TestAsync()
        {
            Log.Information($"TestAsync response");
            //App.Current.Dispatcher.Invoke(() =>
            //{
            //ResImg = Utilities.CopyImageSource(SrcImg);
            //});

            //await App.Current.Dispatcher.InvokeAsync(async () =>
            //{
            //    ResImg = await Utilities.CopyImageSourceAsync(SrcImg);
            //});

            
            ClearDraw();
            App.Current.Dispatcher.Invoke(() =>
            {
                ResImg = Utilities.CopyBitmapSource((BitmapSource)SrcImg);

            });

            var epoch = Utilities.GetCurrentUnixTimestampMilliseconds();
            string OriginalImgName = $"{App.appSettings.Path.ExportImg}{epoch}.bmp";
            //string ChangedImgName = $"{App.appSettings.Path.ExportImg}{epoch}_mod.png";

            Utilities.ImageSourceToGrayBmp(ResImg, OriginalImgName);
            //Utilities.ImageSourceToPng(ResImg, ChangedImgName);

            // 응답 처리
            // 응답을 받았을 때의 로직
            //HansonoSettings settings = JsonSerializer.Deserialize<HansonoSettings>(response.Meta)!;
            //ResTxt = settings.ToJson();

            //ValidationDict[nameof(TestResult)].IsEnabled = true;
            return Task.CompletedTask;
        }

        private void ClearDraw()
        {
            ClearCanvas = true;
            Ellipses.Clear();
            Lines.Clear();
        }

        private bool CanNext()
        {
            Log.Information(nameof(CanNext));

            return (TestResult != -2 && GetValidating(nameof(TDSn)));
        }

        [RelayCommand(CanExecute = nameof(CanNext))]
        private async Task NextAsync()
        {
            Log.Information($"{nameof(NextAsync)}");
            //Log.Information($"ValidateAll(_testCategory) : {ValidateAll(_testCategory)}");

            PTRView? tmpPTR = null;

            if (!Utilities.EnsureFolderExists(App.appSettings.Path.ExportImg))
                return;

            var epoch = Utilities.GetCurrentUnixTimestampMilliseconds();
            string OriginalImgName = $"{App.appSettings.Path.ExportImg}{epoch}.bmp";
            string ChangedImgName = $"{App.appSettings.Path.ExportImg}{epoch}_mod.png";

            Utilities.ImageSourceToGrayBmp(SrcImg, OriginalImgName);
            Utilities.ImageSourceToPng(ResImg, ChangedImgName);

            Test insertTest = new Test
            {
                TestCategoryId = (int)_testCategory,
                TestTypeId = (int)_testType,
                TesterId = _tester.Id,
                OriginalImg = OriginalImgName,
                ChangedImg = ChangedImgName,
                ChangedImgMetadata = ResTxt,
                Result = TestResult,
                Method = 1,
            };

            PrepareTest(_testCategory, insertTest);

            if (await SaveAsync(_testRepository, insertTest))
            {
                string tmp = insertTest.ToString();
                Log.Information(insertTest.ToString());
                ResLogs.Add($"Add test : {tmp}");
            }

            //검사 결과 삭제
            ResTxt = "";

            CellPositions cellPosition = (CellPositions)((int)_testCategory * 10 + (int)_testType);
            SetCellPassFail(insertTest, cellPosition);

            SharedSeqNo? seqNo = await _sharedSeqNoRepository.GetSeqNoAsync();

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

                    passAll = await PassTestCategoryAsync(_testRepository, _testCategory, transducer: _transducer);
                    if (!existNext && id > 0 && passAll)
                    {
                        TransducerModule tdMd = new TransducerModule { Sn = $"tdm-sn{DateTime.Today.ToString("yyMMdd")}{seqNo.TDMdNo.ToString().PadLeft(3, '0')}", TransducerId = id };
                        if (await _transducerModuleRepository.InsertAsync(tdMd))
                        {
                            await _sharedSeqNoRepository.SetSeqNoAsync(SnType.TransducerModule);
                            ResLogs.Add($"Add TDMd Sn : {tdMd.Sn}");
                        }
                    }
                    else if (existNext)
                    {
                        //조회 해서 넣을까?
                        //ResLogs.Add($"Exist TDMd Sn: {_transducerModule.Sn}");
                    }

                    await PTRViewUpsert();
                    break;
                case TestCategories.Process:

                    //id = await GetBySnAsync(_testCategory, _transducerModule.Id);
                    id = _transducerModule.Id;
                    existNext = _probe is not null ? true : false;

                    passAll = await PassTestCategoryAsync(_testRepository, _testCategory, transducerModule: _transducerModule);
                    if (!existNext && id > 0 && passAll)
                    {
                        _motorModule = Controls.InputBoxMotor.Show("Motor Module", "Input Motor Module Lot", _motorModuleRepository);
                        if (_motorModule is null) break;
                        Probe probe = new Probe { Sn = $"UPAG1{DateTime.Today.ToString("yyMMdd")}{seqNo.ProbeNo.ToString().PadLeft(3, '0')}", TransducerModuleId = id, MotorModuleId = _motorModule.Id };
                        if (await _probeRepository.InsertAsync(probe))
                        {
                            //_motorModule = null;
                            await _sharedSeqNoRepository.SetSeqNoAsync(SnType.Probe);
                            ResLogs.Add($"Add Probe Sn : {probe.Sn}");
                        }
                    }
                    else if (existNext)
                    {
                        //ResLogs.Add($"Exist Probe Sn: {_probe.Sn}");
                    }
                    await PTRViewUpsert();
                    break;
                case TestCategories.Dispatch:
                    if (_probe is not null)
                    {
                        if (_pTRView is null)
                            SetBySn(SnType.Probe, _probe.Sn);
                        tmpPTR = await _probeRepository.GetPTRViewAsync(_probe.Sn);
                        if (tmpPTR is not null)
                        {
                            if (_pTRView is not null)
                                tmpPTR.Id = _pTRView.Id;

                            await _pTRViewRepository.UpsertAsync(tmpPTR);
                        }
                    }
                    break;
            }

            ClearCanvas = true;
            ResImg = default!;
            TestResult = -2;
            ValidationDict[nameof(TestResult)].IsEnabled = false;
            OnTDSnChanged(TDSn);
            TDSnIsPopupOpen = false;
        }

        private async Task PTRViewUpsert()
        {
            if (_probe is not null)
            {
                var tmpPTR = await _probeRepository.GetPTRViewAsync(_probe.Sn);
                if (tmpPTR is not null)
                {
                    if (_pTRView is not null)
                        tmpPTR.Id = _pTRView.Id;

                    await _pTRViewRepository.UpsertAsync(tmpPTR);
                }
            }
        }

        private async Task PTRViewUpsert2()
        {
            if (_probe is not null)
            {
                PTRView? tmpPTR = await _pTRViewRepository.GetPTRView(probeSn: _probe.Sn).FirstOrDefaultAsync();

                if (tmpPTR is not null)
                {
                    if (_pTRView is not null)
                        tmpPTR.Id = _pTRView.Id;

                    await _pTRViewRepository.UpsertAsync(tmpPTR);
                }
            }
        }

        private void Init()
        {
            _ellipses.CollectionChanged += Ellipses_CollectionChanged;
            _lines.CollectionChanged += Lines_CollectionChanged;

            DepthIsEnabled.Add(0, new ValidationItem { IsEnabled = true });
            DepthIsEnabled.Add(1, new ValidationItem { IsEnabled = true });
            DepthIsEnabled.Add(2, new ValidationItem { IsEnabled = true });
            DepthIsEnabled.Add(3, new ValidationItem { IsEnabled = true });
            DepthIsEnabled.Add(4, new ValidationItem { IsEnabled = true });

            LineDensityIsEnabled.Add(0, new ValidationItem { IsEnabled = true });
            LineDensityIsEnabled.Add(1, new ValidationItem { IsEnabled = true });
            LineDensityIsEnabled.Add(2, new ValidationItem { IsEnabled = true });
            LineDensityIsEnabled.Add(3, new ValidationItem { IsEnabled = true });


            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var timer = new System.Timers.Timer(1000);//1s
            timer.Elapsed += (s, e) => CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            timer.Start();

            ValidationDict[nameof(TDSn)] = new ValidationItem { WaterMarkText = $"{nameof(TDSn)}을 입력 하세요.", IsEnabled = true };
            ValidationDict[nameof(TDMdSn)] = new ValidationItem { WaterMarkText = $"{nameof(TDMdSn)}을 입력 하세요." };
            ValidationDict[nameof(ProbeSn)] = new ValidationItem { WaterMarkText = $"{nameof(ProbeSn)}을 입력 하세요." };
            ValidationDict[nameof(TestResult)] = new ValidationItem { };
            TestResult = -2;
            SetCellBackgrounds(TestCategories.All, Brushes.LightGray);

            _defaultImg = Utilities.LoadBitmapFromResource("usImg.bmp");

            // 이미지 파일 경로 설정
            string imagePath = "Resources/usImg.bmp";

            // 이미지 로드
            //SrcImg = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            //ResImg = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            SrcImg = Utilities.GetFileToImageSource(imagePath) ?? _defaultImg;
            ResImg = Utilities.GetFileToImageSource(imagePath) ?? _defaultImg;

            //_model = new GlobalModel();
            if (!_model.InitializeLibrary())
            {
                MessageBox.Show("initialize library failure");
                //this.Close();
                return;
            }
            _model.MotorStateChanged += OnMotorStateChanged;
            _model.IpCapsuleIsInnerVisible = true;

            //_selectedSubSetting = _model.Subsetting;
            List<Tuple<int, string>> subSettingList = _model.SubSettings;

            SubSettingList = new ObservableCollection<Tuple<int, string>>(subSettingList);

            SelectedSubSetting = subSettingList.Find(tuple => tuple.Item1 == _model.Subsetting);


            //string targetString = "res_fh";
            //Tuple<int, string>? result = subSettingList.Find(tuple => tuple.Item2.Contains(targetString));
            //_model.Subsetting = result.Item1;

            RenderStart();
        }

        private void Lines_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Lines));
        }

        private void Ellipses_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Ellipses));
        }

        //OnMotorStateChanged: prf_hz:10000, density:4
        private void OnMotorStateChanged(int prfHz, int density)
        {
            Log.Information($"{nameof(OnMotorStateChanged)}: prf_hz:{prfHz}, density:{density}");
            //_motorService.
            SetMotor2();
        }

        private USRenderService usRenderer;

        public void RenderStart()
        {
            usRenderer = new USRenderService(512, 512);
            usRenderer.connectRenderToTargetFunction(UpdateImageSource);
            usRenderer.RenderStart();
        }

        public void RenderEnd()
        {
            if (usRenderer != null)
            {
                usRenderer.RenderEnd();
            }
        }

        //public string LoadingMessage
        //{
        //    get { return model.IsLoading ? "Loading..." : ""; }
        //    set
        //    {
        //    }
        //}

        public void UpdateImageSource(BitmapSource bitmapSource)
        {
            //SrcImg = bitmapSource;

            App.Current.Dispatcher.Invoke(() =>
            {
                //SrcImg = Utilities.BitmapToImageSource(m_bmpRes);
                SrcImg = bitmapSource;
            });
        }

        private bool InitMotor()
        {
            //_motorService.CloseViewRequested += CloseViewRequestedHandler;
            if (_motorService.InitPort())
            {
                _motorService.DataReceived += new SerialDataReceivedEventHandler(SerialDataDataReceivedHandler);
                Log.Information($"Succ:{nameof(InitMotor)}");
                return true;
            }
            else
            {
                Log.Error($"{nameof(InitMotor)}");
                return false;
            }
        }

        private void SerialDataDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (sender is MotorService motorService)
            {
                int RecvSize = motorService.BytesToRead;
                string RecvStr = string.Empty;

                if (RecvSize >= 2)
                {
                    byte[] buff = new byte[2];
                    motorService.Read(buff, 0, 2);
                    Log.Information($"Received : {BitConverter.ToString(buff)}");

                    byte[] bytesToSend = Array.Empty<byte>();
                    if (_motorState == MotorState.disconnect)
                    {
                        _motorState = MotorState.connect;
                        bytesToSend = motorService.GetCommandBytes((int)0xAB55);
                        motorService.Write(bytesToSend, 0, bytesToSend.Length);
                    }
                    else if (_motorState == MotorState.connect)
                    {
                        _motorState = MotorState.start;
                        bytesToSend = motorService.GetCommandBytes((int)0xFA55);
                        motorService.Write(bytesToSend, 0, bytesToSend.Length);
                    }
                }
            }
        }

        private void InitSocket()
        {
            _socketService.DataReceived += DataReceivedHandler;
            _socketService.CloseViewRequested += CloseViewRequestedHandler;
            string serverIP = "127.0.0.1";
            int port = 9999;
            try
            {
                Task.Run(async () =>
                {
                    await _socketService.ConnectAsync(serverIP, port);
                    //await _socketService.ReceiveDataAsync();
                });
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(InitSocket)}: {ex.Message}");
                //Application.Current.MainWindow.Close();
            }
        }

        private void DataReceivedHandler(object? sender, ImgAndMeta response)
        {
            Log.Information($"{nameof(DataReceivedHandler)}:{response.Meta}");
            HansonoSettings settings = JsonSerializer.Deserialize<HansonoSettings>(response.Meta)!;
            //SelectedRPM = _motorService.GetRPMFromDensity(settings.density);
            //SelectedPRF = _motorService.GetPRFFromDepth((int)settings.depth_in_cm);//5개
        }

        private void CloseViewRequestedHandler(object? sender, EventArgs e)
        {
            CloseWindow();
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

        private async void LogIn()
        {
            string name = App.appSettings.TesterName;
            int pcId = App.appSettings.PcId;
            Tester tester = new Tester { Name = name, PcId = pcId };
            if (await _testerRepository.InsertAsync(tester))
            {
                IQueryable<Tester> query = _testerRepository.GetQueryable();
                query = from testers in query
                        where testers.PcId == pcId && testers.Name == name
                        orderby testers.Id descending
                        select testers;

                _tester = query.FirstOrDefault();
            }

            //꺼짐
        }

        // UI
        private void SetCellPassFail(Test item, CellPositions cellPosition)
        {
            switch (item.TestTypeId)
            {
                case 1:
                    if (item.Result > App.TestThresholdDict[11])
                    {
                        SetCellBackgrounds(Brushes.LightGreen, cellPosition);
                    }
                    else
                    {
                        SetCellBackgrounds(Brushes.Tomato, cellPosition);
                    }
                    break;
                case 2:
                    if (item.Result > App.TestThresholdDict[12])
                    {
                        SetCellBackgrounds(Brushes.LightGreen, cellPosition);
                    }
                    else
                    {
                        SetCellBackgrounds(Brushes.Tomato, cellPosition);
                    }
                    break;
                case 3:
                    if (item.Result > App.TestThresholdDict[13])
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
                BorderBackgrounds[(int)item] = new ObservableBrush { Value = brush };
            }
            //OnPropertyChanged(nameof(BorderBackgrounds));
        }

        private void SetCellBackgrounds(TestCategories category, Brush brush)
        {
            switch (category)
            {
                case TestCategories.All:
                    BorderBackgrounds[(int)CellPositions.Row1_Column1] = new ObservableBrush { Value = brush };
                    BorderBackgrounds[(int)CellPositions.Row1_Column2] = new ObservableBrush { Value = brush };
                    BorderBackgrounds[(int)CellPositions.Row1_Column3] = new ObservableBrush { Value = brush };

                    BorderBackgrounds[(int)CellPositions.Row2_Column1] = new ObservableBrush { Value = brush };
                    BorderBackgrounds[(int)CellPositions.Row2_Column2] = new ObservableBrush { Value = brush };
                    BorderBackgrounds[(int)CellPositions.Row2_Column3] = new ObservableBrush { Value = brush };

                    BorderBackgrounds[(int)CellPositions.Row3_Column1] = new ObservableBrush { Value = brush };
                    BorderBackgrounds[(int)CellPositions.Row3_Column2] = new ObservableBrush { Value = brush };
                    BorderBackgrounds[(int)CellPositions.Row3_Column3] = new ObservableBrush { Value = brush };
                    break;
                case TestCategories.Processing:
                    BorderBackgrounds[(int)CellPositions.Row1_Column1] = new ObservableBrush { Value = brush };
                    BorderBackgrounds[(int)CellPositions.Row1_Column2] = new ObservableBrush { Value = brush };
                    BorderBackgrounds[(int)CellPositions.Row1_Column3] = new ObservableBrush { Value = brush };
                    break;
                case TestCategories.Process:
                    BorderBackgrounds[(int)CellPositions.Row2_Column1] = new ObservableBrush { Value = brush };
                    BorderBackgrounds[(int)CellPositions.Row2_Column2] = new ObservableBrush { Value = brush };
                    BorderBackgrounds[(int)CellPositions.Row2_Column3] = new ObservableBrush { Value = brush };
                    break;
                case TestCategories.Dispatch:
                    BorderBackgrounds[(int)CellPositions.Row3_Column1] = new ObservableBrush { Value = brush };
                    BorderBackgrounds[(int)CellPositions.Row3_Column2] = new ObservableBrush { Value = brush };
                    BorderBackgrounds[(int)CellPositions.Row3_Column3] = new ObservableBrush { Value = brush };
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
                    _probe = _probeRepository.GetBySn(sn)
                        .Include(probe => probe.TransducerModule)
                        .Include(probe => probe.MotorModule)
                        .OrderByDescending(x => x.Id).First();
                    _transducerModule = _transducerModuleRepository.GetById(_probe.TransducerModuleId);
                    //_motorModule = _motorModuleRepository.GetById(_probe.MotorModuleId);
                    _transducer = _transducerRepository.GetById(_transducerModule.TransducerId);
                    query = from ptr in query
                            where ptr.ProbeSn == sn
                            orderby ptr.Id descending
                            select ptr;
                    break;
                case SnType.TransducerModule:
                    _transducerModule = _transducerModuleRepository.GetBySn(sn)
                        .Include(tm => tm.Transducer)
                        .OrderByDescending(x => x.Id).First();
                    _transducer = _transducerRepository.GetById(_transducerModule.TransducerId);
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

        private async Task<bool> PassTestCategoryAsync(
            ITestRepository testRepository,
            TestCategories testCategory,
            Transducer? transducer = null,
            TransducerModule? transducerModule = null,
            Probe? probe = null)
        {
            if (transducer == null && transducerModule == null && probe == null)
            {
                throw new ArgumentException("At least one of transducer, transducerModule, or probe must be provided.");
            }

            IQueryable<Test> query;
            Test latestTest;

            switch (testCategory)
            {
                case TestCategories.Processing:
                    if (transducer == null) return false; // Ensure transducer is provided

                    for (int i = 1; i < 4; i++)
                    {
                        query = from tests in testRepository.GetQueryable()
                                where tests.TransducerId == transducer.Id &&
                                      tests.TestCategoryId == 1 &&
                                      tests.TestTypeId == i
                                orderby tests.Id descending
                                select tests;

                        latestTest = await query.FirstOrDefaultAsync() ?? new Test();

                        if (latestTest.Id == 0 || latestTest.Result < App.TestThresholdDict[10 + i])
                        {
                            return false;
                        }
                    }
                    return true;

                case TestCategories.Process:
                    if (transducerModule == null) return false; // Ensure transducerModule is provided

                    for (int i = 1; i < 4; i++)
                    {
                        query = from tests in testRepository.GetQueryable()
                                where tests.TransducerModuleId == transducerModule.Id &&
                                      tests.TestCategoryId == 2 &&
                                      tests.TestTypeId == i
                                orderby tests.Id descending
                                select tests;

                        latestTest = await query.FirstOrDefaultAsync() ?? new Test();

                        if (latestTest.Id == 0 || latestTest.Result < App.TestThresholdDict[20 + i])
                        {
                            return false;
                        }
                    }
                    return true;

                case TestCategories.Dispatch:
                    if (probe == null) return false; // Ensure probe is provided

                    for (int i = 1; i < 4; i++)
                    {
                        query = from tests in testRepository.GetQueryable()
                                where tests.ProbeId == probe.Id &&
                                      tests.TestCategoryId == 3 &&
                                      tests.TestTypeId == i
                                orderby tests.Id descending
                                select tests;

                        latestTest = await query.FirstOrDefaultAsync() ?? new Test();

                        if (latestTest.Id == 0 || latestTest.Result < App.TestThresholdDict[30 + i])
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
        private async Task<bool> SaveAsync(ITestRepository testRepository, Test insertTest)
        {
            return await testRepository.InsertAsync(insertTest);
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
            //OnPropertyChanged(nameof(ValidationDict));
        }

        private void ClearValidatingWaterMark()
        {
            foreach (var item in ValidationDict)
            {
                item.Value.WaterMarkText = $"{item.Key}를 입력하세요.";
            }
        }

        // Example of using the validation methods
        public bool GetValidating(string key)
        {
            if (ValidationDict.ContainsKey(key))
            {
                return ValidationDict[key].IsValid;
            }
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

        public SubData SubData { get; set; } = default!;

        public void ReceiveParameter(object parameter)
        {
            if (parameter is SubData subData)
            {
                SubData = subData;

                Log.Information($"Received parameter {SubData.stringData}");
            }

        }

        protected override void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //base.OnWindowLoaded(sender, e);
            //MessageBox.Show("TestWindow Loaded");
            Log.Information($"{nameof(OnWindowLoaded)}");

            //Init();
            //LogIn();

            if (!InitMotor())
            {
                CloseWindow();
            }
            else
            {
                if (_motorService.IsOpen == true)
                {
                    byte[] bytesToSend = _motorService.GetCommandBytes((int)CMD.CMD_MODE_SEL);
                    _motorService.Write(bytesToSend, 0, bytesToSend.Length);
                }
            }
            //InitSocket();
        }

        protected override void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            //base.OnWindowClosing(sender, e);
            //MessageBox.Show("TestWindow Closing");
            Log.Information($"{nameof(OnWindowClosing)}");
            if (_motorService.IsOpen)
            {
                byte[] bytesToSend = _motorService.GetCommandBytes((int)CMD.CMD_MOTOR_OFF);
                _motorService.Write(bytesToSend, 0, bytesToSend.Length);
                Task.Delay(100);
                //_motorService.Close();
            }

            //_socketService.Dispose();

            //RenderEnd();
            //_model.DestroyLibrary();
            e.Cancel = true;
            if (sender is Window window) 
            {
                window.Hide();
            }
        }

        protected override void OnWindowActivated(object? sender, EventArgs e)
        {
            Log.Information($"{nameof(OnWindowActivated)}");
            if (_motorService.IsOpen == true)
            {
                byte[] bytesToSend = _motorService.GetCommandBytes((int)CMD.CMD_MOTOR_ON);
                _motorService.Write(bytesToSend, 0, bytesToSend.Length);
            }
        }

        System.Windows.Point _startPoint = new System.Windows.Point(0, 0);
        System.Windows.Point _previousPoint = new System.Windows.Point(0, 0);

        [ObservableProperty]
        private bool _clearCanvas = false;

        [ObservableProperty]
        private ObservableCollection<CustomEllipse> _ellipses = new();

        [ObservableProperty]
        private ObservableCollection<CustomLine> _lines = new();

        [ObservableProperty]
        private bool _isDrawing;

        [RelayCommand]
        private void OnMouseDown(MouseButtonEventArgs e)
        {
            switch (_testType)
            {
                case TestTypes.Align:
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        _startPoint = e.GetPosition((IInputElement)e.Source);
                        // 좌표가 (0,0), (0,1), (1,0)인 경우 무시
                        if ((_startPoint.X == 0 && _startPoint.Y == 0) || (_startPoint.X == 0) || (_startPoint.Y == 0))
                        {
                            Log.Information($"Ignored Point X:{_startPoint.X} Y:{_startPoint.Y}");
                            return;
                        }
                        Mouse.Capture((IInputElement)e.Source);
                        IsDrawing = true;
                        _previousPoint = _startPoint;
                        Log.Information($"_startPoint X:{_startPoint.X} Y:{_startPoint.Y}");

                        if (e.Source is FrameworkElement canvas)
                        {
                            double canvasWidth = canvas.ActualWidth;
                            double canvasHeight = canvas.ActualHeight;

                            double centerX = canvasWidth / 2;
                            double centerY = canvasHeight / 2;

                            double Radius = Math.Sqrt(Math.Pow(_startPoint.X - centerX, 2) + Math.Pow(_startPoint.Y - centerY, 2));

                            CustomEllipse tmp = new CustomEllipse();
                            tmp.Ellipse.Width = 2 * Radius;
                            tmp.Ellipse.Height = 2 * Radius;

                            Canvas.SetLeft(tmp.Ellipse, centerX - Radius);
                            Canvas.SetTop(tmp.Ellipse, centerY - Radius);

                            Canvas.SetLeft(tmp.InfoTextBlock, _startPoint.X);
                            Canvas.SetTop(tmp.InfoTextBlock, _startPoint.Y);

                            if (Ellipses.Count >= 2)
                            {
                                Ellipses.RemoveAt(0);
                            }
                            Ellipses.Add(tmp);
                        }
                    }
                    break;
                case TestTypes.Axial:
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Mouse.Capture((IInputElement)e.Source);
                        IsDrawing = true;
                        _startPoint = e.GetPosition((IInputElement)e.Source);
                        _previousPoint = e.GetPosition((IInputElement)e.Source);
                        Log.Information($"_previousPoint x:{_previousPoint.X}, y:{_previousPoint.Y} ");

                        CustomEllipse tmp = new CustomEllipse();

                        // 초기 위치 설정
                        Canvas.SetLeft(tmp.Ellipse, _startPoint.X);
                        Canvas.SetTop(tmp.Ellipse, _startPoint.Y);

                        Canvas.SetLeft(tmp.InfoTextBlock, _startPoint.X);
                        Canvas.SetTop(tmp.InfoTextBlock, _startPoint.Y);

                        if (Ellipses.Count >= 5)
                        {
                            Ellipses.RemoveAt(0);
                        }
                        Ellipses.Add(tmp);
                    }
                    break;
                case TestTypes.Lateral:
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Mouse.Capture((IInputElement)e.Source);
                        IsDrawing = true;
                        _startPoint = e.GetPosition((IInputElement)e.Source);
                        _previousPoint = e.GetPosition((IInputElement)e.Source);

                        CustomLine tmp = new CustomLine();

                        tmp.Line.X1 = _startPoint.X;
                        tmp.Line.Y1 = _startPoint.Y;
                        tmp.Line.X2 = _startPoint.X;
                        tmp.Line.Y2 = _startPoint.Y;

                        Canvas.SetLeft(tmp.InfoTextBlock, _startPoint.X);
                        Canvas.SetTop(tmp.InfoTextBlock, _startPoint.Y);

                        if (Lines.Count >= 2)
                        {
                            Lines.RemoveAt(0);
                        }
                        Lines.Add(tmp);
                    }
                    break;
                default:
                    break;
            }
        }

        [RelayCommand]
        private void OnMouseMove(MouseEventArgs e)
        {
            switch (_testType)
            {
                case TestTypes.Align:
                    //if (IsDrawing && CurrentEllipse != null)
                    //{
                    //    System.Windows.Point currentPoint = e.GetPosition((IInputElement)e.Source);

                    //    // 이미지 중심 계산
                    //    double centerX = 512 / 2;
                    //    double centerY = 512 / 2;

                    //    // 반지름 계산
                    //    double radius = Math.Sqrt(Math.Pow(currentPoint.X - centerX, 2) + Math.Pow(currentPoint.Y - centerY, 2));
                    //    CurrentEllipse.Width = 2 * radius;
                    //    CurrentEllipse.Height = 2 * radius;

                    //    Canvas.SetLeft(CurrentEllipse, centerX - radius);
                    //    Canvas.SetTop(CurrentEllipse, centerY - radius);
                    //}
                    break;
                case TestTypes.Axial:
                    if (IsDrawing && Ellipses.Count > 0)
                    {
                        System.Windows.Point currentPoint = e.GetPosition((IInputElement)e.Source);
                        CustomEllipse currentEllipse = Ellipses[Ellipses.Count - 1];
                        Log.Information($"currentPoint x:{currentPoint.X}, y:{currentPoint.Y} ");

                        // X2, Y2와의 차이를 계산
                        double deltaX = Math.Abs(currentPoint.X - _previousPoint.X);
                        double deltaY = Math.Abs(currentPoint.Y - _previousPoint.Y);
                        //Log.Information($"delta x:{deltaY}, y:{deltaY} ");
                        // X나 Y 차이가 30 이하인 경우에만 처리
                        if (deltaX <= 50 && deltaY <= 50)
                        {
                            _previousPoint = currentPoint;
                            // 반지름 계산
                            double radius = Math.Sqrt(Math.Pow(currentPoint.X - _startPoint.X, 2) + Math.Pow(currentPoint.Y - _startPoint.Y, 2));

                            double pixelValue = Utilities.GetCirclePixelMean((BitmapSource)SrcImg, (int)_startPoint.X, (int)_startPoint.Y, (int)radius, (int)currentEllipse.Ellipse.StrokeThickness);
                            //Log.Information($"Pixel Value at ({_startPoint.X}, {_startPoint.Y}): {pixelValue}");

                            currentEllipse.Ellipse.Width = 2 * radius;
                            currentEllipse.Ellipse.Height = 2 * radius;

                            currentEllipse.InfoTextBlock.Text = ((int)pixelValue).ToString();

                            // 중앙점 기준 위치 조정
                            Canvas.SetLeft(currentEllipse.Ellipse, _startPoint.X - radius);
                            Canvas.SetTop(currentEllipse.Ellipse, _startPoint.Y - radius);
                            Canvas.SetLeft(currentEllipse.InfoTextBlock, currentPoint.X);
                            Canvas.SetTop(currentEllipse.InfoTextBlock, currentPoint.Y);
                        }
                    }
                    break;
                case TestTypes.Lateral:
                    if (IsDrawing && Lines.Count > 0)
                    {
                        System.Windows.Point currentPoint = e.GetPosition((IInputElement)e.Source);
                        CustomLine currentLine = Lines[Lines.Count - 1];
                        Log.Information($"currentPoint x:{currentPoint.X}, y:{currentPoint.Y} ");
                        // X2, Y2와의 차이를 계산
                        double deltaX = Math.Abs(currentPoint.X - currentLine.Line.X2);
                        double deltaY = Math.Abs(currentPoint.Y - currentLine.Line.Y2);

                        // X나 Y 차이가 30 이하인 경우에만 처리
                        if (deltaX <= 50 && deltaY <= 50)
                        {
                            _previousPoint = currentPoint;

                            currentLine.Line.X2 = currentPoint.X;
                            currentLine.Line.Y2 = currentPoint.Y;
                            Log.Information($"X2:Y2 {currentLine.Line.X2}:{currentLine.Line.Y2}");
                            double length = Math.Sqrt(Math.Pow(currentLine.Line.X2 - currentLine.Line.X1, 2) + Math.Pow(currentLine.Line.Y2 - currentLine.Line.Y1, 2));

                            currentLine.InfoTextBlock.Text = ((int)length).ToString();
                            Canvas.SetLeft(currentLine.InfoTextBlock, (currentLine.Line.X2 + currentLine.Line.X1)/2);
                            Canvas.SetTop(currentLine.InfoTextBlock, (currentLine.Line.Y2 + currentLine.Line.Y1) / 2);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        [RelayCommand]
        private void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                IsDrawing = false;
                Mouse.Capture(null); // 마우스 캡처 해제
            }
        }
    }
}
