using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Studio1BTask.Models;
using Studio1BTask.Services;

namespace Studio1BTask.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        // TODO: Use a better way of injecting stuff
        private readonly AccountService _accountService = new AccountService();

        [HttpPost("[action]")]
        public void CreateCustomerAccount([FromBody] CustomerAccountForm form)
        {
            using (var context = new DbContext())
            {
                var newAccount = context.Accounts.Add(new Account
                {
                    Type = 'c',
                    Email = form.Email.ToLower(),
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

                newAccount.Entity.PasswordHash = _accountService.HashPassword(form.Password, newAccount.Entity);
                context.SaveChanges();
            }
        }

        [HttpPost("[action]")]
        public dynamic Login([FromBody] LoginForm form)
        {
            dynamic obj;
            int id;
            string token;

            // Validate
            form.Email = form.Email.Trim().ToLower();

            if (form.Email.Length < 4)
                return null;

            if (form.Password.Length < 8)
                return null;

            using (var context = new DbContext())
            {
                var account = _accountService.ValidateCredentials(form.Email, form.Password, context);

                if (account == null)
                    return null;

                id = account.Id;

                // Return relevant user object
                if (account.Type == 'c')
                {
                    var customer = context.Customers.Find(account.Id);
                    obj = customer;
                }
                else if (account.Type == 's')
                {
                    var seller = context.Sellers.Find(account.Id);
                    obj = seller;
                }
                else
                {
                    return null;
                }

                // Generate new token if empty
                if (string.IsNullOrEmpty(account.AccessToken))
                {
                    account.AccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    context.SaveChanges();
                }

                token = account.AccessToken;
            }

            // Strip account data
            obj.Account = null;

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            });
            Response.Cookies.Append("id", id.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            });
            return obj;
        }
    }
}