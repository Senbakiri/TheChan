using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using HtmlAgilityPack;
using Win2ch.Models.Exceptions;

namespace Win2ch.Models {
    public static class Utils {
        public static async Task ShowConnectionError(COMException exception, string title) {
            var dialog = new MessageDialog(
                $"Проверьте подключение к интернету. Код ошибки: 0x{exception.HResult:X}.",
                title);
            await dialog.ShowAsync();
        }

        public static async Task ShowHttpError(HttpException exception, string title) {
            var dialog = new MessageDialog(
                $"Код ошибки: {exception.Code} {exception.Message}.",
                title);
            await dialog.ShowAsync();
        }

        public static async Task ShowOtherError(Exception exception, string title) {
            var dialog = new MessageDialog(
                exception.Message, title);
            await dialog.ShowAsync();
        }


        public static string RemoveHtml(string html) {
            var doc = new HtmlDocument();
            doc.LoadHtml(html.Replace("<br>", "\n"));
            return HtmlEntity.DeEntitize(doc.DocumentNode.InnerText);
        }
    }
}
