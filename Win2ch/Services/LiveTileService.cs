using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using NotificationsExtensions.Tiles;
using Win2ch.Common;
using Win2ch.Converters;
using Win2ch.Models;

namespace Win2ch.Services {
    class LiveTileService {
        public static LiveTileService Instance { get; } = new LiveTileService();

        private LiveTileService() { }

        public async Task Update() {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            var threadPairs =
                (await FavoritesService.Instance.Threads.GetItems())
                .Where(t => t.UnreadPosts > 0)
                .OrderByDescending(t => t.UnreadPosts)
                .Take(10)
                .Pair()
                .ToList();

            if (!threadPairs.Any()) {
                updater.Clear();
                return;
            }

            updater.EnableNotificationQueue(true);

            foreach (var pair in threadPairs) {
                var list = pair.ToList();

                var content = new TileContent {
                    Visual = new TileVisual {
                        TileLarge = CreateLargeTile(list),
                        TileWide = CreateWideTile(list),
                        TileMedium = CreateMediumTile(list),
                    }
                };

                updater.Update(new TileNotification(content.GetXml()));
            }
        }

        private static TileBinding CreateWideTile(IEnumerable<StoredThreadInfo> threads) {

            var binding = new TileBinding {
                Branding = TileBranding.None
            };

            var content = new TileBindingContentAdaptive();
            foreach (var thread in threads) {
                var unreadPostsText = new NounFormConverter()
                    .Convert(thread.UnreadPosts, typeof(string), "пост поста постов", "ru-RU")
                    .ToString();
                var unreadNewText = new NounFormConverter()
                    .Convert(thread.UnreadPosts, typeof(string), "новый новых новых", "ru-RU");
                var unreadText = $"{thread.UnreadPosts} {unreadNewText} {unreadPostsText}";

                var subgroup = new TileSubgroup();
                subgroup.Children.Add(new TileText {
                    Style = TileTextStyle.Caption,
                    MaxLines = 1,
                    Text = thread.Name
                });

                subgroup.Children.Add(new TileText {
                    Style = TileTextStyle.CaptionSubtle,
                    Text = unreadText
                });

                content.Children.Add(new TileGroup { Children = { subgroup } });
                content.Children.Add(new TileText());
            }

            binding.Content = content;
            return binding;
        }

        private static TileBinding CreateLargeTile(IEnumerable<StoredThreadInfo> threads) {

            var binding = new TileBinding {
                Branding = TileBranding.NameAndLogo
            };

            var content = new TileBindingContentAdaptive();
            foreach (var thread in threads) {
                var unreadPostsText = new NounFormConverter()
                    .Convert(thread.UnreadPosts, typeof(string), "пост поста постов", "ru-RU")
                    .ToString();
                var unreadNewText = new NounFormConverter()
                    .Convert(thread.UnreadPosts, typeof(string), "новый новых новых", "ru-RU");
                var unreadText = $"{thread.UnreadPosts} {unreadNewText} {unreadPostsText}";

                var subgroup = new TileSubgroup();
                subgroup.Children.Add(new TileText {
                    Style = TileTextStyle.Body,
                    Wrap = true,
                    MaxLines = 3,
                    Text = thread.Name
                });

                subgroup.Children.Add(new TileText {
                    Style = TileTextStyle.CaptionSubtle,
                    Text = unreadText
                });

                content.Children.Add(new TileGroup { Children = { subgroup } });
                content.Children.Add(new TileText());
            }

            binding.Content = content;
            return binding;
        }

        private static TileBinding CreateMediumTile(IEnumerable<StoredThreadInfo> threads) {
            var binding = new TileBinding {
                Branding = TileBranding.Logo
            };
            
            var content = new TileBindingContentAdaptive();
            foreach (var thread in threads) {

                var subgroup = new TileSubgroup();
                subgroup.Children.Add(new TileText {
                    Style = TileTextStyle.Caption,
                    MaxLines = 1,
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
