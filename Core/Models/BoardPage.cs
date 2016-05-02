using System.Collections.Generic;

namespace Core.Models {
    public class BoardPage {
        public BoardPage(string boardId,
                         string boardName,
                         string boardInfo,
                         int number,
                         IList<int> pages,
                         IList<BoardThread> threads) {
            BoardId = boardId;
            BoardName = boardName;
            BoardInfo = boardInfo;
            Number = number;
            Pages = pages;
            Threads = threads;
        }

        public string BoardId { get; }
        public string BoardName { get; }
        public string BoardInfo { get; }
        public int Number { get; }
        public IList<int> Pages { get; } 
        public IList<BoardThread> Threads { get; } 
    }
}
