using System.Linq;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Update.Internal
{
    public class SapHanaModificationCommandBatchFactory : IModificationCommandBatchFactory
    {
        private readonly ModificationCommandBatchFactoryDependencies _dependencies;
        private readonly IDbContextOptions _options;

        public SapHanaModificationCommandBatchFactory(
            [NotNull] ModificationCommandBatchFactoryDependencies dependencies,
            [NotNull] IDbContextOptions options)
        {
            Check.NotNull(dependencies, nameof(dependencies));
            Check.NotNull(options, nameof(options));

            _dependencies = dependencies;
            _options = options;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual ModificationCommandBatch Create()
        {
            var optionsExtension = _options.Extensions.OfType<SapHanaOptionsExtension>().FirstOrDefault();

            return new SapHanaModificationCommandBatch(_dependencies, optionsExtension?.MaxBatchSize);
        }
    }
}
