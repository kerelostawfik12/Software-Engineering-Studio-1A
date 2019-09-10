using System.Linq;
using Isopoh.Cryptography.Argon2;
using Studio1BTask.Models;

namespace Studio1BTask.Services
{
    public class AccountService
    {
        private const string ArgonHeaderStuff = "$argon2id$v=19$m=65536,t=3,p=1$";

        public bool ValidateCredentials(string email, string password)
        {
            using (var context = new DbContext())
            {
                var accountToValidate = context.Accounts.First(account => account.Email == email);
                return ValidatePassword(password, accountToValidate);
            }
        }

        private string ObfuscatePassword(string password, Account account)
        {
            // i'm sorry hackers
            return account.Email.Substring(4, 9) + password + "bnepis" + (account.Id + 420) / 69 + account.Id % 3 +
                   account.Id % 401;
        }

        public string HashPassword(string password, Account account)
        {
            return Argon2.Hash(ObfuscatePassword(password, account)).Replace(ArgonHeaderStuff, "");
        }

        private bool ValidatePassword(string password, Account account)
        {
            return Argon2.Verify(ArgonHeaderStuff + account.PasswordHash, ObfuscatePassword(password, account));
        }
    }
}