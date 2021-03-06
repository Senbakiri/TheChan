﻿using System;
using Windows.UI.Xaml.Data;

namespace TheChan.Converters {
    public class InverseBoolConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return !(value is bool && (bool) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new System.NotImplementedException();
        }
    }
}