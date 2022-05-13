using GraphQL.Types;
using GraphQL.Instrumentation;

namespace Financier.Web.GraphQL.Items
{
    public class ItemSchema : Schema
    {
        public ItemSchema(IServiceProvider provider, IEnumerable<IFieldMiddleware> middlewares) : base(provider)
        {
            Query = new ItemQuery();
            Mutation = new ItemMutation();

            foreach (var middleware in middlewares)
            {
                FieldMiddleware.Use(middleware);
            }
        }
    }
}
