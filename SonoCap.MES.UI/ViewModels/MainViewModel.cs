using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonoCap.MES.UI.Commons;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Base;
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

            await _testCategoryRepository.InsertAsync(new TestCategory { Name = Enums.TestCategoryKor.공정용.ToString() });
            await _testCategoryRepository.InsertAsync(new TestCategory { Name = Enums.TestCategoryKor.최종용.ToString() });

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

            //_testRepository --

            //_transducerModuleRepository --

            await _transducerTypeRepository.InsertAsync(new TransducerType { Code = Enums.TransducerType.SCP01.ToString(), Type = "5Mhz" });
            await _transducerTypeRepository.InsertAsync(new TransducerType { Code = Enums.TransducerType.SCP02.ToString(), Type = "7.5Mhz" });

            #region TransducerModules
            for (int i = 1; i <= maxCnt; ++i)
            {
                int transducerTypeId = random.Next(1, 3);
                string TransducerSn = "td-sn " + currentDate + " " + i.ToString("D6");
                string TransducerModuleSn = "tdm-sn " + currentDate + " " + i.ToString("D6");
                transducerModules.Add(new TransducerModule { TransducerModuleSn = TransducerModuleSn, TransducerSn = TransducerSn, TransducerTypeId = transducerTypeId });
                //await _transducerModuleRepository.InsertAsync(new TransducerModule { TransducerModuleSn = TransducerModuleSn, TransducerSn = TransducerSn,TransducerTypeId = transducerTypeId });
            }
            await _transducerModuleRepository.BulkInsertAsync(transducerModules);
            #endregion

            #region Tests
            List<int> _tests = new List<int>();
            for (int i = 0; i < maxCnt / 100 * 2 * 3 / 3; i++)
            {
                _tests.AddRange(Enumerable.Range(1, 3000));
            }
            Utilities.Shuffle(_tests);

            for (int i = 1; i <= maxCnt; ++i)
            {
                for (int j = 1; j <= Enum.GetNames(typeof(Enums.TestCategory)).Length; ++j) //2
                {
                    for (int k = 1; k <= Enum.GetNames(typeof(Enums.TestType)).Length; ++k) //3
                    {
                        int randomValue = 0;
                        int result = 0;
                        for (; ; )
                        {
                            randomValue = random.Next(65, 100);
                            result = randomValue < 70 ? 0 : randomValue;
                            Test test = new Test
                            {
                                CategoryId = j,
                                TestTypeId = k,
                                TesterId = _tests[i - 1],
                                OriginalImg = $"/img/{currentDate}/{Commons.Utilities.MKRandom(10)}",
                                ChangedImg = $"/img/{currentDate}/{Commons.Utilities.MKRandom(10)}",
                                ChangedImgMetadata = Commons.Utilities.MKSHA256(),
                                Result = result,
                                Method = random.Next(1, 3),
                            };
                            test.TransducerModuleId = i;
                            tests.Add(test);
                            //await _testRepository.InsertAsync(test);

                            if (result >= 70)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            await _testRepository.BulkInsertAsync(tests);
            #endregion

            #region Probes
            for (int i = 1; i <= maxCnt; ++i)
            {
                string ProbeSn = "SCGP01" + currentDate + " " + i.ToString("D6");
                probes.Add(new Probe { ProbeSn = ProbeSn, TransducerModuleId = i, MotorModuleId = i });
                //await _probeRepository.InsertAsync(new Probe { ProbeSn = ProbeSn, TransducerModuleId = i, MotorModuleId = i});
            }
            await _probeRepository.BulkInsertAsync(probes);
            #endregion
        }

        [RelayCommand]
        private async Task Select1Async()
        {

            //IEnumerable<Models.Test> enumerable = await _testRepository.GetAllAsync();
            //List<ProbeTestResult> enumerable = _probeRepository.GetProbeSNSql();
            List<ProbeTestResult> enumerable = _probeRepository.GetProbeTestResult();
        }
    }
}