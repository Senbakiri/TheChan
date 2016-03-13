using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;

namespace Win2ch.Common {
    public class Tab : ViewAware, IHaveDisplayName, IActivateWithParameter, IDeactivate, IGuardClose, IChild {
        private bool isActive;
        private bool isInitialized;
        private object parent;
        private string displayName;
        private bool isCloseable = true;
        private Image icon;
        private string badgeContent;
        private bool isLoading;
        private readonly ResourceLoader resourceLoader = new ResourceLoader();

        protected Tab() {
            DisplayName = GetType().FullName;
        }

        public string BadgeContent {
            get { return badgeContent; }
            protected set {
                if (value == badgeContent)
                    return;
                badgeContent = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsLoading {
            get { return isLoading; }
            protected set {
                if (value == isLoading)
                    return;
                isLoading = value;
                NotifyOfPropertyChange();
            }
        }

        public Image Icon {
            get { return icon; }
            protected set {
                if (Equals(value, icon))
                    return;
                icon = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsCloseable {
            get { return isCloseable; }
            protected set {
                if (value == isCloseable)
                    return;
                isCloseable = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or Sets the Display Name
        /// </summary>
        public string DisplayName {
            get { return displayName; }
            set {
                if (value == displayName)
                    return;
                displayName = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or Sets the Parent
        /// </summary>
        public object Parent {
            get { return parent; }
            set {
                if (Equals(value, parent))
                    return;
                parent = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Indicates whether or not this instance is active.
        /// </summary>
        public bool IsActive {
            get { return isActive; }
            private set {
                if (value == isActive)
                    return;
                isActive = value;
                NotifyOfPropertyChange();
            }
        }

        public event EventHandler<ActivationWithParameterEventArgs> Activated;

        public bool IsInitialized {
            get { return isInitialized; }
            private set {
                if (value == isInitialized)
                    return;
                isInitialized = value;
                NotifyOfPropertyChange();
            }
        }

        void IActivateWithParameter.Activate(object parameter) {
            if (IsActive) {
                return;
            }

            var initialized = false;

            if (!IsInitialized) {
                IsInitialized = initialized = true;
                OnInitialize();
            }

            IsActive = true;
            OnActivate(parameter);
            Activated?.Invoke(this, new ActivationWithParameterEventArgs(parameter, initialized));
        }

        protected virtual void OnInitialize() { }

        protected virtual void OnActivate(object parameter) { }

        /// <summary>
        /// Deactivates this instance.
        /// </summary>
        /// <param name="close">Indicates whether or not this instance is being closed.</param>
        void IDeactivate.Deactivate(bool close) {
            if (IsActive || (IsInitialized && close)) {
                AttemptingDeactivation?.Invoke(this, new DeactivationEventArgs {
                    WasClosed = close
                });

                IsActive = false;
                OnDeactivate(close);

                Deactivated?.Invoke(this, new DeactivationEventArgs {
                    WasClosed = close
                });

                if (close) {
                    Views.Clear();
                }
            }
        }

        protected virtual void OnDeactivate(bool close) { }

        /// <summary>
        /// Raised before deactivation.
        /// </summary>
        public event EventHandler<DeactivationEventArgs> AttemptingDeactivation;

        /// <summary>
        /// Raised after deactivation.
        /// </summary>
        public event EventHandler<DeactivationEventArgs> Deactivated;

        /// <summary>
        /// Tries to close this instance.
        /// Also provides an opportunity to pass a dialog result to it's corresponding view.
        /// </summary>
        /// <param name="dialogResult">The dialog result.</param>
        void IClose.TryClose(bool? dialogResult) {
            PlatformProvider.Current.GetViewCloseAction(this, Views.Values, dialogResult).OnUIThread();
        }

        /// <summary>
        /// Called to check whether or not this instance can close.
        /// </summary>
        /// <param name="callback">The implementer calls this action with the result of the close check.</param>
        public virtual void CanClose(Action<bool> callback) {
            callback(true);
        }

        protected string GetLocalizationString(string id) {
            string currentTypeName = GetType().Name;
            string baseId = currentTypeName;
            if (currentTypeName.Contains("ViewModel"))
                baseId = currentTypeName.Remove(currentTypeName.IndexOf("ViewModel", StringComparison.Ordinal));
            baseId += $"/{id.Replace('.', '/')}";
            return resourceLoader.GetString(baseId);
        }
    }
}
