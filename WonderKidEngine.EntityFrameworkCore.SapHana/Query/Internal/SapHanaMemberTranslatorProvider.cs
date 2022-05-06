using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    public class SapHanaMemberTranslatorProvider : RelationalMemberTranslatorProvider
    {
        public SapHanaMemberTranslatorProvider([NotNull] RelationalMemberTranslatorProviderDependencies dependencies)
            : base(dependencies)
        {
            var sqlExpressionFactory = (SapHanaSqlExpressionFactory)dependencies.SqlExpressionFactory;

            AddTranslators(
                new IMemberTranslator[] {
                    new SapHanaDateTimeMemberTranslator(sqlExpressionFactory),
                    new SapHanaStringMemberTranslator(sqlExpressionFactory),
                    new SapHanaTimeSpanMemberTranslator(sqlExpressionFactory),
                });
        }
    }
}
