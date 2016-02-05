using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Win2ch.Models {
    public sealed class FavoriteThread : Thread {

        public int UnreadPosts { get; set; }

        public int LastPostPosition { get; set; }

        public override string Name =>
            string.IsNullOrEmpty(FirstPost?.Subject)
                ? FirstPost?.Comment
                : FirstPost?.Subject;

        [JsonIgnore]
        public override List<Post> Posts {
            get { return new List<Post> {FirstPost}; }
            set { FirstPost = value?.FirstOrDefault(); }
        }

        [JsonProperty]
        public Post FirstPost { get; set; }
        
        [JsonConstructor]
        private FavoriteThread() : base(0, "") { }

        public FavoriteThread(long num, string boardId) : base(num, boardId) {}

        public FavoriteThread(Thread thread) : base(thread.Num, thread.Board.Id) {
            Posts = thread.Posts;
            FilesCount = thread.FilesCount;
            TotalPosts = thread.TotalPosts;
            LastPostPosition = thread.Posts.Count;
        }
    }
}