using System;

using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Func<string, Account> CreateAccount = (name) => new Account
        {
            Name = name
        };
    }
}

