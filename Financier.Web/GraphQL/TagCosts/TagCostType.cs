using GraphQL.Types;

using Financier.Common.Expenses.Models;
using Financier.Web.GraphQL.Tags;

namespace Financier.Web.GraphQL.TagCosts
{
    public class TagCostType : ObjectGraphType<ItemListing>
    {
        public TagCostType()
        {
            Field(t => t.Tags, nullable: false, type: typeof(ListGraphType<TagType>));
            Field(t => t.Amount, nullable: false, type: typeof(DecimalGraphType));
        }
    }
}
