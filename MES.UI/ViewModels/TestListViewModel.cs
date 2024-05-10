using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.UI.Models;
using MES.UI.Repositories;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MES.UI.ViewModels
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
        private int _pcNo = default!;

        [ObservableProperty]
        private string _tester = default!;

        [ObservableProperty]
        private string _probeSn = default!;

        [ObservableProperty]
        private string _tDMdSn = default!;

        [ObservableProperty]
        private string _mTMdSn = default!;

        [ObservableProperty]
        private ObservableCollection<TestProbe> _testProbes = default!;

        [RelayCommand]
        private async Task SearchAsync()
        {
            Debug.WriteLine($"{nameof(SearchAsync)}");
            //IEnumerable<Test> tests = await _testRepository.GetAllAsync();
            //Tests = new ObservableCollection<Test>(tests);
            List<TestProbe> testProbes = _testRepository.GetTestProbe();
            TestProbes = new ObservableCollection<TestProbe>(testProbes);
        }

        [RelayCommand]
        private void Export()
        {
            Debug.WriteLine($"{nameof(Export)}");
        }

        public TestListViewModel(ITestRepository testRepository)
        {
            Title = this.GetType().Name;
            this._testRepository = testRepository;

            TestCategories = new ObservableCollection<string>
            {
                "ALL",
                "공정용",
                "출하용",
            };

            TestCategory = TestCategories[0];

            TestTypes = new ObservableCollection<string>
            {
                "ALL",
                "Aline",
                "Axial",
                "Lateral",
            };

            TestType = TestTypes[0];

            TestResults = new ObservableCollection<string>
            {
                "ALL",
                "FAIL",
                "PASS",
            };

            TestResult = TestResults[0];
            //출력

            PcNo = 1;

            Tester = "yoon";

            ProbeSn = "P S/N";

            TDMdSn = "transducer Module";

            MTMdSn = "aaaa";

            //db 조회

            TestProbes = new ObservableCollection<TestProbe>();
            

            //Probes.Add(new Probe { ProbeSn = ProbeSn, });
        }
    }
}
