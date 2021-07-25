using System;
using GraphQL.Types;

namespace Financier.Web.GraphQL.ItemQueries
{
    public class ItemQuerySchema : Schema
    {
        public ItemQuerySchema(IServiceProvider provider) : base(provider)
        {
            Query = new ItemQueryQuery();
        }
    }
}
