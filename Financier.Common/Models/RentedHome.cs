using System;
using System.Collections.Generic;

using Financier.Common.Models.Expenses;

namespace Financier.Common.Models
{
    public class RentedHome : Product
    {
        public DateTime RentedAt => PurchasedAt;

        public RentedHome(DateTime rentedAt, IDictionary<string, decimal> expenses) : base(rentedAt)
        {
            Liabilities.Add(new MonthlyExpenses(this, expenses));
        }
    }
}
