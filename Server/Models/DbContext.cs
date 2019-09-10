using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Studio1BTask.Models
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        // Remember to add any new tables here. Be careful not to mix the name of the table and the name of the class up.
        public DbSet<Item> Items { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection") ??
                                   File.ReadAllText("database-credentials.txt");
            optionsBuilder.UseSqlServer(connectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    /*
     
    Creating a new table involves four steps. You can complete these steps in any order you want, but don't forget
    to complete them all. It will help a lot to use the existing tables/classes as a guide.
    
    1. Create the class, with all columns in the database given public getters and setters. You must add the 
    {get; set;} syntax to the end as well, or it won't be picked up and weird things will happen for some reason.
    Your primary key must be marked with [Key].
    
    2. Add a new public DbSet member variable to the space near top of the file. The name you give it should be the 
    plural version of whatever the name of the class is, and you should put the actual name of the class like so: 
    public DbSet<Student> Students {get; set;}
    
    3. Create the table in the actual database. You will need a direct connection to the database to do this, using the
    credentials provided in postgres-credentials.txt. This is the most complicated step, so ask Jamie if you need help.
    
    The table must have the EXACT same name as the name of the DbSet at the top of this file (i.e. in plural form, and 
    in PascalCase). Columns must also have the EXACT same name as the member variables of the class. You must also 
    define a primary key. When using foreign keys, you must create an attribute for both the object and foreign key, 
    putting the ForeignKey annotation before the key, with the object name passed as the parameter (see ForeignKeyTest).
    
    IMPORTANT: You MUST surround the name of the table and all its columns with double quotation marks (") like the 
    example below. This also applies if you are using your IDE or something to make changes instead of raw SQL. If 
    the database is giving you errors, it's likely that your names are just wrong.
    
    create table "TestModels" 
    (
        "Id"      integer not null constraint testmodels_pk primary key,
        "ANumber" integer,
        "AString" varchar(20)
    );
    
    4. If the entities are to be fully exposed to the frontend, add them to data-models.ts.
    */
}