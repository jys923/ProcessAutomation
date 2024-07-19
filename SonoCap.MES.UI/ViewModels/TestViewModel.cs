using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Operation.Buffer;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories.Base;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.Services;
using SonoCap.MES.Services.Interfaces;
using SonoCap.MES.UI.Commons;
using SonoCap.MES.UI.Validation;
using SonoCap.MES.UI.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VILib;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class TestViewModel : ViewModelBase, IParameterReceiver
    {
        [ObservableProperty]
        private string _title = default!;

        [ObservableProperty]
        private string _currentTime = DateTime.Today.ToString("yyyy-MM-dd");

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TestCommand))]
        private ObservableDictionary<string, ValidationItem> _validationDict = new();

        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(TestCommand))]
        private bool _tdCellIsEnabled = true;

        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(TestCommand))]
        private bool _tdMdCellIsEnabled = false;

        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(TestCommand))]
        private bool _probeCellIsEnabled = false;

        [ObservableProperty]
        private ObservableDictionary<int, ObservableBrush> _borderBackgrounds = new();

        [ObservableProperty]
        private int _blinkingCellIndex = -1;

        private int _oldRow = -1;
        private int _oldCol = -1;
        private CellPositions _oldCell = (CellPositions)(-1);

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

            Log.Information($"TDSnKeyDown : {e.Key}");
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

        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(TestCommand))]
        //[NotifyCanExecuteChangedFor(nameof(NextCommand))]
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

        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(TestCommand))]
        //[NotifyCanExecuteChangedFor(nameof(NextCommand))]
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

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private int _testResult = -2;
        
        [ObservableProperty]
        private ImageSource _srcImg = default!;

        [ObservableProperty]
        private ImageSource _resImg = default!;

        [ObservableProperty]
        private string _resTxt = default!;

        [ObservableProperty]
        private ObservableCollection<string> _resLogs = new ObservableCollection<string>();

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

        public TestViewModel(
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
        private void CellClick(CellPositions position)
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

            // TestCommand의 CanExecute 상태를 갱신합니다.
            (TestCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void CellRightClick(CellPositions position)
        {
            if (_oldCell != position)
            {
                ResLogs.Add($"fail start test {position}");
                return;
            }

            // TestCommand의 CanExecute 상태를 갱신합니다.
            (TestCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();

            // Test 메서드를 호출합니다.
            if (TestCommand.CanExecute(null))
            {
                TestCommand.Execute(null);
            }
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
            //_motorModule = null;
            _pTRView = null;

            TdMdCellIsEnabled = false;
            ProbeCellIsEnabled = false;

            //BlinkingCellIndex = -1;

            List<Test> tests;
            //정규 표현식 검증 추가
            if (value.Length > 10)
            {
                Log.Information($"Transducer sn {value}");
                if (!IsExistsBySn(SnType.Transducer, value))
                {
                    ValidateField(nameof(TDSn), "TDSn Is Not Exist");
                    SetCellBackgrounds(TestCategories.All, Brushes.LightGray);

                }
                else
                {
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

                    _probe = queryProbe.FirstOrDefault();

                    if (_probe is null)
                        return;
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

#if false
        partial void OnTDMdSnChanged(string value)
        {
            TDMdSnFilterItems();
            TDMdSnIsPopupOpen = !string.IsNullOrEmpty(value) && TDMdSnFilteredItems.Any();

            //ChangeIsEnabled(TestCategories.Process);
            ClearValidatingWaterMark();

            _probe = null;
            _transducerModule = null;
            _transducer = null;
            //_motorModule = null;
            _pTRView = null;

            List<Test> tests;
            //정규 표현식 검증 추가
            if (value.Length > 10)
            {
                Log.Information($"TransducerModule sn {value}");
                if (!IsExistsBySn(SnType.TransducerModule, value))
                {
                    ValidateField(nameof(TDMdSn), "TDMdSn Is Not Exist");
                    SetCellBackgrounds(TestCategories.All, Brushes.LightGray);
                }
                else
                {
                    SetBySn(SnType.TransducerModule, value);
                    ValidateField(nameof(TDMdSn));
                    //ValidationDict[nameof(TDSn)].IsEnabled = false;
                    ValidationDict[nameof(TDSn)].WaterMarkText = _transducerModule.Transducer.Sn;

                    tests = GetTestById(SnType.TransducerModule, _transducerModule.Id);
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

                    _probe = query.FirstOrDefault();

                    if (_probe is null)
                        return;

                    //_motorModule = _probe.MotorModule;
                    ValidationDict[nameof(ProbeSn)].IsEnabled = false;
                    ValidationDict[nameof(ProbeSn)].WaterMarkText = _probe.Sn;

                    tests = GetTestById(SnType.Probe, _probe.Id);
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
                SetCellBackgrounds(TestCategories.All, Brushes.LightGray);
            }
        }

        partial void OnProbeSnChanged(string value)
        {
            ProbeSnFilterItems();
            ProbeSnIsPopupOpen = !string.IsNullOrEmpty(value) && ProbeSnFilteredItems.Any();
            //ChangeIsEnabled(TestCategories.Dispatch);
            ClearValidatingWaterMark();

            _probe = null;
            _transducerModule = null;
            _transducer = null;
            //_motorModule = null;
            _pTRView = null;

            List<Test> tests;
            //정규 표현식 검증 추가
            if (value.Length > 10)
            {
                Log.Information($"Probe sn {value}");
                if (!IsExistsBySn(SnType.Probe, value))
                {
                    ValidateField(nameof(ProbeSn), "ProbeSn Is Not Exist");
                    SetCellBackgrounds(TestCategories.All, Brushes.LightGray);
                }
                else
                {
                    SetBySn(SnType.Probe, value);
                    ValidateField(nameof(ProbeSn));
                    ValidationDict[nameof(TDMdSn)].IsEnabled = false;
                    ValidationDict[nameof(TDMdSn)].WaterMarkText = _probe.TransducerModule.Sn;
                    //ValidationDict[nameof(TDSn)].IsEnabled = false;
                    ValidationDict[nameof(TDSn)].WaterMarkText = _probe.TransducerModule.Transducer.Sn;

                    tests = GetTestById(SnType.Probe, _probe.Id);
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
                SetCellBackgrounds(TestCategories.All, Brushes.LightGray);
            }
        } 
#endif

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
            return BlinkingCellIndex != -1 && GetValidating(nameof(TDSn));
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
        private async Task TestAsync()
        {
            // 데이터 전송
            await _socketService.SendDataAsync("1");

            // 응답을 기다립니다.
            byte[]? response = await _socketService.WaitForResponseAsync();
            // 응답 처리
            if (response != null)
            {
                Log.Information($"TestAsync response");
                // 응답을 받았을 때의 로직
                // 데이터를 받으면 응답 완료
                Bitmap m_bmpRes = new Bitmap(512, 512, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Utilities.ByteArrToBitmap(response, m_bmpRes);

                App.Current.Dispatcher.Invoke(() =>
                {
                    SrcImg = Utilities.BitmapToImageSource(m_bmpRes);
                });

                //draw circle
                Utilities.DrawCircle(m_bmpRes, 512 / 2, 512 / 2, 150, System.Drawing.Color.Red, 3);
                App.Current.Dispatcher.Invoke(() =>
                {
                    ResImg = Utilities.BitmapToImageSource(m_bmpRes);
                    ResTxt = "draw cicle";
                    ResLogs.Add($"Succ Test");
                    TestResult = -2;
                });

                ValidationDict[nameof(TestResult)].IsEnabled = true;
            }
            else
            {
                // 응답을 받지 못했을 때의 로직
                // 예: 타임아웃 처리 등
            }
        }

        private bool CanNext()
        {
            Log.Information(nameof(CanNext));

            return (TestResult != -2 && GetValidating(nameof(TDSn)));


            //if (TestResult == -2)
            //{
            //    return false;
            //}

            //bool res = false;

            //if (GetValidating(nameof(TDSn)))
            //{
            //    res = true;
            //}

            //return res;

            //if (TestResult == -2)
            //{
            //    return false;
            //}

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

        [RelayCommand(CanExecute = nameof(CanNext))]
        private async Task NextAsync() 
        {
            Log.Information($"{nameof(NextAsync)}");
            //Log.Information($"ValidateAll(_testCategory) : {ValidateAll(_testCategory)}");

            if (_testCategory == TestCategories.Process &&
                TestResult > App.TestThresholdDict[20 + (int)_testType] &&
                (PassTestCategoryCnt(_testRepository, _testCategory, _transducerModule.Id) == 2))
            {
                _motorModule = Controls.InputBoxMotor.Show("Motor Module", "Input Motor Module Lot", _motorModuleRepository);

                if (_motorModule == null) return;
            }
            PTRView? tmpPTR = null;

            var epoch = Utilities.GetCurrentUnixTimestampSeconds();

            string OriginalImgName = $"{epoch}_O.bmp";
            string ChangedImgName = $"{epoch}_C.bmp";

            Utilities.ImageSourceToBitmapFile(SrcImg, OriginalImgName);
            Utilities.ImageSourceToBitmapFile(ResImg, ChangedImgName);

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

            if(await SaveAsync(_testRepository, insertTest))
            {
                ResLogs.Add($"Add test : {insertTest.ToJson()}");
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
                    
                    passAll = await PassTestCategoryAsync(_testRepository, _testCategory, id);
                    if ( !existNext && id > 0 && passAll)
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

                    if (_probe is not null)
                    {
                        tmpPTR = await _probeRepository.GetPTRViewAsync(_probe.Sn);
                        if (tmpPTR is not null)
                        {
                            if (_pTRView is not null)
                                tmpPTR.Id = _pTRView.Id;

                            await _pTRViewRepository.UpsertAsync(tmpPTR);
                        }
                    }

                    break;
                case TestCategories.Process:
                    //id = await GetBySnAsync(_testCategory, _transducerModule.Id);
                    id = _transducerModule.Id;
                    existNext = _probe is not null ? true : false;
                    
                    passAll = await PassTestCategoryAsync(_testRepository, _testCategory, id);
                    if (!existNext && id > 0 && passAll)
                    {
                        Probe probe = new Probe { Sn = $"UPAG1{DateTime.Today.ToString("yyMMdd")}{seqNo.ProbeNo.ToString().PadLeft(3, '0')}", TransducerModuleId = id, MotorModuleId = _motorModule.Id };
                        if (await _probeRepository.InsertAsync(probe))
                        {
                            _motorModule = null;
                            await _sharedSeqNoRepository.SetSeqNoAsync(SnType.Probe);
                            ResLogs.Add($"Add Probe Sn : {probe.Sn}");
                        }
                    } else if (existNext)
                    {
                        //ResLogs.Add($"Exist Probe Sn: {_probe.Sn}");
                    }

                    if (_probe is not null)
                    {
                        tmpPTR = await _probeRepository.GetPTRViewAsync(_probe.Sn);
                        if (tmpPTR is not null)
                        {
                            if (_pTRView is not null)
                                tmpPTR.Id = _pTRView.Id;

                            await _pTRViewRepository.UpsertAsync(tmpPTR);
                        }
                    }
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

            SrcImg = default!;
            ResImg = default!;
            TestResult = -2;
            ValidationDict[nameof(TestResult)].IsEnabled = false;
            OnTDSnChanged(TDSn);
        }

        private void Init()
        {
            //// 서버 IP 주소와 포트 번호
            string serverIP = "127.0.0.1";
            int port = 9999;
            Task.Run(async () =>
            {
                try
                {
                    await _socketService.ConnectAsync(serverIP, port);
                    await _socketService.ReceiveDataAsync();
                }
                catch (Exception ex)
                {
                    // 예외 처리 필요
                    Log.Information($"연결 및 데이터 수신 오류: {ex.Message}");
                }
            });

            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var timer = new System.Timers.Timer(1000);//1시간 마다
            timer.Elapsed += (s, e) => CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            timer.Start();

            ValidationDict[nameof(TDSn)] = new ValidationItem { WaterMarkText = $"{nameof(TDSn)}을 입력 하세요.", IsEnabled = true };
            ValidationDict[nameof(TDMdSn)] = new ValidationItem { WaterMarkText = $"{nameof(TDMdSn)}을 입력 하세요." };
            ValidationDict[nameof(ProbeSn)] = new ValidationItem { WaterMarkText = $"{nameof(ProbeSn)}을 입력 하세요." };
            ValidationDict[nameof(TestResult)] = new ValidationItem { };
            TestResult = -2;
            SetCellBackgrounds(TestCategories.All, Brushes.LightGray);

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
            switch (categories)
            {
                case TestCategories.Processing:
                    ValidationDict[nameof(TDSn)].IsEnabled = true;
                break;
                case TestCategories.Process:
                    ValidationDict[nameof(TDMdSn)].IsEnabled = true;
                break;
                case TestCategories.Dispatch:
                    ValidationDict[nameof(ProbeSn)].IsEnabled = true;
                break;
            }
        }

        private void ClearAll()
        {
            ProbeSn = "";
            ValidationDict[nameof(ProbeSn)].IsEnabled = false;
            TDMdSn = "";
            ValidationDict[nameof(TDMdSn)].IsEnabled = false;
            TDSn = "";
            ValidationDict[nameof(TDSn)].IsEnabled = false;
            SrcImg = default!;
            ResImg = default!;
            TestResult = -2;
            _probe = null;
            _transducerModule = null;
            _transducer = null;
            //_motorModule = null;
            SetCellBackgrounds(TestCategories.All, Brushes.LightGray);
        }

        private void ClearAll(TestCategories category)
        {
            switch (category)
            {
                case TestCategories.Processing:
                    ProbeSn = "";
                    TDMdSn = "";
                    break;
                case TestCategories.Process:
                    ProbeSn = "";
                    TDSn = "";
                    break;
                case TestCategories.Dispatch:
                    TDMdSn = "";
                    TDSn = "";
                    break;
            }
            //ValidationDict[nameof(TDSn)].IsEnabled = false;
            ValidationDict[nameof(TDMdSn)].IsEnabled = false;
            ValidationDict[nameof(ProbeSn)].IsEnabled = false;
            SrcImg = default!;
            ResImg = default!;
            TestResult = -2;
            _probe = null;
            _transducerModule = null;
            _transducer = null;
            //_motorModule = null;
            SetCellBackgrounds(TestCategories.All, Brushes.LightGray);
        }

        private void ClearCellBackgrounds()
        {
            BorderBackgrounds[11] = new ObservableBrush { Value = Brushes.LightBlue };
            BorderBackgrounds[12] = new ObservableBrush { Value = Brushes.LightBlue };
            BorderBackgrounds[13] = new ObservableBrush { Value = Brushes.LightBlue };

            BorderBackgrounds[21] = new ObservableBrush { Value = Brushes.LightBlue };
            BorderBackgrounds[22] = new ObservableBrush { Value = Brushes.LightBlue };
            BorderBackgrounds[23] = new ObservableBrush { Value = Brushes.LightBlue };

            BorderBackgrounds[31] = new ObservableBrush { Value = Brushes.LightBlue };
            BorderBackgrounds[32] = new ObservableBrush { Value = Brushes.LightBlue };
            BorderBackgrounds[33] = new ObservableBrush { Value = Brushes.LightBlue };
        }

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
                    _transducerModule =  _transducerModuleRepository.GetById(_probe.TransducerModuleId);
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

        private async Task<bool> IsExistsBySnAsync(SnType snType, string sn)// => snType switch
        {
            return snType switch
            {
                SnType.Probe => await _probeRepository.GetBySn(sn).AnyAsync(),
                SnType.TransducerModule => await _transducerModuleRepository.GetBySn(sn).AnyAsync(),
                SnType.Transducer => await _transducerRepository.GetBySn(sn).AnyAsync(),
                SnType.MotorModule => await _motorModuleRepository.GetBySn(sn).AnyAsync(),
                _ => false
            };
        }

        private async Task<bool> IsExistsBySnAsync2(SnType snType, string sn)
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

        private int PassTestCategoryCnt(ITestRepository testRepository, TestCategories testCategory, int id)
        {
            int res = 0;

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
                                      tests.Result > App.TestThresholdDict[10 + i]
                                orderby tests.Id descending
                                select tests;

                        if (query.Any())
                        {
                            res++;
                        }
                    }
                    break;

                case TestCategories.Process:
                    for (int i = 1; i < 4; i++)
                    {
                        query = from tests in _testRepository.GetQueryable()
                                where tests.TransducerModuleId == id &&
                                      tests.TestCategoryId == 2 &&
                                      tests.TestTypeId == i &&
                                      tests.Result > App.TestThresholdDict[20 + i]
                                orderby tests.Id descending
                                select tests;

                        if (query.Any())
                        {
                            res++;
                        }
                    }
                    break;

                case TestCategories.Dispatch:
                    for (int i = 1; i < 4; i++)
                    {
                        query = from tests in _testRepository.GetQueryable()
                                where tests.ProbeId == id &&
                                      tests.TestCategoryId == 3 &&
                                      tests.TestTypeId == i &&
                                      tests.Result > App.TestThresholdDict[30 + i]
                                orderby tests.Id descending
                                select tests;

                        if (query.Any())
                        {
                            res++;
                        }
                    }
                    break;
            }

            return res;
        }

        private bool PassTestCategory(ITestRepository testRepository, TestCategories testCategory, int id)
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
                                      tests.Result > App.TestThresholdDict[10 + i]
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
                                where tests.TransducerModuleId == id &&
                                      tests.TestCategoryId == 2 &&
                                      tests.TestTypeId == i &&
                                      tests.Result > App.TestThresholdDict[20 + i]
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
                                where tests.ProbeId == id &&
                                      tests.TestCategoryId == 3 &&
                                      tests.TestTypeId == i &&
                                      tests.Result > App.TestThresholdDict[30 + i]
                                orderby tests.Id descending
                                select tests;

                        if (!query.Any())
                        {
                            return false;
                        }
                    }
                    return true;

                default:
                    return false;
            }
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
                                      tests.Result > App.TestThresholdDict[10 + i]
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
                                      tests.Result > App.TestThresholdDict[20 + i]
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
                                      tests.Result > App.TestThresholdDict[30 + i]
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

        private int? GetIdBySnType(SnType snType, int id)
        {
            switch (snType)
            {
                case SnType.TransducerModule:
                    return GetFirstId(_probeRepository, "TransducerModuleId", id);
                case SnType.Transducer:
                    return GetFirstId(_transducerModuleRepository, "TransducerId", id);
                default:
                    return null;
            }
        }

        private int? GetFirstId<T>(IRepositoryBase<T> repository, string idFieldName, int id) where T : class
        {
            var query = repository.GetQueryable();
            query = from item in query
                    where (int)item.GetType().GetProperty(idFieldName).GetValue(item) == id
                    orderby (int)item.GetType().GetProperty("Id").GetValue(item)
                    select item;

            var firstItem = query.FirstOrDefault();
            return firstItem != null ? (int)firstItem.GetType().GetProperty("Id").GetValue(firstItem) : (int?)null;
        }

        public bool IsIdUsed(SnType snType, int id)
        {
            switch (snType)
            {
                case SnType.TransducerModule:
                    return IsIdUsedInRepository(_probeRepository, "TransducerModuleId", id);
                case SnType.Transducer:
                    return IsIdUsedInRepository(_transducerModuleRepository, "TransducerId", id);
                default:
                    return false;
            }
        }

        private bool IsIdUsedInRepository<T>(IRepositoryBase<T> repository, string idFieldName, int id) where T : class
        {
            var query = repository.GetQueryable();
            query = from item in query
                    where (int)item.GetType().GetProperty(idFieldName).GetValue(item) == id
                    select item;

            return query.Any();
        }

        public T? GetEntityIfIdUsed<T>(SnType snType, int id) where T : class
        {
            switch (snType)
            {
                case SnType.TransducerModule:
                    return GetEntityIfIdUsedInRepository(_probeRepository, "TransducerModuleId", id) as T;
                case SnType.Transducer:
                    return GetEntityIfIdUsedInRepository(_transducerModuleRepository, "TransducerId", id) as T;
                default:
                    return null;
            }
        }

        private T? GetEntityIfIdUsedInRepository<T>(IRepositoryBase<T> repository, string idFieldName, int id) where T : class
        {
            var query = repository.GetQueryable();
            query = from item in query
                    where (int)item.GetType().GetProperty(idFieldName).GetValue(item) == id
                    select item;

            return query.FirstOrDefault() as T;
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

            // 각 TestCategory에 대한 필드 검증
            switch (test)
            {
                case TestCategories.Processing:
                    if (!ValidateField(nameof(TDSn), TDSn))
                        return false;
                    break;
                case TestCategories.Process:
                    if (!ValidateField(nameof(TDMdSn), TDMdSn))
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

        public SubData SubData { get; set; } = default!;
        
        public void ReceiveParameter(object parameter)
        {
            if(parameter is SubData subData)
            {
                SubData = subData;

                Log.Information($"Received parameter {SubData.stringData}");
            }
            
        }

        protected override void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //base.OnWindowLoaded(sender, e);
            //MessageBox.Show("TestWindow Loaded");
        }

        protected override void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            //base.OnWindowClosing(sender, e);
            //MessageBox.Show("TestWindow Closing");
        }
    }
}
