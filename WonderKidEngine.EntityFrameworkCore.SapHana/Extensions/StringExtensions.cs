namespace WonderKidEngine.EntityFrameworkCore.SapHana.Extensions
{
    internal static class StringExtensions
    {
        internal static string NullIfEmpty(this string value)
            => value?.Length > 0
                ? value
                : null;
    }
}
