using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Converts;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.Services.Interfaces;
using SonoCap.MES.UI.Commons;
using SonoCap.MES.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class ProbeListViewModel : ViewModelBase
    {
        private readonly IExcelService _excelService;
        private readonly IProbeRepository _probeRepository;
        private readonly IPTRViewRepository _pTRViewRepository;

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

        //[ObservableProperty]
        //private ObservableCollection<string> _testCategories;

        //[ObservableProperty]
        //private string _testCategory = default!;

        //[ObservableProperty]
        //private ObservableCollection<string> _testResults;

        //[ObservableProperty]
        //private string _testResult = default!;

        //[ObservableProperty]
        //private int _pcNo = default!;

        //[ObservableProperty]
        //private string _tester = default!;

        [ObservableProperty]
        private string _probeSn = default!;

        [ObservableProperty]
        private string _tDMdSn = default!;

        [ObservableProperty]
        private string _tDSn = default!;

        [ObservableProperty]
        private string _mTMdSn = default!;

        [ObservableProperty]
        private ObservableCollection<ProbeTestResult> _probes = default!;

        private IEnumerable<PTRView> probes = default!; 

        [ObservableProperty]
        private int _resultCnt;

        [RelayCommand]
        private async Task SearchAsync()
        {
            probes = await _pTRViewRepository.GetProbeTestResultLinqAsync2(
                StartDate,
                EndDate,
                ProbeSn,
                TDMdSn,
                TDSn,
                MTMdSn);
            //Probes = new ObservableCollection<ProbeTestResult>(probes);
            Probes = new ObservableCollection<ProbeTestResult>(PTRViewToProbeTestResult.ToList(probes));
            ResultCnt = Probes.Count;
        }

        [RelayCommand]
        private void Export()
        {
            Log.Information("Export");
            if (Utilities.EnsureFolderExists(App.appSettings.Path.ExportExcel))
            {
                _excelService.ExportToExcel(probes, $"{App.appSettings.Path.ExportExcel}{Utilities.GetCurrentUnixTimestampMilliseconds()}.xlsx");
            }
        }

        [RelayCommand]
        private void ListDoubleClick(object parameter)
        {
            if (parameter is int selectedIndex)
            {
                // 선택된 행의 인덱스를 활용하여 원하는 동작 수행
                Log.Information($"{selectedIndex}:{probes.ElementAt(selectedIndex).ToString()}");
                // selectedItem에 대한 추가 처리 (예: 로그, 다른 속성 업데이트 등)
                Controls.ProbeView.Show("Motor Module", probes.ElementAt(selectedIndex));
                //Controls.InputBox.Show("aaa", "aaa");
            }
        }

        public ProbeListViewModel(
            IExcelService excelService,
            IProbeRepository probeRepository,
            IPTRViewRepository pTRViewRepository)
        {
            Title = this.GetType().Name;
            _excelService = excelService;
            _probeRepository = probeRepository;
            _pTRViewRepository = pTRViewRepository;
            //TestCategories = new ObservableCollection<string>
            //{
            //    "ALL",
            //    "공정용",
            //    "출하용",
            //};

            //TestCategory = TestCategories[0];

            //TestResults = new ObservableCollection<string>
            //{
            //    "ALL",
            //    "FAIL",
            //    "PASS",
            //};

            //TestResult = TestResults[0];
            ////출력

            //PcNo = 1;

            //Tester = "yoon";

            ProbeSn = "";

            TDMdSn = "";

            TDSn = "";

            MTMdSn = "";

            //db 조회

            //Probes.Add(new Probe { ProbeSn = ProbeSn, });
        }
    }
}
