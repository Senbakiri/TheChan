using System.Collections.Generic;

namespace Core.Models {
    public class BoardThread {
        public BoardThread(long number,
                           IList<Post> posts,
                           int skippedPosts, 
                           int skippedPostsWithFiles) {
            Number = number;
            Posts = posts;
            SkippedPosts = skippedPosts;
            SkippedPostsWithFiles = skippedPostsWithFiles;
        }

        public long Number { get; }
        public IList<Post> Posts { get; } 
        public int SkippedPosts { get; }
        public int SkippedPostsWithFiles { get; }
    }
}