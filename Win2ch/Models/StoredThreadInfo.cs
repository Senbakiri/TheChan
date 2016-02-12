using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Win2ch.Annotations;

namespace Win2ch.Models {
    public sealed class StoredThreadInfo : Thread, INotifyPropertyChanged {
        [JsonIgnore]
        private Post _FirstPost;

        private int _LastPostPosition;
        private int _UnreadPosts;

        public int UnreadPosts {
            get { return _UnreadPosts; }
            set {
                if (value == _UnreadPosts)
                    return;
                _UnreadPosts = value;
                RaisePropertyChanged();
            }
        }

        public int LastPostPosition {
            get { return _LastPostPosition; }
            set {
                if (value == _LastPostPosition)
                    return;
                _LastPostPosition = value;
                RaisePropertyChanged();
            }
        }

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
        private StoredThreadInfo() : base(0, "") { }

        public StoredThreadInfo(long num, string boardId) : base(num, boardId) {}

        public StoredThreadInfo(Thread thread) : base(thread.Num, thread.Board.Id) {
            Posts = thread.Posts;
            FilesCount = thread.FilesCount;
            TotalPosts = thread.TotalPosts;
            LastPostPosition = thread.Posts.Count;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}