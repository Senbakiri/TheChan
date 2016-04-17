using System;
using Windows.Web.Http;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Core.Converters;

namespace Core.Operations {
    public abstract class HttpGetOperationBase<TEntity, TResult> : IHttpOperation<TResult> {

        protected abstract IConverter<string, TEntity> EntityConverter { get; }
        protected abstract IConverter<TEntity, TResult> ResultConverter { get; }

        public abstract Uri Uri { get; protected set; }

        public virtual async Task<TResult> ExecuteAsync() {
            IHttpFilter filter = GetHttpFilter();
            var client = new HttpClient(filter);
            SetupClient(client, filter);
            string response = await client.GetStringAsync(Uri);
            return ConvertToResult(ConvertEntity(response));
        }

        protected virtual TEntity ConvertEntity(string response) {
            return EntityConverter != null ? EntityConverter.Convert(response) : default(TEntity);
        }

        protected virtual TResult ConvertToResult(TEntity entity) {
            return ResultConverter != null ? ResultConverter.Convert(entity) : default(TResult);
        }

        protected virtual IHttpFilter GetHttpFilter() {
            var filter = new HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            return filter;
        }

        protected virtual void SetupClient(HttpClient client, IHttpFilter filter) {
            client.DefaultRequestHeaders.TryAppendWithoutValidation(
                "Accept", "text/html,application/xhtml+xml,application/xml");
            client.DefaultRequestHeaders.TryAppendWithoutValidation(
                "Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.TryAppendWithoutValidation(
                "User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            client.DefaultRequestHeaders.TryAppendWithoutValidation(
                "Accept-Charset", "ISO-8859-1");
        }
    }
}