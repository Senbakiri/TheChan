using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using HtmlAgilityPack;
using Microsoft.Xaml.Interactivity;
using Win2ch.Common;
using Win2ch.Common.HtmlConverting;

namespace Win2ch.Behaviors {
    class HtmlBehavior : Behavior<TextBlock> {

        public static DependencyProperty HtmlProperty = DependencyProperty.Register(
            "Html", typeof(string), typeof(HtmlBehavior),
            new PropertyMetadata(string.Empty));

        public static string GetHtml(HtmlBehavior behavior) {
            return behavior.Html;
        }

        public static void SetHtml(HtmlBehavior behavior, string html) {
            behavior.Html = html;
        }

        private string _Html;
        private readonly HtmlConverter _converter = new HtmlConverter();

        public HtmlBehavior() {
            _converter.Selector = Selector<HtmlNode, InlineWrapper>.Begin()
                .AddMatch(CreateNodeNameCondition("strong"),
                    (node, b) => b.Style(i => i.FontWeight = FontWeights.SemiBold))
                .AddMatch(CreateNodeNameCondition("em"),
                    (node, b) => b.Style(i => i.FontStyle = FontStyle.Italic))
                .AddMatch(CheckForClass("unkfunc"),
                    (node, b) => b.Style(i => i.Foreground = GetReplyBrush()))
                .AddMatch(CreateNodeNameCondition("br"), (node, b) => b.Apply(w => w.Text = "\n"))
                .AddMatch(CheckForClass("spoiler"),
                    (node, b) => b.Style(i => i.Foreground = GetSpoilerBrush()))
                .AddMatch(CheckForClass("u"), (node, b) => b.SetConverter(ConvertToUnderline))
                .AddMatch(CheckForClass("s"),
                    (node, b) => b.Style(i => i.Foreground = GetSpoilerBrush()))
                .AddMatch(IsRegularLink, (node, b) => b.SetConverter(
                    CreateRegularLinkConverter(node.GetAttributeValue("href", null))))
                .AddMatch(IsPostLink, (node, b) => b.SetConverter(
                    CreatePostLinkConverter(node)));
        }

        public string Html {
            get { return _Html; }
            set {
                _Html = value;
                RenderHtml();
            }
        }

        public event Action<int> PostNumClicked = delegate { };

        private void RenderHtml() {
            var block = AssociatedObject;
            if (block == null)
                return;

            var inlines = _converter.Convert(Html);
            block.Inlines.Clear();
            foreach (var inline in inlines) {
                block.Inlines.Add(inline);
            }
        }

        private static SolidColorBrush GetReplyBrush() {
            return (SolidColorBrush)Application.Current.Resources["ReplyForeground"];
        }

        private static bool IsRegularLink(HtmlNode node) {
            var href = node.GetAttributeValue("href", null);
            return node.Name.EqualsNc("a")
                && !string.IsNullOrWhiteSpace(href)
                && !href.StartsWith("/");
        }

        private static bool IsPostLink(HtmlNode node) {
            var href = node.GetAttributeValue("href", null);
            return node.Name.EqualsNc("a")
                   && !string.IsNullOrWhiteSpace(href)
                   && node.GetAttributeValue("class", "").Contains("post-reply-link");
        }

        private static Func<InlineWrapper, Inline> CreateRegularLinkConverter(string url) {
            return wrapper => {
                var link = new Hyperlink { NavigateUri = new Uri(HtmlEntity.DeEntitize(url)) };
                link.Inlines.Add(InlineWrapper.StandartConverter(wrapper));
                return link;
            };
        }

        private Func<InlineWrapper, Inline> CreatePostLinkConverter(HtmlNode node) {
            var postNum = node.GetAttributeValue("data-num", 0);
            return wrapper => {
                var link = new Hyperlink {
                    Foreground = (SolidColorBrush)Application.Current.Resources["CustomColorBrush"],
                    UnderlineStyle = UnderlineStyle.None,
                    FontWeight = FontWeights.SemiBold,
                };
                link.Click += (s, e) => PostNumClicked(postNum);
                link.Inlines.Add(InlineWrapper.StandartConverter(wrapper));
                return link;
            };
        }

        private static Inline ConvertToUnderline(InlineWrapper wrapper) {
            var u = new Underline();
            u.Inlines.Add(InlineWrapper.StandartConverter(wrapper));
            return u;
        }

        private static SolidColorBrush GetSpoilerBrush() {
            return (SolidColorBrush)Application.Current.Resources["SpoilerForeground"];
        }

        private static Func<HtmlNode, bool> CheckForClass(string className) {
            return
                n => string.Equals(n.GetAttributeValue("class", null), className, StringComparison.OrdinalIgnoreCase);
        }

        private static Func<HtmlNode, bool> CreateNodeNameCondition(string nodeName) {
            return n => string.Equals(nodeName, n.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
