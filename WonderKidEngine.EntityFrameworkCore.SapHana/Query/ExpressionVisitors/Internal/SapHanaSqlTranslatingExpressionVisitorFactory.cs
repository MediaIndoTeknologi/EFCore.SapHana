using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionTranslators.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal
{
    public class SapHanaSqlTranslatingExpressionVisitorFactory : IRelationalSqlTranslatingExpressionVisitorFactory
    {
        private readonly RelationalSqlTranslatingExpressionVisitorDependencies _dependencies;

        public SapHanaSqlTranslatingExpressionVisitorFactory(
            [NotNull] RelationalSqlTranslatingExpressionVisitorDependencies dependencies,
            [NotNull] IServiceProvider serviceProvider)
        {
            _dependencies = dependencies;
        }

        public virtual RelationalSqlTranslatingExpressionVisitor Create(
            QueryCompilationContext queryCompilationContext,
            QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor)
            => new SapHanaSqlTranslatingExpressionVisitor(
                _dependencies,
                queryCompilationContext,
                queryableMethodTranslatingExpressionVisitor);
    }
}
