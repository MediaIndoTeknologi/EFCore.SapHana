using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.Expressions.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SapHanaQuerySqlGenerator : QuerySqlGenerator
    {
        // The order in which the types are specified matters, because types get matched by using StartsWith.
        private static readonly Dictionary<string, string[]> _castMappings = new Dictionary<string, string[]>
        {
            { "signed", new []{ "tinyint", "smallint", "mediumint", "int", "bigint", "bit" }},
            { "decimal(65,30)", new []{ "decimal" } },
            { "double", new []{ "double" } },
            { "float", new []{ "float" } },
            { "binary", new []{ "binary", "varbinary", "tinyblob", "blob", "mediumblob", "longblob" } },
            { "datetime(6)", new []{ "datetime(6)" } },
            { "datetime", new []{ "datetime" } },
            { "date", new []{ "date" } },
            { "timestamp(6)", new []{ "timestamp(6)" } },
            { "timestamp", new []{ "timestamp" } },
            { "time(6)", new []{ "time(6)" } },
            { "time", new []{ "time" } },
            { "json", new []{ "json" } },
            { "char", new []{ "char", "varchar", "text", "tinytext", "mediumtext", "longtext" } },
            { "nchar", new []{ "nchar", "nvarchar" } },
        };

        private const ulong LimitUpperBound = 18446744073709551610;

        private readonly ISapHanaOptions _options;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SapHanaQuerySqlGenerator(
            [NotNull] QuerySqlGeneratorDependencies dependencies,
            [CanBeNull] ISapHanaOptions options)
            : base(dependencies)
        {
            _options = options;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
            => extensionExpression switch
            {
                SapHanaJsonTraversalExpression jsonTraversalExpression => VisitJsonPathTraversal(jsonTraversalExpression),
                SapHanaColumnAliasReferenceExpression columnAliasReferenceExpression => VisitColumnAliasReference(columnAliasReferenceExpression),
                _ => base.VisitExtension(extensionExpression)
            };

        private Expression VisitColumnAliasReference(SapHanaColumnAliasReferenceExpression columnAliasReferenceExpression)
        {
            Sql.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(columnAliasReferenceExpression.Alias));

            return columnAliasReferenceExpression;
        }

        protected virtual Expression VisitJsonPathTraversal(SapHanaJsonTraversalExpression expression)
        {
            // If the path contains parameters, then the -> and ->> aliases are not supported by SapHana, because
            // we need to concatenate the path and the parameters.
            // We will use JSON_EXTRACT (and JSON_UNQUOTE if needed) only in this case, because the aliases
            // are much more readable.
            var isSimplePath = expression.Path.All(
                l => l is SqlConstantExpression ||
                     l is SapHanaJsonArrayIndexExpression e && e.Expression is SqlConstantExpression);

            if (expression.ReturnsText)
            {
                Sql.Append("JSON_UNQUOTE(");
            }

            if (expression.Path.Count > 0)
            {
                Sql.Append("JSON_EXTRACT(");
            }

            Visit(expression.Expression);

            if (expression.Path.Count > 0)
            {
                Sql.Append(", ");

                if (!isSimplePath)
                {
                    Sql.Append("CONCAT(");
                }

                Sql.Append("'$");

                foreach (var location in expression.Path)
                {
                    if (location is SapHanaJsonArrayIndexExpression arrayIndexExpression)
                    {
                        var isConstantExpression = arrayIndexExpression.Expression is SqlConstantExpression;

                        Sql.Append("[");

                        if (!isConstantExpression)
                        {
                            Sql.Append("', ");
                        }

                        Visit(arrayIndexExpression.Expression);

                        if (!isConstantExpression)
                        {
                            Sql.Append(", '");
                        }

                        Sql.Append("]");
                    }
                    else
                    {
                        Sql.Append(".");
                        Visit(location);
                    }
                }

                Sql.Append("'");

                if (!isSimplePath)
                {
                    Sql.Append(")");
                }

                Sql.Append(")");
            }

            if (expression.ReturnsText)
            {
                Sql.Append(")");
            }

            return expression;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));

            if (selectExpression.Limit != null)
            {
                Sql.AppendLine().Append("LIMIT ");
                Visit(selectExpression.Limit);
            }

            if (selectExpression.Offset != null)
            {
                if (selectExpression.Limit == null)
                {
                    // if we want to use Skip() without Take() we have to define the upper limit of LIMIT
                    Sql.AppendLine().Append($"LIMIT {LimitUpperBound}");
                }

                Sql.Append(" OFFSET ");
                Visit(selectExpression.Offset);
            }
        }

        protected override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            if (sqlFunctionExpression.Name.StartsWith("@@", StringComparison.Ordinal))
            {
                Sql.Append(sqlFunctionExpression.Name);

                return sqlFunctionExpression;
            }

            return base.VisitSqlFunction(sqlFunctionExpression);
        }

        protected override Expression VisitCrossApply(CrossApplyExpression crossApplyExpression)
        {
            Sql.Append("JOIN ");

            if (crossApplyExpression.Table is not TableExpression)
            {
                Sql.Append("LATERAL ");
            }

            Visit(crossApplyExpression.Table);

            Sql.Append(" ON TRUE");

            return crossApplyExpression;
        }

        protected override Expression VisitOuterApply(OuterApplyExpression outerApplyExpression)
        {
            Sql.Append("LEFT JOIN ");

            if (outerApplyExpression.Table is not TableExpression)
            {
                Sql.Append("LATERAL ");
            }

            Visit(outerApplyExpression.Table);

            Sql.Append(" ON TRUE");

            return outerApplyExpression;
        }

        protected override Expression VisitSqlBinary(SqlBinaryExpression sqlBinaryExpression)
        {
            Check.NotNull(sqlBinaryExpression, nameof(sqlBinaryExpression));

            if (sqlBinaryExpression.OperatorType == ExpressionType.Add &&
                sqlBinaryExpression.Type == typeof(string) &&
                sqlBinaryExpression.Left.TypeMapping?.ClrType == typeof(string) &&
                sqlBinaryExpression.Right.TypeMapping?.ClrType == typeof(string))
            {
                Sql.Append("CONCAT(");
                Visit(sqlBinaryExpression.Left);
                Sql.Append(", ");
                Visit(sqlBinaryExpression.Right);
                Sql.Append(")");

                return sqlBinaryExpression;
            }

            var requiresBrackets = RequiresBrackets(sqlBinaryExpression.Left);

            if (requiresBrackets)
            {
                Sql.Append("(");
            }

            Visit(sqlBinaryExpression.Left);

            if (requiresBrackets)
            {
                Sql.Append(")");
            }

            Sql.Append(GetOperator(sqlBinaryExpression));

            // EF uses unary Equal and NotEqual to represent is-null checking.
            // These need to be surrounded with parenthesis in various cases (e.g. where TRUE = x IS NOT NULL).
            // See https://github.com/PomeloFoundation/WonderKidEngine.EntityFrameworkCore.SapHana/issues/1309
            requiresBrackets = RequiresBrackets(sqlBinaryExpression.Right) ||
                               !requiresBrackets &&
                               sqlBinaryExpression.Right is SqlUnaryExpression sqlUnaryExpression &&
                               (sqlUnaryExpression.OperatorType == ExpressionType.Equal || sqlUnaryExpression.OperatorType == ExpressionType.NotEqual);

            if (requiresBrackets)
            {
                Sql.Append("(");
            }

            Visit(sqlBinaryExpression.Right);

            if (requiresBrackets)
            {
                Sql.Append(")");
            }

            return sqlBinaryExpression;
        }

        private static bool RequiresBrackets(SqlExpression expression)
            => expression is SqlBinaryExpression
               || expression is LikeExpression
               || (expression is SqlUnaryExpression unary
                   && unary.Operand.Type == typeof(bool)
                   && (unary.OperatorType == ExpressionType.Equal
                       || unary.OperatorType == ExpressionType.NotEqual));

        public virtual Expression VisitSapHanaRegexp(SapHanaRegexpExpression SapHanaRegexpExpression)
        {
            Check.NotNull(SapHanaRegexpExpression, nameof(SapHanaRegexpExpression));

            Visit(SapHanaRegexpExpression.Match);
            Sql.Append(" REGEXP ");
            Visit(SapHanaRegexpExpression.Pattern);

            return SapHanaRegexpExpression;
        }

        public virtual Expression VisitSapHanaMatch(SapHanaMatchExpression SapHanaMatchExpression)
        {
            Check.NotNull(SapHanaMatchExpression, nameof(SapHanaMatchExpression));

            Sql.Append("MATCH ");
            Sql.Append("(");
            Visit(SapHanaMatchExpression.Match);
            Sql.Append(")");
            Sql.Append(" AGAINST ");
            Sql.Append("(");
            Visit(SapHanaMatchExpression.Against);

            switch (SapHanaMatchExpression.SearchMode)
            {
                case SapHanaMatchSearchMode.NaturalLanguage:
                    break;
                case SapHanaMatchSearchMode.NaturalLanguageWithQueryExpansion:
                    Sql.Append(" WITH QUERY EXPANSION");
                    break;
                case SapHanaMatchSearchMode.Boolean:
                    Sql.Append(" IN BOOLEAN MODE");
                    break;
            }

            Sql.Append(")");

            return SapHanaMatchExpression;
        }

        protected override Expression VisitSqlUnary(SqlUnaryExpression sqlUnaryExpression)
            => sqlUnaryExpression.OperatorType == ExpressionType.Convert
                ? VisitConvert(sqlUnaryExpression)
                : base.VisitSqlUnary(sqlUnaryExpression);

        private SqlUnaryExpression VisitConvert(SqlUnaryExpression sqlUnaryExpression)
        {
            var castMapping = GetCastStoreType(sqlUnaryExpression.TypeMapping);

            if (castMapping == "binary")
            {
                Sql.Append("UNHEX(HEX(");
                Visit(sqlUnaryExpression.Operand);
                Sql.Append("))");
                return sqlUnaryExpression;
            }

            // There needs to be no CAST() applied between the exact same store type. This could happen, e.g. if
            // `System.DateTime` and `System.DateTimeOffset` are used in conjunction, because both use different type
            // mappings, but map to the same store type (e.g. `datetime(6)`).
            //
            // There also is no need for a double CAST() to the same type. Due to only rudimentary CAST() support in
            // SapHana, the final store type of a CAST() operation might be different than the store type of the type
            // mapping of the expression (e.g. "float" will be cast to "double"). So we optimize these cases too.
            //
            // An exception is the JSON data type, when used in conjunction with a parameter (like `JsonDocument`).
            // JSON parameters like that will be serialized to string and supplied as a string parameter to SapHana
            // (at least this seems to be the case currently with SapHanaConnector). To make assignments and comparisons
            // between JSON columns and JSON parameters (supplied as string) work, the string needs to be explicitly
            // converted to JSON.

            var sameInnerCastStoreType = sqlUnaryExpression.Operand is SqlUnaryExpression operandUnary &&
                                         operandUnary.OperatorType == ExpressionType.Convert &&
                                         castMapping.Equals(GetCastStoreType(operandUnary.TypeMapping), StringComparison.OrdinalIgnoreCase);


            Visit(sqlUnaryExpression.Operand);


            return sqlUnaryExpression;
        }

        private string GetCastStoreType(RelationalTypeMapping typeMapping)
        {
            var storeTypeLower = typeMapping.StoreType.ToLower();
            string castMapping = null;
            foreach (var kvp in _castMappings)
            {
                foreach (var storeType in kvp.Value)
                {
                    if (storeTypeLower.StartsWith(storeType))
                    {
                        castMapping = kvp.Key;
                        break;
                    }
                }

                if (castMapping != null)
                {
                    break;
                }
            }

            if (castMapping == null)
            {
                throw new InvalidOperationException($"Cannot cast from type '{typeMapping.StoreType}'");
            }

            if (castMapping == "signed" && storeTypeLower.Contains("unsigned"))
            {
                castMapping = "unsigned";
            }



            return castMapping;
        }

        public virtual Expression VisitSapHanaComplexFunctionArgumentExpression(SapHanaComplexFunctionArgumentExpression SapHanaComplexFunctionArgumentExpression)
        {
            Check.NotNull(SapHanaComplexFunctionArgumentExpression, nameof(SapHanaComplexFunctionArgumentExpression));

            var first = true;
            foreach (var argument in SapHanaComplexFunctionArgumentExpression.ArgumentParts)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Sql.Append(SapHanaComplexFunctionArgumentExpression.Delimiter);
                }

                Visit(argument);
            }

            return SapHanaComplexFunctionArgumentExpression;
        }

        public virtual Expression VisitSapHanaCollateExpression(SapHanaCollateExpression SapHanaCollateExpression)
        {
            Check.NotNull(SapHanaCollateExpression, nameof(SapHanaCollateExpression));

            Sql.Append("CONVERT(");

            Visit(SapHanaCollateExpression.ValueExpression);

            Sql.Append($" USING {SapHanaCollateExpression.Charset}) COLLATE {SapHanaCollateExpression.Collation}");

            return SapHanaCollateExpression;
        }

        public virtual Expression VisitSapHanaBinaryExpression(SapHanaBinaryExpression SapHanaBinaryExpression)
        {
            if (SapHanaBinaryExpression.OperatorType == SapHanaBinaryExpressionOperatorType.NonOptimizedEqual)
            {
                var equalExpression = new SqlBinaryExpression(
                    ExpressionType.Equal,
                    SapHanaBinaryExpression.Left,
                    SapHanaBinaryExpression.Right,
                    SapHanaBinaryExpression.Type,
                    SapHanaBinaryExpression.TypeMapping);

                Visit(equalExpression);
            }
            else
            {
                Sql.Append("(");
                Visit(SapHanaBinaryExpression.Left);
                Sql.Append(")");

                switch (SapHanaBinaryExpression.OperatorType)
                {
                    case SapHanaBinaryExpressionOperatorType.IntegerDivision:
                        Sql.Append(" DIV ");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Sql.Append("(");
                Visit(SapHanaBinaryExpression.Right);
                Sql.Append(")");
            }

            return SapHanaBinaryExpression;
        }

        /// <inheritdoc />
        protected override void CheckComposableSql(string sql)
        {
            // SapHana supports CTE (WITH) expressions within subqueries, as well as others,
            // so we allow any raw SQL to be composed over.
        }
    }
}
