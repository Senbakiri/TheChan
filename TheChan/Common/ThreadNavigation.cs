namespace TheChan.Common {
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

        public ThreadNavigation ScrollToPostByPosition(int position) {
            IsScrollingToPostNeeded = true;
            PostPosition = position;
            return this;
        }

        public ThreadNavigation WithHighlighting(int highlightingStart) {
            IsHighlightingNeeded = true;
            HighlightingStart = highlightingStart;
            return this;
        }

        public string BoardId { get; private set; }

        public long ThreadNumber { get; private set; }

        public bool IsScrollingToPostNeeded { get; private set; }

        public long PostNumber { get; private set; }

        public int PostPosition { get; private set; }

        public bool IsHighlightingNeeded { get; private set; }

        public int HighlightingStart { get; private set; }
    }
}