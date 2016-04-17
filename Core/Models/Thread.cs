using System.Collections.Generic;
using System.Linq;
using Core.Utils;

namespace Core.Models {
    public class Thread {
        public Thread(IList<Post> posts, string boardId, long number) {
            Posts = posts;
            BoardId = boardId;
            Number = number;
        }

        public string BoardId { get; }
        public long Number { get; }
        public IList<Post> Posts { get; }

        public ThreadInfo GetThreadInfo() {
            Post firstPost = Posts.FirstOrDefault();
            if (firstPost == null)
                return null;

            string
                cleanSubject = Html.RemoveHtml(firstPost.Subject),
                cleanText = Html.RemoveHtml(firstPost.Text);

            string description = string.IsNullOrWhiteSpace(cleanSubject) ? cleanText : cleanSubject;

            return new ThreadInfo(BoardId,
                                  Number,
                                  description,
                                  firstPost.Attachments.FirstOrDefault()?.ThumbnailUri,
                                  Posts.Count);
        }
    }
}