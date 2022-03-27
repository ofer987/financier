using GraphQL.Types;

namespace Financier.Web.Auth.GraphQL.CashFlows
{
    public class CashFlowSchema : Schema
    {
        public CashFlowSchema(IServiceProvider provider) : base(provider)
        {
            Query = new CashFlowQuery();
        }
    }
}
