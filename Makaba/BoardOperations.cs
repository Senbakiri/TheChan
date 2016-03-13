using System.Collections.Generic;
using Core.Common;
using Core.Models;
using Core.Operations;

namespace Makaba {
    public class BoardOperations : IBoardOperations {
        public BoardOperations(IHttpOperation<IList<BoardsCategory>> boardsReceiving) {
            BoardsReceiving = boardsReceiving;
        }

        public IHttpOperation<IList<BoardsCategory>> BoardsReceiving { get; }
    }
}