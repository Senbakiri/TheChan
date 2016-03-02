using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Win2ch.Models;

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

        public async Task<StorageFile> DownloadAndCacheAttachment(Attachment attachment) {
            var type = GetCacheItemType(attachment.Type);
            return await DownloadAndCacheItem(attachment.Url, type, attachment.Name);
        }

        private CacheItemType GetCacheItemType(AttachmentType attachmentType) {
            switch (attachmentType) {
                case AttachmentType.Jpg:
                case AttachmentType.Png:
                case AttachmentType.Gif:
                    return CacheItemType.Image;
                case AttachmentType.WebM:
                    return CacheItemType.Video;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attachmentType), attachmentType, null);
            }
        }

        private async Task<ulong> GetFolderSize(StorageFolder folder) {
            var query = folder.CreateFileQueryWithOptions(new QueryOptions {
                FolderDepth = FolderDepth.Deep
            });

            var files = await query.GetFilesAsync();
            ulong size = 0;
            foreach (var file in files) {
                var props = await file.GetBasicPropertiesAsync();
                size += props.Size;
            }

            return size;
        }

        public async Task<ulong> GetCacheItemsSize(CacheItemType type) {
            var folder = await GetFolder(type);
            return await GetFolderSize(folder);
        }

        public async Task<ulong> GetTotalCacheSize() {
            return await GetFolderSize(Root);
        }

        public async Task Clear() {
            var items = await Root.GetItemsAsync();
            foreach (var item in items)
                await item.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }
    }
}
