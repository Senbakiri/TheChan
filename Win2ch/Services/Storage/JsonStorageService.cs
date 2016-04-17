using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Newtonsoft.Json;

namespace Win2ch.Services.Storage {
    public class JsonStorageService<T> : IStorageService<T>   {
        public async Task<T> Load(StorageFolder root, string fileName) {
            var file = await root.TryGetItemAsync(fileName) as IStorageFile;
            if (file == null)
                return default(T);

            string text = await FileIO.ReadTextAsync(file);
            return Deserialize(text);
        }

        protected virtual T Deserialize(string source) {
            return JsonConvert.DeserializeObject<T>(source);
        }

        protected virtual string Serialize(T source) {
            return JsonConvert.SerializeObject(source);
        }

        public async Task Save(StorageFolder root, string fileName, T content) {
            StorageFile file = await root.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, Serialize(content));
        }
    }
}