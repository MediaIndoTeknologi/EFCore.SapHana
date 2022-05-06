using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.Expressions.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    /// <inheritdoc />
    public class SapHanaSqlNullabilityProcessor : SqlNullabilityProcessor
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        /// <summary>
        /// Creates a new instance of the <see cref="SapHanaSqlNullabilityProcessor" /> class.
        /// </summary>
        /// <param name="dependencies">Parameter object containing dependencies for this class.</param>
        /// <param name="useRelationalNulls">A bool value indicating whether relational null semantics are in use.</param>
        public SapHanaSqlNullabilityProcessor(
            [NotNull] RelationalParameterBasedSqlProcessorDependencies dependencies,
            bool useRelationalNulls)
            : base(dependencies, useRelationalNulls)
            => _sqlExpressionFactory = dependencies.SqlExpressionFactory;

        /// <inheritdoc />
        protected override SqlExpression VisitCustomSqlExpression(
            SqlExpression sqlExpression, bool allowOptimizedExpansion, out bool nullable)
            => sqlExpression switch
            {
                SapHanaBinaryExpression binaryExpression => VisitBinary(binaryExpression, allowOptimizedExpansion, out nullable),
                SapHanaCollateExpression collateExpression => VisitCollate(collateExpression, allowOptimizedExpansion, out nullable),
                SapHanaComplexFunctionArgumentExpression complexFunctionArgumentExpression => VisitComplexFunctionArgument(complexFunctionArgumentExpression, allowOptimizedExpansion, out nullable),
                SapHanaMatchExpression matchExpression => VisitMatch(matchExpression, allowOptimizedExpansion, out nullable),
                SapHanaJsonArrayIndexExpression arrayIndexExpression => VisitJsonArrayIndex(arrayIndexExpression, allowOptimizedExpansion, out nullable),
                SapHanaJsonTraversalExpression jsonTraversalExpression => VisitJsonTraversal(jsonTraversalExpression, allowOptimizedExpansion, out nullable),
                SapHanaRegexpExpression regexpExpression => VisitRegexp(regexpExpression, allowOptimizedExpansion, out nullable),
                SapHanaColumnAliasReferenceExpression columnAliasReferenceExpression => VisitColumnAliasReference(columnAliasReferenceExpression, allowOptimizedExpansion, out nullable),
                _ => base.VisitCustomSqlExpression(sqlExpression, allowOptimizedExpansion, out nullable)
            };

        private SqlExpression VisitColumnAliasReference(SapHanaColumnAliasReferenceExpression columnAliasReferenceExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(columnAliasReferenceExpression, nameof(columnAliasReferenceExpression));

            var expression = Visit(columnAliasReferenceExpression.Expression, allowOptimizedExpansion, out nullable);

            return columnAliasReferenceExpression.Update(columnAliasReferenceExpression.Alias, expression);
        }

        /// <summary>
        /// Visits a <see cref="SapHanaBinaryExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="binaryExpression">A <see cref="SapHanaBinaryExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitBinary(
            [NotNull] SapHanaBinaryExpression binaryExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(binaryExpression, nameof(binaryExpression));

            var left = Visit(binaryExpression.Left, allowOptimizedExpansion, out var leftNullable);
            var right = Visit(binaryExpression.Right, allowOptimizedExpansion, out var rightNullable);

            nullable = leftNullable || rightNullable;

            return binaryExpression.Update(left, right);
        }

        /// <summary>
        /// Visits a <see cref="SapHanaCollateExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="collateExpression">A <see cref="SapHanaCollateExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitCollate(
            [NotNull] SapHanaCollateExpression collateExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(collateExpression, nameof(collateExpression));

            var valueExpression = Visit(collateExpression.ValueExpression, allowOptimizedExpansion, out nullable);

            return collateExpression.Update(valueExpression);
        }

        /// <summary>
        /// Visits a <see cref="SapHanaComplexFunctionArgumentExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="complexFunctionArgumentExpression">A <see cref="SapHanaComplexFunctionArgumentExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitComplexFunctionArgument(
            [NotNull] SapHanaComplexFunctionArgumentExpression complexFunctionArgumentExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(complexFunctionArgumentExpression, nameof(complexFunctionArgumentExpression));

            nullable = false;

            var argumentParts = new SqlExpression[complexFunctionArgumentExpression.ArgumentParts.Count];

            for (var i = 0; i < argumentParts.Length; i++)
            {
                argumentParts[i] = Visit(complexFunctionArgumentExpression.ArgumentParts[i], allowOptimizedExpansion, out var argumentPartNullable);
                nullable |= argumentPartNullable;
            }

            return complexFunctionArgumentExpression.Update(argumentParts, complexFunctionArgumentExpression.Delimiter);
        }

        /// <summary>
        /// Visits a <see cref="SapHanaMatchExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="matchExpression">A <see cref="SapHanaMatchExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitMatch(
            [NotNull] SapHanaMatchExpression matchExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(matchExpression, nameof(matchExpression));

            var match = Visit(matchExpression.Match, allowOptimizedExpansion, out var matchNullable);
            var pattern = Visit(matchExpression.Against, allowOptimizedExpansion, out var patternNullable);

            nullable = matchNullable || patternNullable;

            return matchExpression.Update(match, pattern);
        }

        /// <summary>
        /// Visits an <see cref="SapHanaJsonArrayIndexExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="jsonArrayIndexExpression">A <see cref="SapHanaJsonArrayIndexExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitJsonArrayIndex(
            [NotNull] SapHanaJsonArrayIndexExpression jsonArrayIndexExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(jsonArrayIndexExpression, nameof(jsonArrayIndexExpression));

            var index = Visit(jsonArrayIndexExpression.Expression, allowOptimizedExpansion, out nullable);

            return jsonArrayIndexExpression.Update(index);
        }

        /// <summary>
        /// Visits a <see cref="SapHanaJsonTraversalExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="jsonTraversalExpression">A <see cref="SapHanaJsonTraversalExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitJsonTraversal(
            [NotNull] SapHanaJsonTraversalExpression jsonTraversalExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(jsonTraversalExpression, nameof(jsonTraversalExpression));

            var expression = Visit(jsonTraversalExpression.Expression, out nullable);

            List<SqlExpression> newPath = null;
            for (var i = 0; i < jsonTraversalExpression.Path.Count; i++)
            {
                var pathComponent = jsonTraversalExpression.Path[i];
                var newPathComponent = Visit(pathComponent, allowOptimizedExpansion, out var nullablePathComponent);
                nullable |= nullablePathComponent;
                if (newPathComponent != pathComponent && newPath is null)
                {
                    newPath = new List<SqlExpression>();
                    for (var j = 0; j < i; j++)
                    {
                        newPath.Add(newPathComponent);
                    }
                }

                newPath?.Add(newPathComponent);
            }

            nullable = false;

            return jsonTraversalExpression.Update(
                expression,
                newPath is null
                    ? jsonTraversalExpression.Path
                    : newPath.ToArray());
        }

        /// <summary>
        /// Visits a <see cref="SapHanaRegexpExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="regexpExpression">A <see cref="SapHanaRegexpExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitRegexp(
            [NotNull] SapHanaRegexpExpression regexpExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(regexpExpression, nameof(regexpExpression));

            var match = Visit(regexpExpression.Match, out var matchNullable);
            var pattern = Visit(regexpExpression.Pattern, out var patternNullable);

            nullable = matchNullable || patternNullable;

            return regexpExpression.Update(match, pattern);
        }
    }
}
