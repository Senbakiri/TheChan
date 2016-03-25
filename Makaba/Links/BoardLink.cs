using Core.Common;

namespace Makaba.Links {
    public class BoardLink : LinkBase {
        public BoardLink(string boardId) {
            BoardId = boardId;
        }

        public string BoardId { get; }

        public override string GetUrl() {
            return $"https://2ch.hk/{BoardId}/";
        }
    }
}