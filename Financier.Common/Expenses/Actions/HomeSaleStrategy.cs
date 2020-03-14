namespace Financier.Common.Expenses.Actions
{
    public class HomeSaleStrategy : ISaleStrategy
    {
        public decimal Requested { get; }

        public HomeSaleStrategy(decimal requested)
        {
            Requested = requested;
        }

        public decimal GetReturnedPrice(decimal requested)
        {
            var result = 0.00M;
            result += Requested;
            result -= GetFees();

            return decimal.Round(result, 2);
        }

        public decimal GetFees()
        {
            var result = 0.00M;
            result += GetRealtorFees();

            return decimal.Round(result, 2);
        }

        public decimal GetRealtorFees()
        {
            return 0.05M * Requested;
        }
    }
}
