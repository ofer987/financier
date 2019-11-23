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
            Field(t => t.At, nullable: false, type: typeof(DateGraphType));
            Field("amount", t => t.TheRealAmount.ToString("#0.00"), nullable: false, type: typeof(StringGraphType));
            Field(t => t.ItemId, nullable: false, type: typeof(StringGraphType));
        }
    }
}
