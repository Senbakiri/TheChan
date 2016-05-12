using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Core.Common;
using Core.Common.Links;
using Core.Converters;
using Core.Models;
using Core.Operations;
using Makaba.Converters;
using Makaba.Entities;
using Makaba.Utils;

namespace Makaba.Operations {
    public class LoadThreadOperation : HttpGetJsonOperationBase<ThreadEntity, Thread>, ILoadThreadOperation {

        public LoadThreadOperation(IUrlService urlService, IThreadConverter resultConverter) {
            UrlService = urlService;
            ResultConverter = resultConverter;
        }

        private IUrlService UrlService { get; }
        protected override IConverter<ThreadEntity, Thread> ResultConverter { get; }
        public override Uri Uri { get; protected set; }
        public ThreadLink Link { get; set; }
        public int StartPosition { get; set; }

        public override async Task<Thread> ExecuteAsync() {
            var converter = ResultConverter as IThreadConverter;
            if (converter != null)
                converter.Link = Link;
            Uri = UrlService.GetThreadUrl(Link.BoardId, Link.ThreadNumber, StartPosition);
            return await base.ExecuteAsync();
        }

        protected override ThreadEntity ConvertEntity(string response) {
            var postsConverter = new JsonConverter<IList<PostEntity>>();
            return new ThreadEntity {
                Posts = postsConverter.Convert(response)
            };
        }

        protected override void SetupClient(HttpClient client, IHttpFilter filter) {
            base.SetupClient(client, filter);
            HttpClientUtils.SetupClient(UrlService, client, filter);
        }
    }
}