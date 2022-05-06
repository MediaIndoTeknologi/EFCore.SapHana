using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionTranslators.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    public class SapHanaMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
    {
        public SapHanaMethodCallTranslatorProvider(
            [NotNull] RelationalMethodCallTranslatorProviderDependencies dependencies,
            [NotNull] ISapHanaOptions options)
            : base(dependencies)
        {
            var sqlExpressionFactory = (SapHanaSqlExpressionFactory)dependencies.SqlExpressionFactory;
            var relationalTypeMappingSource = (SapHanaTypeMappingSource)dependencies.RelationalTypeMappingSource;

            AddTranslators(new IMethodCallTranslator[]
            {
                new SapHanaByteArrayMethodTranslator(sqlExpressionFactory),
                new SapHanaConvertTranslator(sqlExpressionFactory),
                new SapHanaDateTimeMethodTranslator(sqlExpressionFactory),
                new SapHanaDateDiffFunctionsTranslator(sqlExpressionFactory),
                new SapHanaDbFunctionsExtensionsMethodTranslator(sqlExpressionFactory),
                new SapHanaMathMethodTranslator(sqlExpressionFactory),
                new SapHanaNewGuidTranslator(sqlExpressionFactory),
                new SapHanaObjectToStringTranslator(sqlExpressionFactory),
                new SapHanaRegexIsMatchTranslator(sqlExpressionFactory),
                new SapHanaStringComparisonMethodTranslator(sqlExpressionFactory, options),
                new SapHanaStringMethodTranslator(sqlExpressionFactory, relationalTypeMappingSource, options),
            });
        }
    }
}
