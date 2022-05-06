using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures the column's character set for a property or field based on the applied <see cref="SapHanaCharSetAttribute" />.
    /// </summary>
    public class ColumnCharSetAttributeConvention : PropertyAttributeConventionBase<SapHanaCharSetAttribute>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ColumnCharSetAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public ColumnCharSetAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override void ProcessPropertyAdded(
            IConventionPropertyBuilder propertyBuilder,
            SapHanaCharSetAttribute attribute,
            MemberInfo clrMember,
            IConventionContext context)
            => propertyBuilder.HasCharSet(attribute.CharSetName);
    }
}
