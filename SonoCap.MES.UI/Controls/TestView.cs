using SonoCap.MES.Models;
using SonoCap.MES.UI.ViewModels;

namespace SonoCap.MES.UI.Controls
{
    public class TestView
    {
        public static void Show(string title, Test test) // 추가 매개변수
        {
            TestViewModel viewModel = new TestViewModel(title, test);
            Views.TestView view = new Views.TestView
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
