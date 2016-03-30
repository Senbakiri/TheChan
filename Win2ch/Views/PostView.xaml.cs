using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Core.Common;
using Win2ch.Annotations;
using Win2ch.Behaviors;
using Win2ch.Common;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class PostView {

        public PostView(IShell shell, IBoard board) {
            Shell = shell;
            Board = board;
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as PostViewModel;
            ShowRepliesAsRibbon = true;
        }

        public PostViewModel ViewModel { get; private set; }

        public bool ShowRepliesAsRibbon { get; private set; }

        public IShell Shell { get; }

        public IBoard Board { get; }
        

        private void HtmlBehaviorOnPostClick(object sender, PostClickEventArgs e) {
            ViewModel.DisplayPost(e.Link);
        }
    }
}
