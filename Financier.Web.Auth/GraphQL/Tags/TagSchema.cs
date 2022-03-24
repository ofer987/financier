using System;
using GraphQL.Types;

namespace Financier.Web.Auth.GraphQL.Tags
{
    public class TagSchema : Schema
    {
        public TagSchema(IServiceProvider provider) : base(provider)
        {
            Query = new TagQuery();
        }
    }
}
