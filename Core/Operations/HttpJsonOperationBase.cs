using Core.Converters;

namespace Core.Operations {
    public abstract class HttpJsonOperationBase<T> : HttpOperationBase<T> {
        protected override IConverter<string, T> Converter { get; } = new JsonConverter<T>();
    }
}
