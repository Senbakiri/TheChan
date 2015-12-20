using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Win2ch.Models
{
    public class Post
    {
        private string _Comment;
        private string _name;

        public string Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = WebUtility.HtmlDecode(value);
                Text = RemoveHtml(Comment);
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = RemoveHtml(value); }
        }

        public string Subject { get; set; }
        public string Num { get; set; }
        public string Date { get; set; }
        public string Text { get; private set; }
        public Board Board { get; set; }

        [JsonProperty(PropertyName = "files")]
        public List<ImageInfo> Images { get; set; } = new List<ImageInfo>();

        private string RemoveHtml(string html)
        {
            string result = html;

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
                     @"\\r\\n", string.Empty);

            // Remove remaining tags like <a>, links, images,
            // comments etc - anything that's enclosed inside < >
            result = Regex.Replace(result,
                @"<[^>]*>", string.Empty,
                RegexOptions.IgnoreCase);

            return result.Trim();
        }
    }
}
