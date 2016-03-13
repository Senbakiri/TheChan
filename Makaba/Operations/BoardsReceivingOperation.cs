using System;
using System.Collections.Generic;
using System.Linq;
using Core.Converters;
using Core.Models;
using Core.Operations;
using Makaba.Entities;
using Makaba.Services.Url;

namespace Makaba.Operations {
    public class BoardsReceivingOperation : HttpGetJsonCollectionOperationBase<BoardCategoryEntity, BoardsCategory> {
        private IUrlService UrlService { get; }
        public override Uri Uri { get; }
        protected override IConverter<IList<BoardCategoryEntity>, IList<BoardsCategory>> ResultConverter { get; }

        public BoardsReceivingOperation(
            IUrlService urlService,
            IConverter<IList<BoardCategoryEntity>, IList<BoardsCategory>> resultConverter) {

            UrlService = urlService;
            ResultConverter = resultConverter;
            Uri = UrlService.GetBoardsListUrl();
        }

        protected override IList<BoardCategoryEntity> ConvertEntity(string response) {
            var converter = new JsonConverter<Dictionary<string, BoardEntity[]>>();
            Dictionary<string, BoardEntity[]> dict = converter.Convert(response);
            return dict.Select(kv => new BoardCategoryEntity {
                Name = kv.Key,
                Boards = kv.Value
            }).ToList();
        }
        
    }
}