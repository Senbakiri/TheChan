using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Caliburn.Micro;
using Win2ch.Common;
using Win2ch.Services.Toast;
using WinRTXamlToolkit.Tools;

namespace Win2ch.ViewModels {
    internal sealed class ShellViewModel : Conductor<Tab>.Collection.OneActive, IShell {
        private bool isUpdated = true;
        private readonly CoreDispatcher dispatcher;

        public ShellViewModel(IToastService toastService) {
            ToastService = toastService;
            this.threadsUpdateTimer.Interval = TimeSpan.FromMinutes(1);
            this.threadsUpdateTimer.Tick += ThreadsUpdateTimerOnTick;
            this.threadsUpdateTimer.Start();
            this.dispatcher = Window.Current.Dispatcher;
        }

        public LoadingInfo LoadingInfo { get; } = new LoadingInfo();
        private readonly BackgroundTimer threadsUpdateTimer = new BackgroundTimer();
        private object popupContent;
        private IToastService ToastService { get; }

        public object PopupContent {
            get { return this.popupContent; }
            private set {
                if (Equals(value, this.popupContent))
                    return;
                this.popupContent = value;
                NotifyOfPropertyChange();
            }
        }

        protected override void OnInitialize() {
            Navigate<HomeViewModel>();
        }

        public void CloseTab(Tab tab) {
            DeactivateItem(tab, true);
        }

        public void Navigate<T>(object parameter = null) where T : Tab {
            var item = IoC.Get<T>();
            ActivateItem(item, parameter);
        }

        public void ShowPopup(object content) {
            PopupContent = content;
        }

        public void HidePopup() {
            PopupContent = null;
        }

        public override void ActivateItem(Tab item) {
           ActivateItem(item, null);
        }
        
        private void ActivateItem(Tab item, object parameter) {
            HidePopup();
            if (ActiveItem != item && item != null) {
                (item as IActivateWithParameter).Activate(parameter);
                OnActivationProcessed(item, true);
            }

            ChangeActiveItem(item, false);
        }

        private async void ThreadsUpdateTimerOnTick(object sender, object o) {
            if (!this.isUpdated)
                return;

            this.isUpdated = false;
            var threads = Items.OfType<ThreadViewModel>().ToList();
            if (threads.Count == 0)
                return;

            var i = 0;
            var threadsWithNewPosts = 0;
            string text = Tab.GetLocalizationStringForView("Shell", "UpdatingThread");
            foreach (ThreadViewModel thread in threads) {
                LoadingInfo.InProgress($"{text} {++i}/{threads.Count}");
                try {
                    bool hasNewPosts = await thread.Update();
                    if (hasNewPosts)
                        threadsWithNewPosts++;
                } catch {
                    // OK
                }
            }

            string title = Tab.GetLocalizationStringForView("Shell", "ThreadsWithNewPosts");
            LoadingInfo.Success($"{title}: {threadsWithNewPosts}");
            this.isUpdated = true;
            await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                if ((Window.Current == null || !Window.Current.Visible) && threadsWithNewPosts != 0) {
                    ToastService.ShowSimpleToast($"{title}: {threadsWithNewPosts}");
                }
            });

        }
    }
}
