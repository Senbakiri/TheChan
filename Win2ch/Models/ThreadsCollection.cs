using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;
using Win2ch.Models.Exceptions;
using Win2ch.Views;

namespace Win2ch.Models {
    public class ThreadsCollection : ObservableCollection<Thread>, ISupportIncrementalLoading {
        public Board Board { get; set; }

        public bool HasMoreItems { get; private set; } = true;

        private int _lastPage;

        private CoreDispatcher _dispatcher;

        public delegate void BoardLoadErrorHandler(HttpException exception);

        public event BoardLoadErrorHandler BoardLoadError = delegate { };

        public ThreadsCollection(Board board) {
            Board = board;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count) {
            _dispatcher = Window.Current.Dispatcher;
            return Task.Run(LoadThreads).AsAsyncOperation();
        }

        public void Refresh() {
            _lastPage = 0;
            Clear();
        }

        private async Task<LoadMoreItemsResult> LoadThreads() {
            uint resultCount = 0;

            try {
                var result = await Board.GetThreads(_lastPage);
                ++_lastPage;
                resultCount = (uint)result.Count;
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Fill(result));
            } catch (COMException) {
                if (_lastPage == 0)
                    throw;

                HasMoreItems = false;
            } catch (HttpException e) {
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => BoardLoadError(e));
                HasMoreItems = false;
            }

            return new LoadMoreItemsResult { Count = resultCount };
        }

        private void Fill(List<Thread> threads) {
            var isMobile = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";
            foreach (var thread in threads) {
                if (isMobile) {
                    thread.Posts.RemoveRange(1, thread.Posts.Count - 1);
                    thread.TotalPosts -= 3;
                    Thread.FillPosts(thread.Posts, Board);
                }
                Add(thread);
            }
        }
    }
}
