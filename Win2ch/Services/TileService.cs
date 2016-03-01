using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Newtonsoft.Json.Linq;
using NotificationsExtensions.Tiles;
using Win2ch.Common;
using Win2ch.Converters;
using Win2ch.Models;

namespace Win2ch.Services {
    class TileService {
        public static TileService Instance { get; } = new TileService();

        private TileService() { }

        public async Task UpdateLiveTile() {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            var threadPairs =
                (await FavoritesService.Instance.Threads.GetItems())
                .Where(t => t.UnreadPosts > 0)
                .OrderByDescending(t => t.UnreadPosts)
                .Take(10)
                .Pair()
                .ToList();

            updater.Clear();
            if (!threadPairs.Any()) {
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

        public bool IsThreadPinned(Thread thread) {
            return SecondaryTile.Exists(thread.Num.ToString());
        }

        public async Task<bool> PinThread(Thread thread) {
            var info = new StoredThreadInfo(thread);
            var args = new JObject {
                {"type", "thread"},
                {"board", info.Board.Id},
                {"thread", info.Num}
            };

            var tiles = await SecondaryTile.FindAllAsync();
            var existingTile = tiles.FirstOrDefault(t => t.TileId == info.Num.ToString());
            if (existingTile != null) {
                return !await existingTile.RequestDeleteAsync();
            }

            var tile = new SecondaryTile {
                TileId = info.Num.ToString(),
                Arguments = args.ToString(),
                DisplayName = info.Name,
                VisualElements = {
                    ShowNameOnSquare150x150Logo = true,
                    ShowNameOnWide310x150Logo = true,
                    ShowNameOnSquare310x310Logo = true,
                    Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-100.png")
                }
            };

            return await tile.RequestCreateAsync();
        }

        public bool IsBoardPinned(Board board) {
            return SecondaryTile.Exists(board.Id);
        }

        public async Task<bool> PinBoard(Board board) {
            var args = new JObject {
                {"type", "thread"},
                {"board", board.Id}
            };

            var tiles = await SecondaryTile.FindAllAsync();
            var existingTile = tiles.FirstOrDefault(t => t.TileId == board.Id);
            if (existingTile != null) {
                return !await existingTile.RequestDeleteAsync();
            }

            var tile = new SecondaryTile {
                TileId = board.Id,
                Arguments = args.ToString(),
                DisplayName = $"/{board.Id}/ - {board.Name}",
                VisualElements = {
                    ShowNameOnSquare150x150Logo = true,
                    ShowNameOnWide310x150Logo = true,
                    ShowNameOnSquare310x310Logo = true,
                    Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-100.png")
                }
            };

            return await tile.RequestCreateAsync();
        }
    }
}
