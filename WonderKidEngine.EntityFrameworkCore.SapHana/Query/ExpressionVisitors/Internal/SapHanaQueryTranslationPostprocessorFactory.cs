using Microsoft.EntityFrameworkCore.Query;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal
{
    public class SapHanaQueryTranslationPostprocessorFactory : IQueryTranslationPostprocessorFactory
    {
        private readonly QueryTranslationPostprocessorDependencies _dependencies;
        private readonly RelationalQueryTranslationPostprocessorDependencies _relationalDependencies;
        private readonly ISapHanaOptions _options;
        private readonly SapHanaSqlExpressionFactory _sqlExpressionFactory;

        public SapHanaQueryTranslationPostprocessorFactory(
            QueryTranslationPostprocessorDependencies dependencies,
            RelationalQueryTranslationPostprocessorDependencies relationalDependencies,
            ISapHanaOptions options,
            ISqlExpressionFactory sqlExpressionFactory)
        {
            _dependencies = dependencies;
            _relationalDependencies = relationalDependencies;
            _options = options;
            _sqlExpressionFactory = (SapHanaSqlExpressionFactory)sqlExpressionFactory;
        }

        public virtual QueryTranslationPostprocessor Create(QueryCompilationContext queryCompilationContext)
            => new SapHanaQueryTranslationPostprocessor(
                _dependencies,
                _relationalDependencies,
                queryCompilationContext,
                _options,
                _sqlExpressionFactory);
    }
}
