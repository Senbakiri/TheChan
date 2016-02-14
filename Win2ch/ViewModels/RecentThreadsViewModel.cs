using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Template10.Controls;
using Win2ch.Models;
using Win2ch.Models.Exceptions;
using Win2ch.Mvvm;
using Win2ch.Services;
using Win2ch.Views;

namespace Win2ch.ViewModels {
    public class RecentThreadsViewModel : ViewModelBase {
        private bool _IsLoading;

        public ObservableItemCollection<StoredThreadInfo> RecentThreads { get; }
            = new ObservableItemCollection<StoredThreadInfo>();

        private RecentThreadsService RecentThreadsService { get; } = RecentThreadsService.Instance;

        public bool IsLoading {
            get { return _IsLoading; }
            private set {
                if (_IsLoading == value)
                    return;
                _IsLoading = value;
                RaisePropertyChanged();
            }
        }

        public async Task Load() {
            if (IsLoading)
                return;
            IsLoading = true;

            try {
                var threads = (await RecentThreadsService.GetItems()).Reverse();
                RecentThreads.Clear();
                foreach (var thread in threads) {
                    RecentThreads.Add(thread);
                }
            } catch (Exception e) {
                await Utils.ShowOtherError(e, "Не удалось загрузить недавние треды");
            }

            await Update();
        }

        private async Task Update() {
            IsLoading = true;
            foreach (var thread in RecentThreads) {
                try {
                    await RecentThreadsService.UpdateThread(thread);
                } catch (COMException) {
                } catch (HttpException) {
                } catch (ApiException) {
                    await RecentThreadsService.RemoveThread(thread);
                } catch (Exception e) {
                    await Utils.ShowOtherError(e, "Не удалось загрузить недавние треды");
                }
            }

            await RecentThreadsService.Store();
            IsLoading = false;
        }

        public void GoToThread(StoredThreadInfo storedThreadInfo) {
            var thread = new Thread(storedThreadInfo.Num, storedThreadInfo.Board.Id);
            NavigationService.Navigate(typeof(ThreadPage), thread);
        }

        public async Task RemoveThreadFromRecent(StoredThreadInfo storedThreadInfo) {
            await RecentThreadsService.RemoveThread(storedThreadInfo);
            await Load();
        }

        public async Task Clear() {
            var clear = false;
            var askDialog = new MessageDialog("Вы действительно хотите очистить историю тредов?",
                "Подтверждение");

            askDialog.Commands.Add(new UICommand("Да", _ => clear = true));
            askDialog.Commands.Add(new UICommand("Нет", _ => clear = false));
            await askDialog.ShowAsync();

            if (!clear)
                return;

            await RecentThreadsService.ClearItems();
            await Load();
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state) {
            await Load();
            await base.OnNavigatedToAsync(parameter, mode, state);
        }
    }
}
