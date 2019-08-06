using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Studio1BTask.Models;
using Microsoft.EntityFrameworkCore;
using DbContext = Studio1BTask.Models.DbContext;

namespace Studio1BTask.Controllers
{
    // NOTE: To have objects from foreign keys filled, you will need to use Include(), or they will be empty. 
    // See ForeignKeyTests() or https://docs.microsoft.com/en-us/ef/core/querying/related-data
    [Route("api/[controller]")]
    public class TestDataController : Controller
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
    }
}