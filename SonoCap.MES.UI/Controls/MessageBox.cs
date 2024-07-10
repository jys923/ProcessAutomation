using SonoCap.MES.Models.Enums;
using SonoCap.MES.UI.ViewModels;
using SonoCap.MES.UI.Views;

namespace SonoCap.MES.UI.Controls
{
    public static class MessageBox
  {
    public static MessageBoxExResult Show(string title, string message)
    {
      return Show(title, message, "확인", null, null);
    }

    public static MessageBoxExResult Show(string title, string message, string okText)
    {
      return Show(title, message, okText, null, null);
    }

    public static MessageBoxExResult Show(string title, string message, string okText, string? cancelText)
    {
      return Show(title, message, okText, null, cancelText);
    }

    public static MessageBoxExResult Show(string title, string message, string button1Text, string? button2Text, string? button3Text)
    {
      var viewModel = new MessageBoxViewModel(title, message, button1Text, button2Text, button3Text);
      MessageBoxView view = new MessageBoxView
      {
        DataContext = viewModel
      };

      view.ShowDialog();

      return viewModel.MessageBoxExResult;
    }
  }
}
