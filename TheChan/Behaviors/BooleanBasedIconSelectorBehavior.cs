using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace TheChan.Behaviors {
    public class BooleanBasedIconSelectorBehavior : Behavior<AppBarButton> {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(bool), typeof(BooleanBasedIconSelectorBehavior),
            new PropertyMetadata(false, PropertyChangedCallback));

        public bool Value {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty TrueTextProperty = DependencyProperty.Register(
            "TrueText", typeof(string), typeof(BooleanBasedIconSelectorBehavior),
            new PropertyMetadata(string.Empty, PropertyChangedCallback));

        public string TrueText {
            get { return (string)GetValue(TrueTextProperty); }
            set { SetValue(TrueTextProperty, value); }
        }

        public static readonly DependencyProperty FalseTextProperty = DependencyProperty.Register(
            "FalseText", typeof(string), typeof(BooleanBasedIconSelectorBehavior),
            new PropertyMetadata(string.Empty, PropertyChangedCallback));

        public string FalseText {
            get { return (string)GetValue(FalseTextProperty); }
            set { SetValue(FalseTextProperty, value); }
        }

        public static readonly DependencyProperty TrueIconProperty = DependencyProperty.Register(
            "TrueIcon", typeof(IconElement), typeof(BooleanBasedIconSelectorBehavior),
            new PropertyMetadata(null, PropertyChangedCallback));

        public IconElement TrueIcon {
            get { return (IconElement)GetValue(TrueIconProperty); }
            set { SetValue(TrueIconProperty, value); }
        }

        public static readonly DependencyProperty FalseIconProperty = DependencyProperty.Register(
            "FalseIcon", typeof(IconElement), typeof(BooleanBasedIconSelectorBehavior),
            new PropertyMetadata(null, PropertyChangedCallback));


        public IconElement FalseIcon {
            get { return (IconElement)GetValue(FalseIconProperty); }
            set { SetValue(FalseIconProperty, value); }
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
                                                    DependencyPropertyChangedEventArgs e) {
            ((BooleanBasedIconSelectorBehavior) dependencyObject).UpdateIcon();
        }

        protected override void OnAttached() {
            UpdateIcon();
        }

        private void UpdateIcon() {
            if (AssociatedObject == null)
                return;

            AssociatedObject.Icon = Value ? TrueIcon : FalseIcon;
            AssociatedObject.Label = (Value ? TrueText : FalseText) ?? string.Empty;
        }
    }
}