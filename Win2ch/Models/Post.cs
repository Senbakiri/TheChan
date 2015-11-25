using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        public string Num { get; set; }
        public string Date { get; set; }
    }
}
