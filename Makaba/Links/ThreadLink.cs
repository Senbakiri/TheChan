using Core.Common;

namespace Makaba.Links {
    public class ThreadLink : BoardLink {
        public ThreadLink(string boardId, long threadNumber) : base(boardId) {
            ThreadNumber = threadNumber;
        }

        public long ThreadNumber { get; }

        public override string GetUrl() {
            return base.GetUrl() + $"res/{ThreadNumber}.html";
        }
    }
}