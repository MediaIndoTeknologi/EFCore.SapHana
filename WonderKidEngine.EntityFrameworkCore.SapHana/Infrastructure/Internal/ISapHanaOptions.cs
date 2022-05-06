using Microsoft.EntityFrameworkCore;
using WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal
{
    public interface ISapHanaOptions : ISingletonOptions
    {
        SapHanaConnectionSettings ConnectionSettings { get; }
        CharSet DefaultCharSet { get; }
        CharSet NationalCharSet { get; }
        string DefaultGuidCollation { get; }
        bool NoBackslashEscapes { get; }
        bool ReplaceLineBreaksWithCharFunction { get; }
        SapHanaDefaultDataTypeMappings DefaultDataTypeMappings { get; }
        SapHanaSchemaNameTranslator SchemaNameTranslator { get; }
        bool IndexOptimizedBooleanColumns { get; }
        SapHanaJsonChangeTrackingOptions JsonChangeTrackingOptions { get; }
        bool LimitKeyedOrIndexedStringColumnLength { get; }
        bool StringComparisonTranslations { get; }
    }
}
