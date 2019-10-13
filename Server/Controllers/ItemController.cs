using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
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
        private readonly ImageUploadService _imageUploadService = new ImageUploadService();

        // TODO: Use a better way of injecting stuff
        private readonly SearchService _searchService = new SearchService();

        [HttpGet("[action]")]
        public Item GetItem([FromQuery] int id)
        {
            using (var context = new DbContext())
            {
                var item = context.Items
                    .Include(x => x.Seller)
                    .First(x => x.Id == id && !x.Hidden);
                if (item.Views == null) item.Views = 0;
                item.Views++;
                context.SaveChanges();
                return item;
            }
        }

        [HttpGet("[action]")]
        public IEnumerable<Item> GetItemsBoughtTogether([FromQuery] int id)
        {
            using (var context = new DbContext())
            {
                var items = context.ItemsBoughtTogether
                    .Where(x => x.ItemAId == id && x.ItemA.Hidden == false && x.ItemB.Hidden == false)
                    .OrderByDescending(x => x.Count)
                    .Take(8)
                    .Select(x => x.ItemB)
                    .ToList();
                return items;
            }
        }

        [HttpGet("[action]")]
        public Dictionary<string, dynamic> GetItemPage([FromQuery] int id)
        {
            return new Dictionary<string, dynamic>
            {
                ["item"] = GetItem(id),
                ["boughtTogether"] = GetItemsBoughtTogether(id)
            };
        }

        [HttpPost("[action]")]
        public Dictionary<string, string> UploadImage()
        {
            var obj = new Dictionary<string, string> {["reason"] = "An unknown error occurred."};
            // If above 4MB, refuse it (actually a bit more to be generous)
            if (Request.ContentLength > 4200000)
            {
                obj["reason"] = "Uploads must not exceed 4MB.";
                Response.StatusCode = 403;
                return obj;
            }

            // If not a seller, refuse it
            if (_accountService.ValidateSellerSession(Request.Cookies, new DbContext()) == null)
            {
                obj["reason"] = "Please log in as a seller to use this form.";
                Response.StatusCode = 403;
                return obj;
            }

            const string baseUrl = "https://studio1btask.blob.core.windows.net/images/";
            var blob = Request.Body;

            var fileName = Guid.NewGuid() + ".jpg";
            _imageUploadService.UploadImage(fileName, blob).Wait();
            obj["url"] = baseUrl + fileName;
            return obj;
        }

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
                    SellerId = seller.Id,
                    ImageURL = form.ImageURL,
                    Hidden = false,
                    Purchases = 0
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
                var item = context.Items.First(x => x.Id == simpleInt.Value);
                // Check if the user is a seller.
                var seller = _accountService.ValidateSellerSession(Request.Cookies, context);
                var admin = _accountService.ValidateAdminSession(Request.Cookies, context);
                // If user is not a seller or admin, return.
                if (seller == null && admin == false)
                {
                    Response.StatusCode = 403;
                    return;
                }

                // Do not let a seller remove the item if they are not the owner of the item.
                if (!admin && item.SellerId != seller.Id)
                {
                    Response.StatusCode = 403;
                    return;
                }

                item.Hidden = true;
                context.SaveChanges();
            }
        }

        // If the requesting user is a seller, it will get all mostViewed from that seller. 
        // If the requesting user is an admin, it will get all mostViewed in the database.
        [HttpGet("[action]")]
        public IEnumerable<Item> AllItems()
        {
            using (var context = new DbContext())
            {
                var seller = _accountService.ValidateSellerSession(Request.Cookies, context);
                if (seller != null)
                {
                    var items = context.Items
                        .Where(x => !x.Hidden && x.SellerId == seller.Id)
                        .Include(item => item.Seller)
                        .OrderBy(x => x.Id)
                        .ToList();
                    return items;
                }

                if (_accountService.ValidateAdminSession(Request.Cookies, context))
                {
                    var items = context.Items
                        .Where(x => !x.Hidden)
                        .Include(item => item.Seller)
                        .OrderBy(x => x.Id)
                        .ToList();
                    return items;
                }

                // If the user is not a seller or admin, there is nothing to give them.
                Response.StatusCode = 403;
                return null;
            }
        }

        [HttpGet("[action]")]
        public IEnumerable<Item> MostViewedItems(int count)
        {
            using (var context = new DbContext())
            {
                var items = context.Items
                    .Where(x => !x.Hidden)
                    .Include(item => item.Seller)
                    .OrderByDescending(item => item.Views)
                    .Take(count)
                    .ToList();
                return items;
            }
        }

        [HttpGet("[action]")]
        public IEnumerable<Item> TopSellingItems(int count)
        {
            using (var context = new DbContext())
            {
                var items = context.Items
                    .Where(x => !x.Hidden)
                    .Include(item => item.Seller)
                    .OrderByDescending(item => item.Purchases)
                    .Take(count)
                    .ToList();
                return items;
            }
        }

        [HttpGet("[action]")]
        public Dictionary<string, dynamic> FrontPageItems()
        {
            return new Dictionary<string, dynamic>
            {
                ["topSellers"] = TopSellingItems(16),
                ["mostViewed"] = MostViewedItems(16)
            };
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
                var q = context.Items.Where(x => !x.Hidden);

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
        public Dictionary<string, dynamic> GetItemsInCart()
        {
            var obj = new Dictionary<string, dynamic>();

            using (var context = new DbContext())
            {
                var session = _accountService.ValidateSession(Request.Cookies, context);
                if (session == null)
                {
                    Response.StatusCode = 401; // Unauthorised
                    return null;
                }

                var cartItems = context.CartItems
                    .Where(x => x.SessionId == session.Id)
                    .Include(x => x.Item);

                // If there are any hidden mostViewed in the cart, remove them before returning the mostViewed.
                var itemsToRemove = cartItems.Where(x => x.Item.Hidden);
                if (!itemsToRemove.IsNullOrEmpty())
                {
                    context.CartItems.RemoveRange(itemsToRemove);
                    context.SaveChanges();
                    if (itemsToRemove.Count() == 1)
                        obj["warning"] = "An item in your cart has been removed, as it is no longer available.";
                    else
                        obj["warning"] =
                            "Some mostViewed in your cart have been removed, as they are no longer available.";
                }

                var items = cartItems
                    .Select(cartItem => cartItem.Item)
                    .Where(x => !x.Hidden)
                    .Include(x => x.Seller)
                    .ToList();

                obj["mostViewed"] = items;
                return obj;
            }
        }

        [HttpPost("[action]")]
        public Dictionary<string, dynamic> AddItemToCart([FromBody] SimpleInt id)
        {
            using (var context = new DbContext())
            {
                var session = _accountService.ValidateSession(Request.Cookies, context);
                if (session == null)
                {
                    Response.StatusCode = 401; // Unauthorised
                    return null;
                }

                // If item does not exist, don't add it.
                if (context.Items.FirstOrDefault(x => x.Id == id.Value && !x.Hidden) == null)
                {
                    Response.StatusCode = 404; // Not found
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
        public Dictionary<string, dynamic> RemoveItemFromCart([FromBody] SimpleInt id)
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