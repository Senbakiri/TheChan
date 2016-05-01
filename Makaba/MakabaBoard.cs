using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Common;
using Core.Common.Links;
using Core.Models;
using Core.Operations;

namespace Makaba {
    public class MakabaBoard : IBoard {
        public MakabaBoard(IBoardOperations operations, IUrlService urlService) {
            Operations = operations;
            UrlService = urlService;
        }

        private IBoardOperations Operations { get; }
        public IUrlService UrlService { get; }
        public Task<Thread> LoadThreadAsync(ThreadLink link, int startPosition = 0) {
            ILoadThreadOperation operation = Operations.LoadThread();
            operation.Link = link;
            operation.StartPosition = startPosition;
            return operation.ExecuteAsync();
        }
        

        public Task<IList<BoardsCategory>> LoadBoardsAsync() {
            IHttpOperation<IList<BoardsCategory>> operation = Operations.LoadBoards();
            return operation.ExecuteAsync();
        }

        public Task<BoardPage> LoadBoardPageAsync(string boardId, int pageNumber) {
            ILoadBoardOperation operation = Operations.LoadBoard();
            operation.Id = boardId;
            operation.Page = pageNumber;
            return operation.ExecuteAsync();
        }

        public Task<Post> LoadPostAsync(string boardId, long number) {
            IGetPostOperation operation = Operations.GetPost();
            operation.BoardId = boardId;
            operation.PostNumber = number;
            return operation.ExecuteAsync();
        }

        public Task<PostingResult> PostAsync(PostInfo postInfo, string boardId, long parent = 0) {
            IPostOperation operation = Operations.Post();
            operation.PostInfo = postInfo;
            operation.BoardId = boardId;
            operation.Parent = parent;
            return operation.ExecuteAsync();
        }
    }
}