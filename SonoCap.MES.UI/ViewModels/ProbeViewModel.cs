using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.WpfCommons;
using SonoCap.MES.UI.ViewModels.Base;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using SonoCap.MES.UI.Services;
using CommunityToolkit.Mvvm.Messaging;
using SonoCap.MES.UI.Messages;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class ProbeViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title = default!;

        [ObservableProperty]
        private PTRView _pTRView = default!;

        private BitmapImage _defaultImg = default!;

        [ObservableProperty]
        private ImageSource _srcImg00 = default!;

        [ObservableProperty]
        private ImageSource _resImg00 = default!;

        [ObservableProperty]
        private ImageSource _srcImg01 = default!;

        [ObservableProperty]
        private ImageSource _resImg01 = default!;

        [ObservableProperty]
        private ImageSource _srcImg02 = default!;

        [ObservableProperty]
        private ImageSource _resImg02 = default!;

        [ObservableProperty]
        private ImageSource _srcImg10 = default!;

        [ObservableProperty]
        private ImageSource _resImg10 = default!;

        [ObservableProperty]
        private ImageSource _srcImg11 = default!;

        [ObservableProperty]
        private ImageSource _resImg11 = default!;

        [ObservableProperty]
        private ImageSource _srcImg12 = default!;

        [ObservableProperty]
        private ImageSource _resImg12 = default!;

        [ObservableProperty]
        private ImageSource _srcImg20 = default!;

        [ObservableProperty]
        private ImageSource _resImg20 = default!;

        [ObservableProperty]
        private ImageSource _srcImg21 = default!;

        [ObservableProperty]
        private ImageSource _resImg21 = default!;
        [ObservableProperty]
        private ImageSource _srcImg22 = default!;

        [ObservableProperty]
        private ImageSource _resImg22 = default!;
        
        private readonly ProbeService _probeService;

        public Action? CloseAction { get; set; }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            Log.Information($"{nameof(DeleteAsync)}");
            bool proceed = Controls.MessageBox.Show($"{PTRView.ProbeSn} 데이터 삭제", $"{PTRView.ProbeSn} 데이터 삭제 실행?");
            if (!proceed)
            {
                return;
            }

            Log.Information($"Deleting {PTRView.ProbeSn} data...");

            await _probeService.DeleteProbe(PTRView);

            //CloseAction?.Invoke();  // 창 닫기
            Window?.Close(); // 이 한 줄이면 끝

            WeakReferenceMessenger.Default.Send(
                new ViewModelActionMessage(nameof(ProbeListViewModel), "Refresh")
            );

        }

        public void Initialize2(string title, PTRView pTRView)
        {
            Title = title;
            PTRView = pTRView;

            _defaultImg = Utilities.LoadBitmapFromResource("usImg.bmp");

            var basePath = App.appSettings.Path.ExportImg;
            var phaseMap = App.appSettings.Path.ExportImgPhase;

            string exportPath01 = Utilities.GetExportImgPath(basePath, phaseMap, 1);
            string exportPath02 = Utilities.GetExportImgPath(basePath, phaseMap, 2);
            string exportPath03 = Utilities.GetExportImgPath(basePath, phaseMap, 3);

            SrcImg00 = Utilities.GetFileToImageSource(Path.Combine(exportPath01, PTRView.Test01.OriginalImg)) ?? _defaultImg;
            ResImg00 = Utilities.GetFileToImageSource(Path.Combine(exportPath01, PTRView.Test01.ChangedImg)) ?? _defaultImg;

            SrcImg01 = Utilities.GetFileToImageSource(Path.Combine(exportPath01, PTRView.Test02.OriginalImg)) ?? _defaultImg;
            ResImg01 = Utilities.GetFileToImageSource(Path.Combine(exportPath01, PTRView.Test02.ChangedImg)) ?? _defaultImg;

            SrcImg02 = Utilities.GetFileToImageSource(Path.Combine(exportPath01, PTRView.Test03.OriginalImg)) ?? _defaultImg;
            ResImg02 = Utilities.GetFileToImageSource(Path.Combine(exportPath01, PTRView.Test03.ChangedImg)) ?? _defaultImg;

            SrcImg10 = Utilities.GetFileToImageSource(Path.Combine(exportPath02, PTRView.Test04.OriginalImg)) ?? _defaultImg;
            ResImg10 = Utilities.GetFileToImageSource(Path.Combine(exportPath02, PTRView.Test04.ChangedImg)) ?? _defaultImg;

            SrcImg11 = Utilities.GetFileToImageSource(Path.Combine(exportPath02, PTRView.Test05.OriginalImg)) ?? _defaultImg;
            ResImg11 = Utilities.GetFileToImageSource(Path.Combine(exportPath02, PTRView.Test05.ChangedImg)) ?? _defaultImg;

            SrcImg12 = Utilities.GetFileToImageSource(Path.Combine(exportPath02, PTRView.Test06.OriginalImg)) ?? _defaultImg;
            ResImg12 = Utilities.GetFileToImageSource(Path.Combine(exportPath02, PTRView.Test06.ChangedImg)) ?? _defaultImg;

            SrcImg20 = PTRView.Test07 == null ? _defaultImg : Utilities.GetFileToImageSource(Path.Combine(exportPath03, PTRView.Test07.OriginalImg)) ?? _defaultImg;
            ResImg20 = PTRView.Test07 == null ? _defaultImg : Utilities.GetFileToImageSource(Path.Combine(exportPath03, PTRView.Test07.ChangedImg)) ?? _defaultImg;

            SrcImg21 = PTRView.Test08 == null ? _defaultImg : Utilities.GetFileToImageSource(Path.Combine(exportPath03, PTRView.Test08.OriginalImg)) ?? _defaultImg;
            ResImg21 = PTRView.Test08 == null ? _defaultImg : Utilities.GetFileToImageSource(Path.Combine(exportPath03, PTRView.Test08.ChangedImg)) ?? _defaultImg;

            SrcImg22 = PTRView.Test09 == null ? _defaultImg : Utilities.GetFileToImageSource(Path.Combine(exportPath03, PTRView.Test09.OriginalImg)) ?? _defaultImg;
            ResImg22 = PTRView.Test09 == null ? _defaultImg : Utilities.GetFileToImageSource(Path.Combine(exportPath03, PTRView.Test09.ChangedImg)) ?? _defaultImg;
        }

        public void Initialize(string title, PTRView pTRView)
        {
            Title = title;
            PTRView = pTRView;

            _defaultImg = Utilities.LoadBitmapFromResource("usImg.bmp");
            var basePath = App.appSettings.Path.ExportImg;
            var phaseMap = App.appSettings.Path.ExportImgPhase;

            string path1 = Utilities.GetExportImgPath(basePath, phaseMap, 1);
            string path2 = Utilities.GetExportImgPath(basePath, phaseMap, 2);
            string path3 = Utilities.GetExportImgPath(basePath, phaseMap, 3);

            // Load images safely
            SrcImg00 = Utilities.LoadOrDefault(path1, PTRView.Test01?.OriginalImg, _defaultImg);
            ResImg00 = Utilities.LoadOrDefault(path1, PTRView.Test01?.ChangedImg, _defaultImg);

            SrcImg01 = Utilities.LoadOrDefault(path1, PTRView.Test02?.OriginalImg, _defaultImg);
            ResImg01 = Utilities.LoadOrDefault(path1, PTRView.Test02?.ChangedImg, _defaultImg);

            SrcImg02 = Utilities.LoadOrDefault(path1, PTRView.Test03?.OriginalImg, _defaultImg);
            ResImg02 = Utilities.LoadOrDefault(path1, PTRView.Test03?.ChangedImg, _defaultImg);

            SrcImg10 = Utilities.LoadOrDefault(path2, PTRView.Test04?.OriginalImg, _defaultImg);
            ResImg10 = Utilities.LoadOrDefault(path2, PTRView.Test04?.ChangedImg, _defaultImg);

            SrcImg11 = Utilities.LoadOrDefault(path2, PTRView.Test05?.OriginalImg, _defaultImg);
            ResImg11 = Utilities.LoadOrDefault(path2, PTRView.Test05?.ChangedImg, _defaultImg);

            SrcImg12 = Utilities.LoadOrDefault(path2, PTRView.Test06?.OriginalImg, _defaultImg);
            ResImg12 = Utilities.LoadOrDefault(path2, PTRView.Test06?.ChangedImg, _defaultImg);

            SrcImg20 = Utilities.LoadOrDefault(path3, PTRView.Test07?.OriginalImg, _defaultImg);
            ResImg20 = Utilities.LoadOrDefault(path3, PTRView.Test07?.ChangedImg, _defaultImg);

            SrcImg21 = Utilities.LoadOrDefault(path3, PTRView.Test08?.OriginalImg, _defaultImg);
            ResImg21 = Utilities.LoadOrDefault(path3, PTRView.Test08?.ChangedImg, _defaultImg);

            SrcImg22 = Utilities.LoadOrDefault(path3, PTRView.Test09?.OriginalImg, _defaultImg);
            ResImg22 = Utilities.LoadOrDefault(path3, PTRView.Test09?.ChangedImg, _defaultImg);
        }

        public ProbeViewModel(
            ProbeService probeService
            //string title, 
            //PTRView pTRView
            )
        {
            _probeService = probeService;
            //Title = title;
            //_pTRView = pTRView;

            //_defaultImg = Utilities.LoadBitmapFromResource("usImg.bmp");

            //SrcImg00 = Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test01.OriginalImg)) ?? _defaultImg;
            //ResImg00 = Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test01.ChangedImg)) ?? _defaultImg;

            //SrcImg01 = Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test02.OriginalImg)) ?? _defaultImg;
            //ResImg01 = Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test02.ChangedImg)) ?? _defaultImg;

            //SrcImg02 = Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test03.OriginalImg)) ?? _defaultImg;
            //ResImg02 = Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test03.ChangedImg)) ?? _defaultImg;

            //SrcImg10 = Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test04.OriginalImg)) ?? _defaultImg;
            //ResImg10 = Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test04.ChangedImg)) ?? _defaultImg;

            //SrcImg11 = Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test05.OriginalImg)) ?? _defaultImg;
            //ResImg11 = Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test05.ChangedImg)) ?? _defaultImg;

            //SrcImg12 = Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test06.OriginalImg)) ?? _defaultImg;
            //ResImg12 = Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test06.ChangedImg)) ?? _defaultImg;

            //SrcImg20 = PTRView.Test07 == null ? _defaultImg : Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test07.OriginalImg)) ?? _defaultImg;
            //ResImg20 = PTRView.Test07 == null ? _defaultImg : Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test07.ChangedImg)) ?? _defaultImg;

            //SrcImg21 = PTRView.Test08 == null ? _defaultImg : Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test08.OriginalImg)) ?? _defaultImg;
            //ResImg21 = PTRView.Test08 == null ? _defaultImg : Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test08.ChangedImg)) ?? _defaultImg;

            //SrcImg22 = PTRView.Test09 == null ? _defaultImg : Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test09.OriginalImg)) ?? _defaultImg;
            //ResImg22 = PTRView.Test09 == null ? _defaultImg : 
            //    Utilities.GetFileToImageSource(Path.Combine(App.appSettings.Path.ExportImg, PTRView.Test09.ChangedImg)) ?? _defaultImg;
        }
    }
}
