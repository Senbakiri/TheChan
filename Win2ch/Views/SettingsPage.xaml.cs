using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class SettingsPage {
        public SettingsViewModel ViewModel { get; private set; }

        public SettingsPage() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as SettingsViewModel;
        }
    }
}
