using GraphQL.Types;

using Financier.Common.Expenses.Models;

namespace Financier.Web.GraphQL.Tags
{
    public class TagQuery : ObjectGraphType
    {
        public static class Keys
        {
            // public static string FromMonth = "fromMonth";
            // public static string FromYear = "fromYear";
            // public static string toMonth = "toMonth";
            // public static string toYear = "toYear";
        }

        public TagQuery()
        {
            Field<ListGraphType<TagType>>(
                "list",
                resolve: context =>
                {
                    return Tag.GetAll();
                }
            );
        }
    }
}
