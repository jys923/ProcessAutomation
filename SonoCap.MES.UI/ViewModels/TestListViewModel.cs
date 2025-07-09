using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Converters;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Converts;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.Services.Interfaces;
using SonoCap.MES.UI.Messages;
using SonoCap.MES.UI.ViewModels.Base;
using SonoCap.WpfCommons;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class TestListViewModel : ViewModelBase
    {
        private readonly IExcelService _excelService;
        private readonly ITestRepository _testRepository;
        private readonly ITestCategoryRepository _testCategoryRepository;
        private readonly IPcRepository _pcRepository;
        private readonly ITestTypeRepository _testTypeRepository;
        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        private string _title = default!;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now.AddMonths(-1);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now;

        [RelayCommand]
        private void Day()
        {
            //StartDate = DateTime.Now.AddDays(-1);
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            EndDate = DateTime.Now;
        }

        [RelayCommand]
        private void Week()
        {

            StartDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).Date;
            EndDate = DateTime.Now;
        }

        [RelayCommand]
        private void Month()
        {

            //StartDate = DateTime.Now.AddMonths(-1);
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = DateTime.Now;
        }

        [RelayCommand]
        private void Year()
        {

            //StartDate = DateTime.Now.AddYears(-1);
            StartDate = new DateTime(DateTime.Now.Year, 1, 1);
            EndDate = DateTime.Now;
        }

        [RelayCommand]
        private void All()
        {

            StartDate = DateTime.MinValue;
            EndDate = DateTime.Now;
        }

        [ObservableProperty]
        private ObservableCollection<string> _testCategories = new();

        [ObservableProperty]
        private string _testCategory = default!;

        [ObservableProperty]
        private ObservableCollection<string> _testTypes = new();

        [ObservableProperty]
        private string _testType = default!;

        [ObservableProperty]
        private ObservableCollection<string> _testResults = new();

        [ObservableProperty]
        private string _testResult = default!;

        [ObservableProperty]
        private ObservableCollection<string> _pcs = new();

        [ObservableProperty]
        private string _pc = default!;

        [ObservableProperty]
        private string _tester = default!;

        [ObservableProperty]
        private string _probeSn = default!;

        [ObservableProperty]
        private string _tDMdSn = default!;

        [ObservableProperty]
        private string _tDSn = default!;

        [ObservableProperty]
        private string _mTMdSn = default!;

        [ObservableProperty]
        private ObservableCollection<TestProbe> _testProbes = default!;

        [ObservableProperty]
        private int _resultCnt = 0000001;

        private IEnumerable<Test> tests = default!;

        //[RelayCommand]
        //private async Task SearchAsync()
        //{
        //    List<TestProbe> testProbes = await _testRepository.GetTestProbeLinqAsync(
        //        StartDate,
        //        EndDate,
        //        TestCategory.Equals("ALL") ? (int)Models.Enums.Commons.All : TestCategories.IndexOf(TestCategory),
        //        TestType.Equals("ALL") ? (int)Models.Enums.Commons.All : TestTypes.IndexOf(TestType),
        //        Tester,
        //        Pc.Equals("ALL") ? CommonValues.All : Pcs.IndexOf(Pc),
        //        TestResult.Equals("ALL") ? (int)Models.Enums.Commons.All : TestResults.IndexOf(TestResult),
        //        null,
        //        ProbeSn,
        //        TDMdSn,
        //        TDSn,
        //        MTMdSn,
        //        null);
        //    TestProbes = new ObservableCollection<TestProbe>(testProbes);
        //    ResultCnt = TestProbes.Count;
        //}

        [RelayCommand]
        private async Task SearchAsync()
        {
            try
            {
                IsBusy = true;
                //await Task.Delay(1000);  // 10초 동안 대기
                tests = await _testRepository.GetTestAsync(
                    StartDate,
                    EndDate,
                    TestCategory.Equals("ALL") ? null : TestCategories.IndexOf(TestCategory),
                    TestType.Equals("ALL") ? null : TestTypes.IndexOf(TestType),
                    Tester,
                    Pc.Equals("ALL") ? null : Pcs.IndexOf(Pc),
                    TestResult.Equals("ALL") ? null : TestResults.IndexOf(TestResult),
                    1,
                    ProbeSn,
                    TDMdSn,
                    TDSn,
                    MTMdSn,
                    null);
                // TestProbes 비동기로 생성
                //var testProbesList = await Task.Run(() => TestToTestProbe.ToList(tests));
                //TestProbes = new ObservableCollection<TestProbe>(testProbesList);

                var testProbesList = await TestToTestProbe.ToListAsync(tests);
                TestProbes = new ObservableCollection<TestProbe>(testProbesList);
                ResultCnt = TestProbes.Count;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Export()
        {
            Log.Information("Export");
            IsBusy = true;
            string exportPath = $"{App.appSettings.Path.ExportExcel}{Utilities.GetCurrentUnixTimestampMilliseconds()}.xlsx";

            try
            {
                if (Utilities.EnsureFolderExists(App.appSettings.Path.ExportExcel))
                {
                    var exportData = tests.Select(TestToExportTest.Convert);//.ToList();
                    _excelService.ExportToExcel(exportData, exportPath);
                    // 성공 메시지 출력
                    Controls.MessageBox.Show("Export Successful", $"File exported to: {exportPath}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Export failed");
                // 실패 메시지 출력
                Controls.MessageBox.Show("Export Failed", "An error occurred during export.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ListDoubleClickAsync(object parameter)
        {
            if (parameter is int selectedIndex)
            {
                if (selectedIndex > -1)
                {
                    // 선택된 행의 인덱스를 활용하여 원하는 동작 수행
                    Log.Information($"{selectedIndex}:{tests.ElementAt(selectedIndex).ToString()}");
                    // selectedItem에 대한 추가 처리 (예: 로그, 다른 속성 업데이트 등)
                    Controls.TestView.Show($"Test Id : {tests.ElementAt(selectedIndex).Id}", tests.ElementAt(selectedIndex));
                    //Controls.InputBox.Show("aaa", "aaa");
                }
                else if (selectedIndex == -1)
                {
                    // NextCommand CanExecute 상태를 갱신합니다.
                    (SearchCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();

                    // Next 메서드를 호출합니다.
                    if (SearchCommand.CanExecute(null))
                    {
                        await SearchCommand.ExecuteAsync(null);
                    }
                }
            }
        }

        public TestListViewModel(
            IPcRepository pcRepository,
            ITestTypeRepository testTypeRepository,
            ITestCategoryRepository testCategoryRepository,
            IExcelService excelService,
            ITestRepository testRepository)
        {
            Title = this.GetType().Name;
            _pcRepository = pcRepository;
            _testTypeRepository = testTypeRepository;
            _testCategoryRepository = testCategoryRepository;
            _excelService = excelService;
            _testRepository = testRepository;

        }

        public async Task InitializeAsync()
        {
            var categories = await _testCategoryRepository.GetAllAsync();
            TestCategories = new ObservableCollection<string>(
                new[] { "ALL" }.Concat(categories.Select(c => c.Name))
            );

            var types = await _testTypeRepository.GetAllAsync();
            TestTypes = new ObservableCollection<string>(
                new[] { "ALL" }.Concat(types.Select(t => t.Name))
            );

            TestResults = new ObservableCollection<string>
            {
                "ALL",//0
                "PASS",
                "FAIL",
            };

            // Pc는 Enum 또는 DB 테이블 존재 여부에 따라 다르게 처리
            var pcs = await _pcRepository.GetAllAsync();
            Pcs = new ObservableCollection<string>(
                new[] { "ALL" }.Concat(pcs.Select(p => p.Name))
            );

            TestCategory = TestCategories[0];
            TestType = TestTypes[0];
            TestResult = TestResults[0];
            Pc = Pcs[0];
        }

        protected override void AddMsg()
        {
            var currentViewModelTypeName = GetType().Name;

            RegisterMessageHandler<ViewModelActionMessage>(msg =>
            {
                if (msg.Value.TargetViewModel == currentViewModelTypeName && msg.Value.Action == "Refresh")
                {
                    Log.Information($"currentViewModelTypeName Refresh");
                    _ = SearchAsync();
                }
            });
        }

        protected override void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            base.OnWindowLoaded(sender, e);
            Log.Information($"{nameof(OnWindowLoaded)}");
            _ = InitializeAsync();
        }

        protected override void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            base.OnWindowClosing(sender, e);
            Log.Information($"{nameof(OnWindowClosing)}");
        }

        protected override void OnWindowActivated(object? sender, EventArgs e)
        {
            base.OnWindowActivated(sender, e);
            Log.Information($"{nameof(OnWindowActivated)}");
        }
    }
}
