using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    public class SapHanaQueryCompilationContext : RelationalQueryCompilationContext
    {
        public SapHanaQueryCompilationContext(
            [NotNull] QueryCompilationContextDependencies dependencies,
            [NotNull] RelationalQueryCompilationContextDependencies relationalDependencies, bool async)
            : base(dependencies, relationalDependencies, async)
        {
        }

        public override bool IsBuffering
            => base.IsBuffering ||
               QuerySplittingBehavior == Microsoft.EntityFrameworkCore.QuerySplittingBehavior.SplitQuery;
    }
}
