using System.Collections.Generic;
using Windows.Storage.Streams;

namespace Core.Common {
    public sealed class PostInfo {
        public string Text { get; set; }
        public string EMail { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public bool IsOp { get; set; }
        public List<IRandomAccessStreamReference> Files { get; set; } = new List<IRandomAccessStreamReference>();

        public PostInfo Clone() => new PostInfo {
            Text = Text,
            EMail = EMail,
            Name = Name,
            Subject = Subject,
            IsOp = IsOp,
            Files = Files
        };

        public void Clear() {
            Text = Subject = string.Empty;
        }
    }
}