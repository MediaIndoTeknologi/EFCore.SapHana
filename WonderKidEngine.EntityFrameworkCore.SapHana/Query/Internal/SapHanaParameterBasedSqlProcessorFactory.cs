using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    public class SapHanaParameterBasedSqlProcessorFactory : IRelationalParameterBasedSqlProcessorFactory
    {
        private readonly RelationalParameterBasedSqlProcessorDependencies _dependencies;
        [NotNull] private readonly ISapHanaOptions _options;

        public SapHanaParameterBasedSqlProcessorFactory(
            [NotNull] RelationalParameterBasedSqlProcessorDependencies dependencies,
            [NotNull] ISapHanaOptions options)
        {
            _dependencies = dependencies;
            _options = options;
        }

        public virtual RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
            => new SapHanaParameterBasedSqlProcessor(_dependencies, useRelationalNulls, _options);
    }
}
