using Windows.Web.Http;

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
    }
}