using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Expressions.Internal
{
    public enum SapHanaBinaryExpressionOperatorType
    {
        /// <summary>
        /// TODO
        /// </summary>
        IntegerDivision,

        /// <summary>
        /// Use to force an equals expression, that will not be optimized by EF Core.
        /// Can be used, to force a `value = TRUE` expression.
        /// </summary>
        NonOptimizedEqual,
    }

    public class SapHanaBinaryExpression : SqlExpression
    {
        public SapHanaBinaryExpression(
            SapHanaBinaryExpressionOperatorType operatorType,
            SqlExpression left,
            SqlExpression right,
            Type type,
            RelationalTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));

            OperatorType = operatorType;

            Left = left;
            Right = right;
        }

        public virtual SapHanaBinaryExpressionOperatorType OperatorType { get; }
        public virtual SqlExpression Left { get; }
        public virtual SqlExpression Right { get; }

        protected override Expression Accept(ExpressionVisitor visitor)
            => visitor is SapHanaQuerySqlGenerator SapHanaQuerySqlGenerator // TODO: Move to VisitExtensions
                ? SapHanaQuerySqlGenerator.VisitSapHanaBinaryExpression(this)
                : base.Accept(visitor);

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var left = (SqlExpression)visitor.Visit(Left);
            var right = (SqlExpression)visitor.Visit(Right);

            return Update(left, right);
        }

        public virtual SapHanaBinaryExpression Update(SqlExpression left, SqlExpression right)
            => left != Left || right != Right
                ? new SapHanaBinaryExpression(OperatorType, left, right, Type, TypeMapping)
                : this;

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            var requiresBrackets = RequiresBrackets(Left);

            if (requiresBrackets)
            {
                expressionPrinter.Append("(");
            }

            expressionPrinter.Visit(Left);

            if (requiresBrackets)
            {
                expressionPrinter.Append(")");
            }

            switch (OperatorType)
            {
                case SapHanaBinaryExpressionOperatorType.IntegerDivision:
                    expressionPrinter.Append(" DIV ");
                    break;
                case SapHanaBinaryExpressionOperatorType.NonOptimizedEqual:
                    expressionPrinter.Append(" = ");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            requiresBrackets = RequiresBrackets(Right);

            if (requiresBrackets)
            {
                expressionPrinter.Append("(");
            }

            expressionPrinter.Visit(Right);

            if (requiresBrackets)
            {
                expressionPrinter.Append(")");
            }
        }

        private bool RequiresBrackets(SqlExpression expression)
        {
            return expression is SqlBinaryExpression sqlBinary
                && sqlBinary.OperatorType != ExpressionType.Coalesce
                || expression is LikeExpression;
        }

        public override bool Equals(object obj)
            => obj != null
            && (ReferenceEquals(this, obj)
                || obj is SapHanaBinaryExpression sqlBinaryExpression
                    && Equals(sqlBinaryExpression));

        private bool Equals(SapHanaBinaryExpression sqlBinaryExpression)
            => base.Equals(sqlBinaryExpression)
            && OperatorType == sqlBinaryExpression.OperatorType
            && Left.Equals(sqlBinaryExpression.Left)
            && Right.Equals(sqlBinaryExpression.Right);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), OperatorType, Left, Right);
    }
}
