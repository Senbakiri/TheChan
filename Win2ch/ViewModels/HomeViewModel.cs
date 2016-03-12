using Win2ch.Common;

namespace Win2ch.ViewModels {
    internal sealed class HomeViewModel : Tab {
        private static int pagesOpened;
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
            pagesOpened++;
            Number = pagesOpened;
            DisplayName = $"Page #{Number}";
        }

        protected override void OnActivate(object parameter) {
            if (parameter != null)
                Argument = parameter.ToString();
        }

        protected override void OnDeactivate(bool close) {
            base.OnDeactivate(close);
            if (close)
                pagesOpened--;
        }

        public void OpenNewPage() {
            Shell.Navigate<HomeViewModel>($"Opened from {DisplayName}");
        }
    }
}
