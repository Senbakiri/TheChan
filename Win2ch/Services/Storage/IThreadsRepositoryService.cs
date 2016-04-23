using Core.Common.Links;
using Core.Models;

namespace Win2ch.Services.Storage {
    public interface IThreadsRepositoryService : IItemsRepositoryService<ThreadInfo> {
        ThreadInfo GetThreadInfo(ThreadLink threadLink);
        ThreadInfo GetThreadInfoOrCreate(Thread thread);
    }
}