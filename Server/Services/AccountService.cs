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
            if (session == null)
                return null;
            var isValid = token == session.Token;
            if (!isValid)
                return null;
            return session;
        }

        public dynamic ValidateUser(int sessionId, int accountId, string token, DbContext context)
        {
            var session = context.Sessions.Include(x => x.Account).FirstOrDefault(x => x.Id == sessionId);
            // If session is nonexistent, or not linked with an account, return null.
            if (session?.AccountId == null)
                return null;
            // If token and account id provided do not match with the corresponding entries in the database, return null.
            var isValid = token == session.Token && session.AccountId == accountId;
            if (!isValid)
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

            return null;
        }
    }
}