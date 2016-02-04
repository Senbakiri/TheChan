using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace Win2ch.Services {
    class CacheService {
        private CacheService() { }

        public static CacheService Instance { get; } = new CacheService();

        private StorageFolder Root { get; } = ApplicationData.Current.LocalCacheFolder;

        public async Task<StorageFolder> GetFolder(CacheItemType type) {
            StorageFolder folder;
            try {
                folder = await Root.GetFolderAsync(type.ToString());
            } catch (Exception) {
                folder = await Root.CreateFolderAsync(type.ToString());
            }

            return folder;
        }

        public async Task<StorageFile> DownloadAndCacheItem(string url, CacheItemType type, string name = null) {
            var fileName = string.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString() : name;
            var destination = await GetFolder(type);

            var existingFile = await destination.TryGetItemAsync(fileName);
            if (existingFile != null)
                return existingFile as StorageFile;

            var client = new HttpClient();
            var uri = new Uri(url);
            var response = await client.GetAsync(uri);
            var newFile = await destination.CreateFileAsync(fileName);
            var opened = await newFile.OpenAsync(FileAccessMode.ReadWrite);
            await response.Content.WriteToStreamAsync(opened);
            opened.Dispose();
            return newFile;
        }
    }
}
