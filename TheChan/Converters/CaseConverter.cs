using System;
using Windows.UI.Xaml.Data;

namespace TheChan.Converters {
    public enum CharacterCasing {
        Default, LowerCase, UpperCase
    }

    public class CaseConverter : IValueConverter {
        
        public CharacterCasing Casing { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language) {
            var str = value.ToString();
            switch (Casing) {
                case CharacterCasing.LowerCase:
                    return str.ToLower();
                case CharacterCasing.UpperCase:
                    return str.ToUpper();
                default:
                    return str;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}