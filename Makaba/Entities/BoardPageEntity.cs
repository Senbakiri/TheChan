using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Makaba.Entities {

    [DataContract]
    public class BoardPageEntity {

        [DataMember(Name = "Board")]
        public string BoardId { get; set; }

        [DataMember(Name = "BoardInfo")]
        public string BoardInfo { get; set; }

        [DataMember(Name = "BoardInfoOuter")]
        public string BoardInfoOuter { get; set; }

        [DataMember(Name = "BoardName")]
        public string BoardName { get; set; }

        [DataMember(Name = "board_banner_image")]
        public string BoardBannerImage { get; set; }

        [DataMember(Name = "board_banner_link")]
        public string BoardBannerLink { get; set; }

        [DataMember(Name = "board_speed")]
        public int BoardSpeed { get; set; }

        [DataMember(Name = "bump_limit")]
        public int BumpLimit { get; set; }

        [DataMember(Name = "current_page")]
        public int CurrentPage { get; set; }

        [DataMember(Name = "current_thread")]
        public int CurrentThread { get; set; }

        [DataMember(Name = "default_name")]
        public string DefaultName { get; set; }

        [DataMember(Name = "enable_dices")]
        public bool EnableDices { get; set; }

        [DataMember(Name = "enable_flags")]
        public bool EnableFlags { get; set; }

        [DataMember(Name = "enable_icons")]
        public bool EnableIcons { get; set; }

        [DataMember(Name = "enable_images")]
        public bool EnableImages { get; set; }

        [DataMember(Name = "enable_likes")]
        public bool EnableLikes { get; set; }

        [DataMember(Name = "enable_names")]
        public bool EnableNames { get; set; }

        [DataMember(Name = "enable_posting")]
        public bool EnablePosting { get; set; }

        [DataMember(Name = "enable_sage")]
        public bool EnableSage { get; set; }

        [DataMember(Name = "enable_shield")]
        public bool EnableShield { get; set; }

        [DataMember(Name = "enable_subject")]
        public bool EnableSubject { get; set; }

        [DataMember(Name = "enable_thread_tags")]
        public bool EnableThreadTags { get; set; }

        [DataMember(Name = "enable_trips")]
        public bool EnableTrips { get; set; }

        [DataMember(Name = "enable_video")]
        public bool EnableVideo { get; set; }

        [DataMember(Name = "icons")]
        public IList<BoardIconEntity> Icons { get; set; }

        [DataMember(Name = "is_board")]
        public int IsBoard { get; set; }

        [DataMember(Name = "is_index")]
        public int IsIndex { get; set; }

        [DataMember(Name = "max_comment")]
        public int MaxComment { get; set; }

        [DataMember(Name = "news")]
        public IList<BoardNewsEntity> News { get; set; }

        [DataMember(Name = "pages")]
        public IList<int> Pages { get; set; }

        [DataMember(Name = "threads")]
        public IList<BoardThreadEntity> Threads { get; set; }

        [DataMember(Name = "top")]
        public IList<BoardTopItemEntity> Top { get; set; }
    }
}