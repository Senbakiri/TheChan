using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Template10.Services.SerializationService;

namespace Win2ch.Services {
    internal class StorageService<T> {

        public StorageFolder RootFolder { get; set; }
        public string FileName { get; set; }
        private HashSet<T> Items { get; set; } = new HashSet<T>(); 
        private ISerializationService SerializationService { get; } = Template10.Services.SerializationService.SerializationService.Json;

        public bool IsLoaded { get; private set; }

        public StorageService(StorageFolder rootFolder, string fileName) {
            RootFolder = rootFolder;
            FileName = fileName;
        }

        public async Task<bool> Add(T item) {
            if (!IsLoaded)
                await Load();
            var added = Items.Add(item);
            if (added) await Store();
            return added;
        }

        public async Task Store() {
            var json = SerializationService.Serialize(Items);
            await WriteToFile(FileName, json);
        }

        private async Task WriteToFile(string filename, string content) {
            var file = await RootFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, content);

        }

        private async Task<string> ReadFromFile(string filename) {
            var file = await RootFolder.TryGetItemAsync(filename) as IStorageFile;
            if (file == null)
                return null;

            string result;
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            using (var reader = new StreamReader(stream.AsStreamForRead()))
                result = reader.ReadToEnd();
            return result;
        }

        private async Task Load() {
            if (IsLoaded)
                return;

            var threadsJson = await ReadFromFile(FileName);
            if (!string.IsNullOrWhiteSpace(threadsJson))
                Items = SerializationService.Deserialize<HashSet<T>>(threadsJson);

            IsLoaded = true;
        }

        public async Task<bool> ContainsItem(T item) {
            if (!IsLoaded)
                await Load();
            return Items.Contains(item);
        }

        public async Task<IReadOnlyCollection<T>> GetItems() {
            if (!IsLoaded)
                await Load();

            return Items.ToList().AsReadOnly();
        }

        public async Task<bool> RemoveItem(T item) {
            if (!IsLoaded)
                await Load();
            
            var removed = Items.Remove(item);
            if (removed) await Store();
            return removed;
        }
    }
}
