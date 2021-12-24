using System.Linq;

using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Expenses.Models;

namespace Financier.Web.GraphQL.Items
{
    public class ItemType : ObjectGraphType<Item>
    {
        public ItemType(IDataLoaderContextAccessor dataLoaderAccessor)
        {
            Field(t => t.Id, nullable: false, type: typeof(IdGraphType));
            Field(t => t.ItemId, nullable: false, type: typeof(StringGraphType));
            Field("name", t => t.Description, nullable: false, type: typeof(StringGraphType));
            Field("at", t => t.PostedAt, nullable: false, type: typeof(DateGraphType));
            Field(t => t.Type, nullable: false, type: typeof(EnumerationGraphType<ItemTypes>));
            Field("amount", t => t.Amount.ToString("#0.00"), nullable: false, type: typeof(StringGraphType));
            Field("tags", t => t.Tags.Select(tag => tag.Name), nullable: false, type: typeof(ListGraphType<StringGraphType>));
        }
    }
}
