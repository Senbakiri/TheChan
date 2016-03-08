using Caliburn.Micro;
using Win2ch.Core;

namespace Win2ch.ViewModels {
    internal sealed class ShellViewModel : Conductor<Tab>.Collection.OneActive {
        public ShellViewModel() {
            ActivateItem(new HomeViewModel());
        }

        public void CloseTab(Tab tab) {
            DeactivateItem(tab, true);
        }
    }
}
