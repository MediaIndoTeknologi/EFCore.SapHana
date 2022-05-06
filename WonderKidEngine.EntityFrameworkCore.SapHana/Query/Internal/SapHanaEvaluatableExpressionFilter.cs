using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    public class SapHanaEvaluatableExpressionFilter : RelationalEvaluatableExpressionFilter
    {
        private readonly IEnumerable<ISapHanaEvaluatableExpressionFilter> _SapHanaEvaluatableExpressionFilters;

        public SapHanaEvaluatableExpressionFilter(
            [NotNull] EvaluatableExpressionFilterDependencies dependencies,
            [NotNull] RelationalEvaluatableExpressionFilterDependencies relationalDependencies,
            [NotNull] IEnumerable<ISapHanaEvaluatableExpressionFilter> SapHanaEvaluatableExpressionFilters)
            : base(dependencies, relationalDependencies)
        {
            _SapHanaEvaluatableExpressionFilters = SapHanaEvaluatableExpressionFilters;
        }

        public override bool IsEvaluatableExpression(Expression expression, IModel model)
        {
            foreach (var evaluatableExpressionFilter in _SapHanaEvaluatableExpressionFilters)
            {
                var evaluatable = evaluatableExpressionFilter.IsEvaluatableExpression(expression, model);
                if (evaluatable.HasValue)
                {
                    return evaluatable.Value;
                }
            }

            if (expression is MethodCallExpression methodCallExpression)
            {
                var declaringType = methodCallExpression.Method.DeclaringType;

                if (declaringType == typeof(SapHanaDbFunctionsExtensions) ||
                    declaringType == typeof(SapHanaJsonDbFunctionsExtensions))
                {
                    return false;
                }
            }

            return base.IsEvaluatableExpression(expression, model);
        }
    }
}
