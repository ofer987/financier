namespace Financier.Common.Expenses
{
    public class DummyCashFlow : ICashFlow
    {
        public decimal DailyProfit { get; }

        public DummyCashFlow(decimal dailyProfit)
        {
            DailyProfit = decimal.Round(dailyProfit, 2);
        }
    }
}
