using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.UI.Models;
using MES.UI.Repositories;
using MES.UI.Repositories.interfaces;
using MES.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace MES.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IMotorModuleRepository _motorModuleRepository;
        private readonly IProbeRepository _probeRepository;
        private readonly ITestCategoryRepository _testCategoryRepository;
        private readonly ITesterRepository _testerRepository;
        private readonly ITestRepository _testRepository;
        private readonly ITestTypeRepository _testTypeRepository;
        private readonly ITransducerModuleRepository _transducerModuleRepository;
        private readonly ITransducerTypeRepository _transducerTypeRepository;

        [ObservableProperty]
        private string _title = default!;

        public MainViewModel(
            IMotorModuleRepository motorModuleRepository,
            IProbeRepository probeRepository,
            ITestCategoryRepository testCategoryRepository,
            ITesterRepository testerRepository,
            ITestRepository testRepository,
            ITestTypeRepository testTypeRepository,
            ITransducerModuleRepository transducerModuleRepository,
            ITransducerTypeRepository transducerTypeRepository)
        {
            _motorModuleRepository = motorModuleRepository;
            _probeRepository = probeRepository; 
            _testCategoryRepository = testCategoryRepository;
            _testerRepository = testerRepository;
            _testRepository = testRepository;
            _testTypeRepository = testTypeRepository;
            _transducerModuleRepository = transducerModuleRepository;
            _transducerTypeRepository = transducerTypeRepository;
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
            ProbeListView? probeListView = App.Current.Services.GetService<ProbeListView>()!;
            probeListView.Show();
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
            //var aaa =_probeRepository.GetProbeSN();
            var aaa = _testRepository.GetTestProbe();
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
