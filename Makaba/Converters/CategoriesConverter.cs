using System;
using System.Collections.Generic;
using System.Linq;
using Core.Converters;
using Core.Models;
using Makaba.Entities;
using Makaba.Services.Url;

namespace Makaba.Converters {
    public class CategoriesConverter : IConverter<IList<BoardCategoryEntity>, IList<BoardsCategory>>,
                                       IConverter<BoardCategoryEntity, BoardsCategory> {
        private IUrlService UrlService { get; }

        public CategoriesConverter(IUrlService urlService) {
            UrlService = urlService;
        }

        BoardsCategory IConverter<BoardCategoryEntity, BoardsCategory>.Convert(BoardCategoryEntity source) {
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

        public IList<BoardsCategory> Convert(IList<BoardCategoryEntity> source) {
            var convert = new Func<BoardCategoryEntity, BoardsCategory>((this as IConverter<BoardCategoryEntity, BoardsCategory>).Convert);
            return source.Select(c => convert(c)).ToList();
        }
    }
}