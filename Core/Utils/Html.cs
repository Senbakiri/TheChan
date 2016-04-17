using HtmlAgilityPack;

namespace Core.Utils {
    public static class Html {
        public static string RemoveHtml(string html) {
            var document = new HtmlDocument();
            document.LoadHtml(html.Replace("<br>", "\n"));
            return document.DocumentNode.InnerText;
        }
    }
}