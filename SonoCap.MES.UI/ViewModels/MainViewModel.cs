﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonoCap.MES.UI.Commons;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SonoCap.MES.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SonoCap.Interceptors;
using Microsoft.EntityFrameworkCore;
using SonoCap.MES.UI.Services;
using SonoCap.MES.UI.ViewModels.Base;
using System.Windows;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using SonoCap.MES.Services.Interfaces;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private ProbeListView? _probeListView = default!;
        private TestListView? _testListView = default!;
        private TestingView? _testingView = default!;
        private readonly IExcelService _excelService;
        private readonly IViewService _viewService;
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

        private TcpClient _client = default!;

        public MainViewModel(
            IExcelService excelService,
            IViewService viewService,
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
            _excelService = excelService;
            _viewService = viewService;
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
            KeyDownCommand = new RelayCommand<System.Windows.Input.KeyEventArgs>(OnKeyDown);

            //_client = new TcpClient();

            //// 서버 IP 주소와 포트 번호
            //string serverIP = "127.0.0.1";
            //int port = 9999;

            //// 서버에 연결
            //_client.Connect(IPAddress.Parse(serverIP), port);

            //Task.Run(async () => await ReceiveDataAsync());
            //Init();
        }

        [RelayCommand]
        private void ToBlink()
        {
            //IsBlinking = !IsBlinking;
            //var mainView = App.Current.Services.GetService<TestView>()!;
            //mainView.Show();//Task.Run(() => DoBackgroundWork());
            _viewService.ShowTestListView();
        }

        [RelayCommand]
        private void ToTest()
        {
            _viewService.ShowTestingView(new SubData { stringData = "test" , intData = 123});
            //var mainView = App.Current.Services.GetService<ProbeView>()!;
            //mainView.Show();//Task.Run(() => DoBackgroundWork());
            //_viewService.ShowProbeListView();
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
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                // 이제 filePath를 사용하여 파일을 열 수 있습니다.
                Log.Information($"Import {filePath}");

                //IDictionary<string, List<object>> data = excel.ImportExcel(filePath, "TDSn", "TDSnDate", "MtMdSn", "MtMdSnDate");
                var data = _excelService.ReadColumnsDataByHeaders(filePath, new List<string> { "TDSn", "TDSnDate", "MTLot", "MTLotDate" });

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

        private string _searchText = default!;
        private ObservableCollection<string> _filteredItems = [];
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

        private void OnKeyDown(System.Windows.Input.KeyEventArgs e)
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

        protected override void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //base.OnWindowLoaded(sender, e);
            //MessageBox.Show("MainWindow Loaded");
        }

        protected override void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            //base.OnWindowClosing(sender, e);
            //MessageBox.Show("MainWindow Closing");
        }

        [RelayCommand]
        private void InputBox()
        {
            //Test test = new Test { 

            //}
            //Controls.TestView.Show("아무말 메세지",);

            //var result = Controls.InputBoxMotor.Show("아무말 메세지", "아무말이나 적어보세요 ^^",_motorModuleRepository);
            //if (result == null)
            //{
            //    System.Windows.MessageBox.Show("취소됨");
            //}
            //else
            //{
            //    System.Windows.MessageBox.Show(result.Sn);
            //}

            //MotorModule? result = Controls.InputBoxMotor.Show("아무말 메세지", "아무말이나 적어보세요 ^^");
            //if (result == null)
            //{
            //    MessageBox.Show("취소됨");
            //}
            //else
            //{
            //    MessageBox.Show(result.Sn);
            //}
        }

        [ObservableProperty]
        private string _send = default!;

        [ObservableProperty]
        private ImageSource _srcImg = default!;

        [RelayCommand]
        private async Task SendSocAsync()
        {
            //_client.Send(Send);
            NetworkStream stream =_client.GetStream();
            byte[] msgBuffer = Encoding.UTF8.GetBytes(Send);
            await stream.WriteAsync(msgBuffer);

            //var buffer = new byte[1024];
            //int read = await stream.ReadAsync(buffer, 0, buffer.Length);
            //string msg = Encoding.UTF8.GetString(buffer,0,read);

            //Log.Information($"Received {msg}");

            int dataSize = 1048576;
            // 데이터를 받을 버퍼를 초기화합니다.
            byte[] buffer = new byte[dataSize];
            int totalBytesRead = 0;

            // 데이터를 전부 받을 때까지 반복해서 읽습니다.
            while (totalBytesRead < dataSize)
            {
                int bytesRead = await stream.ReadAsync(buffer, totalBytesRead, dataSize - totalBytesRead);
                if (bytesRead == 0)
                {
                    // 연결이 끊겼거나 EOF
                    throw new IOException("Connection closed prematurely.");
                }
                totalBytesRead += bytesRead;
            }

            Log.Information($"Total bytes read: {totalBytesRead}");
            Bitmap m_bmpRes = new Bitmap(512, 512, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Utilities.ByteArrToBitmap(buffer, m_bmpRes);
            string bmpFileName = String.Format($"{Utilities.GetCurrentUnixTimestampMilliseconds()}.bmp");
            m_bmpRes.Save(bmpFileName, ImageFormat.Bmp);
            ImageSource tmpImg = Utilities.BitmapToImageSource(m_bmpRes);
            SrcImg = (BitmapImage)tmpImg;
            //await App.Current.Dispatcher.InvokeAsync(() =>
            //{
            //    SrcImg = (BitmapImage)tmpImg;
            //});

        }

        private async Task ReceiveDataAsync2()
        {
            NetworkStream stream = _client.GetStream();
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    // 여기서 받은 데이터를 처리하면 됩니다.
                    Log.Information($"Received data: {receivedData}");
                }
                else
                {
                    // 서버가 연결을 끊었을 때 처리할 로직을 추가하세요.
                    break;
                }
            }
        }

        private async Task ReceiveDataAsync3()
        {
            NetworkStream stream = _client.GetStream();

            byte[] buffer = new byte[1048576];
            int totalBytesRead = 0;

            while (totalBytesRead < buffer.Length)
            {
                int bytesRead = await stream.ReadAsync(buffer, totalBytesRead, buffer.Length - totalBytesRead);
                if (bytesRead == 0)
                {
                    // 연결이 끊겼거나 EOF
                    break;
                }
                totalBytesRead += bytesRead;
            }
            // totalBytesRead는 실제로 읽힌 바이트 수를 나타냅니다.
            Log.Information($"Total bytes read: {totalBytesRead}");

            Bitmap m_bmpRes = new Bitmap(512, 512, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Utilities.ByteArrToBitmap(buffer, m_bmpRes);
            m_bmpRes.Save("test.bmp", ImageFormat.Bmp);
        }

        public async Task ReceiveDataAsync()
        {
            try
            {
                NetworkStream stream = _client.GetStream();

                while (_client.Connected)
                {
                    // 헤더를 읽어 데이터 크기를 가져옵니다.
                    //byte[] headerBuffer = new byte[4];
                    //await stream.ReadAsync(headerBuffer, 0, headerBuffer.Length);
                    //int dataSize = BitConverter.ToInt32(headerBuffer, 0);
                    int dataSize = 1048576;
                    // 데이터를 받을 버퍼를 초기화합니다.
                    byte[] buffer = new byte[dataSize];
                    int totalBytesRead = 0;

                    // 데이터를 전부 받을 때까지 반복해서 읽습니다.
                    while (totalBytesRead < dataSize)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, totalBytesRead, dataSize - totalBytesRead);
                        if (bytesRead == 0)
                        {
                            // 연결이 끊겼거나 EOF
                            throw new IOException("Connection closed prematurely.");
                        }
                        totalBytesRead += bytesRead;
                    }

                    Log.Information($"Total bytes read: {totalBytesRead}");
                    Bitmap m_bmpRes = new Bitmap(512, 512, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    Utilities.ByteArrToBitmap(buffer, m_bmpRes);
                    string bmpFileName = String.Format($"{Utilities.GetCurrentUnixTimestampMilliseconds()}.bmp");
                    m_bmpRes.Save(bmpFileName, ImageFormat.Bmp);
                    // 데이터 수신 완료 후 이벤트를 통해 데이터 전달
                    //OnDataReceived(buffer);
                }
            }
            catch (Exception ex)
            {
                // 예외 처리 필요
                Log.Information($"수신 오류: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Receive()
        {
            Log.Information("Receive");
            // 예시: 메서드 호출
            IntPtr outputFinalImageBuf = new IntPtr();// = /* final image buffer */;
            int finalImageBufLength = 512 * 512 * 4;// = /* length of final image buffer */;
            IntPtr outputRawDataBuf = new IntPtr(); // = /* raw data buffer */;

            int byte_per_sample = 2;
            int max_no_sample = 512;
            int max_scanline = 960;
            int rawDataBufLength = max_scanline * max_no_sample * byte_per_sample;
            string outputMetadata;

            ulong result = HsnLibraryService.ipRenderWithCapture(outputFinalImageBuf, finalImageBufLength, outputRawDataBuf, rawDataBufLength, out outputMetadata);

            //if (result != IntPtr.Zero)
            if (result > 0)
            {
                // 성공적으로 호출되었을 때의 처리
                // outputMetadata를 사용하세요
                Log.Information("Pass");
            }
            else
            {
                // 호출 실패 시의 처리
                Log.Information("Fail");
            }
        }

        private static void Init()
        {
            registerCallbackBeforeInitialize();

            if (!SonoCapUsImgService.Initialize())
            {
                Log.Information("initialize Fail");
                return;
            }

            registerCallbackAfterInitialize();

            try
            {
                SonoCapUsImgService.StartProbeDetection();
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}");
                throw;
            }

            //if (!SonoCapUsImgService.IpInitialize())
            //{
            //    Log.Information("ipInitialize Fail");
            //    return;
            //}

            //int width = 512;
            //int height = 512;
            //if (!SonoCapUsImgService.IpResize(width, height))
            //{
            //    //exception
            //    Log.Information("ipResize Fail");
            //    return;
            //}
        }

        private static void registerCallbackBeforeInitialize()
        {
            Loading loading = () =>
            {
                Log.Information("Loading callback executed!");
            };

            Error error = (string message, int value) => 
            {
                Log.Error(message, value);
            };

            bool result = SonoCapUsImgService.RegisterCallbackLoading(loading); //loading_status
            result = SonoCapUsImgService.RegisterCallbackError(error); //error
        }

        private static void mLoadingCallback(bool value)
        {
            Log.Information($"Loading {value}");
        }

        private static void myErrorCallback(string value, int value2)
        {
            Log.Error($"Error {value} {value2}");
        }

        private static void registerCallbackAfterInitialize()
        {
            DeviceAttached deviceAttached = () =>
            {
                Log.Information("DeviceAttached callback executed!");
                SonoCapUsImgService.ActivateProbe();
            };

            DeviceRemoved deviceRemoved = () => 
            { 
                Log.Information("DeviceRemoved callback executed!");
                SonoCapUsImgService.DeactivateProbe();
            };

            MotorSpeed motorSpeed = (int value, int value2) =>
            {
                Log.Information($"{value} {value2}");
            };

            ProbeState probeState = (int value) =>
            {
                Log.Information($"{value}");
            };

            SonoCapUsImgService.RegisterCallbackDeviceAttached(deviceAttached);
            SonoCapUsImgService.RegisterCallbackDeviceRemoved(deviceRemoved);
            SonoCapUsImgService.RegisterCallbackMotorSpeed(motorSpeed);
            SonoCapUsImgService.RegisterCallbackProbeState(probeState);
        }

        private static void DeviceAttachedCallback()
        {
            SonoCapUsImgService.ActivateProbe();
        }

        private static void mDeviceAttachedCallback()
        {
            HsnLibraryService.activateProbe();
        }

        private static void mDeviceRemovedCallback()
        {
            HsnLibraryService.disactivateProbe();
        }
        
        private static void mMotorSpeedCallback(int value, int value2)
        {
            Log.Information($"MotorSpeed prf_hz : {value}, density : {value2}");
        }

        private static void mProbeStateCallback(int value)
        {
            Log.Information($"ProbeState {value}");
        }
        
        private static void init2()
        {
            registerCallbackBeforeInitialize();

            if (!HsnLibraryService.initialize())
            {
                Log.Information("initialize Fail");
                return;
            }

            registerCallbackAfterInitialize();

            try
            {
                HsnLibraryService.startProbeDetection();
            }
            catch (Exception e)
            {
                Log.Information($"{e.Message}");
                throw;
            }
            //HsnLibraryService.startProbeDetection();

            //if (!HsnLibraryService.startProbeDetection())
            //{
            //    Log.Information("startProbeDetection Fail");
            //    return;
            //}


            if (!HsnLibraryService.ipInitialize())
            {
                Log.Information("ipInitialize Fail");
                return;
            }

            int width = 512;
            int height = 512;
            if (!HsnLibraryService.ipResize(width, height))
            {
                //exception
                Log.Information("ipResize Fail");
                return;
            }
        }

        private static void registerCallbackBeforeInitialize2()
        {
            HsnLibraryService.registerCallback_Loading(mLoadingCallback2); //loading_status
            HsnLibraryService.registerCallback_Error(myErrorCallback2); //error
        }

        private static void mLoadingCallback2(bool value)
        {
            Log.Information($"Loading {value}");
        }

        private static void myErrorCallback2(string value, int value2)
        {
            Log.Information($"Error {value} {value2}");
        }

        private static void registerCallbackAfterInitialize2()
        {
            HsnLibraryService.registerCallback_DeviceAttached(mDeviceAttachedCallback2);
            HsnLibraryService.registerCallback_DeviceRemoved(mDeviceRemovedCallback2);
            HsnLibraryService.registerCallback_ENDMotorSpeed(mMotorSpeedCallback2);
            HsnLibraryService.registerCallback_ProbeState(mProbeStateCallback2);//probe state
        }

        private static void mProbeStateCallback2(int value)
        {
            Log.Information($"ProbeState {value}");
        }

        private static void mMotorSpeedCallback2(int value, int value2)
        {
            Log.Information($"MotorSpeed prf_hz : {value}, density : {value2}");
        }

        private static void mDeviceRemovedCallback2()
        {
            HsnLibraryService.disactivateProbe();
        }

        private static void mDeviceAttachedCallback2()
        {
            HsnLibraryService.activateProbe();
        }
    }
}