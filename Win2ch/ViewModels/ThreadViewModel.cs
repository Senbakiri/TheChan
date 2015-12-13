using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;
using Template10.Mvvm;
using Win2ch.Models;
using ViewModelBase = Win2ch.Mvvm.ViewModelBase;

namespace Win2ch.ViewModels
{
    public class ThreadViewModel : ViewModelBase
    {

        public ObservableCollection<Post> Posts
        { get; } = new ObservableCollection<Post>();

        public Thread Thread { get; private set; }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                RaisePropertyChanged();
            }
        }

        private string _FastReplyText;
        public string FastReplyText
        {
            get { return _FastReplyText; }
            set
            {
                _FastReplyText = value;
                RaisePropertyChanged();
            }
        }


        public ICommand RefreshCommand { get; }
        public ICommand FastReplyCommand { get; }

        public ThreadViewModel()
        {
            RefreshCommand = new DelegateCommand(Refresh);
            FastReplyCommand = new DelegateCommand(SendPost);
        }


        private void SendPost()
        {
            Thread.Reply(new NewPostInfo
            {
                Comment = FastReplyText
            });
        }

        private bool CanSendPost()
        {
            return FastReplyText?.Length > 0 && FastReplyText?.Length <= 15000;
        }

        public async void Refresh()
        {
            var newPosts = await Thread.GetPostsFrom(Thread.Posts.Count + 1);
            Thread.Posts.AddRange(newPosts);
            foreach (var newPost in newPosts)
            {
                Posts.Add(newPost);
            }
        }

        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            var thread = parameter as Thread;
            if (thread != null && mode == NavigationMode.New)
            {
                LoadThread(thread);
            }
        }

        private async void LoadThread(Thread thread)
        {
            Title = thread.Name ?? "";
            if (Title.Length == 0)
                Title = "Просмотр треда";

            Posts.Clear();
            var posts = await thread.GetPostsFrom(1);
            Thread = new Thread
            {
                Board = thread.Board,
                Posts = posts
            };


            foreach (var post in posts)
            {
                Posts.Add(post);
            }
        }

    }


}
