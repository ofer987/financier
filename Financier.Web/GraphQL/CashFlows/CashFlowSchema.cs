using GraphQL.Types;
using GraphQL.Instrumentation;

namespace Financier.Web.GraphQL.CashFlows
{
    public class CashFlowSchema : Schema
    {
        public CashFlowSchema(IServiceProvider provider, IEnumerable<IFieldMiddleware> middlewares) : base(provider)
        {
            Query = new CashFlowQuery();

            foreach (var middleware in middlewares)
            {
                FieldMiddleware.Use(middleware);
            }
        }
    }
}
