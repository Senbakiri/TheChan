using System.Collections.Generic;
using Windows.Storage;

namespace Win2ch.Models {
    public class NewPostInfo {
        public string Comment { get; set; }
        public string Subject { get; set; }
        public string EMail { get; set; }
        public string Name { get; set; }
        public List<StorageFile> Files { get; set; }
    }
}