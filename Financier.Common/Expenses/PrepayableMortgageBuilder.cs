using System;

using Financier.Common.Liabilities;

namespace Financier.Common.Expenses
{
    // TODO: rename to MortgagePaymentsBuilder and convert
    // to builder pattern
    public class PrepayableMortgageBuilder
    {
        public IMortgage Mortgage { get; }
        decimal MaximumAllowedPrepaymentPercentage { get; }

        public PrepayableMortgageBuilder(IMortgage baseMortgage, decimal maximumAllowedPrepaymentPercentage = 0.10M)
        {
            Mortgage = baseMortgage;
            MaximumAllowedPrepaymentPercentage = maximumAllowedPrepaymentPercentage;
        }

        public PrepayableMortgage Build()
        {
            var result = new PrepayableMortgage(Mortgage, MaximumAllowedPrepaymentPercentage);
            decimal mortgageBalance = result.InitialValue;
            var startAt = result.InitiatedAt;
            var at = startAt;
            while (mortgageBalance > 0.00M)
            {
                // TODO: verify whether 0:00:00.00 - 1 milliseconds
                // is the previous day
                var endOfMonth = new DateTime(at.Year, at.Month, 1)
                    .AddMonths(1)
                    .AddMilliseconds(-1);
                // FIXME: Is this API to retrieve the 12th month?
                if (endOfMonth.Month == 12)
                {
                    // FIXME: Figure out correct amount
                    var prepayment = CreatePrepayment(
                        result.GetBalance(at),
                        result.Prepayments.MaximumAnnualTotal
                    );
                    result.AddPrepayment(
                        endOfMonth,
                        prepayment
                    );

                    startAt = endOfMonth;
                    mortgageBalance = result.GetBalance(endOfMonth);
                }
                else
                {
                    mortgageBalance = result.GetBalance(at);
                }

                at = at.AddMonths(1);
            }

            return result;
        }

        private decimal CreatePrepayment(decimal balance, decimal maximumTotal)
        {
            return balance < maximumTotal
                ? balance
                : maximumTotal;
        }
    }
}
