using System;
using GraphQL.Types;

namespace Financier.Web.GraphQL.Tags
{
    public class TagSchema : Schema
    {
        public TagSchema(IServiceProvider provider) : base(provider)
        {
            Query = new TagQuery();
        }
    }
}
