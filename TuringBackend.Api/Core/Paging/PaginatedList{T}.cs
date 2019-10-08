using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TuringBackend.Api.Core
{
    public class PaginatedList<T>
    {
        public PaginatedList(IQueryable<T> source, int count)
        {
            TotalCount = count;
            Rows = source.ToList();
        }

        public int TotalCount { get; }

        [JsonProperty("rows")]
        public List<T> Rows { get; }
    }

    public class PaginationMeta
    {
        public PaginationMeta()
        {

        }
        public PaginationMeta(int currentPage, int currentPageSize, int totalPages, int totalRecords)
        {
            CurrentPage = currentPage;
            CurrentPageSize = currentPageSize;
            TotalPages = totalPages;
            TotalRecords = totalRecords;
        }

        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("currentPageSize")]
        public int CurrentPageSize { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("totalRecords")]
        public int TotalRecords { get; set; }

    }
}