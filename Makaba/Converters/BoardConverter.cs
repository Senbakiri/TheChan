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
            return new BoardThread(
                entity.ThreadNum,
                entity.Posts.Take(1).Select(p => new Post(
                    p.Num,
                    p.Parent,
                    p.Subject,
                    p.Name,
                    p.Trip,
                    p.Email,
                    p.Comment,
                    p.IsOp,
                    p.IsBanned,
                    p.IsClosed,
                    p.Sticky != 0,
                    p.Files.Select(f => new Attachment()).ToList(),
                    DateUtils.TimestampToDateTime(p.Timestamp))).ToList(),
                skippedPosts,
                skippedPostsWithFiles);
        }
    }
}