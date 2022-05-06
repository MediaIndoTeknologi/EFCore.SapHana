using System;
using WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class SapHanaCommonJsonChangeTrackingOptionsExtensions
    {
        public static SapHanaJsonChangeTrackingOptions ToJsonChangeTrackingOptions(this SapHanaCommonJsonChangeTrackingOptions options)
            => options switch
            {
                SapHanaCommonJsonChangeTrackingOptions.RootPropertyOnly => SapHanaJsonChangeTrackingOptions.CompareRootPropertyOnly,
                SapHanaCommonJsonChangeTrackingOptions.FullHierarchyOptimizedFast => SapHanaJsonChangeTrackingOptions.CompareStringRootPropertyByEquals |
                                                                                   SapHanaJsonChangeTrackingOptions.CompareDomRootPropertyByEquals |
                                                                                   SapHanaJsonChangeTrackingOptions.SnapshotCallsDeepClone |
                                                                                   SapHanaJsonChangeTrackingOptions.SnapshotCallsClone,
                SapHanaCommonJsonChangeTrackingOptions.FullHierarchyOptimizedSemantically => SapHanaJsonChangeTrackingOptions.CompareStringRootPropertyByEquals |
                                                                                           SapHanaJsonChangeTrackingOptions.CompareDomSemantically |
                                                                                           SapHanaJsonChangeTrackingOptions.HashDomSemantiallyOptimized |
                                                                                           SapHanaJsonChangeTrackingOptions.SnapshotCallsDeepClone |
                                                                                           SapHanaJsonChangeTrackingOptions.SnapshotCallsClone,
                SapHanaCommonJsonChangeTrackingOptions.FullHierarchySemantically => SapHanaJsonChangeTrackingOptions.None,
                _ => throw new ArgumentOutOfRangeException(nameof(options)),
            };
    }
}
