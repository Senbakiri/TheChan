using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Notifications;
using Caliburn.Micro;
using Core.Common;
using Core.Models;
using Ninject;
using Win2ch.Common;
using Win2ch.ViewModels;
using Makaba;
using Template10.Services.SerializationService;
using Win2ch.Common.UI;
using Win2ch.Services.Settings;
using Win2ch.Services.Storage;
using Win2ch.Services.Toast;

namespace Win2ch
{
    sealed partial class App {

        private IKernel kernel;

        public App()
        {
            InitializeComponent();
        }

        protected override void Configure() {
        }

        private  async Task LoadServices() {
            this.kernel = new StandardKernel(new MakabaModule());
            IKernel k = this.kernel;
            k.Bind<IShell>().To<ShellViewModel>().InSingletonScope();
            k.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            k.Bind<IBoard>().To<MakabaBoard>().InSingletonScope();
            k.Bind<IToastService>().To<ToastService>().InSingletonScope();
            k.Bind<IAttachmentViewer>().To<AttachmentViewer>();
            k.Bind<ISerializationService>().ToConstant(SerializationService.Json);
            k.Bind<IStorageService>().To<SerializationStorageService>();
            k.Bind<ISettingsService>().To<SettingsService>().InSingletonScope();

            k.Bind<FavoriteThreadsService>().ToSelf().InSingletonScope();
            k.Bind<RecentThreadsService>().ToSelf().InSingletonScope();
            k.Bind<FavoritePostsService>().ToSelf().InSingletonScope();
            k.Bind<FavoriteBoardsService>().ToSelf().InSingletonScope();

            await k.Get<FavoriteThreadsService>().Load();
            await k.Get<RecentThreadsService>().Load();
            await k.Get<FavoritePostsService>().Load();
            await k.Get<FavoriteBoardsService>().Load();
        }


        protected override async void OnLaunched(LaunchActivatedEventArgs args) {
            await LoadServices();
            DisplayRootViewFor<IShell>();
        }

        protected override object GetInstance(Type service, string key) {
            return this.kernel.Get(service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service) {
            return this.kernel.GetAll(service);
        }

        protected override void BuildUp(object instance) {
            this.kernel.Inject(instance);
        }
    }
}
