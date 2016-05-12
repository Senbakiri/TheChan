using System.Collections.Generic;

namespace Core.Models {
    public class BriefBoardInfo {
        public BriefBoardInfo(int bumpLimit,
                              string defaultName,
                              bool isLikesEnabled,
                              bool isPostingEnabled,
                              bool isThreadTagsEnabled,
                              bool isSageEnabled,
                              bool isTripCodesEnabled,
                              IList<Icon> icons,
                              string id,
                              string name) {

            BumpLimit = bumpLimit;
            DefaultName = defaultName;
            IsLikesEnabled = isLikesEnabled;
            IsPostingEnabled = isPostingEnabled;
            IsThreadTagsEnabled = isThreadTagsEnabled;
            IsSageEnabled = isSageEnabled;
            IsTripCodesEnabled = isTripCodesEnabled;
            Icons = icons;
            Id = id;
            Name = name;
        }

        public int BumpLimit { get; }

        public string DefaultName { get; }
        
        public bool IsLikesEnabled { get; }
        
        public bool IsPostingEnabled { get; }
        
        public bool IsThreadTagsEnabled { get; }
        
        public bool IsSageEnabled { get; }
        
        public bool IsTripCodesEnabled { get; }
        
        public IList<Icon> Icons { get; }
        
        public string Id { get; }
        
        public string Name { get; }
    }
}
