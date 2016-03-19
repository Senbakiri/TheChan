using System.Linq;
using Core.Converters;
using Core.Models;
using Makaba.Entities;

namespace Makaba.Converters {
    public class ThreadConverter : IConverter<ThreadEntity, Thread> {
        public ThreadConverter(PostConverter postConverter) {
            PostConverter = postConverter;
        }

        private PostConverter PostConverter { get; }

        public Thread Convert(ThreadEntity source) {
            PostConverter.BoardId = source.BoardId;
            return new Thread(source.Posts.Select(p => PostConverter.Convert(p)).ToList());
        }
    }
}