using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace Win2ch.Converters {
    public class CaseSet : List<ICase> {
        public static readonly object UndefinedObject = new object();
    }

    public interface ICase {
        object Key { get; set; }
        object Value { get; set; }
        Type KeyType { get; set; }
    }

    public interface ISwitchConverter : IValueConverter {
        CaseSet Cases { get; }
        object Default { get; set; }
        bool TypeMode { get; set; }
    }
}