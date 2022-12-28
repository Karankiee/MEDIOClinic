using CMS.Search;
using System.Collections.Generic;

public class SearchResultModel
{
    public string Query { get; set; }

    public IEnumerable<SearchResultItem> Items { get; set; }
}

