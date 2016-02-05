using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win2ch.Models;

namespace Win2ch.Common {
    public static class CurrentPost {
        // Да, это глобальная переменная. Я вынужден.
        public static NewPostInfo PostInfo { get; set; }
    }
}
