using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Win2ch.Annotations;
using Win2ch.Models;
using Win2ch.ViewModels;
using Win2ch.Views;

namespace Win2ch.Mvvm {
    public abstract class ViewModelBase : Template10.Mvvm.ViewModelBase {
        private static readonly TelemetryClient TelemetryClient = new TelemetryClient();

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state) {
            if (mode == NavigationMode.Back)
                return base.OnNavigatedToAsync(parameter, mode, state);

            var thread = parameter is Thread
                ? (Thread) parameter
                : (parameter as NavigationToThreadWithScrolling)?.Thread;

            var info = (parameter as Board)?.Id ??
                       (string.IsNullOrEmpty(thread?.Name)
                        ? thread?.Num.ToString()
                        : thread.Name);

            TelemetryClient.TrackPageView(new PageViewTelemetry {
                Name = NavigationService.CurrentPageType.Name,
                Properties = {["Info"] = info}
            });

            return base.OnNavigatedToAsync(parameter, mode, state);
        }

        [NotifyPropertyChangedInvocator]
        public override void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            base.RaisePropertyChanged(propertyName: propertyName);
        }
    }
}
