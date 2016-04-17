using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Core.Common;
using Core.Converters;
using Core.Models;
using Core.Operations;
using Makaba.Converters;
using Makaba.Entities;
using Makaba.Utils;

namespace Makaba.Operations {
    public class GetPostOperation : HttpGetJsonOperationBase<IList<PostEntity>, Post>, IGetPostOperation {
        private IUrlService UrlService { get; }
        private PostConverter PostConverter { get; }
        protected override IConverter<IList<PostEntity>, Post> ResultConverter { get; } = null;
        public override Uri Uri { get; protected set; }
        public string BoardId { get; set; }
        public long PostNumber { get; set; }

        public GetPostOperation(IUrlService urlService, PostConverter postConverter) {
            UrlService = urlService;
            PostConverter = postConverter;
        }

        protected override Post ConvertToResult(IList<PostEntity> entity) {
            PostConverter.BoardId = BoardId;
            return PostConverter.Convert(entity.First());
        }

        public override Task<Post> ExecuteAsync() {
            Uri = UrlService.GetPostUrl(BoardId, PostNumber);
            return base.ExecuteAsync();
        }

        protected override void SetupClient(HttpClient client, IHttpFilter filter) {
            base.SetupClient(client, filter);
            HttpClientUtils.SetupClient(UrlService, client, filter);
        }
    }
}