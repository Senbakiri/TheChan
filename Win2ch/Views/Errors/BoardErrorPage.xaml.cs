using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Template10.Services.SerializationService;
using Win2ch.Models.Exceptions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win2ch.Views.Errors
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BoardErrorPage
    {
        public bool Is404 { get; private set; }
        public int ErrorCode { get; private set; }

        public BoardErrorPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Shell.HamburgerMenu.NavigationService.ClearCache(true);
            Shell.HamburgerMenu.NavigationService.ClearHistory();

            var exception = SerializationService.Json.Deserialize<HttpException>(e.Parameter?.ToString());
            if (exception == null)
                return;

            Is404 = exception.Code == HttpStatusCode.NotFound;
            ErrorCode = (int) exception.Code;

            Bindings.Update();
        }

        public void GoHome()
        {
            Shell.HamburgerMenu.NavigationService.Navigate(typeof (MainPage));
        }
    }
}
