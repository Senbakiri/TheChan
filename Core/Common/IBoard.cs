using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Common.Links;
using Core.Models;

namespace Core.Common {
    public interface IBoard {
        //IBoardOperations Operations { get; }
        IUrlService UrlService { get; }

        /// <summary>
        /// Loads the specified thread.
        /// </summary>
        Task<Thread> LoadThreadAsync(ThreadLink link, int startPosition = 0);
        

        /// <summary>
        /// Loads all the boards.
        /// </summary>
        Task<IList<BoardsCategory>> LoadBoardsAsync();

        /// <summary>
        /// Loads the specified board page.
        /// </summary>
        /// <param name="boardId">Board ID.</param>
        /// <param name="pageNumber">Page which to load. Begins from 0.</param>
        /// <returns></returns>
        Task<BoardPage> LoadBoardPageAsync(string boardId, int pageNumber);

        /// <summary>
        /// Loads the post with specified parameters.
        /// </summary>
        /// <param name="boardId">Board ID.</param>
        /// <param name="number">Post number.</param>
        /// <returns></returns>
        Task<Post> LoadPostAsync(string boardId, long number);
    }
}