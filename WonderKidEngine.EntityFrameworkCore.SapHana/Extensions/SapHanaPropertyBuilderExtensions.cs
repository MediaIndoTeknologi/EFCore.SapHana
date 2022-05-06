using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure;
using WonderKidEngine.EntityFrameworkCore.SapHana.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     SapHana specific extension methods for <see cref="PropertyBuilder" />.
    /// </summary>
    public static class SapHanaPropertyBuilderExtensions
    {
        /// <summary>
        ///     Configures the key property to use the SapHana IDENTITY feature to generate values for new entities,
        ///     when targeting SapHana. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder UseSapHanaIdentityColumn(
            [NotNull] this PropertyBuilder propertyBuilder)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var property = propertyBuilder.Metadata;
            property.SetValueGenerationStrategy(SapHanaValueGenerationStrategy.IdentityColumn);

            return propertyBuilder;
        }

        /// <summary>
        ///     Configures the key property to use the SapHana IDENTITY feature to generate values for new entities,
        ///     when targeting SapHana. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <typeparam name="TProperty"> The type of the property being configured. </typeparam>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder<TProperty> UseSapHanaIdentityColumn<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder)
            => (PropertyBuilder<TProperty>)UseSapHanaIdentityColumn((PropertyBuilder)propertyBuilder);

        /// <summary>
        ///     Configures the key property to use the SapHana Computed feature to generate values for new entities,
        ///     when targeting SapHana. This method sets the property to be <see cref="ValueGenerated.OnAddOrUpdate" />.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder UseSapHanaComputedColumn(
            [NotNull] this PropertyBuilder propertyBuilder)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var property = propertyBuilder.Metadata;
            property.SetValueGenerationStrategy(SapHanaValueGenerationStrategy.ComputedColumn);

            return propertyBuilder;
        }

        /// <summary>
        ///     Configures the key property to use the SapHana Computed feature to generate values for new entities,
        ///     when targeting SapHana. This method sets the property to be <see cref="ValueGenerated.OnAddOrUpdate" />.
        /// </summary>
        /// <typeparam name="TProperty"> The type of the property being configured. </typeparam>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder<TProperty> UseSapHanaComputedColumn<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder)
            => (PropertyBuilder<TProperty>)UseSapHanaComputedColumn((PropertyBuilder)propertyBuilder);

        /// <summary>
        /// Configures the charset for the property's column.
        /// </summary>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="charSet">The name of the charset to configure for the property's column.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder HasCharSet(
            [NotNull] this PropertyBuilder propertyBuilder,
            string charSet)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var property = propertyBuilder.Metadata;
            property.SetCharSet(charSet);

            return propertyBuilder;
        }

        /// <summary>
        /// Configures the charset for the property's column.
        /// </summary>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="charSet">The name of the charset to configure for the property's column.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder<TProperty> HasCharSet<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            string charSet)
            => (PropertyBuilder<TProperty>)HasCharSet((PropertyBuilder)propertyBuilder, charSet);

        /// <summary>
        /// Configures the charset for the property's column.
        /// </summary>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="charSet">The name of the charset to configure for the property's column.</param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionPropertyBuilder HasCharSet(
            this IConventionPropertyBuilder propertyBuilder,
            string charSet,
            bool fromDataAnnotation = false)
        {
            if (!propertyBuilder.CanSetCharSet(charSet, fromDataAnnotation))
            {
                return null;
            }

            propertyBuilder.Metadata.SetCharSet(charSet, fromDataAnnotation);
            return propertyBuilder;
        }

        /// <summary>
        /// Returns a value indicating whether the SapHana character set can be set on the column associated with this property.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <param name="charSet"> The name of the character set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <see langword="true" /> if the given value can be set as the character set for the column. </returns>
        public static bool CanSetCharSet(
            this IConventionPropertyBuilder propertyBuilder,
            string charSet,
            bool fromDataAnnotation = false)
            => propertyBuilder.CanSetAnnotation(
                SapHanaAnnotationNames.CharSet,
                charSet,
                fromDataAnnotation);

        /// <summary>
        /// Configures the collation for the property's column.
        /// </summary>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="collation">The name of the collation to configure for the property's column.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        [Obsolete("Call 'UseCollation()' instead.")]
        public static PropertyBuilder HasCollation(
            [NotNull] this PropertyBuilder propertyBuilder,
            string collation)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            return propertyBuilder.UseCollation(collation);
        }

        /// <summary>
        /// Configures the collation for the property's column.
        /// </summary>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="collation">The name of the collation to configure for the property's column.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        [Obsolete("Call 'UseCollation()' instead.")]
        public static PropertyBuilder<TProperty> HasCollation<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            string collation)
            => (PropertyBuilder<TProperty>)HasCollation((PropertyBuilder)propertyBuilder, collation);

        /// <summary>
        /// Restricts the Spatial Reference System Identifier (SRID) for the property's column.
        /// </summary>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="srid">The SRID to configure for the property's column.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder HasSpatialReferenceSystem(
            [NotNull] this PropertyBuilder propertyBuilder,
            int? srid)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var property = propertyBuilder.Metadata;
            property.SetSpatialReferenceSystem(srid);

            return propertyBuilder;
        }

        /// <summary>
        /// Restricts the Spatial Reference System Identifier (SRID) for the property's column.
        /// </summary>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="srid">The SRID to configure for the property's column.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder<TProperty> HasSpatialReferenceSystem<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            int? srid)
            => (PropertyBuilder<TProperty>)HasSpatialReferenceSystem((PropertyBuilder)propertyBuilder, srid);
    }
}
