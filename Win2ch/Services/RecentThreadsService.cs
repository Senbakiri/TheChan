using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Win2ch.Models;

namespace Win2ch.Services {
    public class RecentThreadsService : ThreadStorageService {
        public static RecentThreadsService Instance { get; } = new RecentThreadsService();

        private RecentThreadsService()
            : base(ApplicationData.Current.LocalFolder, "RecentThreads.json") {
        }
    }
}
