using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Win2ch.Services.Storage {
    public abstract class ItemsRepositoryServiceBase<T> : IItemsRepositoryService<T> {
        protected ItemsRepositoryServiceBase(IStorageService<ICollection<T>> storageService,
                                             StorageFolder baseFolder,
                                             string fileName) {
            StorageService = storageService;
            BaseFolder = baseFolder;
            FileName = fileName;
            Items = new HashSet<T>();
        }

        protected virtual IStorageService<ICollection<T>> StorageService { get; }
        protected virtual StorageFolder BaseFolder { get; }
        protected virtual string FileName { get; }

        public ICollection<T> Items { get; protected set; }

        public async Task Load() {
            Items = await StorageService.Load(BaseFolder, FileName) ?? CreateCollection();
        }

        public async Task Save() {
            await StorageService.Save(BaseFolder, FileName, Items);
        }

        protected virtual ICollection<T> CreateCollection() {
            return new HashSet<T>();
        } 
    }
}