using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Template10.Mvvm;
using Win2ch.Models;
using Win2ch.Models.Exceptions;
using Win2ch.Views;
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
                _PostInfo.Comment = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsWorking;

        public bool IsWorking
        {
            get { return _IsWorking; }
            set
            {
                _IsWorking = value;
                RaisePropertyChanged();
            }
        }

        private string _JobStatus;
        private NewPostInfo _PostInfo = new NewPostInfo();

        public string JobStatus
        {
            get { return _JobStatus; }
            set
            {
                _JobStatus = value;
                RaisePropertyChanged();
            }
        }

        public NewPostInfo PostInfo
        {
            get { return _PostInfo; }
            set
            {
                _PostInfo = value;
                FastReplyText = value.Comment;
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand FastReplyCommand { get; }
        public ICommand ShowImageCommand { get; }
        public ICommand AdvancedPostingCommand { get; }


        public ThreadViewModel()
        {
            RefreshCommand = new DelegateCommand(Refresh);
            FastReplyCommand = new DelegateCommand(SendPost);
            ShowImageCommand = new DelegateCommand<ImageInfo>(ShowImage);
            AdvancedPostingCommand = new DelegateCommand(AdvancedPosting);
        }


        private void ShowImage(ImageInfo imageInfo)
        {
            NavigationService.Navigate(typeof (ImagesViewPage),
                new Tuple<ImageInfo, List<ImageInfo>>(imageInfo, Posts.SelectMany(p => p.Images).ToList()));
        }


        private async void SendPost()
        {
            try
            {
                await Thread.Reply(PostInfo);
                FastReplyText = string.Empty;
                Refresh();
            }
            catch (ApiException e)
            {
                await new MessageDialog(e.Message, "Ошибка").ShowAsync();
            }
            
        }

        private void AdvancedPosting()
        {
            NavigationService.Navigate(typeof (PostingPage), new PostingPageNavigationInfo
            {
                PostInfo = PostInfo,
                Thread = Thread
            });
        }

        public async void Refresh()
        {
            IsWorking = true;

            JobStatus = "Получение новых постов";
            var newPosts = await Thread.GetPostsFrom(Thread.Posts.Count + 1);

            if (newPosts.Count > 0)
            {

                JobStatus = "Обработка";
                Thread.Posts.AddRange(newPosts);
                foreach (var newPost in newPosts)
                    Posts.Add(newPost);
                JobStatus = $"Получено новых постов: {newPosts.Count}";
            }
            else
            {
                JobStatus = "Нет новых постов";
            }

            await Task.Delay(2000);
            IsWorking = false;
        }

        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            var thread = parameter as Thread;
            if (thread != Thread)
            {
                LoadThread(thread);
            }
            
            FastReplyText = PostInfo.Comment;
        }

        private async void LoadThread(Thread thread)
        {
            Posts.Clear();
            var posts = await thread.GetPostsFrom(1);

            Title = thread.Name ?? "";
            if (Title.Length == 0)
                Title = "Просмотр треда";

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
