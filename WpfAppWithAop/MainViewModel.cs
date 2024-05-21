#define TOOLKIT

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WpfAppWithAop
{
#if TOOLKIT
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        public MainViewModel()
        {
            Name = "World";
        }

        [RelayCommand]
        private void SayHello()
        {
            Name = "Hello, " + Name + "!";
        }
    }
#else
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private int _a = 0;
        private string _name;
        private ICommand _sayHelloCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public ICommand SayHelloCommand
        {
            get
            {
                if (_sayHelloCommand == null)
                {
                    _sayHelloCommand = new RelayCommand(param => SayHello(), param => CanSayHello());
                }
                return _sayHelloCommand;
            }
        }

        public MainViewModel()
        {
            Name = "World";
        }

        private void SayHello()
        {
            Name = "Hello, " + Name + "!";
        }

        private bool CanSayHello()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
#endif
}