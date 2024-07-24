using System.Collections.ObjectModel;

namespace SonoCap.MES.UI.ViewModels
{
    public class TimeStampedObservableCollection<T> : ObservableCollection<string>
    {
        public new void Add(string item)
        {
            string timestampedItem = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {item}";
            base.Add(timestampedItem);
        }
    }
}
