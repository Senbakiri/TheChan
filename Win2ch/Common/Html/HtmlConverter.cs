using System.Collections.Generic;
using Windows.UI.Xaml.Documents;
using HtmlAgilityPack;

namespace Win2ch.Common.Html {
    public class HtmlConverter {
        public Selector<HtmlNode, InlineWrapper> Selector { get; set; }

        public IEnumerable<Inline> Convert(string html) {
            var doc = new HtmlDocument();
            doc.LoadHtml(html.Replace("\\r\\n", ""));
            var inlines = GetInlines(doc.DocumentNode);
            return inlines;
        }

        private IEnumerable<Inline> GetInlines(HtmlNode node) {
            var result = new List<Inline>();
            foreach (var childNode in node.ChildNodes) {
                var wrapper = new InlineWrapper(GetInlines(childNode), childNode.InnerText);
                var item = Selector == null
                    ? wrapper
                    : Selector.SetBase(wrapper).Select(childNode);
                result.Add(item.Unwrap());
            }

            return result;
        }
    }
}
