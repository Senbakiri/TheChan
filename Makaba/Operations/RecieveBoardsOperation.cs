using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Core.Converters;
using Core.Models;
using Core.Operations;
using Makaba.Entities;
using Makaba.Services.Url;

namespace Makaba.Operations {
    public class RecieveBoardsOperation : HttpGetJsonCollectionOperationBase<BoardsCategoryEntity, BoardsCategory> {
        private IUrlService UrlService { get; }
        public sealed override Uri Uri { get; protected set; }
        protected override IConverter<IList<BoardsCategoryEntity>, IList<BoardsCategory>> ResultConverter { get; }

        public RecieveBoardsOperation(
            IUrlService urlService,
            IConverter<IList<BoardsCategoryEntity>, IList<BoardsCategory>> resultConverter) {

            UrlService = urlService;
            ResultConverter = resultConverter;
            Uri = UrlService.GetBoardsListUrl();
        }

        protected override IList<BoardsCategoryEntity> ConvertEntity(string response) {
            var converter = new JsonConverter<Dictionary<string, BriefBoardEntity[]>>();
            Dictionary<string, BriefBoardEntity[]> dict = converter.Convert(response);
            return dict.Select(kv => new BoardsCategoryEntity {
                Name = kv.Key,
                Boards = kv.Value
            }).ToList();
        }
        
    }
}