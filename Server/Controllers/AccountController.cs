using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studio1BTask.Models;
using Studio1BTask.Services;
using DbContext = Studio1BTask.Models.DbContext;

namespace Studio1BTask.Controllers
{
    // TODO: Clean up some of the messy code here
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        // TODO: Use a better way of injecting stuff
        private readonly AccountService _accountService = new AccountService();
        private readonly string _chatSecret = "sk_test_7XLBVs1PZ8zhQoiqYAGuoqS5";

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


        [HttpPost("[action]")]
        public Dictionary<string, dynamic> CreateSellerAccount([FromBody] CreateSellerForm form)
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
                // Verify that the account is being created by an admin
                if (!_accountService.ValidateAdminSession(Request.Cookies, context))
                {
                    obj["error"] = "Only admins can create seller accounts.";
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
                        Type = 's',
                        Email = form.Email,
                        PasswordHash = null // Fill in password later when id is available to use as salt
                    });

                    // Have to send this to the database before creating the customer entity,
                    // as we don't know what the id is yet until the database tells us.
                    context.SaveChanges();

                    // Create the Seller object
                    context.Sellers.Add(new Seller
                    {
                        Id = newAccount.Entity.Id,
                        Name = form.Name
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

                return obj;
            }
        }

        [HttpPost("[action]")]
        public Dictionary<string, dynamic> CreateAdminAccount([FromBody] AdminAccountForm form)
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
                // Verify that the account is being created by an admin
                if (!_accountService.ValidateAdminSession(Request.Cookies, context))
                {
                    obj["error"] = "Only admins can create admin accounts.";
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
                        Type = 'a',
                        Email = form.Email,
                        PasswordHash = null // Fill in password later when id is available to use as salt
                    });

                    // Have to send this to  the database before creating the customer entity,
                    // as we don't know what the id is yet until the database tells us.
                    context.SaveChanges();


                    // Set the password hash
                    newAccount.Entity.PasswordHash = _accountService.HashPassword(form.Password, newAccount.Entity);

                    context.SaveChanges();
                }
                catch
                {
                    obj["error"] = "An unknown database error occurred.";
                    return obj;
                }

                return obj;
            }
        }


        //might make new controller just adding this here because I lost my code :(
        [HttpGet("[action]")]
        public IEnumerable<Customer> GetAllCustomers()
        {
            using (var context = new DbContext())
            {
                if (!_accountService.ValidateAdminSession(Request.Cookies, context))
                {
                    Response.StatusCode = 403;
                    return null;
                }

                var customers = context.Customers
                    .Include(customer => customer.Account)
                    .ToList();
                return customers;
            }
        }

        //might make new controller just adding this here because I lost my code :(
        [HttpGet("[action]")]
        public IEnumerable<Seller> GetAllSellers()
        {
            using (var context = new DbContext())
            {
                if (!_accountService.ValidateAdminSession(Request.Cookies, context))
                {
                    Response.StatusCode = 403;
                    return null;
                }

                var sellers = context.Sellers
                    .Include(seller => seller.Account)
                    .ToList();
                return sellers;
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
                var user = _accountService.ValidateUser(sessionId, accountId, token, new DbContext());
                // If user is invalid, clear cookies.
                if (user == null)
                    ClearCookies();
                else
                    return user;
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
                    Token = _accountService.CreateSessionToken()
                });
                context.SaveChanges();
                session = newSession.Entity;
            }

            // Provide with token and id cookies
            Response.Cookies.Append("token", session.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Expires = DateTimeOffset.Now.AddYears(1)
            });
            Response.Cookies.Append("sessionId", session.Id.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Expires = DateTimeOffset.Now.AddYears(1)
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
                        Token = _accountService.CreateSessionToken()
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
                IsEssential = true,
                Expires = DateTimeOffset.Now.AddYears(1)
            });
            Response.Cookies.Append("accountId", accountId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Expires = DateTimeOffset.Now.AddYears(1)
            });
            Response.Cookies.Append("sessionId", sessionId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Expires = DateTimeOffset.Now.AddYears(1)
            });

            // Account has successfully been logged into. Page should now refresh.
            return true;
        }

        [HttpPost("[action]")]
        public void Logout()
        {
            ClearCookies();
        }


        [HttpPost("[action]")]
        public void RemoveAccount()
        {
            using (var context = new DbContext())
            {
                var account = _accountService.ValidateAccountSession(Request.Cookies, context);
                context.Accounts.Remove(account);
                context.SaveChanges();
            }
        }

        [HttpPost("[action]")]
        public Dictionary<string, string> ChangePassword([FromBody] Dictionary<string, string> request)
        {
            using (var context = new DbContext())
            {
                var obj = new Dictionary<string, string>();
                // Validate the user's email and old password
                var account = _accountService.ValidateCredentials(request["email"], request["oldPassword"], context);
                var session = _accountService.ValidateSession(Request.Cookies, context);

                if (account == null || account.Id != session.AccountId)
                {
                    obj["error"] = "Invalid credentials.";
                    Response.StatusCode = 403;
                    return obj;
                }

                var newPassword = request["newPassword"];
                if (!_accountService.IsPasswordValid(newPassword, out var reason))
                {
                    obj["error"] = reason;
                    Response.StatusCode = 403;
                    return obj;
                }

                // Set the new password
                var newHash = _accountService.HashPassword(request["newPassword"], account);
                account.PasswordHash = newHash;

                // Reset session token (log user out everywhere)
                context.Sessions.Find(account.SessionId).Token = _accountService.CreateSessionToken();
                context.SaveChanges();

                // Log the user out
                ClearCookies();
                return obj;
            }
        }

        [HttpPost("[action]")]
        public Dictionary<string, string> ChangeEmail([FromBody] Dictionary<string, string> request)
        {
            using (var context = new DbContext())
            {
                var obj = new Dictionary<string, string>();
                // Validate the user's old email and password
                var account = _accountService.ValidateCredentials(request["oldEmail"], request["password"], context);
                var session = _accountService.ValidateSession(Request.Cookies, context);

                if (account == null || account.Id != session.AccountId)
                {
                    obj["error"] = "Invalid credentials.";
                    Response.StatusCode = 403;
                    return obj;
                }

                var newEmail = request["newEmail"];
                if (!_accountService.IsEmailValid(newEmail))
                {
                    obj["error"] = "Invalid email.";
                    Response.StatusCode = 403;
                    return obj;
                }

                // Set the new email
                account.Email = newEmail;

                // Set the new password hash
                var newHash = _accountService.HashPassword(request["password"], account);
                account.PasswordHash = newHash;

                // Reset session token (log user out everywhere)
                context.Sessions.Find(account.SessionId).Token = _accountService.CreateSessionToken();
                context.SaveChanges();

                // Log the user out
                ClearCookies();
                return obj;
            }
        }

        [HttpPost("[action]")]
        public Dictionary<string, string> ChangeName([FromBody] Dictionary<string, string> request)
        {
            using (var context = new DbContext())
            {
                var obj = new Dictionary<string, string>();
                var customer = _accountService.ValidateCustomerSession(Request.Cookies, context);
                var seller = _accountService.ValidateSellerSession(Request.Cookies, context);
                if (customer != null)
                {
                    customer.FirstName = request["firstName"];
                    customer.LastName = request["lastName"];
                }
                else if (seller != null)
                {
                    seller.Name = request["name"];
                }
                else
                {
                    obj["error"] = "Invalid session. Please refresh the page, and try again.";
                    return obj;
                }

                context.SaveChanges();
                return obj;
            }
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

        // This is a monolithic and extremely poorly written function that probably needs to be refactored at some point,
        // but probably won't be as it kinda works okay for the time being and the project is due really soon.
        // Originally the conversation stuff was handled on the backend, but I gave up after about 3 hours, which is why the code is especially strange.
        [HttpGet("[action]")]
        public Dictionary<string, string> StartChat([FromQuery] int? id)
        {
            var obj = new Dictionary<string, string> {["error"] = "", ["otherId"] = ""};

            using (var context = new DbContext())
            {
                // Check if user is a customer or seller
                var customer = _accountService.ValidateCustomerSession(Request.Cookies, context, true);
                var seller = _accountService.ValidateSellerSession(Request.Cookies, context, true);

                string userId, userName, userEmail, userRole;

                if (customer != null)
                {
                    userId = customer.Id.ToString();
                    userName = customer.FirstName + " " + customer.LastName;
                    userEmail = customer.Account.Email;
                    userRole = "customer";
                }
                else if (seller != null)
                {
                    userId = seller.Id.ToString();
                    userName = seller.Name;
                    userEmail = seller.Account.Email;
                    userRole = "seller";
                }
                else
                {
                    // User is not authenticated
                    Response.StatusCode = 403;
                    obj["error"] = "Only registered customers and sellers can send and receive messages.";
                    return obj;
                }

                obj["userId"] = userId;
                obj["userName"] = userName;
                obj["userEmail"] = userEmail;
                obj["userRole"] = userRole;

                // If an id parameter is specified, try to start a new chat
                // (customers are only be able to talk to sellers, and vice versa)
                if (id != null)
                {
                    if (customer != null)
                    {
                        var other = context.Sellers.Include(x => x.Account)
                            .FirstOrDefault(x => x.Id == id);
                        if (other != null)
                        {
                            obj["otherId"] = other.Id.ToString();
                            obj["otherName"] = other.Name;
                        }
                    }
                    else
                    {
                        var other = context.Customers.Include(x => x.Account)
                            .FirstOrDefault(x => x.Id == id);
                        if (other != null)
                        {
                            obj["otherId"] = other.Id.ToString();
                            obj["otherName"] = other.FirstName;
                        }
                    }
                }

                // Generate signature
                var keyByte = new ASCIIEncoding().GetBytes(_chatSecret);
                var userIdBytes = new ASCIIEncoding().GetBytes(userId);
                var hash = new HMACSHA256(keyByte).ComputeHash(userIdBytes);
                // Convert to HEX (Base 16)
                obj["signature"] = string.Concat(Array.ConvertAll(hash, x => x.ToString("x2")));
            }

            return obj;
        }
    }
}