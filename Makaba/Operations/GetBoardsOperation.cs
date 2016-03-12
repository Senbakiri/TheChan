using System;
using System.Collections.Generic;
using System.Linq;
using Core.Converters;
using Core.Operations;
using Makaba.Entities;
using Makaba.Services.Url;

namespace Makaba.Operations {
    public class GetBoardsOperation : HttpJsonCollectionOperationBase<BoardCategoryEntity> {
        public IUrlService UrlService { get; }
        public override Uri Uri { get; }

        public GetBoardsOperation(IUrlService urlService) {
            UrlService = urlService;
            Uri = UrlService.GetBoardsListUrl();
        }

        protected override IList<BoardCategoryEntity> Convert(string response) {
            var converter = new JsonConverter<Dictionary<string, List<BoardEntity>>>();
            Dictionary<string, List<BoardEntity>> dict = converter.Convert(response);
            return dict.Select(kv => new BoardCategoryEntity {
                Name = kv.Key,
                Boards = kv.Value
            }).ToList();
        }
    }
}