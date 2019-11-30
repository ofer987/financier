using GraphQL;
using GraphQL.Types;

namespace Financier.Web.GraphQL.Analyses
{
    public class AnalysisSchema : Schema
    {
        public AnalysisSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<AnalysisQuery>();
        }
    }
}
