using TheChan.ViewModels;

namespace TheChan.Views {

    public sealed partial class BoardView {
        public BoardView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as BoardViewModel;
        }

        private BoardViewModel ViewModel { get; set; }
    }
}
