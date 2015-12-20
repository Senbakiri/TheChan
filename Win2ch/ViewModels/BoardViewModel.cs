using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        private void Threads_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => Board);
        }

        public ThreadsCollection Threads
        { get; private set; }

        public void NavigateToThread(Thread thread)
        {
            NavigationService.Navigate(typeof (ThreadPage), thread);
        }

        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Board = (Board) parameter;
        }
    }
}
