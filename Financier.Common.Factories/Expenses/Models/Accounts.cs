using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Account NewAccount(string name)
        {
            return new Account
            {
                Name = name
            };
        }

        public static Account CreateAccount(string name)
        {
            var account = NewAccount(name);
            using (var db = new Context())
            {
                db.Accounts.Add(account);
                db.SaveChanges();
            }

            return account;
        }
    }
}

