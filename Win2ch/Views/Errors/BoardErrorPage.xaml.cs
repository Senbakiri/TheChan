using System;
using System.Runtime.InteropServices;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Template10.Services.SerializationService;
using Win2ch.Models.Exceptions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win2ch.Views.Errors {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BoardErrorPage {

        public bool IsConnectionError { get; private set; }
        public bool Is404 { get; private set; }
        public string ErrorCode { get; private set; }

        public BoardErrorPage() {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            Shell.HamburgerMenu.NavigationService.ClearCache(true);
            Shell.HamburgerMenu.NavigationService.ClearHistory();

            var exception = SerializationService.Json.Deserialize<HttpException>(e.Parameter?.ToString());
            IsConnectionError = exception.IsConnectionError;

            Is404 = exception.Code == 404;
            ErrorCode = exception.IsConnectionError
                ? "0x" + exception.Code.ToString("X")
                : exception.Code.ToString();

            Bindings.Update();
        }

        public void GoHome() {
            Shell.HamburgerMenu.NavigationService.Navigate(typeof(MainPage));
        }
    }
}
