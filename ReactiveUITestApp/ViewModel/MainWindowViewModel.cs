using ReactiveUI;
using ReactiveUITestApp.Model;
using ReactiveUITestApp.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveUITestApp {
    public class MainWindowViewModel : ReactiveObject {
        public MainWindowViewModel() {
            var canSearch = this.WhenAnyValue(vm => vm.TheText)
                .Select(text => !string.IsNullOrWhiteSpace(text));

            this.Search = ReactiveCommand.CreateFromObservable<string, RepositoryList>(
                 searchTerm => SearchImpl(searchTerm).TakeUntil(this.CancelCommand), canSearch);

            this.CancelCommand = ReactiveCommand.Create(() => { }, this.Search.IsExecuting);

            var textChanges = this.WhenAnyValue(vm => vm.TheText)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler);

            textChanges.InvokeCommand(this, vm => vm.CancelCommand);
            textChanges.InvokeCommand(this, vm => vm.Search);

            this._searchResults =
                Observable.Merge(
                    this.Search.Select(data => data.items.Select(ConvertToViewModel)),
                    Observable.Merge(
                        this.Search.ThrownExceptions.Select(_ => Unit.Default),
                        this.Search.IsExecuting.Where(isExecuting => isExecuting).Select(_ => Unit.Default),
                        canSearch.Where(can => !can).Select(_ => Unit.Default))
                        .Select(_ => Enumerable.Empty<SearchResultItemViewModel>()))
                .ToProperty(this, vm => vm.SearchResults);

            this._searchEventUserInformation =
                Observable.Merge(
                    this.WhenAnyValue(vm => vm.TheText).Select(_ => (string)null),
                    canSearch.DistinctUntilChanged().Where(canExecute => !canExecute).Select(_ => "Please input something."),
                    this.Search.IsExecuting.Where(isExecuting => isExecuting).Select(_ => "Wait one moment."),
                    this.Search.IsExecuting.Where(isExecuting => !isExecuting).PublishLast(
                        _ => Observable.Merge(
                            this.Search.ThrownExceptions.Select(exception => exception.Message),
                            this.WhenAnyValue(vm => vm.SearchResults).Select(results => results != null && results.Any())
                                .Where(hasResults => !hasResults).Select(__ => "No results."))))
                .ToProperty(this, vm => vm.SearchEventUserInformation);
        }

        private SearchResultItemViewModel ConvertToViewModel(Repository repo)
            => new SearchResultItemViewModel(repo.name, repo.size.ToString(), repo.score.ToString());

        private IObservable<RepositoryList> SearchImpl(string searchTerm)
            => Observable.FromAsync(ct => new GithubRestClient().SearchRepositories(searchTerm, ct)).Select(UnwrapResponse);

        private static T UnwrapResponse<T>(IRestResponse<T> response)
            => response.StatusCode == HttpStatusCode.OK ? response.Data : throw new Exception($"Bad response: {response.StatusDescription}");

        public ReactiveCommand<string, RepositoryList> Search { get; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

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