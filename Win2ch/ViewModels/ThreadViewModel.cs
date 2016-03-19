using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common;
using Core.Models;
using Core.Operations;
using Win2ch.Common;
using Win2ch.Extensions;

namespace Win2ch.ViewModels {
    public class ThreadViewModel : Tab {

        public ThreadViewModel(IBoard board, IShell shell) {
            Board = board;
            Shell = shell;
            Operation = Board.Operations.LoadThread();
            Posts = new ObservableCollection<PostViewModel>();
        }

        private IShell Shell { get; }
        private IBoard Board { get; }
        private ILoadThreadOperation Operation { get; }
        public ObservableCollection<PostViewModel> Posts { get; } 

        protected override async void OnActivate(object parameter = null) {
            if (parameter == null)
                return;
            var navigation = (ThreadNavigation) parameter;
            if (string.IsNullOrWhiteSpace(navigation.BoardId))
                throw new ArgumentException();

            Operation.BoardId = navigation.BoardId;
            Operation.ThreadNumber = navigation.ThreadNumber;
            DisplayName = $"/{navigation.BoardId}/ - {navigation.ThreadNumber}";

            IsLoading = true;
            Shell.LoadingInfo.InProgress(null);
            try {
                Thread thread = await Operation.ExecuteAsync();
                DisplayName = GetDisplayName(thread);
                Posts.Clear();
                FillPosts(thread.Posts);
                IsLoading = false;
                Shell.LoadingInfo.Success(null);
            } catch (Exception) {
                Shell.LoadingInfo.Error(null, true, () => OnActivate(navigation));
            }
            
        }

        private static string GetDisplayName(Thread thread) {
            Post first = thread.Posts.First();
            return string.IsNullOrWhiteSpace(first.Subject) ? first.Text : first.Subject;
        }

        private void FillPosts(IList<Post> posts) {
            int offset = Posts.Count + 1;
            for (var i = 0; i < posts.Count; i++) {
                Post post = posts[i];
                Posts.Add(new PostViewModel {
                    Foreground = PostForeground.Gray,
                    Position = offset + i,
                    Post = post
                });
            }
        }
    }
}