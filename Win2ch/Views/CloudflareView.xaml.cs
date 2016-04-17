using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class CloudflareView {
        public CloudflareView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as CloudflareViewModel;
        }

        public CloudflareViewModel ViewModel { get; private set; }
    }
}
