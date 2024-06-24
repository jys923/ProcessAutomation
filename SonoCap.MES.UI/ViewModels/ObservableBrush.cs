using System.ComponentModel;
using System.Windows.Media;

namespace SonoCap.MES.UI.ViewModels
{
    // ObservableBrush 클래스는 INotifyPropertyChanged 인터페이스를 구현합니다.
    // 이 인터페이스는 속성이 변경될 때마다 UI에 알리는 기능을 제공합니다.
    public class ObservableBrush : INotifyPropertyChanged
    {
        // _value는 Brush 타입의 private 필드입니다. 이 필드는 Value 속성의 백업 저장소 역할을 합니다.
        private Brush? _value;

        // Value는 Brush 타입의 public 속성입니다. 이 속성은 get 및 set 접근자를 가지고 있습니다.
        public Brush? Value
        {
            // get 접근자는 _value 필드의 값을 반환합니다.
            get { return _value; }

            // set 접근자는 _value 필드에 새 값을 할당하고, 필요한 경우 OnPropertyChanged를 호출하여 속성이 변경되었음을 알립니다.
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        // PropertyChanged 이벤트는 속성이 변경될 때마다 발생합니다. 이 이벤트는 UI에 속성이 변경되었음을 알립니다.
        public event PropertyChangedEventHandler? PropertyChanged;

        // OnPropertyChanged 메서드는 PropertyChanged 이벤트를 발생시킵니다. 이 메서드는 속성이 변경될 때마다 호출됩니다.
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
