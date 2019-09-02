using System.Linq;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Studio1BTask.Models;

namespace Studio1BTask.Controllers
{
    [Route("api/[controller]")]
    public class AccountController
    {
        private const string ArgonHeaderStuff = "$argon2id$v=19$m=65536,t=3,p=1$";
        [HttpPost("[action]")]
        public void CreateCustomerAccount([FromBody] CustomerAccountForm form)
        {
            using (var context = new DbContext())
            {
                var newAccount = context.Accounts.Add(new Account()
                {
                    Type = 'c',
                    Email = form.Email,
                    PasswordHash = null // Fill in password later when id is available to use as salt
                });
                // Have to send this to the database before creating the customer entity,
                // as we don't know what the id is yet until the database tells us.
                context.SaveChanges();
                context.Customers.Add(new Customer()
                {
                    Id = newAccount.Entity.Id,
                    FirstName = form.FirstName,
                    LastName = form.LastName
                });

                newAccount.Entity.PasswordHash = HashPassword(form.Password, newAccount.Entity);
                context.SaveChanges();
            }
        }

        public bool ValidateCredentials(string email, string password)
        {
            using (var context = new DbContext())
            {
                var accountToValidate = context.Accounts.First(account => account.Email == email);
                return ValidatePassword(password, accountToValidate);
            }
        }
        
        private static string ObfuscatePassword(string password, Account account)
        {
            // i'm sorry hackers
            return account.Email.Substring(4, 9) + password + "bnepis" + (account.Id + 420) / 69 + account.Id % 3 + account.Id % 401;
        }

        private static string HashPassword(string password, Account account)
        {
            return Argon2.Hash(ObfuscatePassword(password, account)).Replace(ArgonHeaderStuff, "");
        }

        private static bool ValidatePassword(string password, Account account)
        {
            return Argon2.Verify(ArgonHeaderStuff + account.PasswordHash, ObfuscatePassword(password, account));
        }

    }
}