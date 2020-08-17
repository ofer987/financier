using GraphQL;
using GraphQL.Types;

namespace Financier.Web.GraphQL.Activities
{
    public class OneHomeSchema : Schema
    {
        public OneHomeSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<OneHomeQuery>();
        }
    }
}
