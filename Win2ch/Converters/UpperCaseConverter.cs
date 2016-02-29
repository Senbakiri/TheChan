using System;
using Windows.UI.Xaml.Data;

namespace Win2ch.Converters {
    public class UpperCaseConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            var source = value?.ToString() ?? "";
            return source.ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}