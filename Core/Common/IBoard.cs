namespace Core.Common {
    public interface IBoard {
        IBoardOperations Operations { get; }
        IUrlService UrlService { get; }
    }
}