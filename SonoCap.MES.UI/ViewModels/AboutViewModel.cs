using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonoCap.MES.UI.ViewModels.Base;
using System.Reflection;
using System.Windows;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class AboutViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _appName = @"sonocap mes";

        [ObservableProperty]
        //private string _versionInfo = $"Version: {Assembly.GetExecutingAssembly().GetName().Version} (빌드: {BuildDate})";
        private string _versionInfo = $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";

        [ObservableProperty]
        private string _releaseNotes =
            """
            v 1.0.0
                - 영상 저장 mp4
                - 사진 저장 BGRA32 > Gray8 > png
                - 영상 PRF 변경 시 모터객체의 내부값 만 변경
                - 모터 시리얼 초기화 시퀀스: motor OFF > mode sel > motor ON
                - snackbar 메시지 큐 사용
            """;

        public string BuildDate => "2025.04.22"; // 또는 자동화 가능

        [RelayCommand]
        private void Close()
        {
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?.Close();
        }
        public AboutViewModel()
        {
            Title = GetType().Name;
        }
    }
}
