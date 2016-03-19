using System.Collections.Generic;
using Core.Models;
using Core.Operations;

namespace Core.Common {
    public interface IBoardOperations {
        IHttpOperation<IList<BoardsCategory>> LoadBoards();
        ILoadBoardOperation LoadBoard();
        ILoadThreadOperation LoadThread();
    }
}