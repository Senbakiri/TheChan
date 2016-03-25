namespace Makaba.Links {
    public class PostLink : ThreadLink {
        public PostLink(string boardId, long threadNumber, long postNumber) : base(boardId, threadNumber) {
            PostNumber = postNumber;
        }

        public long PostNumber { get; }

        public override string GetUrl() {
            return base.GetUrl() + $"#{PostNumber}";
        }
    }
}