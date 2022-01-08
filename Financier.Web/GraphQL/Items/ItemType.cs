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
            Field(t => t.Description, nullable: false, type: typeof(StringGraphType));
            Field(t => t.PostedAt, nullable: false, type: typeof(StringGraphType));
            Field(t => t.Amount, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.ItemId, nullable: false, type: typeof(StringGraphType));
            Field("tags", t => t.Tags.Select(tag => tag.Name), nullable: false, type: typeof(ListGraphType<StringGraphType>));
        }
    }
}
