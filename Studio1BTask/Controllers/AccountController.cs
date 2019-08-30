using System;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Studio1BTask.Models;

namespace Studio1BTask.Controllers
{
    [Route("api/[controller]")]
    public class AccountController
    {
        [HttpPost("[action]")]
        public void CreateCustomerAccount([FromBody] CustomerAccountForm form)
        {
            var passwordHash = Argon2.Hash(form.Password);
            Console.WriteLine(Argon2.Verify(passwordHash, form.Password));
            
            using (var context = new DbContext())
            {
                var newAccount = context.Accounts.Add(new Account()
                {
                    Type = 'c',
                    Email = form.Email,
                    PasswordHash = passwordHash
                });
                // Have to send this to the database before creating the customer entity,
                // as we don't know what the id is yet until the database tells us
                context.SaveChanges();
                context.Customers.Add(new Customer()
                {
                    Id = newAccount.Entity.Id,
                    FirstName = form.FirstName,
                    LastName = form.LastName
                });
                context.SaveChanges();
            }
        }

        public bool ValidateCredentials(string email, string password)
        {
            using (var context = new DbContext())
            {
                
            }

            return false;
        }
    }
}