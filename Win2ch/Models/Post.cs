using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Windows.Web;

namespace Win2ch.Models {
    public class Post {
        private string _Comment;
        private string _Name;
        private string _EMail;
        
        public string Comment {
            get { return _Comment; }
            set {
                _Comment = RemoveHtml(value);
            }
        }
        
        public string Name {
            get { return _Name; }
            set { _Name = RemoveHtml(value); }
        }

        public int Position { get; set; }

        public string EMail {
            get { return _EMail; }
            set {
                _EMail = value.Contains("mailto:") ? value.Substring("mailto:".Length) : value;
            }
        }

        public bool IsSage => EMail.ToLower() == "sage";

        public string Subject { get; set; }
        public long Num { get; set; }
        public string Date { get; set; }
        public Board Board { get; set; }
        public List<Post> Replies { get; set; } = new List<Post>();

        [JsonProperty(PropertyName = "files")]
        public List<ImageInfo> Images { get; set; } = new List<ImageInfo>();

        private string RemoveHtml(string html) {
            string result = WebUtility.HtmlDecode(html);

            result = Regex.Replace(result,
                 @"<( )*br( )*>", "\n",
                 RegexOptions.IgnoreCase);
            result = Regex.Replace(result,
                     @"<( )*li( )*>", "\n",
                     RegexOptions.IgnoreCase);

            // insert line paragraphs (double line breaks) in place
            // if <P>, <DIV> and <TR> tags
            result = Regex.Replace(result,
                     @"<( )*div([^>])*>", "\n",
                     RegexOptions.IgnoreCase);
            result = Regex.Replace(result,
                     @"<( )*tr([^>])*>", "\n",
                     RegexOptions.IgnoreCase);
            result = Regex.Replace(result,
                     @"<( )*p([^>])*>", "\n",
                     RegexOptions.IgnoreCase);
            result = Regex.Replace(result,
                     @"\\r\\n", string.Empty,
                     RegexOptions.IgnoreCase);

            // Remove remaining tags like <a>, links, images,
            // comments etc - anything that's enclosed inside < >
            result = Regex.Replace(result,
                @"<[^>]*>", string.Empty,
                RegexOptions.IgnoreCase);

            return result.Trim();
        }

        protected bool Equals(Post other) {
            return string.Equals(Num, other.Num) && Equals(Board, other.Board);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Post)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (Num.GetHashCode() * 397) ^ (Board?.GetHashCode() ?? 0);
            }
        }
    }
}
