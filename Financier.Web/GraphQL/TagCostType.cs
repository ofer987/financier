using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Expenses.Models;

// TODO: move to Types namespace
namespace Financier.Web.GraphQL
{
    public class TagCostType : ObjectGraphType<TagCost>
    {
        public TagCostType(IDataLoaderContextAccessor dataLoaderAccessor)
        {
            Field(t => t.Tags, nullable: false, type: typeof(ListGraphType<TagType>));
            Field(t => t.Amount, nullable: false, type: typeof(DecimalGraphType));
        }
    }
}
