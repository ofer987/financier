using GraphQL;
using GraphQL.Types;

namespace Financier.Web.GraphQL.ItemQueries
{
    public class ItemQuerySchema : Schema
    {
        public ItemQuerySchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<ItemQueryQuery>();
        }
    }
}
