﻿using System.Collections.Generic;
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
            Bind<IConverter<BoardsCategoryEntity, BoardsCategory>>().To<CategoriesConverter>();
            Bind<IConverter<IList<BoardsCategoryEntity>, IList<BoardsCategory>>>().To<CategoriesConverter>();
            Bind<IConverter<BoardPageEntity, BoardPage>>().To<BoardConverter>();
            Bind<IHttpOperation<IList<BoardsCategory>>>().To<RecieveBoardsOperation>();
            Bind<ILoadBoardOperation>().To<LoadBoardOperation>();
        }
    }
}