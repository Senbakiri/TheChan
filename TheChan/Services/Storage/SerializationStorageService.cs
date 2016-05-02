using System;
using System.Threading.Tasks;
using Windows.Storage;
using Template10.Services.SerializationService;

namespace TheChan.Services.Storage {
    public class SerializationStorageService : IStorageService {
        public SerializationStorageService(ISerializationService serializationService) {
            SerializationService = serializationService;
        }

        private ISerializationService SerializationService { get; }

        public async Task<object> Load(StorageFolder root, string fileName) {
            var file = await root.TryGetItemAsync(fileName) as IStorageFile;
            if (file == null)
                return null;

            string text = await FileIO.ReadTextAsync(file);
            return Deserialize(text);
        }

        public async Task<T> Load<T>(StorageFolder root, string fileName) {
            return (T) await Load(root, fileName);
        }

        protected virtual object Deserialize(string source) {
            return SerializationService.Deserialize(source);
        }

        protected virtual string Serialize<T>(T source) {
            return SerializationService.Serialize(source);
        }


        public async Task Save(StorageFolder root, string fileName, object content) {
            StorageFile file = await root.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, Serialize(content));
        }
    }
}