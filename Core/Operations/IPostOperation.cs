using Core.Common;
using Core.Models;

namespace Core.Operations {
    public interface IPostOperation : IHttpOperation<PostingResult> {
        PostInfo PostInfo { get; set; }
        string BoardId { get; set; }
        long Parent { get; set; }
    }
}