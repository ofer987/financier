using GraphQL;
using GraphQL.Types;

namespace Financier.Web.GraphQL
{
    public class StatementSchema : Schema
    {
        public StatementSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<StatementQuery>();
        }
    }
}
