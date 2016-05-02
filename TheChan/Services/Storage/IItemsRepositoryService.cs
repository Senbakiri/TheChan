using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheChan.Services.Storage {
    public interface IItemsRepositoryService<T> {
        ICollection<T> Items { get; }
        Task Load();
        Task Save();
    }
}