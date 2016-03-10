using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;

namespace Win2ch.Core {
    class Tab : Screen {
        public virtual string BadgeContent { get; set; }

        public virtual bool IsLoading { get; set; }

        public virtual Image Icon { get; set; }

        public virtual bool IsCloseable { get; set; } = true;
    }
}
