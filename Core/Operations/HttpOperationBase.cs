using System;
using Windows.Web.Http;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Core.Converters;

namespace Core.Operations {
    public abstract class HttpOperationBase<T> : IHttpOperation<T> {
        public abstract Uri Uri { get; }

        protected abstract IConverter<string, T> Converter { get; }

        public virtual async Task<T> ExecuteAsync() {
            HttpClient client = CreateHttpClient();
            string response = await client.GetStringAsync(Uri);
            return Convert(response);
        }

        protected virtual T Convert(string response) {
            return Converter != null ? Converter.Convert(response) : default(T);
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