using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Win2ch.Models;
using Win2ch.Models.Api;
using Win2ch.Models.Exceptions;
using Win2ch.Services;
using Win2ch.Views;

namespace Win2ch.ViewModels {
    public class MainPageViewModel : Mvvm.ViewModelBase {

        private StorageService<Board> FavoriteBoards { get; } = FavoritesService.Instance.Boards;

        public MainPageViewModel() {
            Categories = new ObservableCollection<Category>();
        }

        public ObservableCollection<Category> Categories { get; set; }

        private List<Category> HiddenCategories { get; } = new List<Category>();

        public async void LoadBoards() {
            var categories = new List<Category>();
            try {
                categories = await new ApiBoardsProvider().GetCategories();
            } catch (HttpException e) {
                var dialog = new MessageDialog("Код ошибки: " + (int)e.Code, "Не удалось получить список досок");
                dialog.Commands.Add(new UICommand("Попробовать снова", _ => LoadBoards()));
                dialog.Commands.Add(new UICommand("Закрыть приложение", _ => Application.Current.Exit()));
                await dialog.ShowAsync();
            } catch (COMException e) {
                var dialog = new MessageDialog("Код ошибки: 0x" + e.HResult.ToString("X"), "Не удалось получить список досок");
                dialog.Commands.Add(new UICommand("Попробовать снова", _ => LoadBoards()));
                dialog.Commands.Add(new UICommand("Закрыть приложение", _ => Application.Current.Exit()));
                await dialog.ShowAsync();
            }
            var favorites = await FavoriteBoards.GetItems();
            if (favorites.Count > 0) {
                Categories.Add(new Category(favorites) { Name = "Избранное" });
            }

            foreach (var category in categories) {
                // censorship
                if (category.Name != "Взрослым")
                    Categories.Add(category);
                else
                    HiddenCategories.Add(category);
            }
        }

        public void NavigateToBoard(Board board) {
            NavigationService.Navigate(typeof(BoardPage), board);
        }
    }
}

