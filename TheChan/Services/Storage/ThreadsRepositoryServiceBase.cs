using System.Linq;
using Windows.Storage;
using Core.Common.Links;
using Core.Models;
using TheChan.Extensions;

namespace TheChan.Services.Storage {
    public abstract class ThreadsRepositoryServiceBase : ItemsRepositoryServiceBase<ThreadInfo>, IThreadsRepositoryService {


        protected ThreadsRepositoryServiceBase(IStorageService storageService,
                                               StorageFolder baseFolder,
                                               string fileName)
            : base(storageService, baseFolder, fileName) { }

        public ThreadInfo GetThreadInfo(ThreadLink threadLink) {
            return
                Items.FirstOrDefault(t => t.BoardId.EqualsNc(threadLink.BoardId) && t.Number == threadLink.ThreadNumber);
        }

        public ThreadInfo GetThreadInfoOrCreate(Thread thread) {
            ThreadInfo threadInfo = GetThreadInfo(thread.GetLink());
            if (threadInfo != null)
                return threadInfo;
            threadInfo = thread.GetThreadInfo();
            return threadInfo;
        }
    }
}