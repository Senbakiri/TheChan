using Caliburn.Micro;
using Makaba;
using Ninject;
using Win2ch.Common;

namespace Win2ch.ViewModels {
    internal sealed class ShellViewModel : Conductor<Tab>.Collection.OneActive, IShell {

        protected override void OnInitialize() {
            Navigate<HomeViewModel>();
        }

        public void CloseTab(Tab tab) {
            DeactivateItem(tab, true);
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
