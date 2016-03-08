using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Win2ch.ViewModels {
    class ShellViewModel : IConductActiveItem {
        private object _ActiveItem;

        public IEnumerable GetChildren() {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyOfPropertyChange([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Refresh() {
            throw new NotImplementedException();
        }

        public bool IsNotifying { get; set; }

        public void ActivateItem(object item) {
            throw new NotImplementedException();
        }

        public void DeactivateItem(object item, bool close) {
            throw new NotImplementedException();
        }

        public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed;

        public object ActiveItem {
            get { return _ActiveItem; }
            set {
                if (ActiveItem == value)
                    return;
                _ActiveItem = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
