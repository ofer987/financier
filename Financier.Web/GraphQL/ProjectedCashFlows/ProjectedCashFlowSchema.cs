using System;
using GraphQL.Types;

namespace Financier.Web.GraphQL.CashFlows
{
    public class ProjectedCashFlowSchema : Schema
    {
        public ProjectedCashFlowSchema(IServiceProvider provider) : base(provider)
        {
            Query = new ProjectedCashFlowQuery();
        }
    }
}
