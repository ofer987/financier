using GraphQL;
using GraphQL.Types;

namespace Financier.Web.GraphQL.Expenses
{
    public class MyHomeSchema : Schema
    {
        public MyHomeSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<MyHomeQuery>();
        }
    }
}
