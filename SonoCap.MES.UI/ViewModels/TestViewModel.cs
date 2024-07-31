﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.UI.Commons;
using SonoCap.MES.UI.ViewModels.Base;
using System.Windows.Input;
using System.Windows.Media;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class TestViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title = default!;

        [ObservableProperty]
        private Test _test = default!;

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

            SrcImg = Utilities.GetFileToImageSource(Test.OriginalImg) ?? default!;
            ResImg = Utilities.GetFileToImageSource(Test.ChangedImg) ?? default!;

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
