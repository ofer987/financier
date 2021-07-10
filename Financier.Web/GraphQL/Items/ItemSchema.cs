using System;
using GraphQL.Utilities;
using GraphQL.Types;

namespace Financier.Web.GraphQL.Items
{
    public class ItemSchema : Schema
    {
        public ItemSchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<ItemQuery>();
            Mutation = provider.GetRequiredService<ItemMutation>();
        }
    }
}
