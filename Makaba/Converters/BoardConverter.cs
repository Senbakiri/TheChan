using System.Collections.Generic;
using System.Linq;
using Core.Converters;
using Core.Models;
using Makaba.Entities;

namespace Makaba.Converters {
    public class BoardConverter : IConverter<BoardPageEntity, BoardPage> {
        public BoardConverter(PostConverter postConverter) {
            PostConverter = postConverter;
        }
        
        private PostConverter PostConverter { get; }

        public BoardPage Convert(BoardPageEntity source) {
            PostConverter.BoardId = source.BoardId;
            return new BoardPage(
                source.BoardId,
                source.BoardName,
                source.BoardInfo,
                source.CurrentPage,
                source.Pages,
                source.Threads.Select(CreateThread).ToList());
        }

        private BoardThread CreateThread(BoardThreadEntity thread) {
            IList<PostEntity> additionalPosts = thread.Posts.Skip(1).ToList();
            int skippedPosts = thread.PostsCount + additionalPosts.Count;
            int skippedPostsWithFiles = thread.FilesCount + additionalPosts.Count(e => e.Files.Count > 0);
            PostEntity post = thread.Posts.First();
            return new BoardThread(
                thread.ThreadNum,
                PostConverter.Convert(post),
                skippedPosts,
                skippedPostsWithFiles);
        }
    }
}