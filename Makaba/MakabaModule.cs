using Makaba.Operations;
using Makaba.Services.Url;
using Ninject.Modules;

namespace Makaba {
    public class MakabaModule : NinjectModule {
        public override void Load() {
            Bind<IUrlService>().To<UrlService>();
            Bind<GetBoardsOperation>().ToSelf();
        }
    }
}