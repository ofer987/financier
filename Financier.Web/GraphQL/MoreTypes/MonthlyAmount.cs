using System;
using GraphQL.DataLoader;
using GraphQL.Types;

namespace Financier.Web.GraphQL.MoreTypes
{
    public class MonthlyAmount : ObjectGraphType<ValueTuple<DateTime, decimal>>
    {
        public MonthlyAmount(IDataLoaderContextAccessor dataLoaderAccessor)
        {
            Field("At", t => t.Item1, nullable: false, type: typeof(DateGraphType));
            Field("Amount", t => t.Item2, nullable: false, type: typeof(DecimalGraphType));
        }
    }
}
