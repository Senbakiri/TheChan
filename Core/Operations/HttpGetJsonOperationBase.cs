using Core.Converters;

namespace Core.Operations {
    public abstract class HttpGetJsonOperationBase<TEntity, TResult> : HttpGetOperationBase<TEntity, TResult> {
        protected override IConverter<string, TEntity> EntityConverter { get; } = new JsonConverter<TEntity>();
    }
}
