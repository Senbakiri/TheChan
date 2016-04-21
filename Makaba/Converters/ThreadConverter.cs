using System.Collections.Generic;
using System.Linq;
using Core.Common.Links;
using Core.Converters;
using Core.Models;
using Makaba.Entities;

namespace Makaba.Converters {
    public class ThreadConverter : IThreadConverter {
        public ThreadConverter(PostConverter postConverter) {
            PostConverter = postConverter;
        }

        private PostConverter PostConverter { get; }

        public ThreadLink Link { get; set; }

        public Thread Convert(ThreadEntity source) {
            PostConverter.BoardId = Link.BoardId;
            List<Post> posts = source.Posts.Select(p => PostConverter.Convert(p)).ToList();
            return new Thread(posts, Link.BoardId, Link.ThreadNumber);
        }
    }
}