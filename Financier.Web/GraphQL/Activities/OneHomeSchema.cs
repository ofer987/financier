using System;
using GraphQL.Utilities;
using GraphQL.Types;

namespace Financier.Web.GraphQL.Activities
{
    public class OneHomeSchema : Schema
    {
        public OneHomeSchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<OneHomeQuery>();
        }
    }
}
