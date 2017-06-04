using ReactiveUI;
using ReactiveUITestApp.View;
using ReactiveUITestApp.ViewModel;
using Splat;
using System.Windows;

namespace ReactiveUITestApp {
    public partial class App : Application {
        public App() {
            Locator.CurrentMutable.Register(() => new SearchResultItemView(), typeof(IViewFor<SearchResultItemViewModel>));
        }
    }
}
