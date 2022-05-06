using System;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Sets the collation of a type (table), property or field (column) for SapHana.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class SapHanaCollationAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SapHanaCollationAttribute" /> class.
        ///     Implicitly uses <see cref="Microsoft.EntityFrameworkCore.DelegationModes.ApplyToAll"/>.
        /// </summary>
        /// <param name="collation"> The name of the collation to use. </param>
        public SapHanaCollationAttribute(string collation)
            : this(collation, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SapHanaCollationAttribute" /> class.
        /// </summary>
        /// <param name="collation"> The name of the collation to use. </param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the collation and where not.
        /// Ignored when <see cref="SapHanaCollationAttribute"/> is applied to properties/columns.
        /// </param>
        public SapHanaCollationAttribute(string collation, DelegationModes delegationModes)
            : this(collation, (DelegationModes?)delegationModes)
        {
        }

        protected SapHanaCollationAttribute(string collation, DelegationModes? delegationModes)
        {
            CollationName = collation;
            DelegationModes = delegationModes;
        }

        /// <summary>
        ///     The name of the collation to use.
        /// </summary>
        public virtual string CollationName { get; }

        /// <summary>
        /// Finely controls where to recursively apply the collation and where not.
        /// Implicitly uses <see cref="Microsoft.EntityFrameworkCore.DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// Ignored when <see cref="SapHanaCollationAttribute"/> is applied to properties/columns.
        /// </summary>
        public virtual DelegationModes? DelegationModes { get; }
    }
}
