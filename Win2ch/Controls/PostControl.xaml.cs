using System;
using System.Collections.Generic;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Win2ch.Models;
using Win2ch.Services.SettingsServices;
using Win2ch.Views;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win2ch.Controls
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class PostControl : Page
    {


        public static DependencyProperty PostProperty = DependencyProperty.Register(
            "Post",
            typeof(Post), typeof(PostControl),
            PropertyMetadata.Create(() => new Post(), PostPropertyChanged));

        public static DependencyProperty IsSimpleProperty = DependencyProperty.Register(
            "IsSimple",
            typeof(bool), typeof(PostControl),
            PropertyMetadata.Create(false));

        private static void PostPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            dependencyObject.SetValue(DataContextProperty, e.NewValue);
        }

        private readonly ISettingsService _settings = SettingsService.Instance;

        public delegate void PostReplyEventHandler(object sender, PostReplyEventArgs e);

        public event PostReplyEventHandler Reply = delegate { };

        public delegate void ImageClickEventHandler(object sender, ImageClickEventArgs e);

        public event ImageClickEventHandler ImageClick = delegate { };

        public event Action<object, ReplyShowEventArgs> ReplyShowRequested = delegate { };

        public event Action<Post> RepliesListShowRequested = delegate { };

        public bool ShowRepliesAsTree =>
            _settings.RepliesViewMode == RepliesViewMode.Tree ||
            (_settings.RepliesViewMode == RepliesViewMode.Auto && new MouseCapabilities().MousePresent > 0);

        public PostControl()
        {
            this.InitializeComponent();
        }

        public Post Post
        {
            get { return (Post)GetValue(PostProperty); }
            set { SetValue(PostProperty, value); }
        }

        public bool IsSimple
        {
            get { return (bool) GetValue(IsSimpleProperty); }
            set { SetValue(IsSimpleProperty, value); }
        }

        private void PostNum_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Reply(this, new PostReplyEventArgs(PostText.SelectedText, Post));
        }

        private void ImagesGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ImageClick(this, new ImageClickEventArgs((ImageInfo) e.ClickedItem));
        }

        private void ReplyNum_OnPointerAction(object sender, PointerRoutedEventArgs e)
        {
            var elem = (FrameworkElement) e.OriginalSource;
            var post = (Post) elem.DataContext;
            ReplyShowRequested(sender, new ReplyShowEventArgs(Post, post, e));
        }

        private void RepliesButton_OnClick(object sender, RoutedEventArgs e)
        {
            RepliesListShowRequested(Post);
        }
    }


    public class ReplyShowEventArgs
    {
        public ReplyShowEventArgs(Post parent, Post post, PointerRoutedEventArgs pointerEventArgs)
        {
            Parent = parent;
            Post = post;
            PointerEventArgs = pointerEventArgs;
        }

        public PointerRoutedEventArgs PointerEventArgs { get; }
        public Post Parent { get; }
        public Post Post { get; }
    }

    public class PostReplyEventArgs
    {
        public PostReplyEventArgs(string selectedText, Post post)
        {
            SelectedText = selectedText;
            Post = post;
        }

        public string SelectedText { get; }
        public Post Post { get; }
    }

    public class ImageClickEventArgs
    {
        public ImageClickEventArgs(ImageInfo imageInfo)
        {
            ImageInfo = imageInfo;
        }

        public ImageInfo ImageInfo { get; }
    }
}
