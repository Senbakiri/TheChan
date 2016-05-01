using System;
using Windows.UI.Xaml.Data;
using Win2ch.Common.Core;

namespace Win2ch.Converters {
    public class IsInFavoritesToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            var boolVal = (bool) value;
            return boolVal
                ? Tab.GetLocalizationStringForView("Post", "Unfavorite.Text")
                : Tab.GetLocalizationStringForView("Post", "Favorite.Text");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}