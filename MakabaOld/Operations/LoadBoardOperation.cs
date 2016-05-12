using System;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Core.Common;
using Core.Converters;
using Core.Models;
using Core.Operations;
using Makaba.Entities;
using Makaba.Services.Url;
using Makaba.Utils;

namespace Makaba.Operations {
    public class LoadBoardOperation : HttpGetJsonOperationBase<BoardPageEntity, BoardPage>, ILoadBoardOperation {
        private int page;
        private string id;

        public LoadBoardOperation(IUrlService urlService, IConverter<BoardPageEntity, BoardPage> resultConverter) {
            UrlService = urlService;
            ResultConverter = resultConverter;
        }

        private IUrlService UrlService { get; }
        public override Uri Uri { get; protected set; }
        protected override IConverter<BoardPageEntity, BoardPage> ResultConverter { get; }

        public int Page {
            get { return this.page; }
            set {
                this.page = value;
                UpdateUri();
            }
        }

        public string Id {
            get { return this.id; }
            set {
                this.id = value;
                UpdateUri();
            }
        }

        private void UpdateUri() {
            Uri = UrlService.GetBoardPageUrl(Id, Page);
        }

        protected override void SetupClient(HttpClient client, IHttpFilter filter) {
            base.SetupClient(client, filter);
            HttpClientUtils.SetupClient(UrlService, client, filter);
        }
    }
}