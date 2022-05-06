using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.Expressions.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Utilities;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    public class SapHanaSqlExpressionFactory : SqlExpressionFactory
    {
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly RelationalTypeMapping _boolTypeMapping;

        public SapHanaSqlExpressionFactory(SqlExpressionFactoryDependencies dependencies)
            : base(dependencies)
        {
            _typeMappingSource = dependencies.TypeMappingSource;
            _boolTypeMapping = _typeMappingSource.FindMapping(typeof(bool));
        }

        public virtual RelationalTypeMapping FindMapping(
            [NotNull] Type type,
            [CanBeNull] string storeTypeName,
            bool keyOrIndex = false,
            bool? unicode = null,
            int? size = null,
            bool? rowVersion = null,
            bool? fixedLength = null,
            int? precision = null,
            int? scale = null)
            => _typeMappingSource.FindMapping(
                type,
                storeTypeName,
                keyOrIndex,
                unicode,
                size,
                rowVersion,
                fixedLength,
                precision,
                scale);

        #region Expression factory methods

        /// <summary>
        /// Use for any function that could return `NULL` for *any* reason.
        /// </summary>
        /// <param name="name">The SQL name of the function.</param>
        /// <param name="arguments">The arguments of the function.</param>
        /// <param name="returnType">The CLR return type of the function.</param>
        /// <param name="onlyNullWhenAnyNullPropagatingArgumentIsNull">
        /// Set to `false` if the function can return `NULL` even if all of the arguments are not `NULL`. This will disable null-related
        /// optimizations by EF Core.
        /// </param>
        /// <remarks>See https://github.com/dotnet/efcore/issues/23042</remarks>
        /// <returns>The function expression.</returns>
        public virtual SqlFunctionExpression NullableFunction(
            string name,
            IEnumerable<SqlExpression> arguments,
            Type returnType,
            bool onlyNullWhenAnyNullPropagatingArgumentIsNull)
            => NullableFunction(name, arguments, returnType, null, onlyNullWhenAnyNullPropagatingArgumentIsNull);

        /// <summary>
        /// Use for any function that could return `NULL` for *any* reason.
        /// </summary>
        /// <param name="name">The SQL name of the function.</param>
        /// <param name="arguments">The arguments of the function.</param>
        /// <param name="returnType">The CLR return type of the function.</param>
        /// <param name="typeMapping">The optional type mapping of the function.</param>
        /// <param name="onlyNullWhenAnyNullPropagatingArgumentIsNull">
        ///     Set to `false` if the function can return `NULL` even if all of the arguments are not `NULL`. This will disable null-related
        ///     optimizations by EF Core.
        /// </param>
        /// <param name="argumentsPropagateNullability">
        ///     The optional nullability array of the function.
        ///     If omited and <paramref name="onlyNullWhenAnyNullPropagatingArgumentIsNull"/> is
        ///     `true` (the default), all parameters will propagate nullability (meaning if any parameter is `NULL`, the function will
        ///     automatically return `NULL` as well).
        ///     If <paramref name="onlyNullWhenAnyNullPropagatingArgumentIsNull"/> is explicitly set to `false`, the
        ///     null propagating capabilities of the arguments don't matter at all anymore, because the function will never be optimized by
        ///     EF Core in the first place.
        /// </param>
        /// <remarks>See https://github.com/dotnet/efcore/issues/23042</remarks>
        /// <returns>The function expression.</returns>
        public virtual SqlFunctionExpression NullableFunction(
            string name,
            IEnumerable<SqlExpression> arguments,
            Type returnType,
            RelationalTypeMapping typeMapping = null,
            bool onlyNullWhenAnyNullPropagatingArgumentIsNull = true,
            IEnumerable<bool> argumentsPropagateNullability = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(arguments, nameof(arguments));
            Check.NotNull(returnType, nameof(returnType));

            var typeMappedArguments = new List<SqlExpression>();

            foreach (var argument in arguments)
            {
                typeMappedArguments.Add(ApplyDefaultTypeMapping(argument));
            }

            return new SqlFunctionExpression(
                name,
                typeMappedArguments,
                true,
                onlyNullWhenAnyNullPropagatingArgumentIsNull
                    ? (argumentsPropagateNullability ?? Statics.GetTrueValues(typeMappedArguments.Count))
                    : Statics.GetFalseValues(typeMappedArguments.Count),
                returnType,
                typeMapping);
        }

        /// <summary>
        /// Use for any function that will never return `NULL`.
        /// </summary>
        /// <param name="name">The SQL name of the function.</param>
        /// <param name="arguments">The arguments of the function.</param>
        /// <param name="returnType">The CLR return type of the function.</param>
        /// <param name="typeMapping">The optional type mapping of the function.</param>
        /// <remarks>See https://github.com/dotnet/efcore/issues/23042</remarks>
        /// <returns>The function expression.</returns>
        public virtual SqlFunctionExpression NonNullableFunction(
            string name,
            IEnumerable<SqlExpression> arguments,
            Type returnType,
            RelationalTypeMapping typeMapping = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(arguments, nameof(arguments));
            Check.NotNull(returnType, nameof(returnType));

            var typeMappedArguments = new List<SqlExpression>();

            foreach (var argument in arguments)
            {
                typeMappedArguments.Add(ApplyDefaultTypeMapping(argument));
            }

            return new SqlFunctionExpression(
                name,
                typeMappedArguments,
                false,
                Statics.GetFalseValues(typeMappedArguments.Count),
                returnType,
                typeMapping);
        }

        public virtual SapHanaComplexFunctionArgumentExpression ComplexFunctionArgument(
            IEnumerable<SqlExpression> argumentParts,
            string delimiter,
            Type argumentType,
            RelationalTypeMapping typeMapping = null)
        {
            var typeMappedArgumentParts = new List<SqlExpression>();

            foreach (var argument in argumentParts)
            {
                typeMappedArgumentParts.Add(ApplyDefaultTypeMapping(argument));
            }

            return (SapHanaComplexFunctionArgumentExpression)ApplyTypeMapping(
                new SapHanaComplexFunctionArgumentExpression(
                    typeMappedArgumentParts,
                    delimiter,
                    argumentType,
                    typeMapping),
                typeMapping);
        }

        public virtual SapHanaCollateExpression Collate(
            SqlExpression valueExpression,
            string charset,
            string collation)
            => (SapHanaCollateExpression)ApplyDefaultTypeMapping(
                new SapHanaCollateExpression(
                    valueExpression,
                    charset,
                    collation,
                    null));

        public virtual SapHanaRegexpExpression Regexp(
            SqlExpression match,
            SqlExpression pattern)
            => (SapHanaRegexpExpression)ApplyDefaultTypeMapping(
                new SapHanaRegexpExpression(
                    match,
                    pattern,
                    null));

        public virtual SapHanaBinaryExpression SapHanaIntegerDivide(
            SqlExpression left,
            SqlExpression right,
            RelationalTypeMapping typeMapping = null)
            => MakeBinary(
                SapHanaBinaryExpressionOperatorType.IntegerDivision,
                left,
                right,
                typeMapping);

        public virtual SapHanaBinaryExpression NonOptimizedEqual(
            SqlExpression left,
            SqlExpression right,
            RelationalTypeMapping typeMapping = null)
            => MakeBinary(
                SapHanaBinaryExpressionOperatorType.NonOptimizedEqual,
                left,
                right,
                typeMapping);

        public virtual SapHanaColumnAliasReferenceExpression ColumnAliasReference(
            string alias,
            SqlExpression expression,
            Type type,
            RelationalTypeMapping typeMapping = null)
            => new SapHanaColumnAliasReferenceExpression(alias, expression, type, typeMapping);

        #endregion Expression factory methods

        public virtual SapHanaBinaryExpression MakeBinary(
            SapHanaBinaryExpressionOperatorType operatorType,
            SqlExpression left,
            SqlExpression right,
            RelationalTypeMapping typeMapping)
        {
            var returnType = left.Type;

            return (SapHanaBinaryExpression)ApplyTypeMapping(
                new SapHanaBinaryExpression(
                    operatorType,
                    left,
                    right,
                    returnType,
                    null),
                typeMapping);
        }

        public virtual SapHanaMatchExpression MakeMatch(
            SqlExpression match,
            SqlExpression against,
            SapHanaMatchSearchMode searchMode)
        {
            return (SapHanaMatchExpression)ApplyDefaultTypeMapping(
                new SapHanaMatchExpression(
                    match,
                    against,
                    searchMode,
                    null));
        }

        public virtual SapHanaJsonTraversalExpression JsonTraversal(
            [NotNull] SqlExpression expression,
            bool returnsText,
            [NotNull] Type type,
            [CanBeNull] RelationalTypeMapping typeMapping = null)
            => new SapHanaJsonTraversalExpression(
                ApplyDefaultTypeMapping(expression),
                returnsText,
                type,
                typeMapping);

        public virtual SapHanaJsonArrayIndexExpression JsonArrayIndex(
            [NotNull] SqlExpression expression)
            => JsonArrayIndex(expression, typeof(int));

        public virtual SapHanaJsonArrayIndexExpression JsonArrayIndex(
            [NotNull] SqlExpression expression,
            [NotNull] Type type,
            [CanBeNull] RelationalTypeMapping typeMapping = null)
            => (SapHanaJsonArrayIndexExpression)ApplyDefaultTypeMapping(
                new SapHanaJsonArrayIndexExpression(
                    ApplyDefaultTypeMapping(expression),
                    type,
                    typeMapping));

        public override SqlExpression ApplyTypeMapping(SqlExpression sqlExpression, RelationalTypeMapping typeMapping)
            => sqlExpression is not { TypeMapping: null }
                ? sqlExpression
                : ApplyNewTypeMapping(sqlExpression, typeMapping);

        private SqlExpression ApplyNewTypeMapping(SqlExpression sqlExpression, RelationalTypeMapping typeMapping)
            => sqlExpression switch
            {
                // Customize handling for binary expressions.
                SqlBinaryExpression e => ApplyTypeMappingOnSqlBinary(e, typeMapping),

                // SapHana specific expression types:
                SapHanaComplexFunctionArgumentExpression e => ApplyTypeMappingOnComplexFunctionArgument(e),
                SapHanaCollateExpression e => ApplyTypeMappingOnCollate(e),
                SapHanaRegexpExpression e => ApplyTypeMappingOnRegexp(e),
                SapHanaBinaryExpression e => ApplyTypeMappingOnSapHanaBinary(e, typeMapping),
                SapHanaMatchExpression e => ApplyTypeMappingOnMatch(e),
                SapHanaJsonArrayIndexExpression e => e.ApplyTypeMapping(typeMapping),

                _ => base.ApplyTypeMapping(sqlExpression, typeMapping)
            };

        private SqlBinaryExpression ApplyTypeMappingOnSqlBinary(SqlBinaryExpression sqlBinaryExpression, RelationalTypeMapping typeMapping)
        {
            // The default SqlExpressionFactory behavior is to assume that the two operands have the same type, and so to infer one side's
            // mapping from the other if needed. Here we take care of some heterogeneous operand cases where this doesn't work.

            var left = sqlBinaryExpression.Left;
            var right = sqlBinaryExpression.Right;

            var newSqlBinaryExpression = (SqlBinaryExpression)base.ApplyTypeMapping(sqlBinaryExpression, typeMapping);
            return newSqlBinaryExpression;
        }

        private SapHanaComplexFunctionArgumentExpression ApplyTypeMappingOnComplexFunctionArgument(SapHanaComplexFunctionArgumentExpression complexFunctionArgumentExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(complexFunctionArgumentExpression.ArgumentParts.ToArray())
                                      ?? (complexFunctionArgumentExpression.Type.IsArray
                                          ? _typeMappingSource.FindMapping(
                                              complexFunctionArgumentExpression.Type.GetElementType() ??
                                              complexFunctionArgumentExpression.Type)
                                          : _typeMappingSource.FindMapping(complexFunctionArgumentExpression.Type));

            return new SapHanaComplexFunctionArgumentExpression(
                complexFunctionArgumentExpression.ArgumentParts,
                complexFunctionArgumentExpression.Delimiter,
                complexFunctionArgumentExpression.Type,
                inferredTypeMapping ?? complexFunctionArgumentExpression.TypeMapping);
        }

        private SapHanaCollateExpression ApplyTypeMappingOnCollate(SapHanaCollateExpression collateExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(collateExpression.ValueExpression)
                                      ?? _typeMappingSource.FindMapping(collateExpression.ValueExpression.Type);

            return new SapHanaCollateExpression(
                ApplyTypeMapping(collateExpression.ValueExpression, inferredTypeMapping),
                collateExpression.Charset,
                collateExpression.Collation,
                inferredTypeMapping ?? collateExpression.TypeMapping);
        }

        private SqlExpression ApplyTypeMappingOnMatch(SapHanaMatchExpression matchExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(matchExpression.Match) ??
                                      _typeMappingSource.FindMapping(matchExpression.Match.Type);

            return new SapHanaMatchExpression(
                ApplyTypeMapping(matchExpression.Match, inferredTypeMapping),
                ApplyTypeMapping(matchExpression.Against, inferredTypeMapping),
                matchExpression.SearchMode,
                _boolTypeMapping);
        }

        private SqlExpression ApplyTypeMappingOnRegexp(SapHanaRegexpExpression regexpExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(regexpExpression.Match)
                                      ?? _typeMappingSource.FindMapping(regexpExpression.Match.Type);

            return new SapHanaRegexpExpression(
                ApplyTypeMapping(regexpExpression.Match, inferredTypeMapping),
                ApplyTypeMapping(regexpExpression.Pattern, inferredTypeMapping),
                _boolTypeMapping);
        }

        private SqlExpression ApplyTypeMappingOnSapHanaBinary(
            SapHanaBinaryExpression sqlBinaryExpression,
            RelationalTypeMapping typeMapping)
        {
            var left = sqlBinaryExpression.Left;
            var right = sqlBinaryExpression.Right;

            Type resultType;
            RelationalTypeMapping resultTypeMapping;
            RelationalTypeMapping inferredTypeMapping;

            switch (sqlBinaryExpression.OperatorType)
            {
                case SapHanaBinaryExpressionOperatorType.NonOptimizedEqual:
                    inferredTypeMapping = ExpressionExtensions.InferTypeMapping(left, right)
                                          // We avoid object here since the result does not get typeMapping from outside.
                                          ?? (left.Type != typeof(object)
                                              ? _typeMappingSource.FindMapping(left.Type)
                                              : _typeMappingSource.FindMapping(right.Type));
                    resultType = typeof(bool);
                    resultTypeMapping = _boolTypeMapping;
                    break;

                case SapHanaBinaryExpressionOperatorType.IntegerDivision:
                    inferredTypeMapping = typeMapping ?? ExpressionExtensions.InferTypeMapping(left, right);
                    resultType = inferredTypeMapping?.ClrType ?? left.Type;
                    resultTypeMapping = inferredTypeMapping;
                    break;

                default:
                    throw new InvalidOperationException($"Incorrect {nameof(SapHanaBinaryExpression.OperatorType)} for {nameof(SapHanaBinaryExpression)}");
            }

            return new SapHanaBinaryExpression(
                sqlBinaryExpression.OperatorType,
                ApplyTypeMapping(left, inferredTypeMapping),
                ApplyTypeMapping(right, inferredTypeMapping),
                resultType,
                resultTypeMapping);
        }
    }
}
