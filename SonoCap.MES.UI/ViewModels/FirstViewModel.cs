using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.UI.Commons;
using SonoCap.MES.UI.Services;
using SonoCap.MES.UI.ViewModels.Base;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class FirstViewModel : ViewModelBase
    {
        private readonly IViewService _viewService;
        
        [ObservableProperty]
        private string _title = default!;

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

        public FirstViewModel(
            IViewService viewService
            )
        {
            _viewService = viewService;
            Title = this.GetType().Name;
            //string imagePath = "pack://application:,,,/SonoCap.MES.UI;component/Resources/logo.png";
            string imagePath = "Resources/logo.png";
            Logo = Utilities.GetFileToImageSource(imagePath) ?? default!;

            //string imagePath = "/Resources/logo.png";

            // 이미지 로드
            //Logo = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
        }

        //public ImageBrush Logo
        //{
        //    get => _logo;
        //    set => SetProperty(ref _logo, value);
        //}
    }

}
