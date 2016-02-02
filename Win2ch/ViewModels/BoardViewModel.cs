using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Win2ch.Models;
using Windows.System.Profile;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Template10.Mvvm;
using Win2ch.Models.Exceptions;
using Win2ch.Views;

namespace Win2ch.ViewModels {
    public class BoardViewModel : Mvvm.ViewModelBase {
        private Board _Board;
        private ThreadsCollection _threads;

        public Board Board {
            get { return _Board; }
            set {
                _Board = value;
                RaisePropertyChanged();
                if (Threads == null) {
                    Threads = new ThreadsCollection(Board);
                    Threads.BoardLoadError += OnBoardLoadError;
                    Threads.CollectionChanged += Threads_OnCollectionChanged;
                } else
                    Threads.Board = Board;
            }
        }

        public ThreadsCollection Threads {
            get { return _threads; }
            private set {
                _threads = value;
                RaisePropertyChanged();
            }
        }
        
        public ICommand NewThreadCommand { get; }

        public BoardViewModel() {
            NewThreadCommand = new DelegateCommand(NewThread);
        }

        private void NewThread() {
            NavigationService.Navigate(typeof(PostingPage), new PostingPageNavigationInfo() {
                PostInfo = new NewPostInfo(),
                Thread = new Thread(0, Board.Id)
            });
        }

        private void Threads_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            RaisePropertyChanged(() => Board);
        }

        public void NavigateToThread(Thread thread) {
            NavigationService.Navigate(typeof(ThreadPage), thread);
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state) {
            // if user goes to the board from board list, load threads,
            // because they can be restored from navigation cache
            if (mode == NavigationMode.New)
                Threads?.Refresh();

            if (mode == NavigationMode.New || mode == NavigationMode.Forward)
                Board = (Board)parameter;

            return Task.CompletedTask;
        }

        public void Refresh() {
            Threads?.Refresh();
        }

        private void OnBoardLoadError(HttpException exception) {
            NavigationService.Navigate(typeof (Views.Errors.BoardErrorPage), exception,
                new SuppressNavigationTransitionInfo());
        }
    }
}
