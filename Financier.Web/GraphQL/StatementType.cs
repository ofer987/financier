using GraphQL.DataLoader;
using GraphQL.Types;

namespace Financier.Web.GraphQL
{
    public class StatementType : ObjectGraphType<ViewModels.Statement>
    {
        public StatementType(IDataLoaderContextAccessor dataLoaderAccessor)
        {
            Field(t => t.From, nullable: false, type: typeof(DateTimeGraphType));
            Field(t => t.To, nullable: false, type: typeof(DateTimeGraphType));
            Field(t => t.AssetCosts, nullable: false, type: typeof(ListGraphType<TagType>));
            Field(t => t.ExpenseCosts, nullable: false, type: typeof(ListGraphType<TagType>));
        }
    }
}
