using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.UI.ViewModels
{
    public partial class ListViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = default!;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now.AddMonths(-1);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now;

        //[ObservableProperty]
        //private ObservableCollection<string> _testCategories;

        //[ObservableProperty]
        //private string _testCategory = default!;

        //[ObservableProperty]
        //private ObservableCollection<string> _testResults;

        //[ObservableProperty]
        //private string _testResult = default!;

        //[ObservableProperty]
        //private int _pcNo = default!;

        //[ObservableProperty]
        //private string _tester = default!;

        [ObservableProperty]
        private string _probeSn = default!;

        [ObservableProperty]
        private string _tDMdSn = default!;

        [ObservableProperty]
        private string _mTMdSn = default!;

        [ObservableProperty]
        private ObservableCollection<Probe> _probes = default!;

        [RelayCommand]
        private void Search()
        {
            Debug.WriteLine($"{nameof(Search)}");
        }

        [RelayCommand]
        private void Export()
        {
            Debug.WriteLine($"{nameof(Export)}");
        }

        public ListViewModel()
        {
            Title = this.GetType().Name;
            
            //TestCategories = new ObservableCollection<string>
            //{
            //    "ALL",
            //    "공정용",
            //    "출하용",
            //};

            //TestCategory = TestCategories[0];

            //TestResults = new ObservableCollection<string>
            //{
            //    "ALL",
            //    "FAIL",
            //    "PASS",
            //};

            //TestResult = TestResults[0];
            ////출력

            //PcNo = 1;

            //Tester = "yoon";

            ProbeSn = "P S/N";

            TDMdSn = "transducer Module";

            MTMdSn = "aaaa";

            //db 조회

            Probes = new ObservableCollection<Probe>();

            //Probes.Add(new Probe { ProbeSn = ProbeSn, });
        }
    }
}
