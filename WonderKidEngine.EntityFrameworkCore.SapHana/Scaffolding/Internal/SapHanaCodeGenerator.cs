using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Scaffolding.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SapHanaCodeGenerator : ProviderCodeGenerator
    {
        private static readonly MethodInfo _useSapHanaMethodInfo = typeof(SapHanaDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(SapHanaDbContextOptionsBuilderExtensions.UseSapHana),
            typeof(DbContextOptionsBuilder),
            typeof(string),
            typeof(Action<SapHanaDbContextOptionsBuilder>));

        private readonly ISapHanaOptions _options;

        public SapHanaCodeGenerator(
            [NotNull] ProviderCodeGeneratorDependencies dependencies,
            ISapHanaOptions options)
            : base(dependencies)
        {
            _options = options;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override MethodCallCodeFragment GenerateUseProvider(
            string connectionString,
            MethodCallCodeFragment providerOptions)
        {
            // Strip scaffolding specific connection string options first.
            connectionString = new SapHanaScaffoldingConnectionSettings(connectionString).GetProviderCompatibleConnectionString();

            return new MethodCallCodeFragment(
                _useSapHanaMethodInfo,
                providerOptions == null
                    ? new object[] { connectionString }
                    : new object[] { connectionString, new NestedClosureCodeFragment("x", providerOptions) });
        }
    }
}
