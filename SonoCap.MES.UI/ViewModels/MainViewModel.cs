using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonoCap.MES.UI.Commons;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Win32;
using SonoCap.MES.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SonoCap.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private ProbeListView? _probeListView = default!;
        private TestListView? _testListView = default!;
        private TestView? _testView = default!;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMotorModuleRepository _motorModuleRepository;
        private readonly IPcRepository _pcRepository;
        private readonly IProbeRepository _probeRepository;
        private readonly IPTRViewRepository _pTRViewRepository;
        private readonly ISharedSeqNoRepository _sharedSeqNoRepository;
        private readonly ITestCategoryRepository _testCategoryRepository;
        private readonly ITesterRepository _testerRepository;
        private readonly ITestRepository _testRepository;
        private readonly ITestTypeRepository _testTypeRepository;
        private readonly ITransducerRepository _transducerRepository;
        private readonly ITransducerModuleRepository _transducerModuleRepository;
        private readonly ITransducerTypeRepository _transducerTypeRepository;

        [ObservableProperty]
        private bool _isBlinking = false;

        [ObservableProperty]
        private string _title = default!;

        public MainViewModel(
            IServiceProvider serviceProvider,
            IMotorModuleRepository motorModuleRepository,
            IPcRepository pcRepository,
            IProbeRepository probeRepository,
            IPTRViewRepository pTRViewRepository,
            ISharedSeqNoRepository sharedSeqNoRepository,
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
            _pTRViewRepository = pTRViewRepository;
            _sharedSeqNoRepository = sharedSeqNoRepository; 
            _testCategoryRepository = testCategoryRepository;
            _testerRepository = testerRepository;
            _testRepository = testRepository;
            _testTypeRepository = testTypeRepository;
            _transducerRepository = transducerRepository;
            _transducerModuleRepository = transducerModuleRepository;
            _transducerTypeRepository = transducerTypeRepository;
            Title = this.GetType().Name;

            Items = new ObservableCollection<string>
            {
                "Apple", "Banana", "Cherry", "Date", "Elderberry",
                "Fig", "Grape", "Honeydew", "Ice Cream Bean", "Jackfruit",
                "Kiwi", "Lemon", "Mango", "Nectarine", "Orange",
                "Papaya", "Quince", "Raspberry", "Strawberry", "Tomato",
                "Ugli Fruit", "Vanilla", "Watermelon", "Xigua", "Yellow Passionfruit",
                "Zucchini"
            };

            FilteredItems = new ObservableCollection<string>(Items);
            KeyDownCommand = new RelayCommand<KeyEventArgs>(OnKeyDown);
        }

        [RelayCommand]
        private void ToBlink()
        {
            IsBlinking = !IsBlinking;
            //Task.Run(() => DoBackgroundWork());
        }

        [RelayCommand]
        private void ToTest()
        {
            // 이미 열려 있는 창이 있는지 확인
            if (_testView == null || !_testView.IsLoaded)
            {
                _testView = _serviceProvider.GetRequiredService<TestView>();
                //_testView.DataContext = _serviceProvider.GetRequiredService<TestViewModel>();
                _testView.Show();
            }
            else
            {
                // 창이 최소화되어 있으면 최상위로 올리기
                if (_testView.WindowState == System.Windows.WindowState.Minimized)
                {
                    _testView.WindowState = System.Windows.WindowState.Normal;
                }

                // 창을 최상위로 가져오기
                _testView.Activate();
            }
        }

        [RelayCommand]
        private void ToList()
        {
            if (_probeListView == null || !_probeListView.IsLoaded)
            {
                _probeListView = _serviceProvider.GetRequiredService<ProbeListView>();
                _probeListView.Show();
            }
            else
            {
                if (_probeListView.WindowState == System.Windows.WindowState.Minimized)
                {
                    _probeListView.WindowState = System.Windows.WindowState.Normal;
                }

                _probeListView.Activate();
            }
        }

        [RelayCommand]
        private void ToTestList()
        {
            if (_testListView == null || !_testListView.IsLoaded)
            {
                _testListView = _serviceProvider.GetRequiredService<TestListView>();
                _testListView.Show();
            }
            else
            {
                if (_testListView.WindowState == System.Windows.WindowState.Minimized)
                {
                    _testListView.WindowState = System.Windows.WindowState.Normal;
                }

                _testListView.Activate();
            }
        }

        [RelayCommand]
        private async Task Master()
        {
            await _pcRepository.InsertAsync(new Pc { Name = "left" });
            await _pcRepository.InsertAsync(new Pc { Name = "middle" });
            await _pcRepository.InsertAsync(new Pc { Name = "right" });
            await _testCategoryRepository.InsertAsync(new TestCategory { Name = TestCategoriesKor.공정용.ToString() });
            await _testCategoryRepository.InsertAsync(new TestCategory { Name = TestCategoriesKor.최종용.ToString() });
            await _testCategoryRepository.InsertAsync(new TestCategory { Name = TestCategoriesKor.출하용.ToString() });
            await _testTypeRepository.InsertAsync(new TestType { Name = "Align" });
            await _testTypeRepository.InsertAsync(new TestType { Name = "Axial" });
            await _testTypeRepository.InsertAsync(new TestType { Name = "Lateral" });
            await _transducerTypeRepository.InsertAsync(new TransducerType { Code = TransducerTypes.SCP01.ToString(), Type = "5Mhz" });
            await _transducerTypeRepository.InsertAsync(new TransducerType { Code = TransducerTypes.SCP02.ToString(), Type = "7.5Mhz" });
        }

        [RelayCommand]
        private async Task Master2()
        {
            int maxCnt = 100000;
            int resultCnt = 5;

            string currentDate = DateTime.Now.ToString("yyMMdd");

            Random random = new Random();


            List<MotorModule> moterModules = new List<MotorModule>();
            List<Transducer> transducers = new List<Transducer>();
            List<TransducerModule> transducerModules = new List<TransducerModule>();
            List<Test> tests = new List<Test>();
            List<Tester> tester = new List<Tester>();
            List<int> testerIds = new List<int>();
            List<int> pcIds = new List<int>();
            List<Probe> probes = new List<Probe>();
            List<ProbeTestResultView> probeTestResultViews = new List<ProbeTestResultView>();
            List<int> tdIds = new List<int>();

            #region MotorModules
            for (int i = 1; i <= maxCnt; ++i)
            {
                string MotorModuleSn = "mtm-sn " + currentDate + " " + i.ToString("D6");
                moterModules.Add(new MotorModule { Sn = MotorModuleSn });
            }
            await _motorModuleRepository.BulkInsertAsync(moterModules);
            #endregion

            await _pcRepository.InsertAsync(new Pc { Name = "left" });
            await _pcRepository.InsertAsync(new Pc { Name = "middle" });
            await _pcRepository.InsertAsync(new Pc { Name = "right" });

            //_probeRepository--

            await _testCategoryRepository.InsertAsync(new TestCategory { Name = TestCategoriesKor.공정용.ToString() });
            await _testCategoryRepository.InsertAsync(new TestCategory { Name = TestCategoriesKor.최종용.ToString() });
            await _testCategoryRepository.InsertAsync(new TestCategory { Name = TestCategoriesKor.출하용.ToString() });

            pcIds = Enumerable.Range(1, 3).ToList();

            for (int i = 1; i <= maxCnt / 100; i++)
            {
                Utilities.Shuffle(pcIds);
                tester.Add(new Tester { Name = "yoon", PcId = pcIds[0] });
                tester.Add(new Tester { Name = "sang", PcId = pcIds[1] });
                tester.Add(new Tester { Name = "bkko", PcId = pcIds[2] });
            }
            await _testerRepository.BulkInsertAsync(tester);

            await _testTypeRepository.InsertAsync(new TestType { Name = "Align" });
            await _testTypeRepository.InsertAsync(new TestType { Name = "Axial" });
            await _testTypeRepository.InsertAsync(new TestType { Name = "Lateral" });

            await _transducerTypeRepository.InsertAsync(new TransducerType { Code = TransducerTypes.SCP01.ToString(), Type = "5Mhz" });
            await _transducerTypeRepository.InsertAsync(new TransducerType { Code = TransducerTypes.SCP02.ToString(), Type = "7.5Mhz" });

            // #region TestsTDs
            for (int i = 1; i <= 100; ++i)
            {
                testerIds.AddRange(Enumerable.Range(1, 3000));
            }
            //Utilities.Shuffle(_testers);

            #region Transducers
            //tdIds.AddRange(Enumerable.Range(1, maxCnt));

            for (int i = 1; i <= maxCnt; ++i)
            {
                int transducerTypeId = random.Next(1, 3);
                string TransducerSn = "td-sn " + currentDate + " " + i.ToString("D6");
                transducers.Add(new Transducer { Sn = TransducerSn, TransducerTypeId = transducerTypeId });
            }
            await _transducerRepository.BulkInsertAsync(transducers);
            #endregion

            for (int i = 1; i <= maxCnt; ++i)
            {
                for (int k = 1; k <= Enum.GetNames(typeof(TestTypes)).Length; ++k) //3
                {
                    while (true)
                    {
                        int randomValue = random.Next(65, 100);
                        Test test = new Test
                        {
                            TestCategoryId = (int)TestCategories.Processing,
                            TestTypeId = k,
                            TesterId = testerIds[(i - 1) * 3 + (k - 1)],
                            OriginalImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                            ChangedImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                            ChangedImgMetadata = Utilities.MKSHA256(),
                            Result = randomValue,
                            Method = random.Next(1, 3),
                        };

                        test.TransducerId = i;

                        tests.Add(test);

                        if (randomValue >= 70)
                        {
                            break;
                        }
                    }
                }
            }
            await _testRepository.BulkInsertAsync(tests);

            tests.Clear();

            #region TransducerModules
            for (int i = 1; i <= maxCnt; ++i)
            {
                string TransducerModuleSn = "tdm-sn " + currentDate + " " + i.ToString("D6");
                transducerModules.Add(new TransducerModule { Sn = TransducerModuleSn, TransducerId = i });
            }
            await _transducerModuleRepository.BulkInsertAsync(transducerModules);
            #endregion

            for (int i = 1; i <= maxCnt; ++i)
            {
                for (int k = 1; k <= Enum.GetNames(typeof(TestTypes)).Length; ++k) //3
                {
                    while (true)
                    {
                        int randomValue = random.Next(65, 100);
                        Test test = new Test
                        {
                            TestCategoryId = (int)TestCategories.Process,
                            TestTypeId = k,
                            TesterId = testerIds[(i - 1) * 3 + (k - 1)],
                            OriginalImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                            ChangedImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                            ChangedImgMetadata = Utilities.MKSHA256(),
                            Result = randomValue,
                            Method = random.Next(1, 3),
                        };

                        test.TransducerModuleId = i;

                        tests.Add(test);

                        //await _testRepository.InsertAsync(test);

                        if (randomValue >= 70)
                        {
                            break;
                        }
                    }
                }
            }
            await _testRepository.BulkInsertAsync(tests);

            tests.Clear();

            #region Probes
            for (int i = 1; i <= maxCnt; ++i)
            {
                string ProbeSn = "SCGP01" + currentDate + " " + i.ToString("D6");
                probes.Add(new Probe { Sn = ProbeSn, TransducerModuleId = i, MotorModuleId = i });
            }
            await _probeRepository.BulkInsertAsync(probes);
            #endregion

            for (int i = 1; i <= maxCnt; ++i)
            {
                for (int k = 1; k <= Enum.GetNames(typeof(TestTypes)).Length; ++k) //3
                {
                    while (true)
                    {
                        int randomValue = random.Next(65, 100);
                        Test test = new Test
                        {
                            TestCategoryId = (int)TestCategories.Dispatch,
                            TestTypeId = k,
                            TesterId = testerIds[(i - 1) * 3 + (k - 1)],
                            OriginalImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                            ChangedImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                            ChangedImgMetadata = Utilities.MKSHA256(),
                            Result = randomValue,
                            Method = random.Next(1, 3),
                        };

                        test.ProbeId = i;

                        tests.Add(test);

                        //await _testRepository.InsertAsync(test);

                        if (randomValue >= 70)
                        {
                            break;
                        }
                    }
                }
            }
            await _testRepository.BulkInsertAsync(tests);
        }

        [RelayCommand]
        private async Task Master3()
        {
            int maxCnt = 100000;

            string currentDate = DateTime.Now.ToString("yyMMdd");

            Random random = new Random();

            List<MotorModule> moterModules = new List<MotorModule>();
            List<Transducer> transducers = new List<Transducer>();
            List<TransducerModule> transducerModules = new List<TransducerModule>();
            List<Test> tests = new List<Test>();
            List<Tester> tester = new List<Tester>();
            List<int> testerIds = new List<int>();
            List<int> pcIds = new List<int>();
            List<Probe> probes = new List<Probe>();
            List<int> tdIds = new List<int>();

            #region MotorModules
            for (int i = 1; i <= maxCnt; ++i)
            {
                string MotorModuleSn = "mtm" + currentDate + i.ToString("D3");
                moterModules.Add(new MotorModule { Sn = MotorModuleSn });
                //await _motorModuleRepository.InsertAsync(new MotorModule { MotorModuleSn = MotorModuleSn });
            }
            await _motorModuleRepository.BulkInsertAsync(moterModules);
            #endregion

            await _pcRepository.InsertAsync(new Pc { Name = "left" });
            await _pcRepository.InsertAsync(new Pc { Name = "middle" });
            await _pcRepository.InsertAsync(new Pc { Name = "right" });

            //_probeRepository--

            await _testCategoryRepository.InsertAsync(new TestCategory { Name = TestCategoriesKor.공정용.ToString() });
            await _testCategoryRepository.InsertAsync(new TestCategory { Name = TestCategoriesKor.최종용.ToString() });
            await _testCategoryRepository.InsertAsync(new TestCategory { Name = TestCategoriesKor.출하용.ToString() });

            pcIds = Enumerable.Range(1, 3).ToList();

            for (int i = 1; i <= maxCnt / 100; i++)
            {
                Utilities.Shuffle(pcIds);
                tester.Add(new Tester { Name = "yoon", PcId = pcIds[0] });
                tester.Add(new Tester { Name = "sang", PcId = pcIds[1] });
                tester.Add(new Tester { Name = "bkko", PcId = pcIds[2] });
            }
            await _testerRepository.BulkInsertAsync(tester);

            await _testTypeRepository.InsertAsync(new TestType { Name = "Align" });
            await _testTypeRepository.InsertAsync(new TestType { Name = "Axial" });
            await _testTypeRepository.InsertAsync(new TestType { Name = "Lateral" });

            await _transducerTypeRepository.InsertAsync(new TransducerType { Code = TransducerTypes.SCP01.ToString(), Type = "5Mhz" });
            await _transducerTypeRepository.InsertAsync(new TransducerType { Code = TransducerTypes.SCP02.ToString(), Type = "7.5Mhz" });

            #region Transducers
            tdIds.AddRange(Enumerable.Range(1, maxCnt));

            for (int i = 1; i <= maxCnt; ++i)
            {
                int transducerTypeId = random.Next(1, 3);
                string TransducerSn = "td-sn" + currentDate + i.ToString("D3");
                transducers.Add(new Transducer { Sn = TransducerSn, TransducerTypeId = transducerTypeId });
            }
            await _transducerRepository.BulkInsertAsync(transducers);
            #endregion

            #region TransducerModules
            for (int i = 1; i <= maxCnt; ++i)
            {
                string TransducerModuleSn = "tdm-sn" + currentDate + i.ToString("D3");
                transducerModules.Add(new TransducerModule { Sn = TransducerModuleSn, TransducerId = i });
            }
            await _transducerModuleRepository.BulkInsertAsync(transducerModules);
            #endregion

            #region Probes
            for (int i = 1; i <= maxCnt; ++i)
            {
                string ProbeSn = "SCGP01" + currentDate + i.ToString("D3");
                probes.Add(new Probe { Sn = ProbeSn, TransducerModuleId = i, MotorModuleId = i });
            }
            await _probeRepository.BulkInsertAsync(probes);
            #endregion


            // #region TestsTDs
            for (int i = 1; i <= 100; ++i)
            {
                testerIds.AddRange(Enumerable.Range(1, 3000));
            }
            //Utilities.Shuffle(_testers);

            for (int i = 1; i <= maxCnt; ++i)
            {
                for (int k = 1; k <= Enum.GetNames(typeof(TestTypes)).Length; ++k) //3
                {
                    while (true)
                    {
                        int randomValue = random.Next(65, 100);
                        Test test = new Test
                        {
                            TestCategoryId = (int)TestCategories.Processing,
                            TestTypeId = k,
                            TesterId = testerIds[(i - 1) * 3 + (k - 1)],
                            OriginalImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                            ChangedImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                            ChangedImgMetadata = Utilities.MKSHA256(),
                            Result = randomValue,
                            Method = random.Next(1, 3),
                        };

                        test.TransducerId = i;

                        tests.Add(test);

                        //await _testRepository.InsertAsync(test);

                        if (randomValue >= 70)
                        {
                            break;
                        }
                    }
                }
            }
            await _testRepository.BulkInsertAsync(tests);

            tests.Clear();

            for (int i = 1; i <= maxCnt; ++i)
            {
                for (int k = 1; k <= Enum.GetNames(typeof(TestTypes)).Length; ++k) //3
                {
                    while (true)
                    {
                        int randomValue = random.Next(65, 100);
                        Test test = new Test
                        {
                            TestCategoryId = (int)TestCategories.Process,
                            TestTypeId = k,
                            TesterId = testerIds[(i - 1) * 3 + (k - 1)],
                            OriginalImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                            ChangedImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                            ChangedImgMetadata = Utilities.MKSHA256(),
                            Result = randomValue,
                            Method = random.Next(1, 3),
                        };

                        test.TransducerModuleId = i;

                        tests.Add(test);

                        //await _testRepository.InsertAsync(test);

                        if (randomValue >= 70)
                        {
                            break;
                        }
                    }
                }
            }
            await _testRepository.BulkInsertAsync(tests);

            tests.Clear();

            for (int i = 1; i <= maxCnt; ++i)
            {
                for (int k = 1; k <= Enum.GetNames(typeof(TestTypes)).Length; ++k) //3
                {
                    while (true)
                    {
                        int randomValue = random.Next(65, 100);
                        Test test = new Test
                        {
                            TestCategoryId = (int)TestCategories.Dispatch,
                            TestTypeId = k,
                            TesterId = testerIds[(i - 1) * 3 + (k - 1)],
                            OriginalImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                            ChangedImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                            ChangedImgMetadata = Utilities.MKSHA256(),
                            Result = randomValue,
                            Method = random.Next(1, 3),
                        };

                        test.ProbeId = i;

                        tests.Add(test);

                        //await _testRepository.InsertAsync(test);

                        if (randomValue >= 70)
                        {
                            break;
                        }
                    }
                }
            }
            await _testRepository.BulkInsertAsync(tests);
        }

        [ObservableProperty]
        private string _query = default!;

        [RelayCommand]
        private async Task Select1Async()
        {
            List<Probe> res = await _probeRepository.GetBySn(Query).ToListAsync();
            Log.Information("res:" + res);
            //IEnumerable<Models.Test> enumerable = await _testRepository.GetAllAsync();
            //List<ProbeTestResult> enumerable = _probeRepository.GetProbeSNSql();
            //List<ProbeTestResult> enumerable = _probeRepository.GetProbeTestResult();
        }

        [RelayCommand]
        private async Task PTRViewTest()
        {
            PTRView? view = await _probeRepository.GetPTRViewAsync(Query);
            Log.Information("PTRView:" + view?.ToString() ?? "");
        }

        [RelayCommand]
        private async Task TDMdTest()
        {
            TransducerModule tdMd = new TransducerModule { Sn = $"tdm-sn 240624 001", TransducerId = 100002 };
            await _transducerModuleRepository.InsertAsync(tdMd);
        }

        [RelayCommand]
        private async Task ImportExcelFile()
        {
            IEnumerable<Transducer> tds = await _transducerRepository.GetAllAsync();
            IEnumerable<MotorModule> mtMds = await _motorModuleRepository.GetAllAsync();
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                // 이제 filePath를 사용하여 파일을 열 수 있습니다.
                Log.Information($"Import {filePath}");

                ExcelService excel = new ExcelService();

                //IDictionary<string, List<object>> data = excel.ImportExcel(filePath, "TDSn", "TDSnDate", "MtMdSn", "MtMdSnDate");
                var data = excel.ReadColumnsDataByHeaders(filePath, new List<string> { "TDSn", "TDSnDate", "MTLot", "MTLotDate" });

                // 결과 출력
                foreach (var kvp in data)
                {
                    Log.Information($"헤더: {kvp.Key}");
                    Log.Information("값:");
                    foreach (var value in kvp.Value)
                    {
                        Log.Information(value.ToString()??"");
                    }
                    Log.Information("\n");
                }

                Log.Information($"Import {data}");

                if (data.Count > 0)
                {
                    //List<string?> tdSns = data.ContainsKey("TDSn") ? data["TDSn"].ConvertAll(obj => obj.ToString()) : new List<string?>();
                    //List<DateTime> tdSnDates = data.ContainsKey("TDSnDate") ? data["TDSnDate"].ConvertAll(obj => DateTime.Parse(obj.ToString())) : new List<DateTime>();
                    List<SnDate> tdSns = data.ContainsKey("TDSn") ? data["TDSn"] : new List<SnDate>();

                    Utilities.RemoveDuplicateSnDates(ref tdSns);

                    tdSns.RemoveAll(tdSn => tds.Any(td => td.Sn == tdSn.Sn));

                    List<Transducer> transducers = new List<Transducer>();

                    for (int i = 0; i < tdSns.Count; i++)
                    {
                        Transducer transducer = new()
                        {
                            Sn = tdSns[i].Sn,
                            TransducerTypeId = 1,
                            CreatedDate = tdSns[i].Date,
                        };
                        transducers.Add(transducer);
                    }

                    if(transducers.Count > 0)
                    {
                        await _transducerRepository.BulkInsertAsync(transducers);
                    }

                    List<SnDate> mtMdSns = data.ContainsKey("MTLot") ? data["MTLot"] : new List<SnDate>();

                    Utilities.RemoveDuplicateSnDates(ref mtMdSns);
                    mtMdSns.RemoveAll(mtMd => mtMds.Any(mt => mt.Sn == mtMd.Sn));

                    List<MotorModule> motorModules = new List<MotorModule>();

                    for (int i = 0; i < mtMdSns.Count; i++)
                    {
                        MotorModule motor = new()
                        {
                            Sn = mtMdSns[i].Sn,
                            CreatedDate = mtMdSns[i].Date,
                        };
                        motorModules.Add(motor);
                    }

                    if (motorModules.Count > 0)
                    {
                        await _motorModuleRepository.BulkInsertAsync(motorModules);
                    }
                    
                }
            }
        }

        [RelayCommand]
        private void SeqNoTest()
        {
            _sharedSeqNoRepository.UpsertSeqNoAsync(SnType.Probe);
        }

        [RelayCommand]
        private async Task SetPTRViewAsync()
        {
            int cnt = await _probeRepository.SetPTRViewsAsync();
            Log.Information($"cnt: {cnt}");
        }

        private string _searchText;
        private ObservableCollection<string> _filteredItems;
        private bool _isPopupOpen;
        private int _selectedIndex;

        public ObservableCollection<string> Items { get; }

        public ObservableCollection<string> FilteredItems
        {
            get => _filteredItems;
            private set => SetProperty(ref _filteredItems, value);
        }
        
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterItems();
                    IsPopupOpen = !string.IsNullOrEmpty(_searchText) && FilteredItems.Any();
                }
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetProperty(ref _isPopupOpen, value);
        }

        public ICommand KeyDownCommand { get; }

        [Logging]
        public void FilterItems()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredItems = new ObservableCollection<string>(Items);
            }
            else
            {
                FilteredItems = new ObservableCollection<string>(
                    Items.Where(item => item.ToLower().Contains(SearchText.ToLower())));
            }
        }

        private void OnKeyDown(KeyEventArgs e)
        {
            if (e == null) return;

            Log.Information($"OnKeyDown : {e.Key}");
            if (e.Key == Key.Down)
            {
                if (FilteredItems.Count > 0)
                {
                    SelectedIndex = (SelectedIndex + 1) % FilteredItems.Count;
                }
            }
            else if (e.Key == Key.Up)
            {
                if (FilteredItems.Count > 0)
                {
                    SelectedIndex = (SelectedIndex - 1 + FilteredItems.Count) % FilteredItems.Count;
                }
            }
            else if (e.Key == Key.Enter)
            {
                if (SelectedIndex >= 0 && SelectedIndex < FilteredItems.Count)
                {
                    SearchText = FilteredItems[SelectedIndex];
                    IsPopupOpen = false;
                }
            }
        }

        [RelayCommand]
        private async Task GetSnsAsync()
        {
            List<Transducer> aa = await _transducerRepository.GetFilterItems(Query).ToListAsync();
            Log.Information($"Count: {aa.Count}, \nItems: {string.Join("\n", aa)}");
        }
    }
}