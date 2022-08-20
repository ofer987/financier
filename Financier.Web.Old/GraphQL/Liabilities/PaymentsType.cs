using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Liabilities;

namespace Financier.Web.GraphQL.Liabilities
{
    public class PaymentsType : ObjectGraphType<Payments>
    {
        public PaymentsType(IDataLoaderContextAccessor _accessor)
        {
        }
    }
}
