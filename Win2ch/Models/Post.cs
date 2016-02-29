﻿using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Windows.Web;
using HtmlAgilityPack;
using Win2ch.Common;

namespace Win2ch.Models {
    public class Post {
        private string _Name;
        private string _EMail;
        
        public string Comment { get; set; }

        public string Name {
            get { return _Name; }
            set { _Name = Utils.RemoveHtml(value); }
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

        [JsonIgnore]
        public List<Post> Replies { get; set; } = new List<Post>();

        [JsonProperty(PropertyName = "files")]
        public List<Attachment> Images { get; set; } = new List<Attachment>();


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
