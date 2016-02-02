using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Newtonsoft.Json;

namespace Win2ch.Models {
    public class ImageInfo {
        public string Name { get; set; }

        public string Path { get; set; }
        public string Url => string.Format(Urls.BoardUrl, Board.Id, Path);

        public int Width { get; set; }
        public int Height { get; set; }

        public string Thumbnail { get; set; }
        public string ThumbnailUrl => string.Format(Urls.BoardUrl, Board.Id, Thumbnail);

        public int Size { get; set; }

        [JsonProperty(PropertyName = "th_width")]
        public int ThumbnailWidth { get; set; }

        [JsonProperty(PropertyName = "th_height")]
        public int ThumbnailHeight { get; set; }

        public Board Board { get; set; }

        protected bool Equals(ImageInfo other) {
            return string.Equals(Name, other.Name) && Equals(Board, other.Board);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ImageInfo)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Name?.GetHashCode() ?? 0) * 397) ^ (Board?.GetHashCode() ?? 0);
            }
        }
    }
}
