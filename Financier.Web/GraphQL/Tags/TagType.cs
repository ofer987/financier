using GraphQL.Types;

using Financier.Common.Expenses.Models;

// TODO: move to Types namespace
namespace Financier.Web.GraphQL.Tags
{
    public class TagType : ObjectGraphType<Tag>
    {
        public TagType()
        {
            Field(t => t.Name, nullable: false, type: typeof(StringGraphType));
        }
    }
}
