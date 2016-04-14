using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Models;
using Win2ch.Common;

namespace Win2ch.ViewModels {
    public class BoardThreadViewModel {
        public BoardThreadViewModel(BoardThread thread) {
            PostViewModel = new PostViewModel {
                Post = thread.Post,
                ShowReplies = false,
                ShowPostPosition = false,
            };

            ThreadInfo = thread;
        }

        public PostViewModel PostViewModel { get; }

        public BoardThread ThreadInfo { get; }
    }
}