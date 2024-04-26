using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.UI.ViewModels
{
    [ObservableObject]
    public partial class TestViewModel
    {
        [ObservableProperty]
        private string _title = default!;

        public TestViewModel()
        {
            Title = this.GetType().Name;
        }
    }
}
