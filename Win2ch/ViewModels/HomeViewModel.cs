using Caliburn.Micro;
using Win2ch.Core;

namespace Win2ch.ViewModels {
    internal sealed class HomeViewModel : Tab {
        private static int _pagesOpened;

        public int Number { get; }
        private IShell Shell { get; }

        public HomeViewModel(IShell shell) {
            Shell = shell;
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
            Shell.Navigate<HomeViewModel>();
        }
    }
}
