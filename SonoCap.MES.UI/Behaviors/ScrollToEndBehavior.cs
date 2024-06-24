using Microsoft.Xaml.Behaviors;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace SonoCap.MES.UI.Behaviors
{
    public class ScrollToEndBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject.Items is INotifyCollectionChanged notifyCollection)
            {
                notifyCollection.CollectionChanged += OnCollectionChanged;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject.Items is INotifyCollectionChanged notifyCollection)
            {
                notifyCollection.CollectionChanged -= OnCollectionChanged;
            }
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && AssociatedObject.Items.Count > 0)
            {
                var lastItem = AssociatedObject.Items[AssociatedObject.Items.Count - 1];
                AssociatedObject.Dispatcher.InvokeAsync(() => AssociatedObject.ScrollIntoView(lastItem));
            }
        }
    }
}