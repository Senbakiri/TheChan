using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Xaml.Interactivity;

namespace TheChan.Behaviors {
    internal class HighlightBehavior : Behavior<Border> {
        public static readonly DependencyProperty HightlightingStartProperty = DependencyProperty.Register(
            "HightlightingStart", typeof(int), typeof(HighlightBehavior), new PropertyMetadata(-1, PropertyChangedCallback));

        public int HightlightingStart {
            get { return (int)GetValue(HightlightingStartProperty); }
            set { SetValue(HightlightingStartProperty, value); }
        }

        public static readonly DependencyProperty HighlightProperty = DependencyProperty.Register(
            "Highlight", typeof(bool), typeof(HighlightBehavior), new PropertyMetadata(false, PropertyChangedCallback));

        public bool Highlight {
            get { return (bool)GetValue(HighlightProperty); }
            set { SetValue(HighlightProperty, value); }
        }

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(HighlightBehavior), new PropertyMetadata(null, PropertyChangedCallback));

        public Brush Background {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty CurrentPositionProperty = DependencyProperty.Register(
            "CurrentPosition", typeof(int), typeof(HighlightBehavior), new PropertyMetadata(-1, PropertyChangedCallback));

        public int CurrentPosition {
            get { return (int)GetValue(CurrentPositionProperty); }
            set { SetValue(CurrentPositionProperty, value); }
        }

        private Brush oldBackground;

        private static void PropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e) {
            ((HighlightBehavior)s).HighlightItem();
        }

        protected override void OnAttached() {
            this.oldBackground = AssociatedObject.Background;
            HighlightItem();
        }

        private void HighlightItem() {
            if (AssociatedObject == null)
                return;
            AssociatedObject.Background = this.oldBackground ?? new SolidColorBrush(Colors.Transparent);
            if (CurrentPosition == -1 ||
                HightlightingStart == -1 ||
                CurrentPosition < HightlightingStart ||
                !Highlight)
                return;
            AssociatedObject.Background = Background;
        }

        protected override void OnDetaching() {
            AssociatedObject.Background = this.oldBackground;
        }
    }
}