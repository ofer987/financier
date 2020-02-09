using GraphQL;
using GraphQL.Types;

namespace Financier.Web.GraphQL.CashFlows
{
    public class CashFlowSchema : Schema
    {
        public CashFlowSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<CashFlowQuery>();
        }
    }
}
