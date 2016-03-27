using System.Collections.Generic;
using Core.Common;
using Core.Models;
using Core.Operations;
using Ninject;

namespace Makaba.Operations {
    public class BoardOperations : IBoardOperations {

        public BoardOperations() {
            Kernel = new StandardKernel(new MakabaModule());
        }

        private IKernel Kernel { get; }

        public IHttpOperation<IList<BoardsCategory>> LoadBoards() {
            return Kernel.Get<IHttpOperation<IList<BoardsCategory>>>();
        }

        public ILoadBoardOperation LoadBoard() {
            return Kernel.Get<ILoadBoardOperation>();
        }

        public ILoadThreadOperation LoadThread() {
            return Kernel.Get<ILoadThreadOperation>();
        }

        public IGetPostOperation GetPost() {
            return Kernel.Get<IGetPostOperation>();
        }
    }
}