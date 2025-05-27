using Microsoft.Xaml.Behaviors;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace SonoCap.WpfCommons.Behaviors
{
    public class FocusControllerBehavior : Behavior<UIElement>
    {
        public string FocusTargetName { get; set; } = string.Empty;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseDown -= OnPreviewMouseDown;
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var clicked = e.OriginalSource as DependencyObject;
            bool isClickInsideTarget = false;
            FrameworkElement? target = null;

            while (clicked != null)
            {
                if (clicked is FrameworkElement fe)
                {
                    if (fe.Name == FocusTargetName)
                    {
                        isClickInsideTarget = true;
                        target = fe;
                        break;
                    }
                }
                clicked = VisualTreeHelper.GetParent(clicked);
            }

            if (isClickInsideTarget && target != null)
            {
                // 명시적으로 포커스
                target.Focus();
            }
            else
            {
                // 포커스를 없애버림
                Keyboard.ClearFocus();
            }
        }
    }
}