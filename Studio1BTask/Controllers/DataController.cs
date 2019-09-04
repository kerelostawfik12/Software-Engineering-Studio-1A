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
    public class DataController : Controller
    {
        [HttpGet("[action]")]
        public IEnumerable<TestModel> TestModels()
        {
            using (var context = new DbContext())
            {
                return context.TestModels.ToList();
            }
        }

        [HttpGet("[action]")]
        public IEnumerable<ForeignKeyTest> ForeignKeyTests()
        {
            using (var context = new DbContext())
            {
                var fkts = context.ForeignKeyTests
                    .Include(fkt => fkt.TestModel)
                    .ToList();
                return fkts;
            }
        }

        [HttpGet("[action]")]
        public IEnumerable<Item> AllItems()
        {
            using (var context = new DbContext())
            {
                var items = context.Items.Include(item => item.Seller).ToList();
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