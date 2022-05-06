using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Sap.Data.Hana;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal
{
    public class SapHanaRelationalConnection : RelationalConnection, ISapHanaRelationalConnection
    {
        private const string NoBackslashEscapes = "NO_BACKSLASH_ESCAPES";

        private readonly SapHanaOptionsExtension _SapHanaOptionsExtension;

        // ReSharper disable once VirtualMemberCallInConstructor
        public SapHanaRelationalConnection(
            [NotNull] RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
            _SapHanaOptionsExtension = Dependencies.ContextOptions.FindExtension<SapHanaOptionsExtension>() ?? new SapHanaOptionsExtension();
        }

        private bool IsMasterConnection { get; set; }

        protected override DbConnection CreateDbConnection()
            => new HanaConnection(AddConnectionStringOptions(new HanaConnectionStringBuilder(ConnectionString)).ConnectionString);

        public virtual ISapHanaRelationalConnection CreateMasterConnection()
        {
            var relationalOptions = RelationalOptionsExtension.Extract(Dependencies.ContextOptions);
            var connection = (HanaConnection)relationalOptions.Connection;
            var connectionString = connection?.ConnectionString ?? relationalOptions.ConnectionString;

            // Add master connection specific options.
            var csb = new HanaConnectionStringBuilder(connectionString)
            {
                Database = string.Empty,
                Pooling = false
            };

            csb = AddConnectionStringOptions(csb);

            if (connection is null)
                relationalOptions = relationalOptions.WithConnectionString(csb.ConnectionString);

            var optionsBuilder = new DbContextOptionsBuilder();
            var optionsBuilderInfrastructure = (IDbContextOptionsBuilderInfrastructure)optionsBuilder;

            optionsBuilderInfrastructure.AddOrUpdateExtension(relationalOptions);

            return new SapHanaRelationalConnection(Dependencies with { ContextOptions = optionsBuilder.Options })
            {
                IsMasterConnection = true
            };
        }

        private HanaConnectionStringBuilder AddConnectionStringOptions(HanaConnectionStringBuilder builder)
        {
            if (CommandTimeout != null)
            {
                builder.ConnectionTimeout = CommandTimeout.Value;
            }
            var boolHandling = _SapHanaOptionsExtension.DefaultDataTypeMappings?.ClrBoolean;
            switch (boolHandling)
            {
                case null:
                    // Just keep using whatever is already defined in the connection string.
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return builder;
        }

        protected override bool SupportsAmbientTransactions => true;

        // CHECK: Is this obsolete or has it been moved somewhere else?
        // public override bool IsMultipleActiveResultSetsEnabled => false;

        public override void EnlistTransaction(Transaction transaction)
        {
            try
            {
                base.EnlistTransaction(transaction);
            }
            catch (HanaException e)
            {
                if (e.Message == "Already enlisted in a Transaction.")
                {
                    // Return expected exception type.
                    throw new InvalidOperationException(e.Message, e);
                }

                throw;
            }
        }

        public override bool Open(bool errorsExpected = false)
        {
            var result = base.Open(errorsExpected);

            if (result)
            {
                if (_SapHanaOptionsExtension.UpdateSqlModeOnOpen && _SapHanaOptionsExtension.NoBackslashEscapes)
                {
                    AddSqlMode(NoBackslashEscapes);
                }
            }

            return result;
        }

        public override async Task<bool> OpenAsync(CancellationToken cancellationToken, bool errorsExpected = false)
        {
            var result = await base.OpenAsync(cancellationToken, errorsExpected)
                .ConfigureAwait(false);

            if (result)
            {
                if (_SapHanaOptionsExtension.UpdateSqlModeOnOpen && _SapHanaOptionsExtension.NoBackslashEscapes)
                {
                    await AddSqlModeAsync(NoBackslashEscapes)
                        .ConfigureAwait(false);
                }
            }

            return result;
        }

        public virtual void AddSqlMode(string mode)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlInterpolated($@"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', {mode});");

        public virtual Task AddSqlModeAsync(string mode, CancellationToken cancellationToken = default)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlInterpolatedAsync($@"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', {mode});", cancellationToken);

        public virtual void RemoveSqlMode(string mode)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlInterpolated($@"SET SESSION sql_mode = REPLACE(@@sql_mode, {mode}, '');");

        public virtual void RemoveSqlModeAsync(string mode, CancellationToken cancellationToken = default)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlInterpolatedAsync($@"SET SESSION sql_mode = REPLACE(@@sql_mode, {mode}, '');", cancellationToken);
    }
}
