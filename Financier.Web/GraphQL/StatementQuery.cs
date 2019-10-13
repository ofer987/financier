using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;

using Financier.Web.ViewModels;

namespace Financier.Web.GraphQL
{
    public class StatementQuery : ObjectGraphType
    {
        public static class Keys
        {
            public static string Year = "year";
            public static string Month = "month";
        }

        public StatementQuery()
        {
            Field<StatementType>(
                "statement",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.Year
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.Month
                    }
                ),
                resolve: context =>
                {
                    var year = context.GetArgument<int>(Keys.Year);
                    var month = context.GetArgument<int>(Keys.Month);

                    return new Statement(year, month);
                }
            );

            Field<ListGraphType<StatementType>>(
                "statements",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.Year
                    }
                ),
                resolve: context =>
                {
                    var year = context.GetArgument<int>(Keys.Year);

                    // Should this be converted to an array?
                    return GetMonthlyStatements(year).ToList();
                }
            );
        }

        private IEnumerable<Statement> GetMonthlyStatements(int year)
        {
            for (var month = 1; month <= 12; month += 1)
            {
                yield return new Statement(year, month);
            }
        }
    }
}
