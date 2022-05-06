// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal
{
    public class SapHanaOptionsExtension : RelationalOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;

        public SapHanaOptionsExtension()
        {
            ReplaceLineBreaksWithCharFunction = true;

            // TODO: Change to `true` for EF Core 5.
            IndexOptimizedBooleanColumns = false;

            LimitKeyedOrIndexedStringColumnLength = true;
        }

        public SapHanaOptionsExtension([NotNull] SapHanaOptionsExtension copyFrom)
            : base(copyFrom)
        {
            NoBackslashEscapes = copyFrom.NoBackslashEscapes;
            UpdateSqlModeOnOpen = copyFrom.UpdateSqlModeOnOpen;
            ReplaceLineBreaksWithCharFunction = copyFrom.ReplaceLineBreaksWithCharFunction;
            DefaultDataTypeMappings = copyFrom.DefaultDataTypeMappings;
            SchemaBehavior = copyFrom.SchemaBehavior;
            SchemaNameTranslator = copyFrom.SchemaNameTranslator;
            IndexOptimizedBooleanColumns = copyFrom.IndexOptimizedBooleanColumns;
            LimitKeyedOrIndexedStringColumnLength = copyFrom.LimitKeyedOrIndexedStringColumnLength;
            StringComparisonTranslations = copyFrom.StringComparisonTranslations;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

        protected override RelationalOptionsExtension Clone()
            => new SapHanaOptionsExtension(this);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool NoBackslashEscapes { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool UpdateSqlModeOnOpen { get; private set; }

        public virtual bool ReplaceLineBreaksWithCharFunction { get; private set; }

        public virtual SapHanaDefaultDataTypeMappings DefaultDataTypeMappings { get; private set; }

        public virtual SapHanaSchemaBehavior SchemaBehavior { get; private set; }
        public virtual SapHanaSchemaNameTranslator SchemaNameTranslator { get; private set; }
        public virtual bool IndexOptimizedBooleanColumns { get; private set; }
        public virtual bool LimitKeyedOrIndexedStringColumnLength { get; private set; }
        public virtual bool StringComparisonTranslations { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual SapHanaOptionsExtension WithDisabledBackslashEscaping()
        {
            var clone = (SapHanaOptionsExtension)Clone();
            clone.NoBackslashEscapes = true;
            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual SapHanaOptionsExtension WithSettingSqlModeOnOpen()
        {
            var clone = (SapHanaOptionsExtension)Clone();
            clone.UpdateSqlModeOnOpen = true;
            return clone;
        }

        public virtual SapHanaOptionsExtension WithDisabledLineBreakToCharSubstition()
        {
            var clone = (SapHanaOptionsExtension)Clone();
            clone.ReplaceLineBreaksWithCharFunction = false;
            return clone;
        }

        public virtual SapHanaOptionsExtension WithDefaultDataTypeMappings(SapHanaDefaultDataTypeMappings defaultDataTypeMappings)
        {
            var clone = (SapHanaOptionsExtension)Clone();
            clone.DefaultDataTypeMappings = defaultDataTypeMappings;
            return clone;
        }

        public virtual SapHanaOptionsExtension WithSchemaBehavior(SapHanaSchemaBehavior behavior, SapHanaSchemaNameTranslator translator = null)
        {
            if (behavior == SapHanaSchemaBehavior.Translate && translator == null)
            {
                throw new ArgumentException($"The {nameof(translator)} parameter is mandatory when using `{nameof(SapHanaSchemaBehavior)}.{nameof(SapHanaSchemaBehavior.Translate)}` as the specified behavior.");
            }

            var clone = (SapHanaOptionsExtension)Clone();

            clone.SchemaBehavior = behavior;
            clone.SchemaNameTranslator = behavior == SapHanaSchemaBehavior.Translate
                ? translator
                : null;

            return clone;
        }

        public virtual SapHanaOptionsExtension WithIndexOptimizedBooleanColumns(bool enable)
        {
            var clone = (SapHanaOptionsExtension)Clone();
            clone.IndexOptimizedBooleanColumns = enable;
            return clone;
        }

        public virtual SapHanaOptionsExtension WithKeyedOrIndexedStringColumnLengthLimit(bool enable)
        {
            var clone = (SapHanaOptionsExtension)Clone();
            clone.LimitKeyedOrIndexedStringColumnLength = enable;
            return clone;
        }

        public virtual SapHanaOptionsExtension WithStringComparisonTranslations(bool enable)
        {
            var clone = (SapHanaOptionsExtension)Clone();
            clone.StringComparisonTranslations = enable;
            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override void ApplyServices(IServiceCollection services)
            => services.AddEntityFrameworkSapHana();

        private sealed class ExtensionInfo : RelationalExtensionInfo
        {
            private int? _serviceProviderHash;
            private string _logFragment;

            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            private new SapHanaOptionsExtension Extension
                => (SapHanaOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => true;

            public override string LogFragment
            {
                get
                {
                    if (_logFragment == null)
                    {
                        var builder = new StringBuilder();

                        builder.Append(base.LogFragment);
                        _logFragment = builder.ToString();
                    }

                    return _logFragment;
                }
            }

            public override int GetServiceProviderHashCode()
            {
                if (_serviceProviderHash == null)
                {
                    var hashCode = new HashCode();
                    hashCode.Add(base.GetServiceProviderHashCode());
                    hashCode.Add(Extension.NoBackslashEscapes);
                    hashCode.Add(Extension.UpdateSqlModeOnOpen);
                    hashCode.Add(Extension.ReplaceLineBreaksWithCharFunction);
                    hashCode.Add(Extension.DefaultDataTypeMappings);
                    hashCode.Add(Extension.SchemaBehavior);
                    hashCode.Add(Extension.SchemaNameTranslator);
                    hashCode.Add(Extension.IndexOptimizedBooleanColumns);
                    hashCode.Add(Extension.LimitKeyedOrIndexedStringColumnLength);
                    hashCode.Add(Extension.StringComparisonTranslations);

                    _serviceProviderHash = hashCode.ToHashCode();
                }

                return _serviceProviderHash.Value;
            }

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
                => other is ExtensionInfo otherInfo &&
                   base.ShouldUseSameServiceProvider(other) &&
                   Extension.NoBackslashEscapes == otherInfo.Extension.NoBackslashEscapes &&
                   Extension.UpdateSqlModeOnOpen == otherInfo.Extension.UpdateSqlModeOnOpen &&
                   Extension.ReplaceLineBreaksWithCharFunction == otherInfo.Extension.ReplaceLineBreaksWithCharFunction &&
                   Equals(Extension.DefaultDataTypeMappings, otherInfo.Extension.DefaultDataTypeMappings) &&
                   Extension.SchemaBehavior == otherInfo.Extension.SchemaBehavior &&
                   Extension.SchemaNameTranslator == otherInfo.Extension.SchemaNameTranslator &&
                   Extension.IndexOptimizedBooleanColumns == otherInfo.Extension.IndexOptimizedBooleanColumns &&
                   Extension.LimitKeyedOrIndexedStringColumnLength == otherInfo.Extension.LimitKeyedOrIndexedStringColumnLength &&
                   Extension.StringComparisonTranslations == otherInfo.Extension.StringComparisonTranslations;

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["WonderKidEngine.EntityFrameworkCore.SapHana:" + nameof(SapHanaDbContextOptionsBuilder.DisableBackslashEscaping)] = HashCode.Combine(Extension.NoBackslashEscapes).ToString(CultureInfo.InvariantCulture);
                debugInfo["WonderKidEngine.EntityFrameworkCore.SapHana:" + nameof(SapHanaDbContextOptionsBuilder.SetSqlModeOnOpen)] = HashCode.Combine(Extension.UpdateSqlModeOnOpen).ToString(CultureInfo.InvariantCulture);
                debugInfo["WonderKidEngine.EntityFrameworkCore.SapHana:" + nameof(SapHanaDbContextOptionsBuilder.DisableLineBreakToCharSubstition)] = HashCode.Combine(Extension.ReplaceLineBreaksWithCharFunction).ToString(CultureInfo.InvariantCulture);
                debugInfo["WonderKidEngine.EntityFrameworkCore.SapHana:" + nameof(SapHanaDbContextOptionsBuilder.DefaultDataTypeMappings)] = HashCode.Combine(Extension.DefaultDataTypeMappings).ToString(CultureInfo.InvariantCulture);
                debugInfo["WonderKidEngine.EntityFrameworkCore.SapHana:" + nameof(SapHanaDbContextOptionsBuilder.SchemaBehavior)] = HashCode.Combine(Extension.SchemaBehavior).ToString(CultureInfo.InvariantCulture);
                debugInfo["WonderKidEngine.EntityFrameworkCore.SapHana:" + nameof(Extension.SchemaNameTranslator)] = HashCode.Combine(Extension.SchemaNameTranslator).ToString(CultureInfo.InvariantCulture);
                debugInfo["WonderKidEngine.EntityFrameworkCore.SapHana:" + nameof(SapHanaDbContextOptionsBuilder.EnableIndexOptimizedBooleanColumns)] = HashCode.Combine(Extension.IndexOptimizedBooleanColumns).ToString(CultureInfo.InvariantCulture);
                debugInfo["WonderKidEngine.EntityFrameworkCore.SapHana:" + nameof(SapHanaDbContextOptionsBuilder.LimitKeyedOrIndexedStringColumnLength)] = HashCode.Combine(Extension.LimitKeyedOrIndexedStringColumnLength).ToString(CultureInfo.InvariantCulture);
                debugInfo["WonderKidEngine.EntityFrameworkCore.SapHana:" + nameof(SapHanaDbContextOptionsBuilder.EnableStringComparisonTranslations)] = HashCode.Combine(Extension.StringComparisonTranslations).ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
