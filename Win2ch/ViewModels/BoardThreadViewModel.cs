using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Models;
using Win2ch.Common;

namespace Win2ch.ViewModels {
    public class BoardThreadViewModel {
        public BoardThreadViewModel(BoardThread thread) {
            Post = new PostViewModel {
                Post = thread.Post,
                ShowReplies = false,
                ShowPostPosition = false,
                Foreground = PostForeground.Contrast
            };

            ThreadInfo = thread;
        }

        public PostViewModel Post { get; }

        public BoardThread ThreadInfo { get; }
    }
}