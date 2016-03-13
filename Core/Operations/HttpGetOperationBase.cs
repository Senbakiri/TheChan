using System;
using Windows.Web.Http;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Core.Converters;

namespace Core.Operations {
    public abstract class HttpGetOperationBase<TEntity, TResult> : IHttpOperation<TResult> {
        public abstract Uri Uri { get; }

        protected abstract IConverter<string, TEntity> EntityConverter { get; }
        protected abstract IConverter<TEntity, TResult> ResultConverter { get; }

        public virtual async Task<TResult> ExecuteAsync() {
            HttpClient client = CreateHttpClient();
            string response = await client.GetStringAsync(Uri);
            return ConvertToResult(ConvertEntity(response));
        }

        protected virtual TEntity ConvertEntity(string response) {
            return EntityConverter != null ? EntityConverter.Convert(response) : default(TEntity);
        }

        protected virtual TResult ConvertToResult(TEntity entity) {
            return ResultConverter != null ? ResultConverter.Convert(entity) : default(TResult);
        }

        protected virtual HttpClient CreateHttpClient() {
            return new HttpClient(new HttpBaseProtocolFilter {
                CacheControl = {
                    ReadBehavior = HttpCacheReadBehavior.MostRecent
                }
            });
        }
    }
}