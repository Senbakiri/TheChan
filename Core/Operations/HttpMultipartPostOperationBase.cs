using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace Core.Operations {
    public abstract class HttpMultipartPostOperationBase<TEntity, TResult> : HttpPostOperationBase<TEntity, TResult> {
        private HttpMultipartFormDataContent MultipartContent { get; }
        protected sealed override IHttpContent Content { get; set; }

        protected HttpMultipartPostOperationBase() {
            MultipartContent = new HttpMultipartFormDataContent();
            Content = MultipartContent;
        }

        protected void AddString(string name, object value) {
            MultipartContent.Add(new HttpStringContent(value?.ToString() ?? string.Empty), name);
        }

        protected async Task AddFile(IRandomAccessStreamReference streamReference, string name, string fileName) {
            IRandomAccessStreamWithContentType stream = await streamReference.OpenReadAsync();
            var content = new HttpStreamContent(stream);
            content.Headers.ContentType = HttpMediaTypeHeaderValue.Parse(stream.ContentType);
            MultipartContent.Add(content, name, fileName);
        }
    }
}