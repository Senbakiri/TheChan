using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Win2ch.Annotations;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class PostView : INotifyPropertyChanged {
        private Brush postForeground;

        public PostView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as PostViewModel;
            ShowRepliesAsRibbon = new MouseCapabilities().MousePresent == 0;
        }

        public PostViewModel ViewModel { get; private set; }

        public bool ShowRepliesAsRibbon { get; private set; }

        public Brush PostForeground {
            get { return this.postForeground; }
            set {
                if (Equals(value, this.postForeground))
                    return;

                this.postForeground = value;
                NotifyOfPropertyChange();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void NotifyOfPropertyChange([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
