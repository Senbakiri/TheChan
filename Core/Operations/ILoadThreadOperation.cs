using Core.Common.Links;
using Core.Models;

namespace Core.Operations {
    public interface ILoadThreadOperation : IHttpOperation<Thread> {
        ThreadLink Link { get; set; }
        int StartPosition { get; set; }
    }
}