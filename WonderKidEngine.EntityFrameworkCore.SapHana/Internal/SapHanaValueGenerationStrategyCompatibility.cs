using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using WonderKidEngine.EntityFrameworkCore.SapHana.Metadata.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Internal
{
    public static class SapHanaValueGenerationStrategyCompatibility
    {
        public static SapHanaValueGenerationStrategy? GetValueGenerationStrategy(IAnnotation[] annotations)
        {
            var valueGenerationStrategy = ObjectToEnumConverter.GetEnumValue<SapHanaValueGenerationStrategy>(
                annotations.FirstOrDefault(a => a.Name == SapHanaAnnotationNames.ValueGenerationStrategy)?.Value);

            if (!valueGenerationStrategy.HasValue ||
                valueGenerationStrategy == SapHanaValueGenerationStrategy.None)
            {
                var generatedOnAddAnnotation = annotations.FirstOrDefault(a => a.Name == SapHanaAnnotationNames.LegacyValueGeneratedOnAdd)?.Value;
                if (generatedOnAddAnnotation != null && (bool)generatedOnAddAnnotation)
                {
                    valueGenerationStrategy = SapHanaValueGenerationStrategy.IdentityColumn;
                }

                var generatedOnAddOrUpdateAnnotation = annotations.FirstOrDefault(a => a.Name == SapHanaAnnotationNames.LegacyValueGeneratedOnAddOrUpdate)?.Value;
                if (generatedOnAddOrUpdateAnnotation != null && (bool)generatedOnAddOrUpdateAnnotation)
                {
                    valueGenerationStrategy = SapHanaValueGenerationStrategy.ComputedColumn;
                }
            }

            return valueGenerationStrategy;
        }
    }
}
