using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class SettingsView {
        public SettingsView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as SettingsViewModel;
        }

        private SettingsViewModel ViewModel { get; set; }
    }
}
