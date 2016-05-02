using System.Collections.Generic;

namespace Core.Operations {
    public abstract class HttpGetJsonCollectionOperationBase<TEntity, TResult>
        : HttpGetJsonOperationBase<IList<TEntity>, IList<TResult>> { }
}