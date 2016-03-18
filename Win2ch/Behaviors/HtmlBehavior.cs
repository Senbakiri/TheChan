using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using HtmlAgilityPack;
using Microsoft.Xaml.Interactivity;
using Win2ch.Common.Html;
using Win2ch.Extensions;

namespace Win2ch.Behaviors {
    internal class HtmlBehavior : Behavior<TextBlock> {

        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register(
            "Html", typeof(string), typeof(HtmlBehavior),
            new PropertyMetadata(string.Empty, PropertyChangedCallback));


        public string Html {
            get { return (string)GetValue(HtmlProperty); }
            set {
                SetValue(HtmlProperty, value);
            }
        }

        public static readonly DependencyProperty SpoilerForegroundProperty = DependencyProperty.Register(
                                                        "SpoilerForeground", typeof (Brush), typeof (HtmlBehavior),
                                                        new PropertyMetadata(default(Brush), PropertyChangedCallback));

        public Brush SpoilerForeground {
            get { return (Brush) GetValue(SpoilerForegroundProperty); }
            set { SetValue(SpoilerForegroundProperty, value); }
        }

        public static readonly DependencyProperty ReplyForegroundProperty = DependencyProperty.Register(
                                                        "ReplyForeground", typeof (Brush), typeof (HtmlBehavior),
                                                        new PropertyMetadata(default(Brush), PropertyChangedCallback));

        public Brush ReplyForeground {
            get { return (Brush) GetValue(ReplyForegroundProperty); }
            set { SetValue(ReplyForegroundProperty, value); }
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
            ((HtmlBehavior)dependencyObject).RenderHtml();
        }

        private readonly HtmlConverter converter = new HtmlConverter();

        public HtmlBehavior() {
            this.converter.Selector = Selector<HtmlNode, InlineWrapper>.Begin()
                .AddMatch(CreateNodeNameCondition("strong"),
                    (node, b) => b.Style(i => i.FontWeight = FontWeights.SemiBold))
                .AddMatch(CreateNodeNameCondition("em"),
                    (node, b) => b.Style(i => i.FontStyle = FontStyle.Italic))
                .AddMatch(CheckForClass("unkfunc"),
                    (node, b) => b.Style(i => i.Foreground = ReplyForeground))
                .AddMatch(CreateNodeNameCondition("br"), (node, b) => b.Apply(w => w.Text = "\n"))
                .AddMatch(CheckForClass("spoiler"),
                    (node, b) => b.Style(i => i.Foreground = SpoilerForeground))
                .AddMatch(CheckForClass("u"), (node, b) => b.SetConverter(ConvertToUnderline))
                .AddMatch(CheckForClass("s"),
                    (node, b) => b.Style(i => i.Foreground = SpoilerForeground))
                .AddMatch(IsRegularLink, (node, b) => b.SetConverter(
                    CreateRegularLinkConverter(node.GetAttributeValue("href", null))))
                .AddMatch(IsPostLink, (node, b) => b.SetConverter(
                    CreatePostLinkConverter(node)));
        }


        public event Action<int, int> PostNumClicked = delegate { };

        private void RenderHtml() {
            var block = AssociatedObject;
            if (block == null)
                return;

            var inlines = this.converter.Convert(Html);
            block.Inlines.Clear();
            foreach (var inline in inlines) {
                block.Inlines.Add(inline);
            }
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
                try {
                    var link = new Hyperlink {NavigateUri = new Uri(HtmlEntity.DeEntitize(url))};
                    link.Inlines.Add(InlineWrapper.StandartConverter(wrapper));
                    return link;
                } catch (UriFormatException) {
                    return InlineWrapper.StandartConverter(wrapper);
                }
            };
        }

        private Func<InlineWrapper, Inline> CreatePostLinkConverter(HtmlNode node) {
            int postNum = node.GetAttributeValue("data-num", 0);
            int threadNum = node.GetAttributeValue("data-thread", 0);
            return wrapper => {
                var link = new Hyperlink {
                    Foreground = (SolidColorBrush)Application.Current.Resources["SystemControlForegroundAccentBrush"],
                    UnderlineStyle = UnderlineStyle.None,
                    FontWeight = FontWeights.SemiBold,
                };
                link.Click += (s, e) => PostNumClicked(postNum, threadNum);
                link.Inlines.Add(InlineWrapper.StandartConverter(wrapper));
                return link;
            };
        }

        private static Inline ConvertToUnderline(InlineWrapper wrapper) {
            return new Underline {
                Inlines = {
                    InlineWrapper.StandartConverter(wrapper)
                }
            };
        }

        private static Func<HtmlNode, bool> CheckForClass(string className) {
            return  n => className.EqualsNc(n.GetAttributeValue("class", ""));
        }

        private static Func<HtmlNode, bool> CreateNodeNameCondition(string nodeName) {
            return n => nodeName.EqualsNc(n.Name);
        }
    }
}
