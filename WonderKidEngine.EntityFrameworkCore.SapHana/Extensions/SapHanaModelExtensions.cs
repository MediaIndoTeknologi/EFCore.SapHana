using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using WonderKidEngine.EntityFrameworkCore.SapHana.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class SapHanaModelExtensions
    {
        #region ValueGenerationStrategy

        /// <summary>
        ///     Returns the <see cref="SapHanaValueGenerationStrategy" /> to use for properties
        ///     of keys in the model, unless the property has a strategy explicitly set.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The default <see cref="SapHanaValueGenerationStrategy" />. </returns>
        public static SapHanaValueGenerationStrategy? GetValueGenerationStrategy([NotNull] this IModel model)
            => (SapHanaValueGenerationStrategy?)model[SapHanaAnnotationNames.ValueGenerationStrategy];

        /// <summary>
        ///     Attempts to set the <see cref="SapHanaValueGenerationStrategy" /> to use for properties
        ///     of keys in the model that don't have a strategy explicitly set.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="value"> The value to set. </param>
        public static void SetValueGenerationStrategy([NotNull] this IMutableModel model, SapHanaValueGenerationStrategy? value)
            => model.SetOrRemoveAnnotation(SapHanaAnnotationNames.ValueGenerationStrategy, value);

        /// <summary>
        ///     Attempts to set the <see cref="SapHanaValueGenerationStrategy" /> to use for properties
        ///     of keys in the model that don't have a strategy explicitly set.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static SapHanaValueGenerationStrategy? SetValueGenerationStrategy(
            [NotNull] this IConventionModel model, SapHanaValueGenerationStrategy? value, bool fromDataAnnotation = false)
        {
            model.SetOrRemoveAnnotation(SapHanaAnnotationNames.ValueGenerationStrategy, value, fromDataAnnotation);

            return value;
        }

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default <see cref="SapHanaValueGenerationStrategy" />.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default <see cref="SapHanaValueGenerationStrategy" />. </returns>
        public static ConfigurationSource? GetValueGenerationStrategyConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(SapHanaAnnotationNames.ValueGenerationStrategy)?.GetConfigurationSource();

        #endregion ValueGenerationStrategy

        #region CharSet

        /// <summary>
        ///     Returns the character set to use as the default for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The default character set. </returns>
        public static string GetCharSet([NotNull] this IModel model)
            => model[SapHanaAnnotationNames.CharSet] as string;

        /// <summary>
        ///     Attempts to set the character set to use as the default for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="charSet"> The default character set. </param>
        public static void SetCharSet([NotNull] this IMutableModel model, string charSet)
            => model.SetOrRemoveAnnotation(SapHanaAnnotationNames.CharSet, charSet);

        /// <summary>
        ///     Attempts to set the character set to use as the default for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="charSet"> The default character set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static string SetCharSet([NotNull] this IConventionModel model, string charSet, bool fromDataAnnotation = false)
        {
            model.SetOrRemoveAnnotation(SapHanaAnnotationNames.CharSet, charSet, fromDataAnnotation);

            return charSet;
        }

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default character set of the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default character set. </returns>
        public static ConfigurationSource? GetCharSetConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(SapHanaAnnotationNames.CharSet)?.GetConfigurationSource();

        #endregion CharSet

        #region CharSetDelegation

        /// <summary>
        ///     Returns the character set delegation modes for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The character set delegation modes. </returns>
        public static DelegationModes? GetCharSetDelegation([NotNull] this IModel model)
            => ObjectToEnumConverter.GetEnumValue<DelegationModes>(model[SapHanaAnnotationNames.CharSetDelegation]) ??
               (model[SapHanaAnnotationNames.CharSetDelegation] is bool explicitlyDelegateToChildren
                   ? explicitlyDelegateToChildren
                       ? DelegationModes.ApplyToAll
                       : DelegationModes.ApplyToDatabases
                   : null);

        /// <summary>
        ///     Attempts to set the character set delegation modes for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the character set and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        public static void SetCharSetDelegation([NotNull] this IMutableModel model, DelegationModes? delegationModes)
            => model.SetOrRemoveAnnotation(SapHanaAnnotationNames.CharSetDelegation, delegationModes);

        /// <summary>
        ///     Attempts to set the character set delegation modes for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the character set and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static DelegationModes? SetCharSetDelegation([NotNull] this IConventionModel model, DelegationModes? delegationModes, bool fromDataAnnotation = false)
        {
            model.SetOrRemoveAnnotation(SapHanaAnnotationNames.CharSetDelegation, delegationModes, fromDataAnnotation);

            return delegationModes;
        }

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the character set delegation modes of the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default character set delegation modes. </returns>
        public static ConfigurationSource? GetCharSetDelegationConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(SapHanaAnnotationNames.CharSetDelegation)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the actual character set delegation modes for the model/database.
        ///     Always returns a concrete value and never returns <see cref="DelegationModes.Default"/>.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The actual character set delegation modes. </returns>
        public static DelegationModes GetActualCharSetDelegation([NotNull] this IModel model)
        {
            var delegationModes = model.GetCharSetDelegation() ?? DelegationModes.Default;
            return delegationModes == DelegationModes.Default
                ? DelegationModes.ApplyToAll
                : delegationModes;
        }

        #endregion CharSetDelegation

        #region CollationDelegation

        /// <summary>
        ///     Returns the collation delegation modes for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The collation delegation modes. </returns>
        public static DelegationModes? GetCollationDelegation([NotNull] this IModel model)
            => ObjectToEnumConverter.GetEnumValue<DelegationModes>(model[SapHanaAnnotationNames.CollationDelegation]) ??
               (model[SapHanaAnnotationNames.CollationDelegation] is bool explicitlyDelegateToChildren
                   ? explicitlyDelegateToChildren
                       ? DelegationModes.ApplyToAll
                       : DelegationModes.ApplyToDatabases
                   : null);

        /// <summary>
        ///     Attempts to set the collation delegation modes for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the collation and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        public static void SetCollationDelegation([NotNull] this IMutableModel model, DelegationModes? delegationModes)
            => model.SetOrRemoveAnnotation(SapHanaAnnotationNames.CollationDelegation, delegationModes);

        /// <summary>
        ///     Attempts to set the collation delegation modes for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the collation and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static DelegationModes? SetCollationDelegation([NotNull] this IConventionModel model, DelegationModes? delegationModes, bool fromDataAnnotation = false)
        {
            model.SetOrRemoveAnnotation(SapHanaAnnotationNames.CollationDelegation, delegationModes, fromDataAnnotation);

            return delegationModes;
        }

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the collation delegation modes of the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default collation delegation modes. </returns>
        public static ConfigurationSource? GetCollationDelegationConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(SapHanaAnnotationNames.CollationDelegation)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the actual collation delegation modes for the model/database.
        ///     Always returns a concrete value and never returns <see cref="DelegationModes.Default"/>.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The actual collation delegation modes. </returns>
        public static DelegationModes GetActualCollationDelegation([NotNull] this IModel model)
        {
            var delegationModes = model.GetCollationDelegation() ?? DelegationModes.Default;
            return delegationModes == DelegationModes.Default
                ? DelegationModes.ApplyToAll
                : delegationModes;
        }

        #endregion CollationDelegation

        #region GuidCollation

        /// <summary>
        ///     Returns the default collation used for char-based <see cref="Guid"/> columns.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns>
        ///     The <see cref="Guid"/> collation setting.
        ///     An empty string means that no explicit collation will be applied, while <see langword="null"/> means that the default
        ///     collation `ascii_general_ci` will be applied.
        /// </returns>
        public static string GetGuidCollation([NotNull] this IModel model)
            => model[SapHanaAnnotationNames.GuidCollation] as string;

        /// <summary>
        ///     Attempts to set the default collation used for char-based <see cref="Guid"/> columns.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="collation">
        ///     The <see cref="Guid"/> collation setting.
        ///     An empty string means that no explicit collation will be applied, while <see langword="null"/> means that the default
        ///     collation `ascii_general_ci` will be applied.
        /// </param>
        public static void SetGuidCollation([NotNull] this IMutableModel model, string collation)
            => model.SetOrRemoveAnnotation(SapHanaAnnotationNames.GuidCollation, collation);

        /// <summary>
        ///     Attempts to set the default collation used for char-based <see cref="Guid"/> columns.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="collation">
        ///     The <see cref="Guid"/> collation setting.
        ///     An empty string means that no explicit collation will be applied, while <see langword="null"/> means that the default
        ///     collation `ascii_general_ci` will be applied.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static string SetGuidCollation([NotNull] this IConventionModel model, string collation, bool fromDataAnnotation = false)
        {
            model.SetOrRemoveAnnotation(SapHanaAnnotationNames.GuidCollation, collation, fromDataAnnotation);

            return collation;
        }

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the <see cref="Guid"/> collation setting.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" />. </returns>
        public static ConfigurationSource? GetGuidCollationConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(SapHanaAnnotationNames.GuidCollation)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the actual <see cref="Guid"/> default collation used for char-based <see cref="Guid"/> columns.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="defaultCollation"> The default collation to use, if no default collation has been explicitly set. </param>
        /// <returns>
        ///     <see langword="null"/> if no collation should be set, otherwise the concrete collation to apply.
        /// </returns>
        public static string GetActualGuidCollation([NotNull] this IModel model, [CanBeNull] string defaultCollation)
            => model.GetGuidCollation() switch
            {
                null => defaultCollation,
                {Length: <= 0} => null,
                var c => c
            };

        #endregion GuidCollation
    }
}
