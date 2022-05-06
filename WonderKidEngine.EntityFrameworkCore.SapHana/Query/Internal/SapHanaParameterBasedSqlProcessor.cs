#nullable enable

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    public class SapHanaParameterBasedSqlProcessor : RelationalParameterBasedSqlProcessor
    {
        private readonly ISapHanaOptions _options;
        private readonly SapHanaSqlExpressionFactory _sqlExpressionFactory;

        public SapHanaParameterBasedSqlProcessor(
            [NotNull] RelationalParameterBasedSqlProcessorDependencies dependencies,
            bool useRelationalNulls,
            ISapHanaOptions options)
            : base(dependencies, useRelationalNulls)
        {
            _sqlExpressionFactory = (SapHanaSqlExpressionFactory)Dependencies.SqlExpressionFactory;
            _options = options;
        }

        public override SelectExpression Optimize(SelectExpression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));
            Check.NotNull(parametersValues, nameof(parametersValues));

            selectExpression = base.Optimize(selectExpression, parametersValues, out canCache);


            if (_options.IndexOptimizedBooleanColumns)
            {
                selectExpression = (SelectExpression)new SapHanaBoolOptimizingExpressionVisitor(Dependencies.SqlExpressionFactory).Visit(selectExpression);
            }

            selectExpression = (SelectExpression)new SapHanaHavingExpressionVisitor(_sqlExpressionFactory).Visit(selectExpression);

            // Run the compatibility checks as late in the query pipeline (before the actual SQL translation happens) as reasonable.
            selectExpression = (SelectExpression)new SapHanaCompatibilityExpressionVisitor(_options).Visit(selectExpression);

            return selectExpression;
        }

        /// <inheritdoc />
        protected override SelectExpression ProcessSqlNullability(
            SelectExpression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));
            Check.NotNull(parametersValues, nameof(parametersValues));

            selectExpression = new SapHanaSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(selectExpression, parametersValues, out canCache);

            return selectExpression;
        }
    }
}
