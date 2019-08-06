using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Studio1BTask.Models;
using DbContext = Studio1BTask.Models.DbContext;

namespace Studio1BTask.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Queensland", "Hell"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            TestDatabase();
            
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        public void TestDatabase()
        {
            using (var context = new DbContext())
            {
                var key = (int) DateTime.Now.ToFileTimeUtc();
                var newEntity = new TestModel()
                {
                    Id = key, AString = "hii", ANumber = 393
                };
                context.TestModels.Add(newEntity);

                context.ForeignKeyTests.Add(new ForeignKeyTest()
                {
                    Id = 20001 + key,
                    TestModelId = key
                });
                
                context.SaveChanges();

            }
            
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get { return 32 + (int) (TemperatureC / 0.5556); }
            }
        }
    }
}