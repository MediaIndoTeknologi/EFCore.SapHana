using Microsoft.EntityFrameworkCore.Storage;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal
{
    public interface IDefaultValueCompatibilityAware
    {
        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="isDefaultValueCompatible"> Use a default value compatible syntax, or not. </param>
        /// <returns> The newly created mapping. </returns>
        RelationalTypeMapping Clone(bool isDefaultValueCompatible = false);
    }
}
