﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonoCap.MES.UI.Commons;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private ProbeListView? _probeListView;
        private TestListView? _testListView;
        private TestView? _testView;
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

        [ObservableProperty]
        private string _title = default!;

        public MainViewModel(
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
                if(_probeListView.WindowState == System.Windows.WindowState.Minimized)
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
            int maxCnt = 100000;
            int resultCnt = 5;

            string currentDate = DateTime.Now.ToString("yyMMdd");

            Random random = new Random();


            List<MotorModule> moterModules = new List<MotorModule>();
            List<Transducer> transducers = new List<Transducer>();
            List<TransducerModule> transducerModules = new List<TransducerModule>();
            List<Test> tests = new List<Test>();
            List<Tester> tester = new List<Tester>();
            List<Probe> probes = new List<Probe>();
            #region MotorModules
            for (int i = 1; i <= maxCnt; ++i)
            {
                string MotorModuleSn = "mtm-sn " + currentDate + " " + i.ToString("D6");
                moterModules.Add(new MotorModule { MotorModuleSn = MotorModuleSn });
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

            List<int> _pcs = Enumerable.Range(1, 3).ToList();
            
            for (int i = 1; i <= maxCnt / 100; i++)
            {
                Utilities.Shuffle(_pcs);
                tester.Add(new Tester { Name = "yoon", PcId = _pcs[0] });
                tester.Add(new Tester { Name = "sang", PcId = _pcs[1] });
                tester.Add(new Tester { Name = "bkko", PcId = _pcs[2] });
            }
            await _testerRepository.BulkInsertAsync(tester);

            await _testTypeRepository.InsertAsync(new TestType { Name = "Align" });
            await _testTypeRepository.InsertAsync(new TestType { Name = "Axial" });
            await _testTypeRepository.InsertAsync(new TestType { Name = "Lateral" });

            await _transducerTypeRepository.InsertAsync(new TransducerType { Code = TransducerTypes.SCP01.ToString(), Type = "5Mhz" });
            await _transducerTypeRepository.InsertAsync(new TransducerType { Code = TransducerTypes.SCP02.ToString(), Type = "7.5Mhz" });

            #region Transducers

            List<int> _TDs = new List<int>();
            _TDs.AddRange(Enumerable.Range(1, maxCnt));

            for (int i = 1; i <= maxCnt; ++i)
            {
                int transducerTypeId = random.Next(1, 3);
                string TransducerSn = "td-sn " + currentDate + " " + i.ToString("D6");
                transducers.Add(new Transducer { TransducerSn = TransducerSn, TransducerTypeId = transducerTypeId });
            }
            await _transducerRepository.BulkInsertAsync(transducers);
            #endregion

            #region TransducerModules

            for (int i = 1; i <= maxCnt; ++i)
            {
                string TransducerModuleSn = "tdm-sn " + currentDate + " " + i.ToString("D6");
                transducerModules.Add(new TransducerModule { TransducerModuleSn = TransducerModuleSn,TransducerId = i});
            }
            await _transducerModuleRepository.BulkInsertAsync(transducerModules);
            #endregion

            #region Probes
            for (int i = 1; i <= maxCnt; ++i)
            {
                string ProbeSn = "SCGP01" + currentDate + " " + i.ToString("D6");
                probes.Add(new Probe { ProbeSn = ProbeSn, TransducerModuleId = i, MotorModuleId = i });
            }
            await _probeRepository.BulkInsertAsync(probes);
            #endregion

            #region Tests
            List<int> _testers = new List<int>();
            for (int i = 0; i < maxCnt / 100 * 2 * 3 / 3; i++)
            {
                _testers.AddRange(Enumerable.Range(1, 3000));
            }
            Utilities.Shuffle(_testers);

            int transducerIndex = 1;
            int transducerModuleIndex = 1;
            int probeIndex = 1;

            for (int i = 1; i <= maxCnt; ++i)
            {
                for (int j = 1; j <= Enum.GetNames(typeof(TestCategories)).Length; ++j) //2
                {
                    for (int k = 1; k <= Enum.GetNames(typeof(TestTypes)).Length; ++k) //3
                    {
                        while (true)
                        {
                            int randomValue = random.Next(65, 100);
                            Test test = new Test
                            {
                                TestCategoryId = j,
                                TestTypeId = k,
                                TesterId = _testers[i - 1],
                                OriginalImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                                ChangedImg = $"/img/{currentDate}/{Utilities.MKRandom(10)}",
                                ChangedImgMetadata = Utilities.MKSHA256(),
                                Result = randomValue,
                                Method = random.Next(1, 3),
                            };

                            switch ((i - 1) % 3)
                            {
                                case 0:
                                    test.TransducerId = transducerIndex++;
                                    break;
                                case 1:
                                    test.TransducerModuleId = transducerModuleIndex++;
                                    break;
                                case 2:
                                    test.ProbeId = probeIndex++;
                                    break;
                            }

                            tests.Add(test);

                            //await _testRepository.InsertAsync(test);

                            if (randomValue >= 70)
                            {
                                break;
                            }

                            // Indices adjustment to revert the increment
                            switch ((i - 1) % 3)
                            {
                                case 0:
                                    transducerIndex--;
                                    break;
                                case 1:
                                    transducerModuleIndex--;
                                    break;
                                case 2:
                                    probeIndex--;
                                    break;
                            }
                        }
                    }
                }
            }
            await _testRepository.BulkInsertAsync(tests);
            #endregion
        }

        [RelayCommand]
        private async Task Select1Async()
        {

            //IEnumerable<Models.Test> enumerable = await _testRepository.GetAllAsync();
            //List<ProbeTestResult> enumerable = _probeRepository.GetProbeSNSql();
            //List<ProbeTestResult> enumerable = _probeRepository.GetProbeTestResult();
        }
    }
}