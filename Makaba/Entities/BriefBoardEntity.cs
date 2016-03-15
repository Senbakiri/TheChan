using System.Runtime.Serialization;

namespace Makaba.Entities {

    [DataContract]
    public class BriefBoardEntity {

        [DataMember(Name = "bump_limit")]
        public int BumpLimit { get; set; }

        [DataMember(Name = "default_name")]
        public string DefaultName { get; set; }

        [DataMember(Name = "enable_likes")]
        public bool IsLikesEnabled { get; set; }

        [DataMember(Name = "enable_posting")]
        public bool IsPostingEnabled { get; set; }

        [DataMember(Name = "enable_thread_tags")]
        public bool IsThreadTagsEnabled { get; set; }

        [DataMember(Name = "sage")]
        public bool IsSageEnabled { get; set; }

        [DataMember(Name = "tripcodes")]
        public bool IsTripCodesEnabled { get; set; }

        [DataMember]
        public IconEntity[] Icons { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember(Name = "pages")]
        public int PagesCount { get; set; }
    }
}