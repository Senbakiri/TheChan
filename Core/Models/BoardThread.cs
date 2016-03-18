using System.Collections.Generic;

namespace Core.Models {
    public class BoardThread {
        public BoardThread(long number,
                           Post post,
                           int skippedPosts, 
                           int skippedPostsWithFiles) {
            Number = number;
            Post = post;
            SkippedPosts = skippedPosts;
            SkippedPostsWithFiles = skippedPostsWithFiles;
        }

        public long Number { get; }
        public Post Post { get; } 
        public int SkippedPosts { get; }
        public int SkippedPostsWithFiles { get; }
    }
}