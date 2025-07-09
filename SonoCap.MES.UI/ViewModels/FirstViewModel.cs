using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.WpfCommons;
using SonoCap.MES.UI.Services.Interfaces;
using SonoCap.MES.UI.ViewModels.Base;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SonoCap.MES.Services.Interfaces;
using SonoCap.MES.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Messaging;
using SonoCap.MES.UI.Messages;

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
        private readonly MESDbContext _context;
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
        private void ShowInputBoxProbeView()
        {
            Log.Information($"Click {nameof(ShowInputBoxProbeView)}");
            var aa = _viewService.ShowInputBoxProbeView("Probe List", "Input Probe");
            if (aa != null)
            {
                Log.Information($"Probe Sn : {aa}");
            }
        }

        [RelayCommand]
        private async Task AddMasterDataAsync()
        {
            Log.Information($"Click {nameof(AddMasterDataAsync)}");
            
            await _context.SeedAsync();
        }

        [RelayCommand]
        private async Task ImportTDExcelAsync()
        {
            var transducerTypes = await _transducerTypeRepository.GetAllAsync();
            int g1Id = transducerTypes.FirstOrDefault(t => t.Code == "G1")?.Id ?? 1;
            int g2Id = transducerTypes.FirstOrDefault(t => t.Code == "G2")?.Id ?? 1;

            IEnumerable<Transducer> tds = await _transducerRepository.GetAllAsync();
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                Log.Information($"Import : {filePath}");

                try
                {
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

                        // 중복된 항목 제거
                        tdSns.RemoveAll(tdSn => tds.Any(td => td.Sn == tdSn.Sn));

                        List<Transducer> transducers = new List<Transducer>();

                        foreach (var td in tdSns)
                        {
                            Transducer transducer = new()
                            {
                                Sn = td.Sn,
                                TransducerTypeId = td.Type switch
                                {
                                    "G1" => g1Id,
                                    "G2" => g2Id,
                                    _ => 1 // ✅ 기본값 (예: "G1", "G2"가 아닌 경우 1)
                                },
                                CreatedDate = td.Date,
                            };

                            transducers.Add(transducer);
                        }

                        if (transducers.Count > 0)
                        {
                            await _transducerRepository.BulkInsertAsync(transducers);
                        }

                        string snList = string.Empty;

                        if (transducers.Count > 0)
                        {
                            snList = $"Total : {transducers.Count}\n" + string.Join(Environment.NewLine, transducers.Select(t => t.Sn));
                        }

                        Controls.MessageBox.Show("Add TD Sn List", string.IsNullOrEmpty(snList) ? "Nothing new has been added." : snList);
                    }
                }
                catch (ArgumentException ex)
                {
                    // 파일이 유효하지 않거나 헤더를 찾을 수 없을 때 예외 처리
                    Log.Error($"예외 발생: {ex.Message}");
                    Controls.MessageBox.Show("Error", $"{ex.Message}");
                }
                catch (Exception ex)
                {
                    // 일반적인 예외 처리
                    Log.Error($"예외 발생: {ex.Message}");
                    Controls.MessageBox.Show("Error", $"{ex.Message}");
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

                try
                {
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

                        string snList = string.Empty;

                        if (motorModules.Count > 0)
                        {
                            await _motorModuleRepository.BulkInsertAsync(motorModules);
                            snList = $"Total : {motorModules.Count}\n" + string.Join(Environment.NewLine, motorModules.Select(t => t.Sn));
                        }
                        Controls.MessageBox.Show("Add MT Lot List", string.IsNullOrEmpty(snList) ? "Nothing new has been added." : snList);
                    }
                }
                catch (ArgumentException ex)
                {
                    // 파일이 유효하지 않거나 헤더를 찾을 수 없을 때 예외 처리
                    Log.Error($"예외 발생: {ex.Message}");
                    Controls.MessageBox.Show("Error", $"{ex.Message}");
                }
                catch (Exception ex)
                {
                    // 일반적인 예외 처리
                    Log.Error($"예외 발생: {ex.Message}");
                    Controls.MessageBox.Show("Error", $"{ex.Message}");
                }
            }
        }

        [RelayCommand]
        private void GoAboutView()
        {
            Log.Information($"Click {nameof(GoAboutView)}");
            _viewService.ShowAboutView();
        }

        [RelayCommand]
        private void SendMsg()
        {
            Log.Information($"Click {nameof(SendMsg)}");
            WeakReferenceMessenger.Default.Send(
                new ViewModelActionMessage(nameof(ProbeListViewModel), "Refresh")
            );

            WeakReferenceMessenger.Default.Send(
                new ViewModelActionMessage(nameof(TestListViewModel), "Refresh")
            );

            WeakReferenceMessenger.Default.Send(
                new ViewModelActionMessage(nameof(TestingViewModel), "Refresh")
            );
        }

        public FirstViewModel(
            MESDbContext context,
            IExcelService excelService,
            IViewService viewService,
            IMotorModuleRepository motorModuleRepository,
            IPcRepository pcRepository,
            ITestCategoryRepository testCategoryRepository,
            ITestTypeRepository testTypeRepository,
            ITransducerRepository transducerRepository,
            ITransducerTypeRepository transducerTypeRepository)
        {
            _context = context;
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

        //private async Task ImportExcelDataAsync<T>(
        //    Func<Task<IEnumerable<T>>> getAllEntitiesAsync,
        //    Func<IEnumerable<SnDate>, IEnumerable<T>, IEnumerable<T>> filterEntities,
        //    Func<IEnumerable<T>, Task> bulkInsertAsync,
        //    string headerName,
        //    string messageBoxTitle,
        //    Func<IEnumerable<T>, string> generateSnList)
        //{
        //    IEnumerable<T> allEntities = await getAllEntitiesAsync();
        //    Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
        //    {
        //        Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls"
        //    };

        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        string filePath = openFileDialog.FileName;
        //        Log.Information($"Import : {filePath}");

        //        try
        //        {
        //            Dictionary<string, List<SnDate>> data = _excelService.ReadColumnsDataByHeaders(filePath, new List<string> { headerName });

        //            // 결과 출력
        //            foreach (var kvp in data)
        //            {
        //                Log.Information($"헤더: {kvp.Key}");
        //                var sb = new System.Text.StringBuilder();
        //                foreach (var value in kvp.Value)
        //                {
        //                    sb.Append($"{{{value.ToString()}}}" ?? "");
        //                }
        //                Log.Information($"값:{sb.ToString()}");
        //            }

        //            if (data.Count > 0)
        //            {
        //                List<SnDate> snDates = data.ContainsKey(headerName) ? data[headerName] : new List<SnDate>();

        //                Utilities.RemoveDuplicateSnDates(ref snDates);

        //                // 필터링 및 중복 제거
        //                IEnumerable<T> filteredEntities = filterEntities(snDates, allEntities);

        //                if (filteredEntities.Any())
        //                {
        //                    await bulkInsertAsync(filteredEntities);

        //                    string snList = generateSnList(filteredEntities);
        //                    Controls.MessageBox.Show(messageBoxTitle, string.IsNullOrEmpty(snList) ? "Nothing new has been added." : snList);
        //                }
        //                else
        //                {
        //                    Controls.MessageBox.Show(messageBoxTitle, "Nothing new has been added.");
        //                }
        //            }
        //        }
        //        catch (ArgumentException ex)
        //        {
        //            // 파일이 유효하지 않거나 헤더를 찾을 수 없을 때 예외 처리
        //            Log.Error($"예외 발생: {ex.Message}");
        //            Controls.MessageBox.Show("Error", $"{ex.Message}");
        //        }
        //        catch (Exception ex)
        //        {
        //            // 일반적인 예외 처리
        //            Log.Error($"예외 발생: {ex.Message}");
        //            Controls.MessageBox.Show("Error", $"{ex.Message}");
        //        }
        //    }
        //}

        //[RelayCommand]
        //private async Task ImportTDExcelAsync()
        //{
        //    await ImportExcelDataAsync<Transducer>(
        //        getAllEntitiesAsync: () => _transducerRepository.GetAllAsync(),
        //        filterEntities: (snDates, allEntities) => snDates
        //            .Where(tdSn => !allEntities.Any(td => td.Sn == tdSn.Sn))
        //            .Select(td => new Transducer
        //            {
        //                Sn = td.Sn,
        //                TransducerTypeId = 1,
        //                CreatedDate = td.Date,
        //            }),
        //        bulkInsertAsync: entities => _transducerRepository.BulkInsertAsync(entities.Cast<Transducer>()),
        //        headerName: "TDSn",
        //        messageBoxTitle: "Add TD Sn List",
        //        generateSnList: entities => string.Join(Environment.NewLine, entities.Cast<Transducer>().Select(t => t.Sn))
        //    );
        //}

        //[RelayCommand]
        //private async Task ImportMTExcelAsync()
        //{
        //    await ImportExcelDataAsync<MotorModule>(
        //        getAllEntitiesAsync: () => _motorModuleRepository.GetAllAsync(),
        //        filterEntities: (snDates, allEntities) => snDates
        //            .Where(mtMd => !allEntities.Any(mt => mt.Sn == mtMd.Sn))
        //            .Select(mt => new MotorModule
        //            {
        //                Sn = mt.Sn,
        //                CreatedDate = mt.Date,
        //            }),
        //        bulkInsertAsync: entities => _motorModuleRepository.BulkInsertAsync(entities.Cast<MotorModule>()),
        //        headerName: "MTLot",
        //        messageBoxTitle: "Add MT Lot List",
        //        generateSnList: entities => $"Total : {entities.Count()}\n" + string.Join(Environment.NewLine, entities.Cast<MotorModule>().Select(t => t.Sn))
        //    );
        //}
    }
}
