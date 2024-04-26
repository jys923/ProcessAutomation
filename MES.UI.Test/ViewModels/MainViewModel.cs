using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.UI.Test.ViewModels
{
    [ObservableObject]
    public partial class MainViewModel
    {
        [ObservableProperty]
        private string _title = "Main...View";

        public MainViewModel()
        {
            
        }
    }
}
