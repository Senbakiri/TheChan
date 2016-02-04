using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Streams;
using Win2ch.Common;

namespace Win2ch.Models {
    public class NewPostInfo {
        public string Comment { get; set; }
        public string Subject { get; set; }
        public string EMail { get; set; }
        public string Name { get; set; }
        public List<IRandomAccessStreamReference> Files { get; set; }
    }
}