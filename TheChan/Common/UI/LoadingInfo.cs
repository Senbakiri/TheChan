using Caliburn.Micro;
using Action = System.Action;

namespace TheChan.Common.UI {
    public class LoadingInfo : PropertyChangedBase {
        private string message;
        private LoadingState state;
        private Action tryAgainCallback;
        private bool isTryingAgainEnabled;

        public LoadingState State {
            get { return this.state; }
            set {
                if (value == this.state)
                    return;
                this.state = value;
                NotifyOfPropertyChange();
            }
        }

        public string Message {
            get { return this.message; }
            set {
                if (value == this.message)
                    return;
                this.message = value;
                NotifyOfPropertyChange();
            }
        }


        public bool IsTryingAgainEnabled {
            get { return this.isTryingAgainEnabled; }
            private set {
                if (value == this.isTryingAgainEnabled)
                    return;
                this.isTryingAgainEnabled = value;
                NotifyOfPropertyChange();
            }
        }

        public void InProgress(string message) {
            SetState(LoadingState.InProgress, message);
        }

        public void Error(string message, bool canTryAgain = false, Action tryAgainCallback = null) {
            this.tryAgainCallback = tryAgainCallback;
            IsTryingAgainEnabled = canTryAgain;
            SetState(LoadingState.Error, message);
        }

        public void Success(string message) {
            SetState(LoadingState.Success, message);
        }

        private void SetState(LoadingState state, string message) {
            State = state;
            Message = message;
        }

        public void TryAgain() {
            this.tryAgainCallback?.Invoke();
            IsTryingAgainEnabled = false;
        }
    }
}
