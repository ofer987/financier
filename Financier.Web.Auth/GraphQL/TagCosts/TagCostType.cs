using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Expenses.Models;
using Financier.Web.Auth.GraphQL.Tags;

// TODO: move to Types namespace
namespace Financier.Web.Auth.GraphQL.TagCosts
{
    public class TagCostType : ObjectGraphType<ItemListing>
    {
        public TagCostType(IDataLoaderContextAccessor dataLoaderAccessor)
        {
            Field(t => t.Tags, nullable: false, type: typeof(ListGraphType<TagType>));
            Field(t => t.Amount, nullable: false, type: typeof(DecimalGraphType));
        }
    }
}
