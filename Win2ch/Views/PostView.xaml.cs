using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Core.Common;
using Win2ch.Behaviors;
using Win2ch.Common;
using Win2ch.Common.UI;
using Win2ch.Services.Settings;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class PostView {

        private static readonly MouseCapabilities MouseCapabilities = new MouseCapabilities();

        public PostView(IShell shell, IBoard board, ISettingsService settingsService) {
            Shell = shell;
            Board = board;
            SettingsService = settingsService;
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as PostViewModel;
            ShowRepliesAsRibbon = MouseCapabilities.MousePresent == 0;
            PostFontSize = 16 * SettingsService.FontScale;
        }

        private double PostFontSize { get; }
        private ISettingsService SettingsService { get; }
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
            e.Handled = true;
        }

        private void TextBlock_OnSelectionChanged(object sender, RoutedEventArgs e) {
            var block = (TextBlock) sender;
            ViewModel.SelectedText = block.SelectedText ?? string.Empty;
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
