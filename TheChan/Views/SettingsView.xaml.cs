using TheChan.ViewModels;

namespace TheChan.Views {
    public sealed partial class SettingsView {
        public SettingsView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as SettingsViewModel;
        }

        private SettingsViewModel ViewModel { get; set; }
    }
}
