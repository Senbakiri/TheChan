﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Newtonsoft.Json;

namespace Win2ch.Models
{
    public class ImageInfo
    {
        public string Name { get; set; }
        
        public string Path { get; set; }
        public string Url => string.Format(Urls.BoardUrl, Board.Id, Path);

        public int Width { get; set; }
        public int Height { get; set; }
        
        public string Thumbnail { get; set; }
        public string ThumbnailUrl => string.Format(Urls.BoardUrl, Board.Id, Thumbnail);
        
        [JsonProperty(PropertyName = "th_width")]
        public int ThumbnailWidth { get; set; }

        [JsonProperty(PropertyName = "th_height")]
        public int ThumbnailHeight { get; set; }

        public Board Board { get; set; }
    }
}