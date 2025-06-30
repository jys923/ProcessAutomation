using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.ViewModels;
using SonoCap.MES.UI.Views;

namespace SonoCap.MES.UI.Controls
{
    public class InputBoxProbe
    {
        public string? Show(IProbeRepository probeRepository)
        {
            var viewModel = new InputBoxProbeViewModel(probeRepository);
            var view = new InputBoxProbeView
            {
                DataContext = viewModel
            };

            return view.ShowDialog() == true
                ? viewModel.Response
                : null;
        }
    }
}
