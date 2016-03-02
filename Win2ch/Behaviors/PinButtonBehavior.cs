using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace Win2ch.Behaviors {
    class PinButtonBehavior : Behavior<AppBarButton> {
        public static DependencyProperty IsPinnedProperty = DependencyProperty.Register(
            "IsPinned", typeof(bool), typeof(PinButtonBehavior),
            new PropertyMetadata(false));

        private bool _IsPinned;

        public static bool GetIsInFavorties(PinButtonBehavior obj) {
            return obj.IsPinned;
        }

        public static void SetIsInFavorites(PinButtonBehavior obj, bool value) {
            obj.IsPinned = value;
        }

        public bool IsPinned {
            get { return _IsPinned; }
            set {
                _IsPinned = value;
                UpdateIcon();
            }
        }

        protected override void OnAttached() {
            UpdateIcon();
        }

        private void UpdateIcon() {
            Symbol icon;
            string text;
            if (IsPinned) {
                icon = Symbol.UnPin;
                text = "Убрать с начального экрана";
            } else {
                icon = Symbol.Pin;
                text = "Закрепить на начальном экране";
            }

            if (AssociatedObject != null) {
                AssociatedObject.Icon = new SymbolIcon(icon);
                AssociatedObject.Label = text;
            }
        }
    }
}
