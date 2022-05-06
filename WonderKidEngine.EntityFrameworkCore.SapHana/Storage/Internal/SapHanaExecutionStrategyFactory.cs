
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SapHanaExecutionStrategyFactory : RelationalExecutionStrategyFactory
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SapHanaExecutionStrategyFactory(
            [NotNull] ExecutionStrategyDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override IExecutionStrategy CreateDefaultStrategy(ExecutionStrategyDependencies dependencies)
            => new SapHanaExecutionStrategy(dependencies);
    }
}
