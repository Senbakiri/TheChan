using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using NotificationsExtensions.Tiles;
using Win2ch.Converters;
using Win2ch.Models;
using System.Linq;
using Windows.UI.Notifications;

namespace Win2ch.Services {
    public class FavoriteThreadsStorageService : ThreadStorageService {
        public FavoriteThreadsStorageService() : base(ApplicationData.Current.RoamingFolder, "FavThreads.json") {}

        public override async Task Store() {
            await base.Store();
            await UpdateLiveTile();
        }

        private async Task UpdateLiveTile() {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            var threads = (await GetItems()).Where(t => t.UnreadPosts > 0).OrderByDescending(t => t.UnreadPosts);
            if (!threads.Any()) {
                updater.Clear();
                return;
            }

            TileBinding
                mediumTile = CreateMediumTile(threads),
                biggerTile = CreateLargeAndMediumTile(threads);

            var visual = new TileVisual();
            visual.TileLarge = visual.TileWide = biggerTile;
            visual.TileMedium = mediumTile;

            var content = new TileContent {
                Visual = visual
            };

            var notification = new TileNotification(content.GetXml());
            updater.Update(notification);
        }

        private TileBinding CreateLargeAndMediumTile(IEnumerable<StoredThreadInfo> threads) {

            var binding = new TileBinding();

            var content = new TileBindingContentAdaptive();
            foreach (var thread in threads) {
                var unreadPostsText = new NounFormConverter()
                    .Convert(thread.UnreadPosts, typeof (string), "пост поста постов", "ru-RU")
                    .ToString();
                var unreadNewText = new NounFormConverter()
                    .Convert(thread.UnreadPosts, typeof (string), "новый новых новых", "ru-RU");
                var unreadText = $"{thread.UnreadPosts} {unreadNewText} {unreadPostsText}";

                var subgroup = new TileSubgroup();
                subgroup.Children.Add(new TileText {
                    Style = TileTextStyle.Caption,
                    Text = thread.Name
                });

                subgroup.Children.Add(new TileText {
                    Style = TileTextStyle.CaptionSubtle,
                    Text = unreadText
                });

                content.Children.Add(new TileGroup {Children = {subgroup}});
                content.Children.Add(new TileText());
            }

            binding.Content = content;
            return binding;
        }

        private TileBinding CreateMediumTile(IEnumerable<StoredThreadInfo> threads) {
            var binding = new TileBinding();
            var content = new TileBindingContentAdaptive();
            foreach (var thread in threads) {

                var subgroup = new TileSubgroup();
                subgroup.Children.Add(new TileText {
                    Style = TileTextStyle.Caption,
                    Text = thread.Name
                });

                subgroup.Children.Add(new TileText {
                    Style = TileTextStyle.CaptionSubtle,
                    Text = $"+{thread.UnreadPosts}"
                });

                content.Children.Add(new TileGroup { Children = { subgroup } });
            }

            binding.Content = content;
            return binding;
        }
    }
}