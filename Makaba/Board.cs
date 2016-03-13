using Core.Common;

namespace Makaba {
    public class Board : IBoard {
        public Board(IBoardOperations operations) {
            Operations = operations;
        }

        public IBoardOperations Operations { get; }
    }
}
