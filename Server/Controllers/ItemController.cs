using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studio1BTask.Models;
using Studio1BTask.Services;
using DbContext = Studio1BTask.Models.DbContext;

// See data-models.ts for the client-side version of these classes (make sure they are consistent)

namespace Studio1BTask.Controllers
{
    // NOTE: To have objects from foreign keys filled, you will need to use Include(), or they will be empty. 
    // See ForeignKeyTests() or https://docs.microsoft.com/en-us/ef/core/querying/related-data
    [Route("api/[controller]")]
    public class ItemController : Controller
    {
        // TODO: Use a better way of injecting stuff
        private readonly SearchService _searchService = new SearchService();

        [HttpGet("[action]")]
        public Item GetItem([FromQuery] int id)
        {
            using (var context = new DbContext())
            {
                return context.Items.Include(item => item.Seller).First(item => item.Id == id);
            }
        }

        // TODO: Make this actually get the seller id from the post request
        [HttpPost("[action]")]
        public Item CreateItem([FromBody] NewItemForm form)
        {
            form.Price = form.Price.Replace("$", "");
            using (var context = new DbContext())
            {
                var item = context.Items.Add(new Item
                {
                    Description = form.Description,
                    Name = form.Name,
                    Price = decimal.Parse(form.Price),
                    SellerId = 41
                });
                context.SaveChanges();
                return item.Entity;
            }
        }

        [HttpPost("[action]")]
        public void RemoveItem([FromBody] SimpleInt simpleInt)
        {
            using (var context = new DbContext())
            {
                context.Items.Remove(GetItem(simpleInt.Value));
                context.SaveChanges();
            }
        }

        [HttpGet("[action]")]
        public IEnumerable<Item> AllItems()
        {
            using (var context = new DbContext())
            {
                var items = context.Items
                    .Include(item => item.Seller)
                    .ToList();
                return items;
            }
        }

        [HttpGet("[action]")]
        public SearchItemResult SearchItems([FromQuery] string query)
        {
            // TODO: Pages
            // TODO: Separate the search phrase when adding in filters
            if (query == null)
                query = "";
            var searchPhrase = query.ToLower().Trim();

            // Allow for searching by id if input string is an integer
            var searchId = -1;
            if (int.TryParse(searchPhrase.Replace("id:", ""), out var result)) searchId = result;
            var relevanceDict = new Dictionary<int, int>();
            using (var context = new DbContext())
            {
                var q = context.Items as IQueryable<Item>;
                relevanceDict[2] = 1;
                // Include seller data from start so it can be searched
                q = q.Include(item => item.Seller);

                var words = _searchService.GetWordsFromPhrase(searchPhrase);

                // This code just adds another "WHERE" query for each word in the search phrase.
                // Rider really wanted to turn this into a LINQ expression, so I just let it do that lol
                q = words.Select((t, i) => i)
                    .Aggregate(q, (current, i) => current.Where(x =>
                            x.Name.Contains(words[i]) ||
                            x.Description.Contains(words[i]) ||
                            x.Seller.Name.Contains(words[i]) ||
                            x.Id == searchId
                        )
                        // Prioritise name and id matches
                        .OrderBy(x => (x.Name.Contains(words[i]) ? 0 : 5) +
                                      (x.Name.StartsWith(words[i]) ? 0 : 6)
                                      + (x.Id == searchId ? 0 : 8))
                    );

                return new SearchItemResult {Items = q.ToList()};
            }
        }

        [HttpGet("[action]")]
        public IEnumerable<Item> ItemsInCart()
        {
            using (var context = new DbContext())
            {
                var items = context.Items.Include(item => item.Seller).ToList();
                return items;
            }
        }
    }
}