using Caliburn.Micro;

namespace Win2ch.Core {
    public interface IShell {
        void Navigate<T>() where T : Tab;
    }
}