using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Win2ch.Annotations;
using Win2ch.Models;
using Win2ch.Services;
using Win2ch.Services.SettingsServices;

namespace Win2ch.Controls {
    public sealed partial class PostControl : INotifyPropertyChanged {
        public static DependencyProperty PostProperty = DependencyProperty.Register(
            "Post",
            typeof(Post), typeof(PostControl),
            PropertyMetadata.Create(() => new Post(), PostPropertyChanged));

        public static DependencyProperty IsSimpleProperty = DependencyProperty.Register(
            "IsSimple",
            typeof(bool), typeof(PostControl),
            PropertyMetadata.Create(false));

        private static async void PostPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
            var postControl = (PostControl) dependencyObject;
            postControl.SetValue(DataContextProperty, e.NewValue);
            postControl.IsInFavorites = await FavoritesService.Instance.Posts.ContainsItem(postControl.Post);
        }

        public static readonly DependencyProperty MaxLinesProperty = DependencyProperty.Register(
            "MaxLines",
            typeof (int), typeof (PostControl),
            PropertyMetadata.Create(0));

        public int MaxLines {
            get { return (int) GetValue(MaxLinesProperty); }
            set { SetValue(MaxLinesProperty, value); }
        }

        private readonly ISettingsService _settings = SettingsService.Instance;
        private bool _IsInFavorites;

        public delegate void PostReplyEventHandler(object sender, PostReplyEventArgs e);

        public event PostReplyEventHandler Reply = delegate { };

        public delegate void AttachmentClickEventHandler(object sender, AttachmentClickEventArgs e);

        public event AttachmentClickEventHandler AttachmentClick = delegate { };

        public event EventHandler<ReplyShowEventArgs> ReplyShowRequested = delegate { };

        public event Action<Post> RepliesListShowRequested = delegate { };

        public event EventHandler<ParentPostShowEventArgs> ParentPostShowRequested = delegate { };

        public event EventHandler RemovedFromFavorites = delegate { };

        public bool ShowRepliesAsTree =>
            _settings.RepliesViewMode == RepliesViewMode.Tree ||
            (_settings.RepliesViewMode == RepliesViewMode.Auto && new MouseCapabilities().MousePresent > 0);

        public PostControl() {
            InitializeComponent();
        }

        public Post Post {
            get { return (Post)GetValue(PostProperty); }
            set {
                SetValue(PostProperty, value);
            }
        }

        public bool IsSimple {
            get { return (bool)GetValue(IsSimpleProperty); }
            set { SetValue(IsSimpleProperty, value); }
        }

        public bool IsInFavorites {
            get { return _IsInFavorites; }
            private set {
                if (_IsInFavorites == value)
                    return;
                _IsInFavorites = value;
                RaisePropertyChanged();
            }
        }

        private void PostNum_OnTapped(object sender, TappedRoutedEventArgs e) {
            Reply(this, new PostReplyEventArgs(PostText.SelectedText, Post));
        }

        private void ImagesGridView_OnItemClick(object sender, ItemClickEventArgs e) {
            AttachmentClick(this, new AttachmentClickEventArgs((Attachment)e.ClickedItem));
        }

        private void ReplyNum_OnPointerAction(object sender, PointerRoutedEventArgs e) {
            var elem = (FrameworkElement)e.OriginalSource;
            var post = (Post)elem.DataContext;
            ReplyShowRequested(sender, new ReplyShowEventArgs(Post, post, e));
        }

        private void RepliesButton_OnClick(object sender, RoutedEventArgs e) {
            RepliesListShowRequested(Post);
        }

        private void OnPostNumClicked(int postN, int threadNum) {
            ParentPostShowRequested(this, new ParentPostShowEventArgs(Post, threadNum, postN));
        }

        public async void Favorite() {
            var favorites = FavoritesService.Instance.Posts;
            var added = await favorites.Add(Post);
            Post.Replies.Clear();
            if (!added) {
                await favorites.RemoveItem(Post);
                RemovedFromFavorites(this, new EventArgs());
            }
            IsInFavorites = added;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }


    public class ReplyShowEventArgs {
        public ReplyShowEventArgs(Post parent, Post post, PointerRoutedEventArgs pointerEventArgs) {
            Parent = parent;
            Post = post;
            PointerEventArgs = pointerEventArgs;
        }

        public PointerRoutedEventArgs PointerEventArgs { get; }
        public Post Parent { get; }
        public Post Post { get; }
    }

    public class PostReplyEventArgs {
        public PostReplyEventArgs(string selectedText, Post post) {
            SelectedText = selectedText;
            Post = post;
        }

        public string SelectedText { get; }
        public Post Post { get; }
    }

    public class ParentPostShowEventArgs : EventArgs {
        public Post Source { get; }
        public int ThreadNum { get; }
        public int PostNum { get; }

        public ParentPostShowEventArgs(Post source, int threadNum, int postNum) {
            Source = source;
            ThreadNum = threadNum;
            PostNum = postNum;
        }
    }

    public class AttachmentClickEventArgs : EventArgs {
        public AttachmentClickEventArgs(Attachment attachment) {
            Attachment = attachment;
        }

        public Attachment Attachment { get; }
    }
}
