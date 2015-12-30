using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Win2ch.Models;
using Win2ch.ViewModels;

namespace Win2ch.Views
{
    public class PostingPageNavigationInfo
    {
        public NewPostInfo PostInfo { get; set; }
        public Thread Thread { get; set; }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PostingPage : Page
    {
        public PostingViewModel ViewModel => DataContext as PostingViewModel;

        public PostingPage()
        {
            this.InitializeComponent();
        }

        private void TextButton_OnClick(object sender, RoutedEventArgs e)
        {
            TextBox.Focus(FocusState.Programmatic);
        }

        private void AttachedImages_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.AttachedImages.Remove(e.ClickedItem as BitmapImage);
        }
    }
}
