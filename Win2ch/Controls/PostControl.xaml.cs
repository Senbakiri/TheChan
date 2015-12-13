using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Win2ch.Models;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win2ch.Controls
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class PostControl : Page
    {
        public Post Post
        {
            get { return (Post) GetValue(PostProperty); }
            set { SetValue(PostProperty, value); }
        }

        public static DependencyProperty PostProperty = DependencyProperty.Register(
            "Post",
            typeof(Post), typeof(PostControl),
            PropertyMetadata.Create(() => new Post(), PostPropertyChanged));

        private static void PostPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            dependencyObject.SetValue(DataContextProperty, e.NewValue);
        }

        public delegate void PostReplyEventHandler(object sender, PostReplyEventArgs e);
        public event PostReplyEventHandler Reply = delegate { };

        public PostControl()
        {
            this.InitializeComponent();
        }

        private void PostNum_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Reply(this, new PostReplyEventArgs(PostText.SelectedText, Post));
        }
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
}
