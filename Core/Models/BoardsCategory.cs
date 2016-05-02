using System.Collections.Generic;

namespace Core.Models {
    public class BoardsCategory {
        public BoardsCategory(string name, IList<BriefBoardInfo> boards) {
            Name = name;
            Boards = boards;
        }

        public string Name { get; }
        public IList<BriefBoardInfo> Boards { get; }
    }
}