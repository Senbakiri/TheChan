using Core.Models;

namespace TheChan.ViewModels {
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