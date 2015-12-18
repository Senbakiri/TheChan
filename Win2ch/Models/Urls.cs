namespace Win2ch.Models
{
    static class Urls
    {
        public const string BoardsList = "https://2ch.hk/makaba/mobile.fcgi?task=get_boards";

        /// <summary>
        /// 0 - Board;
        /// 1 - Page (0 == index)
        /// </summary>
        public const string ThreadsList = "https://2ch.hk/{0}/{1}.json";

        /// <summary>
        /// 0 - Board;
        /// 1 - Thread;
        /// 2 - Beginning
        /// </summary>
        public const string ThreadPosts =
            "https://2ch.hk/makaba/mobile.fcgi?task=get_thread&board={0}&thread={1}&post={2}";

        public const string Posting = "https://2ch.hk/makaba/posting.fcgi";

        public const string ThreadPostsFromBoardNum =
            "https://2ch.hk/makaba/mobile.fcgi?task=get_thread&board={0}&thread={1}&num={2}";
    }
}
