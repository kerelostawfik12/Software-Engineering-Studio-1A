using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace Studio1BTask.Models
{
    public class TestModelContext : DbContext
    {
        public TestModelContext(DbContextOptions<TestModelContext> options)
            : base(options)
        { }
        
        public DbSet<TestModel> TestModels;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: Use something else for the database and don't expose this string publicly
            var connection = "Server=topsy.db.elephantsql.com;Port=5432;Database=psksfvff;Username=psksfvff;Password=dDW_fcbAqGRYtAQgBPzPk3z4ix4HYhLZ";
            optionsBuilder.UseNpgsql(connection);
            base.OnConfiguring(optionsBuilder);
        } 
    }
    public class TestModel
    {
        public int ANumber;
        public string AString;
    }
}