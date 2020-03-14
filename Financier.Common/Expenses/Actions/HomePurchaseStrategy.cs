using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class HomePurchaseStrategy : IPurchaseStrategy
    {
        public static DateTime BaseDate = new DateTime(2018, 1, 1);
        public Money Requested { get; }
        public DateTime At => Requested.At;

        public HomePurchaseStrategy(Money requested)
        {
            Requested = requested;
        }

        public IEnumerable<Money> GetReturnedPrice()
        {
            yield return Requested.Reverse;

            foreach (var fee in GetFees().Select(item => item.Reverse))
            {
                yield return fee;
            }
        }

        public IEnumerable<Money> GetFees()
        {
            return Enumerable.Empty<Money>()
                .Concat(GetNotaryFees())
                .Concat(GetMunicipalTaxes())
                .Concat(GetMovingFees());
        }

        public IEnumerable<Money> GetNotaryFees()
        {
            yield return new Money(1000.00M, BaseDate);
        }

        public IEnumerable<Money> GetMunicipalTaxes()
        {
            yield return new Money(8500.00M, BaseDate);
        }

        public IEnumerable<Money> GetMovingFees()
        {
            yield return new Money(800.00M, BaseDate);
        }
    }
}
