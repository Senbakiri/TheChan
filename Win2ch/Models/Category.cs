using System.Collections.Generic;

namespace Win2ch.Models
{
    public class Category : List<Board>
    {
        public Category(IEnumerable<Board> collection) : base(collection)
        { }

        public string Name { get; set; }
    }
}
