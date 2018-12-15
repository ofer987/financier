namespace Financier.Common.Models
{
    public interface ILiability : IProduct
    {
        decimal CostAt(int monthAfterInception);

        decimal CostBy(int monthAfterInception);
    }
}
