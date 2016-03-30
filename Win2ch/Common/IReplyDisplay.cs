using Win2ch.ViewModels;
using Win2ch.Views;

namespace Win2ch.Common {
    public interface IReplyDisplay {
        void DisplayReply(ReplyDisplayingEventArgs args);
    }
}