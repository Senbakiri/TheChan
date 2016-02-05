using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
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

        public ICommand AdvancedPostingCommand { get; }

        public ICanScrollToItem<Post> PostScroller { get; set; } 

        public ThreadViewModel() {
            AdvancedPostingCommand = new DelegateCommand(AdvancedPosting);
        }

        public void AddToFavorites() {
            var favService = FavoritesService.Instance;
            favService.AddThread(Thread);
            IsInFavorites = true;
        }

        public async Task<bool> FastReply() {
            try {
                await Thread.Reply(PostInfo);
                FastReplyText = string.Empty;
                return await Refresh();
            } catch (ApiException e) {
                await Utils.ShowOtherError(e, "Ошибка");
            } catch (HttpException e) {
                await Utils.ShowHttpError(e, "Ошибка");
            } catch (COMException e) {
                await Utils.ShowConnectionError(e, "Ошибка");
            }

            return false;
        }

        private void AdvancedPosting() {
            CurrentPost.PostInfo = PostInfo;
            NavigationService.Navigate(typeof(PostingPage), new PostingPageNavigationInfo {
                Thread = new Thread(Thread.Num, Thread.Board.Id)
            });
        }

        public async Task<bool> Refresh() {
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
                JobStatus = "Обработка";
                Thread.Posts.AddRange(newPosts);
                foreach (var newPost in newPosts)
                    Posts.Add(newPost);
                JobStatus = $"Получено новых постов: {newPosts.Count}";
            } else {
                JobStatus = "Нет новых постов";
            }

            await Task.Delay(2000);
            IsWorking = false;
            return newPosts.Count > 0;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state) {
            if (CurrentPost.PostInfo != null)
                PostInfo = CurrentPost.PostInfo;

            if (parameter is Thread) {
                var thread = (Thread) parameter;
                if ( !ReferenceEquals(thread, Thread))
                    await LoadThread(thread);
            } else if (parameter is NavigationToThreadWithScrolling) {
                var nav = (NavigationToThreadWithScrolling) parameter;
                if (!Equals(nav.Thread, Thread))
                    await LoadThread(nav.Thread);

                var post = Posts.FirstOrDefault(p => p.Num == nav.PostNum);
                if (post != null)
                    PostScroller?.ScrollToItem(post);
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
            if (FavoritesService.Instance.IsThreadInFavorites(Thread))
                await FavoritesService.Instance.ResetThread(Thread);
            await base.OnNavigatedFromAsync(state, suspending);
        }
    }

    public class NavigationToThreadWithScrolling {
        public NavigationToThreadWithScrolling(Thread thread, int postNum) {
            Thread = thread;
            PostNum = postNum;
        }

        public Thread Thread { get; }
        public int PostNum { get; }
    }


}
