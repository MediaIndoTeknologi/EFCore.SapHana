using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal
{
    public class SapHanaJsonParameterExpressionVisitor : ExpressionVisitor
    {
        private readonly SapHanaSqlExpressionFactory _sqlExpressionFactory;
        private readonly ISapHanaOptions _options;

        public SapHanaJsonParameterExpressionVisitor(SapHanaSqlExpressionFactory sqlExpressionFactory, ISapHanaOptions options)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
            _options = options;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
            => extensionExpression switch
            {
                SqlParameterExpression sqlParameterExpression => VisitParameter(sqlParameterExpression),
                ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.Update(Visit(shapedQueryExpression.QueryExpression), Visit(shapedQueryExpression.ShaperExpression)),
                _ => base.VisitExtension(extensionExpression)
            };

        protected virtual SqlExpression VisitParameter(SqlParameterExpression sqlParameterExpression)
        {

            return sqlParameterExpression;
        }
    }
}
