using System;
using System.Collections;
using Windows.UI.Xaml.Data;

namespace Win2ch.Converters {
    public class ItemIndexConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            var list = parameter as IList;
            if (list == null)
                return 0;

            return list.IndexOf(value) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}