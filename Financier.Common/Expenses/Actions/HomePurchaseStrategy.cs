namespace Financier.Common.Expenses.Actions
{
    public class HomePurchaseStrategy : IPurchaseStrategy
    {
        public decimal Requested { get; }

        public HomePurchaseStrategy(decimal requested)
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
            result += GetNotaryFees();
            result += GetMunicipalTaxes();
            result += GetMovingFees();

            return decimal.Round(result, 2);
        }

        public decimal GetNotaryFees()
        {
            return 1000.00M;
        }

        public decimal GetMunicipalTaxes()
        {
            return 8500.00M;
        }

        public decimal GetMovingFees()
        {
            return 800.00M;
        }
    }
}
