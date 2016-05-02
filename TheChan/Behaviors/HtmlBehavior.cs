using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Caliburn.Micro;
using Core.Common;
using Core.Common.Links;
using HtmlAgilityPack;
using Microsoft.Xaml.Interactivity;
using TheChan.Common;
using TheChan.Common.Html;
using TheChan.Common.UI;
using TheChan.Extensions;
using TheChan.ViewModels;
using LinkType = Core.Common.LinkType;

namespace TheChan.Behaviors {
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

        public static readonly DependencyProperty SpoilerForegroundProperty =
            DependencyProperty.Register("SpoilerForeground",
                typeof (Brush), typeof (HtmlBehavior),
                new PropertyMetadata(default(Brush), PropertyChangedCallback));

        public Brush SpoilerForeground {
            get { return (Brush) GetValue(SpoilerForegroundProperty); }
            set { SetValue(SpoilerForegroundProperty, value); }
        }

        public static readonly DependencyProperty ReplyForegroundProperty =
            DependencyProperty.Register("ReplyForeground",
                typeof (Brush), typeof (HtmlBehavior),
                new PropertyMetadata(default(Brush), PropertyChangedCallback));

        public Brush ReplyForeground {
            get { return (Brush) GetValue(ReplyForegroundProperty); }
            set { SetValue(ReplyForegroundProperty, value); }
        }
        
        private IUrlService UrlService { get; set; }
        
        private IShell Shell { get; set; }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
            ((HtmlBehavior)dependencyObject).RenderHtml();
        }

        private readonly HtmlConverter converter = new HtmlConverter();

        public HtmlBehavior() {
            Shell = IoC.Get<IShell>();
            UrlService = IoC.Get<IUrlService>();
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
                .AddMatch(IsBoardLink, (node, b) => b.SetConverter(
                    CreateBoardLinkConverter(node)));
        }


        public event EventHandler<PostClickEventArgs> PostClick = delegate { };

        private void RenderHtml() {
            var block = AssociatedObject;
            if (block == null || UrlService == null || Shell == null)
                return;

            var inlines = this.converter.Convert(Html);
            block.Inlines.Clear();
            foreach (var inline in inlines) {
                block.Inlines.Add(inline);
            }
        }

        private bool IsRegularLink(HtmlNode node) {
            string href = node.GetAttributeValue("href", null);
            LinkType type = UrlService.DetermineLinkType(href);
            return node.Name.EqualsNc("a")
                   && type == LinkType.Unknown;
        }

        private bool IsBoardLink(HtmlNode node) {
            string href = node.GetAttributeValue("href", null);
            LinkType type = UrlService.DetermineLinkType(href);
            return node.Name.EqualsNc("a")
                   && (type == LinkType.Board || type == LinkType.Post || type == LinkType.Thread);
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

        private Func<InlineWrapper, Inline> CreateBoardLinkConverter(HtmlNode node) {
            string href = node.GetAttributeValue("href", null);
            LinkType type = UrlService.DetermineLinkType(href);
            LinkBase link = UrlService.GetLink(href);

            return wrapper => {
                var hyperlink = new Hyperlink {
                    Foreground = (SolidColorBrush)Application.Current.Resources["SystemControlForegroundAccentBrush"],
                    UnderlineStyle = UnderlineStyle.None,
                    FontWeight = FontWeights.SemiBold,
                };

                if (type == LinkType.Post) {
                    var postLink = (PostLink) link;
                    hyperlink.Click += (s, e) => PostClick(this, new PostClickEventArgs(postLink));
                } else {
                    hyperlink.Click += (s, e) => NavigateByLink(type, link);
                }

                hyperlink.Inlines.Add(InlineWrapper.StandartConverter(wrapper));
                return hyperlink;
            };
        }

        private void NavigateByLink(LinkType type, LinkBase link) {
            switch (type) {
                case LinkType.Board:
                    Shell.Navigate<BoardViewModel>(((BoardLink)link).BoardId);
                    break;
                case LinkType.Thread:
                case LinkType.Post:
                    var threadLink = (ThreadLink)link;
                    Shell.Navigate<ThreadViewModel>(
                        ThreadNavigation.NavigateToThread(threadLink.BoardId, threadLink.ThreadNumber));
                    break;
            }
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

    public class PostClickEventArgs : EventArgs {
        public PostClickEventArgs(PostLink link) {
            Link = link;
        }

        public PostLink Link { get; }
    }
}
