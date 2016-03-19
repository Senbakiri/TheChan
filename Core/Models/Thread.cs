using System.Collections.Generic;

namespace Core.Models {
    public class Thread {
        public Thread(IList<Post> posts) {
            Posts = posts;
        }

        public IList<Post> Posts { get; } 
    }
}