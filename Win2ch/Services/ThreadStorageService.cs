using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Win2ch.Models;

namespace Win2ch.Services {
    public class ThreadStorageService : StorageService<StoredThreadInfo> {
        public ThreadStorageService(StorageFolder rootFolder, string fileName)
            : base(rootFolder, fileName) {}
       
        public async Task<bool> AddThread(Thread thread) {
            return await Add(new StoredThreadInfo(thread));
        }
        
        public async Task<bool> ContainsThread(Thread thread) {
            return await ContainsItem(new StoredThreadInfo(thread));
        }

        public async Task<bool> UpdateThread(Thread thread) {
            var threads = await GetItems();
            var fav = threads.FirstOrDefault(f => Equals(f, thread));
            if (fav == null)
                return false;
            var posts = await thread.GetPostsFrom(fav.LastPostPosition + 1);
            fav.UnreadPosts += posts.Count;
            fav.LastPostPosition += posts.Count;
            return true;
        }

        public async Task ResetThread(Thread thread) {
            var threads = await GetItems();
            var fav = threads.FirstOrDefault(f => Equals(f, thread));
            if (fav == null)
                return;
            fav.UnreadPosts = 0;
            fav.LastPostPosition = thread.Posts.Count;
            await Store();
        }

        public async Task<bool> RemoveThread(Thread thread) {
            var threads = await GetItems();

            var favoriteThread = thread as StoredThreadInfo;
            var favThread = favoriteThread ?? threads.FirstOrDefault(t => t.Equals(thread));
            var removed = await RemoveItem(favThread);
            return removed;
        }
    }


}
