using System;
using JetBrains.Annotations;
using Sap.Data.Hana;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal
{
    /// <summary>
    ///     Detects the exceptions caused by SapHana transient failures.
    /// </summary>
    public static class SapHanaTransientExceptionDetector
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static bool ShouldRetryOn([NotNull] Exception ex)
            => ex is HanaException SapHanaException
                ? SapHanaException.IsTransient
                : ex is TimeoutException;
    }
}
