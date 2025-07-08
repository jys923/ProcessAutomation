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

namespace SonoCap.MES.UI.ViewModels
{
    public partial class TestViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title = default!;

        [ObservableProperty]
        private Test _test = default!;

        private BitmapImage _defaultImg = default!;

        [ObservableProperty]
        private ImageSource _srcImg = default!;

        [ObservableProperty]
        private ImageSource _resImg = default!;

        [ObservableProperty]
        private Color[] _cellColors = default!;

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

        public TestViewModel(string title, Test test)
        {
            Title = title;
            _test = test;

            _defaultImg = Utilities.LoadBitmapFromResource("usImg.bmp");

            var basePath = App.appSettings.Path.ExportImg;
            var phaseMap = App.appSettings.Path.ExportImgPhase;

            string? sn = test.TransducerModule?.Sn
                  ?? test.Probe?.Sn
                  ?? test.Transducer?.Sn;

            string path = Utilities.GetExportImgPath(basePath, phaseMap, Test.TestCategoryId, sn);

            SrcImg = Utilities.LoadOrDefault(path, Test.OriginalImg, _defaultImg);
            ResImg = Utilities.LoadOrDefault(path, Test.ChangedImg, _defaultImg);

            _cellColors = new Color[9];
            // Initialize all cells to LightBlue
            for (int i = 0; i < 9; i++)
            {
                _cellColors[i] = Colors.LightBlue;
            }
            // Example: set one cell to Yellow
            _cellColors[Test.TestTypeId - 1 + (Test.TestCategoryId -1) * 3] = Colors.Yellow;
        }
    }
}
