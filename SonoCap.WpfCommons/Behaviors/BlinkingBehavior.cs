using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;

namespace SonoCap.WpfCommons.Behaviors
{
    public class BlinkingBehavior : Behavior<Button>
    {
        public int BlinkIndex
        {
            get { return (int)GetValue(BlinkIndexProperty); }
            set { SetValue(BlinkIndexProperty, value); }
        }

        public static readonly DependencyProperty BlinkIndexProperty =
            DependencyProperty.Register("BlinkIndex", typeof(int), typeof(BlinkingBehavior), new PropertyMetadata(0, OnBlinkIndexChanged));

        public int TriggerValue
        {
            get { return (int)GetValue(TriggerValueProperty); }
            set { SetValue(TriggerValueProperty, value); }
        }

        public static readonly DependencyProperty TriggerValueProperty =
            DependencyProperty.Register("TriggerValue", typeof(int), typeof(BlinkingBehavior), new PropertyMetadata(0));

        private Storyboard _storyboard;

        private static void OnBlinkIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as BlinkingBehavior;
            behavior?.UpdateBlinking();
        }

        private void UpdateBlinking()
        {
            if (AssociatedObject == null)
            {
                return;
            }

            if (_storyboard != null)
            {
                _storyboard.Stop(AssociatedObject);
            }

            if (BlinkIndex == TriggerValue)
            {
                _storyboard = new Storyboard
                {
                    RepeatBehavior = RepeatBehavior.Forever
                };

                var animation = new ColorAnimation
                {
                    To = Colors.Yellow,
                    Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                    AutoReverse = true
                };

                Storyboard.SetTarget(animation, AssociatedObject);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));

                _storyboard.Children.Add(animation);
                _storyboard.Begin(AssociatedObject, true);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateBlinking();
        }

        protected override void OnDetaching()
        {
            if (_storyboard != null)
            {
                _storyboard.Stop(AssociatedObject);
            }
            base.OnDetaching();
        }
    }
}
