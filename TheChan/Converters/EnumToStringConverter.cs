using System;
using Windows.UI.Xaml.Data;
using TheChan.Common.Core;

namespace TheChan.Converters {
    public class EnumToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            string typeName = value.GetType().Name;
            string name = value.ToString();
            return Tab.GetLocalizationStringForView(typeName, name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}