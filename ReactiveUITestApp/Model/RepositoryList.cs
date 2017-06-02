using System.Collections.Generic;

namespace ReactiveUITestApp.Model {
    public class RepositoryList {
        public int total_count { get; set; }
        public bool incomplete_results { get; set; }
        public List<Repository> items { get; set; }
    }
}
