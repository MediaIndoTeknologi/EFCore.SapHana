using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures the column's collation for a property or field based on the applied <see cref="SapHanaCollationAttribute" />.
    /// </summary>
    public class ColumnCollationAttributeConvention : PropertyAttributeConventionBase<SapHanaCollationAttribute>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ColumnCollationAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public ColumnCollationAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override void ProcessPropertyAdded(
            IConventionPropertyBuilder propertyBuilder,
            SapHanaCollationAttribute attribute,
            MemberInfo clrMember,
            IConventionContext context)
            => propertyBuilder.UseCollation(attribute.CollationName);
    }
}
