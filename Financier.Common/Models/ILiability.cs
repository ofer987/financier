namespace Financier.Common.Models
{
    public interface ILiability
    {
        decimal CostAt(int monthAfterInception);
    }
}
