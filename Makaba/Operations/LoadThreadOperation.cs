using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Core.Common;
using Core.Converters;
using Core.Models;
using Core.Operations;
using Makaba.Entities;
using Makaba.Utils;

namespace Makaba.Operations {
    public class LoadThreadOperation : HttpGetJsonOperationBase<ThreadEntity, Thread>, ILoadThreadOperation {

        public LoadThreadOperation(IUrlService urlService, IConverter<ThreadEntity, Thread> resultConverter) {
            UrlService = urlService;
            ResultConverter = resultConverter;
        }

        private IUrlService UrlService { get; }
        protected override IConverter<ThreadEntity, Thread> ResultConverter { get; }
        public override Uri Uri { get; protected set; }
        public string BoardId { get; set; }
        public long ThreadNumber { get; set; }
        public int FromPosition { get; set; }

        public override async Task<Thread> ExecuteAsync() {
            Uri = UrlService.GetThreadUrl(BoardId, ThreadNumber, FromPosition);
            return await base.ExecuteAsync();
        }

        protected override ThreadEntity ConvertEntity(string response) {
            var postsConverter = new JsonConverter<IList<PostEntity>>();
            return new ThreadEntity {
                BoardId = BoardId,
                Posts = postsConverter.Convert(response)
            };
        }

        protected override void SetupClient(HttpClient client, IHttpFilter filter) {
            base.SetupClient(client, filter);
            HttpClientUtils.SetupClient(UrlService, client, filter);
        }
    }
}