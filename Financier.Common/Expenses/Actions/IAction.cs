using System;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public interface IAction
    {
        Types Type { get; }
        IProduct Product { get; }
        DateTime At { get; }
        Money Price { get; }
    }
}
