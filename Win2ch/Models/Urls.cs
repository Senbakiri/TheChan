﻿namespace Win2ch.Models {
    static class Urls {
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

        public const string BoardUrl = "https://2ch.hk/{0}/{1}";
        public const string BoardUrlHttp = "http://2ch.hk/{0}/{1}";

        public const string SinglePost = "https://2ch.hk/makaba/mobile.fcgi?task=get_post&board={0}&post={1}";

        public const string PostingCaptcha = "https://2ch.hk/makaba/captcha.fcgi?type=2chaptcha";

        public const string ThreadCaptcha = "https://2ch.hk/makaba/captcha.fcgi?type=2chaptcha&action=thread";

        public const string CaptchaImage = "https://2ch.hk/makaba/captcha.fcgi?type=2chaptcha&action=image&id={0}";
    }
}
