using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Expenses.Models;

// TODO: move to Types namespace
namespace Financier.Web.GraphQL
{
    public class TagType : ObjectGraphType<Tag>
    {
        public TagType(IDataLoaderContextAccessor dataLoaderAccessor)
        {
        }
    }
}
