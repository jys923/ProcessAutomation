using Microsoft.Xaml.Behaviors;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        //private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Add && AssociatedObject.Items.Count > 0)
        //    {
        //        var lastItem = AssociatedObject.Items[AssociatedObject.Items.Count - 1];
        //        AssociatedObject.Dispatcher.InvokeAsync(() => AssociatedObject.ScrollIntoView(lastItem));
        //    }
        //}

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && AssociatedObject.Items.Count > 0)
            {
                // ScrollViewer를 가져옵니다.
                ScrollViewer scrollViewer = GetDescendantByType(AssociatedObject, typeof(ScrollViewer)) as ScrollViewer;

                // ScrollViewer가 있는 경우, 스크롤 위치를 맨 아래로 이동시킵니다.
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToBottom();
                }
            }
        }

        // 지정된 타입의 자손 요소를 가져오는 메서드입니다.
        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null) return null;
            if (element.GetType() == type) return element;
            Visual foundElement = null;
            if (element is FrameworkElement frameworkElement)
            {
                frameworkElement.ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }

    }
}