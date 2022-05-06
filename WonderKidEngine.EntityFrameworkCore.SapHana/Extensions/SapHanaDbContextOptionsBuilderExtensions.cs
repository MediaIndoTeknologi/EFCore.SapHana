using System;
using System.Data.Common;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Sap.Data.Hana;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides extension methods on <see cref="DbContextOptionsBuilder"/> and <see cref="DbContextOptionsBuilder{T}"/>
    /// to configure a <see cref="DbContext"/> to use with SapHana/MariaDB and Pomelo.EntityFrameworkCore.SapHana.
    /// </summary>
    public static class SapHanaDbContextOptionsBuilderExtensions
    {
        /// <summary>
        ///     <para>
        ///         Configures the context to connect to a SapHana compatible database, but without initially setting any
        ///         <see cref="DbConnection" /> or connection string.
        ///     </para>
        ///     <para>
        ///         The connection or connection string must be set before the <see cref="DbContext" /> is used to connect
        ///         to a database. Set a connection using <see cref="RelationalDatabaseFacadeExtensions.SetDbConnection" />.
        ///         Set a connection string using <see cref="RelationalDatabaseFacadeExtensions.SetConnectionString" />.
        ///     </para>
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="SapHanaServerVersion"/> (for SapHana) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="SapHanaOptionsAction"> An optional action to allow additional SapHana specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseSapHana(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [CanBeNull] Action<SapHanaDbContextOptionsBuilder> SapHanaOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            var extension = GetOrCreateExtension(optionsBuilder);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            ConfigureWarnings(optionsBuilder);
            SapHanaOptionsAction?.Invoke(new SapHanaDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     Configures the context to connect to a SapHana compatible database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connectionString"> The connection string of the database to connect to. </param>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="SapHanaServerVersion"/> (for SapHana) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="SapHanaOptionsAction"> An optional action to allow additional SapHana specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseSapHana(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string connectionString,
            [CanBeNull] Action<SapHanaDbContextOptionsBuilder> SapHanaOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotEmpty(connectionString, nameof(connectionString));

            var resolvedConnectionString = new NamedConnectionStringResolver(optionsBuilder.Options)
                .ResolveConnectionString(connectionString);

            var csb = new HanaConnectionStringBuilder(resolvedConnectionString);

            resolvedConnectionString = csb.ConnectionString;

            var extension = (SapHanaOptionsExtension)GetOrCreateExtension(optionsBuilder)
                .WithConnectionString(resolvedConnectionString);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            ConfigureWarnings(optionsBuilder);
            SapHanaOptionsAction?.Invoke(new SapHanaDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     Configures the context to connect to a SapHana compatible database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connection">
        ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
        ///     in the open state then EF will not open or close the connection. If the connection is in the closed
        ///     state then EF will open and close the connection as needed.
        /// </param>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="SapHanaServerVersion"/> (for SapHana) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="SapHanaOptionsAction"> An optional action to allow additional SapHana specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseSapHana(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] DbConnection connection,
            [CanBeNull] Action<SapHanaDbContextOptionsBuilder> SapHanaOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotNull(connection, nameof(connection));

            var resolvedConnectionString = connection.ConnectionString is not null
                ? new NamedConnectionStringResolver(optionsBuilder.Options)
                    .ResolveConnectionString(connection.ConnectionString)
                : null;

            var csb = new HanaConnectionStringBuilder(resolvedConnectionString);

            var extension = (SapHanaOptionsExtension)GetOrCreateExtension(optionsBuilder)
                .WithConnection(connection);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            ConfigureWarnings(optionsBuilder);
            SapHanaOptionsAction?.Invoke(new SapHanaDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     <para>
        ///         Configures the context to connect to a SapHana compatible database, but without initially setting any
        ///         <see cref="DbConnection" /> or connection string.
        ///     </para>
        ///     <para>
        ///         The connection or connection string must be set before the <see cref="DbContext" /> is used to connect
        ///         to a database. Set a connection using <see cref="RelationalDatabaseFacadeExtensions.SetDbConnection" />.
        ///         Set a connection string using <see cref="RelationalDatabaseFacadeExtensions.SetConnectionString" />.
        ///     </para>
        /// </summary>
        /// <typeparam name="TContext"> The type of context to be configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="SapHanaServerVersion"/> (for SapHana) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="SapHanaOptionsAction"> An optional action to allow additional SapHana specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseSapHana<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [CanBeNull] Action<SapHanaDbContextOptionsBuilder> SapHanaOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseSapHana(
                (DbContextOptionsBuilder)optionsBuilder, SapHanaOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a SapHana compatible database.
        /// </summary>
        /// <typeparam name="TContext"> The type of context to be configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connectionString"> The connection string of the database to connect to. </param>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="SapHanaServerVersion"/> (for SapHana) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="SapHanaOptionsAction"> An optional action to allow additional SapHana specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseSapHana<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] string connectionString,
            [CanBeNull] Action<SapHanaDbContextOptionsBuilder> SapHanaOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseSapHana(
                (DbContextOptionsBuilder)optionsBuilder, connectionString, SapHanaOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a SapHana compatible database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connection">
        ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
        ///     in the open state then EF will not open or close the connection. If the connection is in the closed
        ///     state then EF will open and close the connection as needed.
        /// </param>
        /// <typeparam name="TContext"> The type of context to be configured. </typeparam>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="SapHanaServerVersion"/> (for SapHana) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="SapHanaOptionsAction"> An optional action to allow additional SapHana specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseSapHana<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] DbConnection connection,
            [CanBeNull] Action<SapHanaDbContextOptionsBuilder> SapHanaOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseSapHana(
                (DbContextOptionsBuilder)optionsBuilder, connection, SapHanaOptionsAction);

        private static SapHanaOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<SapHanaOptionsExtension>()
               ?? new SapHanaOptionsExtension();

        private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
        {
            var coreOptionsExtension
                = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
                  ?? new CoreOptionsExtension();

            coreOptionsExtension = coreOptionsExtension.WithWarningsConfiguration(
                coreOptionsExtension.WarningsConfiguration.TryWithExplicit(
                    RelationalEventId.AmbientTransactionWarning, WarningBehavior.Throw));

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
        }
    }
}
