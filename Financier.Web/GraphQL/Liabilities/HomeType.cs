using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Models;

namespace Financier.Web.GraphQL.Liabilities
{
    public class HomeType : ObjectGraphType<Home>
    {
        public HomeType(IDataLoaderContextAccessor _accessor)
        {
            Field(t => t.PurchasedAt, nullable: false, type: typeof(DateGraphType));
            Field(t => t.DownPayment, nullable: false, type: typeof(DecimalGraphType));
        }
    }
}
