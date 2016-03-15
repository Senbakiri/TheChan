using System.Runtime.Serialization;

namespace Makaba.Entities {

    [DataContract]
    public class BoardTopItemEntity {
        [DataMember(Name = "board")]
        public string Board { get; set; }

        [DataMember(Name = "info")]
        public string Info { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "speed")]
        public int Speed { get; set; }
    }
}