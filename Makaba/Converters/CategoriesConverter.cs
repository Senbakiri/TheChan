using System;
using System.Collections.Generic;
using System.Linq;
using Core.Converters;
using Core.Models;
using Makaba.Entities;
using Makaba.Services.Url;

namespace Makaba.Converters {
    public class CategoriesConverter : IConverter<IList<BoardsCategoryEntity>, IList<BoardsCategory>>,
                                       IConverter<BoardsCategoryEntity, BoardsCategory> {
        private IUrlService UrlService { get; }

        public CategoriesConverter(IUrlService urlService) {
            UrlService = urlService;
        }

        public BoardsCategory Convert(BoardsCategoryEntity source) {
            IEnumerable<BriefBoardInfo> boards = source.Boards.Select(entity =>
                new BriefBoardInfo(
                    entity.BumpLimit,
                    entity.DefaultName,
                    entity.IsLikesEnabled,
                    entity.IsPostingEnabled,
                    entity.IsThreadTagsEnabled,
                    entity.IsSageEnabled,
                    entity.IsTripCodesEnabled,
                    entity.Icons.Select(icon => new Icon(icon.Name, icon.Number, UrlService.GetFullUrl(icon.Url))).ToList(),
                    entity.Id, entity.Name));

            return new BoardsCategory(source.Name, boards.ToList());
        }

        public IList<BoardsCategory> Convert(IList<BoardsCategoryEntity> source) {
            var convertFunc = new Func<BoardsCategoryEntity, BoardsCategory>((this as IConverter<BoardsCategoryEntity, BoardsCategory>).Convert);
            return source.Select(c => convertFunc(c)).ToList();
        }
    }
}