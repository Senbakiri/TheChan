using Windows.UI.Xaml.Controls;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class RecentThreadsView {
        public RecentThreadsView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as RecentThreadsViewModel;
        }

        private RecentThreadsViewModel ViewModel { get; set; }

        private void Threads_OnItemClick(object sender, ItemClickEventArgs e) {
            ViewModel.NavigateToThread((ThreadInfoViewModel) e.ClickedItem);
        }
    }
}
