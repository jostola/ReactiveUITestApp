using RestSharp;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUITestApp.Model {
    public class GithubRestClient {
        private RestClient Client { get; }
            = new RestClient(@"https://api.github.com/");

        public Task<IRestResponse<RepositoryList>> SearchRepositories(string query, CancellationToken ct) {
            var request = new RestRequest(@"search/repositories", Method.GET);
            request.AddQueryParameter("q", query);
            return Client.ExecuteTaskAsync<RepositoryList>(request, ct);
        }
    }
}
