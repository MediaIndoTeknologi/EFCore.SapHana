using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SapHanaDateDiffFunctionsTranslator : IMethodCallTranslator
    {
        private readonly Dictionary<MethodInfo, string> _methodInfoDateDiffMapping
            = new Dictionary<MethodInfo, string>
            {
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "YEAR" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "YEAR" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "YEAR" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "YEAR" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "YEAR" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "YEAR" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MONTH" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MONTH" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MONTH" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MONTH" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MONTH" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MONTH" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "DAY" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "DAY" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "DAY" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "DAY" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "DAY" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "DAY" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "HOUR" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "HOUR" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "HOUR" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "HOUR" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "HOUR" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "HOUR" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MINUTE" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MINUTE" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MINUTE" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MINUTE" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MINUTE" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MINUTE" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "SECOND" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "SECOND" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "SECOND" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "SECOND" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "SECOND" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "SECOND" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MICROSECOND" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MICROSECOND" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MICROSECOND" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MICROSECOND" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MICROSECOND" },
                { typeof(SapHanaDbFunctionsExtensions).GetRuntimeMethod(nameof(SapHanaDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MICROSECOND" },
            };

        private readonly SapHanaSqlExpressionFactory _sqlExpressionFactory;

        public SapHanaDateDiffFunctionsTranslator(SapHanaSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (_methodInfoDateDiffMapping.TryGetValue(method, out var datePart))
            {
                var startDate = arguments[1];
                var endDate = arguments[2];
                var typeMapping = ExpressionExtensions.InferTypeMapping(startDate, endDate);

                startDate = _sqlExpressionFactory.ApplyTypeMapping(startDate, typeMapping);
                endDate = _sqlExpressionFactory.ApplyTypeMapping(endDate, typeMapping);

                return _sqlExpressionFactory.NullableFunction(
                    "TIMESTAMPDIFF",
                    new[]
                    {
                        _sqlExpressionFactory.Fragment(datePart),
                        startDate,
                        endDate
                    },
                    typeof(int),
                    typeMapping: null,
                    onlyNullWhenAnyNullPropagatingArgumentIsNull: true,
                    argumentsPropagateNullability: new []{false, true, true});
            }

            return null;
        }
    }
}
