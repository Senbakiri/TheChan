using System;
using Core.Converters;
using Core.Models;
using Core.Operations;
using Makaba.Entities;
using Makaba.Services.Url;

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
            Uri = UrlService.GetBoardUrl(Id, Page);
        }
    }
}