using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Win2ch.Services.Storage {
    public abstract class ItemsRepositoryServiceBase<T> : IItemsRepositoryService<T> {
        protected ItemsRepositoryServiceBase(IStorageService<IList<T>> storageService,
                                             StorageFolder baseFolder,
                                             string fileName) {
            StorageService = storageService;
            BaseFolder = baseFolder;
            FileName = fileName;
            Items = new List<T>();
        }

        protected virtual IStorageService<IList<T>> StorageService { get; }
        protected virtual StorageFolder BaseFolder { get; }
        protected virtual string FileName { get; }

        public IList<T> Items { get; protected set; }

        public async Task Load() {
            Items = await StorageService.Load(BaseFolder, FileName) ?? new List<T>();
        }

        public async Task Save() {
            await StorageService.Save(BaseFolder, FileName, Items);
        }
    }
}