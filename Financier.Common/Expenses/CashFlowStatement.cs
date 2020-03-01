using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Extensions;
using Financier.Common.Models;
using Financier.Common.Expenses.Actions;

namespace Financier.Common.Expenses
{
    public class CashFlowStatement
    {
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public Money InitialCash { get; set; } = Money.Zero;
        public Money InitialDebt { get; set; } = Money.Zero;
        public ICashFlow CashFlow { get; set; }
        public decimal DailyProfit => CashFlow.DailyProfit;
        public IDictionary<DateTime, IList<Money>> CashAdjustments = new Dictionary<DateTime, IList<Money>>();

        // private List<Home> homes { get; } = new List<Home>();
        // public IReadOnlyList<Home> Homes => homes.AsReadOnly();

        // public Activity ProductHistory { get; } = new Activity();

        public CashFlowStatement(IDictionary<DateTime, IList<Money>> cashAdjustments, Activity productHistory, DateTime startAt, DateTime endAt) : this(productHistory, startAt, endAt)
        {
            CashAdjustments = cashAdjustments;
        }

        public CashFlowStatement(Activity productHistory, DateTime startAt, DateTime endAt)
        {
            if (endAt < startAt)
            {
                throw new ArgumentOutOfRangeException(nameof(endAt), endAt, $"CashFlow statement should end ({endAt}) after the start date ({startAt})");
            }

            StartAt = startAt;
            EndAt = endAt;

            // ProductHistory = productHistory;
        }

        // public IEnumerable<Money> GetCashChanges()
        // {
        //     foreach (var action in ProductHistory.GetHistories()
        //                                          .SelectMany(action => action)
        //                                          .OrderBy(action => action.At))
        //     {
        //         switch (action.Type)
        //         {
        //             case Types.Purchase:
        //                 yield return new Money(0 - action.Price.Value, action.Price.At);
        //                 break;
        //             case Types.Sale:
        //                 yield return action.Price;
        //                 break;
        //             case Types.Null:
        //                 break;
        //         }
        //     }
        // }
    }
}
