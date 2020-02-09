using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Liabilities;

namespace Financier.Web.GraphQL.Liabilities
{
    public abstract class MortgageType<T> : ObjectGraphType<T> where T : IMortgage
    {
        public MortgageType(IDataLoaderContextAccessor _accessor)
        {
            Field(t => t.BaseValue, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.InitialValue, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.InitiatedAt, nullable: false, type: typeof(DateGraphType));
            Field(t => t.AmortisationPeriodInMonths, nullable: false, type: typeof(IntGraphType));
            Field(t => t.InterestRate, nullable: false, type: typeof(DecimalGraphType));
        }
    }
}
