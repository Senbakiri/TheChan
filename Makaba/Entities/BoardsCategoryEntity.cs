using System.Runtime.Serialization;

namespace Makaba.Entities {

    [DataContract]
    public class BoardsCategoryEntity {

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public BriefBoardEntity[] Boards { get; set; }
    }
}