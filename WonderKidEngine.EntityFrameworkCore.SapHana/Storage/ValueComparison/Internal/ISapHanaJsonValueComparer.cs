using Microsoft.EntityFrameworkCore.ChangeTracking;
using WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Storage.ValueComparison.Internal
{
    public interface ISapHanaJsonValueComparer
    {
        ValueComparer Clone(SapHanaJsonChangeTrackingOptions options);
    }
}
