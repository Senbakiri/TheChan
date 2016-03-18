using Core.Common;

namespace Makaba {
    public class MakabaBoard : IBoard {
        public MakabaBoard(IBoardOperations operations, IUrlService urlService) {
            Operations = operations;
            UrlService = urlService;
        }

        public IBoardOperations Operations { get; }
        public IUrlService UrlService { get; }
    }
}