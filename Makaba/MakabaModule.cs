using System.Collections.Generic;
using Core.Common;
using Core.Converters;
using Core.Models;
using Core.Operations;
using Makaba.Converters;
using Makaba.Entities;
using Makaba.Operations;
using Makaba.Services.Url;
using Ninject.Modules;

namespace Makaba {
    public class MakabaModule : NinjectModule {
        public override void Load() {
            Bind<IUrlService>().To<UrlService>();
            Bind<IBoardOperations>().To<BoardOperations>();

            Bind<IConverter<BoardsCategoryEntity, BoardsCategory>>().To<CategoriesConverter>();
            Bind<IConverter<IList<BoardsCategoryEntity>, IList<BoardsCategory>>>().To<CategoriesConverter>();
            Bind<IConverter<BoardPageEntity, BoardPage>>().To<BoardConverter>();
            Bind<IConverter<ThreadEntity, Thread>>().To<ThreadConverter>();
            Bind<IThreadConverter>().To<ThreadConverter>();
            Bind<IConverter<PostingResultEntity, PostingResult>>().To<PostingResultConverter>();

            Bind<IHttpOperation<IList<BoardsCategory>>>().To<RecieveBoardsOperation>();
            Bind<ILoadBoardOperation>().To<LoadBoardOperation>();
            Bind<ILoadThreadOperation>().To<LoadThreadOperation>();
            Bind<IGetPostOperation>().To<GetPostOperation>();
            Bind<IPostOperation>().To<PostOperation>();
        }
    }
}