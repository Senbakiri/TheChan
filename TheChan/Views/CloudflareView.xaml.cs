using TheChan.ViewModels;

namespace TheChan.Views {
    public sealed partial class CloudflareView {
        public CloudflareView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as CloudflareViewModel;
        }

        public CloudflareViewModel ViewModel { get; private set; }
    }
}
