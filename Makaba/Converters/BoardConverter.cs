using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Core.Converters;
using Core.Models;
using Makaba.Entities;

namespace Makaba.Converters {
    public class BoardConverter : IConverter<BoardPageEntity, BoardPage> {
        public BoardPage Convert(BoardPageEntity source) {
            return new BoardPage(
                source.BoardId,
                source.BoardName,
                source.BoardInfo,
                source.CurrentPage,
                source.Pages,
                source.Threads.Select(CreateThread).ToList());
        }

        private BoardThread CreateThread(BoardThreadEntity entity) {
            IList<BoardPostEntity> additionalPosts = entity.Posts.Skip(1).ToList();
            int skippedPosts = entity.PostsCount + additionalPosts.Count;
            int skippedPostsWithFiles = entity.FilesCount + additionalPosts.Count(e => e.Files.Count > 0);
            BoardPostEntity post = entity.Posts.First();
            return new BoardThread(
                entity.ThreadNum,
                 new Post(
                    post.Num,
                    post.Parent,
                    post.Subject,
                    post.Name,
                    post.Trip,
                    post.Email,
                    post.Comment,
                    post.IsOp,
                    post.IsBanned,
                    post.IsClosed,
                    post.Sticky != 0,
                    post.Files.Select(f => new Attachment()).ToList(),
                    DateUtils.TimestampToDateTime(post.Timestamp)),
                skippedPosts,
                skippedPostsWithFiles);
        }
    }
}