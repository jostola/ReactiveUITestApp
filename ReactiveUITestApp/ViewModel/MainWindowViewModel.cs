using ReactiveUI;
using ReactiveUITestApp.Model;
using ReactiveUITestApp.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactiveUITestApp {
    public class MainWindowViewModel : ReactiveObject {
        public MainWindowViewModel() {
            var canSearch = this.WhenAnyValue(vm => vm.TheText)
                .Select(text => !string.IsNullOrWhiteSpace(text));

            this.Search = ReactiveCommand.CreateFromTask<string, IRestResponse<RepositoryList>>(
                searchTerm => SearchImpl(searchTerm), canSearch);

            this.WhenAnyValue(vm => vm.TheText)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .DistinctUntilChanged()
                .InvokeCommand(this, vm => vm.Search);

            this._searchResults = Observable.Merge(
                    this.Search.Select(response => response.Data.items.Select(repo => new SearchResultItemViewModel(repo.name, repo.size.ToString(), repo.score.ToString()))),
                    this.Search.ThrownExceptions.Select(_ => Enumerable.Empty<SearchResultItemViewModel>())
                )
                .Retry()
                .ToProperty(this, vm => vm.SearchResults);

            this._searchEventUserInformation =
                    Observable.CombineLatest(
                        this.Search.ThrownExceptions.StartWith((Exception)null),
                        this.Search.IsExecuting.StartWith(false),
                        (exception, isExecuting) => {
                            if (exception != null) {
                                return exception.Message;
                            } else if (isExecuting) {
                                return "Wait one moment.";
                            } else if (!this.SearchResults?.Any() ?? false) {
                                return "No results.";
                            } else {
                                return (string)null;
                            }
                        })
                .ToProperty(this, vm => vm.SearchEventUserInformation);
        }

        private async Task<IRestResponse<RepositoryList>> SearchImpl(string searchTerm) {
            var result = await new GithubRestClient().SearchRepositories(searchTerm);

            if (result.StatusCode != HttpStatusCode.OK) {
                throw new Exception("GitHub API request failed. Status description: " + result.StatusDescription);
            }

            return result;
        }

        public ReactiveCommand<string, IRestResponse<RepositoryList>> Search { get; }

        private readonly ObservableAsPropertyHelper<IEnumerable<SearchResultItemViewModel>> _searchResults;
        public IEnumerable<SearchResultItemViewModel> SearchResults => _searchResults.Value;

        private string _theText = "rust";
        public string TheText {
            get { return this._theText; }
            set { this.RaiseAndSetIfChanged(ref this._theText, value); }
        }

        private readonly ObservableAsPropertyHelper<string> _searchEventUserInformation;
        public string SearchEventUserInformation => _searchEventUserInformation.Value;
    }
}