using System.Net;
using Windows.Data.Html;

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
            }
        }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Num { get; set; }
        public string Date { get; set; }
        public string Text => HtmlUtilities.ConvertToText(Comment);
    }
}
