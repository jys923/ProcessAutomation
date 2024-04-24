using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace UI.Test.ViewModels
{
    public class ViewModelLocator
    {
        //public MainViewModel MainViewModel => Ioc.Default.GetService<MainViewModel>();

        public MainViewModel MainViewModel => App.Current.Services!.GetRequiredService<MainViewModel>();
    }
}
