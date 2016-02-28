using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Win2ch.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.System.Profile;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Template10.Common;
using Template10.Utils;
using Win2ch.Models;
using Win2ch.Services;
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

        public override Task OnStartAsync(StartKind startKind, IActivatedEventArgs args) {
            NavigationService.Navigate(typeof(MainPage));
            return Task.CompletedTask;
        }

        public static async Task SetupJumpList() {
            if (!JumpList.IsSupported())
                return;
            var jumpList = await JumpList.LoadCurrentAsync();
            jumpList.Items.Clear();
            var favBoards = await FavoritesService.Instance.Boards.GetItems();
            foreach (var board in favBoards) {
                var item = JumpListItem.CreateWithArguments(board.Id, $"/{board.Id}/ - {board.Name}");
                item.GroupName = "Избранное";
                item.Logo = new Uri("ms-appx:///Assets/Square44x44Logo.targetsize-24.png");
                jumpList.Items.Add(item);
            }

            await jumpList.SaveAsync();
        }

    }
}

