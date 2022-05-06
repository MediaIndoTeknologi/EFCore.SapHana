// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sap.Data.Hana;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Internal
{
    public class SapHanaOptions : ISapHanaOptions
    {
        private static readonly SapHanaSchemaNameTranslator _ignoreSchemaNameTranslator = (_, objectName) => objectName;

        public SapHanaOptions()
        {
            ConnectionSettings = new SapHanaConnectionSettings();

            // We explicitly use `utf8mb4` in all instances, where charset based calculations need to be done, but accessing annotations
            // isn't possible (e.g. in `SapHanaTypeMappingSource`).
            // This is also being used as the universal fallback character set, if no character set was explicitly defined for the model,
            // which will result in similar behavior as in previous versions and ensure that databases use a decent/the recommended charset
            // by default, if none was explicitly set.
            DefaultCharSet = CharSet.Utf8Mb4;

            // NCHAR and NVARCHAR are prefdefined by SapHana.
            NationalCharSet = CharSet.Utf8Mb3;

            // Optimize space and performance for GUID columns.
            DefaultGuidCollation = "ascii_general_ci";

            ReplaceLineBreaksWithCharFunction = true;
            DefaultDataTypeMappings = new SapHanaDefaultDataTypeMappings();

            // Throw by default if a schema is being used with any type.
            SchemaNameTranslator = null;

            // TODO: Change to `true` for EF Core 5.
            IndexOptimizedBooleanColumns = false;

            LimitKeyedOrIndexedStringColumnLength = true;
            StringComparisonTranslations = false;
        }

        public virtual void Initialize(IDbContextOptions options)
        {
            var SapHanaOptions = options.FindExtension<SapHanaOptionsExtension>() ?? new SapHanaOptionsExtension();
            var SapHanaJsonOptions = (SapHanaJsonOptionsExtension)options.Extensions.LastOrDefault(e => e is SapHanaJsonOptionsExtension);

            ConnectionSettings = GetConnectionSettings(SapHanaOptions);
            NoBackslashEscapes = SapHanaOptions.NoBackslashEscapes;
            ReplaceLineBreaksWithCharFunction = SapHanaOptions.ReplaceLineBreaksWithCharFunction;
            DefaultDataTypeMappings = ApplyDefaultDataTypeMappings(SapHanaOptions.DefaultDataTypeMappings, ConnectionSettings);
            SchemaNameTranslator = SapHanaOptions.SchemaNameTranslator ?? (SapHanaOptions.SchemaBehavior == SapHanaSchemaBehavior.Ignore
                ? _ignoreSchemaNameTranslator
                : null);
            IndexOptimizedBooleanColumns = SapHanaOptions.IndexOptimizedBooleanColumns;
            JsonChangeTrackingOptions = SapHanaJsonOptions?.JsonChangeTrackingOptions ?? default;
            LimitKeyedOrIndexedStringColumnLength = SapHanaOptions.LimitKeyedOrIndexedStringColumnLength;
            StringComparisonTranslations = SapHanaOptions.StringComparisonTranslations;
        }

        public virtual void Validate(IDbContextOptions options)
        {
            var SapHanaOptions = options.FindExtension<SapHanaOptionsExtension>() ?? new SapHanaOptionsExtension();
            var SapHanaJsonOptions = (SapHanaJsonOptionsExtension)options.Extensions.LastOrDefault(e => e is SapHanaJsonOptionsExtension);
            var connectionSettings = GetConnectionSettings(SapHanaOptions);


            if (!Equals(NoBackslashEscapes, SapHanaOptions.NoBackslashEscapes))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SapHanaDbContextOptionsBuilder.DisableBackslashEscaping),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(ReplaceLineBreaksWithCharFunction, SapHanaOptions.ReplaceLineBreaksWithCharFunction))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SapHanaDbContextOptionsBuilder.DisableLineBreakToCharSubstition),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(DefaultDataTypeMappings, ApplyDefaultDataTypeMappings(SapHanaOptions.DefaultDataTypeMappings ?? new SapHanaDefaultDataTypeMappings(), connectionSettings)))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SapHanaDbContextOptionsBuilder.DefaultDataTypeMappings),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(
                SchemaNameTranslator,
                SapHanaOptions.SchemaBehavior == SapHanaSchemaBehavior.Ignore
                    ? _ignoreSchemaNameTranslator
                    : SchemaNameTranslator))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SapHanaDbContextOptionsBuilder.SchemaBehavior),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(IndexOptimizedBooleanColumns, SapHanaOptions.IndexOptimizedBooleanColumns))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SapHanaDbContextOptionsBuilder.EnableIndexOptimizedBooleanColumns),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(JsonChangeTrackingOptions, SapHanaJsonOptions?.JsonChangeTrackingOptions ?? default))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SapHanaJsonOptionsExtension.JsonChangeTrackingOptions),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(LimitKeyedOrIndexedStringColumnLength, SapHanaOptions.LimitKeyedOrIndexedStringColumnLength))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SapHanaDbContextOptionsBuilder.LimitKeyedOrIndexedStringColumnLength),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(StringComparisonTranslations, SapHanaOptions.StringComparisonTranslations))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SapHanaDbContextOptionsBuilder.EnableStringComparisonTranslations),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }
        }

        protected virtual SapHanaDefaultDataTypeMappings ApplyDefaultDataTypeMappings(SapHanaDefaultDataTypeMappings defaultDataTypeMappings, SapHanaConnectionSettings connectionSettings)
        {
            defaultDataTypeMappings ??= DefaultDataTypeMappings;

            if (defaultDataTypeMappings.ClrDateTime == SapHanaDateTimeType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrDateTime(SapHanaDateTimeType.DateTime6);
                    /*ServerVersion.Supports.DateTime6
                        ? SapHanaDateTimeType.DateTime6
                        : SapHanaDateTimeType.DateTime);*/
            }

            if (defaultDataTypeMappings.ClrDateTimeOffset == SapHanaDateTimeType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrDateTimeOffset(SapHanaDateTimeType.DateTime6);
                    /*ServerVersion.Supports.DateTime6
                        ? SapHanaDateTimeType.DateTime6
                        : SapHanaDateTimeType.DateTime);*/
            }

            if (defaultDataTypeMappings.ClrTimeSpan == SapHanaTimeSpanType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrTimeSpan(SapHanaTimeSpanType.Time6);
                /*ServerVersion.Supports.DateTime6
                    ? SapHanaTimeSpanType.Time6
                    : SapHanaTimeSpanType.Time);*/
            }

            if (defaultDataTypeMappings.ClrTimeOnlyPrecision < 0)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrTimeOnly(6);
                    /*ServerVersion.Supports.DateTime6
                        ? 6
                        : 0);*/
            }

            return defaultDataTypeMappings;
        }

        private static SapHanaConnectionSettings GetConnectionSettings(SapHanaOptionsExtension relationalOptions)
            => relationalOptions.Connection != null
                ? new SapHanaConnectionSettings(relationalOptions.Connection)
                : new SapHanaConnectionSettings(relationalOptions.ConnectionString);

        protected virtual bool Equals(SapHanaOptions other)
        {
            return Equals(ConnectionSettings, other.ConnectionSettings) &&
                   Equals(DefaultCharSet, other.DefaultCharSet) &&
                   Equals(NationalCharSet, other.NationalCharSet) &&
                   Equals(DefaultGuidCollation, other.DefaultGuidCollation) &&
                   NoBackslashEscapes == other.NoBackslashEscapes &&
                   ReplaceLineBreaksWithCharFunction == other.ReplaceLineBreaksWithCharFunction &&
                   Equals(DefaultDataTypeMappings, other.DefaultDataTypeMappings) &&
                   Equals(SchemaNameTranslator, other.SchemaNameTranslator) &&
                   IndexOptimizedBooleanColumns == other.IndexOptimizedBooleanColumns &&
                   JsonChangeTrackingOptions == other.JsonChangeTrackingOptions &&
                   LimitKeyedOrIndexedStringColumnLength == other.LimitKeyedOrIndexedStringColumnLength &&
                   StringComparisonTranslations == other.StringComparisonTranslations;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((SapHanaOptions)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            hashCode.Add(ConnectionSettings);
            hashCode.Add(DefaultCharSet);
            hashCode.Add(NationalCharSet);
            hashCode.Add(DefaultGuidCollation);
            hashCode.Add(NoBackslashEscapes);
            hashCode.Add(ReplaceLineBreaksWithCharFunction);
            hashCode.Add(DefaultDataTypeMappings);
            hashCode.Add(SchemaNameTranslator);
            hashCode.Add(IndexOptimizedBooleanColumns);
            hashCode.Add(JsonChangeTrackingOptions);
            hashCode.Add(LimitKeyedOrIndexedStringColumnLength);
            hashCode.Add(StringComparisonTranslations);

            return hashCode.ToHashCode();
        }

        public virtual SapHanaConnectionSettings ConnectionSettings { get; private set; }
        public virtual CharSet DefaultCharSet { get; private set; }
        public virtual CharSet NationalCharSet { get; }
        public virtual string DefaultGuidCollation { get; private set; }
        public virtual bool NoBackslashEscapes { get; private set; }
        public virtual bool ReplaceLineBreaksWithCharFunction { get; private set; }
        public virtual SapHanaDefaultDataTypeMappings DefaultDataTypeMappings { get; private set; }
        public virtual SapHanaSchemaNameTranslator SchemaNameTranslator { get; private set; }
        public virtual bool IndexOptimizedBooleanColumns { get; private set; }
        public virtual SapHanaJsonChangeTrackingOptions JsonChangeTrackingOptions { get; private set; }
        public virtual bool LimitKeyedOrIndexedStringColumnLength { get; private set; }
        public virtual bool StringComparisonTranslations { get; private set; }
    }
}
