using GraphQL;
using GraphQL.Types;

namespace Financier.Web.GraphQL.Expenses
{
    public class BalanceSheetSchema : Schema
    {
        public BalanceSheetSchema(IDependencyResolver resolver) : base(resolver)
        {
        }
    }
}
