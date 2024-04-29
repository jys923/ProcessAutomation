using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.UI.ViewModels
{
    public partial class ListViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = default!;

        public ListViewModel()
        {
            Title = this.GetType().Name;
        }
    }
}
