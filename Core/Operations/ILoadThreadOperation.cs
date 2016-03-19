using Core.Models;

namespace Core.Operations {
    public interface ILoadThreadOperation : IHttpOperation<Thread> {
        string BoardId { get; set; }
        long ThreadNumber { get; set; }
        int FromPosition { get; set; }
    }
}