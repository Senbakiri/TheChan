using Core.Converters;
using Core.Models;
using Makaba.Entities;

namespace Makaba.Converters {
    public class PostingResultConverter : IConverter<PostingResultEntity, PostingResult> {
        public PostingResult Convert(PostingResultEntity source) {
            return new PostingResult(string.IsNullOrWhiteSpace(source.Error), source.Num, source.Reason);
        }
    }
}