using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Core.Common;
using Win2ch.Annotations;
using Win2ch.Behaviors;
using Win2ch.Common;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class PostView {

        private static readonly MouseCapabilities MouseCapabilities = new MouseCapabilities();

        public PostView(IShell shell, IBoard board) {
            Shell = shell;
            Board = board;
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as PostViewModel;
            ShowRepliesAsRibbon = MouseCapabilities.MousePresent == 0;
        }

        public PostViewModel ViewModel { get; private set; }

        public bool ShowRepliesAsRibbon { get; }

        public IShell Shell { get; }

        public IBoard Board { get; }

        private void HtmlBehaviorOnPostClick(object sender, PostClickEventArgs e) {
            ViewModel.DisplayPost(e.Link);
        }

        private void ReplyTextBlockOnPointerInteraction(object sender, PointerRoutedEventArgs e) {
            var elem = (FrameworkElement)e.OriginalSource;
            var post = (PostViewModel)elem.DataContext;
            ViewModel.RequestReplyDisplaying(new ReplyDisplayingEventArgs(ViewModel, post, e, (FrameworkElement) sender));
        }
    }

    public class ReplyDisplayingEventArgs {
        public FrameworkElement SourceElement { get; }
        public PostViewModel Parent { get; }
        public PostViewModel Post { get; }
        public PointerRoutedEventArgs PointerEventArgs { get; }

        public ReplyDisplayingEventArgs(PostViewModel parent,
                                        PostViewModel post,
                                        PointerRoutedEventArgs pointerEventArgs,
                                        FrameworkElement sourceElement) {
            Parent = parent;
            Post = post;
            PointerEventArgs = pointerEventArgs;
            SourceElement = sourceElement;
        }
    }
}
