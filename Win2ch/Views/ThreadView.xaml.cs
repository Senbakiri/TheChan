using System.Linq;
using Windows.UI.Xaml.Controls;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class ThreadView {
        public ThreadView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as ThreadViewModel;
        }

        public ThreadViewModel ViewModel { get; private set; }

        public void Up() {
            PostViewModel firstPost = ViewModel?.Posts.FirstOrDefault();
            if (firstPost != null)
                this.Posts.ScrollIntoView(firstPost, ScrollIntoViewAlignment.Leading);
        }

        public void Down() {
            PostViewModel lastPost = ViewModel?.Posts.LastOrDefault();
            if (lastPost != null)
                this.Posts.ScrollIntoView(lastPost, ScrollIntoViewAlignment.Leading);
        }
    }
}
