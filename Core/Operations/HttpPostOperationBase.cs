using System;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Core.Operations {
    public abstract class HttpPostOperationBase<TEntity, TResult> : HttpOperationBase<TEntity, TResult> {

        protected abstract IHttpContent Content { get; set; }

        public override async Task<TResult> ExecuteAsync() {
            IHttpFilter filter = GetHttpFilter();
            var client = new HttpClient(filter);
            SetupClient(client, filter);
            HttpResponseMessage response = await client.PostAsync(Uri, Content);
            response.EnsureSuccessStatusCode();
            string responseMessage = await response.Content.ReadAsStringAsync();
            return ConvertToResult(ConvertEntity(responseMessage));
        }
    }
}