using System.Threading.Tasks;
using Windows.Storage;

namespace Win2ch.Services.Storage {
    public interface IStorageService<T>{
        Task<T> Load(StorageFolder root, string fileName);
        Task Save(StorageFolder root, string fileName, T content);
    }
}