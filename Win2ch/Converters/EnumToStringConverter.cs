using System;
using System.Reflection;
using Windows.UI.Xaml.Data;
using Win2ch.Attributes;

namespace Win2ch.Converters {
    public class EnumToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            if (!(value is Enum))
                return null;

            var @enum = value as Enum;
            var description = @enum.ToString();

            var attrib = GetAttribute<DisplayAttribute>(@enum);
            if (attrib != null)
                description = attrib.Name;

            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language) {
            throw new NotImplementedException();
        }

        private T GetAttribute<T>(Enum enumValue) where T : Attribute {
            return enumValue.GetType().GetTypeInfo()
                .GetDeclaredField(enumValue.ToString())
                .GetCustomAttribute<T>();
        }
    }
}