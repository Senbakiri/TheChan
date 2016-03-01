using System;
using System.Linq;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Win2ch.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.System.Profile;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Newtonsoft.Json.Linq;
using Template10.Common;
using Template10.Utils;
using Win2ch.Models;
using Win2ch.Services;
using Win2ch.ViewModels;
using Win2ch.Views;

namespace Win2ch {
    sealed partial class App {
        public App() {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync();
            InitializeComponent();
            SplashFactory = e => new Splash(e);

            #region App settings

            ISettingsService settings = SettingsService.Instance;
            if (settings.AppTheme != Theme.System)
                RequestedTheme = (ApplicationTheme)settings.AppTheme;
            CacheMaxDuration = settings.CacheMaxDuration;
            ShowShellBackButton = settings.UseShellBackButton;

            #endregion
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args) {
            ApplicationView.GetForCurrentView()?.SetPreferredMinSize(new Size(360, 620));
            if (DeviceUtils.Current().DeviceFamily() == DeviceUtils.DeviceFamilies.Mobile)
                await StatusBar.GetForCurrentView().HideAsync();
            var shell = Window.Current.Content as Shell;
            if (shell == null) {
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
                shell = new Shell(nav);
                Window.Current.Content = shell;
            }
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args) {
            await SetupJumpList();
            await NavigateToNeededPage(args);
        }

        private async Task NavigateToNeededPage(IActivatedEventArgs args) {
            switch (DetermineStartCause(args)) {
                case AdditionalKinds.SecondaryTile:
                case AdditionalKinds.JumpListItem:
                    await NavigateToPageWithArguments(((LaunchActivatedEventArgs)args).Arguments);
                    break;
                default:
                    NavigationService.Navigate(typeof(MainPage));
                    break;
            }
        }

        private async Task NavigateToPageWithArguments(string arguments) {
            var json = JObject.Parse(arguments);
            var board = json["board"].Value<string>();
            switch (json["type"].Value<string>()) {
                case "board":
                    NavigationService.Navigate(typeof (BoardPage), new Board(board));
                    break;
                case "thread":
                    await NavigateToThread(board, json["thread"].Value<long>());
                    break;
            }
        }

        private async Task NavigateToThread(string board, long num) {
            var navigation = ThreadNavigation.NavigateToThread(num, board);
            var threads = await FavoritesService.Instance.Threads.GetItems();
            var favThread = threads.FirstOrDefault(t => t.Num == num);
            if (favThread != null) {
                navigation.WithHighlighting(favThread.LastPostPosition - favThread.UnreadPosts + 1);
            }

            NavigationService.Navigate(typeof (ThreadPage), navigation);
        }

        public static async Task SetupJumpList() {
            if (!JumpList.IsSupported())
                return;
            var jumpList = await JumpList.LoadCurrentAsync();
            jumpList.Items.Clear();
            var favBoards = await FavoritesService.Instance.Boards.GetItems();
            foreach (var board in favBoards) {
                var args = new JObject {
                    {"type", "board" },
                    {"board", board.Id }
                };

                var item = JumpListItem.CreateWithArguments(args.ToString(), $"/{board.Id}/ - {board.Name}");
                item.GroupName = "Избранное";
                item.Logo = new Uri("ms-appx:///Assets/Square44x44Logo.targetsize-24.png");
                jumpList.Items.Add(item);
            }

            await jumpList.SaveAsync();
        }
    }
}

