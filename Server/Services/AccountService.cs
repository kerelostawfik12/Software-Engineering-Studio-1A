using System.Collections.Generic;
using System.Linq;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Studio1BTask.Models;
using DbContext = Studio1BTask.Models.DbContext;

namespace Studio1BTask.Services
{
    public class AccountService
    {
        private const int ArgonTimeCost = 1;
        private const int ArgonMemoryCost = 65536;
        private const string ArgonHeaderStuff = "$argon2id$v=19$m=65536,t=1,p=1$";

        public Account ValidateCredentials(string email, string password, DbContext context)
        {
            var accountToValidate = context.Accounts.FirstOrDefault(account => account.Email == email);
            if (accountToValidate == null)
                return null;

            if (ValidatePassword(password, accountToValidate))
                return accountToValidate;

            return null;
        }

        private string ObfuscatePassword(string password, Account account)
        {
            // i'm sorry hackers
            return account.Email.Substring(4, 9) + password + "bnepis" + (account.Id + 420) / 69 + account.Id % 3 +
                   account.Id % 401;
        }

        public string HashPassword(string password, Account account)
        {
            return Argon2.Hash(ObfuscatePassword(password, account), ArgonTimeCost).Replace(ArgonHeaderStuff, "");
        }

        private bool ValidatePassword(string password, Account account)
        {
            var isValid = Argon2.Verify(ArgonHeaderStuff + account.PasswordHash, ObfuscatePassword(password, account));
            return isValid;
        }

        public Session ValidateSession(int sessionId, string token, DbContext context)
        {
            var session = context.Sessions.Find(sessionId);
            // If the session doesn't exist, return null.
            if (session == null)
                return null;
            // If the session exists, but the provided token is wrong, return null.
            var isValid = token == session.Token;
            if (!isValid)
                return null;
            // The session is valid, and the session object will be returned.
            return session;
        }

        public Session ValidateSession(string sessionId, string token, DbContext context)
        {
            try
            {
                return ValidateSession(int.Parse(sessionId), token, context);
            }
            catch
            {
                return null;
            }
        }

        public Session ValidateSession(dynamic requestCookies, DbContext context)
        {
            try
            {
                var sessionId = int.Parse(requestCookies["sessionId"]);
                var token = requestCookies["token"].ToString();
                return ValidateSession(sessionId, token, context);
            }
            catch
            {
                return null;
            }
        }

        // If authorised, this will return an object with information about the user.
        // If more information is needed, authenticate and get the user directly.
        public Dictionary<string, dynamic> ValidateUser(int sessionId, int accountId, string token, DbContext context)
        {
            var session = ValidateAccountSession(sessionId, accountId, token, context);
            if (session == null)
                return null;

            // Return either a customer or seller object, depending on what type the user is.
            // Account objects need to be stripped out for two reasons: security, and to avoid loops in the JSON.
            if (session.Account.Type == 'c')
            {
                var obj = new Dictionary<string, dynamic>();
                var customer = context.Customers.Find(accountId);
                obj["type"] = 'c';
                obj["id"] = customer.Id;
                obj["firstName"] = customer.FirstName;
                obj["lastName"] = customer.LastName;
                return obj;
            }

            if (session.Account.Type == 's')
            {
                var obj = new Dictionary<string, dynamic>();
                var seller = context.Sellers.Find(accountId);
                obj["type"] = 's';
                obj["id"] = seller.Id;
                obj["name"] = seller.Name;
                return obj;
            }

            if (session.Account.Type == 'a')
            {
                var obj = new Dictionary<string, dynamic>();
                var account = context.Accounts.Find(accountId);
                obj["type"] = 'a';
                obj["id"] = account.Id;
                return obj;
            }

            return null;
        }

        private static Session ValidateAccountSession(int sessionId, int accountId, string token, DbContext context)
        {
            var session = context.Sessions.Include(x => x.Account).FirstOrDefault(x => x.Id == sessionId);
            // If session is nonexistent, or not linked with an account, return null.
            if (session?.AccountId == null)
                return null;
            // If token and account id provided do not match with the corresponding entries in the database, return null.
            var isValid = token == session.Token && session.AccountId == accountId;
            if (!isValid)
                return null;

            return session;
        }

        public Account ValidateAccountSession(dynamic requestCookies, DbContext context)
        {
            try
            {
                var sessionId = int.Parse(requestCookies["sessionId"]);
                var accountId = int.Parse(requestCookies["accountId"]);
                var token = requestCookies["token"].ToString();
                return ValidateAccountSession(sessionId, accountId, token, context);
            }
            catch
            {
                return null;
            }
        }

        private static Customer ValidateCustomerSession(int sessionId, int accountId, string token, DbContext
            context, bool includeAccountObject = false)
        {
            var session = ValidateAccountSession(sessionId, accountId, token, context);
            // If account not valid or not authorised, return null.
            if (session == null)
                return null;
            var account = session.Account;
            // If account is not a customer, return null.
            if (account.Type != 'c') return null;
            Customer customer;
            if (includeAccountObject)
                customer = context.Customers.Include(x => x.Account)
                    .FirstOrDefault(x => x.Id == account.Id);
            else
                customer = context.Customers.Find(account.Id);

            return customer;
        }

        public Customer ValidateCustomerSession(dynamic requestCookies, DbContext context, bool includeAccountObject
            = false)
        {
            try
            {
                var sessionId = int.Parse(requestCookies["sessionId"]);
                var accountId = int.Parse(requestCookies["accountId"]);
                var token = requestCookies["token"].ToString();
                return ValidateCustomerSession(sessionId, accountId, token, context, includeAccountObject);
            }
            catch
            {
                return null;
            }
        }

        private static Seller ValidateSellerSession(int sessionId, int accountId, string token, DbContext
            context, bool includeAccountObject = false)
        {
            var session = ValidateAccountSession(sessionId, accountId, token, context);
            // If account not valid or not authorised, return null.
            if (session == null)
                return null;
            var account = session.Account;
            // If account is not a seller, return null.
            if (account.Type != 's') return null;
            Seller seller;
            if (includeAccountObject)
                seller = context.Sellers.Include(x => x.Account)
                    .FirstOrDefault(x => x.Id == account.Id);
            else
                seller = context.Sellers.Find(account.Id);

            return seller;
        }

        public Seller ValidateSellerSession(dynamic requestCookies, DbContext context, bool includeAccountObject
            = false)
        {
            try
            {
                var sessionId = int.Parse(requestCookies["sessionId"]);
                var accountId = int.Parse(requestCookies["accountId"]);
                var token = requestCookies["token"].ToString();
                return ValidateSellerSession(sessionId, accountId, token, context, includeAccountObject);
            }
            catch
            {
                return null;
            }
        }

        public bool ValidateAdminSession(dynamic requestCookies, DbContext context)
        {
            try
            {
                var sessionId = int.Parse(requestCookies["sessionId"]);
                var accountId = int.Parse(requestCookies["accountId"]);
                var token = requestCookies["token"].ToString();
                return ValidateAdminSession(sessionId, accountId, token, context);
            }
            catch
            {
                return false;
            }
        }

        private static bool ValidateAdminSession(int sessionId, int accountId, string token, DbContext context)
        {
            var session = ValidateAccountSession(sessionId, accountId, token, context);
            // If account not valid or not authorised, return null.
            if (session == null)
                return false;
            var account = session.Account;
            // If account is not an admin, return false.
            return account.Type == 'a';
        }

        public bool IsEmailValid(string email)
        {
            if (email.Length > 3 && email.Length < 80 && email.Contains(".") && email.Contains("@"))
                return true;
            return false;
        }

        // TODO: Add more password restrictions
        public bool IsPasswordValid(string password, out string reason)
        {
            reason = "";
            if (password.Length < 8)
            {
                reason = "Password must be 8 or more characters long.";
                return false;
            }

            if (password.Length > 80)
            {
                reason = "Password is too long.";
                return false;
            }

            return true;
        }

        public bool IsPasswordValid(string password)
        {
            return IsPasswordValid(password, out var x);
        }
    }
}