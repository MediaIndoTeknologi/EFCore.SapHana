using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Utilities;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    public class SapHanaQueryCompilationContextFactory : IQueryCompilationContextFactory
    {
        private readonly QueryCompilationContextDependencies _dependencies;
        private readonly RelationalQueryCompilationContextDependencies _relationalDependencies;

        public SapHanaQueryCompilationContextFactory(
            [NotNull] QueryCompilationContextDependencies dependencies,
            [NotNull] RelationalQueryCompilationContextDependencies relationalDependencies)
        {
            Check.NotNull(dependencies, nameof(dependencies));
            Check.NotNull(relationalDependencies, nameof(relationalDependencies));

            _dependencies = dependencies;
            _relationalDependencies = relationalDependencies;
        }

        public virtual QueryCompilationContext Create(bool async)
            => new SapHanaQueryCompilationContext(_dependencies, _relationalDependencies, async);
    }
}
