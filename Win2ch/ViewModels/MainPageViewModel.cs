using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage;
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

        public ObservableCollection<Category> Categories { get; set; }

        private List<Category> HiddenCategories { get; } = new List<Category>();

        private StorageService<string> AllowedBoards { get; }

        public MainPageViewModel() {
            Categories = new ObservableCollection<Category>();
            AllowedBoards = new StorageService<string>(ApplicationData.Current.RoamingFolder,
                "AllowedBoards.json");
        }
        
        public async void LoadBoards() {
            Categories.Clear();

            var favorites = await FavoriteBoards.GetItems();
            if (favorites.Count > 0) {
                Categories.Add(new Category(favorites) { Name = "Избранное" });
            }

            var categories = new List<Category>();
            try {
                categories = await new ApiBoardsProvider().GetCategories();
            } catch (HttpException e) {
                var dialog = new MessageDialog("Код ошибки: " + e.Code, "Не удалось получить список досок");
                dialog.Commands.Add(new UICommand("Повторить", _ => LoadBoards()));
                await dialog.ShowAsync();
            } catch (COMException e) {
                var dialog = new MessageDialog("Код ошибки: 0x" + e.HResult.ToString("X"), "Не удалось получить список досок");
                dialog.Commands.Add(new UICommand("Повторить", _ => LoadBoards()));
                dialog.Commands.Add(new UICommand("Закрыть"));
                await dialog.ShowAsync();
            }

            foreach (var category in categories) {
                // censorship
                if (category.Name != "Взрослым")
                    Categories.Add(category);
                else {
                    HiddenCategories.Add(category);
                    var cat = await CreateCategoryWithAllowedBoards(category);
                    if (cat.Count > 0)
                        Categories.Add(cat);
                }
            }
        }

        private async Task<Category> CreateCategoryWithAllowedBoards(Category baseCategory) {
            List<string> boards;

            try {
                boards = (await AllowedBoards.GetItems()).ToList();
            } catch (Exception) {
                return baseCategory;
            }

            return new Category(baseCategory.Where(b => boards.Contains(b.Id))) {
                Name = baseCategory.Name
            };
        }

        public async void NavigateToBoard(Board board) {
            try {
                if (HiddenCategories.Any(c => c.Contains(board)))
                    await AllowedBoards.Add(board.Id);
            } catch {
                // it's okay
            }

            NavigationService.Navigate(typeof(BoardPage), board);
        }
    }
}

