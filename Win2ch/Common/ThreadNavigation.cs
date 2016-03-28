namespace Win2ch.Common {
    public class ThreadNavigation {
        private ThreadNavigation() { }

        public static ThreadNavigation NavigateToThread(string boardId, long threadNum) {
            return new ThreadNavigation {
                BoardId = boardId,
                ThreadNumber = threadNum
            };
        }

        public ThreadNavigation ScrollToPost(long postNumber) {
            IsScrollingToPostNeeded = true;
            PostNumber = postNumber;
            return this;
        }

        public string BoardId { get; private set; }

        public long ThreadNumber { get; private set; }

        public bool IsScrollingToPostNeeded { get; private set; }

        public long PostNumber { get; private set; }
    }
}