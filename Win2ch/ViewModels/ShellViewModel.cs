using Caliburn.Micro;
using Win2ch.Core;

namespace Win2ch.ViewModels {
    internal sealed class ShellViewModel : Conductor<Tab>.Collection.OneActive, IShell {
        public void CloseTab(Tab tab) {
            DeactivateItem(tab, true);
        }

        protected override void OnInitialize() {
            Navigate<HomeViewModel>("test");
        }

        public void Navigate<T>(object parameter = null) where T : Tab {
            var item = IoC.Get<T>();
            ActivateItem(item, parameter);
        }

        public override void ActivateItem(Tab item) {
           ActivateItem(item, null);
        }
        
        private void ActivateItem(Tab item, object parameter) {
            if (ActiveItem != item && item != null) {
                (item as IActivateWithParameter).Activate(parameter);
                OnActivationProcessed(item, true);
            }

            ChangeActiveItem(item, false);
        }
    }
}
