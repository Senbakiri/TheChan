using System.Collections.Generic;
using System.Threading.Tasks;

namespace Win2ch.Services.Storage {
    public interface IItemsRepositoryService<T> {
        ICollection<T> Items { get; }
        Task Load();
        Task Save();
    }
}