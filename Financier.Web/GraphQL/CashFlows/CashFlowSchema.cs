using System;
using GraphQL.Utilities;
using GraphQL.Types;

namespace Financier.Web.GraphQL.CashFlows
{
    public class CashFlowSchema : Schema
    {
        public CashFlowSchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<CashFlowQuery>();
        }
    }
}
