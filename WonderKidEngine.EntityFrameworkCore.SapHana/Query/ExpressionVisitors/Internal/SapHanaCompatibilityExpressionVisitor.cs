using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal
{
    public class SapHanaCompatibilityExpressionVisitor : ExpressionVisitor
    {
        private readonly ISapHanaOptions _options;

        public SapHanaCompatibilityExpressionVisitor(ISapHanaOptions options)
        {
            _options = options;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
            => extensionExpression switch
            {
                RowNumberExpression rowNumberExpression => VisitRowNumber(rowNumberExpression),
                CrossApplyExpression crossApplyExpression => VisitCrossApply(crossApplyExpression),
                OuterApplyExpression outerApplyExpression => VisitOuterApply(outerApplyExpression),
                ExceptExpression exceptExpression => VisitExcept(exceptExpression),
                IntersectExpression intersectExpression => VisitIntercept(intersectExpression),
                ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.Update(Visit(shapedQueryExpression.QueryExpression), Visit(shapedQueryExpression.ShaperExpression)),
                _ => base.VisitExtension(extensionExpression)
            };

        protected virtual Expression VisitRowNumber(RowNumberExpression rowNumberExpression)
            => CheckSupport(rowNumberExpression, true);

        protected virtual Expression VisitCrossApply(CrossApplyExpression crossApplyExpression)
            => CheckSupport(crossApplyExpression, true);

        protected virtual Expression VisitOuterApply(OuterApplyExpression outerApplyExpression)
            => CheckSupport(outerApplyExpression, true);

        protected virtual Expression VisitExcept(ExceptExpression exceptExpression)
            => CheckSupport(exceptExpression, true);

        protected virtual Expression VisitIntercept(IntersectExpression intersectExpression)
            => CheckSupport(intersectExpression, true);

        protected virtual Expression CheckSupport(Expression expression, bool isSupported)
            => CheckTranslated(
                isSupported
                    ? base.VisitExtension(expression)
                    : null,
                expression);

        protected virtual Expression CheckTranslated(Expression translated, Expression original)
        {
            if (translated == null)
            {
                throw new InvalidOperationException(
                    CoreStrings.TranslationFailed(original.Print()));
            }

            return translated;
        }
    }
}
