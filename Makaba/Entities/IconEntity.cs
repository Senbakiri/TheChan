using System.Runtime.Serialization;

namespace Makaba.Entities {

    [DataContract]
    public class IconEntity {

        [DataMember]
        public string Name { get; set; }

        [DataMember(Name = "num")]
        public string Number { get; set; }

        [DataMember]
        public string Url { get; set; }
    }
}