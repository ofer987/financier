using System;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class NullAction : Action
    {
        public override bool IsSold => false;
        public override bool CanBuy => true;
        public override bool CanSell => false;
        public override bool IsNull => true;
        public override DateTime At => DateTime.MinValue;
        public override IEnumerable<Money> Transactions 
        {
            get
            {
                yield return Money.Zero;
            }
        }

        public static NullAction Instance = new NullAction();

        private NullAction() : base(Types.Null)
        {
        }
    }
}
