using Core.Converters;

namespace Core.Operations {
    public abstract class HttpPostOperationWithJsonResponseBase<TEntity, TResult> : HttpPostOperationBase<TEntity, TResult> {
        protected override IConverter<string, TEntity> EntityConverter { get; } = new JsonConverter<TEntity>();
    }
}