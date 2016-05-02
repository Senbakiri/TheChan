using Windows.ApplicationModel;
using Windows.UI.Notifications;
using NotificationsExtensions.Toasts;

namespace TheChan.Services.Toast {
    public class ToastService : IToastService {
        private readonly ToastNotifier notifier;

        public ToastService() {
            this.notifier = ToastNotificationManager.CreateToastNotifier();
        }

        public void ShowSimpleToast(string text) {
            var toast = new ToastContent {
                Visual = new ToastVisual {
                    TitleText = new ToastText {
                        Text = Package.Current.DisplayName
                    },

                    BodyTextLine1 = new ToastText {
                        Text = text
                    }
                }
            };

            this.notifier.Show(new ToastNotification(toast.GetXml()));
        }
    }
}