using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures the collation for an entity based on the applied <see cref="SapHanaCollationAttribute" />.
    /// </summary>
    public class TableCollationAttributeConvention : EntityTypeAttributeConventionBase<SapHanaCollationAttribute>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="TableCollationAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public TableCollationAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override void ProcessEntityTypeAdded(
            IConventionEntityTypeBuilder entityTypeBuilder,
            SapHanaCollationAttribute attribute,
            IConventionContext<IConventionEntityTypeBuilder> context)
            => entityTypeBuilder.UseCollation(attribute.CollationName, attribute.DelegationModes);
    }
}
