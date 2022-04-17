using System;
using GraphQL.Types;

namespace Financier.Web.GraphQL.Activities
{
    public class OneHomeSchema : Schema
    {
        public OneHomeSchema(IServiceProvider provider) : base(provider)
        {
            Query = new OneHomeQuery();
        }
    }
}
