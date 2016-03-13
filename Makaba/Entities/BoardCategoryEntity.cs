using System.Runtime.Serialization;

namespace Makaba.Entities {

    [DataContract]
    public class BoardCategoryEntity {

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public BoardEntity[] Boards { get; set; }
    }
}