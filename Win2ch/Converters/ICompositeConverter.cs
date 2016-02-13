using Windows.UI.Xaml.Data;

namespace Win2ch.Converters {
    public interface ICompositeConverter : IValueConverter {
        IValueConverter PostConverter { get; set; }
        object PostConverterParameter { get; set; }
    }
}
