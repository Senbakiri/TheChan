using System.Collections.Generic;
using System.Threading.Tasks;

namespace Win2ch.Services.Storage {
    public interface IItemsRepositoryService<T> {
        IList<T> Items { get; }
        Task Load();
        Task Save();
    }
}