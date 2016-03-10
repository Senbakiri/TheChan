using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using Win2ch.Core;

namespace Win2ch.ViewModels {
    internal sealed class ShellViewModel : Conductor<Tab>.Collection.OneActive {
        public ShellViewModel() {
            ActivateItem(IoC.Get<HomeViewModel>());
        }

        public void CloseTab(Tab tab) {
            DeactivateItem(tab, true);
        }
    }
}
