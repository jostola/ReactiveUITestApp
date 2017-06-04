using ReactiveUI;
using ReactiveUITestApp.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace ReactiveUITestApp.View {
    public partial class SearchResultItemView : UserControl, IViewFor<SearchResultItemViewModel> {
        public SearchResultItemView() {
            InitializeComponent();

            this.WhenActivated(d => {
                d(this.OneWayBind(
                    this.ViewModel, 
                    vm => vm.Name, 
                    v => v.Name.Text));

                d(this.OneWayBind(
                    this.ViewModel,
                    vm => vm.Size,
                    v => v.Size.Text));

                d(this.OneWayBind(
                    this.ViewModel,
                    vm => vm.Score,
                    v => v.Score.Text));

                d(this.BindCommand(
                    this.ViewModel,
                    vm => vm.ACommand,
                    v => v.DoubleClickCommand,
                    vm => vm.Name));
            });
        }

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (SearchResultItemViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(SearchResultItemViewModel), typeof(SearchResultItemView));

        public SearchResultItemViewModel ViewModel {
            get { return (SearchResultItemViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
