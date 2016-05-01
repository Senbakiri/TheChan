using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Microsoft.Xaml.Interactivity;

namespace Win2ch.Behaviors {
    [ContentProperty(Name = "MenuFlyout")]
    public class ContextMenuBehavior : Behavior<FrameworkElement> {

        public static readonly DependencyProperty MenuFlyoutProperty = DependencyProperty.Register(
            "MenuFlyout", typeof(MenuFlyout), typeof(ContextMenuBehavior),
            new PropertyMetadata(default(MenuFlyout)));

        public MenuFlyout MenuFlyout {
            get { return (MenuFlyout)GetValue(MenuFlyoutProperty); }
            set { SetValue(MenuFlyoutProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            "IsEnabled", typeof(bool), typeof(ContextMenuBehavior), new PropertyMetadata(true));

        public bool IsEnabled {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty ShowAtCoordinatesProperty = DependencyProperty.Register(
            "ShowAtCoordinates", typeof(bool), typeof(ContextMenuBehavior), new PropertyMetadata(default(bool)));

        public bool ShowAtCoordinates {
            get { return (bool)GetValue(ShowAtCoordinatesProperty); }
            set { SetValue(ShowAtCoordinatesProperty, value); }
        }

        protected override void OnAttached() {
            AssociatedObject.Holding += AssociatedObjectOnHolding;
            AssociatedObject.RightTapped += AssociatedObjectOnRightTapped;
        }

        protected override void OnDetaching() {
            AssociatedObject.Holding -= AssociatedObjectOnHolding;
            AssociatedObject.RightTapped -= AssociatedObjectOnRightTapped;
        }

        private void AssociatedObjectOnRightTapped(object sender, RightTappedRoutedEventArgs e) {
            if (!IsEnabled)
                return;

            if (ShowAtCoordinates)
                MenuFlyout?.ShowAt(AssociatedObject, e.GetPosition(AssociatedObject));
            else
                MenuFlyout?.ShowAt(AssociatedObject);
        }

        private void AssociatedObjectOnHolding(object sender, HoldingRoutedEventArgs e) {
            if (!IsEnabled)
                return;

            if (ShowAtCoordinates)
                MenuFlyout?.ShowAt(AssociatedObject, e.GetPosition(AssociatedObject));
            else
                MenuFlyout?.ShowAt(AssociatedObject);
        }
    }
}