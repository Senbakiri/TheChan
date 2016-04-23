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
                          int lastReadPostPosition = 0,
                          int unreadPosts = 0) {
            BoardId = boardId;
            Number = number;
            Description = description;
            ThumbnailUri = thumbnailUri;
            LastReadPostPosition = lastReadPostPosition;
            UnreadPosts = unreadPosts;
        }

        public string BoardId { get; }
        public long Number { get; }
        public string Description { get; }
        public Uri ThumbnailUri { get; }
        public int LastReadPostPosition { get; set; }
        public int UnreadPosts { get; set; }

        protected bool Equals(ThreadInfo other) {
            return
                   string.Equals(BoardId, other.BoardId)
                && Number == other.Number
                && string.Equals(Description, other.Description)
                && Equals(ThumbnailUri, other.ThumbnailUri)
                && LastReadPostPosition == other.LastReadPostPosition
                && UnreadPosts == other.UnreadPosts;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ThreadInfo)obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = BoardId?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Number.GetHashCode();
                hashCode = (hashCode * 397) ^ (Description?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (ThumbnailUri?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}