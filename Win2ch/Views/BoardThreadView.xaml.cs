using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class BoardThreadView {
        public BoardThreadView() {
            this.InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as BoardThreadViewModel;
        }

        private BoardThreadViewModel ViewModel { get; set; }
    }
}
