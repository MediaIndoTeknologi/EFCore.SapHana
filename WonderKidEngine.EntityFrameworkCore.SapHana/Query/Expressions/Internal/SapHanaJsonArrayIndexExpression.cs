using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Expressions.Internal
{
    /// <summary>
    /// Represents a SapHana JSON array index (i.e. x[y]).
    /// </summary>
    public class SapHanaJsonArrayIndexExpression : SqlExpression, IEquatable<SapHanaJsonArrayIndexExpression>
    {
        [NotNull]
        public virtual SqlExpression Expression { get; }

        public SapHanaJsonArrayIndexExpression(
            [NotNull] SqlExpression expression,
            [NotNull] Type type,
            [CanBeNull] RelationalTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            Expression = expression;
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update((SqlExpression)visitor.Visit(Expression));

        public virtual SapHanaJsonArrayIndexExpression Update(
            [NotNull] SqlExpression expression)
            => expression == Expression
                ? this
                : new SapHanaJsonArrayIndexExpression(expression, Type, TypeMapping);

        public override bool Equals(object obj)
            => Equals(obj as SapHanaJsonArrayIndexExpression);

        public virtual bool Equals(SapHanaJsonArrayIndexExpression other)
            => ReferenceEquals(this, other) ||
               other != null &&
               base.Equals(other) &&
               Equals(Expression, other.Expression);

        public override int GetHashCode()
            => HashCode.Combine(base.GetHashCode(), Expression);

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append("[");
            expressionPrinter.Visit(Expression);
            expressionPrinter.Append("]");
        }

        public override string ToString()
            => $"[{Expression}]";

        public virtual SqlExpression ApplyTypeMapping(RelationalTypeMapping typeMapping)
            => new SapHanaJsonArrayIndexExpression(Expression, Type, typeMapping);
    }
}
