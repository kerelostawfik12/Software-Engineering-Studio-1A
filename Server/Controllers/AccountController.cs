using System;
using System.Collections.Generic;
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
        public Dictionary<string, dynamic> CreateCustomerAccount([FromBody] CustomerAccountForm form)
        {
            // Create response object to send back to client
            var obj = new Dictionary<string, dynamic>
            {
                ["error"] = null, ["success"] = false
            };

            // Clean up fields
            form.Email = form.Email.Trim().ToLower();

            // Verify that email and password are valid
            if (!_accountService.IsEmailValid(form.Email))
            {
                obj["error"] = "Please provide a valid email.";
                return obj;
            }

            if (!_accountService.IsPasswordValid(form.Password, out var reason))
            {
                obj["error"] = reason;
                return obj;
            }

            using (var context = new DbContext())
            {
                // Verify that the session is actually valid, and that the session is not already linked to an account.
                var session = _accountService.ValidateSession(Request.Cookies, context);
                if (session == null || session.AccountId != null)
                {
                    ClearCookies();
                    Response.StatusCode = 403;
                    obj["error"] =
                        "Invalid session. Please try again, refresh the page, or delete cookies for the site.";
                    return obj;
                }

                // Verify that the provided email is not already in use.
                if (context.Accounts.Any(x => x.Email == form.Email))
                {
                    obj["error"] = "The email address provided is already in use.";
                    return obj;
                }

                // Try actually creating the account.
                try
                {
                    // Create the Account object
                    var newAccount = context.Accounts.Add(new Account
                    {
                        Type = 'c',
                        Email = form.Email,
                        PasswordHash = null, // Fill in password later when id is available to use as salt
                        SessionId = session.Id // Tie the session to the newly created account.
                    });
                    session.Account = newAccount.Entity; // Tie the new account to the existing session.

                    // Have to send this to the database before creating the customer entity,
                    // as we don't know what the id is yet until the database tells us.
                    context.SaveChanges();

                    // Create the Customer object
                    context.Customers.Add(new Customer
                    {
                        Id = newAccount.Entity.Id,
                        FirstName = form.FirstName,
                        LastName = form.LastName
                    });

                    // Set the password hash
                    newAccount.Entity.PasswordHash = _accountService.HashPassword(form.Password, newAccount.Entity);

                    context.SaveChanges();
                }
                catch
                {
                    obj["error"] = "An unknown database error occurred.";
                    return obj;
                }

                // Log in with credentials provided
                var loginSuccessful = Login(new LoginForm {Email = form.Email, Password = form.Password});
                if (loginSuccessful)
                {
                    obj["success"] = true;
                    return obj;
                }

                obj["error"] = "An account was created but could not be logged into. Please try logging in manually.";
                return obj;
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
                // Clear login cookies
                ClearCookies();
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

            // Clear login cookies
            ClearCookies();

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
            ClearCookies();
        }

        private void ClearCookies()
        {
            // Clear all cookies
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