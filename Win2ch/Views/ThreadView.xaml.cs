using Windows.UI.Xaml.Controls;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class ThreadView {
        public ThreadView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as ThreadViewModel;
        }

        public ThreadViewModel ViewModel { get; private set; }
    }
}
