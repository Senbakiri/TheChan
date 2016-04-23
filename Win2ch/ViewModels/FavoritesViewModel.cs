using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Core.Common;
using Win2ch.Extensions;
using Win2ch.Services.Storage;

namespace Win2ch.ViewModels {
    public class FavoritesViewModel : PropertyChangedBase {
        public FavoritesViewModel(FavoriteThreadsService favoriteThreads, IBoard board) {
            FavoriteThreads = favoriteThreads;
            Board = board;
            Threads = new ObservableCollection<ThreadInfoViewModel>();
        }

        private IBoard Board { get; }
        private FavoriteThreadsService FavoriteThreads { get; }

        public ObservableCollection<ThreadInfoViewModel> Threads { get; }

        public void Load() {
            Threads.Clear();
            Threads.AddRange(FavoriteThreads.Items.Select(t => new ThreadInfoViewModel(t)));
        }
    }
}