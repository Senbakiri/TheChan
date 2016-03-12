using System;

namespace Win2ch.Common {
    public interface IActivateWithParameter {
        /// <summary>
        /// Indicates whether or not this instance is active.
        /// 
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Raised after activation occurs.
        /// 
        /// </summary>
        event EventHandler<ActivationWithParameterEventArgs> Activated;

        /// <summary>
        /// Activates this instance.
        /// 
        /// </summary>
        void Activate(object parameter);
    }
}