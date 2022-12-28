using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using CMS.Core;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Routing;

using Kentico.Content.Web.Mvc;

using MEDIOClinic.Models;

namespace CoreTutorial.ViewComponents
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private readonly IPageRetriever pageRetriever;
        private readonly IPageUrlRetriever pageUrlRetriever;

        public NavigationMenuViewComponent(IPageRetriever pageRetriever, IPageUrlRetriever pageUrlRetriever)
        {
            // Initializes instances of required services using dependency injection
            this.pageRetriever = pageRetriever;
            this.pageUrlRetriever = pageUrlRetriever;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Retrieves a collection of page objects with data for the menu (pages of all page types)
            IEnumerable<TreeNode> menuItems = await pageRetriever.RetrieveAsync<TreeNode>(query => query
                                                    // Selects pages that have the 'Show in menu" flag enabled
                                                    .MenuItems()
                                                    // Only loads pages from the first level of the site's content tree
                                                    .NestingLevel(1)
                                                    // Filters the query to only retrieve required columns
                                                    .Columns("DocumentName", "NodeID", "NodeSiteID")
                                                    // Loads pages together with their URL path data (performance reasons)
                                                    .WithPageUrlPaths()
                                                    // Uses the menu item order from the content tree
                                                    .OrderByAscending("NodeOrder"));

            // Creates a collection of view models based on the menu item data
            IEnumerable<MenuItemViewModel> model = menuItems.Select(item => new MenuItemViewModel()
            {
                // Gets the name of the page as the menu item caption text
                MenuItemText = item.DocumentName,
                // Retrieves the URL for the page (as a relative virtual path)
                MenuItemRelativeUrl = pageUrlRetriever.Retrieve(item).RelativePath
            });

            return View(model);
        }
    }
}