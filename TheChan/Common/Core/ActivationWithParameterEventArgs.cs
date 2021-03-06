﻿using System;

namespace TheChan.Common.Core {
    public class ActivationWithParameterEventArgs : EventArgs {
        public ActivationWithParameterEventArgs(object parameter, bool wasInitialized) {
            Parameter = parameter;
            WasInitialized = wasInitialized;
        }

        public object Parameter { get; }

        public bool WasInitialized { get; }
    }
}