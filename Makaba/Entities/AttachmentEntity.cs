using System.Runtime.Serialization;

namespace Makaba.Entities {
    [DataContract]
    public class AttachmentEntity {
        [DataMember(Name = "height")]
        public int Height { get; set; }

        [DataMember(Name = "md5")]
        public string Md5 { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "size")]
        public int Size { get; set; }

        [DataMember(Name = "thumbnail")]
        public string Thumbnail { get; set; }

        [DataMember(Name = "tn_height")]
        public int ThumbnailHeight { get; set; }

        [DataMember(Name = "tn_width")]
        public int ThumbnailWidth { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }

        [DataMember(Name = "width")]
        public int Width { get; set; }
    }
}