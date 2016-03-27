using Core.Models;

namespace Core.Operations {
    public interface IGetPostOperation : IHttpOperation<Post> {
        string BoardId { get; set; }
        long PostNumber { get; set; }
    }
}