using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace Win2ch.Behaviors {
    class FavoriteButtonBehavior : Behavior<AppBarButton> {
        public static DependencyProperty IsInFavoritesProperty = DependencyProperty.Register(
            "IsInFavorites", typeof(bool), typeof(FavoriteButtonBehavior),
            new PropertyMetadata(false));

        private bool _IsInFavorites;

        public static bool GetIsInFavorties(FavoriteButtonBehavior obj) {
            return obj.IsInFavorites;
        }

        public static void SetIsInFavorites(FavoriteButtonBehavior obj, bool value) {
            obj.IsInFavorites = value;
        }

        public bool IsInFavorites {
            get { return _IsInFavorites; }
            set {
                _IsInFavorites = value;
                UpdateIcon();
            }
        }

        protected override void OnAttached() {
            UpdateIcon();
        }

        private void UpdateIcon() {
            Symbol icon;
            string text;
            if (IsInFavorites) {
                icon = Symbol.UnFavorite;
                text = "Удалить из избранного";
            } else {
                icon = Symbol.Favorite;
                text = "Добавить в избранное";
            }

            if (AssociatedObject != null) {
                AssociatedObject.Icon = new SymbolIcon(icon);
                AssociatedObject.Label = text;
            }
        }
    }
}
