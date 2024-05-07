using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.UI.Models;
using MES.UI.Repositories;
using MES.UI.Repositories.interfaces;
using MES.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq;

namespace MES.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ITestRepository _testRepository;
        private readonly IProbeRepository _probeRepository;

        [ObservableProperty]
        private string _title = default!;

        public MainViewModel(ITestRepository testRepository,IProbeRepository probeRepository)
        {
            _testRepository = testRepository;
            _probeRepository = probeRepository;
            Title = this.GetType().Name;
        }

        [RelayCommand]
        private void ToTest()
        {
            Debug.WriteLine("ToTest");
            TestView? testView = App.Current.Services.GetService<TestView>()!;
            //testView.Show();
            testView.ShowDialog();
        }

        [RelayCommand]
        private void ToList()
        {
            Debug.WriteLine("ToList");
            ListView? listView = App.Current.Services.GetService<ListView>()!;
            listView.Show();
        }

        [RelayCommand]
        private void ToTestList()
        {
            Debug.WriteLine("ToTestList");
            TestListView? testListView = App.Current.Services.GetService<TestListView>()!;
            testListView.Show();
        }

        [RelayCommand]
        private async Task Master()
        {
            Debug.WriteLine("Master");
            var aaa =_probeRepository.GetProbeSN();
            Debug.WriteLine("Master:" + aaa.ToList().Count);
        }

        [RelayCommand]
        private async Task Select1Async()
        {
            Debug.WriteLine("Select1");
            //IEnumerable<Models.Test> enumerable = await _testRepository.GetAllAsync();
            List<ProbeTestResult> enumerable = _probeRepository.GetProbeSNSql();
            Debug.WriteLine("Select1:" + enumerable.ToList().Count);
        }
    }
}
