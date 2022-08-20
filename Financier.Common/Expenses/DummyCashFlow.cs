namespace Financier.Common.Expenses
{
    public class DummyCashFlow : CashFlow
    {
        public override decimal DailyProfit { get; }

        public DummyCashFlow(decimal dailyProfit) : base(string.Empty)
        {
            DailyProfit = decimal.Round(dailyProfit, 2);
        }
    }
}
