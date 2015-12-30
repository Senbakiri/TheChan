using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Win2ch.Models;
using Windows.System.Profile;
using Windows.UI.Xaml.Navigation;
using Template10.Mvvm;
using Win2ch.Views;

namespace Win2ch.ViewModels
{
    public class BoardViewModel : Mvvm.ViewModelBase
    {
        private Board _Board;
        public Board Board
        {
            get { return _Board; }
            set
            {
                _Board = value;
                RaisePropertyChanged();
                if (Threads == null)
                {
                    Threads = new ThreadsCollection(Board);
                    Threads.CollectionChanged += Threads_OnCollectionChanged;
                }
                else
                    Threads.Board = Board;
            }
        }

        public ThreadsCollection Threads
        { get; private set; }

        public ICommand ShowImageCommand { get; }
        public ICommand NewThreadCommand { get; }

        public BoardViewModel()
        {
            ShowImageCommand = new DelegateCommand<ImageInfo>(ShowImage);
            NewThreadCommand = new DelegateCommand(NewThread);
        }

        private void NewThread()
        {
            NavigationService.Navigate(typeof (PostingPage), new PostingPageNavigationInfo()
            {
                PostInfo = new NewPostInfo(),
                Thread = new Thread {Board = Board}
            });
        }

        private void ShowImage(ImageInfo imageInfo)
        {
            NavigationService.Navigate(typeof(ImagesViewPage),
                new Tuple<ImageInfo, List<ImageInfo>>(imageInfo,
                    Threads.SelectMany(t => t.Posts.SelectMany(p => p.Images)).ToList()));
        }

        private void Threads_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => Board);
        }


        public void NavigateToThread(Thread thread)
        {
            NavigationService.Navigate(typeof (ThreadPage), thread);
        }

        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            // if user goes to the board from board list, load threads,
            // because they can be restored from navigation cache
            if (mode == NavigationMode.New)
                Threads?.Refresh();
            Board = (Board) parameter;
        }
    }
}
