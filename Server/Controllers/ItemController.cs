using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studio1BTask.Models;
using DbContext = Studio1BTask.Models.DbContext;

// See data-models.ts for the client-side version of these classes (make sure they are consistent)

namespace Studio1BTask.Controllers
{
    // NOTE: To have objects from foreign keys filled, you will need to use Include(), or they will be empty. 
    // See ForeignKeyTests() or https://docs.microsoft.com/en-us/ef/core/querying/related-data
    [Route("api/[controller]")]
    public class ItemController : Controller
    {
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