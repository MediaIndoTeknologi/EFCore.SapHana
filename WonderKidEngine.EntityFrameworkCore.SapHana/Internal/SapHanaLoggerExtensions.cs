using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static class SapHanaLoggerExtensions
    {
        private static string DecimalTypeKeyWarning(EventDefinitionBase definition, EventData payload)
        {
            var d = (EventDefinition<string, string>)definition;
            var p = (PropertyEventData)payload;
            return d.GenerateMessage(
                p.Property.Name,
                p.Property.DeclaringEntityType.DisplayName());
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void DecimalTypeDefaultWarning(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Model.Validation> diagnostics,
            [NotNull] IProperty property)
        {
            var definition = SapHanaResources.LogDefaultDecimalTypeColumn(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, property.Name, property.DeclaringEntityType.DisplayName());
            }

            if (diagnostics.NeedsEventData(definition, out var diagnosticSourceEnabled, out var simpleLogEnabled))
            {
                var eventData = new PropertyEventData(
                    definition,
                    DecimalTypeDefaultWarning,
                    property);

                diagnostics.DispatchEventData(definition, eventData, diagnosticSourceEnabled, simpleLogEnabled);
            }
        }

        private static string DecimalTypeDefaultWarning(EventDefinitionBase definition, EventData payload)
        {
            var d = (EventDefinition<string, string>)definition;
            var p = (PropertyEventData)payload;
            return d.GenerateMessage(
                p.Property.Name,
                p.Property.DeclaringEntityType.DisplayName());
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void ByteIdentityColumnWarning(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Model.Validation> diagnostics,
            [NotNull] IProperty property)
        {
            var definition = SapHanaResources.LogByteIdentityColumn(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, property.Name, property.DeclaringEntityType.DisplayName());
            }

            if (diagnostics.NeedsEventData(definition, out var diagnosticSourceEnabled, out var simpleLogEnabled))
            {
                var eventData = new PropertyEventData(
                    definition,
                    ByteIdentityColumnWarning,
                    property);

                diagnostics.DispatchEventData(definition, eventData, diagnosticSourceEnabled, simpleLogEnabled);
            }
        }

        private static string ByteIdentityColumnWarning(EventDefinitionBase definition, EventData payload)
        {
            var d = (EventDefinition<string, string>)definition;
            var p = (PropertyEventData)payload;
            return d.GenerateMessage(
                p.Property.Name,
                p.Property.DeclaringEntityType.DisplayName());
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void ColumnFound(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [NotNull] string tableName,
            [NotNull] string columnName,
            int ordinal,
            [NotNull] string dataTypeName,
            int maxLength,
            int precision,
            int scale,
            bool nullable,
            bool identity,
            [CanBeNull] string defaultValue,
            [CanBeNull] string computedValue,
            bool? stored)
        {
            var definition = SapHanaResources.LogFoundColumn(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(
                    diagnostics,
                    l => l.LogDebug(
                        definition.EventId,
                        null,
                        definition.MessageFormat,
                        tableName,
                        columnName,
                        ordinal,
                        dataTypeName,
                        maxLength,
                        precision,
                        scale,
                        nullable,
                        identity,
                        defaultValue,
                        computedValue,
                        stored));
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void ForeignKeyFound(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [NotNull] string foreignKeyName,
            [NotNull] string tableName,
            [NotNull] string principalTableName,
            [NotNull] string onDeleteAction)
        {
            var definition = SapHanaResources.LogFoundForeignKey(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, foreignKeyName, tableName, principalTableName, onDeleteAction);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void DefaultSchemaFound(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [NotNull] string schemaName)
        {
            var definition = SapHanaResources.LogFoundDefaultSchema(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, schemaName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void TypeAliasFound(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [NotNull] string typeAliasName,
            [NotNull] string systemTypeName)
        {
            var definition = SapHanaResources.LogFoundTypeAlias(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, typeAliasName, systemTypeName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void PrimaryKeyFound(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [NotNull] string primaryKeyName,
            [NotNull] string tableName)
        {
            var definition = SapHanaResources.LogFoundPrimaryKey(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, primaryKeyName, tableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void UniqueConstraintFound(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [NotNull] string uniqueConstraintName,
            [NotNull] string tableName)
        {
            var definition = SapHanaResources.LogFoundUniqueConstraint(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, uniqueConstraintName, tableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void IndexFound(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [NotNull] string indexName,
            [NotNull] string tableName,
            bool unique)
        {
            var definition = SapHanaResources.LogFoundIndex(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, indexName, tableName, unique);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void ForeignKeyReferencesMissingPrincipalTableWarning(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [CanBeNull] string foreignKeyName,
            [CanBeNull] string tableName,
            [CanBeNull] string principalTableName)
        {
            var definition = SapHanaResources.LogPrincipalTableNotInSelectionSet(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, foreignKeyName, tableName, principalTableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void ForeignKeyPrincipalColumnMissingWarning(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [NotNull] string foreignKeyName,
            [NotNull] string tableName,
            [NotNull] string principalColumnName,
            [NotNull] string principalTableName)
        {
            var definition = SapHanaResources.LogPrincipalColumnNotFound(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, foreignKeyName, tableName, principalColumnName, principalTableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void MissingSchemaWarning(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [CanBeNull] string schemaName)
        {
            var definition = SapHanaResources.LogMissingSchema(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, schemaName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void MissingTableWarning(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [CanBeNull] string tableName)
        {
            var definition = SapHanaResources.LogMissingTable(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, tableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void SequenceFound(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [NotNull] string sequenceName,
            [NotNull] string sequenceTypeName,
            bool cyclic,
            int increment,
            long start,
            long min,
            long max)
        {
            // No DiagnosticsSource events because these are purely design-time messages
            var definition = SapHanaResources.LogFoundSequence(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(
                    diagnostics,
                    l => l.LogDebug(
                        definition.EventId,
                        null,
                        definition.MessageFormat,
                        sequenceName,
                        sequenceTypeName,
                        cyclic,
                        increment,
                        start,
                        min,
                        max));
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void TableFound(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            [NotNull] string tableName)
        {
            var definition = SapHanaResources.LogFoundTable(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, tableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void DefaultValueNotSupportedWarning(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Migrations> diagnostics,
            [NotNull] string defaultValue,
            [NotNull] string columnType)
        {
            var definition = SapHanaResources.LogDefaultValueNotSupported(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, new System.Exception()
                {
                    Source = $"{defaultValue} - {columnType}"
                });
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }
    }
}
