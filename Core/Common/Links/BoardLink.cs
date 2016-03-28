namespace Core.Common.Links {
    public class BoardLink : LinkBase {
        public BoardLink(string boardId) {
            BoardId = boardId;
        }

        public string BoardId { get; }
    }
}