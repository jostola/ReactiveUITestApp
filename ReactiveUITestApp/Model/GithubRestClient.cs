using RestSharp;
using System.Threading.Tasks;

namespace ReactiveUITestApp.Model {
    public class GithubRestClient {
        private RestClient Client { get; }
            = new RestClient(@"https://api.github.com/");

        public Task<IRestResponse<RepositoryList>> SearchRepositories(string searchQuery, string sortByProperty, string orderByProperty) {
            var request = new RestRequest(@"search/repositories", Method.GET);
            request.AddQueryParameter("q", searchQuery);
            request.AddQueryParameter("sort", sortByProperty);
            request.AddQueryParameter("order", orderByProperty);
            return Client.ExecuteTaskAsync<RepositoryList>(request);
        }
    }
}
