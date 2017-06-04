using ReactiveUI;
using System.Linq;

namespace ReactiveUITestApp.ViewModel {
    public class SearchResultItemViewModel : ReactiveObject {
        public SearchResultItemViewModel(string name, string size, string score) {
            this.Size = size;
            this.Score = score;

            this.ACommand = ReactiveCommand.Create<string, string>(
                s => new string(s.ToCharArray().Reverse().ToArray()));

            this._name = ACommand.ToProperty(this, vm => vm.Name, name);
        }

        private readonly ObservableAsPropertyHelper<string> _name;
        public string Name => _name.Value;

        public string Size { get; }

        public string Score { get; }

        public ReactiveCommand<string, string> ACommand { get; }
    }
}
