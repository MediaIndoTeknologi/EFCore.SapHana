using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Migrations
{
    /// <summary>
    ///     SapHana specific extension methods for <see cref="MigrationBuilder" />.
    /// </summary>
    public static class SapHanaMigrationBuilderExtensions
    {
        /// <summary>
        ///     <para>
        ///         Returns true if the database provider currently in use is the SapHana provider.
        ///     </para>
        /// </summary>
        /// <param name="migrationBuilder"> The migrationBuilder from the parameters on <see cref="Migration.Up(MigrationBuilder)" /> or <see cref="Migration.Down(MigrationBuilder)" />. </param>
        /// <returns> True if SapHana is being used; false otherwise. </returns>
        public static bool IsSapHana([NotNull] this MigrationBuilder migrationBuilder)
            => string.Equals(migrationBuilder.ActiveProvider,
                typeof(SapHanaOptionsExtension).GetTypeInfo().Assembly.GetName().Name,
                StringComparison.Ordinal);

        /// <summary>
        ///     Builds an <see cref="SapHanaDropPrimaryKeyAndRecreateForeignKeysOperation" /> to drop an existing primary key and optionally
        ///     recreate all foreign keys of the table.
        /// </summary>
        /// <param name="migrationBuilder"> The migrationBuilder from the parameters on <see cref="Migration.Up(MigrationBuilder)" /> or <see cref="Migration.Down(MigrationBuilder)" />. </param>
        /// <param name="name"> The name of the primary key constraint to drop. </param>
        /// <param name="table"> The table that contains the key. </param>
        /// <param name="schema"> The schema that contains the table, or <see langword="null" /> to use the default schema. </param>
        /// <param name="recreateForeignKeys"> The sole reasion to use this extension method. Set this parameter to `true`, to force all
        /// foreign keys of the table be be dropped before the primary key is dropped, and created again afterwards.</param>
        /// <returns> A builder to allow annotations to be added to the operation. </returns>
        public static OperationBuilder<SapHanaDropPrimaryKeyAndRecreateForeignKeysOperation> DropPrimaryKey(
            [NotNull] this MigrationBuilder migrationBuilder,
            [NotNull] string name,
            [NotNull] string table,
            [CanBeNull] string schema = null,
            bool recreateForeignKeys = false)
        {
            Check.NotNull(migrationBuilder, nameof(migrationBuilder));
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));

            var operation = new SapHanaDropPrimaryKeyAndRecreateForeignKeysOperation
            {
                Schema = schema,
                Table = table,
                Name = name,
                RecreateForeignKeys = recreateForeignKeys,
            };
            migrationBuilder.Operations.Add(operation);

            return new OperationBuilder<SapHanaDropPrimaryKeyAndRecreateForeignKeysOperation>(operation);
        }

        /// <summary>
        ///     Builds an <see cref="SapHanaDropPrimaryKeyAndRecreateForeignKeysOperation" /> to drop an existing unique constraint and optionally
        ///     recreate all foreign keys of the table.
        /// </summary>
        /// <param name="migrationBuilder"> The migrationBuilder from the parameters on <see cref="Migration.Up(MigrationBuilder)" /> or <see cref="Migration.Down(MigrationBuilder)" />. </param>
        /// <param name="name"> The name of the constraint to drop. </param>
        /// <param name="table"> The table that contains the constraint. </param>
        /// <param name="schema"> The schema that contains the table, or <see langword="null" /> to use the default schema. </param>
        /// <param name="recreateForeignKeys"> The sole reasion to use this extension method. Set this parameter to `true`, to force all
        /// foreign keys of the table be be dropped before the primary key is dropped, and created again afterwards.</param>
        /// <returns> A builder to allow annotations to be added to the operation. </returns>
        public static OperationBuilder<SapHanaDropUniqueConstraintAndRecreateForeignKeysOperation> DropUniqueConstraint(
            [NotNull] this MigrationBuilder migrationBuilder,
            [NotNull] string name,
            [NotNull] string table,
            [CanBeNull] string schema = null,
            bool recreateForeignKeys = false)
        {
            Check.NotNull(migrationBuilder, nameof(migrationBuilder));
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));

            var operation = new SapHanaDropUniqueConstraintAndRecreateForeignKeysOperation
            {
                Schema = schema,
                Table = table,
                Name = name,
                RecreateForeignKeys = recreateForeignKeys,
            };
            migrationBuilder.Operations.Add(operation);

            return new OperationBuilder<SapHanaDropUniqueConstraintAndRecreateForeignKeysOperation>(operation);
        }
    }
}
