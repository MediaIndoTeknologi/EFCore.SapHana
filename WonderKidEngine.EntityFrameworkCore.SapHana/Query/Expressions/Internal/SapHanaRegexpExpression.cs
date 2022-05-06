using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Expressions.Internal
{
    public class SapHanaRegexpExpression : SqlExpression
    {
        public SapHanaRegexpExpression(
            [NotNull] SqlExpression match,
            [NotNull] SqlExpression pattern,
            RelationalTypeMapping typeMapping)
            : base(typeof(bool), typeMapping)
        {
            Check.NotNull(match, nameof(match));
            Check.NotNull(pattern, nameof(pattern));

            Match = match;
            Pattern = pattern;
        }

        public virtual SqlExpression Match { get; }
        public virtual SqlExpression Pattern { get; }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            return visitor is SapHanaQuerySqlGenerator SapHanaQuerySqlGenerator // TODO: Move to VisitExtensions
                ? SapHanaQuerySqlGenerator.VisitSapHanaRegexp(this)
                : base.Accept(visitor);
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var match = (SqlExpression)visitor.Visit(Match);
            var pattern = (SqlExpression)visitor.Visit(Pattern);

            return Update(match, pattern);
        }

        public virtual SapHanaRegexpExpression Update(SqlExpression match, SqlExpression pattern)
            => match != Match ||
               pattern != Pattern
                ? new SapHanaRegexpExpression(match, pattern, TypeMapping)
                : this;

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((SapHanaRegexpExpression)obj);
        }

        private bool Equals(SapHanaRegexpExpression other)
            => Equals(Match, other.Match)
               && Equals(Pattern, other.Pattern);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Match.GetHashCode();
                hashCode = (hashCode * 397) ^ Pattern.GetHashCode();

                return hashCode;
            }
        }

        public override string ToString() => $"{Match} REGEXP {Pattern}";

        protected override void Print(ExpressionPrinter expressionPrinter)
            => expressionPrinter.Append(ToString());
    }
}
