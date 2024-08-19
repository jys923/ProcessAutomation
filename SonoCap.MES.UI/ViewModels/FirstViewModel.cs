using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.Services;
using SonoCap.MES.Services.Interfaces;
using SonoCap.MES.UI.Commons;
using SonoCap.MES.UI.Services;
using SonoCap.MES.UI.ViewModels.Base;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class FirstViewModel : ViewModelBase
    {
        private readonly IExcelService _excelService;
        private readonly IViewService _viewService;
        private readonly IMotorModuleRepository _motorModuleRepository;
        private readonly IPcRepository _pcRepository;
        private readonly ITestCategoryRepository _testCategoryRepository;
        private readonly ITestTypeRepository _testTypeRepository;
        private readonly ITransducerRepository _transducerRepository;
        private readonly ITransducerTypeRepository _transducerTypeRepository;
        [ObservableProperty]
        private string _title = default!;

        private BitmapImage _defaultLogo = default!;

        [ObservableProperty]
        private ImageSource _logo = default!;

        [RelayCommand]
        private void GoTestingView()
        {
            Log.Information($"Click {nameof(GoTestingView)}");
            _viewService.ShowTestingView(new SubData { stringData = "test", intData = 123 });
        }

        [RelayCommand]
        private void GoTestListView()
        {
            Log.Debug($"Click {nameof(GoTestListView)}");
            _viewService.ShowTestListView();
        }

        [RelayCommand]
        private void GoProbeListView()
        {
            Log.Information($"Click {nameof(GoProbeListView)}");
            _viewService.ShowProbeListView();
        }

        [RelayCommand]
        private async Task AddMasterDataAsync()
        {
            Log.Information($"Click {nameof(AddMasterDataAsync)}");
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
        private async Task ImportTDExcelAsync()
        {
            IEnumerable<Transducer> tds = await _transducerRepository.GetAllAsync();
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                // 이제 filePath를 사용하여 파일을 열 수 있습니다.
                Log.Information($"Import : {filePath}");

                Dictionary<string, List<SnDate>> data = _excelService.ReadColumnsDataByHeaders(filePath, new List<string> { "TDSn" });

                // 결과 출력
                foreach (var kvp in data)
                {
                    Log.Information($"헤더: {kvp.Key}");
                    var sb = new System.Text.StringBuilder();
                    foreach (var value in kvp.Value)
                    {
                        sb.Append($"{{{value.ToString()}}}" ?? "");
                    }
                    Log.Information($"값:{sb.ToString()}");
                }

                if (data.Count > 0)
                {
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

                    if (transducers.Count > 0)
                    {
                        await _transducerRepository.BulkInsertAsync(transducers);
                    }

                    string snList = string.Join(Environment.NewLine, transducers.Select(t => t.Sn));
                    Controls.MessageBox.Show("Add TD Sn List", snList);
                }
            }
        }

        [RelayCommand]
        private async Task ImportMTExcelAsync()
        {
            IEnumerable<MotorModule> mtMds = await _motorModuleRepository.GetAllAsync();
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                // 이제 filePath를 사용하여 파일을 열 수 있습니다.
                Log.Information($"Import : {filePath}");

                Dictionary<string, List<SnDate>> data = _excelService.ReadColumnsDataByHeaders(filePath, new List<string> { "MTLot" });

                // 결과 출력
                foreach (var kvp in data)
                {
                    Log.Information($"헤더: {kvp.Key}");
                    var sb = new System.Text.StringBuilder();
                    foreach (var value in kvp.Value)
                    {
                        sb.Append($"{{{value.ToString()}}}" ?? "");
                    }
                    Log.Information($"값:{sb.ToString()}");
                }

                if (data.Count > 0)
                {
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

                    string snList = string.Join(Environment.NewLine, motorModules.Select(t => t.Sn));
                    Controls.MessageBox.Show("Add MT Lot List", snList);
                }
            }
        }

        public FirstViewModel(
            IExcelService excelService,
            IViewService viewService,
            IMotorModuleRepository motorModuleRepository,
            IPcRepository pcRepository,
            ITestCategoryRepository testCategoryRepository,
            ITestTypeRepository testTypeRepository,
            ITransducerRepository transducerRepository,
            ITransducerTypeRepository transducerTypeRepository)
        {
            _excelService = excelService;
            _viewService = viewService;
            _motorModuleRepository = motorModuleRepository;
            _pcRepository = pcRepository;
            _testCategoryRepository = testCategoryRepository;
            _testTypeRepository = testTypeRepository;
            _transducerRepository = transducerRepository;
            _transducerTypeRepository = transducerTypeRepository;
            
            Title = this.GetType().Name;

            _defaultLogo = Utilities.LoadBitmapFromResource("logo.png");

            string imagePath = "Resources/logo.png";
            Logo = Utilities.GetFileToImageSource(imagePath) ?? _defaultLogo;
        }
    }

}
