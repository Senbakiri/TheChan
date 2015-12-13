﻿using System.Collections.ObjectModel;
using Win2ch.Models;
using Windows.System.Profile;
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
                LoadThreads();
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Thread> Threads
        { get; } = new ObservableCollection<Thread>();

        private async void LoadThreads()
        {
            if (Board == null)
                return;

            var threads = await Board.GetThreads(0);
            Threads.Clear();

            var isMobile = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobsdaile";
            foreach (var thread in threads)
            {
                Threads.Add(thread);
                if (isMobile && thread.Posts.Count > 0)
                    thread.Posts.RemoveRange(1, thread.Posts.Count - 1);
            }
        }

        public void NavigateToThread(Thread thread)
        {
            NavigationService.Navigate(typeof (ThreadPage), thread);
        }
    }
}
