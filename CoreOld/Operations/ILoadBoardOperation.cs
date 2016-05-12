using Core.Models;

namespace Core.Operations {
    public interface ILoadBoardOperation : IHttpOperation<BoardPage> {
        int Page { get; set; }
        string Id { get; set; }
    }
}