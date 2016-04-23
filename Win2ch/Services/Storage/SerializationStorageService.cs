using System;
using System.Threading.Tasks;
using Windows.Storage;
using Template10.Services.SerializationService;

namespace Win2ch.Services.Storage {
    public class SerializationStorageService<T> : IStorageService<T> {
        public SerializationStorageService(ISerializationService serializationService) {
            SerializationService = serializationService;
        }

        private ISerializationService SerializationService { get; }

        public async Task<T> Load(StorageFolder root, string fileName) {
            var file = await root.TryGetItemAsync(fileName) as IStorageFile;
            if (file == null)
                return default(T);

            string text = await FileIO.ReadTextAsync(file);
            return Deserialize(text);
        }

        protected virtual T Deserialize(string source) {
            return SerializationService.Deserialize<T>(source);
        }

        protected virtual string Serialize(T source) {
            return SerializationService.Serialize(source);
        }


        public async Task Save(StorageFolder root, string fileName, T content) {
            StorageFile file = await root.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, Serialize(content));
        }
    }
}