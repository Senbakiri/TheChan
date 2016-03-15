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
            Bind<IConverter<BoardsCategoryEntity, BoardsCategory>>().To<CategoriesConverter>();
            Bind<IConverter<IList<BoardsCategoryEntity>, IList<BoardsCategory>>>().To<CategoriesConverter>();
            Bind<IHttpOperation<IList<BoardsCategory>>>().To<BoardsReceivingOperation>();
            Bind<IBoardOperations>().To<BoardOperations>();
        }
    }
}