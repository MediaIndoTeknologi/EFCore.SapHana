using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures the character set for an entity based on the applied <see cref="SapHanaCharSetAttribute" />.
    /// </summary>
    public class TableCharSetAttributeConvention : EntityTypeAttributeConventionBase<SapHanaCharSetAttribute>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="TableCharSetAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public TableCharSetAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override void ProcessEntityTypeAdded(
            IConventionEntityTypeBuilder entityTypeBuilder,
            SapHanaCharSetAttribute attribute,
            IConventionContext<IConventionEntityTypeBuilder> context)
            => entityTypeBuilder.HasCharSet(attribute.CharSetName, attribute.DelegationModes);
    }
}
