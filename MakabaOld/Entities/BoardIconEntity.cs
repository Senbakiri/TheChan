using System.Runtime.Serialization;

namespace Makaba.Entities {

    [DataContract]
    public class BoardIconEntity {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "num")]
        public int Num { get; set; }
    }
}