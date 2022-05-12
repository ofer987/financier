using GraphQL.Types;

namespace Financier.Web.GraphQL.Items
{
    public class ItemSchema : Schema
    {
        public ItemSchema(IServiceProvider provider) : base(provider)
        {
            Query = new ItemQuery();
            Mutation = new ItemMutation();
        }
    }
}
