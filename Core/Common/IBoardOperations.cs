using System.Collections.Generic;
using Core.Models;
using Core.Operations;

namespace Core.Common {
    public interface IBoardOperations {
        IHttpOperation<IList<BoardsCategory>> BoardsReceiving { get; }
    }
}