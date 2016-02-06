using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;

namespace Win2ch.Behaviors {
    public class ContextMenuBehavior : Behavior<FrameworkElement> {

        public static readonly DependencyProperty MenuFlyoutProperty = DependencyProperty.Register(
            "MenuFlyout", typeof (MenuFlyout), typeof (ContextMenuBehavior),
            new PropertyMetadata(default(MenuFlyout)));

        public MenuFlyout MenuFlyout {
            get { return (MenuFlyout) GetValue(MenuFlyoutProperty); }
            set { SetValue(MenuFlyoutProperty, value); }
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
            MenuFlyout?.ShowAt(AssociatedObject, e.GetPosition(AssociatedObject));
        }

        private void AssociatedObjectOnHolding(object sender, HoldingRoutedEventArgs e) {
            MenuFlyout?.ShowAt(AssociatedObject, e.GetPosition(AssociatedObject));
        }
    }
}
