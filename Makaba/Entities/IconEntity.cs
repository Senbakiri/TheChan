using System.Runtime.Serialization;

namespace Makaba.Entities {

    [DataContract]
    public class IconEntity {

        [DataMember]
        public string Name { get; set; }

        [DataMember(Name = "num")]
        public int Number { get; set; }

        [DataMember]
        public string Url { get; set; }
    }
}