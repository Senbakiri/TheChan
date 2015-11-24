using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win2ch.Models
{
    static class Urls
    {
        public const string BoardsList = "https://2ch.hk/makaba/mobile.fcgi?task=get_boards";
        public const string ThreadsList = "https://2ch.hk/{0}/{1}.json";
    }
}
