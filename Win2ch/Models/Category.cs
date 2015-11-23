using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win2ch.Models
{
    public class Category : List<Board>
    {
        public Category(IEnumerable<Board> collection) : base(collection)
        { }

        public string Name { get; set; }
    }
}
