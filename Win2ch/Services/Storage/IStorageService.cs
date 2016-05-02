using System.Threading.Tasks;
using Windows.Storage;

namespace Win2ch.Services.Storage {
    public interface IStorageService {
        Task<object> Load(StorageFolder root, string fileName);
        Task<T> Load<T>(StorageFolder root, string fileName);
        Task Save(StorageFolder root, string fileName, object content);
    }
}