namespace Core.Converters {
    public interface IConverter<in TSource, out TResult> {
        TResult Convert(TSource source);
    }
}