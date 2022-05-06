using System;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sap.Data.Hana;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal
{
    // TODO: Use as base class for all type mappings.
    /// <summary>
    /// The base class for mapping SapHana-specific types. It configures parameters with the
    /// <see cref="SapHanaDbType"/> provider-specific type enum.
    /// </summary>
    public abstract class SapHanaTypeMapping : RelationalTypeMapping
    {
        /// <summary>
        /// The database type used by SapHana.
        /// </summary>
        public virtual HanaDbType SapHanaDbType { get; }

        // ReSharper disable once PublicConstructorInAbstractClass
        public SapHanaTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            HanaDbType SapHanaDbType,
            DbType? dbType = null,
            bool unicode = false,
            int? size = null,
            ValueConverter valueConverter = null,
            ValueComparer valueComparer = null)
            : base(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(clrType, valueConverter, valueComparer), storeType, StoreTypePostfix.None, dbType, unicode, size))
            => SapHanaDbType = SapHanaDbType;

        /// <summary>
        /// Constructs an instance of the <see cref="SapHanaTypeMapping"/> class.
        /// </summary>
        /// <param name="parameters">The parameters for this mapping.</param>
        /// <param name="SapHanaDbType">The database type of the range subtype.</param>
        protected SapHanaTypeMapping(RelationalTypeMappingParameters parameters, HanaDbType SapHanaDbType)
            : base(parameters)
            => SapHanaDbType = SapHanaDbType;

        protected override void ConfigureParameter(DbParameter parameter)
        {
            if (!(parameter is HanaParameter SapHanaParameter))
            {
                throw new ArgumentException($"SapHana-specific type mapping {GetType()} being used with non-SapHana parameter type {parameter.GetType().Name}");
            }

            base.ConfigureParameter(parameter);

            SapHanaParameter.HanaDbType = SapHanaDbType;
        }
    }
}
