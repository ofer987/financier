using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Expenses.Models;

namespace Financier.Web.GraphQL.ItemQueries
{
    public class MonthlyItemResultType : ObjectGraphType<MonthlyItemResult>
    {
        public MonthlyItemResultType(IDataLoaderContextAccessor dataLoaderAccessor)
        {
            Field("Year", t => t.Year, nullable: false, type: typeof(IntGraphType));
            Field("Month", t => t.Month, nullable: false, type: typeof(IntGraphType));
            Field("Amount", t => t.Amount, nullable: false, type: typeof(DecimalGraphType));
        }
    }
}
