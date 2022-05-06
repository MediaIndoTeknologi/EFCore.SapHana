using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal
{
    public abstract class SapHanaJsonTypeMappingSourcePlugin
        : IRelationalTypeMappingSourcePlugin
    {
        [NotNull]
        public virtual ISapHanaOptions Options { get; }

        protected SapHanaJsonTypeMappingSourcePlugin(
            [NotNull] ISapHanaOptions options)
        {
            Options = options;
        }

        public virtual RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName;

            if (clrType == typeof(SapHanaJsonString))
            {
                clrType = typeof(string);
                storeTypeName = "json";
            }

            if (storeTypeName != null)
            {
                clrType ??= typeof(string);
                return storeTypeName.Equals("json", StringComparison.OrdinalIgnoreCase)
                    ? (RelationalTypeMapping)Activator.CreateInstance(
                        SapHanaJsonTypeMappingType.MakeGenericType(clrType),
                        storeTypeName,
                        GetValueConverter(clrType),
                        GetValueComparer(clrType),
                        Options)
                    : null;
            }

            return FindDomMapping(mappingInfo);
        }

        protected abstract Type SapHanaJsonTypeMappingType { get; }
        protected abstract RelationalTypeMapping FindDomMapping(RelationalTypeMappingInfo mappingInfo);
        protected abstract ValueConverter GetValueConverter(Type clrType);
        protected abstract ValueComparer GetValueComparer(Type clrType);
    }
}
