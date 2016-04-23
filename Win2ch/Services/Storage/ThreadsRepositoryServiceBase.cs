using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Core.Common.Links;
using Core.Models;
using Win2ch.Extensions;

namespace Win2ch.Services.Storage {
    public abstract class ThreadsRepositoryServiceBase : ItemsRepositoryServiceBase<ThreadInfo>, IThreadsRepositoryService {


        protected ThreadsRepositoryServiceBase(IStorageService<IList<ThreadInfo>> storageService,
                                               StorageFolder baseFolder,
                                               string fileName)
            : base(storageService, baseFolder, fileName) { }

        public ThreadInfo GetThreadInfo(ThreadLink threadLink) {
            return
                Items.FirstOrDefault(t => t.BoardId.EqualsNc(threadLink.BoardId) && t.Number == threadLink.ThreadNumber);
        }
    }
}