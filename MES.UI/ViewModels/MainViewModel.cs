using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace MES.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = default!;

        public MainViewModel()
        {
            Title = this.GetType().Name;
        }

        [RelayCommand]
        private void ToTest()
        {
            Debug.WriteLine("ToTest");
            TestView? testView = App.Current.Services.GetService<TestView>()!;
            testView.Show();
        }

        [RelayCommand]
        private void ToList()
        {
            Debug.WriteLine("ToList");
            ListView? listView = App.Current.Services.GetService<ListView>()!;
            listView.Show();
        }
    }
}
