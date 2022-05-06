using System.Collections;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal
{
    public class ByteArrayComparer : ValueComparer<byte[]>
    {
        public ByteArrayComparer()
            : base(
                (v1, v2) => StructuralComparisons.StructuralEqualityComparer.Equals(v1, v2),
                v => StructuralComparisons.StructuralEqualityComparer.GetHashCode(v),
                v => v == null ? null : v.ToArray())
        {
        }
    }
}
