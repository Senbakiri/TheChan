namespace Win2ch.Common {
    public class ThreadNavigation {
        private ThreadNavigation() { }

        public static ThreadNavigation NavigateToThread(string boardId, long threadNum) {
            return new ThreadNavigation {
                BoardId = boardId,
                ThreadNumber = threadNum
            };
        }

        public string BoardId { get; private set; }

        public long ThreadNumber { get; private set; }
    }
}