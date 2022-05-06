using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal
{
    public class SapHanaCompiledQueryCacheKeyGenerator : RelationalCompiledQueryCacheKeyGenerator
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public SapHanaCompiledQueryCacheKeyGenerator(
            [NotNull] CompiledQueryCacheKeyGeneratorDependencies dependencies,
            [NotNull] RelationalCompiledQueryCacheKeyGeneratorDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override object GenerateCacheKey(Expression query, bool async)
        {
            var extensions = RelationalDependencies.ContextOptions.FindExtension<SapHanaOptionsExtension>();
            return new SapHanaCompiledQueryCacheKey(
                GenerateCacheKeyCore(query, async),
                extensions?.NoBackslashEscapes ?? false);
        }

        private readonly struct SapHanaCompiledQueryCacheKey
        {
            private readonly RelationalCompiledQueryCacheKey _relationalCompiledQueryCacheKey;
            private readonly bool _noBackslashEscapes;

            public SapHanaCompiledQueryCacheKey(
                RelationalCompiledQueryCacheKey relationalCompiledQueryCacheKey,
                bool noBackslashEscapes)
            {
                _relationalCompiledQueryCacheKey = relationalCompiledQueryCacheKey;
                _noBackslashEscapes = noBackslashEscapes;
            }

            public override bool Equals(object obj)
                => !(obj is null)
                   && obj is SapHanaCompiledQueryCacheKey key
                   && Equals(key);

            private bool Equals(SapHanaCompiledQueryCacheKey other)
                => _relationalCompiledQueryCacheKey.Equals(other._relationalCompiledQueryCacheKey)
                   && _noBackslashEscapes == other._noBackslashEscapes
                ;

            public override int GetHashCode()
                => HashCode.Combine(_relationalCompiledQueryCacheKey,
                    _noBackslashEscapes);
        }
    }
}
