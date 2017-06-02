using ReactiveUI;
using ReactiveUITestApp.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace ReactiveUITestApp {
    public class MainWindowViewModel : ReactiveObject {
        public MainWindowViewModel() {
            this.Search = ReactiveCommand.CreateFromTask<string, IRestResponse<RepositoryList>>(
                searchTerm => new GithubRestClient().SearchRepositories(searchTerm, null, null));

            this.WhenAnyValue(vm => vm.TheText)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .InvokeCommand(this.Search);

            this._searchResults = this.Search
                .Select(response => response?.Data?.items ?? new List<Repository>())
                .ToProperty(this, vm => vm.SearchResults);

            var searchErrors = 
                Observable.Merge(
                    this.Search.Select(response => (response?.Data?.items != null)),
                    this.Search.ThrownExceptions.Select(_ => false)
                ).Select(isResponseGood => isResponseGood ? (string)null : "An error has occured.");

            var searchActivity = this.Search.IsExecuting
                .Select(isExecuting => isExecuting ? "Wait one moment." : (string)null);

            this._isSearching = this.Search.IsExecuting
                .ToProperty(this, vm => vm.IsSearching);

            this._searchEventUserInformation = Observable.CombineLatest(searchErrors, searchActivity)
                .Select(a => a.FirstOrDefault(s => (s != null)))
                .ToProperty(this, vm => vm.SearchEventUserInformation);
        }

        public ReactiveCommand<string, IRestResponse<RepositoryList>> Search { get; }

        private readonly ObservableAsPropertyHelper<List<Repository>> _searchResults;
        public List<Repository> SearchResults => _searchResults.Value;

        private string _theText = "rust";
        public string TheText {
            get { return this._theText; }
            set { this.RaiseAndSetIfChanged(ref this._theText, value); }
        }

        private readonly ObservableAsPropertyHelper<string> _searchEventUserInformation;
        public string SearchEventUserInformation => _searchEventUserInformation.Value;

        private readonly ObservableAsPropertyHelper<bool> _isSearching;
        public bool IsSearching => _isSearching.Value;
    }
}