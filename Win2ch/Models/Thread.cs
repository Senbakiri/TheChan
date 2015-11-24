using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win2ch.Models
{
    public class Thread
    {
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}
