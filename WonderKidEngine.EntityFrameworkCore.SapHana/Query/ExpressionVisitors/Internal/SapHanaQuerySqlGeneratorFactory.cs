using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal
{
    public class SapHanaQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;
        private readonly ISapHanaOptions _options;

        public SapHanaQuerySqlGeneratorFactory(
            [NotNull] QuerySqlGeneratorDependencies dependencies,
            ISapHanaOptions options)
        {
            _dependencies = dependencies;
            _options = options;
        }

        public virtual QuerySqlGenerator Create()
            => new SapHanaQuerySqlGenerator(_dependencies, _options);
    }
}
