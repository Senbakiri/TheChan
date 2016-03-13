using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Caliburn.Micro;
using Core.Common;
using Ninject;
using Win2ch.Common;
using Win2ch.ViewModels;

namespace Win2ch
{
    sealed partial class App {

        private IKernel kernel;

        public App()
        {
            InitializeComponent();
        }

        protected override void Configure() {
            kernel = new StandardKernel(new Makaba.MakabaModule());
            kernel.Bind<IShell>().To<ShellViewModel>().InSingletonScope();
            kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            kernel.Bind<IBoard>().To<Makaba.Board>();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args) {
            DisplayRootViewFor<IShell>();
        }

        protected override object GetInstance(Type service, string key) {
            return kernel.Get(service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service) {
            return kernel.GetAll(service);
        }

        protected override void BuildUp(object instance) {
            kernel.Inject(instance);
        }
    }
}
