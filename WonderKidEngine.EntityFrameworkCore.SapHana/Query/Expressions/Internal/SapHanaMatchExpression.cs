using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Expressions.Internal
{
    public class SapHanaMatchExpression : SqlExpression
    {
        public SapHanaMatchExpression(
            SqlExpression match,
            SqlExpression against,
            SapHanaMatchSearchMode searchMode,
            RelationalTypeMapping typeMapping)
            : base(typeof(bool), typeMapping)
        {
            Check.NotNull(match, nameof(match));
            Check.NotNull(against, nameof(against));

            Match = match;
            Against = against;
            SearchMode = searchMode;
        }

        public virtual SapHanaMatchSearchMode SearchMode { get; }

        public virtual SqlExpression Match { get; }
        public virtual SqlExpression Against { get; }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            return visitor is SapHanaQuerySqlGenerator SapHanaQuerySqlGenerator // TODO: Move to VisitExtensions
                ? SapHanaQuerySqlGenerator.VisitSapHanaMatch(this)
                : base.Accept(visitor);
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var match = (SqlExpression)visitor.Visit(Match);
            var against = (SqlExpression)visitor.Visit(Against);

            return Update(match, against);
        }

        public virtual SapHanaMatchExpression Update(SqlExpression match, SqlExpression against)
            => match != Match || against != Against
                ? new SapHanaMatchExpression(
                    match,
                    against,
                    SearchMode,
                    TypeMapping)
                : this;

        public override bool Equals(object obj)
            => obj != null && ReferenceEquals(this, obj)
            || obj is SapHanaMatchExpression matchExpression && Equals(matchExpression);

        private bool Equals(SapHanaMatchExpression matchExpression)
            => base.Equals(matchExpression)
            && SearchMode == matchExpression.SearchMode
            && Match.Equals(matchExpression.Match)
            && Against.Equals(matchExpression.Against);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), SearchMode, Match, Against);

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append("MATCH ");
            expressionPrinter.Append($"({expressionPrinter.Visit(Match)})");
            expressionPrinter.Append(" AGAINST ");
            expressionPrinter.Append($"({expressionPrinter.Visit(Against)}");

            switch (SearchMode)
            {
                case SapHanaMatchSearchMode.NaturalLanguage:
                    break;
                case SapHanaMatchSearchMode.NaturalLanguageWithQueryExpansion:
                    expressionPrinter.Append(" WITH QUERY EXPANSION");
                    break;
                case SapHanaMatchSearchMode.Boolean:
                    expressionPrinter.Append(" IN BOOLEAN MODE");
                    break;
            }

            expressionPrinter.Append(")");
        }
    }
}
