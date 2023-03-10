using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CMS.Membership;
using CMS.Search;

namespace MEDIOClinic.Controllers
{
    public class SearchController : Controller
    {
        // Adds the smart search indexes that will be used to perform searches
        public static readonly string[] searchIndexes = new string[] { "MEDIOClinic" };
        // Sets the limit of items per page of search results
        private const int PAGE_SIZE = 10;



        public ActionResult SearchIndex(string searchText)
        {
            // Displays the search page without any search results if the query is empty
            if (String.IsNullOrWhiteSpace(searchText))
            {
                // Creates a model representing empty search results
                SearchResultModel emptyModel = new SearchResultModel
                {
                    Items = new List<SearchResultItem>(),
                    Query = String.Empty
                };

                return View(emptyModel);
            }

            // Searches the specified index and gets the matching results
            SearchParameters searchParameters = SearchParameters.PrepareForPages(searchText, searchIndexes, 1, PAGE_SIZE, MembershipContext.AuthenticatedUser);
            SearchResult searchResult = SearchHelper.Search(searchParameters);

            // Creates a model with the search result items
            SearchResultModel model = new SearchResultModel
            {
                Items = searchResult.Items,
                Query = searchText
            };

            return View(model);
        }
    }
}
