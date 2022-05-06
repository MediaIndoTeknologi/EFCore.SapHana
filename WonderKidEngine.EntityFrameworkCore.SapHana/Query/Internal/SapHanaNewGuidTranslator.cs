using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    public class SapHanaNewGuidTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo = typeof(Guid).GetRuntimeMethod(nameof(Guid.NewGuid), Array.Empty<Type>());
        private readonly SapHanaSqlExpressionFactory _sqlExpressionFactory;

        public SapHanaNewGuidTranslator(SapHanaSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            return _methodInfo.Equals(method)
                ? _sqlExpressionFactory.NonNullableFunction(
                    "UUID",
                    Array.Empty<SqlExpression>(),
                    method.ReturnType)
                : null;
        }
    }
}
