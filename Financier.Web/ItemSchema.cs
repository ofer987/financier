using GraphQL;
using GraphQL.Types;

namespace Financier.Web
{
    public class ItemSchema : Schema
    {
        public ItemSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<ItemQuery>();
        }
    }
}
