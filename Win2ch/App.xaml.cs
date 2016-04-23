using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Caliburn.Micro;
using Core.Common;
using Core.Models;
using Ninject;
using Win2ch.Common;
using Win2ch.ViewModels;
using Makaba;
using Win2ch.Common.UI;
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
            this.kernel = new StandardKernel(new MakabaModule());
            this.kernel.Bind<IShell>().To<ShellViewModel>().InSingletonScope();
            this.kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            this.kernel.Bind<IBoard>().To<MakabaBoard>().InSingletonScope();
            this.kernel.Bind<IToastService>().To<ToastService>().InSingletonScope();
            this.kernel.Bind<IAttachmentViewer>().To<AttachmentViewer>();
            this.kernel.Bind<IStorageService<IList<ThreadInfo>>>().To<JsonStorageService<IList<ThreadInfo>>>();
            this.kernel.Bind<IStorageService<IList<Post>>>().To<JsonStorageService<IList<Post>>>();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args) {
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
