using System.ComponentModel;

namespace SonoCap.MES.UI.Validation
{
    public class ValidationItem : INotifyPropertyChanged
    {
        private bool _isValid = false;
        private string _message = string.Empty;
        private string _waterMarkText = string.Empty;
        private bool _isEnabled = false;

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public string WaterMarkText
        {
            get { return _waterMarkText; }
            set
            {
                _waterMarkText = value;
                OnPropertyChanged(nameof(WaterMarkText));
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
