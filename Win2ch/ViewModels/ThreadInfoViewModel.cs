using Caliburn.Micro;
using Core.Models;

namespace Win2ch.ViewModels {
    public class ThreadInfoViewModel : PropertyChangedBase {
        public ThreadInfoViewModel(ThreadInfo threadInfo) {
            ThreadInfo = threadInfo;
        }

        public ThreadInfo ThreadInfo { get; private set; }

        public int LastReadPostPosition {
            get { return ThreadInfo.LastReadPostPosition; }
            set {
                if (value == ThreadInfo.LastReadPostPosition)
                    return;
                ThreadInfo.LastReadPostPosition = value;
                NotifyOfPropertyChange();
            }
        }

        public int UnreadPosts {
            get { return ThreadInfo.UnreadPosts; }
            set {
                if (value == ThreadInfo.UnreadPosts)
                    return;
                ThreadInfo.UnreadPosts = value;
                NotifyOfPropertyChange();
            }
        }
    }
}