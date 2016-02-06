using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Win2ch.Models {
    public sealed class FavoriteThread : Thread {
        [JsonIgnore]
        private Post _FirstPost;

        public int UnreadPosts { get; set; }

        public int LastPostPosition { get; set; }
        
        [JsonProperty]
        public new string Name { get; private set; }
            

        [JsonIgnore]
        public override List<Post> Posts {
            get { return new List<Post> {FirstPost}; }
            set { FirstPost = value?.FirstOrDefault(); }
        }

        [JsonProperty]
        public string ThumbnailUrl { get; private set; }

        [JsonIgnore]
        public Post FirstPost {
            get { return _FirstPost; }
            set {
                _FirstPost = value;
                if (value == null)
                    return;

                var clear = Utils.RemoveHtml(_FirstPost.Comment);
                if (clear.Length > 50)
                    clear = clear.Substring(0, 50);
                ThumbnailUrl = _FirstPost.Images?.FirstOrDefault()?.ThumbnailUrl;

                Name = string.IsNullOrEmpty(_FirstPost.Subject)
                        ? clear
                        : _FirstPost.Subject;
            }
        }

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