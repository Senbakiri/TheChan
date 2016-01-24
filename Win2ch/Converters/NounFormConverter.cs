using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace Win2ch.Converters {
    public class NounFormConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            if (!(parameter is string))
                return "";

            var count = (value as IEnumerable<object>).Count();
            var num = count % 100;
            var variants = ((string)parameter).Split(' ');

            if (num >= 11 && num <= 19)
                return variants[2];

            var i = num % 10;
            switch (i) {
                case 1:
                    return variants[0];
                case 2:
                case 3:
                case 4:
                    return variants[1];
                default:
                    return variants[2];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}
