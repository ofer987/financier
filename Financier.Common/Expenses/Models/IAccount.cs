using System;
using System.Collections.Generic;

namespace Financier.Common.Expenses.Models
{
    public interface IAccount
    {
        string Name { get; set; }

        List<Card> Cards { get; set; }

        IEnumerable<Item> GetAllItems(DateTime from, DateTime to);

        void Delete();
    }
}
