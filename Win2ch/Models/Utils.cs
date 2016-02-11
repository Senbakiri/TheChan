using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Popups;
using HtmlAgilityPack;
using Win2ch.Models.Exceptions;

namespace Win2ch.Models {
    public static class Utils {
        private static IAsyncOperation<IUICommand> _messageDialogCommand;

        public static async Task ShowConnectionError(COMException exception, string title) {
            await ShowDialog(
                $"Проверьте подключение к интернету. Код ошибки: 0x{exception.HResult:X}.",
                title);
        }

        public static async Task ShowHttpError(HttpException exception, string title) {
            await ShowDialog($"Код ошибки: {exception.Code} {exception.Message}.",
                title);
        }

        public static async Task ShowOtherError(Exception exception, string title) {
            await ShowDialog(exception.Message, title);
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
