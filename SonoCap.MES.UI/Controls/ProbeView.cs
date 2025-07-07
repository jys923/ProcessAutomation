using Microsoft.Extensions.DependencyInjection;
using SonoCap.MES.Models;
using SonoCap.MES.UI.ViewModels;

namespace SonoCap.MES.UI.Controls
{
    public class ProbeView
    {
        public static void Show(string title, PTRView probe) // 추가 매개변수
        {
            //ProbeViewModel viewModel = new ProbeViewModel(title, probe);
            var viewModel = App.Current.Services.GetRequiredService<ProbeViewModel>();
            viewModel.Initialize(title, probe);
            Views.ProbeView view = new Views.ProbeView
            {
                DataContext = viewModel
            };

            //return view.ShowDialog() ?? false
            //  ? viewModel.Response
            //  : null;
            view.ShowDialog();
        }
    }
}
