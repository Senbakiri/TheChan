using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.LockScreen;
using Windows.UI.Notifications;
using NotificationsExtensions.Badges;

namespace Win2ch.Services {
    public sealed class BadgeService {
        public static BadgeService Instance { get; } = new BadgeService();

        private BadgeService() { }

        public async Task Update() {
            var count = (await FavoritesService.Instance.Threads.GetItems())
                .Count(t => t.UnreadPosts > 0);

            var updater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
            var content = new BadgeNumericNotificationContent {
                Number = (uint) count
            };

            var notification = new BadgeNotification(content.GetXml());
            updater.Update(notification);
        }
    }
}