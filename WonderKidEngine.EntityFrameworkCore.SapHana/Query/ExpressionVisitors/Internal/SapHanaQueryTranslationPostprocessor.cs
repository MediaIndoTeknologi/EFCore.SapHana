using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal
{
    public class SapHanaQueryTranslationPostprocessor : RelationalQueryTranslationPostprocessor
    {
        private readonly ISapHanaOptions _options;
        private readonly SapHanaSqlExpressionFactory _sqlExpressionFactory;

        public SapHanaQueryTranslationPostprocessor(
            QueryTranslationPostprocessorDependencies dependencies,
            RelationalQueryTranslationPostprocessorDependencies relationalDependencies,
            QueryCompilationContext queryCompilationContext,
            ISapHanaOptions options,
            SapHanaSqlExpressionFactory sqlExpressionFactory)
            : base(dependencies, relationalDependencies, queryCompilationContext)
        {
            _options = options;
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public override Expression Process(Expression query)
        {
            query = base.Process(query);

            query = new SapHanaJsonParameterExpressionVisitor(_sqlExpressionFactory, _options).Visit(query);

            return query;
        }
    }
}
