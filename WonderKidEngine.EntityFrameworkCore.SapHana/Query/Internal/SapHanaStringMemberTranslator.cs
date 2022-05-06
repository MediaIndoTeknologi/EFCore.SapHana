using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    public class SapHanaStringMemberTranslator : IMemberTranslator
    {
        private readonly SapHanaSqlExpressionFactory _sqlExpressionFactory;

        public SapHanaStringMemberTranslator(SapHanaSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MemberInfo member,
            Type returnType,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (member.Name == nameof(string.Length)
                && instance?.Type == typeof(string))
            {
                return _sqlExpressionFactory.NullableFunction(
                    "CHAR_LENGTH",
                    new[] { instance },
                    returnType);
            }

            return null;
        }
    }
}
