using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Makaba.Entities {
    [DataContract]
    public class BoardThreadEntity {
        [DataMember(Name = "files_count")]
        public int FilesCount { get; set; }

        [DataMember(Name = "posts")]
        public IList<PostEntity> Posts { get; set; }

        [DataMember(Name = "posts_count")]
        public int PostsCount { get; set; }

        [DataMember(Name = "thread_num")]
        public long ThreadNum { get; set; }
    }
}