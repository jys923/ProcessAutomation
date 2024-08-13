using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.UI.Commons;
using SonoCap.MES.UI.ViewModels.Base;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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


        [RelayCommand]
        private void KeyDown(KeyEventArgs keyEventArgs)
        {
            Key key = keyEventArgs.Key == Key.System ? keyEventArgs.SystemKey : keyEventArgs.Key;
            Log.Information($"{nameof(KeyDown)} {nameof(key)}: {key}");
            if (key == Key.Escape)
            {
                App.Current.MainWindow.Close();
            }
        }

        public ProbeViewModel()
        {
            Title = this.GetType().Name;
        }

        public ProbeViewModel(string title, PTRView pTRView)
        {
            Title = title;
            _pTRView = pTRView;

            _defaultImg = Utilities.LoadBitmapFromResource("usImg.bmp");

            SrcImg00 = Utilities.GetFileToImageSource(PTRView.Test01.OriginalImg) ?? _defaultImg;
            ResImg00 = Utilities.GetFileToImageSource(PTRView.Test01.ChangedImg) ?? _defaultImg;

            SrcImg01 = Utilities.GetFileToImageSource(PTRView.Test02.OriginalImg) ?? _defaultImg;
            ResImg01 = Utilities.GetFileToImageSource(PTRView.Test02.ChangedImg) ?? _defaultImg;

            SrcImg02 = Utilities.GetFileToImageSource(PTRView.Test03.OriginalImg) ?? _defaultImg;
            ResImg02 = Utilities.GetFileToImageSource(PTRView.Test03.ChangedImg) ?? _defaultImg;

            SrcImg10 = Utilities.GetFileToImageSource(PTRView.Test04.OriginalImg) ?? _defaultImg;
            ResImg10 = Utilities.GetFileToImageSource(PTRView.Test04.ChangedImg) ?? _defaultImg;

            SrcImg11 = Utilities.GetFileToImageSource(PTRView.Test05.OriginalImg) ?? _defaultImg;
            ResImg11 = Utilities.GetFileToImageSource(PTRView.Test05.ChangedImg) ?? _defaultImg;

            SrcImg12 = Utilities.GetFileToImageSource(PTRView.Test06.OriginalImg) ?? _defaultImg;
            ResImg12 = Utilities.GetFileToImageSource(PTRView.Test06.ChangedImg) ?? _defaultImg;

            SrcImg20 = Utilities.GetFileToImageSource(PTRView.Test07?.OriginalImg ?? null ) ?? _defaultImg;
            ResImg20 = Utilities.GetFileToImageSource(PTRView.Test07?.ChangedImg ?? null ) ?? _defaultImg;

            SrcImg21 = Utilities.GetFileToImageSource(PTRView.Test08?.OriginalImg ?? null ) ?? _defaultImg;
            ResImg21 = Utilities.GetFileToImageSource(PTRView.Test08?.ChangedImg ?? null ) ?? _defaultImg;

            SrcImg22 = Utilities.GetFileToImageSource(PTRView.Test09?.OriginalImg ?? null) ?? _defaultImg;
            ResImg22 = Utilities.GetFileToImageSource(PTRView.Test09?.ChangedImg ?? null) ?? _defaultImg;
        }
    }
}
