using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Win2ch.Converters {
    public enum CharacterCasing {
        Default, LowerCase, UpperCase
    }

    public class CaseConverter : IValueConverter {
        
        public CharacterCasing Casing { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language) {
            switch (Casing) {
                case CharacterCasing.LowerCase:
                    return value.ToString().ToLower();
                case CharacterCasing.UpperCase:
                    return value.ToString().ToUpper();
                default:
                    return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}