using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;
using Sap.Data.Hana;
using WonderKidEngine.EntityFrameworkCore.SapHana.Extensions;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Metadata.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Scaffolding.Internal
{
    public class SapHanaDatabaseModelFactory : DatabaseModelFactory
    {
        private readonly IDiagnosticsLogger<DbLoggerCategory.Scaffolding> _logger;
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly ISapHanaOptions _options;

        protected virtual SapHanaScaffoldingConnectionSettings Settings { get; set; }

        public SapHanaDatabaseModelFactory(
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger,
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            [NotNull] ISapHanaOptions options)
        {
            Check.NotNull(logger, nameof(logger));
            Check.NotNull(typeMappingSource, nameof(typeMappingSource));
            Check.NotNull(options, nameof(options));

            _logger = logger;
            _typeMappingSource = typeMappingSource;
            _options = options;
            Settings = new SapHanaScaffoldingConnectionSettings(string.Empty);
        }

        public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
        {
            Check.NotEmpty(connectionString, nameof(connectionString));
            Check.NotNull(options, nameof(options));

            Settings = new SapHanaScaffoldingConnectionSettings(connectionString);

            using var connection = new HanaConnection(Settings.GetProviderCompatibleConnectionString());
            return Create(connection, options);
        }

        public override DatabaseModel Create(DbConnection connection, DatabaseModelFactoryOptions options)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(options, nameof(options));

            SetupSapHanaOptions(connection);
            var connectionStartedOpen = connection.State == ConnectionState.Open;
            if (!connectionStartedOpen)
            {
                connection.Open();
            }

            try
            {
                return GetDatabase(connection, options);
            }
            finally
            {
                if (!connectionStartedOpen)
                {
                    connection.Close();
                }
            }
        }

        protected virtual void SetupSapHanaOptions(DbConnection connection)
        {
            // Set the actual server version from the open connection here, so we can
            // access it from ISapHanaOptions later when generating the code for the
            // `UseSapHana()` call.

            if (Equals(_options, new SapHanaOptions()))
            {
                _options.Initialize(
                    new DbContextOptionsBuilder()
                        .UseSapHana(connection)
                        .Options);
            }
        }

        protected virtual DatabaseModel GetDatabase(DbConnection connection, DatabaseModelFactoryOptions options)
        {
            var databaseModel = new DatabaseModel
            {
                DatabaseName = connection.Database,
                DefaultSchema = GetDefaultSchema(connection)
            };

            var schemaList = options.Schemas.ToList();
            var tableList = options.Tables.ToList();
            var tableFilter = GenerateTableFilter(tableList, schemaList);

            var tables = GetTables(connection, tableFilter, (string)databaseModel[SapHanaAnnotationNames.CharSet], databaseModel.Collation);
            foreach (var table in tables)
            {
                table.Database = databaseModel;
                databaseModel.Tables.Add(table);
            }

            return databaseModel;
        }

        protected virtual string GetDefaultSchema(DbConnection connection)
            => null;

        protected virtual Func<string, string, bool> GenerateTableFilter(
            IReadOnlyList<string> tables,
            IReadOnlyList<string> schemas)
            => tables.Count > 0 ? (s, t) => tables.Contains(t) : (Func<string, string, bool>)null;

        //TODO: NANTI LEPAS SCHEMA NYA
        private const string GetTablesQuery = @"SELECT SCHEMA_NAME,TABLE_NAME,TABLE_TYPE FROM SYS.TABLES WHERE SCHEMA_NAME ='TDES'";
        protected virtual IEnumerable<DatabaseTable> GetTables(
            DbConnection connection,
            Func<string, string, bool> filter,
            string defaultCharSet,
            string defaultCollation)
        {
            using (var command = connection.CreateCommand())
            {
                var tables = new List<DatabaseTable>();
                command.CommandText = GetTablesQuery;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetValueOrDefault<string>("TABLE_NAME");
                        var type = reader.GetValueOrDefault<string>("TABLE_TYPE");
                        var schema = reader.GetValueOrDefault<string>("SCHEMA_NAME");

                        var table = new DatabaseTable();

                        table.Schema = schema;
                        table.Name = name;

                        var isValidByFilter = filter?.Invoke(table.Schema, table.Name) ?? true;
                        var isValidBySettings = !(table is DatabaseView) || Settings.Views;

                        if (isValidByFilter &&
                            isValidBySettings)
                        {
                            tables.Add(table);
                        }
                    }
                }

                // This is done separately due to MARS property may be turned off
                GetColumns(connection, tables, filter, defaultCharSet, defaultCollation);
                GetPrimaryKeys(connection, tables);
                GetIndexes(connection, tables, filter);
                GetConstraints(connection, tables);

                return tables;
            }
        }

        private const string GetColumnsQuery = @"SELECT TABLE_NAME,SCHEMA_NAME,COLUMN_NAME,DATA_TYPE_NAME,CASE WHEN IS_NULLABLE = 'TRUE' THEN TRUE ELSE FALSE END  AS IS_NULLABLE, IFNULL(GENERATION_TYPE,DEFAULT_VALUE) DEFAULT_VAL,COMMENTS 
                                                FROM SYS.TABLE_COLUMNS  WHERE SCHEMA_NAME ='TDES' AND TABLE_NAME ='{0}' ORDER BY POSITION;";

        protected virtual void GetColumns(
            DbConnection connection,
            IReadOnlyList<DatabaseTable> tables,
            Func<string, string, bool> tableFilter,
            string defaultCharSet,
            string defaultCollation)
        {
            foreach (var table in tables)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(GetColumnsQuery, table.Name);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tableName = reader.GetValueOrDefault<string>("TABLE_NAME");
                            var schemaName = reader.GetValueOrDefault<string>("SCHEMA_NAME");
                            var name = reader.GetValueOrDefault<string>("COLUMN_NAME");
                            var defaultValue = reader.GetValueOrDefault<string>("DEFAULT_VAL");
                            var nullable = reader.GetValueOrDefault<bool>("IS_NULLABLE");
                            var dataType = reader.GetValueOrDefault<string>("DATA_TYPE_NAME");
                            var comment = reader.GetValueOrDefault<string>("COMMENTS");

                            var isDefaultValueSqlFunction = IsDefaultValueSqlFunction(defaultValue, dataType);
                            var isDefaultValueExpression = false;

                            ValueGenerated? valueGenerated;
                            if (!string.IsNullOrWhiteSpace(defaultValue) && defaultValue.ToUpper().Contains("IDENTITY"))
                            {
                                valueGenerated = ValueGenerated.OnAdd;
                            }
                            else
                            {
                                valueGenerated = null;
                            }
                            if(tableName == "REF_DATI1" && name == "UPDATE_BY")
                            {

                            }

                            var column = new DatabaseColumn
                            {
                                Table = table,
                                Name = name,
                                StoreType = dataType,
                                IsNullable = nullable,
                                DefaultValueSql = CreateDefaultValueString(defaultValue, dataType, isDefaultValueSqlFunction, isDefaultValueExpression, nullable),
                                ValueGenerated = valueGenerated,
                                Comment = string.IsNullOrEmpty(comment)
                                    ? null
                                    : comment
                            };

                            table.Columns.Add(column);
                        }
                    }
                }
            }
        }

        private bool IsDefaultValueSqlFunction(string defaultValue, string dataType)
        {
            if (defaultValue == null)
            {
                return false;
            }

            // SapHana uses `CURRENT_TIMESTAMP` (or `CURRENT_TIMESTAMP(6)`),
            // while MariaDB uses `current_timestamp()` (or `current_timestamp(6)`).
            // MariaDB also allows the usage of `curdate()` as a default for datetime or timestamp columns, but this is handled by the next
            // section.
            if ((string.Equals(dataType, "timestamp", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(dataType, "datetime", StringComparison.OrdinalIgnoreCase)) &&
                Regex.IsMatch(defaultValue, @"^CURRENT_TIMESTAMP(?:\(\d*\))?$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            return true;
        }

        protected virtual string CreateDefaultValueString(
            string defaultValue, string dataType, bool isSqlFunction, bool isDefaultValueExpression,bool IsNullable)
        {
            if (defaultValue == null)
            {
                return null;
            }
            if (IsNullable)
                return null;
            // Handle boolean values.
            if (string.Equals(dataType, "boolean", StringComparison.OrdinalIgnoreCase)
                && !IsNullable)
            {
                return null;
            }

            if (isSqlFunction ||
                isDefaultValueExpression)
            {
                return defaultValue;
            }


            return "'" + defaultValue.Replace(@"\", @"\\").Replace("'", "''") + "'";
        }

        private const string GetPrimaryQuery = @"SELECT SCHEMA_NAME,TABLE_NAME,COLUMN_NAME,CONSTRAINT_NAME FROM SYS.CONSTRAINTS 
                                            WHERE SCHEMA_NAME ='TDES'  AND IS_PRIMARY_KEY ='TRUE' AND TABLE_NAME ='{0}';";

        protected virtual void GetPrimaryKeys(
            DbConnection connection,
            IReadOnlyList<DatabaseTable> tables)
        {
            foreach (var table in tables)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(GetPrimaryQuery,table.Name);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                var key = new DatabasePrimaryKey
                                {
                                    Table = table,
                                    Name = reader.GetValueOrDefault<string>("CONSTRAINT_NAME")
                                };
                                var columnPk = reader.GetValueOrDefault<string>("COLUMN_NAME");
                                var pk = table.Columns.Single(y => y.Name == columnPk);
                                if (pk.ValueGenerated == null &&
                                    (pk.DefaultValueSql == null ||
                                     string.Equals(pk.DefaultValueSql, "SYSUUID", StringComparison.OrdinalIgnoreCase)
                                    ))
                                {
                                    pk.ValueGenerated = ValueGenerated.OnAdd;
                                    pk.DefaultValueSql = null;
                                }
                                key.Columns.Add(pk);
                                table.PrimaryKey = key;
                            }
                            catch (Exception ex)
                            {
                                _logger.Logger.LogError(ex, "Error assigning primary key for {table}.", table.Name);
                            }
                        }
                    }
                }
            }
        }

        private const string GetIndexesQuery = @"SELECT SCHEMA_NAME,TABLE_NAME,INDEX_NAME,CONSTRAINT,STRING_AGG(CONCAT(CONCAT(COLUMN_NAME,'='),ASCENDING_ORDER),';') COLUMNS 
                            FROM SYS.INDEX_COLUMNS  WHERE SCHEMA_NAME ='TDES' AND TABLE_NAME ='{0}'
                            GROUP BY SCHEMA_NAME,TABLE_NAME,INDEX_NAME,CONSTRAINT;";

        protected virtual void GetIndexes(
            DbConnection connection,
            IReadOnlyList<DatabaseTable> tables,
            Func<string, string, bool> tableFilter)
        {
            foreach (var table in tables)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(GetIndexesQuery, table.Name);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                var constraintName = reader.GetValueOrDefault<string>("CONSTRAINT");
                                if(!string.IsNullOrWhiteSpace(constraintName) && !constraintName.ToUpper().Contains("PRIMARY"))
                                {
                                    var indexName = reader.GetValueOrDefault<string>("INDEX_NAME");
                                    var columnsRaw = reader.GetValueOrDefault<string>("COLUMNS");
                                    DatabaseIndex index = new DatabaseIndex()
                                    {
                                        IsUnique = constraintName.ToUpper().Contains("UNIQUE"),
                                        Name = indexName,
                                        Table = table
                                    };
                                    var columns = columnsRaw.Split(';');
                                    foreach(var column in columns)
                                    {
                                        var columnName = column.Split('=')[0];
                                        var isAscending = column.Split('=')[1];
                                        index.Columns.Add(FindColumn(table, columnName));
                                    }
                                    table.Indexes.Add(index);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Logger.LogError(ex, "Error assigning index for {table}.", table.Name);
                            }
                        }
                    }
                }
            }
        }
        private const string GetConstraintsQuery = @"select 
SCHEMA_NAME,TABLE_NAME,COLUMN_NAME,CONSTRAINT_NAME,REFERENCED_SCHEMA_NAME,REFERENCED_TABLE_NAME,REFERENCED_COLUMN_NAME,DELETE_RULE 
from SYS.REFERENTIAL_CONSTRAINTS  WHERE SCHEMA_NAME ='TDES' AND TABLE_NAME ='{0}';";

        protected virtual void GetConstraints(
            DbConnection connection,
            IReadOnlyList<DatabaseTable> tables)
        {
            foreach (var table in tables)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(GetConstraintsQuery, table.Name);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var referenceSchema = reader.GetValueOrDefault<string>("REFERENCED_SCHEMA_NAME");
                            var referenceTable = reader.GetValueOrDefault<string>("REFERENCED_TABLE_NAME");
                            var referencePrincipal = tables.FirstOrDefault(t => string.Equals(t.Schema, referenceSchema, StringComparison.OrdinalIgnoreCase) 
                                                                                && string.Equals(t.Name, referenceTable, StringComparison.OrdinalIgnoreCase));
                            if(referencePrincipal!=null)
                            {
                                var columns = reader.GetValueOrDefault<string>("COLUMN_NAME");
                                var PrincipalColumns = reader.GetValueOrDefault<string>("REFERENCED_COLUMN_NAME");

                                var foreignKey = new DatabaseForeignKey { 
                                    Name = reader.GetValueOrDefault<string>("CONSTRAINT_NAME"), 
                                    OnDelete = ConvertToReferentialAction(reader.GetValueOrDefault<string>("DELETE_RULE")),
                                    Table = table,
                                    PrincipalTable = referencePrincipal
                                };
                                var fk_column = table.Columns.FirstOrDefault(d => string.Equals(d.Name, columns, StringComparison.OrdinalIgnoreCase));
                                var ref_fk_column = referencePrincipal.Columns.FirstOrDefault(d => string.Equals(d.Name, PrincipalColumns, StringComparison.OrdinalIgnoreCase));
                                if(fk_column!=null && ref_fk_column != null)
                                {
                                    foreignKey.Columns.Add(fk_column);
                                    foreignKey.PrincipalColumns.Add(ref_fk_column);
                                    table.ForeignKeys.Add(foreignKey);
                                }
                            }
                        }
                    }
                }
            }
        }


        protected virtual ReferentialAction? ConvertToReferentialAction(string onDeleteAction)
            => onDeleteAction.ToUpperInvariant() switch
            {
                "NO ACTION" => ReferentialAction.NoAction,
                "RESTRICT" => ReferentialAction.NoAction,
                "CASCADE" => ReferentialAction.Cascade,
                "SET NULL" => ReferentialAction.SetNull,
                _ => null
            };

        private DatabaseColumn GetColumn(DatabaseTable table, string columnName)
            => FindColumn(table, columnName) ??
               throw new InvalidOperationException($"Could not find column '{columnName}' in table '{table.Name}'.");

        private DatabaseColumn FindColumn(DatabaseTable table, string columnName)
            => table.Columns.SingleOrDefault(c => string.Equals(c.Name, columnName, StringComparison.Ordinal)) ??
               table.Columns.SingleOrDefault(c => string.Equals(c.Name, columnName, StringComparison.OrdinalIgnoreCase));
    }
}
