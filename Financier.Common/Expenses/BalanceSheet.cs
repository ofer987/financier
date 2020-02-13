using System;
using System.Linq;

using Financier.Common.Models;

namespace Financier.Common.Expenses
{
    public class BalanceSheet
    {
        public Money InitialCash { get; }
        public Money InitialDebt { get; }
        public ICashFlow CashFlow { get; }
        public decimal DailyProfit => CashFlow.DailyProfit;
        public Home Home { get; }

        public BalanceSheet(Money cash, Money debt, ICashFlow cashFlow, Home home)
        {
            InitialCash = cash;
            InitialDebt = debt;
            Home = home;
        }

        public decimal GetAssets(IInflation inflation, DateTime at)
        {
            var result = 0.00M
                + InitialCash.GetValueAt(inflation, at)
                + Home.DownPayment.GetValueAt(inflation, at)
                + Home.Financing.GetMonthlyPayments(at)
                    .Select(payment => payment.Principal.GetValueAt(inflation, at).Value)
                    .Sum();

            return decimal.Round(result, 2);
        }

        public decimal GetLiabilities(IInflation inflation, DateTime at)
        {
            var result = 0.00M
                + InitialDebt.GetValueAt(inflation, at)
                + Home.Financing.GetBalance(at);

            return decimal.Round(result, 2);
        }

        public decimal GetNetWorth(IInflation inflation, DateTime at)
        {
            return 0.00M
                + GetAssets(inflation, at) 
                - GetLiabilities(inflation, at);
        }

        public virtual decimal GetBalance(int months)
        {
            throw new NotImplementedException();
        }
    }
}
