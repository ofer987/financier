using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Actions;

namespace Financier.Common.Flows
{
    public class Simple 
    {
        public List<IAction> Actions { get; }

        public DateTime StartedAt { get; }

        public decimal StartingBalance { get; }

        public Simple(decimal startingBalance, DateTime startedAt)
        {
            StartingBalance = startingBalance;
            StartedAt = startedAt;
        }

        public decimal GetBalance(DateTime at)
        {
            return Actions
                .Where(action => action.At <= at)
                .OrderBy(action => action.At)
                .Aggregate(StartingBalance, (balance, action) => balance + action.PriceAt(at));
        }
    }
}
