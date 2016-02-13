using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using HtmlAgilityPack;
using Microsoft.ApplicationInsights;
using Win2ch.Models.Exceptions;

namespace Win2ch.Models {
    public static class Utils {
        private static IAsyncOperation<IUICommand> _messageDialogCommand;
        private static TelemetryClient _telemetryClient = new TelemetryClient();

        public static async Task ShowConnectionError(COMException exception, string title) {
            _telemetryClient.TrackException(exception);
            await ShowDialog(
                $"Проверьте подключение к интернету. Код ошибки: 0x{exception.HResult:X}.",
                title);
        }

        public static async Task ShowHttpError(HttpException exception, string title) {
            _telemetryClient.TrackException(exception);
            await ShowDialog($"Код ошибки: {exception.Code} {exception.Message}.",
                title);
        }

        public static async Task ShowOtherError(Exception exception, string title) {
            _telemetryClient.TrackException(exception);
            await ShowDialog(exception.Message, title);
        }

        public static void TrackError(Exception e) {
            _telemetryClient.TrackException(e);
        }

        private static async Task ShowDialog(string text, string title) {
            try {
                _messageDialogCommand?.Cancel();
                var dialog = new MessageDialog(text, title);
                _messageDialogCommand = dialog.ShowAsync();
                await _messageDialogCommand;
                _messageDialogCommand = null;
            } catch (UnauthorizedAccessException) {
                _messageDialogCommand = null;
            } catch (TaskCanceledException) {
                _messageDialogCommand = null;
            }
        }

        public static string RemoveHtml(string html) {
            var doc = new HtmlDocument();
            doc.LoadHtml(html.Replace("<br>", "\n"));
            return HtmlEntity.DeEntitize(doc.DocumentNode.InnerText);
        }
    }
}
