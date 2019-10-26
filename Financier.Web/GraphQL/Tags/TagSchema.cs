using GraphQL;
using GraphQL.Types;

namespace Financier.Web.GraphQL.Tags
{
    public class TagSchema : Schema
    {
        public TagSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<TagQuery>();
        }
    }
}
