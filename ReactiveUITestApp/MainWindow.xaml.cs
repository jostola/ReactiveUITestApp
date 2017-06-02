using ReactiveUI;
using System.Windows;

namespace ReactiveUITestApp {
    public partial class MainWindow : IViewFor<MainWindowViewModel> {
        public MainWindow() {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();

            this.WhenActivated(d => {
                d(this.Bind(this.ViewModel, vm => vm.TheText, v => v.TheTextBox.Text));
                d(this.OneWayBind(this.ViewModel, vm => vm.SearchResults, v => v.SearchResults.ItemsSource));
                d(this.OneWayBind(this.ViewModel, vm => vm.IsSearching, v => v.SearchResults.Visibility, sel => sel ? Visibility.Collapsed : Visibility.Visible));
                d(this.OneWayBind(this.ViewModel, vm => vm.SearchEventUserInformation, v => v.UserInformationLabel.Content));
                d(this.OneWayBind(this.ViewModel, vm => vm.SearchEventUserInformation, v => v.UserInformationLabel.Visibility, sel => sel == null ? Visibility.Collapsed : Visibility.Visible));
            });
        }

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (MainWindowViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainWindowViewModel), typeof(MainWindow));

        public MainWindowViewModel ViewModel {
            get { return (MainWindowViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
