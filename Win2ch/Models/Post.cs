using System.Net;
using System.Text.RegularExpressions;

namespace Win2ch.Models
{
    public class Post
    {
        private string _Comment;
        public string Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = WebUtility.HtmlDecode(value);
                Text = RemoveHtml(Comment);
            }
        }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Num { get; set; }
        public string Date { get; set; }
        public string Text { get; private set; }

        private string RemoveHtml(string html)
        {
            string result = html;

            result = Regex.Replace(result,
                 @"<( )*br( )*>", "\r",
                 RegexOptions.IgnoreCase);
            result = Regex.Replace(result,
                     @"<( )*li( )*>", "\r",
                     RegexOptions.IgnoreCase);

            // insert line paragraphs (double line breaks) in place
            // if <P>, <DIV> and <TR> tags
            result = Regex.Replace(result,
                     @"<( )*div([^>])*>", "\r\r",
                     RegexOptions.IgnoreCase);
            result = Regex.Replace(result,
                     @"<( )*tr([^>])*>", "\r\r",
                     RegexOptions.IgnoreCase);
            result = Regex.Replace(result,
                     @"<( )*p([^>])*>", "\r\r",
                     RegexOptions.IgnoreCase);

            // Remove remaining tags like <a>, links, images,
            // comments etc - anything that's enclosed inside < >
            result = Regex.Replace(result,
                @"<[^>]*>", string.Empty,
                RegexOptions.IgnoreCase);

            return result.Trim();
        }
    }
}
