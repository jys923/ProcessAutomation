using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.UI.Commons;
using SonoCap.MES.UI.ViewModels.Base;
using System.Windows.Input;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class ProbeViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title = default!;

        [ObservableProperty]
        private PTRView _pTRView = default!;

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

            SrcImg00 = Utilities.GetFileToImageSource(PTRView.Test01.OriginalImg) ?? default!;
            ResImg00 = Utilities.GetFileToImageSource(PTRView.Test01.ChangedImg) ?? default!;

            SrcImg01 = Utilities.GetFileToImageSource(PTRView.Test02.OriginalImg) ?? default!;
            ResImg01 = Utilities.GetFileToImageSource(PTRView.Test02.ChangedImg) ?? default!;

            SrcImg02 = Utilities.GetFileToImageSource(PTRView.Test03.OriginalImg) ?? default!;
            ResImg02 = Utilities.GetFileToImageSource(PTRView.Test03.ChangedImg) ?? default!;

            SrcImg10 = Utilities.GetFileToImageSource(PTRView.Test04.OriginalImg) ?? default!;
            ResImg10 = Utilities.GetFileToImageSource(PTRView.Test04.ChangedImg) ?? default!;

            SrcImg11 = Utilities.GetFileToImageSource(PTRView.Test05.OriginalImg) ?? default!;
            ResImg11 = Utilities.GetFileToImageSource(PTRView.Test05.ChangedImg) ?? default!;

            SrcImg12 = Utilities.GetFileToImageSource(PTRView.Test06.OriginalImg) ?? default!;
            ResImg12 = Utilities.GetFileToImageSource(PTRView.Test06.ChangedImg) ?? default!;

            SrcImg20 = Utilities.GetFileToImageSource(PTRView.Test07?.OriginalImg ?? null ) ?? default!;
            ResImg20 = Utilities.GetFileToImageSource(PTRView.Test07?.ChangedImg ?? null ) ?? default!;

            SrcImg21 = Utilities.GetFileToImageSource(PTRView.Test08?.OriginalImg ?? null ) ?? default!;
            ResImg21 = Utilities.GetFileToImageSource(PTRView.Test08?.ChangedImg ?? null ) ?? default!;

            SrcImg22 = Utilities.GetFileToImageSource(PTRView.Test09?.OriginalImg ?? null) ?? default!;
            ResImg22 = Utilities.GetFileToImageSource(PTRView.Test09?.ChangedImg ?? null) ?? default!;
        }
    }
}
