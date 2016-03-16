using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Makaba.Entities {

    [DataContract]
    public class BoardPostEntity {
        [DataMember(Name = "banned")]
        public bool IsBanned { get; set; }

        [DataMember(Name = "closed")]
        public bool IsClosed { get; set; }

        [DataMember(Name = "comment")]
        public string Comment { get; set; }

        [DataMember(Name = "date")]
        public string Date { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "files")]
        public IList<AttachmentEntity> Files { get; set; }

        [DataMember(Name = "files_count")]
        public int FilesCount { get; set; }

        [DataMember(Name = "hidden_num")]
        public long HiddenNum { get; set; }

        [DataMember(Name = "lasthit")]
        public int LastHit { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "num")]
        public long Num { get; set; }

        [DataMember(Name = "op")]
        public bool IsOp { get; set; }

        [DataMember(Name = "parent")]
        public long Parent { get; set; }

        [DataMember(Name = "posts_count")]
        public int PostsCount { get; set; }

        [DataMember(Name = "sticky")]
        public int Sticky { get; set; }

        [DataMember(Name = "subject")]
        public string Subject { get; set; }

        [DataMember(Name = "tags")]
        public string Tags { get; set; }

        [DataMember(Name = "timestamp")]
        public long Timestamp { get; set; }

        [DataMember(Name = "trip")]
        public string Trip { get; set; }

        [DataMember(Name = "icon")]
        public string Icon { get; set; }
    }
}