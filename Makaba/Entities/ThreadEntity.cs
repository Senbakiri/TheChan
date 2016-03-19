using System.Collections.Generic;

namespace Makaba.Entities {
    public class ThreadEntity {
        public string BoardId { get; set; }
        public IList<PostEntity> Posts { get; set; } 
    }
}