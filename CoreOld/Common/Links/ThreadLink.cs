namespace Core.Common.Links {
    public class ThreadLink : BoardLink {
        public ThreadLink(string boardId, long threadNumber) : base(boardId) {
            ThreadNumber = threadNumber;
        }

        public long ThreadNumber { get; }
        
    }
}