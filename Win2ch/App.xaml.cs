﻿using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Caliburn.Micro;
using Win2ch.ViewModels;

namespace Win2ch
{
    sealed partial class App {

        private WinRTContainer _container;

        public App()
        {
            InitializeComponent();
        }

        protected override void Configure() {
            _container = new WinRTContainer();
            _container.RegisterWinRTServices();
            _container
                .Singleton<ShellViewModel>()
                .PerRequest<HomeViewModel>();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args) {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key) {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service) {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance) {
            _container.BuildUp(instance);
        }
    }
}
