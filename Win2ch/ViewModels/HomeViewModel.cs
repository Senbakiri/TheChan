using Win2ch.Core;

namespace Win2ch.ViewModels {
    internal sealed class HomeViewModel : Tab {
        private static int _pagesOpened;
        private string argument;

        public int Number { get; }
        private IShell Shell { get; }

        public string Argument {
            get { return argument; }
            private set {
                if (value == argument)
                    return;
                argument = value;
                NotifyOfPropertyChange();
            }
        }

        public HomeViewModel(IShell shell) {
            Shell = shell;
            _pagesOpened++;
            Number = _pagesOpened;
            DisplayName = $"Page #{Number}";
        }

        protected override void OnActivate(object parameter) {
            Argument = parameter?.ToString();
        }

        protected override void OnDeactivate(bool close) {
            base.OnDeactivate(close);
            if (close)
                _pagesOpened--;
        }

        public void OpenNewPage() {
            Shell.Navigate<HomeViewModel>($"Opened from {DisplayName}");
        }
    }
}
