using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Studio1BTask.Models;
using Studio1BTask.Services;

namespace Studio1BTask.Controllers
{
    // TODO: Clean up some of the messy code here
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        // TODO: Use a better way of injecting stuff
        private readonly AccountService _accountService = new AccountService();

        [HttpPost("[action]")]
        public bool CreateCustomerAccount([FromBody] CustomerAccountForm form)
        {
            var sessionId = 0;
            var isSessionIdInt = Request.Cookies["sessionId"] != null && int.TryParse(Request.Cookies["sessionId"], out
                                     sessionId);
            if (!isSessionIdInt)
                return false;
            var token = Request.Cookies["token"];

            using (var context = new DbContext())
            {
                // Verify that the data session is actually valid, and that the session is not already linked to an account.
                var session = _accountService.ValidateSession(sessionId, token, context);
                if (session == null || session.AccountId != null)
                {
                    Logout();
                    // Show error 403 forbidden if the user tries to create an account whilst already logged in
                    Response.StatusCode = 403;
                    return false;
                }

                var newAccount = context.Accounts.Add(new Account
                {
                    Type = 'c',
                    Email = form.Email.ToLower(),
                    PasswordHash = null, // Fill in password later when id is available to use as salt
                    SessionId = session.Id // Tie the data session to the newly created account.
                });
                session.Account = newAccount.Entity;

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

                // Log in with credentials provided
                return Login(new LoginForm {Email = form.Email, Password = form.Password});
            }
        }

        // A session will be created the first time the user loads the website. 
        // Sessions can be used for things that don't need an account, such as shopping carts. 
        [HttpGet("[action]")]
        public dynamic CreateSession()
        {
            // Don't create a new session if token and session cookies already exist.
            // (if only one of these exists, something has gone wrong and a new session will be created anyway)
            // Instead, return the data logged in user. (null if not logged in)
            if (Request.Cookies.ContainsKey("token") && Request.Cookies.ContainsKey("sessionId"))
            {
                var sessionId = int.Parse(Request.Cookies["sessionId"]);
                var token = Request.Cookies["token"];

                if (!Request.Cookies.ContainsKey("accountId"))
                    return false;

                var accountId = int.Parse(Request.Cookies["accountId"]);

                // Return the data logged in user
                return _accountService.ValidateUser(sessionId, accountId, token, new DbContext());
            }

            // If someone with an account id does not already have a valid session, log them out. (this shouldn't happen)
            if (Request.Cookies.ContainsKey("accountId"))
            {
                Logout();
                return false;
            }

            Session session;
            using (var context = new DbContext())
            {
                var newSession = context.Sessions.Add(new Session
                {
                    Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                });
                context.SaveChanges();
                session = newSession.Entity;
            }

            // Provide with token and id cookies
            Response.Cookies.Append("token", session.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            });
            Response.Cookies.Append("sessionId", session.Id.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            });

            return true;
        }

        // Returns whether or not the login was successful. If successful, the frontend should refresh.
        [HttpPost("[action]")]
        public bool Login([FromBody] LoginForm form)
        {
            int accountId, sessionId;
            string token;

            // Validate
            form.Email = form.Email.Trim().ToLower();

            if (form.Email.Length < 4)
                return false;

            if (form.Password.Length < 8)
                return false;

            using (var context = new DbContext())
            {
                var account = _accountService.ValidateCredentials(form.Email, form.Password, context);

                if (account == null)
                    return false;

                accountId = account.Id;

                var session = context.Sessions.FirstOrDefault(x => x.Id == account.SessionId);
                // Generate new session and token if empty for whatever reason
                if (session == null || string.IsNullOrEmpty(session.Token))
                {
                    var newSession = context.Sessions.Add(new Session
                    {
                        Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                    });

                    // Link session to account, and account to session
                    newSession.Entity.Account = account;
                    account.Session = newSession.Entity;

                    context.SaveChanges();
                }

                token = account.Session.Token;
                sessionId = (int) account.SessionId;
            }

            // Provide with token and id cookies
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            });
            Response.Cookies.Append("accountId", accountId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            });
            Response.Cookies.Append("sessionId", sessionId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            });

            // Account has successfully been logged into. Page should now refresh.
            return true;
        }

        [HttpPost("[action]")]
        public void Logout()
        {
            foreach (var (key, value) in Request.Cookies)
                // Give cookies an expiry date in the past to effectively delete them
                Response.Cookies.Append(key, value, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    Expires = DateTime.Now.AddYears(-1)
                });
        }
    }
}