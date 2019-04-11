using GraphQL.Types;

using Financier.Common.Expenses.Models;

namespace Financier.Web
{
    public class ItemQuery : ObjectGraphType
    {
        public ItemQuery()
        {
            Field<ListGraphType<ItemType>>(
                "items",
                // resolve: context => Item.GetAll()
                resolve: _ => Item.GetAll()
            );
        }
    }
}
