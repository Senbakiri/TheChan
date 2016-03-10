using Windows.UI.Xaml;
using Caliburn.Micro;
using Win2ch.Core;

namespace Win2ch.ViewModels {
    internal sealed class HomeViewModel : Tab {
        private static int _pagesOpened = 0;

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
            IoC.Get<ShellViewModel>().ActivateItem(IoC.Get<HomeViewModel>());
        }
    }
}
