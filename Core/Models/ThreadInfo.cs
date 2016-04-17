using System;

namespace Core.Models {

    /// <summary>
    /// Used for favorites and recent threads
    /// </summary>
    public class ThreadInfo {
        public ThreadInfo(string boardId,
                          long number,
                          string description,
                          Uri thumbnailUri,
                          int lastReadedPostPosition = 0,
                          int unreadPosts = 0) {
            BoardId = boardId;
            Number = number;
            Description = description;
            ThumbnailUri = thumbnailUri;
            LastReadedPostPosition = lastReadedPostPosition;
            UnreadPosts = unreadPosts;
        }

        public string BoardId { get; }
        public long Number { get; }
        public string Description { get; }
        public Uri ThumbnailUri { get; }
        public int LastReadedPostPosition { get; set; }
        public int UnreadPosts { get; set; }
    }
}