using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Caliburn.Micro;
using Win2ch.Common;
using Win2ch.ViewModels;

namespace Win2ch
{
    sealed partial class App {

        private WinRTContainer container;

        public App()
        {
            InitializeComponent();
        }

        protected override void Configure() {
            container = new WinRTContainer();
            container.RegisterWinRTServices();
            container
                .PerRequest<HomeViewModel>()
                .Singleton<IShell, ShellViewModel>()
                .Singleton<IEventAggregator, EventAggregator>();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args) {
            DisplayRootViewFor<IShell>();
        }

        protected override object GetInstance(Type service, string key) {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service) {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance) {
            container.BuildUp(instance);
        }
    }
}
