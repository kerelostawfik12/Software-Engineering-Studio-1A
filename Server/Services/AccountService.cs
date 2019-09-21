using System.Linq;
using Isopoh.Cryptography.Argon2;
using Studio1BTask.Models;

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
    }
}