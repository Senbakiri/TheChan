using System;
using Windows.Web.Http;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Core.Converters;

namespace Core.Operations {
    public abstract class HttpGetOperationBase<TEntity, TResult> : HttpOperationBase<TEntity, TResult> {

        public override async Task<TResult> ExecuteAsync() {
            IHttpFilter filter = GetHttpFilter();
            var client = new HttpClient(filter);
            SetupClient(client, filter);
            string response = await client.GetStringAsync(Uri);
            return ConvertToResult(ConvertEntity(response));
        }

    }
}