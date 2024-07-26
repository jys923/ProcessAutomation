using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Converts;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories.Interfaces;
using System.Collections.ObjectModel;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class TestListViewModel : ObservableObject
    {
        private readonly ITestRepository _testRepository;

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
        private ObservableCollection<string> _testCategories;

        [ObservableProperty]
        private string _testCategory = default!;

        [ObservableProperty]
        private ObservableCollection<string> _testTypes;

        [ObservableProperty]
        private string _testType = default!;

        [ObservableProperty]
        private ObservableCollection<string> _testResults;

        [ObservableProperty]
        private string _testResult = default!;

        [ObservableProperty]
        private ObservableCollection<string> _pcs;

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
            IEnumerable<Test> tests = await _testRepository.GetTestAsync(
                StartDate,
                EndDate,
                TestCategory.Equals("ALL") ? (int)Models.Enums.Commons.All : TestCategories.IndexOf(TestCategory),
                TestType.Equals("ALL") ? (int)Models.Enums.Commons.All : TestTypes.IndexOf(TestType),
                Tester,
                Pc.Equals("ALL") ? CommonValues.All : Pcs.IndexOf(Pc),
                TestResult.Equals("ALL") ? (int)Models.Enums.Commons.All : TestResults.IndexOf(TestResult),
                null,
                ProbeSn,
                TDMdSn,
                TDSn,
                MTMdSn,
                null);

            TestProbes = new ObservableCollection<TestProbe>(TestToTestProbe.ToList(tests));
            ResultCnt = TestProbes.Count;
        }

        [RelayCommand]
        private void Export()
        {
            Log.Information("Export");
        }

        public TestListViewModel(ITestRepository testRepository)
        {
            Title = this.GetType().Name;
            this._testRepository = testRepository;

            TestCategories = new ObservableCollection<string>
            {
                "ALL",//0
                "공정용",
                "출하용",
                "최종용",
            };

            TestCategory = TestCategories[0];

            TestTypes = new ObservableCollection<string>
            {
                "ALL",//0
                "Aline",
                "Axial",
                "Lateral",
            };

            TestType = TestTypes[0];

            TestResults = new ObservableCollection<string>
            {
                "ALL",//0
                "PASS",
                "FAIL",
            };

            TestResult = TestResults[0];
            //출력

            Pcs = new ObservableCollection<string>
            {
                "ALL",
                "Left",
                "Middle",
                "Right",
            };

            Pc = Pcs[0];

            //Tester = "yoon";

            //ProbeSn = "P S/N";

            //TDMdSn = "transducer Module";

            //MTMdSn = "motor";

            //db 조회

            //TestProbes = new ObservableCollection<TestProbe>();

            //Probes.Add(new Probe { ProbeSn = ProbeSn, });
        }
    }
}
