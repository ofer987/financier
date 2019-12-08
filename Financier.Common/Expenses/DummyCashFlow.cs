namespace Financier.Common.Expenses
{
    public class DummyCashFlow : ICashFlow
    {
        public decimal DailyProfit { get; }

        public DummyCashFlow(decimal DailyProfit)
        {
            DailyProfit = decimal.Round(DailyProfit, 2);
        }
    }
}
