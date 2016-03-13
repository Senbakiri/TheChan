using System;

namespace Core.Models {
    public class Icon {
        public Icon(string name, int number, Uri url) {
            Name = name;
            Number = number;
            Url = url;
        }

        public string Name { get; }
        public int Number { get; }
        public Uri Url { get; }
    }
}