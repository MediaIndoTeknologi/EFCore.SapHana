using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionTranslators.Internal
{
    public class SapHanaRegexIsMatchTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(Regex).GetRuntimeMethod(nameof(Regex.IsMatch), new[] { typeof(string), typeof(string) });

        private readonly SapHanaSqlExpressionFactory _sqlExpressionFactory;

        public SapHanaRegexIsMatchTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = (SapHanaSqlExpressionFactory)sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
            => _methodInfo.Equals(method)
                ? _sqlExpressionFactory.Regexp(
                    arguments[0],
                    arguments[1])
                : null;
    }
}
