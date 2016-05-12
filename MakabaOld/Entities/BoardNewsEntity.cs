using System.Runtime.Serialization;

namespace Makaba.Entities {

    [DataContract]
    public class BoardNewsEntity {
        [DataMember(Name = "date")]
        public string Date { get; set; }

        [DataMember(Name = "num")]
        public string Num { get; set; }

        [DataMember(Name = "subject")]
        public string Subject { get; set; }
    }
}