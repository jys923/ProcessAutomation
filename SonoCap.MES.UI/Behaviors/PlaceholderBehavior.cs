using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SonoCap.MES.UI.Behaviors
{
    public class PlaceholderBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(PlaceholderBehavior), new PropertyMetadata(default(string), OnPlaceholderChanged));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += RemovePlaceholder;
            AssociatedObject.LostFocus += ShowPlaceholder;
            ShowPlaceholder(AssociatedObject, null);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= RemovePlaceholder;
            AssociatedObject.LostFocus -= ShowPlaceholder;
        }

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlaceholderBehavior behavior && behavior.AssociatedObject != null)
            {
                behavior.ShowPlaceholder(behavior.AssociatedObject, null);
            }
        }

        private void ShowPlaceholder(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Foreground = Brushes.Gray;
                textBox.Text = Placeholder;
            }
        }

        private void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text == Placeholder)
            {
                textBox.Foreground = Brushes.Black;
                textBox.Text = string.Empty;
            }
        }
    }

}
