using System;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public interface IAction
    {
        Types Type { get; }
        IProduct Product { get; }
        DateTime At { get; }
        IEnumerable<Money> Transactions { get; }
        Money Price { get; }

        IAction Next { get; set; }

        bool IsSold { get; }
        bool IsLastAction { get; }
        bool CanBuy { get; }
        bool CanSell { get; }
        bool IsNull { get; }

        IEnumerable<IAction> GetActions();
    }
}
