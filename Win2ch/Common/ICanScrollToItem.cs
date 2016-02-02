using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win2ch.Common {
    public interface ICanScrollToItem<in T> {
        void ScrollToItem(T item);
    }
}
