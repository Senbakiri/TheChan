using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Template10.Mvvm;
using Win2ch.Common;
using Win2ch.Models;
using Win2ch.Models.Exceptions;
using Win2ch.Services;
using Win2ch.Views;
using ViewModelBase = Win2ch.Mvvm.ViewModelBase;

namespace Win2ch.ViewModels {
    public class ThreadViewModel : ViewModelBase {
        public ObservableCollection<Post> Posts { get; } = new ObservableCollection<Post>();

        public Thread Thread { get; private set; }

        private string _Title;
        public string Title {
            get { return _Title; }
            set {
                _Title = value;
                RaisePropertyChanged();
            }
        }

        private string _FastReplyText;
        public string FastReplyText {
            get { return _FastReplyText; }
            set {
                _FastReplyText = value;
                _PostInfo.Comment = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsWorking;

        public bool IsWorking {
            get { return _IsWorking; }
            set {
                _IsWorking = value;
                RaisePropertyChanged();
            }
        }

        private string _JobStatus;
        private NewPostInfo _PostInfo = new NewPostInfo();
        private bool _IsInFavorites;
        private int _HighlightedPostsStart;
        private bool _HighlightPosts;

        public string JobStatus {
            get { return _JobStatus; }
            set {
                _JobStatus = value;
                RaisePropertyChanged();
            }
        }

        public NewPostInfo PostInfo {
            get { return _PostInfo; }
            set {
                _PostInfo = value;
                FastReplyText = value.Comment;
            }
        }

        public bool IsInFavorites {
            get { return _IsInFavorites; }
            set {
                _IsInFavorites = value;
                RaisePropertyChanged();
            }
        }

        public int HighlightedPostsStart {
            get { return _HighlightedPostsStart; }
            private set {
                if (value == _HighlightedPostsStart)
                    return;
                _HighlightedPostsStart = value;
                RaisePropertyChanged();
            }
        }

        public bool HighlightPosts {
            get { return _HighlightPosts; }
            private set {
                if (value == _HighlightPosts)
                    return;
                _HighlightPosts = value;
                RaisePropertyChanged();
            }
        }

        public ICommand AdvancedPostingCommand { get; }

        public ICanScrollToItem<Post> PostScroller { get; set; }

        public ThreadViewModel() {
            AdvancedPostingCommand = new DelegateCommand(AdvancedPosting);
        }

        public async void Favorite() {
            try {
                var favService = FavoritesService.Instance.Threads;
                var isAdded = await favService.AddThread(Thread);
                if (!isAdded)
                    await favService.RemoveThread(Thread);
                IsInFavorites = await favService.ContainsThread(Thread);
            } catch (Exception e) {
                await Utils.ShowOtherError(e, "Не удалось удалить тред из избранного или добавить его");
            }
        }

        public async Task<bool> FastReply() {
            try {
                IsWorking = true;
                JobStatus = "Отправка";
                await Thread.Reply(PostInfo);
                FastReplyText = string.Empty;
                return await Refresh(true);
            } catch (ApiException e) {
                await Utils.ShowOtherError(e, "Ошибка");
            } catch (HttpException e) {
                await Utils.ShowHttpError(e, "Ошибка");
            } catch (COMException e) {
                await Utils.ShowConnectionError(e, "Ошибка");
            } finally {
                IsWorking = false;
            }

            return false;
        }

        private void AdvancedPosting() {
            CurrentPost.PostInfo = PostInfo;
            NavigationService.Navigate(typeof(PostingPage), new PostingPageNavigationInfo {
                Thread = new Thread(Thread.Num, Thread.Board.Id)
            });
        }

        public async Task<bool> Refresh(bool scrollToLastPost = false) {
            HighlightPosts = false;
            IsWorking = true;
            JobStatus = "Получение новых постов";
            List<Post> newPosts;

            try {
                newPosts = await Thread.GetPostsFrom(Thread.Posts.Count + 1);
            } catch (ApiException e) {
                await Utils.ShowOtherError(e, "Сервер вернул ошибку");
                IsWorking = false;
                return false;
            } catch (HttpException e) {
                await Utils.ShowHttpError(e, "Не удалось получить посты");
                IsWorking = false;
                return false;
            } catch (COMException e) {
                await Utils.ShowConnectionError(e, "Не удалось получить посты");
                IsWorking = false;
                return false;
            }

            if (newPosts.Count > 0) {
                HighlightedPostsStart = Posts.Count + 1;
                HighlightPosts = true;
                JobStatus = "Обработка";
                Thread.Posts.AddRange(newPosts);
                foreach (var newPost in newPosts)
                    Posts.Add(newPost);
                if (scrollToLastPost)
                    PostScroller?.ScrollToItem(Posts.LastOrDefault());
                JobStatus = $"Получено новых постов: {newPosts.Count}";
            } else {
                JobStatus = "Нет новых постов";
            }

            HighlightedPostsStart = Posts.Count;
            HighlightPosts = true;

            await Task.Delay(2000);
            IsWorking = false;
            return newPosts.Count > 0;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state) {
            var favThreadsService = FavoritesService.Instance.Threads;

            if (CurrentPost.PostInfo != null)
                PostInfo = CurrentPost.PostInfo;

            var navigationInfo = (ThreadNavigationInfo) parameter;
            if (navigationInfo.ForceRefresh || !Equals(navigationInfo.Thread, Thread)) {
                await LoadThread(navigationInfo.Thread);
                if (Posts.Count > 0 && !navigationInfo.PostNum.HasValue)
                    PostScroller?.ScrollToItem(Posts.First());
            }

            if (navigationInfo.PostNum.HasValue) {
                var post = Posts.FirstOrDefault(p => p.Position == navigationInfo.PostNum.Value);
                if (post != null) {
                    PostScroller?.ScrollToItem(post);
                    HighlightPosts = navigationInfo.Highlight;
                    HighlightedPostsStart = navigationInfo.PostNum.Value;
                }
            }

            try {
                IsInFavorites = await favThreadsService.ContainsThread(Thread);
            } catch (Exception e) {
                Utils.TrackError(e);
            }

            try {
                await RecentThreadsService.Instance.AddThread(Thread);
            } catch (Exception e) {
                await Utils.ShowOtherError(e, "Не удалось добавить тред в недавние");
            }

            FastReplyText = PostInfo.Comment;
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        private async Task LoadThread(Thread thread) {
            Posts.Clear();
            Thread = thread;
            Title = "Просмотр треда";

            try {
                var posts = await thread.GetPostsFrom(1);
                if (!string.IsNullOrEmpty(thread.Name))
                    Title = thread.Name;

                Thread.Posts = posts;

                foreach (var post in posts) {
                    Posts.Add(post);
                }
            } catch (ApiException e) {
                await Utils.ShowOtherError(e, "Сервер вернул ошибку");
            } catch (HttpException e) {
                await Utils.ShowHttpError(e, "Не удалось загрузить тред.");
            }
            catch (COMException e) {
                await Utils.ShowConnectionError(e, "Не удалось загрузить тред");
            }
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending) {
            try {
                var favsService = FavoritesService.Instance.Threads;
                var recentService = RecentThreadsService.Instance;
                if (await favsService.ContainsThread(Thread))
                    await favsService.ResetThread(Thread);
                if (await recentService.ContainsThread(Thread))
                    await recentService.ResetThread(Thread);
            } catch (Exception e) {
                await Utils.ShowOtherError(e, "Не удалось обновить информацию о треде");
            }

            await base.OnNavigatedFromAsync(state, suspending);
        }
    }

    public class ThreadNavigationInfo {
        public ThreadNavigationInfo(Thread thread, int? postNum = null, bool highlight = false) {
            Thread = new Thread(thread.Num, thread.Board.Id);
            PostNum = postNum;
            Highlight = highlight;
        }
        
        [JsonConstructor]
        private ThreadNavigationInfo() { }

        public ThreadNavigationInfo(long threadNum, string boardId, int? postNum = null, bool highlight = false) {
            Thread = new Thread(threadNum, boardId);
            PostNum = postNum;
            Highlight = highlight;
        }
        
        [JsonProperty]
        public Thread Thread { get; private set; }

        [JsonProperty]
        public int? PostNum { get; private set; }

        [JsonProperty]
        public bool Highlight { get; private set; }

        [JsonProperty]
        public bool ForceRefresh { get; set; }
    }


}
