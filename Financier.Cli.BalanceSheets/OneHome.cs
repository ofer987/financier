using System;

using Financier.Common.Expenses;
using Financier.Common.Models;

namespace Financier.Cli.BalanceSheets
{
    public class OneHome
    {
        public ProjectedBalanceSheet IncomeStatement { get; }

        public OneHome()
        {
            IncomeStatement = new ProjectedBalanceSheet(new DummyCashFlow(50), 0, 0, DateTime.Now);
            SetLivingHome();
        }

        public BalanceSheet GetValueAt(DateTime at)
        {
            return IncomeStatement.GetProjectionAt(at);
        }

        private void SetLivingHome()
        {
            var purchasedAt = new DateTime(2019, 1, 1);
            var livingHome = new Home(
                "Principal Home",
                purchasedAt,
                80000.00M
            );

            IncomeStatement.Purchase(livingHome, livingHome.DownPayment, livingHome.PurchasedAt);
            IncomeStatement.OneTimePurchase(new SimpleProduct("notary", 2000.00M), purchasedAt);
            IncomeStatement.Sell(livingHome, 500000.00M, new DateTime(2029, 1, 1));
        }
    }
}
