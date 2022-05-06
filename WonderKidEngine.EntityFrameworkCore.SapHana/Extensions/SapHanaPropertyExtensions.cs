using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WonderKidEngine.EntityFrameworkCore.SapHana.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Metadata.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     SapHana specific extension methods for properties.
    /// </summary>
    public static class SapHanaPropertyExtensions
    {
        /// <summary>
        ///     <para>
        ///         Returns the <see cref="SapHanaValueGenerationStrategy" /> to use for the property.
        ///     </para>
        ///     <para>
        ///         If no strategy is set for the property, then the strategy to use will be taken from the <see cref="IModel" />.
        ///     </para>
        /// </summary>
        /// <returns> The strategy, or <see cref="SapHanaValueGenerationStrategy.None"/> if none was set. </returns>
        public static SapHanaValueGenerationStrategy? GetValueGenerationStrategy([NotNull] this IReadOnlyProperty property, StoreObjectIdentifier storeObject = default)
        {
            var annotation = property[SapHanaAnnotationNames.ValueGenerationStrategy];
            if (annotation != null)
            {
                // Allow users to use the underlying type value instead of the enum itself.
                // Workaround for: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.SapHana/issues/1205
                //return ObjectToEnumConverter.GetEnumValue<SapHanaValueGenerationStrategy>(annotation);
                return ObjectToEnumConverter.GetEnumValue<SapHanaValueGenerationStrategy>(annotation);
            }

            if (property.GetContainingForeignKeys().Any(fk => !fk.IsBaseLinking()) ||
                property.TryGetDefaultValue(storeObject, out _) ||
                property.GetDefaultValueSql() != null ||
                property.GetComputedColumnSql() != null)
            {
                return null;
            }

            if (storeObject != default &&
                property.ValueGenerated == ValueGenerated.Never)
            {
                return property.FindSharedStoreObjectRootProperty(storeObject)
                    ?.GetValueGenerationStrategy(storeObject);
            }

            if (property.ValueGenerated == ValueGenerated.OnAdd &&
                IsCompatibleIdentityColumn(property))
            {
                return SapHanaValueGenerationStrategy.IdentityColumn;
            }

            if (property.ValueGenerated == ValueGenerated.OnAddOrUpdate &&
                IsCompatibleComputedColumn(property))
            {
                return SapHanaValueGenerationStrategy.ComputedColumn;
            }

            return null;
        }

        /// <summary>
        ///     Sets the <see cref="SapHanaValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="value"> The strategy to use. </param>
        public static void SetValueGenerationStrategy(
            [NotNull] this IMutableProperty property, SapHanaValueGenerationStrategy? value)
        {
            CheckValueGenerationStrategy(property, value);

            property.SetOrRemoveAnnotation(SapHanaAnnotationNames.ValueGenerationStrategy, value);
        }

        /// <summary>
        ///     Sets the <see cref="SapHanaValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="value"> The strategy to use. </param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static SapHanaValueGenerationStrategy? SetValueGenerationStrategy([NotNull] this IConventionProperty property, SapHanaValueGenerationStrategy? value, bool fromDataAnnotation = false)
        {
            CheckValueGenerationStrategy(property, value);

            property.SetOrRemoveAnnotation(SapHanaAnnotationNames.ValueGenerationStrategy, value, fromDataAnnotation);

            return value;
        }

        /// <summary>
        /// Returns the <see cref="ConfigurationSource" /> for the <see cref="SapHanaValueGenerationStrategy" />.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the <see cref="SapHanaValueGenerationStrategy" />.</returns>
        public static ConfigurationSource? GetValueGenerationStrategyConfigurationSource(this IConventionProperty property)
            => property.FindAnnotation(SapHanaAnnotationNames.ValueGenerationStrategy)?.GetConfigurationSource();

        private static void CheckValueGenerationStrategy(IReadOnlyProperty property, SapHanaValueGenerationStrategy? value)
        {
            if (value != null)
            {
                var propertyType = property.ClrType;

                if (value == SapHanaValueGenerationStrategy.IdentityColumn
                    && !IsCompatibleIdentityColumn(property))
                {
                    throw new ArgumentException(
                            $"Identity value generation cannot be used for the property '{property.Name}' on entity type '{property.DeclaringEntityType.DisplayName()}' because the property type is '{propertyType.ShortDisplayName()}'. Identity value generation can only be used with integer, DateTime, and DateTimeOffset properties.");
                }

                if (value == SapHanaValueGenerationStrategy.ComputedColumn
                    && !IsCompatibleComputedColumn(property))
                {
                    throw new ArgumentException(
                        $"Computed value generation cannot be used for the property '{property.Name}' on entity type '{property.DeclaringEntityType.DisplayName()}' because the property type is '{propertyType.ShortDisplayName()}'. Computed value generation can only be used with DateTime and DateTimeOffset properties.");
                }
            }
        }

        /// <summary>
        ///     Returns a value indicating whether the property is compatible with <see cref="SapHanaValueGenerationStrategy.IdentityColumn"/>.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> <see langword="true"/> if compatible. </returns>
        public static bool IsCompatibleIdentityColumn(IReadOnlyProperty property)
            => IsCompatibleAutoIncrementColumn(property) ||
               IsCompatibleCurrentTimestampColumn(property);

        /// <summary>
        ///     Returns a value indicating whether the property is compatible with an `AUTO_INCREMENT` column.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> <see langword="true"/> if compatible. </returns>
        public static bool IsCompatibleAutoIncrementColumn(IReadOnlyProperty property)
        {
            var valueConverter = GetConverter(property);
            var type = (valueConverter?.ProviderClrType ?? property.ClrType).UnwrapNullableType();
            return type.IsInteger() ||
                   type == typeof(decimal);
        }

        /// <summary>
        ///     Returns a value indicating whether the property is compatible with a `CURRENT_TIMESTAMP` column default.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> <see langword="true"/> if compatible. </returns>
        public static bool IsCompatibleCurrentTimestampColumn(IReadOnlyProperty property)
        {
            var valueConverter = GetConverter(property);
            var type = (valueConverter?.ProviderClrType ?? property.ClrType).UnwrapNullableType();
            return type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset);
        }

        /// <summary>
        ///     Returns a value indicating whether the property is compatible with <see cref="SapHanaValueGenerationStrategy.ComputedColumn"/>.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> <see langword="true"/> if compatible. </returns>
        public static bool IsCompatibleComputedColumn(IReadOnlyProperty property)
        {
            var type = property.ClrType;

            // RowVersion uses byte[] and the BytesToDateTimeConverter.
            return (type == typeof(DateTime) || type == typeof(DateTimeOffset)) && !HasConverter(property)
                   || type == typeof(byte[]) && !HasExternalConverter(property);
        }

        private static bool HasConverter(IReadOnlyProperty property)
            => GetConverter(property) != null;

        private static bool HasExternalConverter(IReadOnlyProperty property)
        {
            var converter = GetConverter(property);
            return converter != null && !(converter is BytesToDateTimeConverter);
        }

        private static ValueConverter GetConverter(IReadOnlyProperty property)
            => property.FindTypeMapping()?.Converter ?? property.GetValueConverter();

        /// <summary>
        /// Returns the name of the charset used by the column of the property.
        /// </summary>
        /// <param name="property">The property of which to get the columns charset from.</param>
        /// <returns>The name of the charset or null, if no explicit charset was set.</returns>
        public static string GetCharSet([NotNull] this IReadOnlyProperty property)
            => property[SapHanaAnnotationNames.CharSet] as string ??
               property.GetSapHanaLegacyCharSet();

        /// <summary>
        /// Returns the name of the charset used by the column of the property, defined as part of the column type.
        /// </summary>
        /// <remarks>
        /// It was common before 5.0 to specify charsets this way, because there were no character set specific annotations available yet.
        /// Users might still use migrations generated with previous versions and just add newer migrations on top of those.
        /// </remarks>
        /// <param name="property">The property of which to get the columns charset from.</param>
        /// <returns>The name of the charset or null, if no explicit charset was set.</returns>
        internal static string GetSapHanaLegacyCharSet([NotNull] this IReadOnlyProperty property)
        {
            var columnType = property.GetColumnType();

            if (columnType is not null)
            {
                const string characterSet = "character set";
                const string charSet = "charset";

                var characterSetOccurrenceIndex = columnType.IndexOf(characterSet, StringComparison.OrdinalIgnoreCase);
                var clauseLength = characterSet.Length;

                if (characterSetOccurrenceIndex < 0)
                {
                    characterSetOccurrenceIndex = columnType.IndexOf(charSet, StringComparison.OrdinalIgnoreCase);
                    clauseLength = charSet.Length;
                }

                if (characterSetOccurrenceIndex >= 0)
                {
                    var result = string.Concat(
                        columnType.Skip(characterSetOccurrenceIndex + clauseLength)
                            .SkipWhile(c => c == ' ')
                            .TakeWhile(c => c != ' '));

                    if (result.Length > 0)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the name of the charset in use by the column of the property.
        /// </summary>
        /// <param name="property">The property to set the columns charset for.</param>
        /// <param name="charSet">The name of the charset used for the column of the property.</param>
        public static void SetCharSet([NotNull] this IMutableProperty property, string charSet)
            => property.SetOrRemoveAnnotation(SapHanaAnnotationNames.CharSet, charSet);

        /// <summary>
        /// Sets the name of the charset in use by the column of the property.
        /// </summary>
        /// <param name="property">The property to set the columns charset for.</param>
        /// <param name="charSet">The name of the charset used for the column of the property.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static string SetCharSet([NotNull] this IConventionProperty property, string charSet, bool fromDataAnnotation = false)
        {
            property.SetOrRemoveAnnotation(SapHanaAnnotationNames.CharSet, charSet, fromDataAnnotation);

            return charSet;
        }

        /// <summary>
        /// Returns the <see cref="ConfigurationSource" /> for the character set.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the character set.</returns>
        public static ConfigurationSource? GetCharSetConfigurationSource(this IConventionProperty property)
            => property.FindAnnotation(SapHanaAnnotationNames.CharSet)?.GetConfigurationSource();

        /// <summary>
        /// Returns the name of the collation used by the column of the property.
        /// </summary>
        /// <param name="property">The property of which to get the columns collation from.</param>
        /// <returns>The name of the collation or null, if no explicit collation was set.</returns>
#pragma warning disable 618
        internal static string GetSapHanaLegacyCollation([NotNull] this IReadOnlyProperty property)
            => property[SapHanaAnnotationNames.Collation] as string;
#pragma warning restore 618

        /// <summary>
        /// Returns the Spatial Reference System Identifier (SRID) used by the column of the property.
        /// </summary>
        /// <param name="property">The property of which to get the columns SRID from.</param>
        /// <returns>The SRID or null, if no explicit SRID has been set.</returns>
        public static int? GetSpatialReferenceSystem([NotNull] this IReadOnlyProperty property)
            => (int?)property[SapHanaAnnotationNames.SpatialReferenceSystemId];

        /// <summary>
        /// Sets the Spatial Reference System Identifier (SRID) in use by the column of the property.
        /// </summary>
        /// <param name="property">The property to set the columns SRID for.</param>
        /// <param name="srid">The SRID to configure for the property's column.</param>
        public static void SetSpatialReferenceSystem([NotNull] this IMutableProperty property, int? srid)
            => property.SetOrRemoveAnnotation(SapHanaAnnotationNames.SpatialReferenceSystemId, srid);

        /// <summary>
        /// Sets the Spatial Reference System Identifier (SRID) in use by the column of the property.
        /// </summary>
        /// <param name="property">The property to set the columns SRID for.</param>
        /// <param name="srid">The SRID to configure for the property's column.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static int? SetSpatialReferenceSystem([NotNull] this IConventionProperty property, int? srid, bool fromDataAnnotation = false)
        {
            property.SetOrRemoveAnnotation(SapHanaAnnotationNames.SpatialReferenceSystemId, srid, fromDataAnnotation);

            return srid;
        }

        /// <summary>
        /// Returns the <see cref="ConfigurationSource" /> for the Spatial Reference System Identifier (SRID).
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the Spatial Reference System Identifier (SRID).</returns>
        public static ConfigurationSource? GetSpatialReferenceSystemConfigurationSource(this IConventionProperty property)
            => property.FindAnnotation(SapHanaAnnotationNames.SpatialReferenceSystemId)?.GetConfigurationSource();
    }
}
