using Caliburn.Micro;
using Win2ch.Core;

namespace Win2ch.ViewModels {
    internal sealed class HomeViewModel : Tab {
        private static int _pagesOpened;

        public int Number { get; }

        public HomeViewModel() {
            _pagesOpened++;
            Number = _pagesOpened;
            DisplayName = $"Page #{Number}";
        }

        protected override void OnDeactivate(bool close) {
            base.OnDeactivate(close);
            if (close)
                _pagesOpened--;
        }

        public void OpenNewPage() {
            IoC.Get<IShell>().Navigate<HomeViewModel>();
        }
    }
}
