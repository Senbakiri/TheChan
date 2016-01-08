using Windows.UI.Xaml.Controls;
using Win2ch.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win2ch.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; private set; }

        public SettingsPage()
        {
            this.InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as SettingsViewModel;
        }
    }
}
