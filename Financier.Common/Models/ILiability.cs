using System;

namespace Financier.Common.Models
{
    public interface ILiability
    {
        IProduct Product { get; }

        decimal CostAt(int monthAfterInception);

        decimal CostAt(DateTime at);

        decimal CostBy(int monthAfterInception);

        decimal CostBy(DateTime at);
    }
}
