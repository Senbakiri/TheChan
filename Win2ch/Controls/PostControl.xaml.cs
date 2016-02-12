﻿using System;
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

namespace Win2ch.Controls {
    public sealed partial class PostControl : Page {


        public static DependencyProperty PostProperty = DependencyProperty.Register(
            "Post",
            typeof(Post), typeof(PostControl),
            PropertyMetadata.Create(() => new Post(), PostPropertyChanged));

        public static DependencyProperty IsSimpleProperty = DependencyProperty.Register(
            "IsSimple",
            typeof(bool), typeof(PostControl),
            PropertyMetadata.Create(false));

        private static void PostPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
            dependencyObject.SetValue(DataContextProperty, e.NewValue);
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

        public delegate void PostReplyEventHandler(object sender, PostReplyEventArgs e);

        public event PostReplyEventHandler Reply = delegate { };

        public delegate void ImageClickEventHandler(object sender, ImageClickEventArgs e);

        public event ImageClickEventHandler ImageClick = delegate { };

        public event EventHandler<ReplyShowEventArgs> ReplyShowRequested = delegate { };

        public event Action<Post> RepliesListShowRequested = delegate { };

        public event EventHandler<ParentPostShowEventArgs> ParentPostShowRequested = delegate { };

        public bool ShowRepliesAsTree =>
            _settings.RepliesViewMode == RepliesViewMode.Tree ||
            (_settings.RepliesViewMode == RepliesViewMode.Auto && new MouseCapabilities().MousePresent > 0);

        public PostControl() {
            InitializeComponent();
        }

        public Post Post {
            get { return (Post)GetValue(PostProperty); }
            set { SetValue(PostProperty, value); }
        }

        public bool IsSimple {
            get { return (bool)GetValue(IsSimpleProperty); }
            set { SetValue(IsSimpleProperty, value); }
        }

        private void PostNum_OnTapped(object sender, TappedRoutedEventArgs e) {
            Reply(this, new PostReplyEventArgs(PostText.SelectedText, Post));
        }

        private void ImagesGridView_OnItemClick(object sender, ItemClickEventArgs e) {
            ImageClick(this, new ImageClickEventArgs((ImageInfo)e.ClickedItem));
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

    public class ImageClickEventArgs : EventArgs {
        public ImageClickEventArgs(ImageInfo imageInfo) {
            ImageInfo = imageInfo;
        }

        public ImageInfo ImageInfo { get; }
    }
}
