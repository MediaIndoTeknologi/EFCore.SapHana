using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Expressions.Internal
{
    /// <summary>
    /// Allows to reference an alias from within the same SELECT statement, e.g. in a HAVING clause.
    /// </summary>
    public class SapHanaColumnAliasReferenceExpression : SqlExpression, IEquatable<SapHanaColumnAliasReferenceExpression>
    {
        [NotNull]
        public virtual string Alias { get; }

        [NotNull]
        public virtual SqlExpression Expression { get; }

        public SapHanaColumnAliasReferenceExpression(
            [NotNull] string alias,
            [NotNull] SqlExpression expression,
            [NotNull] Type type,
            [CanBeNull] RelationalTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            Alias = alias;
            Expression = expression;
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => this;

        public virtual SapHanaColumnAliasReferenceExpression Update(
            [NotNull] string alias,
            [NotNull] SqlExpression expression)
            => alias == Alias &&
               expression.Equals(Expression)
                ? this
                : new SapHanaColumnAliasReferenceExpression(alias, expression, Type, TypeMapping);

        public override bool Equals(object obj)
            => Equals(obj as SapHanaColumnAliasReferenceExpression);

        public virtual bool Equals(SapHanaColumnAliasReferenceExpression other)
            => ReferenceEquals(this, other) ||
               other != null &&
               base.Equals(other) &&
               Equals(Expression, other.Expression);

        public override int GetHashCode()
            => HashCode.Combine(base.GetHashCode(), Expression);

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append("`");
            expressionPrinter.Append(Alias);
            expressionPrinter.Append("`");
        }

        public override string ToString()
            => $"`{Alias}`";

        public virtual SqlExpression ApplyTypeMapping(RelationalTypeMapping typeMapping)
            => new SapHanaColumnAliasReferenceExpression(Alias, Expression, Type, typeMapping);
    }
}
