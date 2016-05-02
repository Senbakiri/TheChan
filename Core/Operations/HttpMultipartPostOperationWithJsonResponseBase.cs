using Core.Converters;

namespace Core.Operations {
    public abstract class HttpMultipartPostOperationWithJsonResponseBase<TEntity, TResult> : HttpMultipartPostOperationBase<TEntity, TResult> {
        protected override IConverter<string, TEntity> EntityConverter { get; } = new JsonConverter<TEntity>();
    }
}