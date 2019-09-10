using Microsoft.AspNetCore.Mvc;
using Studio1BTask.Models;
using Studio1BTask.Services;

namespace Studio1BTask.Controllers
{
    [Route("api/[controller]")]
    public class AccountController
    {
        // TODO: Use a better way of injecting stuff
        private readonly AccountService _service = new AccountService();

        [HttpPost("[action]")]
        public void CreateCustomerAccount([FromBody] CustomerAccountForm form)
        {
            using (var context = new DbContext())
            {
                var newAccount = context.Accounts.Add(new Account
                {
                    Type = 'c',
                    Email = form.Email,
                    PasswordHash = null // Fill in password later when id is available to use as salt
                });
                // Have to send this to the database before creating the customer entity,
                // as we don't know what the id is yet until the database tells us.
                context.SaveChanges();
                context.Customers.Add(new Customer
                {
                    Id = newAccount.Entity.Id,
                    FirstName = form.FirstName,
                    LastName = form.LastName
                });

                newAccount.Entity.PasswordHash = _service.HashPassword(form.Password, newAccount.Entity);
                context.SaveChanges();
            }
        }
    }
}