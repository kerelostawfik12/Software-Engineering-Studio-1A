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
    // See https://docs.microsoft.com/en-us/ef/core/querying/related-data
    [Route("api/[controller]")]
    public class ItemController : Controller
    {
        private readonly AccountService _accountService = new AccountService();

        // TODO: Use a better way of injecting stuff
        private readonly SearchService _searchService = new SearchService();

        [HttpGet("[action]")]
        public Item GetItem([FromQuery] int id)
        {
            using (var context = new DbContext())
            {
                var item = context.Items.Include(x => x.Seller).First(x => x.Id == id);
                if (item.Views == null) item.Views = 0;
                item.Views++;
                context.SaveChanges();
                return item;
            }
        }

        // TODO: Make this actually get the seller id from the post request
        [HttpPost("[action]")]
        public Item CreateItem([FromBody] NewItemForm form)
        {
            form.Price = form.Price.Replace("$", "");
            using (var context = new DbContext())
            {
                var seller = _accountService.ValidateSellerSession(Request.Cookies, context);
                if (seller == null)
                {
                    Response.StatusCode = 403;
                    return null;
                }

                var item = context.Items.Add(new Item
                {
                    Description = form.Description,
                    Name = form.Name,
                    Price = decimal.Parse(form.Price),
                    SellerId = seller.Id
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
                // If an admin, just remove the item.
                if (_accountService.ValidateAdminSession(Request.Cookies, context))
                {
                    context.Items.Remove(GetItem(simpleInt.Value));
                    context.SaveChanges();
                    return;
                }

                // Else, check if the user is a seller.
                var seller = _accountService.ValidateSellerSession(Request.Cookies, context);
                // If user is not a seller, return.
                if (seller == null)
                {
                    Response.StatusCode = 403;
                    return;
                }

                var item = GetItem(simpleInt.Value);
                // Do not let a seller remove the item if they are not the owner of the item.
                if (item.SellerId != seller.Id)
                {
                    Response.StatusCode = 403;
                    return;
                }

                context.Items.Remove(item);
                context.SaveChanges();
            }
        }

        // If the requesting user is a seller, it will get all items from that seller. 
        // If the requesting user is an admin, it will get all items in the database.
        [HttpGet("[action]")]
        public IEnumerable<Item> AllItems()
        {
            using (var context = new DbContext())
            {
                var seller = _accountService.ValidateSellerSession(Request.Cookies, context);
                if (seller != null)
                {
                    var items = context.Items
                        .Where(x => x.SellerId == seller.Id)
                        .Include(item => item.Seller)
                        .ToList();
                    return items;
                }

                if (_accountService.ValidateAdminSession(Request.Cookies, context))
                {
                    var items = context.Items
                        .Include(item => item.Seller)
                        .ToList();
                    return items;
                }

                // If the user is not a seller or admin, there is nothing to give them.
                Response.StatusCode = 403;
                return null;
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

            using (var context = new DbContext())
            {
                var q = context.Items as IQueryable<Item>;

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
        public IEnumerable<Item> GetItemsInCart()
        {
            using (var context = new DbContext())
            {
                var session = _accountService.ValidateSession(Request.Cookies, context);
                if (session == null)
                {
                    Response.StatusCode = 401; // Unauthorised
                    return null;
                }

                var items = context.CartItems
                    .Where(x => x.SessionId == session.Id)
                    .Include(x => x.Item)
                    .Select(cartItem => cartItem.Item).Include(x => x.Seller)
                    .ToList();
                return items;
            }
        }

        [HttpPost("[action]")]
        public IEnumerable<Item> AddItemToCart([FromBody] SimpleInt id)
        {
            using (var context = new DbContext())
            {
                var session = _accountService.ValidateSession(Request.Cookies, context);
                if (session == null)
                {
                    Response.StatusCode = 401; // Unauthorised
                    return null;
                }

                context.CartItems.Add(new CartItem
                {
                    SessionId = session.Id,
                    ItemId = id.Value
                });

                context.SaveChanges();
                return GetItemsInCart();
            }
        }

        [HttpPost("[action]")]
        public IEnumerable<Item> RemoveItemFromCart([FromBody] SimpleInt id)
        {
            using (var context = new DbContext())
            {
                var session = _accountService.ValidateSession(Request.Cookies, context);
                if (session == null)
                {
                    Response.StatusCode = 401; // Unauthorised
                    return null;
                }

                var itemToRemove =
                    context.CartItems.FirstOrDefault(x => x.SessionId == session.Id && x.ItemId == id.Value);
                if (itemToRemove == null)
                {
                    // Item does not exist or has already been removed
                    Response.StatusCode = 404;
                    return null;
                }

                context.CartItems.Remove(itemToRemove);
                context.SaveChanges();
                return GetItemsInCart();
            }
        }
    }
}